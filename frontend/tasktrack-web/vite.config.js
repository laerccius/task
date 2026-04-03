import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    // Tenta ler a porta do Aspire (process.env.PORT) ou usa 5173 como fallback
    port: process.env.PORT ? parseInt(process.env.PORT) : 5173,
    strictPort: true, // Importante: força o Vite a falhar se não conseguir a porta exata
    host: true       // Garante que o Aspire consiga rotear o tráfego
  }
})