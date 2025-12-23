namespace MusicWaveVisualizer.Audios;

public static class WavFileReader
{
    public static IEnumerable<float> ReadSamples(string filePath)
    {
        using var fs = File.OpenRead(filePath);
        using var reader = new BinaryReader(fs);

        // WAV PCM header padrão = 44 bytes
        fs.Seek(44, SeekOrigin.Begin);

        while (fs.Position < fs.Length)
        {
            short sample = reader.ReadInt16();     // PCM 16 bits
            yield return sample / 32768f;          // normalização [-1, 1]
        }
    }
}