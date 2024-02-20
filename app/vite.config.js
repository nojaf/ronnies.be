import {defineConfig} from 'vite'
import react from '@vitejs/plugin-react'
import fable from "../../vite-plugin-fable"

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react(), fable({jsxRuntime: "automatic", noReflection: true, exclude: ["Nojaf.Fable.React.Plugin"]})],
    server: {
        host: '0.0.0.0',
        port: 4000
    }
})
