namespace MusicWaveVisualizer.Audios;

public sealed class AudioSampleBuffer(int size)
{
    private readonly float[] _buffer = new float[size];
    private int _index = 0;
    private readonly object _lock = new();

    public void Add(float sample)
    {
        lock (_lock)
        {
            _buffer[_index] = sample;
            _index = (_index + 1) % _buffer.Length;
        }
    }

    public float[] Snapshot()
    {
        lock (_lock)
        {
            return _buffer.ToArray();
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            _index = 0;
        }
    }
}