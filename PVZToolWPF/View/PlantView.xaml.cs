using CommunityToolkit.Mvvm.Messaging;
using PVZToolWPF.Model;
using PVZToolWPF.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PVZToolWPF.View
{
    /// <summary>
    /// PlantView.xaml 的交互逻辑
    /// </summary>
    public partial class PlantView : Window, IRecipient<UpdateModel>
    {
        private Kernel32.SafeHPROCESS? safeHPROCESS;
        private HWND hwnd;
        private const double DPI = 1;
        private const double PLANTRECTHEIGHT = 75 / DPI;
        private const double PLANTRECTWIDTH = 75 / DPI;
        private const double BulletHEIGHT = 30 / DPI;
        private const double BulletWIDTH = 30 / DPI;
        private const double ZombieWIDTH = 100 / DPI;
        private const double ZombieHEIGHT = 110 / DPI;
        public PlantView()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.RegisterAll(this, PVZMsgToken.Update);
        }

        void IRecipient<UpdateModel>.Receive(UpdateModel message)
        {
            this.safeHPROCESS = message.SafeHPROCESS;
            this.hwnd = message.Hwnd;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);
            SolidColorBrush brush = new SolidColorBrush(Colors.Green)
            {
                Opacity = 0.2
            };
            //drawingContext.DrawRectangle(brush, null, new Rect(new Point(0, 0), this.RenderSize));
            this.DrawPlant(drawingContext);
            this.DrawBullet(drawingContext);
            this.DrawZombies(drawingContext);
        }
        private void DrawZombies(DrawingContext dc)
        {
            int address = 0x6a9ec0;
            Pen pen = new(Brushes.Yellow, 1);
            Typeface typeface = new("宋体");
            double fontSize = 12;
            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            for (int i = 0; i < 100; i++)
            {
                double state = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0x90, 0x28 + i * 0x15C);
                
                double x = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0x90, 0x8 + i * 0x15C) / DPI;
                double y = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0x90, 0xC + i * 0x15C) / DPI;
                System.Windows.Rect rect = new(new Point(x, y), new Size(ZombieWIDTH, ZombieHEIGHT));
                dc.DrawRectangle(Brushes.Transparent, pen, rect);
                int type = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xC8, 0x5C + i * 0x94);
                string text = $"t:{type}";
                dc.DrawText(new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black,
                    pixelsPerDip),
                    new Point(x, y));
            }
        }
        private void DrawBullet(DrawingContext dc)
        {
            int address = 0x6a9ec0;
            Pen pen = new(Brushes.Blue, 1);
            Typeface typeface = new("宋体");
            double fontSize = 12;
            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            for (int i = 0; i < 100; i++)
            {
                int isDisappear = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xC8, 0x50 + i * 0x94);
                if (isDisappear > 0)
                    continue;
                double x = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xC8, 0x8 + i * 0x94) / DPI;
                double y = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xC8, 0xC + i * 0x94) / DPI;
                System.Windows.Rect rect = new(new Point(x, y), new Size(BulletWIDTH, BulletHEIGHT));
                dc.DrawRectangle(Brushes.Transparent, pen, rect);
                int type = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xC8, 0x5C + i * 0x94);
                string text = $"t:{type}";
                dc.DrawText(new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black,
                    pixelsPerDip),
                    new Point(x, y));
            }
        }
        private void DrawPlant(DrawingContext dc)
        {
            int address = 0x6a9ec0;
            Pen pen = new(Brushes.Red, 1);
            Typeface typeface = new("宋体");
            double fontSize = 12;
            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            for (int i = 0; i <= 5 * 9; i++)
            {
                double x = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x8 + i * 0x14C) / DPI;
                double y = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0xc + i * 0x14C) / DPI;
                int blood = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x40 + i * 0x14C);
                int vis = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x3c + i * 0x14C);
                System.Windows.Rect rect = new(new Point(x, y), new Size(PLANTRECTWIDTH, PLANTRECTHEIGHT));
                dc.DrawRectangle(Brushes.Transparent, pen, rect);
                int row = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x1C + i * 0x14C);
                int col = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x28 + i * 0x14C);
                int type = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x24 + i * 0x14C);
                string text = $"I:{i}\nHP:{blood}\nr:{row}c:{col}\nt:{type}\n";
                text += $"v:{vis}\n";
                dc.DrawText(new FormattedText(
                    text, 
                    System.Globalization.CultureInfo.CurrentUICulture, 
                    FlowDirection.LeftToRight, 
                    typeface,
                    fontSize,
                    Brushes.Black,
                    pixelsPerDip), 
                    new Point(x, y));
            }
        }
        private void Update()
        {
            if(!User32.IsMinimized(hwnd) && User32.GetClientRect(this.hwnd, out Vanara.PInvoke.RECT rect))
            {
                User32.GetWindowRect(this.hwnd, out Vanara.PInvoke.RECT winRect);

                this.Width = rect.Width / DPI;
                this.Height = rect.Height / DPI;
                this.Left = winRect.Left / DPI;
                this.Top = winRect.Top / DPI + winRect.Height - rect.Height;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            nint hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = User32.GetWindowLong(hwnd, User32.WindowLongFlags.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, User32.WindowLongFlags.GWL_EXSTYLE, extendedStyle | (int)User32.WindowStylesEx.WS_EX_TRANSPARENT);
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(10),
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            this.Update();
            this.InvalidateVisual();
        }
    }
}
