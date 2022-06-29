using System.Collections.Generic;

namespace TravelBot.Models
{
    public class Restaurants
    {
        public Results results { get; set; }
    }
    public class Results
    {
        public List<Data> Data { get; set; }
    }
    public class Data
    {
        //public Result_object Result_object { get; set; }
        public string Location_id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Location_string { get; set; }
        public string Ranking { get; set; }
        public string Rating { get; set; }
        public string Open_now_text { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public List<Cuisine> Cuisine { get; set; }
    }
    public class Cuisine
    {
        public string Name { get; set; }
    }
}
