using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Access4All.Fragments;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Access4All
{
    class catAdapter : BaseExpandableListAdapter
    {
        Activity activity = null;
        List<Categories> categories = null;
        private categoriesFragment categoriesFragment = null;

        public catAdapter(Activity activity, List<Categories> categories)
        {
            this.activity = activity;
            this.categories = categories;
        }

        public catAdapter(categoriesFragment categoriesFragment, List<Categories> group)
        {
            this.categoriesFragment = categoriesFragment;
            this.categories = group;
        }

        public override int GroupCount
        {
            get
            {
                return categories.Count;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return categories[groupPosition].Locations[childPosition].ToString();
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return categories[groupPosition].locations.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                MainActivity act = (MainActivity)MainActivity.activity;
                LayoutInflater inflater = (LayoutInflater)act.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.item_layout, null);
            }
            TextView textViewItem = convertView.FindViewById<TextView>(Resource.Id.item);
            string content = (string)GetChild(groupPosition, childPosition);
            textViewItem.Text = content;
            return convertView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return categories[groupPosition].title;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                MainActivity act = (MainActivity)MainActivity.activity;
                LayoutInflater inflater = (LayoutInflater)act.GetSystemService(Activity.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.group_item, null);
            }
            string textGroup = (string)GetGroup(groupPosition);
            TextView textViewGroup = convertView.FindViewById<TextView>(Resource.Id.group);
            textViewGroup.Text = textGroup;
            return convertView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}