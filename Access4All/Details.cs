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
    class Details
    {


        private string infoTitle = "";

        public Details(string infoTitle)
        {
            this.infoTitle = infoTitle;
        }
        
        public string InfoTitle { get => infoTitle; set => infoTitle = value; }
    }
}