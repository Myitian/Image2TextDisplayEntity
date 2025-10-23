using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Image2TextDisplayEntity.WPF;

public class ViewModel : INotifyPropertyChanged
{
    public static readonly Color DefaultColor = Color.FromArgb(64, 0, 0, 0);
    public static readonly Color TransparentColor = Color.FromArgb(0, 0, 0, 0);
    public Color? CachedAutoColor
    {
        get
        {
            if (CachedAverageColor is null)
                return null;
            double R = CachedAverageColor.Value.R / 255;
            double G = CachedAverageColor.Value.G / 255;
            double B = CachedAverageColor.Value.B / 255;
            double r = R < 0.04045 ? R / 12.92 : Math.Pow((R + 0.055) / 1.055, 2.4);
            double g = G < 0.04045 ? G / 12.92 : Math.Pow((G + 0.055) / 1.055, 2.4);
            double b = B < 0.04045 ? B / 12.92 : Math.Pow((B + 0.055) / 1.055, 2.4);
            double y = 0.2126 * r + 0.7152 * g + 0.0722 * b;
            double Y = y <= 0.0031308 ? 12.92 * y : 1.055 * Math.Pow(y, 1 / 2.4) - 0.055;
            return Y > 0.5 ? Colors.White : Colors.Black;
        }
    }
    public Color? CachedAverageColor;

    public bool IsProcessing
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IsProcessing));
        }
    } = false;

    public string ImagePath
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(ImagePath));
        }
    } = "";

    public BitmapSource? BitmapSource
    {
        get => field;
        set
        {
            field = value;
            BitmapForProcess = null;
            OnPropertyChanged(nameof(BitmapSource));
        }
    } = null;

    public string CurrentImagePath
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(CurrentImagePath));
        }
    } = "";

    public BitmapSource? BitmapForProcess
    {
        get => field;
        set
        {
            field = value;
            CachedAverageColor = null;
            OnPropertyChanged(nameof(BitmapForProcess));
        }
    } = null;

    public bool NeedCrop
    {
        get => field;
        set
        {
            field = value;
            BitmapForProcess = null;
            OnPropertyChanged(nameof(NeedCrop));
            OnPropertyChanged(nameof(MaxLayer));
        }
    } = false;

    private int _targetWidth = 1;
    public int TargetWidth
    {
        get => _targetWidth;
        set
        {
            UpdateTargetWidth(value, KeepScale);
        }
    }

    private int _targetHeight = 1;
    public int TargetHeight
    {
        get => _targetHeight;
        set
        {
            UpdateTargetHeight(value, KeepScale);
        }
    }

    public bool KeepScale
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(KeepScale));
        }
    } = true;

    public CutMode CutMode
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(CutMode));
        }
    } = CutMode.All;

    public int LayerHeight
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(LayerHeight));
            OnPropertyChanged(nameof(MaxLayer));
        }
    } = 1;

    public int MaxLayer => ((NeedCrop ? TargetHeight : BitmapSource?.PixelHeight) + LayerHeight - 1) / LayerHeight ?? 1;

    public int Layer
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Layer));
        }
    } = 1;

    public string SpawnEggName
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SpawnEggName));
        }
    } = "{\"text\":\"文本展示实体\",\"italic\":false}";

    public Direction Direction
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Direction));
        }
    } = Direction.FixedAxis;

    public HorizontalDirection HorizontalDirection
    {
        get => field;
        set
        {
            field = value;
            YawAngle = ((int)value + 3) % 4 * 90;
            OnPropertyChanged(nameof(HorizontalDirection));
        }
    } = HorizontalDirection.South;

    public VerticalDirection VerticalDirection
    {
        get => field;
        set
        {
            field = value;
            PitchAngle = ((int)value - 1) * 90;
            OnPropertyChanged(nameof(VerticalDirection));
        }
    } = VerticalDirection.Side;

    public float YawAngle
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(YawAngle));
        }
    } = 0;

    public float PitchAngle
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(PitchAngle));
        }
    } = 0;

    public float RollAngle
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(RollAngle));
        }
    } = 0;

    public BackgroundColorMode BackgroundColorMode
    {
        get => field;
        set
        {
            field = value;
            UpdateColor(value);
            OnPropertyChanged(nameof(BackgroundColorMode));
        }
    } = BackgroundColorMode.Default;

    public string ColorString
    {
        get => field;
        set
        {
            field = value;
            UpdateColor(BackgroundColorMode.Custom);
            OnPropertyChanged(nameof(ColorString));
        }
    } = "";

    public Color CurrentColor
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(CurrentColor));
        }
    } = DefaultColor;

    public bool IsCustomColorError
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IsCustomColorError));
        }
    } = false;

    public float PixelPerBlock
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(PixelPerBlock));
        }
    } = 8;

    public bool IsBlockLightEnabled
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IsBlockLightEnabled));
        }
    } = false;

    public int BlockLight
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(BlockLight));
        }
    } = 15;

    public bool IsSkyLightEnabled
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IsSkyLightEnabled));
        }
    } = false;

    public int SkyLight
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SkyLight));
        }
    } = 15;

    public float OffsetX
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(OffsetX));
        }
    } = 0;

    public float OffsetY
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(OffsetY));
        }
    } = 0;

    public float OffsetZ
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(OffsetZ));
        }
    } = 0;

    public int ContainerLayerCount
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(ContainerLayerCount));
        }
    } = 27;

    public int ShulkerBoxContainerCount
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(ShulkerBoxContainerCount));
        }
    } = 4;

    /// <summary>
    /// 属性已改变
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// 属性已改变
    /// </summary>
    protected void OnPropertyChanged(string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    internal void UpdateBitmapForProcess()
    {
        if (BitmapSource is null)
            return;
        if (NeedCrop)
        {
            TransformedBitmap tbmp = new(
                BitmapSource,
                new ScaleTransform(
                    (double)TargetWidth / BitmapSource.PixelWidth,
                    (double)TargetHeight / BitmapSource.PixelHeight));
            BitmapForProcess = tbmp;
        }
        else
        {
            BitmapForProcess = BitmapSource;
        }
    }

    internal void UpdateTargetWidth(int targetWidth, bool keepScale)
    {
        _targetWidth = targetWidth;
        BitmapForProcess = null;
        OnPropertyChanged(nameof(TargetWidth));
        if (keepScale && BitmapSource is not null)
        {
            UpdateTargetHeight((int)Math.Round((double)BitmapSource.PixelHeight / BitmapSource.PixelWidth * targetWidth), false);
        }
    }

    internal void UpdateTargetHeight(int targetHeight, bool keepScale)
    {
        _targetHeight = targetHeight;
        BitmapForProcess = null;
        OnPropertyChanged(nameof(TargetHeight));
        OnPropertyChanged(nameof(MaxLayer));
        if (keepScale && BitmapSource is not null)
        {
            UpdateTargetWidth((int)Math.Round((double)BitmapSource.PixelWidth / BitmapSource.PixelHeight * targetHeight), false);
        }
    }

    internal void UpdateColor(BackgroundColorMode mode)
    {
        Color? color;
        switch (mode)
        {
            case BackgroundColorMode.Default:
                CurrentColor = DefaultColor;
                break;
            case BackgroundColorMode.Transparent:
                CurrentColor = TransparentColor;
                break;
            case BackgroundColorMode.Black:
                CurrentColor = Colors.Black;
                break;
            case BackgroundColorMode.Average:
                color = CachedAverageColor;
                if (color is not null)
                    CurrentColor = color.Value;
                break;
            case BackgroundColorMode.Custom:
                color = ColorExtension.FromCSSValue(ColorString);
                if (color is not null)
                    CurrentColor = color.Value;
                IsCustomColorError = color is null;
                break;
        }
    }
}

public enum CutMode
{
    All,
    Horizontal
}

public enum Direction
{
    FixedAxis,
    FixedCustom,
    CanRotateAroundVerticalAxis,
    CanRotateAroundHorizontalAxis,
    FacingPlayer
}

public enum HorizontalDirection
{
    East,
    South,
    West,
    North
}

public enum VerticalDirection
{
    Top,
    Side,
    Bottom
}

public enum BackgroundColorMode
{
    Default,
    Transparent,
    Black,
    Average,
    Custom
}
