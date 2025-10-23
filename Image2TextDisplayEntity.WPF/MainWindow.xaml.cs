using Microsoft.Win32;
using System.Buffers;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Image2TextDisplayEntity.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    public ViewModel ViewModel { get; } = new();
    public const int TextViewLimit = 8 * 1024 * 1024;
    public static readonly UTF8Encoding UTF8 = new(false);
    public static readonly SearchValues<char> SNBTSingleQuoteEscape = SearchValues.Create("'\\");
    private readonly StringBuilder sb = new(131072);

    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 属性已改变
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// 属性已改变
    /// </summary>
    protected void OnPropertyChanged(string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void B_Browse_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog ofd = new()
        {
            Filter = "Image Files|*.png;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.bmp;*.wmp;*.ico|PNG|*.png|JPEG|*.jpg;*.jpeg;*.jpe;*.jfif|GIF|*.gif|TIFF|*.tif;*.tiff|BMP|*.bmp|WMP|*.wmp|ICON|*.ico|Any Files|*.*"
        };
        if (ofd.ShowDialog() == true)
        {
            ViewModel.ImagePath = Path.GetFullPath(ofd.FileName);
        }
    }
    private void B_Load_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ViewModel.IsProcessing = true;
            if (Uri.TryCreate(ViewModel.ImagePath, UriKind.RelativeOrAbsolute, out Uri? uri))
            {
                BitmapImage bmp = new(uri);
                FormatConvertedBitmap bgr24bmp = new(bmp, PixelFormats.Bgr24, null, 0);
                ViewModel.BitmapSource = bgr24bmp;
                ViewModel.CurrentImagePath = uri.ToString();
            }
            else
            {
                MessageBox.Show("Uri格式错误");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.StackTrace, ex.Message);
        }
        finally
        {
            ViewModel.IsProcessing = false;
        }
    }
    private void B_MinusShulkerBox_Click(object sender, RoutedEventArgs e)
    {
        int newLayer = ViewModel.Layer - ViewModel.ContainerLayerCount * ViewModel.ShulkerBoxContainerCount;
        ViewModel.Layer = Math.Clamp(newLayer, 1, ViewModel.MaxLayer);
    }
    private void B_MinusContainer_Click(object sender, RoutedEventArgs e)
    {
        int newLayer = ViewModel.Layer - ViewModel.ContainerLayerCount;
        ViewModel.Layer = Math.Clamp(newLayer, 1, ViewModel.MaxLayer);
    }
    private void B_AddContainer_Click(object sender, RoutedEventArgs e)
    {
        int newLayer = ViewModel.Layer + ViewModel.ContainerLayerCount;
        ViewModel.Layer = Math.Clamp(newLayer, 1, ViewModel.MaxLayer);
    }
    private void B_AddShulkerBox_Click(object sender, RoutedEventArgs e)
    {
        int newLayer = ViewModel.Layer + ViewModel.ContainerLayerCount * ViewModel.ShulkerBoxContainerCount;
        ViewModel.Layer = Math.Clamp(newLayer, 1, ViewModel.MaxLayer);
    }
    private unsafe void B_Generate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ViewModel.IsProcessing = true;
            sb.Clear();
            (string? message, bool showString) = NBTGenerator.Create(sb, ViewModel);
            if (message is not null)
                MessageBox.Show(message, "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (showString)
                TB_Output.Text = sb.ToString();
            else
                sb.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.StackTrace, ex.Message);
        }
        finally
        {
            ViewModel.IsProcessing = false;
        }
    }
    private void B_GenerateContainer_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ViewModel.IsProcessing = true;
            sb.Clear();
            (string? message, bool showString) = NBTGenerator.CreateContainer(sb, ViewModel);
            if (message is not null)
                MessageBox.Show(message, "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (showString)
                TB_Output.Text = sb.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.StackTrace, ex.Message);
        }
        finally
        {
            ViewModel.IsProcessing = false;
        }
    }
    private void B_GenerateShulkerBox_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ViewModel.IsProcessing = true;
            sb.Clear();
            (string? message, bool showString) = NBTGenerator.CreateShulkerBox(sb, ViewModel);
            if (message is not null)
                MessageBox.Show(message, "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (showString)
                TB_Output.Text = sb.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.StackTrace, ex.Message);
        }
        finally
        {
            ViewModel.IsProcessing = false;
        }
    }

    private void B_ResetPos_Click(object sender, RoutedEventArgs e)
    {
        ZI_LargeImage.SetPos((ZI_LargeImage.ContainerWidth - ZI_LargeImage.ContentWidth) / 2,
                            (ZI_LargeImage.ContainerHeight - ZI_LargeImage.ContentHeight) / 2);
    }
    private void B_ResetZoom_Click(object sender, RoutedEventArgs e)
    {
        ZI_LargeImage.Zoom = 1;
    }
    private void B_Clear_Click(object sender, RoutedEventArgs e)
    {
        TB_Output.Clear();
    }
    private void B_Save_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog sfd = new()
        {
            FileName = "spawnegg",
            DefaultExt = ".snbt",
            Filter = "String NBT (.snbt)|*.snbt|Any Files|*.*"
        };
        if (sfd.ShowDialog() is true)
        {
            string path = Path.GetFullPath(sfd.FileName);
            string? dir = Path.GetDirectoryName(path);
            if (dir is not null)
                Directory.CreateDirectory(dir);
            File.WriteAllText(path, TB_Output.Text, UTF8);
        }
    }
    private void B_Copy_Click(object sender, RoutedEventArgs e)
    {
        do
        {
            try
            {
                Clipboard.SetDataObject(TB_Output.Text);
                break;
            }
            catch
            {

            }
        }
        while (true);
    }
}