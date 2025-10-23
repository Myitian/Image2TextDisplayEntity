using System.Runtime.InteropServices;
using System.Text;

namespace Image2TextDisplayEntity;

public struct TextDisplayEntityMetadata
{
    public int? BlockLight;
    public int? SkyLight;
    public int? BackgroundColor;
    public int? PixelPerLine;
    public Billboard? Billboard;
    public float PixelPerBlock;
    public float Yaw;
    public float Pitch;
    public float Roll;
    public float OffsetX;
    public float OffsetY;
    public float OffsetZ;

    /// <summary>
    /// Create TextDisplayEntity NBT
    /// </summary>
    /// <param name="sb">String builder to save the nbt</param>
    /// <param name="raw">BGR24 pixels</param>
    /// <param name="meta">Metadata</param>
    /// <param name="compress">Compressed format</param>
    /// <returns>length of text part</returns>
    public static int Create(StringBuilder sb, ReadOnlySpan<byte> raw, ref readonly TextDisplayEntityMetadata meta, bool compress = true)
    {
        sb.Append("{id:'minecraft:text_display'");

        switch (meta.Billboard)
        {
            case Image2TextDisplayEntity.Billboard.Fixed:
                sb.Append(",billboard:fixed");
                break;
            case Image2TextDisplayEntity.Billboard.Horizontal:
                sb.Append(",billboard:horizontal");
                break;
            case Image2TextDisplayEntity.Billboard.Vertical:
                sb.Append(",billboard:vertical");
                break;
            case Image2TextDisplayEntity.Billboard.Center:
                sb.Append(",billboard:center");
                break;
        }

        sb.Append($",Rotation:[{meta.Yaw}f,{meta.Pitch}f]");

        if (meta.BackgroundColor.HasValue)
            sb.Append($",background:{meta.BackgroundColor}");

        if (meta.BlockLight.HasValue)
        {
            if (meta.SkyLight.HasValue)
                sb.Append($",brightness:{{block:{meta.BlockLight},sky:{meta.SkyLight}}}");
            else
                sb.Append($",brightness:{{block:{meta.BlockLight}}}");
        }
        else if (meta.SkyLight.HasValue)
            sb.Append($",brightness:{{sky:{meta.SkyLight}}}");

        if (meta.PixelPerLine.HasValue)
            sb.Append($",line_width:{meta.PixelPerLine * 8}");

        sb.Append(",transformation:{left_rotation:[0f,0f,0f,1f]");
        sb.Append($",right_rotation:{{angle:{meta.Roll / 360 * MathF.PI}f,axis:[0f,0f,1f]}}");
        sb.Append($",scale:[{5 / meta.PixelPerBlock}f,{4 / meta.PixelPerBlock}f,1f]");
        sb.Append($",translation:[{meta.OffsetX}f,{meta.OffsetY}f,{meta.OffsetZ}f]}}");

        sb.Append(",text:'[");

        int len0 = sb.Length;
        ReadOnlySpan<Pixel> pixels = MemoryMarshal.Cast<byte, Pixel>(raw);
        Pixel prev = new();
        for (int i = 0, pos = 0; i < pixels.Length; i++)
        {
            Pixel p = pixels[i];
            if (compress)
            {
                if (i > 0 && prev == p)
                {
                    sb.Append('⬛');
                    continue;
                }
                if (pos > 0)
                    AppendColor(sb.Append("\",\"color\":\""), prev, compress).Append("\"}");
                if (i > 0)
                    sb.Append(',');
                sb.Append("{\"text\":\"⬛");
                prev = p;
                pos++;
            }
            else
            {
                if (i > 0)
                    sb.Append(',');
                AppendColor(sb.Append("{\"text\":\"⬛\",\"color\":\""), p, compress).Append("\"}");
            }
        }
        if (compress && pixels.Length > 0)
            AppendColor(sb.Append("\",\"color\":\""), prev, compress).Append("\"}");

        sb.Append("]'}");
        return sb.Length - len0 + pixels.Length * 2;

        static StringBuilder AppendColor(StringBuilder sb, Pixel p, bool compress)
        {
            if (compress && Pixel.ShorterNames.TryGetValue(p, out string? name))
                return sb.Append(name);
            else
                return sb.Append($"#{p.R:X2}{p.G:X2}{p.B:X2}");
        }
    }
}

public enum Billboard
{
    Fixed,
    Vertical,
    Horizontal,
    Center
}
