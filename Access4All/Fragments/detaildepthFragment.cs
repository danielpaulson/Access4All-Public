using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using Environment = System.Environment;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Android.Text.Util;
using Android.Content;

namespace Access4All.Fragments
{
    public class detaildepthFragment : Android.Support.V4.App.Fragment, MainActivity.IBackButtonListener
    {
        string curLocation = "";
        string selection = "";
        string prevView = "";
        List<Categories> group = new List<Categories>();
        TextView myTextTest = null;
        string table = "";
        int est_id = 0;
        int rest_id = 0;
        Button mapsButton = null;
        Button phoneButton = null;
        Button websiteButton = null;
        String PHONE = "";
        String WEBSITE = "";

        public override void OnCreate(Bundle savedInstanceState)
        {
            Bundle b = Arguments;
            curLocation = b.GetString("location");
            selection = b.GetString("selection");
            prevView = b.GetString("prevView");
            string test = curLocation + " " + selection;
            MainActivity act = (MainActivity)this.Activity;
            
            

            base.OnCreate(savedInstanceState);
        }

        public static detaildepthFragment NewInstance()
        {
            var detaildepthfrag = new detaildepthFragment { Arguments = new Bundle() };
            return detaildepthfrag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.detail_layout, null);
            TextView t = (TextView)v.FindViewById(Resource.Id.textView1);
            myTextTest = (TextView)v.FindViewById(Resource.Id.textView1);
            mapsButton = v.FindViewById<Button>(Resource.Id.mapsButton);
            phoneButton = v.FindViewById<Button>(Resource.Id.phoneButton);
            websiteButton = v.FindViewById<Button>(Resource.Id.websiteButton);
            
            
            mapsButton.Visibility = ViewStates.Invisible;
            mapsButton.Enabled = false;

            phoneButton.Visibility = ViewStates.Invisible;
            phoneButton.Enabled = false;

            websiteButton.Visibility = ViewStates.Invisible;
            websiteButton.Enabled = false;

            string unparsedData,parsedData;
            Bundle b = Arguments;
            curLocation = b.GetString("location");
            selection = b.GetString("selection");
            string test = curLocation + " " + selection;
            
            for (int i = 0; i < SplashActivity.ALL_LOCATIONS.Count; i++)
            {
                if (curLocation.CompareTo(SplashActivity.ALL_LOCATIONS[i].name) == 0)
                    est_id = SplashActivity.ALL_LOCATIONS[i].est_id;
            }

            
            if (selection.CompareTo("Information") == 0)
            {
                
                table = "establishment";
                unparsedData = GetData("");
                parsedData = parseGeneralInformation(unparsedData, curLocation);
                t.Text = parsedData;
                mapsButton.Visibility = ViewStates.Visible;
                mapsButton.Enabled = true;
                mapsButton.Click += async delegate
                {
                    await getGeolocationAsync(unparsedData);
                };
                phoneButton.Click += delegate {
                    var uri = Android.Net.Uri.Parse("tel:"+PHONE);
                    var intent = new Intent(Intent.ActionDial, uri);
                    StartActivity(intent);
                };
                websiteButton.Click += delegate
                {
                    var uri = Android.Net.Uri.Parse(WEBSITE);
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                };
            }

            else if (selection.CompareTo("Parking on street") == 0)
            {
                table = "parking";
                unparsedData = GetData(est_id.ToString());
                parsedData = parseParkingInformation(unparsedData, curLocation);
                t.Text = parsedData;
            }

            else if (selection.CompareTo("Access to transit") == 0)
            {
                table = "parking"; 
                unparsedData = GetData(curLocation);
                parsedData = parseTransitData(unparsedData);
                t.Text = parsedData;
            }

            else if (selection.CompareTo("Exterior pathway") == 0)
            {
                
                table = "exterior_pathways";
                unparsedData = GetData(table);
                parsedData = parseExteriorData(unparsedData);
                t.Text = parsedData;
            }

            else if (selection.CompareTo("Entrances") == 0)
            {
                table = "main_entrance";
                unparsedData = GetData(curLocation);
                parsedData = parseMainEntrance(unparsedData, curLocation);
                t.Text = parsedData;
                
            }

            else if (selection.CompareTo("Elevators") == 0)
            {
                table = "elevator";
                unparsedData = GetData(curLocation);
                parsedData = parseElevators(unparsedData);
                t.Text = parsedData;
            }

            else if (selection.CompareTo("Interior") == 0)
            {
                table = "interior";
                unparsedData = GetData(table);
                parsedData = parseInterior(unparsedData);
                t.Text = parsedData;
                
            }

            else if (selection.CompareTo("Seating") == 0)
            {
                table = "seating";
                unparsedData = GetData(table);
                parsedData = parseSeating(unparsedData);
                t.Text = parsedData;
            }

            else if (selection.CompareTo("Restroom") == 0)
            {
                table = "restroom";
                unparsedData = GetData(table);
                parsedData = parseRestroom(unparsedData);
                table = "restroom_info";
                unparsedData = GetData(table);
                parsedData += parseRestroomInfo(unparsedData);
                t.Text = parsedData;
            }

            else if (selection.CompareTo("Communication, Technologies & Cust. Svc.") == 0)
            {
                table = "communication";
                unparsedData = GetData(table);
                parsedData = parseCommunication(unparsedData);
                t.Text = parsedData;
                
            }
            t.Enabled = true;
            return v;   
        }


        private async Task getGeolocationAsync(string unparsedData)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string loc = "";
            string name = "";
            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["est_id"]).Equals(est_id))
                {
                    name = (string)json["name"];
                    loc = (((string)json["street"]) + " " + ((string)json["city"]) + ", " + ((string)json["state"]) + " " + ((string)json["zip"]));
                }
            }
            await OpenMaps(name, loc);
        }

        public async Task OpenMaps(string name, string loc)
        {
            Xamarin.Essentials.Location location1 = new Xamarin.Essentials.Location();
            try
            {
                var address = loc;
                var locations = await Geocoding.GetLocationsAsync(address);

                location1 = locations?.FirstOrDefault();
            }
            catch (FeatureNotSupportedException)
            {
                Toast.MakeText(MainActivity.activity, "Feature not supported on this device", ToastLength.Long).Show();
            }
            catch (Exception)
            {
                Toast.MakeText(MainActivity.activity, "Error parsing location", ToastLength.Long).Show();
            }
            var location = new Xamarin.Essentials.Location(location1.Latitude, location1.Longitude);
            var options = new MapLaunchOptions { Name = name };
            await Map.OpenAsync(location, options);
        }

        private string parseRestroomInfo(string unparsedData)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string data = "";

            string debugValue;

            string restroom_desc = "";
            string easy_open = "";
            int? lbs_force = 0;
            string clearing = "";
            double? opening = 0;
            string opens_out = "";
            string use_fist = "";
            string can_turn_around = "";
            double? turn_width = 0;
            double? turn_depth = 0;
            string close_chair_inside = "";
            string grab_bars = "";
            string seat_height_req = "";
            double? seat_height = 0;
            string flush_auto_fist = "";
            string ambulatory_accessible = "";
            double? bar_height = 0;
            string coat_hook = "";
            double? hook_height = 0;
            string sink = "";
            double? sink_height = 0;
            string faucet = "";
            double? faucet_depth = 0;
            string faucet_auto_fist = "";
            string sink_clearance = "";
            double? sink_clearance_height = 0;
            string sink_pipes = "";
            string soap_dispenser = "";
            double? soap_height = 0;
            string dry_fist = "";
            int? dry_control_height = 0;
            string mirror = "";
            double? mirror_height = 0;
            string shelves = "";
            double? shelf_height = 0;
            string trash_receptacles = "";
            string hygiene_seat_cover = "";
            double? hygiene_cover_height = 0;
            string lighting = "";
            string lighting_type = "";
            string comment = "";


            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["rest_id"]) == rest_id)
                {
                    data += "The Restroom in this location has the following information: \n\r\n\r";
                    restroom_desc = (string)json["restroom_desc"];
                    if (restroom_desc.ToLower().CompareTo("") != 0)
                    {
                        if (restroom_desc.ToLower().Equals("unisex"))
                            restroom_desc = "unisex/family";
                        data += "• This restroom is " + restroom_desc + "\n\r\n\r";
                    }
                    easy_open = (string)json["easy_open"];
                    if (easy_open.ToLower().CompareTo("yes") == 0)
                        data += "• This restroom door is easy to open.";

                    if (!json["lbs_force"].Equals(null))
                    {
                        debugValue = (string)json["lbs_force"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                        {
                            lbs_force = (int)json["lbs_force"];
                            data += " Requiring " + lbs_force + " lbs or less force to open. \n\r\n\r";
                        }
                        else
                            data += "\n\r\n\r";
                    }

                    clearing = (string)json["clearance"];
                    if (clearing.ToLower().CompareTo("yes") == 0)
                        data += "• Door has at least 32” clearance when open 90 degrees. \n\r\n\r";

                    if (!json["opening"].Equals(null))
                    {
                        debugValue = (string)json["opening"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                            opening = (double)json["opening"];
                    }

                    opens_out = (string)json["opens_out"];
                    if (opens_out.ToLower().CompareTo("yes") == 0)
                        data += "• Stall door opens away from you.\n\r\n\r";
                    else
                        data += "• Stall door opens towards you.\n\r\n\r";

                    use_fist = (string)json["use_fist"];
                    if (use_fist.ToLower().CompareTo("yes") == 0)
                        data += "• Doors, handles, and levers can be operated with a closed fist. \n\r\n\r";
                    //Not sure what to say with turn around, width and depth
                    can_turn_around = (string)json["can_turn_around"];
                    if(can_turn_around.ToLower().Equals("yes"))
                    {
                        data += "• The stall or room is large enough for a wheelchair or walker to turn around with at least 60 inches wide and 56 inches deep.\n\r\n\r";
                    }

                    if (!json["turn_width"].Equals(null))
                    {
                        debugValue = (string)json["turn_width"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                        {
                            turn_width = (double)json["turn_width"];
                            data += "• Actual width: " + turn_width + " inches\n\r\n\r";
                        }
                    }
                    if (!json["turn_depth"].Equals(null))
                    {
                        debugValue = (string)json["turn_depth"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                        {
                            turn_depth = (double)json["turn_depth"];
                            data += "• Actual depth: " + turn_depth+ " inches\n\r\n\r";
                        }
                    }

                    close_chair_inside = (string)json["close_chair_inside"];
                    if (close_chair_inside.ToLower().CompareTo("yes") == 0)
                        data += "• Room door can be closed once a wheelchair is inside. \n\r\n\r";

                    grab_bars = (string)json["grab_bars"];
                    if (grab_bars.ToLower().CompareTo("yes") == 0)
                        data += "• Grab bars are easily reachable behind toilet and on side wall nearest toilet. \n\r\n\r";
                    seat_height_req = (string)json["seat_height_req"];
                    if (seat_height_req.ToLower().CompareTo("yes") == 0)
                        data += "• Height of toilet seat is at least 17” from the floor. \n\r\n\r";


                    if (!json["seat_height"].Equals(null))
                    {
                        debugValue = (string)json["seat_height"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                            seat_height = (double)json["seat_height"];
                    }
                    //seat heigh req covers this section

                    flush_auto_fist = (string)json["flush_auto_fist"];
                    if (flush_auto_fist.ToLower().CompareTo("yes") == 0)
                        data += "• Toilet flushes automatically or can be flushed with a closed fist. \n\r\n\r";
                    ambulatory_accessible = (string)json["ambulatory_accessible"];
                    if(ambulatory_accessible.ToLower().Equals("yes"))
                    {
                        data += "• At least one stall is abulatory accessible.\n\r\n\r";
                    }
                    if (!json["bar_height"].Equals(null))
                    {
                        debugValue = (string)json["bar_height"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                        {
                            bar_height = (double)json["bar_height"];
                        }
                    }

                    coat_hook = (string)json["coat_hook"];
                    if (coat_hook.ToLower().CompareTo("yes") == 0)
                        data += "• Restroom has a coat hook available. \n\r\n\r";

                    if (!json["hook_height"].Equals(null))
                    {
                        debugValue = (string)json["hook_height"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                        {
                            hook_height = (double)json["hook_height"];
                            if(hook_height<=48 && hook_height>=35)
                            data += "• The coat hook is between 35 inches and 48 inches from the floor. \n\r\n\r";
                        }
                    }

                    sink = (string)json["sink"];
                    if(sink.ToLower().Equals("yes"))
                    {
                        data += "• The height of the sink/countertop is 34 inches or less from the front edge of the sink counter. \n\r\n\r";
                    }

                    if (!json["sink_height"].Equals(null))
                    {
                        debugValue = (string)json["sink_height"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                            sink_height = (double)json["sink_height"];
                    }

                    faucet = (string)json["faucet"];
                    if(faucet.ToLower().Equals("yes"))
                    {
                        data += "• The faucet control is 17 inches or less from the front edge of the sink counter. \n\r\n\r";
                    }


                    if (!json["faucet_depth"].Equals(null))
                    {
                        debugValue = (string)json["faucet_depth"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                            faucet_depth = (double)json["faucet_depth"];
                    }

                    faucet_auto_fist = (string)json["faucet_auto_fist"];
                    if (faucet_auto_fist.ToLower().CompareTo("yes") == 0)
                        data += "• The faucets are automatic or can be operated with a closed fist. \n\r\n\r";

                    sink_clearance = (string)json["sink_clearance"];
                    if (sink_clearance.ToLower().CompareTo("yes") == 0)
                        data += "• There is room for a wheelchair to roll under the sink. \n\r\n\r";

                    if (!json["sink_clearance_height"].Equals(null))
                    {
                        debugValue = (string)json["sink_clearance_height"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                            sink_clearance_height = (double)json["sink_clearance_height"];
                    }

                    sink_pipes = (string)json["sink_pipes"];
                    if (sink_pipes.ToLower().CompareTo("yes") == 0)
                        data += "• All pipes under the sink are covered to prevent injury or burns. \n\r\n\r";

                    soap_dispenser = (string)json["soap_dispenser"];
                    if (soap_dispenser.ToLower().CompareTo("yes") == 0)
                        data += "• Height of soap dispenser control is no more than 48” from floor, and can be reached from a chair. \n\r\n\r";

                    if (!json["soap_height"].Equals(null))
                    {
                        debugValue = (string)json["soap_height"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                            soap_height = (double)json["soap_height"];
                    }

                    dry_fist = (string)json["dry_fist"];
                    if (dry_fist.ToLower().CompareTo("yes") == 0)
                        data += "• Hand dryer or towel dispensers can be operated automatically or with closed fist. \n\r\n\r";

                    if (!json["dry_control_height"].Equals(null))
                    {
                        debugValue = (string)json["dry_control_height"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                        {
                            dry_control_height = (int)json["dry_control_height"];
                            if(dry_control_height<=48)
                                data += "• Controls for hand dryer or towel dispenser are 48 inches or less from the floor. \n\r\n\r";
                        }
                    }



                    mirror = (string)json["mirror"];
                    if (mirror.ToLower().CompareTo("yes") == 0)
                        data += "• The bottom edge of the lowest mirror is a maximum of 40” from the floor. \n\r\n\r";

                    if (!json["mirror_height"].Equals(null))
                    {
                        debugValue = (string)json["mirror_height"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                            mirror_height = (double)json["mirror_height"];
                    }

                    shelves = (string)json["shelves"];
                    if(shelves.ToLower().Equals("yes"))
                    {
                        data += "• Shelves are a maximum of 48 inches from the floor. \n\r\n\r";
                    }

                    if (!json["shelf_height"].Equals(null))
                    {
                        debugValue = (string)json["shelf_height"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                            shelf_height = (double)json["shelf_height"];
                    }

                    trash_receptacles = (string)json["trash_receptacles"];
                    if (trash_receptacles.ToLower().CompareTo("yes") == 0)
                        data += "• Trash receptacles are positioned so they do not block the route of the door. \n\r\n\r";

                    hygiene_seat_cover = (string)json["hygiene_seat_cover"];
                    if(hygiene_seat_cover.ToLower().Equals("yes"))
                    {
                        data += "• Feminine hygiene products and toilet seat cover dispensers are 48 inches or less from the floor. \n\r\n\r";
                    }

                    if (!json["hygiene_cover_height"].Equals(null))
                    {
                        debugValue = (string)json["hygiene_cover_height"];
                        if (debugValue != null && debugValue.ToLower().CompareTo("n/a") != 0 && debugValue.ToLower().CompareTo("") != 0)
                            hygiene_cover_height = (double)json["hygiene_cover_height"];
                    }

                    lighting = (string)json["lighting"];
                    lighting_type = (string)json["lighting_type"];
                    if (lighting.ToLower().CompareTo("yes") == 0)
                        data += "• Lighting level is " + lighting_type + ", and is adequate for mobility and reading signs. \n\r\n\r";

                    comment = (string)json["comment"];
                }
            }
            return data;
        }

        private string parseRestroom(string unparsedData)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string data = "";

            string public_restroom = "";
            int? total_num = -1;
            int? designated_number = -1;
            int? num_wheelchair_sign = -1;
            string sign_accessible = "";
            string sign_location = "";
            string key_needed = "";
            string comment = "";
            string debugValue = "";


            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["est_id"]) == est_id)
                {
                    public_restroom = (string)json["public_restroom"];
                    this.rest_id = (int)json["restroom_id"];
                    if (!json["total_num"].Equals(null))
                    {
                        debugValue = (string)json["total_num"];
                        if (debugValue != null)
                            total_num = (int)json["total_num"];
                    }
                    if (!json["designated_number"].Equals(null))
                    {
                        debugValue = (string)json["designated_number"];
                        if (debugValue != null)
                            designated_number = (int)json["designated_number"];
                    }
                    if (!json["num_wheelchair_sign"].Equals(null))
                    {
                        debugValue = (string)json["num_wheelchair_sign"];
                        if (debugValue != null)
                            num_wheelchair_sign = (int)json["num_wheelchair_sign"];
                    }
                    sign_accessible = (string)json["sign_accessable"];
                    sign_location = (string)json["sign_location"];
                    key_needed = (string)json["key_needed"];
                    comment = (string)json["comment"];

                    if(public_restroom != null && public_restroom.ToLower().CompareTo("yes")==0)
                    {
                        data += "• There is "+ total_num +" public restroom(s) in the establishment" + "\n\r\n\r";
                    }
                    if(designated_number>1)
                    {
                        data += "• The number of restrooms designated family, unisex, or assisted use is " + designated_number + "\n\r\n\r";
                    }
                    if(num_wheelchair_sign != -1)
                    {
                        data += "• The number of restrooms with a wheelchair sign is "+ num_wheelchair_sign + ". \n\r\n\r";
                    }
                    if(sign_accessible != null && sign_accessible.ToLower().CompareTo("yes")==0)
                    {
                        data += "• This restroom has"+ num_wheelchair_sign +" ‘wheelchair accessible’ sign(s)" + "\n\r\n\r";
                    }
                    if(sign_location != null && sign_location.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Signage is on latch sign of door between 48 and 60 inches of floor\n\r\n\r";
                    }
                    if(key_needed != null && key_needed.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Users need to ask someone for a key to use the restroom" + "\n\r\n\r";
                    }
                    if (key_needed != null && key_needed.ToLower().CompareTo("no") == 0)
                    {
                        data += "• Users do not need to ask someone for a key to use the restroom" + "\n\r\n\r";
                    }
                    if(comment != null)
                    {
                        data += "• " + comment + ". \n\r\n\r";
                    }
                }
            }
            return data;
        }

        private string parseSeating(string unparsedData)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string data = "";

            int seating_id = 0;
            string seating_no_step = "";
            string table_aisles = "";
            string legroom = "";
            string num_legroom = ""; // some entries are empty and parsing int will break
            string rearranged = "";
            string num_table_rearranged = ""; // string because it can be "all" in database
            string num_chair_rearranged = ""; //same as above
            string round_tables = "";
            int num_round_tables = 0;
            string lighting = "";
            string lighting_option = "";
            string lighting_type = "";
            string adjustible_lighting = "";
            string low_visual_slim = "";
            string quiet_table = "";
            string low_sound = "";
            string designated_space = "";
            int num_desig_space = 0;
            string companion_space = "";
            string comment = "";

            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["est_id"]) == est_id)
                {
                    seating_id = (int)json["seating_id"];
                    seating_no_step = (string)json["seating_no_step"];
                    table_aisles = (string)json["table_aisles"];
                    legroom = (string)json["legroom"];
                    num_legroom = (string)json["num_legroom"];
                    num_table_rearranged = (string)json["num_table_rearranged"];
                    num_chair_rearranged = (string)json["num_chair_rearranged"];
                    rearranged = (string)json["rearranged"];
                    round_tables = (string)json["round_tables"];
                    num_round_tables = (int)json["num_round_tables"];
                    lighting = (string)json["lighting"];
                    lighting_type = (string)json["lighting_type"];
                    lighting_option = (string)json["lighting_option"];
                    adjustible_lighting = (string)json["adjustible_lighting"];
                    low_visual_slim = (string)json["low_visual_slim"];
                    quiet_table = (string)json["quiet_table"];
                    low_sound = (string)json["low_sound"];
                    designated_space = (string)json["designated_space"];
                    num_desig_space = (int)json["num_desig_space"];
                    companion_space = (string)json["companion_space"];
                    comment = (string)json["comment"];

                    if (seating_no_step.ToLower().CompareTo("yes") == 0)
                    {
                        data += "• One or more seating areas in the common area can be accessed without steps. \n\r\n\r";

                        if (table_aisles.ToLower().CompareTo("yes") == 0)
                        {
                            data += "• Customers can maneuver between tables without bumping into chairs.  \n\r\n\r";
                        }

                        if (legroom.ToLower().CompareTo("yes") == 0)
                        {
                            data += "• There are tables with legroom for wheelchair users.";
                            if (num_legroom.ToLower().CompareTo("") != 0 && num_legroom.CompareTo("0") != 0)
                                data += " (" + num_legroom + ")";

                            data += "\n\r\n\r";


                        }
                        if (rearranged != null)
                        {
                            if (rearranged.ToLower().CompareTo("yes") == 0)
                            {
                                if (num_table_rearranged.ToLower().CompareTo("all") == 0)
                                    data += "• All tables can be moved or rearranged.  \n\r\n\r";
                                else if (num_table_rearranged != null && num_chair_rearranged.CompareTo(" ") != 0 && num_table_rearranged.CompareTo("0") != 0)
                                    data += "• " + num_table_rearranged + " tables can be moved or rearranged.  \n\r\n\r";
                                if (num_chair_rearranged.ToLower().CompareTo("all") == 0)
                                    data += "• All chairs can be moved or rearranged.  \n\r\n\r";
                                else if (num_chair_rearranged != null && num_chair_rearranged.CompareTo(" ") != 0 && num_chair_rearranged.CompareTo("0") != 0)
                                    data += "• " + num_chair_rearranged + " chairs can be moved or rearranged.  \n\r\n\r";

                            }
                        }

                        if (lighting != null)
                        {
                            if (lighting.ToLower().CompareTo("yes") == 0)
                            {
                                if (lighting_option.ToLower().CompareTo("day") == 0 && lighting_option.CompareTo(" ") != 0)
                                    data += "• Lighting level is " + lighting_type + " in daytime, and is adequate for mobility and reading menu / program \n\r\n\r";
                                else if (lighting_option.ToLower().CompareTo("night") == 0 && lighting_option.CompareTo(" ") != 0)
                                    data += "• Lighting level is " + lighting_type + " in daytime, and is adequate for mobility and reading menu / program \n\r\n\r";
                                else //else it's N/A
                                    data += "• Lighting level is is adequate for mobility and reading menu / program \n\r\n\r";
                            }
                        }
                        if (adjustible_lighting != null && adjustible_lighting.CompareTo(" ") != 0)
                        {
                            if (adjustible_lighting.ToLower().CompareTo("yes") == 0)
                            {
                                data += "• One or more spaces have adjustable lighting. \n\r\n\r";
                            }
                        }
                        if (low_visual_slim != null && low_visual_slim.CompareTo(" ") != 0)
                        {
                            if (low_visual_slim.ToLower().CompareTo("yes") == 0)
                            {
                                data += "• There are one or more areas with low visual stimulation. \n\r\n\r";
                            }
                        }
                        if (low_sound != null && low_sound.CompareTo(" ") != 0)
                        {
                            if (low_sound.ToLower().CompareTo("yes") == 0)
                            {
                                data += "• There are one or more areas with low or no background sound, and / or sound-absorbing surfaces. \n\r\n\r";
                            }
                        }
                        if (companion_space != null && companion_space.CompareTo(" ") != 0)
                        {
                            if (companion_space.ToLower().CompareTo("yes") == 0)
                            {
                                data += "• There are spaces for companions to sit next to the wheelchair users. \n\r\n\r";
                            }

                        }
                        if(quiet_table != null && quiet_table.ToLower().Equals("yes"))
                        {
                            data += "• There is a quiet table, room or area available on request. \n\r\n\r";
                        }

                        if (comment.CompareTo("") != 0)
                        {
                            data += "• " + comment + "\n\r\n\r";
                        }
                    }
                }

            }
                return data;
        }
        

        private string parseElevators(string unparsedData)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string data = "";

            string is_elevator = "";
            string location = "";
            string works = "";
            string no_assist = "";
            string button_height = "";
            string outside_btn_height = "";
            string inside_btn_height = "";
            string button_use_fist = "";
            string braille = "";
            string audible_tones = "";
            string lighting = "";
            string lighting_type = "";
            string elevator_depth = "";
            string comment = "";

            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["est_id"]) == est_id)
                {
                    is_elevator = (string)json["is_elevator"];
                    location = (string)json["location"];
                    works = (string)json["works"];
                    no_assist = (string)json["no_assist"];
                    button_height = (string)json["button_height"];
                    outside_btn_height = (string)json["outside_btn_height"];
                    inside_btn_height = (string)json["inside_btn_height"];
                    button_use_fist = (string)json["button_use_fist"];
                    braille = (string)json["braille"];
                    audible_tones = (string)json["audible_tones"];
                    lighting = (string)json["lighting"];
                    lighting_type = (string)json["lighting_type"];
                    elevator_depth = (string)json["elevator_depth"];
                    comment = (string)json["comment"];

                    if(is_elevator.ToLower().CompareTo("yes")==0)
                    {
                        data += "• There is an elevator at this establishment located at " + location + " and it " + works + "\n\r\n\r";
                    }
                    

                    if (no_assist.ToLower().CompareTo("yes")==0)
                    {
                        data += "• No assistance is needed for the elevator(s)" + "\n\r\n\r";
                    }
                    if(button_height.ToLower().CompareTo("yes")==0)
                    {
                        data += "• The outside button height is " + outside_btn_height + " inches, the inside button height is " + inside_btn_height + " inches, and the buttons are able to be pushed with a closed fist: " + button_use_fist + "\n\r\n\r";
                    }
                    if(braille.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Buttons and signs have braille markings and raised letters/ numbers" + "\n\r\n\r";
                    }
                    if(audible_tones.ToLower().CompareTo("yes")==0)
                    {
                        data += "• There are audible tones in the elevator" + "\n\r\n\r";
                    }
                    if(lighting.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Lighting level is " + lighting_type + " in daytime, and is adequate for mobility and reading signs" + "\n\r\n\r";
                    }
                    if(elevator_depth.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Elevator depth is at least 54 inches" + "\n\r\n\r";
                    }
                    if(comment.CompareTo("")!=0)
                    {
                        data += "• " + comment + "\n\r\n\r"; 
                    }
                }
            }

            if (data.CompareTo("")==0)
            {
                data = "• No elevator or lift is needed as this business is on the ground level";
            }
            return data;
        }

        private string parseCommunication(string unparsedData)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string data = "";

            string public_phone = "";
            string phone_clearance = "";
            string num_phone = "";
            string tty = "";
            string staff_tty = "";
            string assisted_listening = "";
            string assisted_listen_type = "";
            string assisted_listen_receiver = "";
            string listening_signage = "";
            string staff_listening = "";
            string acoustics = "";
            string acoustics_level = "";
            string alt_comm_methods = "";
            string alt_comm_type = "";
            string staff_ASL = "";
            string captioning_default = "";
            string theater_captioning = "";
            string theater_capt_type = "";
            string auditory_info_visual = "";
            string visual_info_auditory = "";
            string website_text_reader = "";
            string alt_contact = "";
            string alt_contact_type = "";
            string shopping_assist = "";
            string assist_service = "";
            string assist_fee = "";
            string store_scooter = "";
            string scooter_fee = "";
            string scooter_location = "";
            string restaurant_allergies = "";
            string staff_disable_trained = "";
            string staff_disable_trained_desc = "";
            string items_reach = "";
            string service_alt_manner = "";
            string senior_discount = "";
            string senior_age = "";
            string annual_A4A_review = "";
            string comment = "";

            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["est_id"]) == est_id)
                {
                    public_phone = (string)json["public_phone"];
                    phone_clearance = (string)json["phone_clearance"];
                    num_phone = (string)json["num_phone"];
                    tty = (string)json["tty"];
                    staff_tty = (string)json["staff_tty"];
                    assisted_listening = (string)json["assisted_listening"];
                    assisted_listen_type = (string)json["assisted_listen_type"];
                    assisted_listen_receiver = (string)json["assisted_listen_receiver"];
                    listening_signage = (string)json["listening_signage"];
                    staff_listening = (string)json["staff_listening"];
                    acoustics = (string)json["acoustics"];
                    acoustics_level = (string)json["acoustics_level"];
                    alt_comm_methods = (string)json["alt_comm_methods"];
                    alt_comm_type = (string)json["alt_comm_type"];
                    staff_ASL = (string)json["staff_ASL"];
                    captioning_default = (string)json["captioning_default"];
                    theater_captioning = (string)json["theater_captioning"];
                    theater_capt_type = (string)json["theater_capt_type"];
                    auditory_info_visual = (string)json["auditory_info_visual"];
                    visual_info_auditory = (string)json["visual_info_auditory"];
                    website_text_reader = (string)json["website_text_reader"];
                    alt_contact = (string)json["alt_contact"];
                    alt_contact_type = (string)json["alt_contact_type"];
                    shopping_assist = (string)json["shopping_assist"];
                    assist_service = (string)json["assist_service"];
                    assist_fee = (string)json["assist_fee"];
                    store_scooter = (string)json["store_scooter"];
                    scooter_fee = (string)json["scooter_fee"];
                    scooter_location = (string)json["scooter_location"];
                    restaurant_allergies = (string)json["restaurant_allergies"];
                    staff_disable_trained = (string)json["staff_disable_trained"];
                    staff_disable_trained_desc = (string)json["staff_disable_trained_desc"];
                    items_reach = (string)json["items_reach"];
                    service_alt_manner = (string)json["service_alt_manner"];
                    senior_discount = (string)json["senior_discount"];
                    senior_age = (string)json["senior_age"];
                    annual_A4A_review = (string)json["annual_A4A_review"];
                    comment = (string)json["comment"];


                    if(public_phone.ToLower().CompareTo("yes")==0)
                    {
                        data += "• One or more public phones are available with adjustable volume control" + "\n\r\n\r";
                    }
                    if(phone_clearance.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Public phones have controls minimum 48 inches from floor, protruding less than 4 inches from wall" + "\n\r\n\r";
                    }
                    if(tty.ToLower().CompareTo("yes")==0)
                    {
                        data += "• TTY is available" + "\n\r\n\r";
                    }
                    if(staff_tty.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Staff are trained in use of TTY and how to accept relay calls" + "\n\r\n\r";
                    }
                    if(assisted_listening.ToLower().CompareTo("yes")==0)
                    {
                        data += "• There are assisted listening devices available of type" + assisted_listen_type + " with assisted listen receiver " + assisted_listen_receiver + "\n\r\n\r";
                    }
                    if(listening_signage.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Signs about listening devices are clearly displayed" + "\n\r\n\r";
                    }
                    if(staff_listening.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Staff are trained to use assisted listening devices" + "\n\r\n\r";
                    }
                    if(acoustics.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Acoustics are comfortable (no echoing, loud music, etc). Noise level = " + acoustics_level + "\n\r\n\r";
                    }
                    if(alt_comm_methods.ToLower().CompareTo("yes")==0)
                    {
                        data += "• If a customer is unable to hear, other forms of communication include: " + alt_comm_type + "\n\r\n\r";
                    }
                    if(staff_ASL.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Staff have received instructions on how to provide ASL services upon request" + "\n\r\n\r";
                    }
                    if(captioning_default.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Captioning is turned 'on' as default for TVs or projected video" + "\n\r\n\r";
                    }
                    if(theater_captioning.ToLower().CompareTo("yes")==0)
                    {
                        data += "• If this is a theater," + theater_capt_type +" is available." + "\n\r\n\r";
                    }
                    if(auditory_info_visual.ToLower().CompareTo("yes") == 0)
                    {
                        data += "• Auditory information is presented visually (special of the day written down, etc.)" + "\n\r\n\r";
                    }
                    if(visual_info_auditory.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Visual information is presented audibly (employees will read information out load, etc.)" + "\n\r\n\r";
                    }
                    if(website_text_reader.ToLower().CompareTo("yes")==0)
                    {
                        data += "• The establishment’s website is accessible to users of screen text readers" + "\n\r\n\r";
                    }
                    if(alt_contact.ToLower().CompareTo("yes")==0)
                    {
                        data += "• The following alternate means are available for patrons to order, contact, or schedule: " + alt_contact_type + "\n\r\n\r";
                    }
                    if(shopping_assist.ToLower().CompareTo("yes")==0)
                    {
                        data += "• The establishment offers shopping assistance or delivery." + "\n\r\n\r";
                    }
                    if(assist_service.ToLower().CompareTo("yes")==0)
                    {
                        data += "• The establishment charges $" + assist_fee + " for delivery or assistance.\n\r\n\r";
                    }
                    if(store_scooter.ToLower().CompareTo("yes")==0)
                    {
                        data += "• The establishment provides scooters at a cost of $" + scooter_fee + " and are located at " + scooter_location + "\n\r\n\r";
                    }
                    if(restaurant_allergies.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Information on food allergies or sensitivities are available" + "\n\r\n\r";
                    }
                    if(staff_disable_trained.ToLower().CompareTo("yes")==0)
                    {
                        data += "• The staff have received training in how to provide disability friendly customer service" + "\n\r\n\r";
                    }
                    if(items_reach.ToLower().CompareTo("yes")==0)
                    {
                        data += "• All items are within reach or assistance is offered to reach them" + "\n\r\n\r";
                    }
                    if(service_alt_manner.ToLower().CompareTo("yes")==0)
                    {
                        data += "• If goods and services are not accessible they are provided in alternative manner" + "\n\r\n\r";
                    }
                    if(senior_discount.ToLower().CompareTo("yes")==0)
                    {
                        data += "• The establishment offers a senior discount, beginning at age " + senior_age + "\n\r\n\r";
                    }
                    if(annual_A4A_review.ToLower().Equals("yes"))
                    {
                        data += "• Management has agreed to annual A4A reviews. \n\r\n\r";
                    }
                    if(comment.CompareTo("")!=0)
                    {
                        data += "• " + comment + "\n\r\n\r";
                    }

                }
            }
            return data;
        }

        private string parseInterior(string unparsedData)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string data = "";

            int interior_id = 0;
            string int_door_open_clearance = "";
            double int_opening_measurement = 0;
            string int_door_easy_open = "";
            double int_door_open_force = 0;
            string int_door_use_with_fist = "";
            string five_second_close = "";
            string hallway_width = "";
            double narrowest_width = 0;
            string wheelchair_turnaround = "";
            string hallway_obstacles = "";
            string hallway_clear = "";
            string lighting = "";
            string lighting_type = "";
            string service_counter = "";
            double counter_height = 0;
            double writing_surface_height = 0;
            string drinking_fountain = "";
            string comment = "";


            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["est_id"]) == est_id)
                {
                    interior_id = (int)json["interior_id"];
                    int_door_open_clearance = (string)json["int_door_open_clearance"];
                    int_opening_measurement = (double)json["int_opening_measurement"]; 
                    int_door_easy_open = (string)json["int_door_easy_open"]; 
                    int_door_open_force = (double)json["int_door_open_force"];
                    int_door_use_with_fist = (string)json["int_door_use_with_fist"];
                    five_second_close = (string)json["five_second_close"];
                    hallway_width = (string)json["hallway_width"];
                    narrowest_width = (double)json["narrowest_width"];
                    wheelchair_turnaround = (string)json["wheelchair_turnaround"];
                    hallway_obstacles = (string)json["hallway_obstacles"];
                    hallway_clear = (string)json["hallway_clear"];
                    lighting = (string)json["lighting"];
                    lighting_type = (string)json["lighting_type"]; 
                    service_counter = (string)json["service_counter"]; 
                    counter_height = (double)json["counter_height"]; 
                    writing_surface_height = (double)json["writing_surface_height"]; 
                    drinking_fountain = (string)json["drinking_fountain"];
                    comment = (string)json["comment"];


                    if(int_door_open_clearance.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Interior doors (aside from restrooms) have at least 32 inches clearance when the door is open at 90 degrees" + "\n\r\n\r";
                    }

                    if(int_opening_measurement > 0)
                    {
                        data += "• Interior door is "+ int_opening_measurement + " inches wide\n\r\n\r";
                    }

                    if(int_door_easy_open.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Interior doors are easy to open, requiring 5 lbs. or less of force(" + int_opening_measurement + " lbs)\n\r\n\r";
                    }

                    if(int_door_use_with_fist.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Door handles can be operated with a closed fist or opened automatically or push button" + "\n\r\n\r";
                    }

                    if(five_second_close.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Doors stay open for at least five seconds" + "\n\r\n\r";
                    }

                    if(hallway_width.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Hallways and aisles are at least 36 inches wide, or not less than " + narrowest_width + " inches for four foot intervals\n\r\n\r";
                    }

                    if(wheelchair_turnaround.ToLower().CompareTo("yes")==0)
                    {
                        data += "• There are locations that allow 60 inches for a wheelchair to turn around" + "\n\r\n\r";
                    }

                    if(hallway_obstacles.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Hallways and aisles are clear of obstacles, tripping hazards, objects protruding more than 4 inches or lower than 80 inches" + "\n\r\n\r";
                    }

                    if(lighting.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Lighting level is " + lighting_type + " in daytime, and is adequate for mobility and reading signs" + "\n\r\n\r";
                    }

                    if(service_counter.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Lowest service counter is no higher than 38 inches with a clear view from a sitting position and a check writing surface is no higher than 34 inches. Service counter height: " + service_counter + "\n\r\n\r";
                    }

                    if(drinking_fountain.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Accessible drinking fountain has spout no higher than 36 inches from floor and easy to operate controls" + "\n\r\n\r";
                    }

                    if(comment.CompareTo("")!=0)
                    {
                        data += "• " + comment + "\n\r\n\r";
                    }
                }
            }
            return data;
        }

        private string parseMainEntrance(string unparsedData, string curLocation)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string data = "";

            int main_ent_id = 0;
            int total_num_public_entrances = 0;
            string main_ent_accessible = "";
            string alt_ent_accessible = "";
            string accessable_signage = "";
            string ground_level = "";
            string threshold_level = "";
            string threshold_beveled = "";
            double beveled_height = 0;
            string door_action = "";
            double opening_measurement = 0;
            string door_open_clearance = "";
            double door_open_force = 0;
            string door_easy_open = "";
            string door_use_with_fist = "";
            string lighting_type = "";
            string door_auto_open = "";
            string second_door_inside = "";
            string min_dist_between_doors = "";
            string lighting = "";
            string lighting_option = "";
            string comment = "";


            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["est_id"]) == est_id)
                {
                    main_ent_id = (int)json["main_ent_id"];
                    total_num_public_entrances = ((int)json["total_num_public_entrances"]);
                    main_ent_accessible = ((string)json["main_ent_accessible"]).ToLower();
                    alt_ent_accessible = ((string)json["alt_ent_accessible"]).ToLower();
                    accessable_signage = ((string)json["accessable_signage"]).ToLower();
                    ground_level = ((string)json["ground_level"]).ToLower();
                    threshold_level = ((string)json["threshold_level"]).ToLower();
                    threshold_beveled = ((string)json["threshold_beveled"]).ToLower();
                    beveled_height = ((double)json["beveled_height"]);
                    door_open_clearance = ((string)json["door_open_clearance"]).ToLower();
                    door_action = ((string)json["door_action"]).ToLower();
                    door_open_force = ((double)json["door_open_force"]);
                    door_easy_open = ((string)json["door_easy_open"]).ToLower();
                    door_use_with_fist = ((string)json["door_use_with_fist"]).ToLower();
                    door_auto_open = ((string)json["door_auto_open"]).ToLower();
                    second_door_inside = ((string)json["second_door_inside"]).ToLower();
                    min_dist_between_doors = ((string)json["min_dist_between_doors"]).ToLower();
                    lighting = ((string)json["lighting"]).ToLower();
                    opening_measurement = ((double)json["opening_measurement"]);
                    lighting_type = ((string)json["lighting_type"]).ToLower();
                    lighting_option = ((string)json["lighting_option"]).ToLower();
                    comment = ((string)json["comment"]).ToLower();

                    data += ("• " + "The establishment has " + total_num_public_entrances + " public entrance(s).\n\r\n\r");

                    if (main_ent_accessible.ToLower().CompareTo("yes") == 0)
                        data += ("• The main entrance is wheelchair accessible. \n\r\n\r");


                    if (ground_level.ToLower().CompareTo("yes") == 0)
                        data += ("• Ground floor is level inside and outside entrance door. \n\r\n\r");


                    if (threshold_level.ToLower().CompareTo("yes") == 0)
                        data += ("• Threshold of door is level. \n\r\n\r");

                    if (threshold_beveled.ToLower().CompareTo("yes") == 0)
                    {
                        if (beveled_height <= 0.5)
                            data += ("• Door threshold is no more than ½ inch high. \n\r\n\r");
                        else
                            data += ("• Door threshold is more than ½ inch high. (actual "+beveled_height+" of an inch). \n\r\n\r");
                    }


                    if (door_action.ToLower().CompareTo("open in") == 0)
                        data += ("• As you enter, door opens away from you. \n\r\n\r");
                    else if(door_action.ToLower().CompareTo("open out") == 0)
                        data += ("• As you enter, door opens toward you. \n\r\n\r");
                    else
                        data += ("• As you enter, door slides to the side. \n\r\n\r");

                    if (opening_measurement > 0.00)
                        data += ("• Door has "+opening_measurement+" inch clearance when open 90 degrees. \n\r\n\r");

                    if (door_easy_open.ToLower().CompareTo("yes") == 0)
                        data += ("• Door is easy to open, requiring 10 lbs or less of force ("+ door_open_force + " lbs). \n\r\n\r");

                    if (door_use_with_fist.ToLower().CompareTo("yes") == 0)
                        data += ("• Door handles can be operated with closed fist, automatically or with a push button. \n\r\n\r");

                    if (lighting.ToLower().CompareTo("yes") == 0)
                        data += ("• Lighting level is "+lighting_type+" in "+lighting_option+" time, and is adequate for mobility and reading signs. \n\r\n\r");                        
                }
            }

            return data;
        }

        private string parseExteriorData(string unparsedData)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string data = "";
            string service_animal = "";
            string service_animal_location = "";
            string has_exterior_path = "";
            string min_width = "";
            string pathway_surface = "";
            string pathway_curbs = "";
            string tactile_warning = "";
            string slope = "";
            string lighting = "";
            string lighting_option = "";
            string lighting_type = "";
            string comment = "";

            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["est_id"]) == est_id)
                {
                    service_animal = (string)json["service_animal"];
                    service_animal_location = (string)json["service_animal_location"];
                    has_exterior_path = (string)json["has_exterior_path"];
                    min_width = (string)json["min_width"];
                    pathway_surface = (string)json["pathway_surface"];
                    pathway_curbs = (string)json["pathway_curbs"];
                    tactile_warning = (string)json["tactile_warning"];
                    slope = (string)json["slope"];
                    lighting = (string)json["lighting"];
                    lighting_option = (string)json["lighting_option"];
                    lighting_type = (string)json["lighting_type"];
                    comment = (string)json["comment"];

                    if (has_exterior_path.ToLower().CompareTo("yes") == 0)
                        data += "• This establishment has exterior pathways";

                    if (min_width.ToLower().CompareTo("yes")==0)
                        data += "• Sidewalk pathway is minimum 44 inches wide" + "\n\r\n\r";

                    if (pathway_curbs.ToLower().CompareTo("yes") == 0)
                        data += "• Pathway has curb ramps and curb cuts where needed" + "\n\r\n\r";

                    if (pathway_surface.ToLower().CompareTo("yes") == 0)
                        data += "• Surface is slip resistant, unbroken, level and free of obstacles" + "\n\r\n\r";

                    if (tactile_warning.ToLower().CompareTo("yes") == 0)
                        data += "• There are tactile warning strips or high contrast paint at curb ramps, stairwells, building entrances, parking areas and pedestrian crossings" + "\n\r\n\r";

                    /*if(slope.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Exterior has a ramp to enter the establishment" + "\n\r\n\r";
                        data += "• Ramp is at least 36 inches wide between handrails" + "\n\r\n\r";
                        data += "• For each section of the ramp, the running slope is no greater than 1:12, i.e. for every inch of height change there are at least 12 inches of ramp run. (actual 13 inches height & 204 inches length)" + "\n\r\n\r";
                        data += "• There is a level landing that is at least 60 inches long and at least as wide as the ramp at top of ramp" + "\n\r\n\r";
                        data += "• The ramp is clear of obstacles and protrusions of 4 inches or more from the sides" + "\n\r\n\r";
                        data += "• Ramp surface is firm, slip-resistant, and unbroken" + "\n\r\n\r";
                    }*/
                    //This section needs to be parsed in the ramp section

                    if (service_animal.ToLower().CompareTo("yes")==0)
                        data += "• There is a service animal relief area located at " + service_animal_location + "\n\r\n\r";

                    if(lighting.ToLower().CompareTo("yes")==0)
                    {
                        data += "• Lighting level is " + lighting_type + " in " + lighting_option + ", and is adequate for mobility and reading signs" + "\n\r\n\r";
                    }
                }
            }
            return data;
        }

        private string parseParkingInformation(string unparsedData, string loc)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string data = "";
            

            int park_id = 0;
            string lot_type = "";
            string street_metered = "";
            string parking_type = "";
            string total_spaces = "";
            string reserved_spaces = "";
            string general_accessible_spaces = "";
            string van_accessible_spaces = "";
            string reserve_space_sign = "";
            string reserve_space_obstacles = "";
            string comment = "";

            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["est_id"]) == est_id)
                {
                    park_id = (int)json["park_id"];
                    lot_type = ((string)json["lot_free"]).ToLower();
                    street_metered = ((string)json["street_metered"]).ToLower();
                    parking_type = ((string)json["parking_type"]).ToLower();
                    total_spaces = ((string)json["total_num_spaces"]).ToLower();
                    reserved_spaces = ((string)json["num_reserved_spaces"]).ToLower();
                    general_accessible_spaces = ((string)json["num_accessable_space"]).ToLower();
                    van_accessible_spaces = ((string)json["num_van_accessible"]).ToLower();
                    reserve_space_sign = ((string)json["reserve_space_sign"]).ToLower();
                    reserve_space_obstacles = ((string)json["reserve_space_obstacles"]).ToLower();
                    comment = ((string)json["comment"]).ToLower();
                    
                    if (street_metered.CompareTo("not metered") == 0)
                        street_metered = "free";

                    data += "• This establishment has the following types of parking: " + lot_type + " lot, " + street_metered + " street\n\r\n\r";
                    data += "• There are a total of " + total_spaces + " on the premises\n\r\n\r";

                    if (parking_type.CompareTo("other") != 0)
                        data += "• This establishment has " + parking_type + " parking\n\r\n\r";

                    data += "• " + general_accessible_spaces + " accessible parking spaces with a 5 foot loading aisle\n\r\n\r";
                    data += "• " + van_accessible_spaces + " ‘van accessible’ parking spaces with an 8 foot loading aisle\n\r\n\r";

                    if (reserve_space_sign.CompareTo("yes") == 0)
                        data += "• " + "Accessible parking spaces have signs that are not obstructed when a vehicle is parked there\n\r\n\r";

                    if (reserve_space_obstacles.CompareTo("yes") == 0)
                        data += "• Surface is level, unbroken, firm, slip resistant, and free of obstacles\n\r\n\r";

                    JArray jsonArray_route_from_parking = JArray.Parse(GetDataTable("route_from_parking", "park_id=" + park_id.ToString()));

                    for (int j = 0; j < jsonArray_route_from_parking.Count; j++)
                    {
                        JToken json_route = jsonArray_route_from_parking[j];

                        int route_park_id = (int)json_route["route_park_id"];
                        string distance = ((string)json_route["distance"]).ToLower();
                        string min_width = ((string)json_route["min_width"]).ToLower();
                        string route_surface = ((string)json_route["route_surface"]).ToLower();
                        string route_curbs = ((string)json_route["route_curbs"]).ToLower();
                        string tactile_warning = ((string)json_route["tactile_warning"]).ToLower();
                        string covered = ((string)json_route["covered"]).ToLower();
                        string lighting = ((string)json_route["lighting"]).ToLower();
                        string lighting_option = ((string)json_route["lighting_option"]).ToLower();
                        string lighting_type = ((string)json_route["lighting_type"]).ToLower();
                        string comment_route = ((string)json_route["comment"]).ToLower();
                        string recommendations_route = ((string)json_route["recommendations"]).ToLower();

                        if (j == 0)
                            data += "\n\r\n\rRoute from nearest accessible parking area to accessible entrance:\n\r\n\r\n\r\n\r";

                        data += "• Distance from nearest accessible parking to wheelchair accessible entrance is " + distance + " feet\n\r\n\r";

                        if (min_width.CompareTo("yes") == 0)
                            data += "• Route is at least 44 inches wide\n\r\n\r";

                        if (route_surface.CompareTo("yes") == 0)
                            data += "• Surface is level, unbroken, firm, slip resistant, and free of obstacles\n\r\n\r";

                        if (route_curbs.CompareTo("yes") == 0)
                            data += "• Route has curb ramps and curb cuts where needed\n\r\n\r";

                        if (tactile_warning.CompareTo("yes") == 0)
                            data += "• Route has tactile warning strips installed at curb ramps, stairwells, building entrances, parking areas and pedestrian crossings.\n\r\n\r";

                        if (covered.CompareTo("yes") == 0) 
                            data += "• Route is covered\n\r\n\r";

                        if (lighting.CompareTo("yes") == 0)
                            data += "• Lighting level is " + lighting_type + " in " + lighting_option + "time, and is adequate for mobility and reading signs\n\r\n\r";
                        if(comment_route.Length >0)
                            data += "• " + comment_route;
                    }

                    JArray jsonArray_passenger_loading = JArray.Parse(GetDataTable("passenger_loading", "park_id=" + park_id.ToString()));

                    for (int j = 0; j < jsonArray_passenger_loading.Count; j++)
                    {
                        JToken json_passenger = jsonArray_passenger_loading[j];
                   
                        int passenger_id = (int)json_passenger["passenger_id"];
                        string designated_zone = ((string)json_passenger["designated_zone"]).ToLower();
                        string p_distance = ((string)json_passenger["distance"]).ToLower();
                        string p_min_width = ((string)json_passenger["min_width"]).ToLower();
                        string passenger_surface = ((string)json_passenger["passenger_surface"]).ToLower();
                        string tactile_warning_strips = ((string)json_passenger["tactile_warning_strips"]).ToLower();
                        string passenger_curbs = ((string)json_passenger["passenger_curbs"]).ToLower();
                        string p_covered = ((string)json_passenger["covered"]).ToLower();
                        string p_lighting = ((string)json_passenger["lighting"]).ToLower();
                        string p_lighting_option = ((string)json_passenger["lighting_option"]).ToLower();
                        string p_lighting_type = ((string)json_passenger["lighting_type"]).ToLower();
                        string p_comment = ((string)json_passenger["comment"]).ToLower();
                        string p_recommendations = ((string)json_passenger["recommendations"]).ToLower();

                        if (j == 0)
                            data += "\n\r\n\rPassenger Loading Zone\n\r\n\r\n\r\n\r";

                        if (designated_zone.CompareTo("no") == 0)
                            data += "There is no designated passenger loading zone for the establishment\n\r\n\r";
                        else
                        {
                            data += "• The passenger loading zone is " + p_distance + " feet of the wheelchair accessible entrance\n\r\n\r";

                            if (p_min_width.CompareTo("yes") == 0)
                                data += "• The route from the loading zone to the entrance is at least 44 inches\n\r\n\r"; 

                            if (passenger_surface.CompareTo("yes") == 0)
                                data += "• Surface is level, unbroken, firm, slip resistant, and free of obstacles\n\r\n\r";

                            if (tactile_warning_strips.CompareTo("yes") == 0)
                                data += "• Route has tactile warning strips installed at curb ramps, stairwells, building entrances, parking areas and pedestrian crossings.\n\r\n\r";

                            if (passenger_curbs.CompareTo("yes") == 0)
                                data += "• Loading zone has curb ramps and curb cuts where needed\n\r\n\r";

                            if (p_covered.CompareTo("yes") == 0) //no way to determine partially covered
                                data += "• Route is covered\n\r\n\r";

                            if (p_lighting.CompareTo("yes") == 0)
                                data += "• Lighting level is " + p_lighting_type + " in " + p_lighting_option + " time\n\r\n\r";
                        }
                    }
                }
            }
            return data;
        }

        private string parseGeneralInformation(string unparsedData, string loc)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string text = loc + Environment.NewLine;
            MainActivity act = (MainActivity)this.Activity;
            string website = "";
            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((string)json["name"]).Equals(loc))
                {
                    string phone = "";
                    website = ((string)json["website"]); 
                    loc += Environment.NewLine;
                    loc += (((string)json["street"]) +" "+ ((string)json["city"]) +", " +((string)json["state"])+" "+ ((string)json["zip"]) + Environment.NewLine);
                    loc += Environment.NewLine;
                    if (website.CompareTo("") != 0)
                    {
                        if (website.StartsWith("http://www."))
                            WEBSITE = website;
                        else if (website.StartsWith("www."))
                            WEBSITE = "http://" + website;
                        else
                            WEBSITE = "http://www." + website;
                        websiteButton.Visibility = ViewStates.Visible;
                        websiteButton.Enabled = true;
                        loc += "Website: ";
                        loc += (website + Environment.NewLine);
                        loc += Environment.NewLine;
                    }
                    
                    
                    string PhoneText = "";
                    phone = ((string)json["phone"]);
                    
                    string phoneExpand = "";
                    if (phone.CompareTo("") != 0)
                    {
                        PhoneText += "Phone Number: ";
                        loc += "Phone Number: ";
                        if (phone.Length == 10)
                        {
                            PHONE = phone;
                            long phoneNum = long.Parse(phone);
                            phoneButton.Visibility = ViewStates.Visible;
                            phoneButton.Enabled = true;
                            phoneExpand = string.Format("{0: ###-###-####}", phoneNum);
                            PhoneText += phoneExpand;
                            loc += phoneExpand;

                        }
                        else
                        {
                            phoneButton.Visibility = ViewStates.Visible;
                            phoneButton.Enabled = true;
                            PHONE = convertPhone(phone);
                            PhoneText += phone;
                            loc += phone;
                        }


                        /* - This feels like a waste of time. May revisit during the weekend.
                        textView.Text = PhoneText;
                        // textView.SetTextAppearance("?android:attr/textApperanceLarge");
                        Linkify.AddLinks(textView, MatchOptions.PhoneNumbers);
                        textView.LinksClickable=true;
                        textView.SetMinHeight(55);
                        textView.SetMinWidth(55);
                        textView.SetTextSize(Android.Util.ComplexUnitType.Sp, 25);
                        textView.SetTextColor(Android.Graphics.Color.Black);
                        LINLAY.AddView(textView);
                        */
                        

     
                    }
                }
            }
            return loc;
        }

        private string convertPhone(string phone)
        {
            string [] temp = phone.Split("-");
            string res = "";
            for(int i = 0; i<temp.Length; i++)
            {
                res += temp[i];
            }
            return res;
        }

        private string parseTransitData(string unparsedData)
        {
            JArray jsonArray = JArray.Parse(unparsedData);
            string data = "";

            int park_id = 0;

            for (int i = 0; i < jsonArray.Count; i++)
            {
                JToken json = jsonArray[i];

                if (((int)json["est_id"]) == est_id)
                {
                    park_id = (int)json["park_id"];

                    JArray jsonArray_sta_bus = JArray.Parse(GetDataTable("sta_bus", "park_id=" + park_id.ToString()));

                    for (int j = 0; j < jsonArray_sta_bus.Count; j++)
                    {
                        JToken json_bus = jsonArray_sta_bus[j];

                        int sta_id = (int)json_bus["sta_id"];
                        string sta_service_area = ((string)json_bus["sta_service_area"]).ToLower();
                        string distance = ((string)json_bus["distance"]).ToLower();
                        string min_width = ((string)json_bus["min_width"]).ToLower();
                        string route_surface = ((string)json_bus["route_surface"]).ToLower();
                        string tactile_warning_strips = ((string)json_bus["tactile_warning_strips"]).ToLower();
                        string curb_cuts = ((string)json_bus["curb_cuts"]).ToLower();
                        string lighting = ((string)json_bus["lighting"]).ToLower();
                        string lighting_option = ((string)json_bus["lighting_option"]).ToLower();
                        string lighting_type = ((string)json_bus["lighting_type"]).ToLower();
                        string shelter_bench = ((string)json_bus["shelter_bench"]).ToLower();
                        string comment = ((string)json_bus["comment"]).ToLower();
                        string recommendations = ((string)json_bus["recommendations"]).ToLower();

                        if (sta_service_area.CompareTo("yes") == 0)
                            data += "• Establishment is located within the STA Service Area for paratransit\n\r\n\r";

                        data += "• Distance from nearest bus stop to wheelchair accessible entrance is " + distance + " feet\n\r\n\r";

                        if (min_width.CompareTo("yes") == 0)
                            data += "• Route is at least 44 inches wide\n\r\n\r";

                        if (route_surface.CompareTo("yes") == 0)
                            data += "• Surface is level, firm, unbroken, slip resistant, free of obstacles\n\r\n\r";

                        if (tactile_warning_strips.CompareTo("yes") == 0)
                            data += "• Route has tactile warning strips installed at curb ramps, stairwells, building entrances, parking areas and pedestrian crossings.\n\r\n\r";

                        if (shelter_bench.CompareTo("yes") == 0)
                            data += "• Bus stop has a shelter or a bench\n\r\n\r";

                        if (lighting.CompareTo("yes") == 0)
                            data += "• Lighting level is " + lighting_type + " in " + lighting_option + "time\n\r\n\r";

                        JArray jsonArray_sta_route = JArray.Parse(GetDataTable("sta_route", "sta_bus_id=" + sta_id.ToString()));

                        data += "\n\r\n\rRoute(s): ";

                        for (int k = 0; k < jsonArray_sta_route.Count; k++)
                        {
                            JToken json_route = jsonArray_sta_route[k];

                            data += ((string)json_route["route_num"]).ToLower() + ", ";

                        }
                        data = data.Substring(0, data.Length - 2);
                        data += "\n\r\n\rStop Number: ";

                        for (int k = 0; k < jsonArray_sta_route.Count; k++)
                        {
                            JToken json_route = jsonArray_sta_route[k];

                            //this took way longer than it should have
                            int north_bound_stop = int.Parse((string)json_route["north_bound_stop"]);
                            int south_bound_stop = int.Parse((string)json_route["south_bound_stop"]);
                            int east_bound_stop = int.Parse((string)json_route["east_bound_stop"]);
                            int west_bound_stop = int.Parse((string)json_route["west_bound_stop"]);
                            string ns_bounds = "";
                            string ew_bounds = "";

                            if (north_bound_stop != 0)
                                ns_bounds += north_bound_stop + " Northbound";
                            if (south_bound_stop != 0 && north_bound_stop != 0)
                                ns_bounds += "/";
                            if (south_bound_stop != 0)
                                ns_bounds += south_bound_stop + "Southbound";

                            if (east_bound_stop != 0)
                                ew_bounds += east_bound_stop + " Eastbound";
                            if (west_bound_stop != 0 && east_bound_stop != 0)
                                ew_bounds += "/";
                            if (west_bound_stop != 0)
                                ew_bounds += west_bound_stop + " Westbound";

                            if (ns_bounds.Length != 0)
                                ns_bounds += "\n\r";

                            if (ew_bounds.Length != 0)
                                ew_bounds += "\n\r";

                            data += ns_bounds + ew_bounds;
                        }
                    }
                }
            }
            return data;
        }

        public void OnBackPressed()
        {
            Android.Support.V4.App.Fragment fragment = null;
            Bundle args = new Bundle();
            args.PutString("location", curLocation);
            args.PutString("selection", selection);
            args.PutString("prevView", prevView);
            fragment = detailFragment.NewInstance();
            fragment.Arguments = args;
            base.FragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content_frame, fragment)
                        .Commit();
        }
            

        private string GetData(string search_specifics) //using default table + key to search table by
        {

            var request = HttpWebRequest.Create(String.Format(@"http://access4allspokane.org/RESTapi/" + table + "/?" + search_specifics));
            request.ContentType = "application/json";
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    if (String.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Response contained empty body...");
                    }
                    else
                    {
                        return content;
                    }
                }
            }
            return "NULL";
        }

        private string GetDataTable(string table_name, string search_specifics) //search a passed in table name + a passed in key
        {

            var request = HttpWebRequest.Create(String.Format(@"http://access4allspokane.org/RESTapi/" + table_name + "/?" + search_specifics));
            request.ContentType = "application/json";
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    if (String.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Response contained empty body...");
                    }
                    else
                    {
                        return content;
                    }
                }
            }
            return "NULL";
        }
    }

}
