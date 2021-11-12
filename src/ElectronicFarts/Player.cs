using System;
using Microsoft.Xna.Framework;
using SadConsole;

namespace ElectronicFarts
{
    public class Player : Actor
    {
        public Player(Color foreground, Color background, int glyph, int width =4, int height = 4) : base(foreground, background, glyph, width, height)
        {
        }
    }
    
    public abstract class TileBase : Cell
    {

        public bool IsBlockingMove;
        public bool IsBlockingLOS;

        protected string Name;

        public TileBase(Color foreground, Color background, int glyph, bool blockingMove=false, bool blockingLOS=false, String name="") : base(foreground, background, glyph)
        {
            IsBlockingMove = blockingMove;
            IsBlockingLOS = blockingLOS;
            Name = name;
        }
    }
    
    public class TileFloor : TileBase
    {
        public TileFloor(bool blocksMovement = false, bool blocksLOS = false) : base(Color.DarkGray, Color.Transparent, '.', blocksMovement, blocksLOS)
        {
            Name = "Floor";
        }
    }
    
    public class TileWall : TileBase
    {
        public TileWall( int glyph, bool blocksMovement=true, bool blocksLOS=true) : base(Color.LightGray, Color.Transparent, glyph, blocksMovement, blocksLOS)
        {
            Name = "Wall";
        }
    }
}
