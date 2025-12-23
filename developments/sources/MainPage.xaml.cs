using NAudio.Wave;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace MusicWaveVisualizer;

public partial class MainPage : ContentPage
{
    private WaveOutEvent? waveOut;
    private AudioFileReader? audioReader;
    private BufferedWaveProvider? bufferedProvider;
    private float[] bufferSamples = new float[1024];
    private int sampleCount = 0;

    public MainPage()
    {
        InitializeComponent();

        // Timer para atualizar o canvas ~60 FPS
        Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            WaveCanvas.InvalidateSurface();
            return true;
        });
    }

    private async void OnPickWavClicked(object sender, EventArgs e)
    {
        try
        {
            var wavFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.audio" } },
                { DevicePlatform.MacCatalyst, new[] { "public.audio" } },
                { DevicePlatform.Android, new[] { "audio/wav" } },
                { DevicePlatform.WinUI, new[] { ".wav" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecione um arquivo WAV",
                FileTypes = wavFileType
            });

            if (result == null)
            {
                await DisplayAlert("Aviso", "Nenhum arquivo selecionado.", "OK");
                return;
            }

            // Usa diretamente o caminho físico no Windows, Android ou iOS
            string filePath = result.FullPath ?? throw new Exception("Não foi possível obter o caminho do arquivo");

            // Inicializa NAudio
            waveOut?.Dispose();
            audioReader?.Dispose();

            audioReader = new AudioFileReader(filePath);
            waveOut = new WaveOutEvent();
            waveOut.Init(audioReader);
            waveOut.Play();

            if (audioReader != null)
                Task.Run(() => StreamAudio(audioReader));
            
            await DisplayAlert("Sucesso", "Tocando e visualizando WAV em tempo real!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }
    private void StreamAudio(AudioFileReader reader)
    {
        var tempBuffer = new float[1024];
        var byteBuffer = new byte[tempBuffer.Length * sizeof(float)];

        while (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
        {
            int read = reader.Read(tempBuffer, 0, tempBuffer.Length);
            if (read == 0) break;

            Buffer.BlockCopy(tempBuffer, 0, byteBuffer, 0, read * sizeof(float));
            bufferedProvider?.AddSamples(byteBuffer, 0, read * sizeof(float));

            lock (bufferSamples)
            {
                sampleCount = read;
                Array.Copy(tempBuffer, bufferSamples, read);
            }

            Thread.Sleep(16);
        }
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);

        float width = e.Info.Width;
        float height = e.Info.Height;

        using var paint = new SKPaint
        {
            Color = SKColors.Lime,
            StrokeWidth = 2,
            IsAntialias = true
        };

        float midY = height / 2;
        float amplitude = height * 0.4f;

        float[] localBuffer;
        lock (bufferSamples)
        {
            localBuffer = bufferSamples.Take(sampleCount).ToArray();
        }

        if (localBuffer.Length > 1)
        {
            float step = width / localBuffer.Length;
            for (int i = 1; i < localBuffer.Length; i++)
            {
                float x1 = (i - 1) * step;
                float y1 = midY - localBuffer[i - 1] * amplitude;

                float x2 = i * step;
                float y2 = midY - localBuffer[i] * amplitude;

                canvas.DrawLine(x1, y1, x2, y2, paint);
            }
        }
    }
}