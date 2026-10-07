// =====================================================================
// SOUBOR:   frontend/src/Roman.jsx
// PROJEKT:  PortalMCXI
// VERZE:    v2.0
// AUTOR:    Ing. Roman Fišer
// POPIS:    Osobní vizitka - Centrovaný design, přepínání profesí.
// ZMĚNY:    Odstraněny padající ikony médií. Návrat k centrálnímu UI.
// =====================================================================

import { getJson } from './api';
import React, { useState, useEffect } from 'react';
import { 
  LayoutDashboard, Globe, Activity, Terminal, 
  ChevronRight, Check, ExternalLink, Heart, Sparkles, Clock, User
} from 'lucide-react';

  // Záchranná data pro případ, že by API zrovna nebylo dostupné
  const FALLBACK_PROJECTS = [
    { id: 'joga', name: 'Jóga s Miškou', url: 'https://jogasmiskou.cz', desc: 'Rezervační systém a E-shop', icon: Heart, color: 'text-rose-500', bg: 'bg-rose-50' },
    { id: 'casna', name: 'CasNa (Tracker)', url: 'https://api.rosimcxi.eu/scalar/v1', desc: 'Sledování času a projektů', icon: Clock, color: 'text-amber-500', bg: 'bg-amber-50' },
    { id: 'tatvy', name: 'Esoterika & Tatvy', url: 'https://wisdomes.eu', desc: 'Výpočty tater a numerologie', icon: Sparkles, color: 'text-purple-500', bg: 'bg-purple-50' }
  ];

export default function Roman() {
  const [activeProf, setActiveProf] = useState('all');
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);

  // Kategorie profesí
  const PROFESSIONS = [
    { id: 'all', label: 'Všechny dovednosti' },
    { id: 'dev', label: 'Vývoj & Backend' },
    { id: 'data', label: 'Databáze & Analýza' },
    { id: 'manage', label: 'Management & Konzultace' }
  ];

  // Seznam technologií
  const SKILLS = [
    { text: 'C# & .NET 10', cats: ['dev'] },
    { text: 'ASP.NET Core / Minimal API', cats: ['dev'] },
    { text: 'Delphi', cats: ['dev'] },
    { text: 'MS SQL Server', cats: ['data'] },
    { text: 'Oracle DB', cats: ['data'] },
    { text: 'Převod dat (XML, CSV, XLS)', cats: ['data'] },
    { text: 'Python & AI integrace', cats: ['dev', 'data'] },
    { text: 'React & Vite Dashboardy', cats: ['dev'] },
    { text: 'IT Analytik & Konzultant', cats: ['manage'] },
    { text: 'Projektový management', cats: ['manage'] }
  ];

  // Načtení projektů z našeho C# API
  useEffect(() => {
    getJson('/api/projects')
      .then(data => {
        if(data && data.length > 0) {
          const PROJECT_STYLES = {
            1: { icon: Heart, color: 'text-rose-500', bg: 'bg-rose-50' },
            2: { icon: Activity, color: 'text-indigo-500', bg: 'bg-indigo-50' },
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
          })).filter(p => p.id !== 4); // Skryjeme odkaz na sebe sama
          
          setProjects(styled);
        }
      })
      .catch(err => {
        console.log("Jedeme v offline režimu z přednastavených dat.", err);
        setProjects(FALLBACK_PROJECTS);
      })
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-[#020617] flex flex-col font-sans selection:bg-indigo-500/30">
      <main className="flex-1 p-8 overflow-y-auto">
        <div className="max-w-4xl mx-auto mt-12 md:mt-24">

          {/* Jméno uprostřed */}
          <div className="text-center mb-16 animate-in fade-in slide-in-from-bottom-4 duration-700">
             <h1 className="text-6xl md:text-8xl font-black text-slate-900 dark:text-white mb-6 italic tracking-tighter">
                Roman Fišer
             </h1>
             <p className="text-slate-500 font-black uppercase tracking-[0.5em] text-[10px] md:text-xs">
                Ing. & Senior IT Consultant
             </p>
          </div>

          {/* Navigace na Dashboard a Diagnostiku */}
          <div className="flex flex-col sm:flex-row gap-4 justify-center mb-24 animate-in fade-in duration-1000 delay-300">
             <button onClick={() => window.location.hash = ''} className="px-10 py-4 bg-indigo-600 text-white rounded-full font-black text-sm shadow-xl shadow-indigo-600/20 hover:bg-indigo-500 hover:scale-105 transition-all flex items-center justify-center gap-3 uppercase tracking-widest">
                <LayoutDashboard size={18}/> Hlavní Dashboard
             </button>
             <button onClick={() => window.location.hash = 'logs'} className="px-10 py-4 bg-white dark:bg-slate-800 text-slate-700 dark:text-slate-300 rounded-full font-black text-sm shadow-xl shadow-slate-200 dark:shadow-none hover:bg-slate-50 dark:hover:bg-slate-700 hover:scale-105 transition-all flex items-center justify-center gap-3 uppercase tracking-widest border border-slate-100 dark:border-slate-700">
                <Terminal size={18}/> Diagnostika
             </button>
          </div>

          {/* Přepínání profesí */}
          <div className="mb-24 animate-in fade-in duration-1000 delay-500">
             <div className="flex flex-wrap justify-center gap-3 mb-8">
                {PROFESSIONS.map(p => (
                   <button
                     key={p.id}
                     onClick={() => setActiveProf(p.id)}
                     className={`px-6 py-3 rounded-2xl font-bold text-sm transition-all ${
                        activeProf === p.id
                        ? 'bg-indigo-600 text-white shadow-lg shadow-indigo-500/30 scale-105'
                        : 'bg-white dark:bg-slate-800 text-slate-500 hover:text-slate-900 dark:hover:text-white border border-slate-200 dark:border-slate-700 hover:border-indigo-500/50'
                     }`}
                   >
                      {p.label}
                   </button>
                ))}
             </div>

             <div className="flex flex-wrap justify-center gap-3 min-h-[160px]">
                {SKILLS.filter(s => activeProf === 'all' || s.cats.includes(activeProf)).map((skill, idx) => (
                   <span key={idx} className="flex items-center gap-2 px-5 py-3 bg-white dark:bg-slate-800/80 border border-slate-200 dark:border-slate-700 rounded-xl text-sm text-slate-700 dark:text-slate-300 transition-all font-bold shadow-sm">
                      <Check className="w-4 h-4 text-emerald-500" /> {skill.text}
                   </span>
                ))}
             </div>
          </div>

          {/* Sekce Projektů (Načítáno z API) */}
          <div className="animate-in fade-in duration-1000 delay-700">
            <h2 className="text-2xl font-black text-center text-slate-900 dark:text-white mb-10 italic tracking-tight">Vybrané projekty z API</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {loading ? (
                 <div className="col-span-1 md:col-span-2 text-center text-slate-500 font-bold animate-pulse">Načítám projekty...</div>
              ) : projects.map(p => (
                 <a key={p.id} href={p.url} target="_blank" rel="noreferrer" className="bg-white dark:bg-slate-900 p-8 rounded-[2rem] shadow-xl border border-slate-100 dark:border-slate-800 hover:scale-[1.02] transition-all group relative overflow-hidden flex flex-col justify-between">
                    <div>
                      <div className={`w-14 h-14 ${p.bg} dark:bg-slate-800/50 rounded-2xl flex items-center justify-center mb-6 group-hover:rotate-6 transition-transform shadow-sm border border-white/10`}>
                        <p.icon size={28} className={p.color} />
                      </div>
                      <h3 className="font-black text-xl text-slate-900 dark:text-white mb-2 tracking-tight">{p.name}</h3>
                      <p className="text-sm text-slate-500 dark:text-slate-400 font-medium leading-relaxed">{p.desc}</p>
                    </div>
                    <div className="mt-8 flex justify-end">
                       <ExternalLink size={20} className="text-slate-300 dark:text-slate-600 group-hover:text-indigo-500 transition-colors" />
                    </div>
                 </a>
              ))}
            </div>
          </div>

        </div>
      </main>
    </div>
  );
}