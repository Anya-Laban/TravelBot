using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBot.wish_list
{
    public class Hotel_M: Element
    {
        public int id { get; set; }
        public string name { get; set; }
        public string thumbnailUrl { get; set; }
        public double starRating { get; set; }
        public Address address { get; set; }
        public WelcomeRewards welcomeRewards { get; set; }
        public GuestReviews guestReviews { get; set; }

        public List<Landmark> landmarks { get; set; }
        public RatePlan ratePlan { get; set; }
        public string neighbourhood { get; set; }
        public Deals deals { get; set; }
        public string pimmsAttributes { get; set; }
        public string providerType { get; set; }
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
        public string extendedAddress { get; set; }
        public string locality { get; set; }
        public string postalCode { get; set; }
        public string region { get; set; }
        public string countryName { get; set; }
        public string countryCode { get; set; }
        public bool obfuscate { get; set; }
    }
    public class WelcomeRewards
    {
        public bool collect { get; set; }
    }
    public class GuestReviews
    {
        public double unformattedRating { get; set; }
        public string rating
        {
            get
            {
                if (rating == null) return "Not found";
                else return rating;
            }
            set { }
        }
        public int total { get; set; }
        public int scale { get; set; }
    }
    public class Landmark
    {
        public string label { get; set; }
        public string distance { get; set; }
    }
    public class RatePlan
    {
        public Price price { get; set; }
        public Features features { get; set; }
        public string type { get; set; }
    }
    public class Price
    {
        public string current { get; set; }
        public double exactCurrent { get; set; }
        public string old { get; set; }
        public string info { get; set; }
        public string additionalInfo { get; set; }
        public string fullyBundledPricePerStay { get; set; }
    }
    public class Features
    {
        public bool freeCancellation { get; set; }
        public bool paymentPreference { get; set; }
        public bool noCCRequired { get; set; }
    }
    public class Deals
    {
        public SecretPrice secretPrice { get; set; }
        public string priceReasoning { get; set; }
    }
    public class SecretPrice
    {
        public string dealText { get; set; }
    }

}

