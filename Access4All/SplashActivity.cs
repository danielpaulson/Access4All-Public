using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using Xamarin.Essentials;
using Android.Widget;

namespace Access4All
{
    [Activity(Theme = "@style/AppTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        List<Categories> group = new List<Categories>();

        public static List<Location> ALL_LOCATIONS = new List<Location>();

        public static List<Location> arts_and_entertainment_locations = new List<Location>();
        public static List<Location> automotive_locations = new List<Location>();
        public static List<Location> bank_and_finance_locations = new List<Location>();
        public static List<Location> education_locations = new List<Location>();
        public static List<Location> food_and_drink_locations = new List<Location>();
        public static List<Location> government_and_community_locations = new List<Location>();
        public static List<Location> healthcare_locations = new List<Location>();
        public static List<Location> news_and_media_locations = new List<Location>();
        public static List<Location> professional_services_locations = new List<Location>();
        public static List<Location> real_estate_locations = new List<Location>();
        public static List<Location> religion_locations = new List<Location>();
        public static List<Location> retail_locations = new List<Location>();
        public static List<Location> sports_and_recreation_locations = new List<Location>();
        public static List<Location> travel_locations = new List<Location>();
        public static List<Location> utilities_locations = new List<Location>();
        public static List<Location> other_locations = new List<Location>();

        /** these don't appear in the db, but they are on the website **/
        public static List<Location> business_locations = new List<Location>();
        public static List<Location> home_and_garden_locations = new List<Location>();
        public static List<Location> nightlife_locations = new List<Location>();
        public static List<Location> personal_services_locations = new List<Location>();
        public static List<Location> pet_locations = new List<Location>();
        public static List<Location> restaurant_and_coffee_shop_locations = new List<Location>();
        public static bool appState = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            if (savedInstanceState != null)
            {
                appState = savedInstanceState.GetBoolean("appState");
            }

            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
            var connectivity = Connectivity.NetworkAccess;
            if (connectivity == Xamarin.Essentials.NetworkAccess.Internet)
            {
                if (appState != true)
                {
                    setData();
                    appState = true;
                }
            }
        }
        

        async void SimulateStartup()
        {
            await Task.Delay(500);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        public override void OnBackPressed()
        {
            //Do Nothing
        }

        private void setData()
        {
            string data = GetData();
            JArray jsonArray = JArray.Parse(data);

            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];
                Location loc = new Location();
                loc.est_id = (int)json["est_id"];
                loc.website = (string)json["website"];
                loc.subtype = (string)json["subtype"];
                loc.name = (string)json["name"];
                loc.state = (string)json["state"];
                loc.date = (string)json["date"];
                loc.street = (string)json["street"];
                loc.city = (string)json["city"];
                loc.zip = (string)json["zip"];
                loc.phone = (string)json["phone"];
                loc.tty = (string)json["tty"];
                loc.contact_fname = (string)json["contact_fname"];
                loc.contact_lname = (string)json["contact_lname"];
                loc.contact_title = (string)json["contact_title"];
                loc.contact_email = (string)json["contact_email"];
                loc.user_id = (int)json["user_id"];
                loc.cat_id = (int)json["cat_id"];
                loc.config_id = (int)json["config_id"];
                loc.config_comment = (string)json["config_comment"];
                ALL_LOCATIONS.Add(loc);

                if ((int)json["cat_id"] == 1)
                    arts_and_entertainment_locations.Add(loc);
                else if ((int)json["cat_id"] == 2)
                    automotive_locations.Add(loc);
                else if ((int)json["cat_id"] == 3)
                    bank_and_finance_locations.Add(loc);
                else if ((int)json["cat_id"] == 4)
                    education_locations.Add(loc);
                else if ((int)json["cat_id"] == 5)
                    food_and_drink_locations.Add(loc);
                else if ((int)json["cat_id"] == 6)
                    government_and_community_locations.Add(loc);
                else if ((int)json["cat_id"] == 7)
                    healthcare_locations.Add(loc);
                else if ((int)json["cat_id"] == 8)
                    news_and_media_locations.Add(loc);
                else if ((int)json["cat_id"] == 9)
                    professional_services_locations.Add(loc);
                else if ((int)json["cat_id"] == 10)
                    real_estate_locations.Add(loc);
                else if ((int)json["cat_id"] == 11)
                    religion_locations.Add(loc);
                else if ((int)json["cat_id"] == 12)
                    retail_locations.Add(loc);
                else if ((int)json["cat_id"] == 13)
                    sports_and_recreation_locations.Add(loc);
                else if ((int)json["cat_id"] == 14)
                    travel_locations.Add(loc);
                else if ((int)json["cat_id"] == 15)
                    utilities_locations.Add(loc);
                else if ((int)json["cat_id"] == 16)
                    other_locations.Add(loc);
            }

            group.Add(new Categories("Arts, Entertainment, Culture", arts_and_entertainment_locations));
            group.Add(new Categories("Automotive", automotive_locations));
            group.Add(new Categories("Business Services", business_locations));//not in db
            group.Add(new Categories("Education", education_locations));
            group.Add(new Categories("Financial Services", bank_and_finance_locations));
            group.Add(new Categories("Food, Groceries", food_and_drink_locations));
            group.Add(new Categories("Public Services, Government", government_and_community_locations));
            group.Add(new Categories("Health, Medical, Dental, Mobility aids", healthcare_locations));
            group.Add(new Categories("Home & Garden", home_and_garden_locations));//not in db
            group.Add(new Categories("Mass Media, Printing, Publishing", news_and_media_locations));
            group.Add(new Categories("Nightlife", nightlife_locations));//not in db
            group.Add(new Categories("Recreation, Fitness", sports_and_recreation_locations));
            group.Add(new Categories("Personal Services", personal_services_locations));//not in db
            group.Add(new Categories("Pets", pet_locations));//not in db
            group.Add(new Categories("Professional Services", professional_services_locations));
            group.Add(new Categories("Religious Organizations", religion_locations));
            group.Add(new Categories("Restaurants, Coffee Shops", restaurant_and_coffee_shop_locations));//conflicts with food & grocery/ not in db
            group.Add(new Categories("Shopping", retail_locations));//probably
            group.Add(new Categories("Travel, Hotel, Motel", travel_locations));
        }

        private string GetData()
        {
            var request = HttpWebRequest.Create(string.Format(@"http://access4allspokane.org/RESTapi/establishment"));
            request.ContentType = "application/json";
            request.Method = "GET";

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            Console.Out.WriteLine("Response contained empty body...");
                        }
                        else
                        {
                            return content;
                        }
                    }
                }
            }
            catch (Exception e)
            { }
            return "NULL";
        }
    }
}