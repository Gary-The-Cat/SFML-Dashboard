using SFML.Graphics;

namespace Shared.ExtensionMethods
{
    public static class ColorExtensions
    {
        public static Color Darken(this Color color, double amount)
        {
            var darkenedColor = new Color();
            darkenedColor.R = color.R < amount ? (byte)color.R : (byte)(color.R - amount);
            darkenedColor.G = color.G < amount ? (byte)color.G : (byte)(color.G - amount);
            darkenedColor.B = color.B < amount ? (byte)color.B : (byte)(color.B - amount);
            darkenedColor.A = byte.MaxValue;

            return darkenedColor;
        }

        public static Color ToSfmlColour(this System.Drawing.Color color)
        {
            var colour = new Color();
            colour.R = color.R;
            colour.G = color.G;
            colour.B = color.B;
            colour.A = byte.MaxValue;

            return colour;
        }
    }
}
