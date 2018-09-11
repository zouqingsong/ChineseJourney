using System;
using Serilog.Core;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace ChineseJourney.Common.View
{
    public class SwitchControl : SKCanvasView
    {
        public static BindableProperty IsToggledProperty = BindableProperty.Create(@"IsToggled", typeof(bool), typeof(SwitchControl), false, propertyChanged: HandleToggledChanged, defaultBindingMode:BindingMode.TwoWay);
        public static BindableProperty BorderColorProperty = BindableProperty.Create(@"BorderColor", typeof(Color), typeof(SliderControl), Color.White);
        public static BindableProperty InactiveColorProperty = BindableProperty.Create(@"InactiveColor", typeof(Color), typeof(SliderControl), Color.Gray);
        public static BindableProperty ActiveColorProperty = BindableProperty.Create(@"ActiveColor", typeof(Color), typeof(SliderControl), Color.Green);

        const float yGap = 0.05f;

        public SwitchControl()
        {
            HeightRequest = 35;
            WidthRequest = 70;
            PaintSurface += OnCanvasViewPaintSurface;
            EnableTouchEvents = true;
            Touch += OnTouchEffectAction;
        }

        private void OnTouchEffectAction(object sender, SKTouchEventArgs args)
        {
            switch (args.ActionType)
            {
                case SKTouchAction.Released:
                    IsToggled = !IsToggled;
                    break;
            }
        }


        static void HandleToggledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as SwitchControl;
            if (c != null)
            {
                c.IsToggled = (bool)newValue;
                c.InvalidateSurface();
                c.Toggled?.Invoke(c, new ToggledEventArgs(c.IsToggled));
            }
        }

        public bool IsToggled
        {
            get { return (bool)GetValue(IsToggledProperty); }
            set { SetValue(IsToggledProperty, value); }
        }

        public event EventHandler<ToggledEventArgs> Toggled;
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }


        public Color ActiveColor
        {
            get { return (Color)GetValue(ActiveColorProperty); }
            set { SetValue(ActiveColorProperty, value); }
        }

        public Color InactiveColor
        {
            get { return (Color)GetValue(InactiveColorProperty); }
            set { SetValue(InactiveColorProperty, value); }
        }
        public void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            using (SKPaint fillPaint = new SKPaint())
            {
                using (SKPaint outlinePaint = new SKPaint())
                {
                    SKPath path = new SKPath();
                    float y = yGap * info.Height;
                    float width = info.Height * 1.6f;
                    float radius = info.Height / 2f;
                    SKRect rect1 = new SKRect(y, y, info.Height - y, info.Height - y);
                    SKRect rect2 = new SKRect(width - info.Height, y, width - y, info.Height - y);
                    path.ArcTo(rect1, 90f, 180f, false);
                    path.LineTo(width - radius, y);
                    path.ArcTo(rect2, 270f, 180f, false);
                    path.Close();

                    outlinePaint.Style = SKPaintStyle.Stroke;
                    outlinePaint.StrokeWidth = 2;
                    outlinePaint.Color = BorderColor.ToSKColor();
                    outlinePaint.IsAntialias = true;

                    fillPaint.Style = SKPaintStyle.Fill;
                    fillPaint.Color = IsToggled ? ActiveColor.ToSKColor() : InactiveColor.ToSKColor();
                    fillPaint.IsAntialias = true;

                    canvas.DrawPath(path, fillPaint);
                    canvas.DrawPath(path, outlinePaint);

                    fillPaint.Color = BorderColor.ToSKColor();
                    canvas.DrawOval(IsToggled ? rect2 : rect1, fillPaint);
                }
            }
        }
    }
}