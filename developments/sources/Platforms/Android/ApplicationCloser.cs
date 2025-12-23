using MusicWaveVisualizer.Interfaces;
using UIKit;

namespace MusicWaveVisualizer;

using Android.OS;

public class ApplicationCloser : IApplicationCloser
{
    public void Quit()
    {
        Process.KillProcess(Process.MyPid());
    }
}