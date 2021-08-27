
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ZXing;
using ZXing.Common;

namespace ScreenBarScanner
{
    /// <summary>
    /// CaptureWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CaptureWindow : Window
    {
        private double x;
        private double y;
        private double width;
        private double height;
        private double _dpiRatio;
        //private double _screenW;
        //private double _screenH;
        private TextDialog textDialog = null;
        private IBarcodeReader barcodeReader = null;

        private bool isMouseDown = false;
        public bool Invert = false;

        public CaptureWindow()
        {
            InitializeComponent();
        }

        public double dpiRatio
        {
            get => this._dpiRatio;
            set { }
        }

        //public double screenH
        //{
        //    get => this._screenH;
        //    set { }
        //}

        //public double screenW
        //{
        //    get => this._screenW;
        //    set { }
        //}

        private void CaptureWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
            x = e.GetPosition(null).X;
            y = e.GetPosition(null).Y;
        }

        private void CaptureWindow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.End();
        }

        private void End()
        {
            if (isMouseDown)
            {
                isMouseDown = false;
            }
            x = 0.0;
            y = 0.0;
            CaptureCanvas.Children.Clear();
            this.Hide();
        }

        private void CaptureWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                CaptureWindow_MouseRightButtonDown(sender, null);
                return;
            }
            if (isMouseDown)
            {
                // 1. 通过一个矩形来表示目前截图区域
                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
                double dx = e.GetPosition(null).X;
                double dy = e.GetPosition(null).Y;
                double rectWidth = Math.Abs(dx - x);
                double rectHeight = Math.Abs(dy - y);
                SolidColorBrush brush = new SolidColorBrush(Colors.White);
                rect.Width = rectWidth;
                rect.Height = rectHeight;
                rect.Fill = brush;
                rect.Stroke = brush;
                rect.StrokeThickness = 1;
                if (dx < x)
                {
                    Canvas.SetLeft(rect, dx);
                    Canvas.SetTop(rect, dy);
                }
                else
                {
                    Canvas.SetLeft(rect, x);
                    Canvas.SetTop(rect, y);
                }

                CaptureCanvas.Children.Clear();
                CaptureCanvas.Children.Add(rect);

                if (e.LeftButton == MouseButtonState.Released)
                {
                    CaptureCanvas.Children.Clear();
                    // 2. 获得当前截图区域
                    width = Math.Abs(e.GetPosition(null).X - x);
                    height = Math.Abs(e.GetPosition(null).Y - y);
                    Bitmap bitmap = null;

                    if (e.GetPosition(null).X > x)
                    {
                        bitmap = CaptureScreenWithCurrentDpi(x, y, width, height);
                    }
                    else
                    {
                        bitmap = CaptureScreenWithCurrentDpi(e.GetPosition(null).X, e.GetPosition(null).Y, width, height);
                    }
                    if (bitmap != null)
                    {
                        this.ShowText(this.BarScan(bitmap));
                    }
                    else
                    {
                        MessageBox.Show("Error.");
                    }
                    this.End();
                }
            }
        }

        private void ShowText(Result result)
        {
            if (result == null)
            {
                Console.WriteLine("Decode failed.");
                MessageBox.Show("Failed");
            }
            else
            {
                if (this.textDialog == null || this.textDialog.IsClosed)
                {
                    this.textDialog = new TextDialog();
                }
                //this._screenW = this.ActualWidth;
                //this._screenH = this.ActualHeight;
                this.textDialog.Left = this.Width / 2;
                this.textDialog.Top = this.Height / 2;
                this.textDialog.text = result.Text;
                this.textDialog.Title = Enum.GetName(typeof(BarcodeFormat), result.BarcodeFormat);
                this.textDialog.Show();
                if (!this.textDialog.IsActive)
                {
                    this.textDialog.Activate();
                }
                Console.WriteLine("BarcodeFormat: {0}", result.BarcodeFormat);
                Console.WriteLine("Result: {0}", result.Text);
            }
        }

        private Bitmap CaptureScreen(double x, double y, double width, double height)
        {
            int ix = Convert.ToInt32(x);
            int iy = Convert.ToInt32(y);
            int iw = Convert.ToInt32(width);
            int ih = Convert.ToInt32(height);

            Bitmap bitmap = new Bitmap(iw, ih);
            using (System.Drawing.Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih));
                if (this.Invert)
                {
                    bitmap = this.BmpInvert(bitmap, iw, ih);
                }
                return bitmap;
            }
        }

        public Bitmap CaptureScreenWithCurrentDpi(double x, double y, double width, double height)
        {
            //GetDpiRatio方法，请参阅：https://huchengv5.gitee.io/post/WPF-%E5%A6%82%E4%BD%95%E8%8E%B7%E5%8F%96%E7%B3%BB%E7%BB%9FDPI.html
            this._dpiRatio = GetDpiRatio(this);
            return CaptureScreen(x * _dpiRatio, y * _dpiRatio, width * _dpiRatio, height * _dpiRatio);
        }

        //获取当前dpi的缩放比
        public double GetDpiRatio(Window window)
        {
            var currentGraphics = Graphics.FromHwnd(new WindowInteropHelper(window).Handle);
            //96是100%的dpi
            return currentGraphics.DpiX / 96;
        }

        private Result BarScan(Bitmap bitmap)
        {
            var source = new BitmapLuminanceSource(bitmap);

            // using http://zxingnet.codeplex.com/
            // PM> Install-Package ZXing.Net
            if (this.barcodeReader == null)
            {
                this.barcodeReader = new BarcodeReader(null, null, ls => new GlobalHistogramBinarizer(ls))
                {
                    AutoRotate = true,
                    TryInverted = true,
                    Options = new DecodingOptions
                    {
                        TryHarder = true,
                        //PureBarcode = true,
                        /*PossibleFormats = new List<BarcodeFormat>
                        {
                            BarcodeFormat.CODE_128
                            //BarcodeFormat.EAN_8,
                            //BarcodeFormat.CODE_39,
                            //BarcodeFormat.UPC_A
                        }*/
                    }
                };
            }
            //var newhint = new KeyValuePair<DecodeHintType, object>(DecodeHintType.ALLOWED_EAN_EXTENSIONS, new Object());
            //reader.Options.Hints.Add(newhint);

            return barcodeReader.Decode(source);
        }

        /// <summary>
        /// 将图片进行反色处理
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        /// <returns>被反色后的图片</returns>
        public Bitmap BmpInvert(Bitmap mybm, int width, int height)
        {
            Bitmap bm = new Bitmap(width, height);//初始化一个记录处理后的图片的对象
            int x, y, resultR, resultG, resultB;
            System.Drawing.Color pixel;

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前坐标的像素值
                    resultR = 255 - pixel.R;//反红
                    resultG = 255 - pixel.G;//反绿
                    resultB = 255 - pixel.B;//反蓝
                    bm.SetPixel(x, y, System.Drawing.Color.FromArgb(resultR, resultG, resultB));//绘图
                }
            }

            return bm;//返回经过反色处理后的图片
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!this.textDialog.IsClosed)
            {
                this.textDialog.Close();
            }
        }
    }
}