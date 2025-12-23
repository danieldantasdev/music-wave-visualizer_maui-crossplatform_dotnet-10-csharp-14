using SkiaSharp;

namespace MusicWaveVisualizer.Renderings;

public static class WaveformRenderer
{
    public static void Draw(SKCanvas canvas, float[] samples, int width, int height)
    {
        canvas.Clear(SKColors.Black);

        using var paint = new SKPaint();
        paint.Color = SKColors.Lime;
        paint.StrokeWidth = 2;
        paint.IsAntialias = true;

        float midY = height / 2f;
        float amplitude = height * 0.45f;
        float step = width / (float)samples.Length;

        for (int i = 1; i < samples.Length; i++)
        {
            canvas.DrawLine(
                (i - 1) * step,
                midY - samples[i - 1] * amplitude,
                i * step,
                midY - samples[i] * amplitude,
                paint
            );
        }
    }
}