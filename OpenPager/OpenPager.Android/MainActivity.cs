﻿using System;
using Android.App;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Media;
using Android.Widget;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using OpenPager.Models;
using Plugin.CurrentActivity;
using Plugin.Permissions;

namespace OpenPager.Droid
{
    [Activity(Label = "OpenPager", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly Lazy<App> _app = new Lazy<App>();

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(bundle);
            
            var appcenterKey = System.Environment.GetEnvironmentVariable("APPCENTER_KEY");
            if (!String.IsNullOrEmpty(appcenterKey))
            {
                AppCenter.Start(appcenterKey, typeof(Analytics), typeof(Crashes));
            }

            CheckPlayService();

            Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);

            LoadApplication(_app.Value);

            CheckOperationJson(Intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Android.Content.Intent intent)
        {
            base.OnNewIntent(intent);
            CheckOperationJson(intent);
        }

        private void CheckOperationJson(Android.Content.Intent intent)
        {
            if (!intent.HasExtra(MyFirebaseMessagingService.INTENT_EXTRA_OPERATION))
            {
                return;
            }

            var operationJson = intent.GetStringExtra(MyFirebaseMessagingService.INTENT_EXTRA_OPERATION);
            if (String.IsNullOrEmpty(operationJson))
            {
                return;
            }

            AddAlarmFlags();

            var operation = JsonConvert.DeserializeObject<Operation>(operationJson);
            _app.Value.PushOperationAsync(operation);
        }

        private void AddAlarmFlags()
        {
            Window.AddFlags(WindowManagerFlags.DismissKeyguard |
                            WindowManagerFlags.ShowWhenLocked |
                            WindowManagerFlags.TurnScreenOn |
                            WindowManagerFlags.AllowLockWhileScreenOn);
        }

        private void CheckPlayService()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    Toast.MakeText(this, GoogleApiAvailability.Instance.GetErrorString(resultCode), ToastLength.Long);
                }
                else
                {
                    Toast.MakeText(this, "This device is not supported", ToastLength.Long);
                }
            }
        }
    }
}