using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Myitian.Controls;

/// <summary>
/// ColorDisplayer.xaml 的交互逻辑
/// </summary>
public partial class ColorDisplayer : UserControl
{
    public ColorDisplayer()
    {
        InitializeComponent();
    }

    /// <summary>颜色</summary>
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Color), typeof(ColorDisplayer),
            new UIPropertyMetadata(Colors.Black));
    [Description("Color")]
    [Category("Common Properties")]
    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
}
