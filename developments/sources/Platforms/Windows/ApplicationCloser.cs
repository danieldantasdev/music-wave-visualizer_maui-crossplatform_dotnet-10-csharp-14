using MusicWaveVisualizer.Interfaces;
using UIKit;

namespace MusicWaveVisualizer;

using Microsoft.UI.Dispatching;

public class ApplicationCloser : IApplicationCloser
{
    public void Quit()
    {
        App.Current.Exit();
    }
}