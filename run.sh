#!/bin/bash
npm i --prefix ./functions
dotnet tool restore
dotnet restore
bun install --cwd ./app
parallel --line-buffer -j 3 ::: \
    "dotnet fable ./Functions.fsproj --cwd ./functions -e .js --watch --fableLib \"@fable-org/fable-library-js\" --noCache --test:MSBuildCracker" \
    "firebase emulators:start --project=ronnies-210509" \
    "dotnet fable watch ./App.fsproj --cwd ./app -e .js -o ./out --fableLib fable-library --noReflection --exclude 'Nojaf.Fable.React.Plugin' --run bunx --bun vite -d"

# dotnet fable ./App.fsproj --cwd ./app -e .jsx -o ./out --fableLib "@fable-org/fable-library-js" --noReflection --exclude 'Nojaf.Fable.React.Plugin' --test:MSBuildCracker

# dotnet fable ./Functions.fsproj -e .js --watch --fableLib "@fable-org/fable-library-js" --noCache --test:MSBuildCracker