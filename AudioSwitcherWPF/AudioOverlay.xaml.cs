using System;
using System.Threading.Tasks;
using System.Windows;

namespace AudioSwitcherWPF
{
    public partial class AudioOverlay : Window
    {
        public AudioOverlay(string message)
        {
            InitializeComponent();
            txtMessage.Text = message;

            // Placera i nedre högra hörnet
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Loaded += (s, e) =>
            {
                this.Left = desktopWorkingArea.Right - this.ActualWidth - 10;
                this.Top = desktopWorkingArea.Bottom - this.ActualHeight - 10;
            };

            // Stäng overlay efter 2 sekunder
            this.Loaded += async (s, e) =>
            {
                await Task.Delay(2000);
                this.Close();
            };
        }
    }
}