using System;

namespace Spectra.AMD.vendor
{
    public interface IAmdAdapter : IDisposable
    {
        void SetSaturationOnAllDisplays(int vibranceLevel);
        void SetSaturationOnDisplay(int vibranceLevel, string displayName);
        bool IsAvailable();
        void Init();
    }
}
