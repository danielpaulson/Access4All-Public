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
    public class Location : Java.Lang.Object
    {
        public Location()
        {

        }

        public Location(int est_id, string name, string website, string subtype, string state, string date, string street, string city, string zip, string phone, string tty, string contact_fname,string contact_lname, string contact_title, string contact_email, int user_id, int cat_id, int config_id, string config_comment)
        {
            this.cat_id = cat_id;
            this.city = city;
            this.config_comment = config_comment;
            this.config_id = config_id;
            this.contact_email = contact_email;
            this.contact_fname = contact_fname;
            this.contact_lname = contact_lname;
            this.contact_title = contact_title;
            this.date = date;
            this.est_id = est_id;
            this.name = name;
            this.phone = phone;
            this.state = state;
            this.street = street;
            this.subtype = subtype;
            this.tty = tty;
            this.user_id = user_id;
            this.website = website;
            this.zip = zip;

        }

        public override string ToString()
        {
            return name;
        }

        public int est_id { get; set; }
        public string name { get; set; }
        public string website { get; set; }
        public string subtype { get; set; }
        public string date { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string phone { get; set; }
        public string tty { get; set; }
        public string contact_fname { get; set; }
        public string contact_lname { get; set; }
        public string contact_title { get; set; }
        public string contact_email { get; set; }
        public int user_id { get; set; }
        public int cat_id { get; set; }
        public int config_id { get; set; }
        public string config_comment { get; set; }

    }
}
