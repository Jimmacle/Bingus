using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bingus.Core;

public class HighResolutionTimer : IDisposable
{
    private delegate void TimerEventHandler(int id, int msg, IntPtr user, int dw1, int dw2);

    private const int TIME_PERIODIC = 1;
    private const int EVENT_TYPE = TIME_PERIODIC;
    private int _timerId;

    public event Action? Tick;
    public int Interval { get; }

    public HighResolutionTimer(int intervalMs)
    {
        Interval = intervalMs;
    }

    public void Start()
    {
        if (!OperatingSystem.IsWindows())
            throw new NotSupportedException("High resolution timers require the Windows Multimedia API.");
        
        if (Enabled)
            return;

        timeBeginPeriod(1);
        _timerId = timeSetEvent(Interval, 0, TimerHandler, IntPtr.Zero, EVENT_TYPE);
    }

    private void TimerHandler(int id, int msg, IntPtr user, int dw1, int dw2)
    {
        Tick?.Invoke();
    }

    public void Stop()
    {
        if (!Enabled)
            return;

        timeKillEvent(_timerId);
        timeEndPeriod(1);
        _timerId = 0;
    }

    public bool Enabled => _timerId != 0;

    private void ReleaseUnmanagedResources()
    {
        Stop();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~HighResolutionTimer()
    {
        Debug.Fail("High resolution timer was not disposed.");
        ReleaseUnmanagedResources();
    }

    [DllImport("winmm.dll")]
    private static extern int timeSetEvent(
        int delay,
        int resolution,
        TimerEventHandler handler,
        IntPtr user,
        int eventType);

    [DllImport("winmm.dll")]
    private static extern int timeKillEvent(int id);

    [DllImport("winmm.dll")]
    private static extern int timeBeginPeriod(int msec);

    [DllImport("winmm.dll")]
    private static extern int timeEndPeriod(int msec);
}
