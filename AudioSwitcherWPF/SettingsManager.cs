using System.IO;
using System.Text.Json;

namespace AudioSwitcherWPF
{
    public class SettingsData
    {
        public string Device1 { get; set; } = "";
        public string Device2 { get; set; } = "";
    }

    public static class SettingsManager
    {
        private static readonly string filePath = Path.Combine(
            System.AppContext.BaseDirectory, "settings.json");

        public static SettingsData Load()
        {
            if (!File.Exists(filePath))
                return new SettingsData();

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<SettingsData>(json) ?? new SettingsData();
        }

        public static void Save(SettingsData data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}