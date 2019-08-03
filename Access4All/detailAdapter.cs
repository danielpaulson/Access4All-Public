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

namespace Access4All
{
    class detailAdapter : BaseExpandableListAdapter
    {
        Activity activity = null;
        List<Details> details = null;
        private detailFragment detailFragment = null;

        public detailAdapter(Activity activity, List<Details> details)
        {
            this.activity = activity;
            this.details = details;
        }

        public detailAdapter(detailFragment detailFragment, List<Details> group)
        {
            this.detailFragment = detailFragment;
            this.details = group;
        }

        public override int GroupCount
        {
            get
            {
                return details.Count;
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
            return null;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return 0;
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
            return details[groupPosition].InfoTitle;
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