using Microsoft.Xna.Framework;

namespace ElectronicFarts
{
    public class Laser : Actor
    {
        public bool HeatSeeking { get; set; }
        public Laser(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph, width, height)
        {
        }
    }
}