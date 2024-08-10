using System.Runtime.InteropServices;

namespace Image2TextDisplayEntity;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Pixel
{
    public byte B;
    public byte G;
    public byte R;
}
