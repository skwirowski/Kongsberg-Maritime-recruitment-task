namespace SensorEmitter
{
    public class CreateMessageFromTelegram
    {
        public class SignalMeasurements()
        {
            public int signalValue { get; set; }
            public string signalQuality { get; set; }
        }
        public static int GenerateSignalValue(int MinValue, int MaxValue)
        {
            Random rnd = new Random();
            int signalValue = rnd.Next(MinValue, MaxValue + 1);

            return signalValue;
        }
        public static SignalMeasurements SignalClassifier(int MinValue, int MaxValue)
        {
            int singalValue = GenerateSignalValue(MinValue, MaxValue);
            int signalQualityRange = MaxValue - MinValue;
            decimal signalValueToRangeRatio = Math.Abs((decimal)singalValue / signalQualityRange * 100);
            string signalQuality = String.Empty;

            switch(signalValueToRangeRatio)
            {
                case <= 10:
                    signalQuality = "Alarm";
                    break;
                case <= 25:
                    signalQuality = "Warning";
                    break;
                case <= 75:
                    signalQuality = "Normal";
                    break;
                case <= 90:
                    signalQuality = "Warning";
                    break;
                default:
                    signalQuality = "Alarm";
                    break;
            }

            return new SignalMeasurements()
            {
                signalValue = singalValue,
                signalQuality = signalQuality
            };
        }
        public static string CreateMessage(SensorConfig sensorConfig)
        {
            int id = sensorConfig.ID;
            // Uppercase first character of Type string
            string type = String.Concat(sensorConfig.Type.Select((currentChar, index) => index == 0 ? Char.ToUpper(currentChar) : currentChar));

            SignalMeasurements signalMeasurements = SignalClassifier(sensorConfig.MinValue, sensorConfig.MaxValue);
            int value = signalMeasurements.signalValue;
            string quality = signalMeasurements.signalQuality;

            return $"$FIX, {id}, {type}, {value}, {quality}*";
        }
    }
}
