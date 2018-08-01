﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenPager.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace OpenPager.Views
{
    public class OperationMapPage : ContentPage
    {
        public OperationMapPage(Operation operation)
        {
            var map = new Map(
                MapSpan.FromCenterAndRadius(
                    new Position(37, -122), Distance.FromMiles(0.3)))
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(map);
            Content = stack;
        }
    }
}