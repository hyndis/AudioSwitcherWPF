using System.Linq;
using System.Collections.Generic;
using System.Windows;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Threading.Tasks;

namespace AudioSwitcherWPF
{
    public partial class MainWindow : Window
    {
        // Global controller och cache
        public static CoreAudioController AudioCtrl { get; private set; } = new CoreAudioController();
        public static List<CoreAudioDevice>? CachedDevices { get; set; } // PUBLIC set fixar CS0272
        private static readonly object CacheLock = new();

        private bool isDevice1Active = true;

        public MainWindow()
        {
            InitializeComponent();
            Task.Run(() => PreloadDevices());
        }

        private void PreloadDevices()
        {
            try
            {
                var devices = AudioCtrl.GetDevices(DeviceType.Playback, DeviceState.Active).ToList();
                lock (CacheLock)
                {
                    CachedDevices = devices;
                }
            }
            catch
            {
                lock (CacheLock)
                {
                    CachedDevices = null;
                }
            }
        }

        public List<string> GetAudioDevices()
        {
            lock (CacheLock)
            {
                if (CachedDevices != null)
                    return CachedDevices.Select(d => d.FullName).ToList();
            }

            var devices = AudioCtrl.GetDevices(DeviceType.Playback, DeviceState.Active)
                                   .Select(d => d.FullName)
                                   .ToList();
            lock (CacheLock)
            {
                CachedDevices = AudioCtrl.GetDevices(DeviceType.Playback, DeviceState.Active).ToList();
            }
            return devices;
        }

        private async void btnToggle_Click(object sender, RoutedEventArgs e)
        {
            var settings = SettingsManager.Load();

            if (string.IsNullOrEmpty(settings.Device1) || string.IsNullOrEmpty(settings.Device2))
            {
                MessageBox.Show("Välj två ljudenheter i inställningarna först.");
                return;
            }

            string deviceToUse = isDevice1Active ? settings.Device2 : settings.Device1;

            CoreAudioDevice? device = null;
            lock (CacheLock)
            {
                if (CachedDevices != null)
                    device = CachedDevices.FirstOrDefault(d => d.FullName == deviceToUse);
            }

            if (device == null)
            {
                device = AudioCtrl.GetDevices(DeviceType.Playback, DeviceState.Active)
                                  .FirstOrDefault(d => d.FullName == deviceToUse);
            }

            if (device != null)
            {
                try
                {
                    await Task.Run(() => device.SetAsDefaultAsync().GetAwaiter().GetResult());

                    Task.Run(() =>
                    {
                        try
                        {
                            lock (CacheLock)
                            {
                                CachedDevices = AudioCtrl.GetDevices(DeviceType.Playback, DeviceState.Active).ToList();
                            }
                        }
                        catch { }
                    });

                    isDevice1Active = !isDevice1Active;

                    var overlay = new AudioOverlay($"Aktiv ljudenhet:\n{deviceToUse}");
                    overlay.Show();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Kunde inte byta ljudenhet:\n{ex.Message}");
                }
            }
            else
            {
                MessageBox.Show($"Enheten '{deviceToUse}' hittades inte.\nKontrollera stavning exakt som den visas i inställningarna.");
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }
    }
}