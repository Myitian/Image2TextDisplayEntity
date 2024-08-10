﻿using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Myitian.Controls;

public partial class NumericUpDown : UserControl, INotifyPropertyChanged
{
    private bool suppressTextChange = false;

    public NumericUpDown()
    {
        InitializeComponent();
    }

    private static int ClampToInt(decimal value)
    {
        if (value < int.MinValue)
            return int.MinValue;
        else if (value > int.MaxValue)
            return int.MaxValue;
        else
            return (int)Math.Round(value);
    }

    /// <summary>整数模式</summary>
    public static readonly DependencyProperty IntegerModeProperty =
        DependencyProperty.Register(nameof(IntegerMode), typeof(bool), typeof(NumericUpDown),
            new UIPropertyMetadata(false));
    [Description("Integer Mode")]
    [Category("Common Properties")]
    public bool IntegerMode
    {
        get => (bool)GetValue(IntegerModeProperty);
        set
        {
            if (value)
            {
                MinValue = ClampToInt(MinValue);
                MaxValue = ClampToInt(MaxValue);
                Value = ClampToInt(Value);
                Step = ClampToInt(Step);
            }
            SetValue(IntegerModeProperty, value);
        }
    }

    /// <summary>最小值</summary>
    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register(nameof(MinValue), typeof(decimal), typeof(NumericUpDown),
            new UIPropertyMetadata(decimal.Zero));
    [Description("Min Value")]
    [Category("Common Properties")]
    public decimal MinValue
    {
        get => (decimal)GetValue(MinValueProperty);
        set
        {
            if (IntegerMode)
                value = ClampToInt(value);
            SetValue(MinValueProperty, value);
            Value = value;
        }
    }

    /// <summary>最大值</summary>
    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register(nameof(MaxValue), typeof(decimal), typeof(NumericUpDown),
            new UIPropertyMetadata(100m));
    [Description("Max Value")]
    [Category("Common Properties")]
    public decimal MaxValue
    {
        get => (decimal)GetValue(MaxValueProperty);
        set
        {
            if (IntegerMode)
                value = ClampToInt(value);
            SetValue(MaxValueProperty, value);
            Value = value;
        }
    }

    /// <summary>步长</summary>
    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register(nameof(Step), typeof(decimal), typeof(NumericUpDown),
            new UIPropertyMetadata(decimal.One));
    [Description("Step")]
    [Category("Common Properties")]
    public decimal Step
    {
        get => (decimal)GetValue(StepProperty);
        set
        {
            if (IntegerMode)
                value = ClampToInt(value);
            SetValue(StepProperty, value);
        }
    }

    /// <summary>值</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(NumericUpDown),
            new UIPropertyMetadata(decimal.Zero));
    [Description("Value")]
    [Category("Common Properties")]
    public decimal Value
    {
        get => (decimal)GetValue(ValueProperty);
        set
        {
            Math.Clamp(value, MinValue, MaxValue);
            if (IntegerMode)
                value = ClampToInt(value);
            SetValue(ValueProperty, value);
            OnPropertyChanged(nameof(Value));
            if (!suppressTextChange)
                NUDTextBox.Text = Value.ToString();
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

    private void NUDButtonUP_Click(object sender, RoutedEventArgs e)
    {
        if (Value < MinValue)
        {
            Value = MinValue;
        }
        else
        {
            decimal newVal = Value + Step;
            if (newVal < MaxValue)
                Value = newVal;
            else
                Value = MaxValue;
        }
    }

    private void NUDButtonDown_Click(object sender, RoutedEventArgs e)
    {
        if (Value > MaxValue)
        {
            Value = MaxValue;
        }
        else
        {
            decimal newVal = Value - Step;
            if (newVal > MinValue)
                Value = newVal;
            else
                Value = MinValue;
        }
    }

    private void NUDTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            NUDButtonUP.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(NUDButtonUP, [true]);
        }
        if (e.Key == Key.Down)
        {
            NUDButtonDown.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(NUDButtonDown, [true]);
        }
    }

    private void NUDTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(NUDButtonUP, [false]);

        if (e.Key == Key.Down)
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(NUDButtonDown, [false]);
    }

    private void NUDTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NUDTextBox.Text))
        {
            Value = 0;
        }
        else if (decimal.TryParse(NUDTextBox.Text, out decimal n))
        {
            if (n > MaxValue)
            {
                Value = MaxValue;
            }
            else if (n < MinValue)
            {
                Value = MinValue;
            }
            else
            {
                suppressTextChange = true;
                Value = n;
                suppressTextChange = false;
            }
        }
        else
        {
            NUDTextBox.Text = Value.ToString();
        }
    }

    private void NUDTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        NUDTextBox.Text = Value.ToString();
    }
}
