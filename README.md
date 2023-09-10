# Ronnies.be

## Prerequisites

- firebase SDK
- bun.sh (`curl -fsSL https://bun.sh/install | bash`)

## Init

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
dotnet fable watch ./App.fsproj -e .js -o ./out --fableLib fable-library --noReflection --run bun run dev
```

## Build application

```bash
dotnet fable ./App.fsproj -e .js -o ./out --fableLib fable-library --noReflection --run bun run build
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