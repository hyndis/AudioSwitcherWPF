using System.Linq;
using System.Collections.Generic;
using System.Windows;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioSwitcherWPF
{
    public partial class MainWindow : Window
    {
        private bool isDevice1Active = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Toggle-knapp: växla mellan Device1 och Device2
        private async void btnToggle_Click(object sender, RoutedEventArgs e)
        {
            var settings = SettingsManager.Load();

            if (string.IsNullOrEmpty(settings.Device1) || string.IsNullOrEmpty(settings.Device2))
            {
                MessageBox.Show("Välj två ljudenheter i inställningarna först.");
                return;
            }

            string deviceToUse = isDevice1Active ? settings.Device2 : settings.Device1;

            var controller = new CoreAudioController();
            var device = controller.GetDevices(DeviceType.Playback, DeviceState.Active)
                                   .FirstOrDefault(d => d.FullName == deviceToUse);

            if (device != null)
            {
                try
                {
                    // Byt standardljudenhet
                    await device.SetAsDefaultAsync();
                    isDevice1Active = !isDevice1Active;

                    // Visa overlay längst ned till höger
                    var overlay = new AudioOverlay($"Ljudenhet: {deviceToUse}");
                    overlay.Show();

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Kunde inte byta ljudenhet:\n{ex.Message}");
                }
            }
            else
            {
                MessageBox.Show($"Enheten '{deviceToUse}' hittades inte.\nKontrollera stavning exakt som den visas i ljudenheter.");
            }
        }

        // Knapp för inställningar
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }
    }
}