# Ronnies.be

## Prerequisites

- firebase cli (https://firebase.google.com/docs/cli)
- bun.sh (`curl -fsSL https://bun.sh/install | bash`)
- `sudo apt install parallel`

## Init

## Compile functions

```bash
dotnet fable -e .js Functions.fsproj --watch --noCache
```

## Run emulators

```bash
firebase emulators:start --project=ronnies-210509
```

## Seed

```bash
dotnet fsi seed.fsx
```

## Run application

```bash
dotnet fable watch --cwd ./app ./App.fsproj -e .js -o ./out --fableLib fable-library --noReflection --run bun run dev
```

## Build application

```bash
dotnet fable ./App.fsproj --cwd ./app -e .js -o ./out --fableLib fable-library --noReflection --exclude "Nojaf.Fable.React.Plugin" --run bun run build
```

## Deploy frontend

```bash
firebase deploy --only hosting
```

## Format code

```bash
dotnet fantomas app build.fsx
```

## View in use port in Ubuntu

```bash
# Try and kill firebase processes
sudo pkill -f firebase

# Define the ports you want to search for
PORTS=("4000" "6006" "6007")

# Search for processes using the specified ports
for PORT in "${PORTS[@]}"; do
  PID=$(sudo lsof -nP | grep LISTEN | grep ":$PORT " | awk '{print $2}')

  # Check if any processes are found
  if [ -n "$PID" ]; then
    echo "Processes found listening on port $PORT: $PID"
    # Kill the processes
    sudo kill -9 $PID
    echo "Processes killed."
  else
    echo "No processes found listening on port $PORT."
  fi
done
```

```bash
sudo lsof -nP | grep LISTEN
```