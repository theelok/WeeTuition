import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5089',
        changeOrigin: false, // Keep origin as localhost:5173
        secure: false,
        ws: true,
        configure: (proxy, _options) => {
          proxy.on('proxyRes', (proxyRes, req, res) => {
            // Rewrite Set-Cookie headers to work with proxy
            const setCookieHeaders = proxyRes.headers['set-cookie'];
            if (setCookieHeaders) {
              proxyRes.headers['set-cookie'] = setCookieHeaders.map(cookie => {
                // Remove domain restriction, set path to /, ensure SameSite=Lax
                return cookie
                  .replace(/Domain=[^;]+/gi, '')
                  .replace(/Path=[^;]+/gi, 'Path=/')
                  .replace(/SameSite=None/gi, 'SameSite=Lax')
                  .replace(/;\s*$/, ''); // Remove trailing semicolon
              });
            }
          });
        }
      }
    }
  }
})
