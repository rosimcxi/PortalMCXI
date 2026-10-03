import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
  server: {
    // Povolení domén pro přístup k vývojovému serveru nebo při proxyování
    allowedHosts: [
      'roman.rosimcxi.eu',
      'rosimcxi.eu',
      'localhost'
    ]
  }
})
