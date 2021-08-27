using System.Windows;

namespace ScreenBarScanner
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CaptureWindow captureWindow = null;
        private bool _invert = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        public bool Invert
        {
            get => this._invert;
            set
            {
                this._invert = value;
                if (this.captureWindow != null)
                {
                    this.captureWindow.Invert = value;
                }
            }
        }

        private void Capture_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (this.captureWindow == null)
            {
                this.captureWindow = new CaptureWindow();
            }
            this.captureWindow.Invert = this._invert;
            this.captureWindow.Show();
        }

        private void Invert_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Invert = true;
        }

        private void Invert_CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            this.Invert = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.captureWindow != null)
            {
                try
                {
                    this.captureWindow.Close();
                }
                catch
                {

                }
            }
        }
    }
}
