using System.Text.Json;

namespace SensorEmitter
{
    public class SensorConfig
    {
        public int ID {get; set;}
        public string Type { get; set;}
        public int MinValue { get; set;}
        public int MaxValue { get; set;}
        public string EncoderType { get; set;}
        public int Frequency { get; set;}
    }

    public class SensorConfigList
    {
        public SensorConfig[] Sensors { get; set; }
    }

    public static class DeserializeConfig
    {
        public static string GetJsonFilePath()
        {
            string fileName = "sensorConfig.json";
            DirectoryInfo directoryPathInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent;
            string path = Path.Combine(directoryPathInfo.FullName, fileName);
            return path;
        }
        public static async Task<SensorConfigList> ReadAsync()
        {
            string filePath = GetJsonFilePath();
            using FileStream openStream = File.OpenRead(filePath);

            SensorConfigList? sensorConfigList = await JsonSerializer.DeserializeAsync<SensorConfigList>(openStream);

            return sensorConfigList;
        }
    }
}
