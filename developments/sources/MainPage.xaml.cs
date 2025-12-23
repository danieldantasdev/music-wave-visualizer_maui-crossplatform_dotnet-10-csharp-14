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

    private volatile bool _isPaused;
    private volatile bool _hasAudioLoaded;
    private string? _currentFilePath;

    public MainPage(
        IApplicationCloser appCloser,
        IAudioPlayer audioPlayer)
    {
        InitializeComponent();

        _appCloser = appCloser;
        _audioPlayer = audioPlayer;

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            if (!_isPaused)
                WaveCanvas.InvalidateSurface();

            return true;
        });
    }

    private async void OnPickWavClicked(object sender, EventArgs e)
    {
        FileResult? result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Selecione um arquivo WAV"
        });

        if (result?.FullPath is null)
            return;

        _currentFilePath = result.FullPath;

        _sampleBuffer.Clear();
        _audioPlayer.Stop();

        _isPaused = false;
        _hasAudioLoaded = true;

        _audioPlayer.Play(_currentFilePath);

        _ = Task.Run(() => StreamSamples(_currentFilePath));
    }
    
    private void StreamSamples(string path)
    {
        foreach (var sample in WavFileReader.ReadSamples(path))
        {
            while (_isPaused)
                Thread.Sleep(10);

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
    
    private void OnPlayClicked(object sender, EventArgs e)
    {
        if (!_hasAudioLoaded || _currentFilePath is null)
            return;

        _isPaused = false;

        if (!_audioPlayer.IsPlaying)
            _audioPlayer.Play(_currentFilePath);
    }

    private void OnPauseClicked(object sender, EventArgs e)
    {
        if (!_hasAudioLoaded)
            return;

        _isPaused = true;
        _audioPlayer.Pause();
    }

    private void OnStopClicked(object sender, EventArgs e)
    {
        if (!_hasAudioLoaded)
            return;

        _isPaused = true;
        _audioPlayer.Stop();
        _sampleBuffer.Clear();
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
        _appCloser.Quit();
    }
}