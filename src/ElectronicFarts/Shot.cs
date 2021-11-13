using Microsoft.Xna.Framework;

namespace ElectronicFarts
{
    public class Shot : Actor
    {
        public Shot(Color foreground, Color background, int glyph, int width = 4, int height = 4) : base(foreground, background, glyph, width, height)
        {
        }
    }
}