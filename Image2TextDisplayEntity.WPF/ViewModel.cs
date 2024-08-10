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

    private bool _isProcessing = false;
    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            _isProcessing = value;
            OnPropertyChanged(nameof(IsProcessing));
        }
    }

    private string _imagePath = "";
    public string ImagePath
    {
        get => _imagePath;
        set
        {
            _imagePath = value;
            OnPropertyChanged(nameof(ImagePath));
        }
    }

    private BitmapSource? _bitmapSource = null;
    public BitmapSource? BitmapSource
    {
        get => _bitmapSource;
        set
        {
            _bitmapSource = value;
            BitmapForProcess = null;
            OnPropertyChanged(nameof(BitmapSource));
        }
    }

    private string _currentImagePath = "";
    public string CurrentImagePath
    {
        get => _currentImagePath;
        set
        {
            _currentImagePath = value;
            OnPropertyChanged(nameof(CurrentImagePath));
        }
    }

    private BitmapSource? _bitmapForProcess = null;
    public BitmapSource? BitmapForProcess
    {
        get => _bitmapForProcess;
        set
        {
            _bitmapForProcess = value;
            CachedAverageColor = null;
            OnPropertyChanged(nameof(BitmapForProcess));
        }
    }

    private bool _needCrop = false;
    public bool NeedCrop
    {
        get => _needCrop;
        set
        {
            _needCrop = value;
            BitmapForProcess = null;
            OnPropertyChanged(nameof(NeedCrop));
            OnPropertyChanged(nameof(MaxLayer));
        }
    }

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

    private bool _keepScale = true;
    public bool KeepScale
    {
        get => _keepScale;
        set
        {
            _keepScale = value;
            OnPropertyChanged(nameof(KeepScale));
        }
    }

    private CutMode _cutMode = CutMode.All;
    public CutMode CutMode
    {
        get => _cutMode;
        set
        {
            _cutMode = value;
            OnPropertyChanged(nameof(CutMode));
        }
    }

    private int _layerHeight = 1;
    public int LayerHeight
    {
        get => _layerHeight;
        set
        {
            _layerHeight = value;
            OnPropertyChanged(nameof(LayerHeight));
            OnPropertyChanged(nameof(MaxLayer));
        }
    }

    public int MaxLayer => ((NeedCrop ? TargetHeight : BitmapSource?.PixelHeight) + LayerHeight - 1) / LayerHeight ?? 1;

    private int _layer = 1;
    public int Layer
    {
        get => _layer;
        set
        {
            _layer = value;
            OnPropertyChanged(nameof(Layer));
        }
    }

    private string _spawnEggName = "{\"text\":\"\",\"italic\":false,\"extra\":[{\"text\":\"文本\",\"color\":\"#66CCFF\",\"bold\":true},{\"text\":\"展示\",\"color\":\"white\"},{\"text\":\"实体\",\"color\":\"#39C5BB\"}]}";
    public string SpawnEggName
    {
        get => _spawnEggName;
        set
        {
            _spawnEggName = value;
            OnPropertyChanged(nameof(SpawnEggName));
        }
    }

    private Direction _direction = Direction.FixedAxis;
    public Direction Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            OnPropertyChanged(nameof(Direction));
        }
    }

    private HorizontalDirection _horizontalDirection = HorizontalDirection.South;
    public HorizontalDirection HorizontalDirection
    {
        get => _horizontalDirection;
        set
        {
            _horizontalDirection = value;
            YawAngle = ((int)value + 3) % 4 * 90;
            OnPropertyChanged(nameof(HorizontalDirection));
        }
    }

    private VerticalDirection _verticalDirection = VerticalDirection.Side;
    public VerticalDirection VerticalDirection
    {
        get => _verticalDirection;
        set
        {
            _verticalDirection = value;
            PitchAngle = ((int)value - 1) * 90;
            OnPropertyChanged(nameof(VerticalDirection));
        }
    }

    private float _yawAngle = 0;
    public float YawAngle
    {
        get => _yawAngle;
        set
        {
            _yawAngle = value;
            OnPropertyChanged(nameof(YawAngle));
        }
    }

    private float _pitchAngle = 0;
    public float PitchAngle
    {
        get => _pitchAngle;
        set
        {
            _pitchAngle = value;
            OnPropertyChanged(nameof(PitchAngle));
        }
    }

    private float _rollAngle = 0;
    public float RollAngle
    {
        get => _rollAngle;
        set
        {
            _rollAngle = value;
            OnPropertyChanged(nameof(RollAngle));
        }
    }

    private BackgroundColorMode _backgroundColorMode = BackgroundColorMode.Default;
    public BackgroundColorMode BackgroundColorMode
    {
        get => _backgroundColorMode;
        set
        {
            _backgroundColorMode = value;
            UpdateColor(value);
            OnPropertyChanged(nameof(BackgroundColorMode));
        }
    }

    private string _colorString = "";
    public string ColorString
    {
        get => _colorString;
        set
        {
            _colorString = value;
            UpdateColor(BackgroundColorMode.Custom);
            OnPropertyChanged(nameof(ColorString));
        }
    }

    private Color _currentColor = DefaultColor;
    public Color CurrentColor
    {
        get => _currentColor;
        set
        {
            _currentColor = value;
            OnPropertyChanged(nameof(CurrentColor));
        }
    }

    private bool _isCustomColorError = false;
    public bool IsCustomColorError
    {
        get => _isCustomColorError;
        set
        {
            _isCustomColorError = value;
            OnPropertyChanged(nameof(IsCustomColorError));
        }
    }

    private float _pixelPerBlock = 8;
    public float PixelPerBlock
    {
        get => _pixelPerBlock;
        set
        {
            _pixelPerBlock = value;
            OnPropertyChanged(nameof(PixelPerBlock));
        }
    }

    private bool _isBlockLightEnabled = false;
    public bool IsBlockLightEnabled
    {
        get => _isBlockLightEnabled;
        set
        {
            _isBlockLightEnabled = value;
            OnPropertyChanged(nameof(IsBlockLightEnabled));
        }
    }

    private int _blockLight = 15;
    public int BlockLight
    {
        get => _blockLight;
        set
        {
            _blockLight = value;
            OnPropertyChanged(nameof(BlockLight));
        }
    }

    private bool _isSkyLightEnabled = false;
    public bool IsSkyLightEnabled
    {
        get => _isSkyLightEnabled;
        set
        {
            _isSkyLightEnabled = value;
            OnPropertyChanged(nameof(IsSkyLightEnabled));
        }
    }

    private int _skyLight = 15;
    public int SkyLight
    {
        get => _skyLight;
        set
        {
            _skyLight = value;
            OnPropertyChanged(nameof(SkyLight));
        }
    }

    private float _offsetX = 0;
    public float OffsetX
    {
        get => _offsetX;
        set
        {
            _offsetX = value;
            OnPropertyChanged(nameof(OffsetX));
        }
    }

    private float _offsetY = 0;
    public float OffsetY
    {
        get => _offsetY;
        set
        {
            _offsetY = value;
            OnPropertyChanged(nameof(OffsetY));
        }
    }

    private float _offsetZ = 0;
    public float OffsetZ
    {
        get => _offsetZ;
        set
        {
            _offsetZ = value;
            OnPropertyChanged(nameof(OffsetZ));
        }
    }

    private int _chestLayerCount = 27;
    public int ChestLayerCount
    {
        get => _chestLayerCount;
        set
        {
            _chestLayerCount = value;
            OnPropertyChanged(nameof(ChestLayerCount));
        }
    }

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
