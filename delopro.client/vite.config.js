import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import fs from 'fs';
import child_process from 'child_process';
import { env } from 'process';

const certificateName = "delopro.client";
const certFilePath = `${certificateName}.pem`;
const keyFilePath = `${certificateName}.key`;

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7250';

export default defineConfig(({ mode }) => {
  const isDev = mode === 'development'
  console.log("ENVIRONMENT: " + mode)
  if (isDev) {
    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
      const result = child_process.spawnSync('dotnet',
        [
          'dev-certs',
          'https',
          '--export-path',
          certFilePath,
          '--format',
          'Pem',
          '--no-password',
        ],
         { stdio: 'inherit' })

        if (result.status !== 0) {
          throw new Error("Could not create certificate.")
        }
      }
    }

    return {
      plugins: [plugin()],
      appType: 'spa',
      resolve: {
        alias: { '@': fileURLToPath(new URL('./src', import.meta.url))

        }
      },
      server: isDev
        ? {
            proxy: {
               '^/delopro': {
                 target,
                 secure: false
              }
            },
            port: 5173,
            https: {
              key: fs.readFileSync(keyFilePath), cert: fs.readFileSync(certFilePath),
            }
          } : undefined,
          build: {
            chunkSizeWarningLimit: 2000
          }
        }
      })
