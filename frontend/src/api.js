const configuredBase = import.meta.env.VITE_API_BASE_URL ?? '';
export const apiBase = configuredBase.replace(/\/$/, '');

export async function getJson(path, options = {}) {
  const response = await fetch(`${apiBase}${path}`, options);
  if (!response.ok) throw new Error(`API HTTP ${response.status}`);
  return response.json();
}
