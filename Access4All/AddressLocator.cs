using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Access4All
{
    class AddressLocator : IComparable<AddressLocator>
    {
        string name = "";
        string address = "";
        float lon = 0;
        float lat = 0;
        float distance = 0;

        public AddressLocator(string name, string address, float lon, float lat)
        {
            this.name = name;
            this.address = address;
            this.lon = lon;
            this.lat = lat;
            this.distance = 0;
        }

        public float Distance
        {
            get
            {
                return this.distance;
            }
            set
            {
                this.distance = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string Address
        {
            get
            {
                return this.address;
            }
        }

        public float Lon
        {
            get
            {
                return this.lon;
            }
        }
        public float Lat
        {
            get
            {
                return this.lat;
            }
        }

        public int CompareTo(AddressLocator that)
        {
            return this.distance.CompareTo(that.distance);
        }

    }
}