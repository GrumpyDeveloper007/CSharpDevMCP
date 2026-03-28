using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

namespace CSharpDevMCP.FlaUI;

/// <summary>
/// Manages UI Automation sessions and launched applications
/// </summary>
public class SessionManager : IDisposable
{
    private readonly UIA3Automation _automation;
    private readonly Dictionary<string, Application> _applications = [];
    private readonly Dictionary<string, Window> _windows = [];
    private int _windowCounter = 0;

    public SessionManager()
    {
        _automation = new UIA3Automation();
    }

    public string RegisterWindow(Window window)
    {
        var handle = $"w{++_windowCounter}";
        _windows[handle] = window;
        return handle;
    }

    public Window? GetWindow(string handle)
    {
        return _windows.TryGetValue(handle, out var window) ? window : null;
    }

    public List<(string handle, string title, string? processName)> ListWindows()
    {
        var desktop = _automation.GetDesktop();
        var windows = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window));

        var result = new List<(string, string, string?)>();
        foreach (var w in windows)
        {
            var window = w.AsWindow();
            if (window != null && !string.IsNullOrEmpty(window.Title))
            {
                var handle = RegisterWindow(window);
                string? processName = null;
                try
                {
                    processName = window.Properties.ProcessId.TryGetValue(out var pid)
                        ? System.Diagnostics.Process.GetProcessById(pid).ProcessName
                        : null;
                }
                catch { }

                result.Add((handle, window.Title, processName));
            }
        }
        return result;
    }

    public void FocusWindow(string handle)
    {
        var window = GetWindow(handle) ?? throw new Exception($"Window not found: {handle}");
        window.Focus();
    }

    public void CloseWindow(string handle)
    {
        var window = GetWindow(handle) ?? throw new Exception($"Window not found: {handle}");
        window.Close();
        _windows.Remove(handle);
    }

    public void Dispose()
    {
        foreach (var app in _applications.Values)
        {
            try { app.Close(); } catch { }
        }
        _applications.Clear();
        _windows.Clear();
        _automation.Dispose();
    }
}
