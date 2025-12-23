using MusicWaveVisualizer.Interfaces;
using UIKit;

namespace MusicWaveVisualizer;

public class ApplicationCloser : IApplicationCloser
{
    public void Quit()
    {
        UIApplication.SharedApplication.PerformSelector(
            new ObjCRuntime.Selector("terminateWithSuccess"),
            UIApplication.SharedApplication,
            0f
        );
    }
}