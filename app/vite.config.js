import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import fable from "../../vite-plugin-fable";
import Inspect from "vite-plugin-inspect";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    Inspect(),
    fable({
      jsx: "automatic",
      noReflection: true,
      exclude: ["Nojaf.Fable.React.Plugin"],
    }),
    react({ include: /\.(fs|jsx)$/ }),
  ],
  server: {
    host: "0.0.0.0",
    port: 4000,
  },
});
