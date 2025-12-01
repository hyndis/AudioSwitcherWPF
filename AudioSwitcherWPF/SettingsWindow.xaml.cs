using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AudioSwitcherWPF
{
    public partial class SettingsWindow : Window
    {
        private SettingsData settings;

        public SettingsWindow()
        {
            InitializeComponent();

            settings = SettingsManager.Load();

            comboBoxDevice1.SelectedItem = settings.Device1 ?? "";
            comboBoxDevice2.SelectedItem = settings.Device2 ?? "";

            LoadDevicesAsync();
        }

        private async void LoadDevicesAsync()
        {
            List<CoreAudioDevice> devices = MainWindow.CachedDevices ?? await Task.Run(() =>
                MainWindow.AudioCtrl.GetDevices(DeviceType.Playback, DeviceState.Active).ToList());

            MainWindow.CachedDevices = devices;

            var names = devices.Select(d => d.FullName).ToList();

            Dispatcher.Invoke(() =>
            {
                comboBoxDevice1.ItemsSource = names;
                comboBoxDevice2.ItemsSource = names;

                if (!string.IsNullOrEmpty(settings.Device1) && names.Contains(settings.Device1))
                    comboBoxDevice1.SelectedItem = settings.Device1;

                if (!string.IsNullOrEmpty(settings.Device2) && names.Contains(settings.Device2))
                    comboBoxDevice2.SelectedItem = settings.Device2;
            });
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            btnRefresh.IsEnabled = false;
            var devices = await Task.Run(() => MainWindow.AudioCtrl.GetDevices(DeviceType.Playback, DeviceState.Active).ToList());
            MainWindow.CachedDevices = devices;

            var names = devices.Select(d => d.FullName).ToList();
            comboBoxDevice1.ItemsSource = names;
            comboBoxDevice2.ItemsSource = names;
            btnRefresh.IsEnabled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            settings.Device1 = comboBoxDevice1.SelectedItem?.ToString() ?? "";
            settings.Device2 = comboBoxDevice2.SelectedItem?.ToString() ?? "";
            SettingsManager.Save(settings);
            this.Close();
        }
    }
}
