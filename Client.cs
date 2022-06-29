
using TravelBot.Models;
using TravelBot.wish_list;

namespace TravelBot
{
    public class Client
    {
        public List<Element> wishList = new List<Element>();
        public  Hotels _hotels { get; set; }
        public  Restaurants _restaurants { get; set; }
        public  Places _places { get; set; }

        public string _city { get;  set; }
        public bool _chooseCity { get; set; } = false;

        public string checkinHotel { get; set; }
        public bool _checkinHotel { get; set; } = false;

        public string checkoutHotel { get; set; }
        public bool _checkoutHotel { get; set; } = false;

        public string adults_number { get; set; }
        public bool _adults_number { get; set; } = false;

        public string hotelsSortOrder { get; set; }
        public bool _hotelsSortOrder { get; set; } = false;

        public string limitRestaurants { get; set; }
        public bool _limitRestaurants { get; set; } = false;


        public string placeLon { get; set; }
        public string placeLat { get; set; }
        public string limitPlaces { get; set; }
        public bool _limitPlaces { get; set; } = false;

        public string ratingPlaces { get; set; }
        public bool _ratingPlaces { get; set; } = false;

        public bool _addPlacesElement { get; set; } = false;
        public bool _addRestaurantsElement { get; set; } = false;
        public bool _addHotelsElement { get; set; } = false;

        public bool _distancePlaces { get; set; } = false;
        public bool _distanceRestaurants { get; set; } = false;
        public bool _distanceHotels { get; set; } = false;
        public bool _distanceWishList { get; set; } = false;
        public bool _removeSomething { get; set; } = false;
        public bool _removeAll { get; set; } = false;
        public void AllFalse()
        {
            _addHotelsElement = false;
            _addPlacesElement = false;
            _addRestaurantsElement = false;
            _adults_number = false;
            _checkinHotel = false;
            _checkoutHotel = false;
            _chooseCity = false;
            _distanceHotels = false;
            _distancePlaces = false;
            _distanceRestaurants = false;
            _distanceWishList = false;
            _limitPlaces = false;
            _limitRestaurants = false;
            _ratingPlaces = false;
            _removeAll = false;
            _removeSomething = false;
            _adults_number = false;
        }
    }
}
