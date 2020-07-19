const resolve = (path) => require("path").join(__dirname, path);

module.exports = {
    entry: resolve("fsharp/client.fsproj"),
    outDir: resolve("./src/bin"),
    fable:{
        define: ["FABLE_COMPILER"]
    },
    babel: {
        // plugins: ["@babel/plugin-transform-modules-commonjs"],
        // sourceMaps: true,
        presets: [ ["@babel/preset-env", {"modules": false}] ],
        filename: "client.fsproj"
    },
    allFiles: true
};