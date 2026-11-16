using System.Diagnostics;
using AlienCaseLights.Core.Devices;
using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class EffectRunner : IDisposable
{
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(250);

    private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(10);

    private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(1);

    private readonly CaseController controller;

    private readonly CancellationTokenSource cancellation = new();

    private readonly Stopwatch clock = Stopwatch.StartNew();

    private volatile LightProfile profile;

    private volatile bool enabled = true;

    private volatile bool connected;

    private volatile bool refreshRequested;

    private Task? loop;

    public EffectRunner(CaseController controller, LightProfile profile)
    {
        this.controller = controller;
        this.profile = profile;
    }

    public event Action<bool>? ConnectionChanged;

    public LightProfile Profile
    {
        get => profile;
        set => profile = value;
    }

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    public bool IsConnected => connected;

    public static void Compose(LightEffect effect, LightProfile profile, double seconds, bool enabled, Rgb[] frame)
    {
        if (enabled)
        {
            effect.Render(seconds, profile, frame);
        }
        else
        {
            Array.Fill(frame, Rgb.Black);
        }

        for (int index = 0; index < frame.Length; index++)
        {
            frame[index] = frame[index].Scale(profile.Brightness);
        }
    }

    public void Start()
    {
        loop ??= Task.Run(RunAsync);
    }

    public void Refresh()
    {
        refreshRequested = true;
    }

    public void Dispose()
    {
        cancellation.Cancel();

        try
        {
            loop?.Wait(TimeSpan.FromSeconds(3));
        }
        catch (AggregateException)
        {
        }

        controller.Dispose();
        cancellation.Dispose();
    }

    private async Task RunAsync()
    {
        using PeriodicTimer timer = new(UpdateInterval);

        Rgb[] frame = new Rgb[CaseLayout.Zones.Count];
        Rgb[]? lastSent = null;
        TimeSpan lastSentAt = TimeSpan.Zero;
        TimeSpan nextAttempt = TimeSpan.Zero;

        try
        {
            while (await timer.WaitForNextTickAsync(cancellation.Token))
            {
                TimeSpan now = clock.Elapsed;
                LightProfile current = profile;

                Compose(EffectCatalog.Find(current.EffectName), current, now.TotalSeconds, enabled, frame);

                Rgb[] output = frame.Select(color => color.Quantize()).ToArray();

                if (refreshRequested)
                {
                    refreshRequested = false;
                    lastSent = null;
                }

                bool due = lastSent is null || now - lastSentAt > RefreshInterval || !output.SequenceEqual(lastSent);

                if (!due || now < nextAttempt)
                {
                    continue;
                }

                bool sent = controller.ShowFrame(output);

                SetConnected(sent);

                lastSent = sent ? output : null;
                lastSentAt = now;
                nextAttempt = sent ? TimeSpan.Zero : now + RetryInterval;
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void SetConnected(bool value)
    {
        if (connected == value)
        {
            return;
        }

        connected = value;
        ConnectionChanged?.Invoke(value);
    }
}
