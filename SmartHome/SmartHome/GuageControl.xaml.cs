using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartHome
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuageControl : ContentView
    {
        private static int startDegree = 160; //135
        private static int endDegree = 220; //270
        private float radius, cx, cy = 0;
        private float circleMidX;
        private float circleMidY;
        private static bool usePercentage = true;
        private double angle = 0.0;

        public static readonly BindableProperty PercentProperty = BindableProperty.Create(
                                                         propertyName: "Percent",
                                                         returnType: typeof(double),
                                                         declaringType: typeof(GuageControl),
                                                         defaultValue: 0.0,
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         propertyChanged: titleTextPropertyChanged);

        private static void titleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var currentGuage = (GuageControl)bindable;
            currentGuage.Percent = (double)newValue < 0 && (double)newValue > 100 ? 0.0 : (double)newValue;
            usePercentage = true;
            currentGuage.DefaultCanvas.InvalidateSurface();
        }

        public double Percent
        {
            get { return (double)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }
        SKPaint gaugleDefaultColor = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = Color.LightGray.ToSKColor(),
            StrokeWidth = 35,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        SKPaint gaugleFillColor = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = Color.FromHex("2B4BEC").ToSKColor(),
            StrokeWidth = 40,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        SKPaint rectFillColor = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = Color.FromHex("F03226").ToSKColor(),
            IsAntialias = true
        };

        SKPaint blackFillColor = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3,
            Color = Color.Red.ToSKColor(),
            IsAntialias = true
        };

        public GuageControl()
        {
            InitializeComponent();
        }

        private void DefaultCanvas_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Skia stuff happens
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            var minSize = Math.Min(info.Size.Height / 2, info.Size.Width / 2);
            var rulesDistance = 5;

            canvas.Clear();

            SKRect containerRectangle = new SKRect(0, 0, minSize, minSize);
            canvas.Translate((e.Info.Width / 2) - containerRectangle.MidX, (e.Info.Height / 2) - containerRectangle.MidY);

            using (SKPath path = new SKPath())
            {
                path.AddArc(containerRectangle, startDegree, endDegree);
                canvas.DrawPath(path, gaugleDefaultColor);
            }

            using (SKPath path = new SKPath())
            {
                var anglePercent = (endDegree * Percent) / 100;
                path.AddArc(containerRectangle, startDegree, (float)anglePercent);
                canvas.DrawPath(path, gaugleFillColor);
            }

            circleMidX = containerRectangle.MidX;
            circleMidY = containerRectangle.MidY;

            canvas.Translate(circleMidX, circleMidY);
             
            canvas.DrawCircle(0, 0, 8, rectFillColor);

            canvas.RotateDegrees(360);
            if (usePercentage) 
                angle = Math.PI * (startDegree + (endDegree * Percent) / 100) / 180.0;

            radius = containerRectangle.Height / 2;
            cx = (float)(radius * Math.Cos(angle));
            cy = (float)(radius * Math.Sin(angle));

            canvas.DrawCircle(cx,cy, 35, rectFillColor); // x=cos(angle)*rayon et y =sin(angle)*rayon

            canvas.RotateDegrees(startDegree - 45);
            for (int angle = 0; angle <= endDegree; angle += 5)
            {
                var size = (float)(containerRectangle.Height / 2.5) + rulesDistance;
                canvas.DrawLine(size, size, size + 10, size + 10, blackFillColor);
                canvas.RotateDegrees(5);
            }
        }

        private void TouchEffect_TouchAction(object sender, TouchActionEventArgs args)
        {
            Vector2 point = new Vector2()
            {
                X = (float)args.Location.X,
                Y = (float)args.Location.Y
            };

            Vector2 center = new Vector2()
            {
                X = (float)circleMidX,
                Y = (float)circleMidY
            };

            Vector2 relPoint = point - center;

            var angle = GetAngle(point, center);
            //if (angle < 0)
            //{
            //    angle = (angle + (Math.PI * 2));
            //}
            usePercentage = false;
            this.angle = angle;
            DefaultCanvas.InvalidateSurface();
            //var value = (angle / endDegree) * 100;
            //Percent = value;
        }

        //https://stackoverflow.com/questions/54280085/calculate-degrees-of-angle-using-x-y-coordinates
        public static double GetAngle(Vector2 point, Vector2 center)
        {
            Vector2 relPoint = point - center;
            return (ToDegrees((float)Math.Atan2((double)relPoint.Y, (double)relPoint.X)) + 450f) % (float)endDegree;
        }

        public static double ToDegrees(float radians) => radians * 180f / Math.PI;
    }
}