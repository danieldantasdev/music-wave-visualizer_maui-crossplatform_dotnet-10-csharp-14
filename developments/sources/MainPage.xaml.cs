using MusicWaveVisualizer.Audios;
using MusicWaveVisualizer.Interfaces;
using MusicWaveVisualizer.Renderings;
using SkiaSharp.Views.Maui;

namespace MusicWaveVisualizer;

public partial class MainPage : ContentPage
{
    private readonly AudioSampleBuffer _sampleBuffer = new(1024);
    private readonly IApplicationCloser _appCloser;

    public MainPage(IApplicationCloser appCloser)
    {
        InitializeComponent();
        this._appCloser = appCloser;

        Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            WaveCanvas.InvalidateSurface();
            return true;
        });
    }

    private async void OnPickWavClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync();
        if (result == null)
            return;

        var path = result.FullPath!;
        
        // Player.Source = path;
        // Player.Play();

        Task.Run(() => StreamSamples(path));
    }

    private void StreamSamples(string path)
    {
        foreach (var sample in WavFileReader.ReadSamples(path))
        {
            _sampleBuffer.Add(sample);
            Thread.Sleep(1);
        }
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        WaveformRenderer.Draw(
            e.Surface.Canvas,
            _sampleBuffer.Snapshot(),
            e.Info.Width,
            e.Info.Height
        );
    }
    
    private void OnExitClicked(object sender, EventArgs e)
    {
        QuitApplication();
    }
    
    private void QuitApplication()
    {
        _appCloser.Quit();
    }
}