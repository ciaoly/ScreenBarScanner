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
using System.Windows.Shapes;

namespace ScreenBarScanner
{
    /// <summary>
    /// TextDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TextDialog : Window
    {
        private String _text;
        public bool IsClosed { get; private set; }

        public TextDialog()
        {
            InitializeComponent();
        }

        private void CopyBtnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_text);
        }

        public String text
        {
            get => _text;
            set
            {
                _text = value;
                TextBox.Text = value;
                TextBox.SelectAll();
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }

    }
}
