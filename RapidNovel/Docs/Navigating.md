# Navigating in RapidNovel

How page navigation works and how to add new pages.

## How it works

The main content area of `MainWindow` is a single `ContentControl` bound to a *view model*, not a view:

```xml
<!-- Views/MainWindow.axaml -->
<ContentControl Content="{Binding Navigation.CurrentPage}"/>
```

The pipeline that turns a view model into visible UI:

1. **`MainWindowViewModel`** exposes `Navigation` (`INavigationService`), injected via DI.
2. **`NavigationService`** holds `CurrentPage` (a `ViewModelBase`, observable) and can swap it via `NavigateTo<T>()`, resolving the page VM from the DI container on demand.
3. When `CurrentPage` changes, the `ContentControl`'s `ContentPresenter` sets the new VM as `DataContext` and asks the app-level **`ViewLocator`** (registered in `App.axaml`) for a view.
4. `ViewLocator` maps `XxxViewModel` → `XxxView` by replacing the `"ViewModel"` suffix with `"View"`, then instantiates that view and returns it.

So the window never knows about specific pages — only about `INavigationService`.

## Adding a new page (step by step)

Say you want a `Characters` page.

**1. Create the view model**

```csharp
// ViewModels/CharactersPageViewModel.cs
namespace RapidNovel.ViewModels;

public partial class CharactersPageViewModel : ViewModelBase
{
    // page state, [ObservableProperty] fields, commands...
}
```

It must inherit `ViewModelBase` — `ViewLocator.Match` only matches `ViewModelBase` instances.

**2. Create the view**

```xml
<!-- Views/CharactersPage.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:RapidNovel.ViewModels"
             x:DataType="vm:CharactersPageViewModel"
             x:Class="RapidNovel.Views.CharactersPage">
    <!-- content... -->
</UserControl>
```

With a matching code-behind that just calls `InitializeComponent()`.

Naming matters: `CharactersPageViewModel` in `ViewModels/` + `CharactersPage` in `Views/` is exactly what `ViewLocator` expects.

**3. Register it in DI**

```csharp
// Services/DI/ServiceCollectionExtensions.cs
services.AddTransient<CharactersPageViewModel>(); // or AddSingleton — see "Lifetimes" below
```

**4. Navigate to it**

From anywhere that has an `INavigationService` (the window VM, a menu command, or another page VM):

```csharp
Navigation.NavigateTo<CharactersPageViewModel>();
```

`CurrentPage` is observable, so the UI swaps automatically — no manual DataContext juggling.

## Where to get `INavigationService`

`NavigationService` is a singleton, so it can be injected anywhere the container resolves:

```csharp
public partial class CharactersPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;

    public CharactersPageViewModel(INavigationService navigation)
    {
        _navigation = navigation;
    }
}
```

This is the standard way for a page to navigate to another page.

## Hooking up the menu

`MainWindow.axaml`'s menu items currently have no commands. Wire them through the window VM:

```csharp
// MainWindowViewModel.cs
[RelayCommand]
private void OpenCharacters() => Navigation.NavigateTo<CharactersPageViewModel>();
```

```xml
<MenuItem Header="Characters" Command="{Binding OpenCharactersCommand}"/>
```

Because `MainWindowViewModel` gets `INavigationService` injected, these commands are just one-liners.

## Lifetimes

- **`NavigationService`** — singleton (registered as such; do not change).
- **Page VMs** — your choice:
  - `AddTransient` — a fresh instance every time you navigate to it (good for detail pages, stateless lists).
  - `AddSingleton` — one instance kept alive across navigations; state survives leaving and returning (good for the landing page, long-lived editor state).

`MainPageViewModel` is currently registered as a singleton, which is why navigating "back" to it shows the same instance.

## Passing data into a page

`NavigateTo<T>()` has no parameters, so pages get dependencies only through their constructors. For runtime data (e.g. "open project X"), do it in two steps:

```csharp
Navigation.NavigateTo<CharactersPageViewModel>();
var page = (CharactersPageViewModel)Navigation.CurrentPage;
page.Project = project;
```

Or, if this becomes common, add a setup overload to the service:

```csharp
void NavigateTo<T>(Action<T>? setup = null) where T : ViewModelBase
{
    var page = _services.GetRequiredService<T>();
    setup?.Invoke(page);
    CurrentPage = page;
}
```

## Checklist / common pitfalls

| Pitfall | Result | Fix |
|---|---|---|
| Page VM not registered in DI | `InvalidOperationException` at navigation time | Register it in `ServiceCollectionExtensions` |
| VM doesn't inherit `ViewModelBase` | Blank content / "Not Found" text | Inherit `ViewModelBase` |
| View named wrong (`CharacterPage` vs `CharactersPage`) | "Not Found: ..." text from `ViewLocator` | Match `XxxViewModel` ↔ `XxxView` exactly |
| Binding against the wrong DataContext | Silently blank UI (compiled bindings fail silently) | Set `x:DataType` to the page's VM; rely on ViewLocator to set DataContext |

> Debugging tip: while developing a view, add `<Design.DataContext><vm:YourPageViewModel/></Design.DataContext>` inside the page for previewer support. It does not affect runtime — the `ContentPresenter`/ViewLocator sets the real DataContext.

## Files involved

| File | Role |
|---|---|
| `Services/Navigation/NavigationService.cs` | `INavigationService` + implementation, owns `CurrentPage` |
| `ViewModels/MainWindowViewModel.cs` | Exposes `Navigation`; root VM of the window |
| `Views/MainWindow.axaml` | `<ContentControl Content="{Binding Navigation.CurrentPage}"/>` |
| `ViewLocator.cs` | Maps `XxxViewModel` → `XxxView` |
| `App.axaml` | Registers `ViewLocator` as the app data template |
| `Services/DI/ServiceCollectionExtensions.cs` | Where all VMs and services are registered |
