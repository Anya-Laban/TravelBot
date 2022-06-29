using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;
using Newtonsoft.Json;
using TravelBot.Models;
using TravelBot.wish_list;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace TravelBot
{
    public class Bot
    {
        private static string _token { get; set; } = "5500132738:AAE8M_rxEfI1NoYswO-SAUBtwO2b2fYLkEk";

        public Dictionary<long, Client> allClients = new Dictionary<long, Client>();
         
        TelegramBotClient botClient = new TelegramBotClient(_token);
        CancellationToken cancellationToken = new CancellationToken();
        ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };

        public async Task Start()
        {
            botClient.StartReceiving(HandlerUpdateAsync, HandlerError, receiverOptions, cancellationToken);
            var botMe = await botClient.GetMeAsync();
            Console.WriteLine($"Бот {botMe.Username} почав працювати");
            Console.ReadKey();

        }
        private Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Помилка в телеграм бот АПІ:\n {apiRequestException.ErrorCode}" +
                $"\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandlerMessage(botClient, update.Message);
                return;
            }
            if (update?.Type == UpdateType.CallbackQuery)
            {
                await HandlerCallbackQuery(botClient, update.CallbackQuery);
                return;
            }
        }
        private async Task HandlerCallbackQuery(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            Client client = null;
            for (int i = 0; i < allClients.Keys.Count; i++)
            {
                if (callbackQuery.Message.Chat.Id == allClients.Keys.ToList()[i])
                {
                    client = allClients[allClients.Keys.ToList()[i]];
                    break;
                }
                else continue;
            }
            if (client == null)
            {
                client = new Client();
                allClients.Add(callbackQuery.Message.Chat.Id, client);
            }

            if (callbackQuery.Data == "places")
            {
                 await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter the name of the place you want to add");
                client._addPlacesElement = true;
                return;
            }
            else if (callbackQuery.Data == "placesDistance")
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter the name of the place you want to know the distance");
                client._distancePlaces = true;
                return;
            }
            else if (callbackQuery.Data == "hotels")
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter the name of the hotel you want to add");
                client._addHotelsElement = true;
                return;
            }
            else if (callbackQuery.Data == "hotelsDistance")
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter the name of the hotel you want to know the distance");
                client._distanceHotels = true;
                return;
            }
            else if (callbackQuery.Data == "restaurants")
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter the name of the restaurant you want to add");
                client._addRestaurantsElement = true;
                return;
            }
            else if (callbackQuery.Data == "restaurantsDistance")
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter the name of the restaurant you want to know the distance");
                client._distanceRestaurants = true;
                return;
            }
            else if (callbackQuery.Data == "wishListDistance")
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter the name of the object in your wishlist you want to know the distance");
                client._distanceWishList = true;
                return;
            }
            else if (callbackQuery.Data == "removeSomething")
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter the name of the object in your wishlist you want to remove");
                client._removeSomething = true;
                return;
            }
            else if (client._hotelsSortOrder==true)
            {
                client._hotelsSortOrder = false;
                client.hotelsSortOrder = callbackQuery.Data;
               

                string Id = await HotelsID(client._city);
                if (Id == null || Id == "")
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Sorry, I didn't find the city you entered, press /start again to enter the city name");
                    return;
                }
                var hotels = await Hotels(Id, client);
                if (hotels != null)
                {
                    for (int i = 0; i < hotels.Count; i++)
                    {
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"{hotels[i]}");
                    }
                    InlineKeyboardMarkup keyboard = new(new[]
                       {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("add to wish list", "hotels"),
                            InlineKeyboardButton.WithCallbackData("look at the distance", "hotelsDistance" )
                        },
                    });
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Do you want to add a hotel from the list to your wish list or look at the distance to the hotel", replyMarkup: keyboard);
                    return;
                }
                else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Sorry, nothing was found for this query");
                return;
            }
        }
        private async Task HandlerMessage(ITelegramBotClient botClient, Message message)
        { 
            Client client = null;
            for (int i=0; i< allClients.Keys.Count; i++)
            {
                if (message.Chat.Id == allClients.Keys.ToList()[i])
                {
                    client = allClients[allClients.Keys.ToList()[i]];
                    break;
                }
                else continue;
            }
            if (client == null)
            {
                client = new Client();
                allClients.Add(message.Chat.Id, client);
            }
            
            if (message.Text == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Enter the city to travel");
                client._chooseCity = true;
                return;

            }
            else if (message.Type == MessageType.Text && client._chooseCity == true)
            {
                client._city = message.Text;
                client._chooseCity = false;
                await botClient.SendTextMessageAsync(message.Chat.Id, "To show the listed commands in the menu enter /keyboard:");
                return;
            }
            
            else if (message.Text == "/keyboard")
            {
                ReplyKeyboardMarkup keyboard = new(new[]
                {
                    new KeyboardButton[] { "Weather", "Hotels" },
                    new KeyboardButton[] { "Restaurants", "Interesting places" },
                     new KeyboardButton[] { "Your wishlist"  }
                })
                { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Select one of the listed commands in the menu:", replyMarkup: keyboard);
                return;
            }
            
            else if (message.Text == "Weather")
            {// працює норм
                string weather = await Weather(client._city);
                if (weather == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Sorry, I didn't find the city you entered, press /start again to enter the city name");
                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{weather}");
                    return;
                }

            }
            else if (message.Text == "Hotels")
            {
                client.AllFalse();
                 await botClient.SendTextMessageAsync(message.Chat.Id, $"Enter the check-in date in this formate: xx.xx (month-day)");
                client._checkinHotel = true;
                return;
            }
            else if (message.Text == "Restaurants")
            {//працює норм 
                client.AllFalse();
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Enter the number of restaurants you want to see");
                client._limitRestaurants = true;
                return;
            }
            else if (message.Text == "Interesting places")
            {
                client.AllFalse();
                CityPlaces placeId = await PlacesId(client._city);

                if (placeId == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Sorry, I didn't find the city you entered, press /start again to enter the city name");
                    return;
                }
                else
                {
                    client.placeLon = placeId.Lon;
                    client.placeLat = placeId.Lat;
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                   $"Enter the rating of the objects popularity," +
                   $"1 - minimum, 3 - maximum, h - object is referred to the cultural heritage.\n" +
                   $" Available values: 1, 2, 3, 1h, 2h, 3h\n" +
                   $" Objects from all categories are returned by default if you enter anything else.\n" );
                    client._ratingPlaces = true;
                    return;
                }

            }
            else if (message.Text == "Your wishlist")
            {
                client.AllFalse();
                client.wishList = null;
                if (client._city == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Please, enter the city name and try again");
                    client._chooseCity = true;
                    return;
                }
                client.wishList = await GetPlace(message.Chat.Id.ToString(), client._city);

                if (client.wishList == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Error");
                    return;
                }
                else if (client.wishList.Count == 0)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "You have not added anything to your wish list yet");
                    return;
                }
                else
                {
                    List<string> _wishList = await ShowWishList(client.wishList);
                    for (int i = 0; i < _wishList.Count; i++)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{_wishList[i]}");
                    }
                    InlineKeyboardMarkup keyboard = new(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("look at the distance", "wishListDistance" ),
                            InlineKeyboardButton.WithCallbackData("remove something from your wish list", "removeSomething"),
                        },
                    });
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Do you want to remove something from your wish list or look at the distance to something", replyMarkup: keyboard);
                    return;
                }
                return;
            }
           
            else if (message.Type == MessageType.Text && client._checkinHotel == true)
            {
                client._checkinHotel = false;

                string st = message.Text;
                Regex regex = new Regex(@"(\D)");
                client.checkinHotel = regex.Replace(st,"-");
               
               

                await botClient.SendTextMessageAsync(message.Chat.Id, $"Enter the check-out date in this formate: xx.xx (month-day)");
                client._checkoutHotel = true;
                return;
            }
            else if (message.Type == MessageType.Text && client._checkoutHotel == true)
            {
                client._checkoutHotel = false;

                string st = message.Text;
                Regex regex = new Regex(@"(\D)");
                client.checkoutHotel = regex.Replace(st, "-");

                await botClient.SendTextMessageAsync(message.Chat.Id, $"Enter the number of people to check into the hotel");
                client._adults_number = true;
                return;
            }
            else if (message.Type == MessageType.Text && client._adults_number == true)
            {
                client.adults_number = message.Text;
                client._adults_number = false;
                client._hotelsSortOrder = true;

                InlineKeyboardMarkup keyboard = new(new[]
                {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("bestseller", "BEST_SELLER"),
                            InlineKeyboardButton.WithCallbackData("price(highest first)", "PRICE_HIGHEST_FIRST" )
                        },
                       new[]
                       {
                            InlineKeyboardButton.WithCallbackData("star rating(highest first)", "STAR_RATING_HIGHEST_FIRST" ),
                            InlineKeyboardButton.WithCallbackData("star rating(lowest first)", "STAR_RATING_LOWEST_FIRST" )
                       }
                });
                await botClient.SendTextMessageAsync(message.Chat.Id, "Select the sort type of hotels", replyMarkup: keyboard);
                return;
            }

            else if (message.Type == MessageType.Text && client._addHotelsElement == true)
            {
                client._addHotelsElement = false;
                string searchHotel = message.Text;

                var result = await PostPlace(message.Chat.Id.ToString(), searchHotel, "hotel", client);
                botClient.SendTextMessageAsync(message.Chat.Id, result);
                return;
            }
            else if (message.Type == MessageType.Text && client._distanceHotels == true)
            {
                string searchHotel = message.Text;

                for (int i = 0; i < client._hotels.data.body.searchResults.results.Count; i++)
                {
                    if (searchHotel == client._hotels.data.body.searchResults.results[i].name)
                    {
                        message = await botClient.SendLocationAsync(
                        chatId: message.Chat.Id,
                        latitude: client._hotels.data.body.searchResults.results[i].Coordinate.lat,
                        longitude: client._hotels.data.body.searchResults.results[i].Coordinate.lon,
                        cancellationToken: cancellationToken);
                        client._distanceHotels = false;
                        return;
                    }
                }
                client._distanceHotels = false;
                botClient.SendTextMessageAsync(message.Chat.Id, "Sorry, I can't find such hotel in the list above, please check if you entered the name correctly.");
                return;
            }
            
            else if (message.Type == MessageType.Text && client._limitRestaurants == true)
            {

                client.limitRestaurants = message.Text;
                client._limitRestaurants = false;

                string restaurantsId = await RestaurantsID(client._city);
                if (restaurantsId == null || restaurantsId=="")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Sorry, I didn't find the city you entered, press /start again to enter the city name");
                    return;
                }
                var restaurants = await Restaurants(restaurantsId, client);
                if (restaurants == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Sorry, nothing was found for this query");
                    return;
                }
                for (int i = 0; i < restaurants.Count; i++)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{restaurants[i]}");
                }
                InlineKeyboardMarkup keyboard = new(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("add to wish list", "restaurants"),
                            InlineKeyboardButton.WithCallbackData("look at the distance", "restaurantsDistance" )
                        },
                    });
                await botClient.SendTextMessageAsync(message.Chat.Id, "Do you want to add a restaurant from the list to your wish list or look at the distance to the restaurant", replyMarkup: keyboard);
                return;
            }
           
            else if (message.Type == MessageType.Text && client._distanceRestaurants == true)
            {
                string searchRestaurant = message.Text;

                for (int i = 0; i < client._restaurants.results.Data.Count; i++)
                {
                    if (searchRestaurant == client._restaurants.results.Data[i].Name)
                    {
                        message = await botClient.SendLocationAsync(
                        chatId: message.Chat.Id,
                        latitude: client._restaurants.results.Data[i].Latitude,
                        longitude: client._restaurants.results.Data[i].Longitude,
                        cancellationToken: cancellationToken);
                        client._distanceRestaurants = false;
                        return;
                    }
                }
                client._distanceRestaurants = false;
                botClient.SendTextMessageAsync(message.Chat.Id, "Sorry, I can't find such restaurant in the list above, please check if you entered the name correctly.");
                return;
            }
            else if (message.Type == MessageType.Text && client._addRestaurantsElement == true)
            {
                client._addRestaurantsElement = false;
                string searchRestaurant = message.Text;

                var result = await PostPlace(message.Chat.Id.ToString(), searchRestaurant, "restaurant", client);
                botClient.SendTextMessageAsync(message.Chat.Id, result);
                return;
            }
           
            else if (message.Type == MessageType.Text && client._ratingPlaces == true)
            {
                client._ratingPlaces = false;
                if (message.Text == "1" || message.Text == "2" || message.Text == "3" || message.Text == "1h" || message.Text == "2h" || message.Text == "3h")
                {
                    client.ratingPlaces = message.Text;
                }
                else
                {
                    client.ratingPlaces = "0";
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, "Enter the number of interesting places you want to see");
                client._limitPlaces = true;
                return;
            }
            else if (message.Type == MessageType.Text && client._limitPlaces == true)
            {
                client.limitPlaces = message.Text;
                client._limitPlaces = false;
                string places = await Places(client);
                if (places == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Sorry, nothing was found for this query");
                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, places);

                    InlineKeyboardMarkup keyboard = new(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("add to wish list", "places"),
                            InlineKeyboardButton.WithCallbackData("look at the distance", "placesDistance" )
                        },
                    });
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Do you want to add an interesting place from the list to your wish list or look at the distance to the interesting place", replyMarkup: keyboard);
                }

                return;
            }
            
            else if (message.Type == MessageType.Text && client._distancePlaces == true)
            {
                string searchPlace = message.Text;

                for (int i = 0; i < client._places.features.Count; i++)
                {
                    if (searchPlace == client._places.features[i].properties.Name)
                    {
                        message = await botClient.SendLocationAsync(
                        chatId: message.Chat.Id,
                        latitude: client._places.features[i].geometry.Coordinates[1],
                        longitude: client._places.features[i].geometry.Coordinates[0],
                        cancellationToken: cancellationToken);
                        client._distancePlaces = false;
                        return;
                    }
                }
                client._distancePlaces = false;
                botClient.SendTextMessageAsync(message.Chat.Id, "Sorry, I can't find such place in the list above, please check if you entered the name correctly.");
                return;
            }
            else if (message.Type == MessageType.Text && client._addPlacesElement == true)
            {
                client._addPlacesElement = false;
                string searchPlace = message.Text;
                var result = await PostPlace(message.Chat.Id.ToString(), searchPlace, "place", client);

                botClient.SendTextMessageAsync(message.Chat.Id, result);
                return;
            }
            
            else if (message.Type == MessageType.Text && client._distanceWishList == true)
            {
                string searchPlace = message.Text;
                var el = FindElement(client.wishList, searchPlace);
                if (el == null) await botClient.SendTextMessageAsync(message.Chat.Id, "Sorry, I can't find such place in the list above, please check if you entered the name correctly.");
                else
                {
                    if (el is Hotel_M)
                    {
                        var hotelM = (Hotel_M)el;
                        message = await botClient.SendLocationAsync(chatId: message.Chat.Id, latitude: hotelM.Coordinate.lat, longitude: hotelM.Coordinate.lon, cancellationToken: cancellationToken);
                        client._distanceWishList = false;
                        return;
                    }
                    else if (el is Restaurant_M)
                    {
                        var restaurantM = (Restaurant_M)el;
                        message = await botClient.SendLocationAsync(chatId: message.Chat.Id, latitude: restaurantM.Latitude, longitude: restaurantM.Longitude, cancellationToken: cancellationToken);
                        client._distanceWishList = false;
                        return;
                    }
                    else if (el is Place_M)
                    {
                        var placeM = (Place_M)el;
                        message = await botClient.SendLocationAsync(chatId: message.Chat.Id, latitude: placeM.geometry.Coordinates[1], longitude: placeM.geometry.Coordinates[0], cancellationToken: cancellationToken);
                        client._distanceWishList = false;
                        return;
                    }
                }
                client._distanceWishList = false;
                return;
            }
            else if (message.Type == MessageType.Text && client._removeSomething == true)
            {
                client._removeSomething = false;
                string deletePlace = message.Text;
                string result = await DeletePlace(message.Chat.Id, client._city, deletePlace);
                await botClient.SendTextMessageAsync(message.Chat.Id, result);
                return;
            }
        }
        public Element FindElement(List<Element> wishList, string searchPlace)
        {
            for (int k = 0; k < wishList.Count; k++)
            {
                if (wishList[k] is Place_M)
                {
                    Place_M _place = (Place_M)wishList[k];
                    if(_place.properties.Name == searchPlace) return _place;
                }
                else if (wishList[k] is Restaurant_M)
                {
                    Restaurant_M _restaurant = (Restaurant_M)wishList[k];
                    if (_restaurant.Name == searchPlace) return _restaurant;
                }
                else if (wishList[k] is Hotel_M)
                {
                    Hotel_M _hotel = (Hotel_M)wishList[k];
                    if (_hotel.name == searchPlace) return _hotel;
                }
            }
            return null;
        }
        public async Task<string> DeletePlace(long Id, string city, string placeName)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.address);
                var response = await client.DeleteAsync($"/DeletePlace/{Id}?city={city}&name={placeName}");
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
        }
        public async Task<List<string>> ShowWishList(List<Element> wishList)
        {
            List<string> showWishList = new List<string>();
            for (int k = 0; k < wishList.Count; k++)
            {

                if (wishList[k] is Place_M)
                {
                    Place_M _place = (Place_M)wishList[k];
                    StringBuilder sb = new StringBuilder(_place.properties.Kinds);
                    sb.Replace('_', ' ');
                    string place =
                        $"The name of the interesting place: {_place.properties.Name}\n" +
                        $"  Kinds: {sb}\n" +
                        $"  Rating: {_place.properties.Rate}\n";
                    showWishList.Add(place);
                }
                else if (wishList[k] is Restaurant_M)
                {
                    Restaurant_M _restaurant = (Restaurant_M)wishList[k];

                    string cuisine = "";
                    for (int j = 0; j < _restaurant.Cuisine.Count; j++)
                    {
                        cuisine += $"{_restaurant.Cuisine[j].Name}, ";
                    }
                    string restaurant =
                    $"The name of the restaurant: {_restaurant.Name}\n" +
                    $"{_restaurant.Open_now_text}\n" +
                    $"Location: {_restaurant.Location_string}\n" +
                    $"Ranking: {_restaurant.Ranking}\n" +
                    $"Rating: {_restaurant.Rating}\n" +
                    $"Description: {_restaurant.Description} \n" +
                    $"Phone: {_restaurant.Phone} \n" +
                    $"Website: {_restaurant.Website}\n" +
                    $"Address: {_restaurant.Address}\n" +
                    $"Cuisine: {cuisine}";
                    showWishList.Add(restaurant);

                }
                else if (wishList[k] is Hotel_M)
                {
                    Hotel_M _hotel = (Hotel_M)wishList[k];
                    string landmarks = null;
                    for (int j = 0; j < _hotel.landmarks.Count; j++)
                    {
                        landmarks += $"\n{_hotel.landmarks[j].label} - {_hotel.landmarks[j].distance}, ";
                    }
                    string hotel =
                        $"The name of the hotel: {_hotel.name} \n" +
                        $"Star rating: {_hotel.starRating}\n" +
                        $"Adress: {_hotel.address.streetAddress} {_hotel.address.locality}\n" +
                        $"Price: {_hotel.ratePlan.price.current}\n" +
                        $"{_hotel.ratePlan.price.info}\n" +
                        $"Several plases nearby: {landmarks}\n";
                    showWishList.Add(hotel);
                }
            }
            return showWishList;
        }
        public async Task<string> PostPlace(string Id, string searchPlace, string name,  Client client)
        {
            string json = null;
            string nameObject = null;

            if (name == "place")
            {
                if (client._places == null) return null;
                for (int i = 0; i < client._places.features.Count; i++)
                {
                    if (searchPlace == client._places.features[i].properties.Name)
                    {
                        nameObject = client._places.features[i].properties.Name;
                        var el = client._places.features[i];
                        json = JsonConvert.SerializeObject(el);
                        break;
                    }
                }
            }
            else if (name == "restaurant")
            {
                if (client._restaurants == null) return null;
                for (int i = 0; i < client._restaurants.results.Data.Count; i++)
                {
                    if (searchPlace == client._restaurants.results.Data[i].Name)
                    {
                        nameObject = client._restaurants.results.Data[i].Name;
                        var el = client._restaurants.results.Data[i];
                        json = JsonConvert.SerializeObject(el);
                        break;
                    }
                }
                
            }
            else if (name == "hotel")
            {
                if (client._hotels == null) return null;
                for (int i = 0; i < client._hotels.data.body.searchResults.results.Count; i++)
                {
                    if (searchPlace == client._hotels.data.body.searchResults.results[i].name)
                    {
                        nameObject = client._hotels.data.body.searchResults.results[i].name;
                        var el = client._hotels.data.body.searchResults.results[i];
                        json = JsonConvert.SerializeObject(el);
                        break;
                    }
                }
            }

            if(json == null)
            {
                return "Sorry, I can't find such object in the list above, please check if you entered the name correctly.";
            }
            var url = $"{Constant.address}/PostPlace?idClient={Id}&name={name}&city={client._city}&nameObject={nameObject}";

            HttpClient httpClient = new HttpClient();
            string data = json;
            var content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            
            if (response.IsSuccessStatusCode)
            {
                return "Added sucessfully";
            }
            else return "Error";

        }
        public async Task<List<Element>> GetPlace(string Id, string city)
        {

            HttpClient _client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Constant.address}/GetPlace?idClient={Id}&city={city}"),

            };
            using (var response = await _client.SendAsync(request))
            {
               // response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();
                List<AddPlace> result = JsonConvert.DeserializeObject<List<AddPlace>>(body);
                if(result == null)
                {
                    return null;
                }
                 List<Element> wishList = new List<Element>();
                for(int i = 0; i< result.Count; i++)
                {
                    if (result[i]._name == "place")
                    {
                        Element place = JsonConvert.DeserializeObject<Place_M>(result[i]._placeName);
                        wishList.Add(place);
                    }
                    else if(result[i]._name == "hotel")
                    {
                        Element hotel = JsonConvert.DeserializeObject<Hotel_M>(result[i]._placeName);
                        wishList.Add(hotel);
                    }
                    else if (result[i]._name == "restaurant")
                    {
                        Element restaurant = JsonConvert.DeserializeObject<Restaurant_M>(result[i]._placeName);
                        wishList.Add(restaurant);
                    }
                }

                return wishList;
            }
        }
        public async Task<CityPlaces> PlacesId(string cityName)
        {

            HttpClient _client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Constant.address}/PlacesInCity?City={cityName}"),

            };
            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CityPlaces>(body);
                return result;
            }
        }
        public async Task<string> Places(Client client)
        {

            HttpClient _client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Constant.address}/Places?Max_Radius=1000&Lon={client.placeLon}&Lat={client.placeLat}&Rating={client.ratingPlaces}&Limit={client.limitPlaces}")
            };
            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                client._places = JsonConvert.DeserializeObject<Places>(body);
                if (client._places.features == null) return null;
                string places = null;
                for (int i = 0; i < client._places.features.Count; i++)
                {
                    StringBuilder sb = new StringBuilder(client._places.features[i].properties.Kinds);
                    sb.Replace('_', ' ');
                    string place =
                        $"\nName: {client._places.features[i].properties.Name}\n" +
                        $"  Kinds: {sb}\n" +
                        $"  Rating: {client._places.features[i].properties.Rate}\n";
                    places += place;
                }
                return places;
            }
        }
        public async Task<string> RestaurantsID(string cityName)
        {

            HttpClient _client = new HttpClient();
           
            string Id = null;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{Constant.address}/RestaurantsIdCity?City={cityName}"),
            };
            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                Id = await response.Content.ReadAsStringAsync();

                return Id;
            }

        }
        public async Task<List<string>> Restaurants(string cityID, Client client)
        {
            List<string> restaurants = new List<string>();
            HttpClient _client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{Constant.address}/Restaurants?Id={cityID}&Limit={client.limitRestaurants}"),
            };
            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                client._restaurants = JsonConvert.DeserializeObject<Restaurants>(body);
                
                if (client._restaurants == null)
                {
                    return restaurants = null;
                }
                else
                {
                    for (int i = 0; i < client._restaurants.results.Data.Count; i++)
                    {
                        string cuisine = "";
                        for (int j = 0; j < client._restaurants.results.Data[i].Cuisine.Count; j++)
                        {
                            cuisine += $"{client._restaurants.results.Data[i].Cuisine[j].Name}, ";
                        }
                        string restaurant =
                        $"Name: {client._restaurants.results.Data[i].Name}\n" +
                        $"{client._restaurants.results.Data[i].Open_now_text}\n" +
                        $"Location: {client._restaurants.results.Data[i].Location_string}\n" +
                        $"Ranking: {client._restaurants.results.Data[i].Ranking}\n" +
                        $"Rating: {client._restaurants.results.Data[i].Rating}\n" +
                        $"Description: {client._restaurants.results.Data[i].Description} \n" +
                        $"Phone: {client._restaurants.results.Data[i].Phone} \n" +
                        $"Website: {client._restaurants.results.Data[i].Website}\n" +
                        $"Address: {client._restaurants.results.Data[i].Address}\n" +
                        $"Cuisine: {cuisine.TrimEnd()}";
                        restaurants.Add(restaurant);
                    }
                    return restaurants;
                }

            }

        }
        public async Task<string> HotelsID(string cityName)
        {
            HttpClient _client = new HttpClient();
            string id= null;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Constant.address}/HotelsIdCity?City={cityName}")
            };
            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                id = JsonConvert.DeserializeObject<string>(body);
                return id;
            }

        }
        public async Task<List<string>> Hotels(string cityID,  Client client)
        {
            List<string> hotels = new List<string>();

            HttpClient _client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Constant.address}/Hotels?Id={cityID}&Checkin_date=2022-{client.checkinHotel}&Checkout_date=2022-{client.checkoutHotel}&Adults_number={client.adults_number}&SortOrder={client.hotelsSortOrder}")
            };
            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                string q = body;
                client._hotels = JsonConvert.DeserializeObject<Hotels>(body);

                if (client._hotels.data != null && client._hotels.data.body.searchResults.results.Count !=0)
                {
                    for (int i = 0; i < client._hotels.data.body.searchResults.results.Count; i++)
                    {
                        string landmarks = null;
                        for (int j = 0; j < client._hotels.data.body.searchResults.results[i].landmarks.Count; j++)
                        {
                            landmarks += $"\n{client._hotels.data.body.searchResults.results[i].landmarks[j].label} - {client._hotels.data.body.searchResults.results[i].landmarks[j].distance}, ";
                        }
                        string hotel =
                            $"\nName: {client._hotels.data.body.searchResults.results[i].name} \n\n" +
                            $"Star rating: {client._hotels.data.body.searchResults.results[i].starRating}\n" +
                            $"Adress: {client._hotels.data.body.searchResults.results[i].address.streetAddress} {client._hotels.data.body.searchResults.results[i].address.locality}\n" +
                            $"Price: {client._hotels.data.body.searchResults.results[i].ratePlan.price.current}\n" +
                            $"{client._hotels.data.body.searchResults.results[i].ratePlan.price.info}\n" +
                            $"Several plases nearby: {landmarks}\n";
                        hotels.Add(hotel);
                    }
                    return hotels;
                }
                else return null;

            }

        }
        public async Task<string> Weather(string cityName)
        {

            HttpClient _client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Constant.address}/GetWeather?City={cityName}")
            };
            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<WeatherByCity>(body);
                string weather = $"Weather in {cityName} for 7 days: (Day 1 - it's today)\n ";
                if (result == null)
                {
                    return weather = null;
                }

                string day_;
                for (int i = 0; i < result.list.Count; i++)
                {
                    day_ =
                        $"\n\nDay {i + 1}:" +
                        $"\nTemperature:\n" +
                        $"Min - {result.list[i].Temp.Min}\n" +
                        $"Max - {result.list[i].Temp.Max} \n" +
                        $"in the morning - {result.list[i].Temp.Morn}\n " +
                        $"in the afternoon - {result.list[i].Temp.Day}\n " +
                        $"in the evening - {result.list[i].Temp.Eve}\n " +
                        $"at night - {result.list[i].Temp.Night}\n" +
                        $" Description: {result.list[i].Weather[0].description}\n";
                    weather += day_;
                }
                return weather;
            }
        }
    }
}

