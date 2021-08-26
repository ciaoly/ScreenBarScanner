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
                if(this.captureWindow != null)
                {
                    this.captureWindow.Invert = value;
                }
            }
        }

        private void Capture_Btn_Click(object sender, RoutedEventArgs e)
        {
            if(this.captureWindow == null)
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
    }
}
