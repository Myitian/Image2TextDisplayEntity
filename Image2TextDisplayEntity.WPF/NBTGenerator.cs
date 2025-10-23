using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Image2TextDisplayEntity.WPF;

public static class NBTGenerator
{
    private static readonly SearchValues<char> snbtSingleQuoteEscape = SearchValues.Create("'\\");

    public static Result Create(StringBuilder sb, ViewModel model, int? layerOverride = null, float? offsetYOverride = null)
    {
        if (model.BitmapForProcess is null)
            model.UpdateBitmapForProcess();
        if (model.BitmapForProcess is null)
            return new("尚未加载图像！", false);
        Int32Rect rect;
        if (model.BackgroundColorMode is BackgroundColorMode.Average
            && model.CachedAverageColor is null)
        {
            int tp = model.BitmapForProcess.PixelWidth * model.BitmapForProcess.PixelHeight;
            int ts = tp * 3;
            byte[] bufferAll = ArrayPool<byte>.Shared.Rent(ts);
            try
            {
                rect = new(0, 0, model.BitmapForProcess.PixelWidth, model.BitmapForProcess.PixelHeight);
                model.BitmapForProcess.CopyPixels(rect, bufferAll, rect.Width * 3, 0);
                ReadOnlySpan<Pixel> pixels = MemoryMarshal.Cast<byte, Pixel>(bufferAll.AsSpan(0, ts));
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
            finally
            {
                ArrayPool<byte>.Shared.Return(bufferAll);
            }
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
        if (targetPixels > MainWindow.TextViewLimit)
            return new($"像素过多！\n当前值：{targetPixels}\n程序不会生成像素大于{MainWindow.TextViewLimit}图像的文本展示实体", false);
        int targetSize = targetPixels * 3;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(targetSize);
        try
        {
            model.BitmapForProcess.CopyPixels(rect, buffer, rect.Width * 3, 0);

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
            sb.Append("{entity_data:");
            int lenTDE = TextDisplayEntityMetadata.Create(sb, buffer.AsSpan(0, targetSize), in meta, true);
            if (sb.Length > MainWindow.TextViewLimit)
                return Result.StringTooLarge(sb.Length, MainWindow.TextViewLimit);
            sb.Append(",item_name:'");
            sb.AppendEscaped(model.SpawnEggName, snbtSingleQuoteEscape, out int lenName);
            if (sb.Length > MainWindow.TextViewLimit)
                return Result.StringTooLarge(sb.Length, MainWindow.TextViewLimit);
            sb.Append("'}");
            if (sb.Length > MainWindow.TextViewLimit)
                return Result.StringTooLarge(sb.Length, MainWindow.TextViewLimit);
            if (lenTDE > ushort.MaxValue)
                return new($"像素过多，导致字符串过大！\n当前值：{lenTDE}\n大于等于65536字节的NBT字符串会无法传输至服务器", true);
            if (lenName > ushort.MaxValue)
                return new($"刷怪蛋物品名称过长，导致字符串过大！\n当前值：{lenName}\n大于等于65536字节的NBT字符串会无法传输至服务器", true);
            return Result.NoMessageSuccess;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
    public static Result CreateContainer(StringBuilder sb, ViewModel model, int? layerOverride = null, float? offsetYOverride = null)
    {
        sb.Append("{container:[");
        int layer = layerOverride ?? model.Layer;
        float offY0 = offsetYOverride ?? model.OffsetY;
        int startLayer = model.CutMode is CutMode.Horizontal ? layer : 1;
        int maxLayer = model.CutMode is CutMode.Horizontal ? Math.Min(layer + model.ContainerLayerCount - 1, model.MaxLayer) : 1;
        float layerHeightB = model.LayerHeight / model.PixelPerBlock;
        for (int i = startLayer; i <= maxLayer; i++)
        {
            if (i != startLayer)
                sb.Append(',');
            sb.Append($"{{slot:{i - startLayer},item:{{id:cod_spawn_egg,components:");
            float offY = offY0 + (i - 1) * layerHeightB;
            Result result = Create(sb, model, i, offY);
            if (result.Message is not null)
            {
                if (result.ShowString)
                    sb.Append("}}]}");
                if (sb.Length >= MainWindow.TextViewLimit)
                    return Result.StringTooLarge(sb.Length, MainWindow.TextViewLimit);
                return result;
            }
            sb.Append("}}");
        }
        sb.Append("]}");
        if (sb.Length >= MainWindow.TextViewLimit)
            return Result.StringTooLarge(sb.Length, MainWindow.TextViewLimit);
        return Result.NoMessageSuccess;
    }
    public static Result CreateShulkerBox(StringBuilder sb, ViewModel model, int? layerOverride = null, float? offsetYOverride = null)
    {
        sb.Append("{container:[");
        int layer0 = layerOverride ?? model.Layer;
        float offY = offsetYOverride ?? model.OffsetY;
        int startLayer = model.CutMode is CutMode.Horizontal ? layer0 : 1;
        int boxCount = model.CutMode is CutMode.Horizontal ? model.ShulkerBoxContainerCount : 1;
        for (int i = 0, layer = startLayer; i < boxCount && layer <= model.MaxLayer; i++, layer += model.ContainerLayerCount)
        {
            if (i != 0)
                sb.Append(',');
            sb.Append($"{{slot:{i},item:{{id:chest,components:");
            Result result = CreateContainer(sb, model, layer, offY);
            if (result.Message is not null)
            {
                if (result.ShowString)
                    sb.Append("}}]}");
                if (sb.Length >= MainWindow.TextViewLimit)
                    return Result.StringTooLarge(sb.Length, MainWindow.TextViewLimit);
                return result;
            }
            sb.Append("}}");
        }
        sb.Append("]}");
        if (sb.Length >= MainWindow.TextViewLimit)
            return Result.StringTooLarge(sb.Length, MainWindow.TextViewLimit);
        return Result.NoMessageSuccess;
    }

    public readonly struct Result(string? message, bool showString)
    {
        public readonly string? Message = message;
        public readonly bool ShowString = showString;

        public readonly static Result NoMessageSuccess = new(null, true);

        public static Result StringTooLarge(int current, int max)
            => new($"字符串过大！\n当前值：{current}\n程序不会显示长度大于等于{max}的字符串", false);

        public void Deconstruct(out string? message, out bool showString)
        {
            message = Message;
            showString = ShowString;
        }
    }
}
