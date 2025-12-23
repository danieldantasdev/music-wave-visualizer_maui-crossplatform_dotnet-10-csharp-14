using MusicWaveVisualizer.Audios;
using MusicWaveVisualizer.Interfaces;
using MusicWaveVisualizer.Renderings;
using SkiaSharp.Views.Maui;

namespace MusicWaveVisualizer;

public partial class MainPage : ContentPage
{
    private readonly AudioSampleBuffer _sampleBuffer = new(1024);
    private readonly IApplicationCloser _appCloser;
    private readonly IAudioPlayer _audioPlayer;

    public MainPage(IApplicationCloser appCloser, IAudioPlayer audioPlayer)
    {
        InitializeComponent();
        _appCloser = appCloser;
        _audioPlayer = audioPlayer;

        Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            WaveCanvas.InvalidateSurface();
            return true;
        });
    }

    private async void OnPickWavClicked(object sender, EventArgs e)
    {
        FileResult? result = await FilePicker.Default.PickAsync();
        if (result == null) return;

        string path = result.FullPath!;
        _audioPlayer.Play(path);

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