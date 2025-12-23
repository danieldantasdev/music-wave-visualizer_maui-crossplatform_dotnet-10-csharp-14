using AVFoundation;
using Foundation;
using MusicWaveVisualizer.Interfaces;

namespace MusicWaveVisualizer;

public class MacAudioPlayer : IAudioPlayer
{
    private AVAudioPlayer? _player;

    public bool IsPlaying => _player?.Playing ?? false;

    public void Play(string filePath)
    {
        if (_player == null)
        {
            var url = NSUrl.FromFilename(filePath);
            _player = AVAudioPlayer.FromUrl(url);
            _player.PrepareToPlay();
        }

        _player.Play();
    }

    public void Pause()
    {
        _player?.Pause();
    }

    public void Stop()
    {
        if (_player == null) return;

        _player.Stop();
        _player.CurrentTime = 0;
    }
}