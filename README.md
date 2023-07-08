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
sudo lsof -nP | grep LISTEN
```