// Soubor:    Log.jsx
// Projekt:   PortalMCXI
// Verze:     v1.0 (Nezávislá na API)
// Autor:     Ing. Roman Fišer
// Popis:     Zobrazuje systémová data VPS přímo ze souboru build_info.json generovaného Pipeline.

import React, { useState, useEffect } from 'react';
import { 
  Terminal, 
  HardDrive, 
  Cpu, 
  Activity, 
  RefreshCw, 
  ChevronLeft, 
  Box, 
  Clock 
} from 'lucide-react';

export default function Log({ onBack }) {
  const [logData, setLogData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Funkce pro načtení statického JSON souboru vygenerovaného Pipeline v37.0
  const fetchLogs = async () => {
    setLoading(true);
    setError(null);
    try {
      // Soubor je umístěn v public složce, takže je dostupný přímo na kořenové URL webu
      const response = await fetch('/build_info.json');
      if (!response.ok) {
        throw new Error("Soubor build_info.json nebyl nalezen. Proběhl již Deploy verze 37.0?");
      }
      const data = await response.json();
      setLogData(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchLogs();
    // Automatické osvěžení každých 60 sekund, pokud by Pipeline zrovna běžela
    const interval = setInterval(fetchLogs, 60000);
    return () => clearInterval(interval);
  }, []);

  return (
    <div className="min-h-screen bg-[#020617] text-slate-300 font-mono p-4 md:p-8 selection:bg-indigo-500/30">
      <div className="max-w-4xl mx-auto">
        
        {/* Navigace zpět na Dashboard / Roman.jsx */}
        <button 
          onClick={onBack} 
          className="flex items-center gap-2 text-indigo-400 mb-8 hover:text-indigo-300 transition-all group"
        >
          <ChevronLeft size={20} className="group-hover:-translate-x-1 transition-transform" /> 
          Zpět na Dashboard
        </button>

        {/* Hlavička diagnostiky v terminálovém stylu */}
        <div className="flex items-center justify-between mb-8 border-b border-slate-800 pb-6">
          <div>
            <h1 className="text-2xl font-bold text-white flex items-center gap-3">
              <Terminal className="text-indigo-500" /> VPS Diagnostics
            </h1>
            <p className="text-[10px] text-slate-500 mt-1 uppercase tracking-[0.3em]">Snapshot systému z posledního nasazení</p>
          </div>
          <button 
            onClick={fetchLogs} 
            className="p-3 bg-slate-900 hover:bg-slate-800 rounded-xl transition-all border border-slate-800 shadow-lg"
            title="Aktualizovat data"
          >
            <RefreshCw size={20} className={loading ? 'animate-spin text-indigo-400' : 'text-slate-400'} />
          </button>
        </div>

        {error ? (
          <div className="bg-rose-950/20 border border-rose-900/40 p-6 rounded-2xl text-rose-400 flex items-center gap-4 animate-pulse">
            <Activity size={24} />
            <div>
              <p className="font-bold">Diagnostika nedostupná</p>
              <p className="text-sm opacity-80">{error}</p>
            </div>
          </div>
        ) : (
          <div className="grid gap-6">
            
            {/* Sekce: Timestamp nasazení */}
            <div className="bg-slate-900/40 border border-slate-800 rounded-2xl p-6 shadow-sm">
              <div className="flex items-center gap-3 mb-2 text-indigo-400">
                <Clock size={16} />
                <h2 className="text-[10px] font-bold uppercase tracking-wider text-slate-500">Čas poslední aktualizace (Pipeline)</h2>
              </div>
              <div className="text-xl font-bold text-white">
                {loading ? "Načítám..." : logData?.buildTime || "Neznámý čas"}
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Sekce: Disk - přímé info z Contaba */}
              <div className="bg-slate-900/40 border border-slate-800 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4 text-emerald-400">
                  <HardDrive size={16} />
                  <h2 className="text-[10px] font-bold uppercase tracking-wider text-slate-500">Využití disku (SSD NVMe)</h2>
                </div>
                <div className="bg-black/30 p-4 rounded-lg border border-slate-800 text-[11px] text-emerald-500 font-bold tracking-tight">
                  {loading ? "Skenuji..." : logData?.disk || "Data chybí"}
                </div>
              </div>

              {/* Sekce: RAM - reálný stav paměti */}
              <div className="bg-slate-900/40 border border-slate-800 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4 text-amber-400">
                  <Cpu size={16} />
                  <h2 className="text-[10px] font-bold uppercase tracking-wider text-slate-500">Stav operační paměti</h2>
                </div>
                <div className="bg-black/30 p-4 rounded-lg border border-slate-800 text-[11px] text-amber-500 font-bold tracking-tight">
                  {loading ? "Měřím..." : logData?.ram || "Data chybí"}
                </div>
              </div>
            </div>

            {/* Sekce: Docker - výpis služeb a jejich stavu */}
            <div className="bg-slate-900/40 border border-slate-800 rounded-2xl p-6">
              <div className="flex items-center gap-3 mb-6 text-indigo-400">
                <Box size={16} />
                <h2 className="text-[10px] font-bold uppercase tracking-wider text-slate-500">Stav běžících Docker služeb</h2>
              </div>
              <div className="grid gap-3">
                {loading ? (
                  <div className="animate-pulse space-y-3">
                    <div className="h-12 bg-slate-800/50 rounded-xl"></div>
                    <div className="h-12 bg-slate-800/50 rounded-xl w-5/6"></div>
                  </div>
                ) : logData?.docker?.split('|').map((item, i) => {
                  const [name, status] = item.split(': ');
                  if (!name) return null;
                  return (
                    <div key={i} className="flex items-center justify-between p-4 bg-black/20 rounded-xl border border-slate-800/50 hover:border-indigo-500/30 transition-colors group">
                      <span className="text-xs font-bold text-slate-300 group-hover:text-white transition-colors">
                        {name}
                      </span>
                      <span className={`text-[9px] px-3 py-1 rounded-full border font-bold uppercase tracking-widest shadow-sm ${
                        status?.toLowerCase().includes('up') 
                          ? 'bg-emerald-950/30 text-emerald-400 border-emerald-900/30' 
                          : 'bg-rose-950/30 text-rose-400 border-rose-900/30'
                      }`}>
                        {status}
                      </span>
                    </div>
                  );
                })}
              </div>
            </div>

          </div>
        )}

        <footer className="mt-16 text-center">
          <p className="text-[9px] uppercase tracking-[0.5em] text-slate-600 font-bold">
            PortalMCXI Diagnostics Module &bull; Build v37.0 &bull; IP: 62.84.181.247
          </p>
        </footer>
      </div>
    </div>
  );
}