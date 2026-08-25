using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace NextIconFc.WebView;

public sealed class MainForm : Form
{
    private const string VirtualHost = "app.local";
    private readonly WebView2 _webView = new();
    private readonly Label _status = new();

    public MainForm()
    {
        Text = "NEXTICON FC";
        Width = 1280;
        Height = 800;
        MinimumSize = new Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(8, 9, 11);

        _status.Dock = DockStyle.Fill;
        _status.ForeColor = Color.White;
        _status.BackColor = Color.FromArgb(8, 9, 11);
        _status.Font = new Font("Segoe UI", 11f);
        _status.TextAlign = ContentAlignment.MiddleCenter;
        _status.Text = "Starting WebView2…";

        _webView.Dock = DockStyle.Fill;
        _webView.Visible = false;

        Controls.Add(_webView);
        Controls.Add(_status);

        Shown += async (_, _) => await StartAsync();
    }

    private async Task StartAsync()
    {
        try
        {
            var wwwroot = ResolveWwwRoot();
            if (wwwroot is null)
            {
                _status.Text =
                    "Game files missing.\n\n" +
                    "Put the NEXTICON FC production build into a folder named \"wwwroot\"\n" +
                    "next to NextIconFC.exe (contents of dist/: index.html, assets/, event-heroes/, …).\n\n" +
                    "Or download a release zip that already includes wwwroot.";
                return;
            }

            var userData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NEXTICON-FC-webview");
            Directory.CreateDirectory(userData);

            var env = await CoreWebView2Environment.CreateAsync(userDataFolder: userData);
            await _webView.EnsureCoreWebView2Async(env);

            var core = _webView.CoreWebView2;
            core.Settings.AreDefaultContextMenusEnabled = true;
            core.Settings.AreDevToolsEnabled = false;
            core.Settings.IsStatusBarEnabled = false;
            core.Settings.IsZoomControlEnabled = true;

            // Serve the Vite dist over https://app.local so SPA absolute paths work offline.
            core.SetVirtualHostNameToFolderMapping(
                VirtualHost,
                wwwroot,
                CoreWebView2HostResourceAccessKind.Allow);

            core.NavigationCompleted += (_, args) =>
            {
                if (!args.IsSuccess)
                {
                    _status.Visible = true;
                    _webView.Visible = false;
                    _status.Text = $"Failed to load the game (WebErrorStatus: {args.WebErrorStatus}).";
                }
            };

            _status.Visible = false;
            _webView.Visible = true;
            core.Navigate($"https://{VirtualHost}/index.html");
        }
        catch (WebView2RuntimeNotFoundException)
        {
            _status.Text =
                "Microsoft Edge WebView2 Runtime is not installed.\n\n" +
                "Install the Evergreen Runtime from Microsoft, then start NextIconFC.exe again:\n" +
                "https://developer.microsoft.com/microsoft-edge/webview2/";
        }
        catch (Exception ex)
        {
            _status.Text = $"Could not start WebView2:\n\n{ex.Message}";
        }
    }

    private static string? ResolveWwwRoot()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "wwwroot"),
            Path.Combine(AppContext.BaseDirectory, "dist"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "wwwroot")),
        };

        foreach (var dir in candidates)
        {
            if (File.Exists(Path.Combine(dir, "index.html")))
                return Path.GetFullPath(dir);
        }

        return null;
    }
}
