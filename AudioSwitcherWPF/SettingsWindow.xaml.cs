using System.Linq;
using System.Windows;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioSwitcherWPF
{
    public partial class SettingsWindow : Window
    {
        private SettingsData settings;

        public SettingsWindow()
        {
            InitializeComponent();

            var controller = new CoreAudioController();
            var devices = controller.GetDevices(DeviceType.Playback, DeviceState.Active)
                                    .Select(d => d.FullName)
                                    .ToList();

            comboBoxDevice1.ItemsSource = devices;
            comboBoxDevice2.ItemsSource = devices;

            settings = SettingsManager.Load();
            comboBoxDevice1.SelectedItem = settings.Device1;
            comboBoxDevice2.SelectedItem = settings.Device2;
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