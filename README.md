# NEXTICON FC — WebView2 portable shell

Windows desktop wrapper for [NEXTICON-FC](https://github.com/dominikca32-maker/NEXTICON-FC).  
Runs the production build inside **Microsoft Edge WebView2** — no Node, no browser tab.

Repo: https://github.com/dominikca32-maker/NEXTICON-FC-webview

## Download EXE

1. Open [Actions](https://github.com/dominikca32-maker/NEXTICON-FC-webview/actions) → latest **Build portable EXE** → artifact `NEXTICON-FC-portable-win-x64`, **or**
2. Open [Releases](https://github.com/dominikca32-maker/NEXTICON-FC-webview/releases) and download the zip.

Then:

1. Unzip
2. Put game files into `wwwroot/` if the zip only has a README there (see below)
3. Run `NextIconFC.exe`
4. Needs [WebView2 Evergreen Runtime](https://developer.microsoft.com/microsoft-edge/webview2/) (usually already on Windows 10/11 with Edge)

## Full portable in one zip (recommended on your Windows PC)

```powershell
git clone https://github.com/dominikca32-maker/NEXTICON-FC-webview.git
git clone https://github.com/dominikca32-maker/NEXTICON-FC.git
cd NEXTICON-FC
git checkout staging
pnpm install --frozen-lockfile
pnpm build
cd ..\NEXTICON-FC-webview
.\scripts\pack-local.ps1 -GameDist ..\NEXTICON-FC\dist
# → publish\NextIconFC.exe + publish\wwwroot\
```

## Optional: auto-bundle game in GitHub Actions

Add a classic PAT with `repo` scope as Actions secret **`GAME_REPO_TOKEN`** on this repo (needs read access to private `NEXTICON-FC`).  
CI will then build staging into `wwwroot` and ship a complete zip.

## How it works

Maps `https://app.local` → local `wwwroot/` via WebView2 `SetVirtualHostNameToFolderMapping`, then opens `index.html`.

## Limits

- Windows x64 only — phone: PWA or laptop LAN server
- FlagCDN flags need internet; Event heroes are local once `wwwroot` is complete

## License

WebView host here; game content follows NEXTICON-FC.
