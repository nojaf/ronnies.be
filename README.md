# Ronnies.be

## Init

### Local CDN server

```bash
docker run -p 9004:8080 -d --name esm ghcr.io/esm-dev/esm.sh:latest
```

## Run application

```bash
dotnet fsi build.fsx -p Watch
```

## Format code

```bash
dotnet fantomas app build.fsx
```

## Seed

```bash
dotnet fsi seed.fsx
```
## View in use port in Ubuntu

```bash
# Define the ports you want to search for
PORTS=("4000" "6006")

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