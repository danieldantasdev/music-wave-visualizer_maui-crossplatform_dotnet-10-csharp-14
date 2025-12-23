namespace MusicWaveVisualizer.Interfaces;

public interface IAudioPlayer
{
    void Play(string filePath);
    void Stop();
}