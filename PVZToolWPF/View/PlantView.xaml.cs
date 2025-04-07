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
            this.DrawPlant(drawingContext);
        }
        private void DrawPlant(DrawingContext dc)
        {
            int address = 0x6a9ec0;
            Pen pen = new(Brushes.Red, 1);
            for (int i = 0; i < 5 * 9; i++)
            {
                double x = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x8 + i * 0x14C) / 1.5;
                double y = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0xc + i * 0x14C) / 1.5;
                if (x == 0 || y == 0)
                    continue;
                int blood = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xAC, 0x40 + i * 0x14C);
                System.Windows.Rect rect = new(new Point(x, y), new Size(50, 50));
                dc.DrawRectangle(Brushes.Transparent, pen, rect);
                //dc.DrawText(new FormattedText(blood.ToString(), System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, null, 12), new Point(x, y));
            }
        }
        private void Update()
        {
            if(User32.GetWindowRect(this.hwnd, out Vanara.PInvoke.RECT rect))
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
