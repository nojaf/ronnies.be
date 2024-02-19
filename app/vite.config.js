import {defineConfig} from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        host: '0.0.0.0',
        port: 4000,
        watch: {
            ignored: [
                "**/*.fs",
                "**/*.fsi" // Don't watch F# files
            ]
        }
    }
})
