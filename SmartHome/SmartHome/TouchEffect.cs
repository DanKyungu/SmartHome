using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SmartHome
{
    public class TouchEffect : RoutingEffect
    {
        public delegate void TouchActionEventHandler(object sender, TouchActionEventArgs args);

        public event TouchActionEventHandler TouchAction;

        public TouchEffect() : base("SmartHome.TouchEffect")
        {
        }

        public bool Capture { set; get; }

        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}
