using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Access4All.Fragments
{
    public class categoriesFragment : Android.Support.V4.App.Fragment
    {
        catAdapter mAdapter = null;
        List<Categories> group = new List<Categories>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            setData();
            mAdapter = new catAdapter(this, group);
        }

        

        private void setData()
        {
           
            group.Add(new Categories("Arts, Entertainment, Culture", SplashActivity.arts_and_entertainment_locations));
            group.Add(new Categories("Automotive", SplashActivity.automotive_locations));
            group.Add(new Categories("Business Services", SplashActivity.business_locations));
            group.Add(new Categories("Education", SplashActivity.education_locations));
            group.Add(new Categories("Financial Services", SplashActivity.bank_and_finance_locations));
            group.Add(new Categories("Food, Groceries", SplashActivity.food_and_drink_locations));
            group.Add(new Categories("Public Services, Government", SplashActivity.government_and_community_locations));
            group.Add(new Categories("Health, Medical, Dental, Mobility aids", SplashActivity.healthcare_locations));
            group.Add(new Categories("Home & Garden", SplashActivity.home_and_garden_locations));
            group.Add(new Categories("Mass Media, Printing, Publishing", SplashActivity.news_and_media_locations));
            group.Add(new Categories("Nightlife", SplashActivity.nightlife_locations));
            group.Add(new Categories("Recreation, Fitness", SplashActivity.sports_and_recreation_locations));
            group.Add(new Categories("Personal Services", SplashActivity.personal_services_locations));
            group.Add(new Categories("Pets", SplashActivity.pet_locations));
            group.Add(new Categories("Professional Services", SplashActivity.professional_services_locations));
            group.Add(new Categories("Religious Organizations", SplashActivity.religion_locations));
            group.Add(new Categories("Restaurants, Coffee Shops", SplashActivity.restaurant_and_coffee_shop_locations));
            group.Add(new Categories("Shopping", SplashActivity.retail_locations));
            group.Add(new Categories("Travel, Hotel, Motel", SplashActivity.travel_locations));
            
        }
        

        public static categoriesFragment NewInstance()
        {
            var catfrag = new categoriesFragment { Arguments = new Bundle() };
            return catfrag;
        }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.categoriesLayout, null);
            ExpandableListView ex = (ExpandableListView)v.FindViewById(Resource.Id.expandableListView1);
            ex.SetAdapter(mAdapter);
            ex.ChildClick += HandleSelect;
            return v;
        }

        private void HandleSelect(object sender, ExpandableListView.ChildClickEventArgs e)
        {
            string value = "";
            value = mAdapter.GetChild(e.GroupPosition, e.ChildPosition).ToString();
            Android.Support.V4.App.Fragment fragment = null;
            Bundle args = new Bundle();
            args.PutString("location", value);
            args.PutString("prevView", "categories");
            fragment = detailFragment.NewInstance();
            fragment.Arguments = args;
            base.FragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content_frame, fragment)
                        .Commit();
            
        }

        public bool OnChildClick(ExpandableListView parent, View v, int groupPosition, int childPosition, long id)
        {
            return true;
        }
        
    }
}