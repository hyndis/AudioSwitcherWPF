using System.Threading.Tasks;
using System.Windows;

namespace AudioSwitcherWPF
{
    public partial class AudioOverlay : Window
    {
        public AudioOverlay(string message, int showMs = 1600)
        {
            InitializeComponent();
            txtMessage.Text = message;

            this.Loaded += (s, e) =>
            {
                var desktopWorkingArea = SystemParameters.WorkArea;
                this.Left = desktopWorkingArea.Right - this.ActualWidth - 10;
                this.Top = desktopWorkingArea.Bottom - this.ActualHeight - 10;

                _ = CloseAfterDelay(showMs); // ignorerar CS4014, OK för overlay
            };
        }

        private async Task CloseAfterDelay(int ms)
        {
            await Task.Delay(ms);
            Dispatcher.Invoke(() => this.Close());
        }
    }
}