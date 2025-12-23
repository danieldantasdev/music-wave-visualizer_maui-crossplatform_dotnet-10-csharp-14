using AVFoundation;
using Foundation;
using MusicWaveVisualizer.Interfaces;

namespace MusicWaveVisualizer;

public class MacAudioPlayer : IAudioPlayer
{
    private AVAudioPlayer? player;

    public void Play(string filePath)
    {
        player?.Stop();

        var url = NSUrl.FromFilename(filePath);
        player = AVAudioPlayer.FromUrl(url);
        player.PrepareToPlay();
        player.Play();
    }

    public void Stop()
    {
        player?.Stop();
    }
}