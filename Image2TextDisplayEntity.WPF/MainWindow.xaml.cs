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
    private readonly UTF8Encoding utf8 = new(false);
    private readonly SearchValues<char> snbtSingleQuoteEscape = SearchValues.Create("'\\");

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
            string fullPath = Path.GetFullPath(ViewModel.ImagePath);
            BitmapImage bmp = new(new(fullPath));
            FormatConvertedBitmap bgr24bmp = new(bmp, PixelFormats.Bgr24, null, 0);
            ViewModel.BitmapSource = bgr24bmp;
            ViewModel.CurrentImagePath = fullPath;
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
    private void B_MinusChest_Click(object sender, RoutedEventArgs e)
    {
        int newLayer = ViewModel.Layer - ViewModel.ChestLayerCount;
        ViewModel.Layer = Math.Clamp(newLayer, 1, ViewModel.MaxLayer);
    }
    private void B_AddChest_Click(object sender, RoutedEventArgs e)
    {
        int newLayer = ViewModel.Layer + ViewModel.ChestLayerCount;
        ViewModel.Layer = Math.Clamp(newLayer, 1, ViewModel.MaxLayer);
    }
    private unsafe void B_Generate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ViewModel.IsProcessing = true;
            StringBuilder sb = new();
            int lenTDE = NBTGenerator.Create(sb, ViewModel);
            int lenName = utf8.GetByteCount(ViewModel.SpawnEggName) + ViewModel.SpawnEggName.AsSpan().Count("'\\");
            if (sb.Length >= 2097152)
            {
                MessageBox.Show($"字符串过大！\n当前值：{sb.Length}\n程序不会显示长度大于等于2097152的字符串", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            TB_Output.Text = sb.ToString();
            if (lenTDE > ushort.MaxValue)
                MessageBox.Show($"像素过多，导致字符串过大！\n当前值：{lenTDE}\n大于等于65536字节的NBT字符串会无法传输至服务器", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (lenName > ushort.MaxValue)
                MessageBox.Show($"刷怪蛋物品名称过长，导致字符串过大！\n当前值：{lenName}\n大于等于65536字节的NBT字符串会无法传输至服务器", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
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
    private void B_GenerateBox_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ViewModel.IsProcessing = true;
            StringBuilder sb = new("{Items:[");
            int startLayer = ViewModel.CutMode switch
            {
                CutMode.Horizontal
                    => ViewModel.Layer,
                _
                    => 1
            };
            int maxLayer = ViewModel.CutMode switch
            {
                CutMode.Horizontal
                    => Math.Min(ViewModel.Layer + ViewModel.ChestLayerCount - 1, ViewModel.MaxLayer),
                _
                    => 1
            };
            for (int i = startLayer; i <= maxLayer; i++)
            {
                if (i != startLayer)
                    sb.Append(',');
                sb.Append($"{{Count:1b,Slot:{i - startLayer}b,id:cod_spawn_egg,tag:");
                int lenTDE = NBTGenerator.Create(sb,
                    ViewModel,
                    i,
                    ViewModel.OffsetY + (i - 1) * (float)NUD_Offset_Step.Value);
                int lenName = utf8.GetByteCount(ViewModel.SpawnEggName) + ViewModel.SpawnEggName.AsSpan().Count("'\\");
                if (sb.Length >= 2097152)
                {
                    MessageBox.Show($"字符串过大！\n当前值：{sb.Length}\n程序不会显示长度大于等于2097152的字符串", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (lenTDE > ushort.MaxValue)
                {
                    MessageBox.Show($"像素过多，导致字符串过大！\n当前值：{lenTDE}\n大于等于65536字节的NBT字符串会无法传输至服务器", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (lenName > ushort.MaxValue)
                {
                    MessageBox.Show($"刷怪蛋物品名称过长，导致字符串过大！\n当前值：{lenName}\n大于等于65536字节的NBT字符串会无法传输至服务器", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                sb.Append('}');
            }
            sb.Append("]}");
            if (sb.Length >= 2097152)
            {
                MessageBox.Show($"字符串过大！\n当前值：{sb.Length}\n程序不会显示长度大于等于2097152的字符串", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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
            File.WriteAllText(path, TB_Output.Text, utf8);
        }
    }
    private void B_Copy_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(TB_Output.Text);
    }
}