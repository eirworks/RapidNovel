# Status Bar

The status bar is the thin strip pinned to the bottom of `MainWindow`. It shows
transient, non-blocking notifications — one at a time, for **3 seconds** each —
with an optional severity level that colors a dot on the left.

```
[●] Project "My Novel" saved          ← green dot (Success), visible 3s
[●] Autosave failed: disk full        ← red dot (Error), visible 3s
[●] Checking for updates…             ← blue dot (Info, default)
```

## Queue behavior

- `Enqueue` shows the message **immediately** when the bar is idle.
- If the bar is already showing something, the new status is **queued** and
  shown only after every earlier status has had its full 3 seconds.
- When the queue is empty, the bar goes blank.
- `Clear()` hides the current status and drops everything still queued.

The display duration is configurable at construction
(`new StatusService(TimeSpan.FromSeconds(5))`), defaulting to 3 seconds.

## How to use

### 1. DI-created classes (view models, commands, services) — recommended

Inject `IStatusService` in the constructor:

```csharp
using RapidNovel.Models.Enums;
using RapidNovel.Services.Status;

public class SomeViewModel : ViewModelBase
{
    private readonly IStatusService _status;

    public SomeViewModel(IStatusService status)
    {
        _status = status;
    }

    public void Save()
    {
        try
        {
            // ... save work ...
            _status.Enqueue("Project saved", StatusSeverity.Success);
        }
        catch (Exception ex)
        {
            _status.Enqueue($"Save failed: {ex.Message}", StatusSeverity.Error);
        }
    }
}
```

The service is registered as a **singleton** in
`Services/DI/ServiceCollectionExtensions.cs`, so every resolved class gets the
same instance and all statuses share one queue.

### 2. Windows / user controls created outside DI

Views are created by `new` (windows) or by the `ViewLocator` (user controls), so
they cannot use constructor injection. Resolve the service through the
application-wide provider instead:

```csharp
using Microsoft.Extensions.DependencyInjection;
using RapidNovel.Services.Status;

// e.g. in a window's Opened/Loaded handler or a control's code-behind:
App.Services?.GetRequiredService<IStatusService>()
    .Enqueue("Opened About window", StatusSeverity.Info);
```

`App.Services` is `null` until DI is fully built, hence the `?.` guard.
If the control has a view model as `DataContext`, prefer letting the **view
model** enqueue (pattern 1) and keep the view free of service calls.

### 3. From background threads

`Enqueue` is safe to call from any thread — it marshals to the UI thread
internally before touching the queue or timer. No extra dispatching needed.

## Severity levels

| Severity | Dot color | Use for |
| --- | --- | --- |
| `StatusSeverity.Info` (default) | blue `#2B6CB0` | neutral updates ("Checking for updates…") |
| `StatusSeverity.Success` | green `#2F855A` | completed operations ("Project saved") |
| `StatusSeverity.Warning` | amber `#B7791F` | attention needed, not an error |
| `StatusSeverity.Error` | red `#C53030` | failed operations ("Save failed: …") |

The dot color mapping lives in `Converters/StatusSeverityToBrushConverter.cs`.
The message text itself is always the neutral theme foreground.

## File map

| File | Role |
| --- | --- |
| `Views/StatusBarView.axaml` | Bottom-docked bar UI (dot + message) |
| `Services/Status/StatusService.cs` | `IStatusService` — queue + 3s timer |
| `Models/StatusItem.cs` | A queued status (`Message` + `Severity`) |
| `Models/Enums/StatusSeverity.cs` | Severity enum |
| `Converters/StatusSeverityToBrushConverter.cs` | Severity → dot color |
| `Views/MainWindow.axaml` | Hosts `StatusBarView` (DockPanel.Dock="Bottom") |

## Real-world examples in this repo

- `Commands/SaveProjectCommand.cs` — green "Project saved" on success, red
  "Failed to save project: …" on exception.
- `Views/AboutWindow.axaml.cs` — posts "Opened About window" via
  `App.Services` (pattern 2 demo).

## Key points

- Always pass a `StatusSeverity` explicitly; `Info` is the default.
- Keep messages short and human-readable — they only last 3 seconds.
- Don't use the status bar for persistent or critical errors; it is transient
  by design.
