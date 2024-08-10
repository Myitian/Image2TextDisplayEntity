using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Myitian.Controls;

/// <summary>可缩放图像框</summary>
/// <remarks>修改自 https://www.cnblogs.com/huangli321456/p/5113554.html</remarks>
public partial class ZoomableImage : UserControl, INotifyPropertyChanged
{
    /// <summary>可缩放图像框</summary>
    public ZoomableImage()
    {
        InitializeComponent();
        IMG.RenderTransform = tfGroup;
    }

    /// <summary>变换组</summary>
    public readonly TransformGroup tfGroup = new()
    {
        Children = {
            new ScaleTransform(1, 1),
            new TranslateTransform(0, 0)
        }
    };

    /// <summary>图像源属性</summary>
    /// <summary>拉伸属性</summary>
    /// <summary>拉伸方向属性</summary>

    /// <summary>图像源（访问Image控件）</summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(ImageSource), typeof(ZoomableImage),
            new UIPropertyMetadata(null));
    [Description("Source")]
    [Category("Common Properties")]
    public ImageSource? Source
    {
        get => GetValue(SourceProperty) as ImageSource;
        set => SetValue(SourceProperty, value);
    }
    /// <summary>拉伸（访问Image控件）</summary>
    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ZoomableImage),
            new UIPropertyMetadata(Stretch.Uniform));
    [Description("Stretch")]
    [Category("Common Properties")]
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }
    /// <summary>拉伸方向（访问Image控件）</summary>
    public static readonly DependencyProperty StretchDirectionProperty =
        DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(ZoomableImage),
            new UIPropertyMetadata(StretchDirection.Both));
    [Description("StretchDirection")]
    [Category("Common Properties")]
    public StretchDirection StretchDirection
    {
        get => (StretchDirection)GetValue(StretchDirectionProperty);
        set => SetValue(StretchDirectionProperty, value);
    }
    /// <summary>缩放级别</summary>
    public double Zoom
    {
        get => (tfGroup.Children[0] as ScaleTransform)?.ScaleX ?? 1;
        set => SetZoom(CC.ActualWidth / 2, CC.ActualHeight / 2, value, true);
    }
    /// <summary>最大缩放级别</summary>
    public static readonly DependencyProperty MaxZoomProperty =
        DependencyProperty.Register("MaxZoom", typeof(double), typeof(ZoomableImage),
            new UIPropertyMetadata(16.0));
    public double MaxZoom
    {
        get => (double)GetValue(MaxZoomProperty);
        set => SetValue(MaxZoomProperty, value);
    }
    /// <summary>最小缩放级别</summary>
    public static readonly DependencyProperty MinZoomProperty =
        DependencyProperty.Register("MinZoom", typeof(double), typeof(ZoomableImage),
            new UIPropertyMetadata(0.05));
    public double MinZoom
    {
        get => (double)GetValue(MinZoomProperty);
        set => SetValue(MinZoomProperty, value);
    }
    /// <summary>缩放因子</summary>
    public static readonly DependencyProperty ZoomFactorProperty =
        DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ZoomableImage),
            new UIPropertyMetadata(0.001));
    public double ZoomFactor
    {
        get => (double)GetValue(ZoomFactorProperty);
        set => SetValue(ZoomFactorProperty, value);
    }
    /// <summary>容器宽</summary>
    public double ContainerWidth => CC.ActualWidth;
    /// <summary>容器高</summary>
    public double ContainerHeight => CC.ActualHeight;
    /// <summary>内容宽</summary>
    public double ContentWidth => CC.ActualWidth * Zoom;
    /// <summary>内容高</summary>
    public double ContentHeight => CC.ActualHeight * Zoom;

    private void UpdateControls(ImageSource newval) => IMG.Source = newval;
    private void UpdateControls(Stretch newval) => IMG.Stretch = newval;
    private void UpdateControls(StretchDirection newval) => IMG.StretchDirection = newval;


    private bool mouseDown;
    private Point mouseXY = new(0, 0);

    /// <summary>
    /// 属性已改变
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// 属性已改变
    /// </summary>
    protected void OnPropertyChanged(string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void DoMouseMove(ContentControl img, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            TranslateTransform tt = (TranslateTransform)tfGroup.Children[1];
            Point pos = e.GetPosition(img);
            tt.X += pos.X - mouseXY.X;
            tt.Y += pos.Y - mouseXY.Y;
            mouseXY = pos;
#if DEBUG
            Console.WriteLine($"*****MOVE");
            Console.WriteLine($"X:{tt.X}");
            Console.WriteLine($"Y:{tt.Y}");
            Console.WriteLine($"P:({mouseXY})");
#endif
        }
    }

    private void DoWheelZoom(Point point, double delta)
    {
        double minZoom = MinZoom, maxZoom = MaxZoom;
        Point pointToContent = tfGroup.Inverse.Transform(point);
        ScaleTransform st = (ScaleTransform)tfGroup.Children[0];

        st.ScaleX += delta;
        if (st.ScaleX <= minZoom)
        {
            st.ScaleX = minZoom;
            st.ScaleY = minZoom;
        }
        else if (st.ScaleX >= maxZoom)
        {
            st.ScaleX = maxZoom;
            st.ScaleY = maxZoom;
        }
        else
        {
            st.ScaleY += delta;
        }
        TranslateTransform tt = (TranslateTransform)tfGroup.Children[1];
        tt.X = point.X - (pointToContent.X * st.ScaleX);
        tt.Y = point.Y - (pointToContent.Y * st.ScaleY);
        OnPropertyChanged(nameof(Zoom));
#if DEBUG
        Console.WriteLine($"*****ZOOM");
        Console.WriteLine($"AW:{IMG.ActualWidth}");
        Console.WriteLine($"AH:{IMG.ActualHeight}");
        Console.WriteLine($"S:{st.ScaleX}");
#endif
    }

    /// <summary>设置坐标</summary>
    /// <param name="point">坐标点</param>
    public void SetPos(Point point)
    {
        TranslateTransform tt = (TranslateTransform)tfGroup.Children[1];
        tt.X = point.X;
        tt.Y = point.Y;
        mouseXY = point;
#if DEBUG
        Console.WriteLine($"*****SETPOS");
        Console.WriteLine($"X:{tt.X}");
        Console.WriteLine($"Y:{tt.Y}");
        Console.WriteLine($"P:({mouseXY})");
#endif
    }
    /// <summary>设置坐标</summary>
    /// <param name="x">横坐标</param>
    /// <param name="y">纵坐标</param>
    public void SetPos(double x, double y)
    {
        TranslateTransform tt = (TranslateTransform)tfGroup.Children[1];
        tt.X = x;
        tt.Y = y;
        mouseXY = new Point(x, y);
#if DEBUG
        Console.WriteLine($"*****SETPOS");
        Console.WriteLine($"X:{tt.X}");
        Console.WriteLine($"Y:{tt.Y}");
        Console.WriteLine($"P:({mouseXY})");
#endif
    }
    /// <summary>设置缩放</summary>
    /// <param name="zoom">目标缩放级别</param>
    /// <returns>若超出缩放范围，返回<c>false</c>，否则返回<c>true</c></returns>
    public bool SetZoom(double zoom)
    {
        double minZoom = MinZoom, maxZoom = MaxZoom;
        ScaleTransform st = (ScaleTransform)tfGroup.Children[0];
        bool result = true;
        if (zoom <= minZoom)
        {
            st.ScaleX = minZoom;
            st.ScaleY = minZoom;
            result = false;
        }
        else if (zoom >= maxZoom)
        {
            st.ScaleX = maxZoom;
            st.ScaleY = maxZoom;
            result = false;
        }
        else
        {
            st.ScaleX = zoom;
            st.ScaleY = zoom;
        }
        OnPropertyChanged(nameof(Zoom));
        return result;
    }
    /// <summary>设置缩放</summary>
    /// <param name="point">缩放中心</param>
    /// <param name="zoom">目标缩放级别</param>
    /// <param name="transformPos">是否应用坐标变换</param>
    /// <returns>若超出缩放范围，返回<c>false</c>，否则返回<c>true</c></returns>
    public bool SetZoom(Point point, double zoom, bool transformPos = false)
    {
        Point pointToContent = point;
        if (transformPos)
        {
            pointToContent = tfGroup.Inverse.Transform(point);
        }
        bool result = SetZoom(zoom);
        ScaleTransform st = (ScaleTransform)tfGroup.Children[0];
        TranslateTransform tt = (TranslateTransform)tfGroup.Children[1];
        tt.X = point.X - (pointToContent.X * st.ScaleX);
        tt.Y = point.Y - (pointToContent.Y * st.ScaleY);
        OnPropertyChanged(nameof(Zoom));
#if DEBUG
        Console.WriteLine($"*****ZOOM");
        Console.WriteLine($"AW:{IMG.ActualWidth}");
        Console.WriteLine($"AH:{IMG.ActualHeight}");
        Console.WriteLine($"S:{st.ScaleX}");
#endif
        return result;
    }
    /// <summary>设置缩放</summary>
    /// <param name="x">缩放中心横坐标</param>
    /// <param name="y">缩放中心纵坐标</param>
    /// <param name="zoom">目标缩放级别</param>
    /// <param name="transformPos">是否应用坐标变换</param>
    /// <returns>若超出缩放范围，返回<c>false</c>，否则返回<c>true</c></returns>
    public bool SetZoom(double x, double y, double zoom, bool transformPos = false) => SetZoom(new Point(x, y), zoom, transformPos);
    /// <summary>重置位置</summary>
    public void Reset()
    {
        SetPos(0, 0);
        SetZoom(1);
    }



    private void ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is ContentControl img)
        {
            img.CaptureMouse();
            mouseDown = true;
            mouseXY = e.GetPosition(img);
        }
    }

    private void ContentControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is ContentControl img)
        {
            img.ReleaseMouseCapture();
            mouseDown = false;
        }
    }

    private void ContentControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (sender is ContentControl img && mouseDown)
        {
            DoMouseMove(img, e);
        }
    }

    private void ContentControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is ContentControl img)
        {
            Point point = e.GetPosition(img);
            double delta = e.Delta * ZoomFactor;
            DoWheelZoom(point, delta);
        }
    }

    private void ContentControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        //WIP
    }
}
