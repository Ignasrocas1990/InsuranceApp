using SkiaSharp;

using c = SkiaSharp.SKColors;

namespace Insurance_app.SupportClasses
{
    public static class StaticOptions
    {
        public static string MyRealmAppId = "application-0-bvutx";
        public static readonly double StepNeeded = 10000;
        public static readonly int MovUpdateArraySize = 5;

        public static readonly SKColor White = c.WhiteSmoke;
        
        public static readonly SKColor[] ChartColors=
        {
            c.Blue,c.LightBlue,c.Red,c.Aqua,c.Black,c.LightGreen
            ,c.MediumVioletRed,c.Yellow,c.Orange,c.Green,c.Firebrick
        };


    }
}