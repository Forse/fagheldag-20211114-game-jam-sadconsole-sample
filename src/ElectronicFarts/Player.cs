using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SadConsole;

namespace ElectronicFarts
{
    public class Player : Actor
    {
        public Player(Color foreground, Color background, int glyph, int width =4, int height = 4) : base(foreground, background, glyph, width, height)
        {
            Health = 5;
        }
        
        public void TakeDamage()
        {
            Health -= 1;
        }

        public bool IsDead => Health <= 0;
    }
    
    public class Shot : Actor
    {
        public Shot(Color foreground, Color background, int glyph, int width = 4, int height = 4) : base(foreground, background, glyph, width, height)
        {
        }
    }

    public class PlayerGroup
    {
        public PlayerGroup(int floorValue, int centerValue)
        {
            Players = new List<Player>
            {
                new Player(Color.Yellow, Color.Transparent, 8, 4, 4),
                new Player(Color.Yellow, Color.Transparent, 8, 4, 4),
                new Player(Color.Yellow, Color.Transparent, 8, 4, 4),
                new Player(Color.Yellow, Color.Transparent, 8, 4, 4),
                new Player(Color.Yellow, Color.Transparent, 8, 4, 4),
                new Player(Color.Yellow, Color.Transparent, 8, 4, 4),
            };

            Players[0].Position = new Point(centerValue - 1, floorValue);
            Players[1].Position = new Point(centerValue, floorValue);
            Players[2].Position = new Point(centerValue + 1, floorValue);
            Players[3].Position = new Point(centerValue - 1, floorValue-1);
            Players[4].Position = new Point(centerValue, floorValue-1);
            Players[5].Position = new Point(centerValue + 1, floorValue-1);
        }
        
        public List<Player> Players { get; private set; }
        public bool IsShooting { get; set; }

        public bool MoveBy(Point p)
        {
            foreach (var player in Players)
            {
                if (!GameLoop.IsTileWalkable(player.Position + p))
                {
                    return false;
                }
            }
            foreach (var player in Players)
            {
                player.MoveBy(p);
            }
            return true;
        }

        public Player TakeDamage()
        {
            var playerToRemove = Players.LastOrDefault();
            if (playerToRemove == null) return null;
            Players.Remove(playerToRemove);
            return playerToRemove;
        }

        public bool IsHIt(Point target)
        {
            if (Players.Any(s => s.Position == target))
                return true;
            return false;
        }

        public int GetLeftValue()
        {
            var value = 1000;
            foreach (var player in Players)
            {
                if (player.Position.X < value)
                    value = player.Position.X;
            }

            return value;
        }
        
        public int GetRightValue()
        {
            var value = 0;
            foreach (var player in Players)
            {
                if (player.Position.X > value)
                    value = player.Position.X;
            }

            return value;
        }
    }
    
    public class Asteroid : Actor
    {
        public Asteroid(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph, width, height)
        {
        }
    }
    
    public class HealthEntity : Actor
    {
        public HealthEntity(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph, width, height)
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
