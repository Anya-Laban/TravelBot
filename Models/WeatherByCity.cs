namespace TravelBot.Models
{
    public class WeatherByCity
    {
        public List<List> list { get; set; }
    }
    public class List
    {
        public Temp Temp { get; set; }
        public double Humidity { get; set; }
        public List<Weather> Weather { get; set; }
        public double Gust { get; set; }
        public double Clouds { get; set; }
        public double Rain { get; set; }
    }
    public class Temp
    {
        public double Day { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Night { get; set; }
        public double Eve { get; set; }
        public double Morn { get; set; }
    }
    public class Weather
    {
        public string description { get; set; }
    }
}