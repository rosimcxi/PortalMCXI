#!/usr/bin/env python3
"""Spustí pouze izolované lokální API; nikdy se nepřipojuje k produkci ani DB."""
import argparse
import concurrent.futures
import datetime as dt
import json
import os
from pathlib import Path
import socket
import subprocess
import time
import urllib.error
import urllib.request


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--output-dir', default='artifacts/baseline')
    parser.add_argument('--source-sha', default=os.environ.get('GITHUB_SHA', 'LOCAL_WORKING_TREE'))
    args = parser.parse_args()
    root = Path(__file__).resolve().parents[1]
    project = root / 'backend/PortalMCXIBackend'
    dll = project / 'bin/Release/net10.0/PortalMCXIBackend.dll'
    out = Path(args.output_dir).resolve()
    out.mkdir(parents=True, exist_ok=True)
    rows = []
    run_id = 'PORTAL-BASELINE-' + dt.datetime.now(dt.timezone.utc).strftime('%Y%m%dT%H%M%SZ')

    def check(name, condition):
        rows.append({'check': name, 'status': 'OK' if condition else 'BAD'})
        if not condition:
            raise AssertionError(name)

    for environment in ('Development', 'Production'):
        with socket.socket() as sock:
            sock.bind(('127.0.0.1', 0))
            port = sock.getsockname()[1]
        base = f'http://127.0.0.1:{port}'
        env = dict(os.environ, ASPNETCORE_ENVIRONMENT=environment, DOTNET_ENVIRONMENT=environment,
                   Portal__EnableDemoModules='true')
        process = None
        with (out / f'{environment}.log').open('w') as log:
            try:
                process = subprocess.Popen([args.dotnet, str(dll), '--urls', base,
                                            '--contentRoot', str(project)], env=env, stdout=log, stderr=log)

                def request(path, method='GET', body=None):
                    data = json.dumps(body).encode() if body is not None else None
                    req = urllib.request.Request(base + path, data=data, method=method,
                                                 headers={'Content-Type': 'application/json'})
                    try:
                        response = urllib.request.urlopen(req, timeout=8)
                    except urllib.error.HTTPError as error:
                        response = error
                    with response:
                        raw = response.read().decode()
                        try:
                            value = json.loads(raw)
                        except json.JSONDecodeError:
                            value = raw
                        return response.status, value

                for _ in range(100):
                    if process.poll() is not None:
                        raise RuntimeError('API skončilo před health kontrolou')
                    try:
                        if request('/api/health') == (200, {'status': 'OK'}):
                            break
                    except (OSError, urllib.error.URLError):
                        pass
                    time.sleep(0.1)
                check(f'{environment}: health', request('/api/health') == (200, {'status': 'OK'}))
                status, api = request('/openapi/v1.json')
                check(f'{environment}: OpenAPI', status == 200 and '/api/health' in api['paths'])
                check(f'{environment}: projekty', request('/api/projects')[0] == 200)
                status, stats = request('/api/stats')
                check(f'{environment}: ukázkové stats označeny', status == 200 and stats['isDemo'] is True)
                check(f'{environment}: neplatné datum', request('/api/esoterika/numerologie?birthDateStr=invalid')[0] == 400)
                check(f'{environment}: datum v budoucnosti', request('/api/esoterika/kondiciogram?birthDateStr=2999-01-01')[0] == 400)
                status, numerology = request('/api/esoterika/numerologie?birthDateStr=2000-01-01')
                check(f'{environment}: numerologie', status == 200 and numerology['lifeNumber'] == 4)
                status, rhythm = request('/api/esoterika/kondiciogram?birthDateStr=2000-01-01')
                check(f'{environment}: kondiciogram', status == 200 and all(-100 <= v <= 100 for v in rhythm.values()))
                check(f'{environment}: Info a tatvy routy', '/api/info/morning' in api['paths'] and '/api/esoterika/tatvy' in api['paths'])

                if environment == 'Production':
                    for route in ('/api/casna/dashboard', '/api/monitoring/dashboard', '/api/todos/',
                                  '/api/snippets/', '/api/projects-management/', '/api/users/'):
                        check(f'Production: demo nezveřejněno {route}', request(route)[0] == 404)
                    check('Production: login nezveřejněn', request('/api/users/login', 'POST', {})[0] == 404)
                    continue

                for route in ('/api/monitoring/dashboard', '/api/todos/', '/api/snippets/',
                              '/api/projects-management/', '/api/users/'):
                    check(f'Development: modul {route}', request(route)[0] == 200)
                check('Development: login nevydává falešný token', request('/api/users/login', 'POST', {})[0] == 501)
                check('Development: neznámý úkol', request('/api/casna/start', 'POST', {'taskId': 999})[0] == 404)
                with concurrent.futures.ThreadPoolExecutor(max_workers=8) as pool:
                    starts = list(pool.map(lambda _: request('/api/casna/start', 'POST', {'taskId': 1})[0], range(8)))
                check('Development: atomický start', starts.count(200) == 1 and starts.count(400) == 7)
                status, dashboard = request('/api/casna/dashboard')
                check('Development: DTO aktivního úkolu', status == 200 and dashboard['activeTask']['taskId'] == 1
                      and dashboard['activeTask']['projectId'] == 1 and len(dashboard['projects']) == 2)
                check('Development: stop', request('/api/casna/stop', 'POST', {'note': 'smoke'})[0] == 200)
                check('Development: opakovaný stop', request('/api/casna/stop', 'POST', {'note': 'duplicate'})[0] == 400)
                _, dashboard = request('/api/casna/dashboard')
                check('Development: historie', dashboard['activeTask'] is None and len(dashboard['history']) == 1
                      and dashboard['history'][0]['note'] == 'smoke' and dashboard['history'][0]['projectId'] == 1)
                check('Development: prázdný název úkolu', request('/api/todos/', 'POST', {'title': '', 'category': 'test'})[0] == 400)
                status, todo = request('/api/todos/', 'POST', {'title': 'Smoke test', 'category': 'Test'})
                check('Development: vytvoření úkolu', status == 201 and todo['isCompleted'] is False)
                status, changed = request(f"/api/todos/{todo['id']}/toggle", 'PUT')
                check('Development: změna úkolu', status == 200 and changed['isCompleted'] is True)
                check('Development: neznámý úkol toggle', request('/api/todos/999999/toggle', 'PUT')[0] == 404)
            except Exception as error:
                rows.append({'check': environment + ': dokončení', 'status': 'BAD', 'error': str(error)})
            finally:
                if process is not None and process.poll() is None:
                    process.terminate()
                    try:
                        process.wait(timeout=10)
                    except subprocess.TimeoutExpired:
                        process.kill()
                        process.wait()

    result = {'run_id': run_id, 'source_sha': args.source_sha, 'node': socket.gethostname(),
              'status': 'OK' if rows and all(r['status'] == 'OK' for r in rows) else 'BAD',
              'checks': rows, 'tested_target': False, 'rfu_dispatch': False,
              'shared_result_bus_status': 'PENDING', 'cloud_mirror_status': 'PENDING'}
    (out / 'AI_INFO.json').write_text(json.dumps(result, ensure_ascii=False, indent=2) + '\n')
    report = f"# PortalMCXI baseline: {result['status']}\n\nRUN_ID={run_id}\nSOURCE_SHA={args.source_sha}\n\n"
    report += '| Kontrola | Stav |\n|---|---|\n' + '\n'.join(f"| {r['check']} | {r['status']} |" for r in rows)
    report += '\n\nTESTED_TARGET=NO; RFU_DISPATCH=NO; SHARED_RESULT_BUS=PENDING; CLOUD_MIRROR=PENDING\n'
    (out / 'RESULT.md').write_text(report, encoding='utf-8')
    print(report)
    return 0 if result['status'] == 'OK' else 1


if __name__ == '__main__':
    raise SystemExit(main())
