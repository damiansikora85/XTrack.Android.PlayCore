﻿using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Com.Google.Android.Play.Core.Appupdate;
using Com.Google.Android.Play.Core.Install.Model;
using Com.Google.Android.Play.Core.Tasks;
using Java.Lang;
using System;
using Android.Content;

namespace Sample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnSuccessListener
    {
        private IAppUpdateManager _appUpdateManager;
        private const int UpdateRequestCode = 123;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var buttonTest = FindViewById<Button>(Resource.Id.buttonUpdate);
            buttonTest.Click += OnCheckUpdateClick;
            _appUpdateManager = AppUpdateManagerFactory.Create(this);
        }

        private void OnCheckUpdateClick(object sender, EventArgs e)
        {
            var appUpdateInfoTask = _appUpdateManager.AppUpdateInfo;
            appUpdateInfoTask.AddOnSuccessListener(this);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnSuccess(Java.Lang.Object data)
        {
            if (data is AppUpdateInfo appUpdateInfo)
            {
                if (appUpdateInfo.UpdateAvailability() == UpdateAvailability.UpdateAvailable
                          // For a flexible update, use AppUpdateType.FLEXIBLE
                          && appUpdateInfo.IsUpdateTypeAllowed(AppUpdateType.Immediate))
                {
                    // Request the update.
                    _appUpdateManager.StartUpdateFlowForResult(
                    // Pass the intent that is returned by 'getAppUpdateInfo()'.
                    appUpdateInfo,
                    // Or 'AppUpdateType.FLEXIBLE' for flexible updates.
                    AppUpdateType.Immediate,
                    // The current activity making the update request.
                    this,
                    // Include a request code to later monitor this update request.
                    UpdateRequestCode);
                }
                else
                {
                    Toast.MakeText(this, "No updates", ToastLength.Short).Show();
                }
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == UpdateRequestCode)
            {
                if (resultCode != Result.Ok)
                {
                    Toast.MakeText(this, "Update flow failed! Result code: " + resultCode, ToastLength.Short).Show();
                    // If the update is cancelled or fails,
                    // you can request to start the update again.
                }
            }
        }
    }
}