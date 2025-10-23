#if NET9_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Runtime.InteropServices;

namespace Image2TextDisplayEntity;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Pixel : IEquatable<Pixel>
{
#if NET9_0_OR_GREATER
    public static readonly FrozenDictionary<Pixel, string> ShorterNames = new Dictionary<Pixel, string>()
#else
    public static readonly IReadOnlyDictionary<Pixel, string> ShorterNames = new Dictionary<Pixel, string>()
#endif
    {
        { new(0x000000), "black" },
        { new(0xFFAA00), "gold" },
        { new(0xAAAAAA), "gray" },
        { new(0x5555FF), "blue" },
        { new(0x55FF55), "green" },
        { new(0x55FFFF), "aqua" },
        { new(0xFF5555), "red" },
        { new(0xFFFF55), "yellow" },
        { new(0xFFFFFF), "white" },
#if NET9_0_OR_GREATER
    }.ToFrozenDictionary();
#else
    };
#endif
    public byte B;
    public byte G;
    public byte R;

    public Pixel(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }
    public Pixel(int color)
    {
        R = (byte)color;
        G = (byte)(color >> 8);
        B = (byte)(color >> 16);
    }

    public readonly bool Equals(Pixel other)
        => B == other.B && G == other.G && R == other.R;
    public override readonly bool Equals(object? obj)
        => obj is Pixel other && Equals(other);
    public override readonly int GetHashCode()
        => (B << 16) | (G << 8) | R;

    public static bool operator ==(Pixel left, Pixel right)
        => left.Equals(right);
    public static bool operator !=(Pixel left, Pixel right)
        => !left.Equals(right);
}
