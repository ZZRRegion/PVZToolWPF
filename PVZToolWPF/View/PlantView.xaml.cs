using CommunityToolkit.Mvvm.Messaging;
using PVZToolWPF.Model;
using PVZToolWPF.Util;
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
            base.OnRender(drawingContext);
            //this.DrawPlant(drawingContext);
            //this.DrawBullet(drawingContext);
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
                
                double x = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0x90, 0x8 + i * 0x15C) / 1.5;
                double y = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0x90, 0xC + i * 0x15C) / 1.5;
                System.Windows.Rect rect = new(new Point(x, y), new Size(50, 100));
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
                double x = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xC8, 0x8 + i * 0x94) / 1.5;
                double y = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xC8, 0xC + i * 0x94) / 1.5;
                System.Windows.Rect rect = new(new Point(x, y), new Size(50, 50));
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
                double x = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x8 + i * 0x14C) / 1.5;
                double y = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0xc + i * 0x14C) / 1.5;
                int blood = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x40 + i * 0x14C);
                System.Windows.Rect rect = new(new Point(x, y), new Size(50, 50));
                dc.DrawRectangle(Brushes.Transparent, pen, rect);
                int row = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x1C + i * 0x14C);
                int col = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x28 + i * 0x14C);
                int type = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x24 + i * 0x14C);
                string text = $"I:{i}\nHP:{blood}\nr:{row}c:{col}\nt:{type}";
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
            if(!User32.IsMinimized(hwnd) && User32.GetWindowRect(this.hwnd, out Vanara.PInvoke.RECT rect))
            {
                this.Left = rect.Left / 1.5;
                this.Top = rect.Top / 1.5;
                this.Width = rect.Width / 1.5;
                this.Height = rect.Height / 1.5;
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
