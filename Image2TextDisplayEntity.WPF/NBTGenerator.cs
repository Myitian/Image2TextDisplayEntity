using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Image2TextDisplayEntity.WPF;

public static class NBTGenerator
{
    private static nint buffer = nint.Zero;
    private static int bufferSize = 0;
    private static readonly SearchValues<char> snbtSingleQuoteEscape = SearchValues.Create("'\\");

    public unsafe static int Create(StringBuilder sb, ViewModel model, int? layerOverride = null, float? offsetYOverride = null)
    {
        if (model.BitmapForProcess is null)
            model.UpdateBitmapForProcess();
        if (model.BitmapForProcess is null)
            return -1;
        Int32Rect rect;
        if (model.BackgroundColorMode is BackgroundColorMode.Average
            && model.CachedAverageColor is null)
        {
            int tp = model.BitmapForProcess.PixelWidth * model.BitmapForProcess.PixelHeight;
            int ts = tp * 3;
            if (bufferSize < ts)
            {
                buffer = new(NativeMemory.Realloc(buffer.ToPointer(), (nuint)ts));
                bufferSize = ts;
            }
            rect = new(0, 0, model.BitmapForProcess.PixelWidth, model.BitmapForProcess.PixelHeight);
            model.BitmapForProcess.CopyPixels(rect, buffer, bufferSize, rect.Width * 3);
            ReadOnlySpan<Pixel> pixels = new(buffer.ToPointer(), tp);
            long rSum = 0, gSum = 0, bSum = 0;
            foreach (Pixel pixel in pixels)
            {
                rSum += pixel.R;
                gSum += pixel.G;
                bSum += pixel.B;
            }
            double dtp = tp;
            model.CachedAverageColor = Color.FromRgb(
                (byte)Math.Clamp((int)Math.Round(rSum / dtp), 0, 255),
                (byte)Math.Clamp((int)Math.Round(gSum / dtp), 0, 255),
                (byte)Math.Clamp((int)Math.Round(bSum / dtp), 0, 255));
            model.UpdateColor(model.BackgroundColorMode);
        }
        switch (model.CutMode)
        {
            case CutMode.All:
                rect = new(0, 0, model.BitmapForProcess.PixelWidth, model.BitmapForProcess.PixelHeight);
                break;
            case CutMode.Horizontal:
                int layer = layerOverride ?? model.Layer;
                int height = model.BitmapForProcess.PixelHeight;
                int y = height - model.LayerHeight * layer;
                int h = y < 0 ? model.LayerHeight + y : model.LayerHeight;
                y = Math.Clamp(y, 0, height);
                rect = new(0, y, model.BitmapForProcess.PixelWidth, h);
                break;
        }
        int targetPixels = rect.Width * rect.Height;
        int targetSize = targetPixels * 3;
        if (bufferSize < targetSize)
        {
            buffer = new(NativeMemory.Realloc(buffer.ToPointer(), (nuint)targetSize));
            bufferSize = targetSize;
        }
        model.BitmapForProcess.CopyPixels(rect, buffer, bufferSize, rect.Width * 3);

        int color = (model.CurrentColor.A << 24)
                  | (model.CurrentColor.R << 16)
                  | (model.CurrentColor.G << 8)
                  | model.CurrentColor.B;
        TextDisplayEntityMetadata meta = new()
        {
            BlockLight = model.IsBlockLightEnabled ? model.BlockLight : null,
            SkyLight = model.IsSkyLightEnabled ? model.SkyLight : null,
            BackgroundColor = model.BackgroundColorMode is BackgroundColorMode.Default ? null : color,
            PixelPerLine = rect.Width,
            Billboard = model.Direction switch
            {
                Direction.CanRotateAroundVerticalAxis => Billboard.Vertical,
                Direction.CanRotateAroundHorizontalAxis => Billboard.Horizontal,
                Direction.FacingPlayer => Billboard.Center,
                _ => Billboard.Fixed
            },
            PixelPerBlock = model.PixelPerBlock,
            Yaw = model.Direction switch
            {
                Direction.FixedAxis
                or Direction.FixedCustom
                or Direction.CanRotateAroundHorizontalAxis
                    => model.YawAngle,
                _
                    => 0
            },
            Pitch = model.Direction switch
            {
                Direction.FixedAxis
                or Direction.FixedCustom
                or Direction.CanRotateAroundVerticalAxis
                    => model.PitchAngle,
                _
                    => 0
            },
            Roll = model.RollAngle,
            OffsetX = model.OffsetX,
            OffsetY = offsetYOverride ?? model.OffsetY,
            OffsetZ = model.OffsetZ
        };
        sb.Append("{EntityTag:");
        int lenTDE = TextDisplayEntityMetadata.Create(sb, new(buffer.ToPointer(), targetSize), ref meta);
        sb.Append(",display:{Name:'");
        sb.AppendEscaped(model.SpawnEggName, snbtSingleQuoteEscape);
        sb.Append("'}}");
        return lenTDE;
    }
}
