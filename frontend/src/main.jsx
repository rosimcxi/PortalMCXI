// =====================================================================
// SOUBOR:   main.jsx
// PROJEKT:  PortalMCXI
// VERZE:    v79.0
// ZMĚNA:    2026-05-17
// AUTOR:    Ing. Roman Fišer
// POPIS:    Dynamické stahování tlačítek (odkazů) z .NET API
// =====================================================================

import React, { useState, useEffect } from 'react';
import ReactDOM from 'react-dom/client';
import { 
  LayoutDashboard, Globe, Database, Server, Activity, 
  Users, Mail, Cpu, Check, RefreshCw, 
  AlertCircle, Terminal, ChevronLeft, Download, Box, 
  Search, AlertTriangle, User, ExternalLink,
  Clock, Sparkles, Heart, Monitor, ListChecks,
  ChevronRight, BarChart3, ShieldCheck, FileCode2, Network
} from 'lucide-react';

// Záchranná data, pokud by C# API zrovna nebylo dostupné
const FALLBACK_PROJECTS = [
  { id: 'roman', name: 'Osobní web Roman', url: 'https://roman.rosimcxi.eu', icon: User, color: 'text-indigo-400', bg: 'bg-indigo-50', desc: 'Profesní portfolio' },
  { id: 'joga', name: 'Jóga s Miškou', url: 'https://jogasmiskou.cz', icon: Heart, color: 'text-rose-500', bg: 'bg-rose-50', desc: 'Rezervační systém' },
  { id: 'casna', name: 'CasNa (Tracker)', url: 'https://api.rosimcxi.eu/scalar/v1', icon: Clock, color: 'text-amber-500', bg: 'bg-amber-50', desc: 'Sledování času' },
  { id: 'tatvy', name: 'Esoterika & Tatvy', url: 'https://wisdomes.eu', icon: Sparkles, color: 'text-purple-500', bg: 'bg-purple-50', desc: 'Výpočty tater' }
];

const FILE_VERSIONS = [
  { file: 'main.jsx', version: 'v79.0', desc: 'Oprava syntaxe a renderování' },
  { file: 'azure-pipelines.yml', version: 'v85.0', desc: 'Ochrana JSON parsování' },
  { file: 'docker-compose.yml', version: 'v3.30', desc: 'Zahrnutí vizitky' },
  { file: 'backend/Program.cs', version: 'v10.9', desc: 'API Endpointy' }
];

const LogView = ({ onBack }) => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('overview');

  const fetchLogs = async () => {
    setLoading(true);
    try {
      const res = await fetch('/build_info.json?t=' + Date.now());
      if (res.ok) setData(await res.json());
    } catch (e) { 
      console.error("Diagnostika nedostupná."); 
    } finally { 
      setLoading(false); 
    }
  };

  useEffect(() => { fetchLogs(); }, []);

  const TabBtn = ({ id, label, icon: Icon }) => (
    <button 
      onClick={() => setActiveTab(id)}
      className={`flex items-center gap-2 px-4 py-2 rounded-xl text-[10px] font-black uppercase tracking-wider transition-all ${activeTab === id ? 'bg-indigo-600 text-white shadow-lg' : 'text-slate-500 hover:text-slate-300'}`}
    >
      <Icon size={14} /> {label}
    </button>
  );

  const isApiHealthy = data?.apiError?.includes('Application started');

  return (
    <div className="min-h-screen bg-[#020617] text-slate-300 font-mono p-4 md:p-8">
      <div className="max-w-7xl mx-auto">
        <div className="flex flex-col lg:flex-row justify-between items-start lg:items-center mb-10 gap-6">
          <button onClick={onBack} className="flex items-center gap-2 text-indigo-400 hover:text-indigo-300 font-bold transition-all group">
            <ChevronLeft size={20} className="group-hover:-translate-x-1 transition-transform" /> Zpět na Hub
          </button>
          <div className="flex flex-wrap bg-slate-900/80 p-1.5 rounded-2xl border border-slate-800 backdrop-blur-xl">
            <TabBtn id="overview" label="Systém" icon={Activity} />
            <TabBtn id="processes" label="Procesy" icon={ListChecks} />
            <TabBtn id="docker" label="Docker" icon={BarChart3} />
            <TabBtn id="nginx" label="Nginx & Proxy" icon={Network} />
            <TabBtn id="logs" label="API Konsole" icon={Terminal} />
          </div>
        </div>

        <div className="flex items-center justify-between mb-8 border-b border-slate-800 pb-8">
          <div>
            <h1 className="text-3xl font-black text-white flex items-center gap-3 italic">
              <Terminal className="text-indigo-500" /> Black Box Console
            </h1>
            <p className="text-[10px] text-slate-500 mt-2 uppercase tracking-[0.4em]">VPS Contabo Live Monitoring</p>
          </div>
          <div className="flex gap-3">
            <button onClick={fetchLogs} className="p-4 bg-indigo-600 rounded-2xl text-white hover:bg-indigo-500 transition-all shadow-xl shadow-indigo-500/20">
              <RefreshCw size={20} className={loading ? 'animate-spin' : ''} />
            </button>
          </div>
        </div>

        {activeTab === 'overview' && (
          <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 font-sans animate-in fade-in duration-500">
            <div className="lg:col-span-4 space-y-6">
              <div className="bg-slate-900/40 p-8 rounded-3xl border border-slate-800 shadow-2xl relative overflow-hidden">
                <p className="text-[10px] text-indigo-400 font-black uppercase mb-6 tracking-widest text-center">Systémové zdroje</p>
                <div className="space-y-4">
                  <div className="flex justify-between border-b border-slate-800 pb-2"><span className="text-xs text-slate-500 font-bold">Disk:</span><span className="text-sm font-black text-emerald-500">{data?.disk || "N/A"}</span></div>
                  <div className="flex justify-between border-b border-slate-800 pb-2"><span className="text-xs text-slate-500 font-bold">RAM:</span><span className="text-sm font-black text-amber-500">{data?.ram || "N/A"}</span></div>
                  <div className="pt-4 text-[9px] text-slate-600 uppercase font-bold text-center">Nasazení: {data?.buildTime}</div>
                </div>
                <Activity size={100} className="absolute -right-10 -bottom-10 text-slate-800/20" />
              </div>

              <div className="bg-slate-900/40 p-8 rounded-3xl border border-slate-800 shadow-2xl">
                <p className="text-[10px] text-indigo-400 font-black uppercase mb-6 tracking-widest text-center flex items-center justify-center gap-2">
                  <FileCode2 size={14} /> Verze nasazených souborů
                </p>
                <div className="space-y-4 text-sm font-mono">
                  {FILE_VERSIONS.map((f, i) => (
                     <div key={i} className="flex justify-between items-center border-b border-slate-800 pb-2">
                       <span className="text-[10px] text-slate-400">{f.file}</span>
                       <span className="text-xs font-black text-indigo-300">{f.version}</span>
                     </div>
                  ))}
                </div>
              </div>
            </div>

            <div className="lg:col-span-8 bg-slate-900/40 p-8 rounded-3xl border border-slate-800 shadow-2xl">
                <p className="text-[10px] text-indigo-400 font-black uppercase mb-8 tracking-widest text-center">Docker Status</p>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {data?.docker?.split('|').filter(x => x).map((s, i) => (
                    <div key={i} className="flex justify-between items-center p-4 bg-black/30 rounded-2xl border border-slate-800/50 hover:border-indigo-500/30 transition-all">
                      <span className="truncate text-xs font-black text-slate-300">{s.split(': ')[0]}</span>
                      <span className={`text-[9px] px-3 py-1 rounded-full font-black uppercase ${s.includes('Up') ? 'bg-emerald-500/10 text-emerald-500' : 'bg-rose-500/10 text-rose-500'}`}>
                        {s.split(': ')[1]?.split('(')[0]}
                      </span>
                    </div>
                  ))}
                </div>
            </div>
          </div>
        )}

        {activeTab === 'processes' && (
          <div className="bg-slate-900/40 rounded-3xl border border-slate-800 p-8 overflow-x-auto">
             <pre className="text-[10px] text-slate-400 font-mono whitespace-pre leading-relaxed">{data?.linuxProcs?.split('|').join('\n')}</pre>
          </div>
        )}

        {activeTab === 'docker' && (
          <div className="bg-slate-900/40 rounded-3xl border border-slate-800 p-8 overflow-x-auto">
             <pre className="text-[11px] text-emerald-500/80 font-mono whitespace-pre leading-loose">{data?.dockerStats?.split('|').join('\n')}</pre>
          </div>
        )}

        {activeTab === 'nginx' && (
          <div className="space-y-6 animate-in fade-in duration-500">
            <div className="bg-slate-900/40 rounded-3xl border border-slate-800 p-8">
              <p className="text-[10px] text-indigo-400 font-black uppercase mb-6 tracking-widest flex items-center gap-2">
                <Globe size={14} /> Služba NPM
              </p>
              <div className="flex items-center gap-4 bg-black/40 p-4 rounded-xl border border-slate-800/50">
                <div className={`w-3 h-3 rounded-full ${data?.nginxStatus?.includes('Up') ? 'bg-emerald-500 animate-pulse' : 'bg-rose-500'}`}></div>
                <span className="text-sm font-mono text-slate-300">{data?.nginxStatus?.replace('|', '') || "Stav neznámý"}</span>
              </div>
            </div>
            
            <div className="bg-slate-900/40 rounded-3xl border border-slate-800 p-8">
              <p className="text-[10px] text-indigo-400 font-black uppercase mb-6 tracking-widest flex items-center gap-2">
                <Check size={14} /> Výsledek testu konfigurace (nginx -t)
              </p>
              <pre className="text-[11px] text-amber-500/90 font-mono whitespace-pre leading-loose bg-black/60 p-5 rounded-xl border border-slate-800/50">
                {data?.nginxTest?.split('|').join('\n') || "Test neproběhl."}
              </pre>
            </div>

            <div className="bg-slate-900/40 rounded-3xl border border-slate-800 p-8">
              <p className="text-[10px] text-indigo-400 font-black uppercase mb-6 tracking-widest flex items-center gap-2">
                <Terminal size={14} /> Poslední Nginx logy
              </p>
              <pre className="text-[10px] text-slate-400 font-mono whitespace-pre leading-relaxed bg-black/60 p-5 rounded-xl border border-slate-800/50 overflow-y-auto max-h-[300px]">
                {data?.nginxLogs?.split('|').join('\n') || "Logy nejsou dostupné."}
              </pre>
            </div>
          </div>
        )}

        {activeTab === 'logs' && (
          <div className={`${isApiHealthy ? 'bg-emerald-950/5 border-emerald-900/20' : 'bg-rose-950/5 border-rose-900/20'} border p-8 rounded-3xl shadow-2xl animate-in fade-in duration-500`}>
            <p className={`text-[10px] ${isApiHealthy ? 'text-emerald-500' : 'text-rose-500'} font-black uppercase mb-8 flex items-center gap-3 tracking-widest italic`}>
              <Terminal size={18} /> API Konsole (portal_api) - {isApiHealthy ? 'ONLINE' : 'ERROR'}
            </p>
            <div className={`bg-black/60 p-6 rounded-2xl font-mono text-[10px] ${isApiHealthy ? 'text-emerald-300/90' : 'text-rose-300'} overflow-y-auto max-h-[600px] whitespace-pre-wrap border ${isApiHealthy ? 'border-emerald-900/10' : 'border-rose-900/10'} leading-relaxed`}>
              {data?.apiError ? data.apiError.split('|').join('\n') : "API Log je momentálně prázdný."}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

const RomanView = ({ projects }) => (
  <div className="min-h-screen bg-slate-50 dark:bg-[#020617] flex flex-col items-center justify-center p-8 font-sans selection:bg-indigo-500/30">
    <div className="max-w-4xl w-full text-center">
      <div className="flex flex-wrap gap-4 justify-center mb-12">
        {projects.map(p => (
          <a key={p.id} href={p.url} target="_blank" rel="noreferrer" className="p-4 bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl shadow-xl hover:-translate-y-1 transition-all">
            <p.icon size={22} className={p.color} />
          </a>
        ))}
      </div>
      <h1 className="text-6xl md:text-8xl font-black text-slate-900 dark:text-white mb-6 italic tracking-tighter">Roman Fišer</h1>
      <p className="text-slate-500 font-black uppercase tracking-[0.5em] text-[10px] mb-16">Ing. & Senior IT Consultant</p>
      <div className="flex flex-col sm:flex-row gap-4 justify-center">
        <button onClick={() => window.location.hash = ''} className="px-12 py-5 bg-indigo-600 text-white rounded-3xl font-black text-sm shadow-2xl hover:bg-indigo-500 flex items-center gap-3 justify-center uppercase tracking-widest">
            <LayoutDashboard size={18}/> Dashboard
        </button>
        <button onClick={() => window.location.hash = 'logs'} className="px-12 py-5 bg-slate-200 dark:bg-slate-800 text-slate-700 dark:text-slate-300 rounded-3xl font-black text-sm hover:opacity-80 flex items-center gap-3 justify-center uppercase tracking-widest border border-transparent">
            <Terminal size={18}/> Diagnostika
        </button>
      </div>
    </div>
  </div>
);

const DashboardView = ({ projects }) => (
  <div className="min-h-screen bg-slate-50 dark:bg-[#010409] flex flex-col lg:flex-row font-sans selection:bg-indigo-500/30">
    <aside className="w-full lg:w-80 bg-white dark:bg-slate-900 border-r border-slate-200 dark:border-slate-800 p-8 flex flex-col shadow-2xl">
      <div className="flex items-center gap-4 mb-12">
        <div className="bg-indigo-600 p-2.5 rounded-2xl text-white shadow-xl shadow-indigo-600/30"><Activity size={24} /></div>
        <span className="text-2xl font-black tracking-tighter text-slate-900 dark:text-white italic">PortalMCXI</span>
      </div>
      <nav className="space-y-2 mb-12">
        <button className="w-full flex items-center gap-4 px-6 py-4 rounded-3xl text-sm font-black bg-indigo-50 dark:bg-indigo-900/20 text-indigo-600 border border-indigo-100 dark:border-indigo-900/30 shadow-sm"><LayoutDashboard size={20}/> Dashboard</button>
        <button onClick={() => window.location.hash = 'logs'} className="w-full flex items-center gap-4 px-6 py-4 rounded-3xl text-sm font-bold text-slate-500 hover:bg-slate-50 dark:hover:bg-slate-800 transition-all group border border-transparent">
          <Terminal size={20} className="text-indigo-500 group-hover:rotate-12 transition-transform"/> Diagnostika
        </button>
      </nav>
      <nav className="space-y-1.5">
        <p className="text-[10px] font-black text-slate-400 uppercase tracking-widest px-6 mb-4">Moje Projekty (z API)</p>
        {projects.map(p => (
          <a key={p.id} href={p.url} target="_blank" rel="noreferrer" className="w-full flex items-center justify-between px-6 py-4 rounded-3xl text-sm font-bold text-slate-600 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-800 transition-all group border border-transparent">
            <div className="flex items-center gap-4">
              <p.icon size={20} className={`${p.color} group-hover:scale-110 transition-transform`} />
              {p.name}
            </div>
            <ChevronRight size={16} className="opacity-20 group-hover:opacity-100 group-hover:translate-x-1 transition-all" />
          </a>
        ))}
      </nav>
      <div className="mt-auto pt-8 border-t border-slate-100 dark:border-slate-800 flex justify-between items-center text-[10px] text-slate-400 font-black uppercase tracking-widest px-4">
         <span>62.84.181.247</span>
         <ShieldCheck size={16} className="text-emerald-500" />
      </div>
    </aside>

    <main className="flex-1 p-8 lg:p-12 overflow-y-auto">
      <header className="flex justify-between items-center mb-12">
        <h2 className="text-4xl font-black text-slate-900 dark:text-white tracking-tight italic">Super Hub</h2>
        <div className="flex items-center gap-4 bg-white dark:bg-slate-900 px-6 py-3 rounded-full border border-slate-100 dark:border-slate-800 shadow-xl shadow-indigo-500/5">
          <span className="text-[10px] text-indigo-500 font-black uppercase tracking-widest">{new Date().toLocaleTimeString()}</span>
          <div className="w-10 h-10 rounded-full bg-indigo-50 dark:bg-indigo-900/30 flex items-center justify-center text-indigo-500 border border-indigo-100 dark:border-indigo-800"><User size={20} /></div>
        </div>
      </header>

      <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-8 mb-16">
        {projects.map(p => (
          <a key={p.id} href={p.url} target="_blank" rel="noreferrer" className="bg-white dark:bg-slate-900 p-8 rounded-[3rem] shadow-xl border border-slate-100 dark:border-slate-800 hover:scale-[1.03] transition-all group relative overflow-hidden">
            <div className={`w-14 h-14 ${p.bg} dark:bg-slate-800/50 rounded-2xl flex items-center justify-center mb-6 group-hover:rotate-6 transition-transform shadow-sm border border-white/10`}>
              <p.icon size={28} className={p.color} />
            </div>
            <h3 className="font-black text-slate-900 dark:text-white mb-2 flex items-center gap-2 text-base tracking-tight">{p.name}</h3>
            <p className="text-[11px] text-slate-400 font-medium leading-relaxed">{p.desc}</p>
          </a>
        ))}
      </div>

      <div className="grid grid-cols-1 xl:grid-cols-3 gap-10">
          <div className="xl:col-span-2 bg-indigo-600 p-12 rounded-[4rem] text-white relative overflow-hidden shadow-2xl shadow-indigo-600/30 group cursor-pointer" onClick={() => window.location.hash = 'logs'}>
            <div className="relative z-10">
              <h3 className="text-3xl font-black mb-4 flex items-center gap-4 italic"><Monitor size={36} /> Server Health Control</h3>
              <p className="text-indigo-100 mb-10 text-sm max-w-lg leading-loose font-bold uppercase tracking-wide opacity-90">Diagnostika VPS, logy z API a systémové prostředky Contaba v reálném čase.</p>
              <div className="flex gap-4">
                <button className="bg-white text-indigo-600 px-10 py-4 rounded-[2rem] font-black text-sm shadow-2xl hover:scale-105 transition-all flex items-center gap-2">
                   <Terminal size={20} /> Otevřít Diagnostiku
                </button>
              </div>
            </div>
            <Activity className="absolute -right-16 -bottom-16 text-white/5 w-80 h-80 group-hover:scale-110 transition-transform duration-1000" />
          </div>

          <div className="bg-slate-900 p-10 rounded-[4rem] text-white flex flex-col justify-between border border-slate-800 shadow-2xl relative overflow-hidden">
             <div className="bg-indigo-500/10 p-4 rounded-2xl border border-indigo-500/20 w-fit mb-4 relative z-10"><Cpu className="text-indigo-400" size={32} /></div>
             <div className="relative z-10">
                <h4 className="text-2xl font-black mb-2 italic">Runtime Active</h4>
                <div className="flex items-center gap-3 text-emerald-500 text-[11px] font-black uppercase tracking-[0.2em]">
                   <span className="w-2.5 h-2.5 bg-emerald-500 rounded-full animate-pulse shadow-lg shadow-emerald-500/50"></span> VPS Online
                </div>
                <p className="text-[10px] text-slate-500 mt-6 font-bold leading-relaxed tracking-wide italic">Ubuntu 24.04 &bull; .NET 10.0 &bull; Docker Compose</p>
             </div>
             <Activity className="absolute -left-10 -bottom-10 text-slate-800/10 w-48 h-48" />
          </div>
      </div>
    </main>
  </div>
);

export default function App() {
  const [view, setView] = useState('home');
  const [projects, setProjects] = useState(FALLBACK_PROJECTS);

  // Zajištění navigace hashů
  useEffect(() => {
    const handleNav = () => {
      const isRoman = window.location.hostname.includes('roman.');
      if (window.location.hash === '#logs') setView('logs');
      else if (isRoman) setView('roman');
      else setView('home');
    };
    window.addEventListener('hashchange', handleNav);
    handleNav(); 
    return () => window.removeEventListener('hashchange', handleNav);
  }, []);

  // Tahání dat z živého .NET API!
  useEffect(() => {
    fetch('https://api.rosimcxi.eu/api/projects')
      .then(res => res.json())
      .then(data => {
        if(data && data.length > 0) {
          // Mapování ikon podle ID, které poslal C#
          const PROJECT_STYLES = {
            1: { icon: Heart, color: 'text-rose-500', bg: 'bg-rose-50' },
            2: { icon: Activity, color: 'text-indigo-400', bg: 'bg-indigo-50' },
            3: { icon: Sparkles, color: 'text-purple-500', bg: 'bg-purple-50' },
            4: { icon: User, color: 'text-emerald-500', bg: 'bg-emerald-50' }
          };
          
          const styled = data.map(p => ({
            id: p.id,
            name: p.name,
            url: p.url,
            desc: p.type,
            icon: PROJECT_STYLES[p.id]?.icon || Globe,
            color: PROJECT_STYLES[p.id]?.color || 'text-slate-500',
            bg: PROJECT_STYLES[p.id]?.bg || 'bg-slate-50'
          }));
          setProjects(styled);
        }
      })
      .catch(err => console.log("Jedeme v offline režimu z přednastavených dat.", err));
  }, []);

  if (view === 'logs') return <LogView onBack={() => { window.location.hash = ''; }} />;
  if (view === 'roman') return <RomanView projects={projects} />;

  return <DashboardView projects={projects} />;
}

if (typeof document !== 'undefined') {
  const rootElement = document.getElementById('root');
  if (rootElement && !rootElement._reactRootContainer) {
    const root = ReactDOM.createRoot(rootElement);
    root.render(<App />);
  }
}