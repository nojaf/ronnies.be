module.exports = {
  mount: {
    public: { url: "/", static: true },
    src: { url: "/dist" },
  },
  plugins: [
    "@snowpack/plugin-react-refresh",
    "@snowpack/plugin-sass",
    [
      "@snowpack/plugin-run-script",
      {
        cmd: "dotnet fable ./fsharp/client.fsproj --outDir ./src/bin",
        watch: "dotnet fable watch ./fsharp/client.fsproj --outDir ./src/bin",
        output: "stream",
      },
    ],
  ],
  devOptions: {
    output: "stream",
  }
};
