# NEXTICON FC — WebView2 portable shell

Windows desktop wrapper for [NEXTICON-FC](https://github.com/dominikca32-maker/NEXTICON-FC).  
Runs the production build inside **Microsoft Edge WebView2** — no Node, no browser tab.

## Download

Open **Releases** on this repo and download `NEXTICON-FC-portable-win-x64.zip` after CI finishes.

1. Unzip  
2. Start `NextIconFC.exe`  
3. Needs [WebView2 Evergreen Runtime](https://developer.microsoft.com/microsoft-edge/webview2/) (normally already on Windows 10/11 with Edge)

## Layout

```
NextIconFC.exe
wwwroot/          ← Vite dist (index.html, assets/, event-heroes/, …)
```

The host maps `https://app.local` → `wwwroot/` so SPA paths work offline.

## Build on Windows (recommended for first EXE)

```powershell
# Game
git clone https://github.com/dominikca32-maker/NEXTICON-FC.git game
cd game
git checkout staging
pnpm install --frozen-lockfile
pnpm build

# Shell
cd ..\NEXTICON-FC-webview
dotnet publish src\NextIconFc.WebView\NextIconFc.WebView.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -o publish
robocopy game\dist publish\wwwroot /E
.\publish\NextIconFC.exe
```

Or after syncing `wwwroot`: `\.\scripts\pack-local.ps1 -GameDist ..\NEXTICON-FC\dist`

## CI

Workflow **Build portable EXE** publishes a Release zip when `wwwroot/index.html` is present on `main`.

To refresh the bundled game, copy a fresh `dist/` into `wwwroot/` and push.

For automatic game checkout from the private NEXTICON-FC repo, add a repo secret `GAME_REPO_TOKEN` (PAT with `repo` scope) — optional workflow input below can use it later.

## Limits

- **Windows x64 only** (WebView2). Phone → PWA or laptop as LAN server.
- Country flags (FlagCDN) need internet; Event heroes and the rest are local.
- Optional Ranking / cloud login need backend + network.

## License

WebView host code in this repo. Game content follows NEXTICON-FC.
