using System;
using ChineseJourney.Common.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace ChineseJourney.Common.View
{
    public class PaintEventArgs : SKPaintSurfaceEventArgs
    {
        public PaintEventArgs(SKSurface surface, SKImageInfo info, SKRect rect, SKSize size) : base(surface, info)
        {
            Rect = rect;
            Size = size;
        }

        public SKRect Rect { set; get; }

        public SKSize Size { set; get; }
    }

    public interface IImageDisplayControl
    {
        double ZoomFactor { get; set; }
        SKRect Bound { get; set; }

        event EventHandler<PaintEventArgs> OnPaint;
    }
    public class SkiaImage : SKCanvasView, IImageDisplayControl
    {
        public SkiaImage()
        {
            PaintSurface += ImageDisplayControl_PaintSurface;
        }

        public static BindableProperty ImageProperty = BindableProperty.Create(@"Image", typeof(SKBitmap), typeof(SkiaImage), null, propertyChanged: ImageChanged);
        public static readonly BindableProperty ForceToLeftProperty = BindableProperty.Create(@"ForceToLeft", typeof(bool), typeof(SkiaImage), false);

        public bool ForceToLeft
        {
            get { return (bool)GetValue(ForceToLeftProperty); }
            set { SetValue(ForceToLeftProperty, value); }
        }

        private SKBitmap _image;

        public double ZoomFactor { get; set; } = 1.0f;
        private static void ImageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as SkiaImage;
            if (c != null)
            {
                c._image = newValue as SKBitmap;
                c.InvalidateSurface();
            }
        }

        public SKBitmap Image => _image;

        public SKRect Bound { get; set; }

        void ImageDisplayControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_image != null)
            {
                SKRect rect;
                DrawingHelper.Draw(e, _image, ForceToLeft, out rect);
                ZoomFactor = _image.Width / rect.Width;
                Bound = rect;
                OnPaint?.Invoke(this, new PaintEventArgs(e.Surface, e.Info, rect, new SKSize(_image.Width, _image.Height)));
            }
        }

        public event EventHandler<PaintEventArgs> OnPaint;
    }
}