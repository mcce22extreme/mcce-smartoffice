namespace Mcce.SmartOffice.Api.Generators
{
    public interface IWeiGenerator
    {
        int GenerateWei(double temperature, double humidity, double co2Level);
    }

    public class WeiGenerator : IWeiGenerator
    {
        private static readonly IDictionary<int, int> _temperatureMap = new Dictionary<int, int>()
        {
            { 30, 20 },
            { 28, 40 },
            { 26, 60 },
            { 24, 80 },
            { 22, 100 },
            { 20, 70 },
            { 18, 60 },
            { 16, 10 },
        };

        private static readonly IDictionary<int, int> _co2Map = new Dictionary<int, int>()
        {
            { 1000, 10 },
            { 900, 20 },
            { 850, 40 },
            { 800, 50 },
            { 750, 60 },
            { 700, 80 },
            { 650, 90 },
            { 600, 100 },
        };

        private static readonly IDictionary<int, int> _humidityMap = new Dictionary<int, int>()
        {
            { 80, 10 },
            { 70, 40 },
            { 60, 60 },
            { 50, 80 },
            { 40, 100 },
            { 30, 80 },
            { 20, 50 },
            { 10, 10 },
        };

        public int GenerateWei(double temperature, double humidity, double co2Level)
        {
            var tempWei = GenerateTemperatureWei(temperature);
            var humidityWei = GenerateHumidityWei(humidity);
            var co2Wei = GenerateCo2Wei(co2Level);

            return (int)Math.Round((double)(tempWei + humidityWei + co2Wei) / 3);
        }

        private int GenerateTemperatureWei(double temperature)
        {
            var closestMatch = _temperatureMap.Keys.Aggregate((x, y) => Math.Abs(x - temperature) < Math.Abs(y - temperature) ? x : y);
            return _temperatureMap[closestMatch];
        }

        private int GenerateCo2Wei(double co2Level)
        {
            var closestMatch = _co2Map.Keys.Aggregate((x, y) => Math.Abs(x - co2Level) < Math.Abs(y - co2Level) ? x : y);
            return _co2Map[closestMatch];
        }

        private int GenerateHumidityWei(double humidity)
        {
            var closestMatch = _humidityMap.Keys.Aggregate((x, y) => Math.Abs(x - humidity) < Math.Abs(y - humidity) ? x : y);
            return _humidityMap[closestMatch];
        }
    }
}
