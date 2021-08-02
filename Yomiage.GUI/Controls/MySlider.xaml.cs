using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yomiage.GUI.Controls
{
    /// <summary>
    /// MySlider.xaml の相互作用ロジック
    /// </summary>
    public partial class MySlider : UserControl
    {

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(MySlider),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(
                "DefaultValue",
                typeof(double),
                typeof(MySlider),
                new PropertyMetadata(0.0));

        public double DefaultValue
        {
            get { return (double)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }


        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(double),
                typeof(MySlider),
                new PropertyMetadata(-1.0));

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }


        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(double),
                typeof(MySlider),
                new PropertyMetadata(1.0));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }


        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register(
                "TickFrequency",
                typeof(double),
                typeof(MySlider),
                new PropertyMetadata(1.0));

        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }


        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(
                "SmallChange",
                typeof(double),
                typeof(MySlider),
            new PropertyMetadata(1.0));

        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }


        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                "IsEnable",
                typeof(bool),
                typeof(MySlider),
                new PropertyMetadata(true));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }


        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(MySlider),
                new PropertyMetadata("Title"));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value);  }
        }


        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register(
                "StringFormat",
                typeof(string),
                typeof(MySlider),
                new PropertyMetadata("0.00"));

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }


        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(
                "Unit",
                typeof(string),
                typeof(MySlider),
                new PropertyMetadata("Unit"));

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }


        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                "Foreground",
                typeof(Brush),
                typeof(MySlider),
                new PropertyMetadata(null));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }


        public MySlider()
        {
            InitializeComponent();

        }

        private void Button_Default(object sender, RoutedEventArgs e)
        {
            Value = DefaultValue;
        }
    }
}
