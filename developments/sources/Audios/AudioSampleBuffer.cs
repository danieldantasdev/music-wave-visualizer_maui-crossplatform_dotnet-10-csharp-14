namespace MusicWaveVisualizer.Audios;

public sealed class AudioSampleBuffer
{
    private readonly float[] _buffer;
    private int _index;

    public AudioSampleBuffer(int size)
    {
        _buffer = new float[size];
    }

    public void Add(float sample)
    {
        _buffer[_index++ % _buffer.Length] = sample;
    }

    public float[] Snapshot()
    {
        return _buffer.ToArray();
    }
}