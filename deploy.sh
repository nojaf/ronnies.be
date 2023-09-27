#!/bin/bash
npm i --prefix ./functions
dotnet tool restore
dotnet restore
bun install --cwd ./app
dotnet fable ./App.fsproj --cwd ./app -e .js -o ./out --fableLib fable-library --noReflection --run bun run build
dotnet fable ./Functions.fsproj --cwd ./functions -e .js --noCache
firebase deploy --only hosting,functions
