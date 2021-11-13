using Microsoft.Xna.Framework;

namespace ElectronicFarts
{
    public class TileWall : TileBase
    {
        public TileWall( int glyph, bool blocksMovement=true, bool blocksLOS=true) : base(Color.LightGray, Color.Transparent, glyph, blocksMovement, blocksLOS)
        {
            Name = "Wall";
        }
    }
}