﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChineseJourney.Common.Controller;
using ChineseJourney.Common.Helpers;
using ChineseJourney.Common.Model;
using ChineseJourney.Common.View;
using SkiaSharp.Extended.Svg;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChineseJourney.Common
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HanziPage : ContentPage
	{
	    public HanziPage ()
		{
			InitializeComponent ();
		    Model  = new HanZiModel();
		    BindingContext = Model;
		    Model.IsAnimationEnabled = true;
            Model.Zi = "赢";
		}
	    private HanZiModel Model { get; }
    }
}