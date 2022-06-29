using System.Collections.Generic;

namespace TravelBot.Models
{
    public class Hotels
    {
        public string result { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public Body body { get; set; }
        }
       

    }
    public class Body
    {
        public SearchResults searchResults { get; set; }
    }
    public class SearchResults
    {
        public int totalCount { get; set; }
        public List<Result> results { get; set; }
    }
    public class Result
    {
        public string name { get; set; }
        public string thumbnailUrl { get; set; }
        public double starRating { get; set; }
        public Address address { get; set; }
 
        public List<Landmark> landmarks { get; set; }
        public RatePlan ratePlan { get; set; }
        public Coordinate Coordinate { get; set; }
    }
    public class Coordinate
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }
    public class Address
    {
        public string streetAddress { get; set; }
        public string locality { get; set; }

    }
    public class Landmark
    {
        public string label { get; set; }
        public string distance { get; set; }
    }
    public class RatePlan
    {
        public Price price { get; set; }
    }
    public class Price
    {
        public string current { get; set; }
        public string info { get; set; }
        public string additionalInfo { get; set; }
        public string totalPricePerStay { get; set; }
    }
}
