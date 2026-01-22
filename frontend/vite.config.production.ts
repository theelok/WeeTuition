import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Production build configuration
export default defineConfig({
  plugins: [react()],
  build: {
    outDir: 'dist',
    assetsDir: 'assets',
    sourcemap: false,
    minify: 'terser',
  },
  // No proxy in production - API calls go directly to the backend domain
})
