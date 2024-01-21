﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sldc.Themes.ElementThemes
{
    public static class PrimaryButtonStyleHelper
    {
        public static readonly DependencyProperty CornerRadiusProperty =
           DependencyProperty.RegisterAttached(
               "CornerRadius",
               typeof(CornerRadius),
               typeof(PrimaryButtonStyleHelper),
               new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsRender));
        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }
        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }
    }
}