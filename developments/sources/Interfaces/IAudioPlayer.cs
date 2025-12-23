namespace MusicWaveVisualizer.Interfaces;

public interface IAudioPlayer
{
    void Play(string filePath);
    void Pause();
    void Stop();
    bool IsPlaying { get; }
}