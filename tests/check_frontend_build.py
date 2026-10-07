"""Kontrola výstupu buildu: offline CSS a žádný Vite development vstup/CDN."""
from pathlib import Path
import re

dist = Path(__file__).resolve().parents[1] / 'frontend/dist'
html = (dist / 'index.html').read_text()
assert 'cdn.tailwindcss.com' not in html, 'Produkce nesmí záviset na Tailwind CDN'
assert '/src/main.jsx' not in html, 'HTML musí odkazovat na sestavené assety'
css = list(dist.glob('assets/*.css'))
assert css and any('.min-h-screen' in f.read_text() for f in css), 'Chybí vygenerované Tailwind styly'
for asset in re.findall(r'(?:href|src)="(/assets/[^\"]+)"', html):
    assert (dist / asset.lstrip('/')).is_file(), f'Chybí asset {asset}'
print('FRONTEND_BUILD_ASSETS=OK')
