using System;
using System.Collections.Generic;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using RapidNovel.Models;
using RapidNovel.Models.Enums;

namespace RapidNovel.Services.Status;

/// <summary>
/// Queue of status messages shown one at a time in the main window status bar.
/// Every status is displayed for <see cref="DisplayDuration"/> before it disappears
/// (or the next queued status is shown).
/// </summary>
public interface IStatusService
{
    /// <summary>The status currently shown in the status bar, or <c>null</c> when idle.</summary>
    StatusItem? CurrentStatus { get; }

    /// <summary>How long a single status stays visible before it disappears / the next one shows.</summary>
    TimeSpan DisplayDuration { get; }

    /// <summary>
    /// Queues a status. Shown immediately when the bar is idle; otherwise it waits
    /// until every earlier status has had its <see cref="DisplayDuration"/>.
    /// Safe to call from any thread.
    /// </summary>
    void Enqueue(string message, StatusSeverity severity = StatusSeverity.Info);

    /// <summary>Hides the current status and drops everything still queued.</summary>
    void Clear();
}

/// <inheritdoc cref="IStatusService"/>
public partial class StatusService : ObservableObject, IStatusService
{
    private readonly Queue<StatusItem> _queue = new();
    private readonly DispatcherTimer _timer;

    /// <summary>The status currently shown, or <c>null</c> when the bar is idle.</summary>
    [ObservableProperty]
    private StatusItem? _currentStatus;

    /// <inheritdoc/>
    public TimeSpan DisplayDuration { get; }

    public StatusService()
        : this(null)
    {
    }

    public StatusService(TimeSpan? displayDuration)
    {
        DisplayDuration = displayDuration ?? TimeSpan.FromSeconds(3);
        _timer = new DispatcherTimer { Interval = DisplayDuration };
        _timer.Tick += OnTick;
    }

    /// <inheritdoc/>
    public void Enqueue(string message, StatusSeverity severity = StatusSeverity.Info)
    {
        var item = new StatusItem(message, severity);

        if (Dispatcher.UIThread.CheckAccess())
        {
            EnqueueCore(item);
        }
        else
        {
            // The queue and timer must only be touched on the UI thread.
            Dispatcher.UIThread.Post(() => EnqueueCore(item));
        }
    }

    private void EnqueueCore(StatusItem item)
    {
        if (CurrentStatus is null)
        {
            // Bar is idle — show this status immediately and give it its full duration.
            CurrentStatus = item;
            _timer.Start();
        }
        else
        {
            // A status is on screen — line up behind the others already waiting.
            _queue.Enqueue(item);
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _timer.Stop();
        _queue.Clear();
        CurrentStatus = null;
    }

    /// <summary>Fired after the current status's <see cref="DisplayDuration"/> has elapsed.</summary>
    private void OnTick(object? sender, EventArgs e)
    {
        if (_queue.Count > 0)
        {
            // Show the next queued status for its own full duration.
            CurrentStatus = _queue.Dequeue();
            _timer.Stop();
            _timer.Start();
        }
        else
        {
            // Nothing left queued — the bar goes back to idle.
            _timer.Stop();
            CurrentStatus = null;
        }
    }
}
