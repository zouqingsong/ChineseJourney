using System;
using System.Diagnostics;
using System.Windows.Input;
using ChineseJourney.Common.Helpers;
using Serilog.Core;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using ZibaobaoLib.Helpers;

namespace ChineseJourney.Common.View
{
    public class SliderControl: SKCanvasView
    {
        public static BindableProperty MinimumProperty = BindableProperty.Create(@"Minimum", typeof(double), typeof(SliderControl), 0.0, propertyChanged: MinValueChanged);
        public static BindableProperty MaximumProperty = BindableProperty.Create(@"Maximum", typeof(double), typeof(SliderControl), 1.0, propertyChanged: MaxValueChanged);
        public static BindableProperty ValueProperty = BindableProperty.Create(@"Value", typeof(double), typeof(SliderControl), 0.3, propertyChanged: HandleValueChanged, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.Create<SliderControl, ICommand>(p => p.ValueChangedCommand, null);
        public static BindableProperty ActiveBarColorProperty = BindableProperty.Create(@"ActiveBarColor", typeof(Color), typeof(SliderControl), Color.Red);
        public static BindableProperty InactiveBarColorProperty = BindableProperty.Create(@"InactiveBarColor", typeof(Color), typeof(SliderControl), Color.Gray);
        public static BindableProperty ControlBarColorProperty = BindableProperty.Create(@"ControlBarColor", typeof(Color), typeof(SliderControl), Color.LightGray);
        public static BindableProperty IsBusyProperty = BindableProperty.Create(@"IsBusy", typeof(bool), typeof(SliderControl), false, propertyChanged: HandleIsBusyChanged, defaultBindingMode: BindingMode.TwoWay);
        public static BindableProperty NativeFeelProperty = BindableProperty.Create(@"NativeFeel", typeof(bool), typeof(SwitchControl), false);


        const float yGap = 0.35f;
        public SliderControl()
        {
            PaintSurface += OnCanvasViewPaintSurface;
            HeightRequest = 46;
            EnableTouchEvents = true;
            Touch += OnTouchEffectAction;
        }

        public SliderControl(double min, double max, double val):this()
        {
            Minimum = min;
            Maximum = max;
            Value = val;
        }

        private bool _isPressed;
        private void OnTouchEffectAction(object sender, SKTouchEventArgs args)
        {
            SliderControl theView = (SliderControl)sender;
            double x = args.Location.X;
            switch (args.ActionType)
            {
                case SKTouchAction.Pressed:
                    _isPressed = true;
                    break;

                case SKTouchAction.Moved:
                    if (_isPressed)
                    {
                        Value = (Maximum - Minimum) * x / theView.Width + Minimum;
                        InvalidateSurface();
                    }
                    break;

                case SKTouchAction.Released:
                    if (_isPressed)
                    {
                        if (ValueChangedCommand != null && ValueChangedCommand.CanExecute(Value))
                        {
                            ValueChangedCommand.Execute(Value);
                        }
                    }
                    _isPressed = false;
                    break;

                case SKTouchAction.Cancelled:
                    _isPressed = false;
                    break;
            }
        }

        private static void MinValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as SliderControl;
            if (c != null)
            {
                c.Minimum = (double)newValue;
                if (c.Minimum >= c.Maximum)
                {
                    c.Maximum = c.Minimum + 1;
                }
                c.InvalidateSurface();
            }
        }

        private static void MaxValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as SliderControl;
            if (c != null)
            {
                c.Maximum = (double)newValue;
                if (c.Maximum <= c.Minimum)
                {
                    c.Maximum = c.Minimum + 1;
                }
                c.InvalidateSurface();
            }
        }

        static void HandleIsBusyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as SliderControl;
            if (c != null)
            {
                c.IsBusy = (bool) newValue;
                c.InvalidateSurface();
            }
        }
        static void HandleValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as SliderControl;
            if (c != null)
            {
                double oldV = c.Value;
                c.Value = (double)newValue;
                if (c.Value <= c.Minimum)
                {
                    c.Value = c.Minimum;
                }

                if (c.Value >= c.Maximum)
                {
                    c.Value = c.Maximum;
                }
                c.InvalidateSurface();
                c.ValueChanged?.Invoke(c, new ValueChangedEventArgs(oldV, c.Value));
            }
        }
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public ICommand ValueChangedCommand
        {
            get { return (ICommand)GetValue(ValueChangedCommandProperty); }
            set { SetValue(ValueChangedCommandProperty, value); }
        }

        public Color ActiveBarColor
        {
            get { return (Color)GetValue(ActiveBarColorProperty); }
            set { SetValue(ActiveBarColorProperty, value); }
        }

        public Color InactiveBarColor
        {
            get { return (Color)GetValue(InactiveBarColorProperty); }
            set { SetValue(InactiveBarColorProperty, value); }
        }

        public Color ControlBarColor
        {
            get { return (Color)GetValue(ControlBarColorProperty); }
            set { SetValue(ControlBarColorProperty, value); }
        }

        public bool NativeFeel
        {
            get { return (bool)GetValue(NativeFeelProperty); }
            set { SetValue(NativeFeelProperty, value); }
        }

        Timer _busyIndicatorTimer;
        double _busyIndicatorProgress;
        object _busyIndicatorLock = new object();
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set
            {
                Debug.WriteLine($@"SlideControl IsBusy: {value}");
                SetValue(IsBusyProperty, value);
                if (value)
                {
                    CreateBusyIndicatorTimer();
                }
                else
                {
                    lock (_busyIndicatorLock)
                    {
                        _busyIndicatorTimer?.Cancel();
                        _busyIndicatorTimer = null;
                    }
                }
            }
        }

        void CreateBusyIndicatorTimer()
        {
            lock (_busyIndicatorLock)
            {
                _busyIndicatorTimer?.Cancel();
                _busyIndicatorTimer = new Timer(state =>
                {
                    _busyIndicatorProgress += 0.1;
                    if (_busyIndicatorProgress >= 1)
                    {
                        _busyIndicatorProgress = 0;
                    }

                    InvalidateSurface();
                    if (IsBusy)
                    {
                        CreateBusyIndicatorTimer();
                    }
                }, null, 200, -1);
            }
        }


        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        public void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            using (SKPaint fillPaint = new SKPaint())
            using (SKPaint outlinePaint = new SKPaint())
            {
                SKPath path = new SKPath();
                float y = yGap * info.Height;
                double ratio = (Value - Minimum) / (Maximum - Minimum);
                float radius = info.Height / 2f;
                SKRect rect1 = new SKRect(0, y, info.Height - 2 * y, info.Height - y);
                SKRect rect2 = new SKRect(info.Width - info.Height + 2 * y, y, info.Width, info.Height - y);
                if (!NativeFeel)
                {
                    path.ArcTo(rect1, 90f, 180f, false);
                    path.LineTo(Math.Max((float)Math.Min(info.Width * ratio, info.Width - radius), radius), y);
                    path.LineTo(Math.Max((float)Math.Min(info.Width * ratio, info.Width - radius), radius), info.Height - y);
                    path.Close();
                }
                else
                {
                    path.MoveTo(0, radius);
                    path.LineTo((float)(info.Width * ratio), radius);
                }
                outlinePaint.Style = SKPaintStyle.Stroke;

                if (!NativeFeel)
                {
                    outlinePaint.StrokeWidth = 2;
                    outlinePaint.Color = new SKColor(255, 255, 255, 100);
                }
                else
                {
                    outlinePaint.StrokeWidth = 5;
                    outlinePaint.Color = Color.Red.ToSKColor();
                }
                outlinePaint.IsAntialias = true;

                fillPaint.Style = SKPaintStyle.Fill;
                fillPaint.Color = ActiveBarColor.ToSKColor();
                fillPaint.IsAntialias = true;

                if (!NativeFeel) canvas.DrawPath(path, fillPaint);
                canvas.DrawPath(path, outlinePaint);

                SKPath path1 = new SKPath();
                if (!NativeFeel)
                {
                    path1.ArcTo(rect2, 90f, -180f, false);
                    path1.LineTo(Math.Min((float)Math.Max(info.Width * ratio, radius), info.Width - radius), y);
                    path1.LineTo(Math.Min((float)Math.Max(info.Width * ratio, radius), info.Width - radius), info.Height - y);
                    path1.Close();
                }
                else
                {
                    path1.MoveTo((float)(info.Width * ratio), radius);
                    path1.LineTo(info.Width, radius);
                }

                fillPaint.Color = InactiveBarColor.ToSKColor();
                if (!NativeFeel)
                {
                    canvas.DrawPath(path1, fillPaint);
                }
                else
                {
                    outlinePaint.Color = Color.Gray.ToSKColor();
                }
                canvas.DrawPath(path1, outlinePaint);
                fillPaint.Color = !NativeFeel ? ControlBarColor.ToSKColor() : SKColors.White;

                SKPoint centerpt = new SKPoint((float)(info.Width * ratio), info.Height / 2f);
                if (centerpt.X - radius < 0)
                {
                    centerpt.X = radius;
                }

                if (centerpt.X + radius > info.Width)
                {
                    centerpt.X = info.Width - radius;
                }

                canvas.DrawCircle(centerpt.X, centerpt.Y, radius, fillPaint);
                if (!NativeFeel)
                {
                    canvas.DrawCircle(centerpt.X, centerpt.Y, radius, outlinePaint);
                }
                if (IsBusy)
                {
                    //CircularProgressControl.DrawProgressCircle(args, centerpt.X - radius, centerpt.Y - radius, 0, _busyIndicatorProgress, Color.Red, ControlBarColor, 1);
                }

            }
        }
    }
}
