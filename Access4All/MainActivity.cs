using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Access4All.Fragments;
using Android.Content;
using Android.Speech;
using Xamarin.Essentials;
using Android.Content.PM;

namespace Access4All
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        public static Activity activity;

        public interface IBackButtonListener
        {
            void OnBackPressed();
        }

        public override void OnBackPressed()
        {
            var currentFragment = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
            var listener = currentFragment as IBackButtonListener;
            if (listener != null)
            {
                listener.OnBackPressed();
                return;
            }
            base.OnBackPressed();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_main);
            activity = this;

            BottomNavigationView navigation = (BottomNavigationView)FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            Android.Support.V4.App.Fragment fragment = null;
            fragment = categoriesFragment.NewInstance();
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();
            var connectivity = Connectivity.NetworkAccess;
            if (connectivity != NetworkAccess.Internet)
            {
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("Error");
                alert.SetMessage("Device is not connected to the internet, please check network connection");
                alert.SetButton("OK", (c, ev) =>
                {
                    this.FinishAffinity();
                });
                alert.Show();
            }

            if (CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted)
            {
                RequestPermissions(new string[] { Android.Manifest.Permission.AccessCoarseLocation, Android.Manifest.Permission.AccessFineLocation }, 0);
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean("appState", SplashActivity.appState);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 100) // Voice search
            {
                if (resultCode == Result.Ok) 
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                       
                        var voiceInput = matches[0];

                        if (voiceInput.Length > 500)
                            voiceInput = voiceInput.Substring(0, 500);

                        string voiceString = voiceInput;

                        SearchView searchView = (SearchView)this.FindViewById(Resource.Id.searchView1);
                       
                        searchView.SetQuery(voiceString, true);
                    }
                }
                
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            Android.Support.V4.App.Fragment fragment = null;
            switch (item.ItemId)
            {

                case Resource.Id.navigation_search:
                    fragment = searchFragment.NewInstance();
                    if (fragment == null)
                        return false;
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content_frame, fragment)
                        .Commit();

                    return true;

                case Resource.Id.navigation_categories:
                    fragment = categoriesFragment.NewInstance();
                    if (fragment == null)
                        return false;
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content_frame, fragment)
                        .Commit();

                    return true;

            }
            return true;
        }
    }
}

