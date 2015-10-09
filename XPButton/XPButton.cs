﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace XP
{
    public sealed class XPButton : Button
    {
        private Grid _shadowGrid;
        private ContentPresenter _contentPresenter;
        private Border _backgroundBorder;
        private List<Border> _shadowBorders = new List<Border>();


        public double ShadowLength
        {
            get { return (double)GetValue(ShadowLengthProperty); }
            set { SetValue(ShadowLengthProperty, value); }
        }
        public static readonly DependencyProperty ShadowLengthProperty =
            DependencyProperty.Register("ShadowLength", typeof(double), typeof(XPButton), new PropertyMetadata(0));

        public int ShadowBorderCount
        {
            get { return (int)GetValue(ShadowBorderCountProperty); }
            set { SetValue(ShadowBorderCountProperty, value); }
        }
        public static readonly DependencyProperty ShadowBorderCountProperty =
            DependencyProperty.Register("ShadowBorderCount", typeof(int), typeof(XPButton), new PropertyMetadata(0));

        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }
        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.Register("ShadowColor", typeof(Brush), typeof(XPButton), new PropertyMetadata(Colors.Black));

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(double), typeof(XPButton), new PropertyMetadata(0));




        public XPButton()
        {
            this.DefaultStyleKey = typeof(XPButton);
            Loaded += XPButton_Loaded;
            SizeChanged += XPButton_SizeChanged;
        }

        private void XPButton_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateShadowEffect();
        }

        private void XPButton_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ShadowBorderCount; i++)
            {
                var border = new Border() { Background = new SolidColorBrush(Colors.Transparent) };
                _shadowBorders.Add(border);
                _shadowGrid.Children.Add(border);
            }
            UpdateShadowEffect();
        }

        void UpdateShadowEffect()
        {
            var lenGap = ShadowLength / ShadowBorderCount;
            var alphaArr = BuildShadowAlpha();
            var width = Math.Max(0, ActualWidth - ShadowLength * 2 );
            var height = Math.Max(0, ActualHeight - ShadowLength * 2);
            int index = 0;
           
            _shadowBorders.ForEach(o =>
            {
                o.Width = ActualWidth - lenGap * index;
                o.Height = ActualHeight - lenGap * index;
                o.BorderThickness = new Thickness(lenGap);
                o.CornerRadius = new CornerRadius(CornerRadius < 1 ? o.Width / 2 : CornerRadius * o.Width/ActualWidth);
                o.BorderBrush = new SolidColorBrush(Color.FromArgb(alphaArr[index], ShadowColor.R, ShadowColor.G, ShadowColor.B));
                index++;
            });
            _contentPresenter.Width = width;
            _contentPresenter.Height = height;
            _backgroundBorder.Width = width + lenGap;
            _backgroundBorder.Height = height + lenGap;
            _backgroundBorder.Margin = new Thickness(-lenGap / 2);
            _backgroundBorder.CornerRadius = new CornerRadius(CornerRadius < 1 ? _backgroundBorder.Width / 2 : CornerRadius * _backgroundBorder.Width / ActualWidth);
        }

        byte[] BuildShadowAlpha()
        {
            byte[] alphaArray = new byte[ShadowBorderCount];
            alphaArray[0] = 60;
            for(int i=0;i<ShadowBorderCount-1;i++)
            {
                alphaArray[i + 1] = (byte)Math.Max(5, alphaArray[i] - Math.Max(20 - 5 * i, 5));
            }
            return alphaArray.Reverse().ToArray();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _shadowGrid = (Grid)GetTemplateChild("ShadowGrid");
            _contentPresenter = (ContentPresenter)GetTemplateChild("ContentPresenter");
            _backgroundBorder = (Border)GetTemplateChild("BackgroundBorder");
        }
    }
}
