using System.Runtime.InteropServices;

namespace Spectra.AMD.vendor.adl64
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct AdlDisplayId
    {
        internal int DisplayLogicalIndex;
        internal int DisplayPhysicalIndex;
        internal int DisplayLogicalAdapterIndex;
        internal int DisplayPhysicalAdapterIndex;
    }
}
