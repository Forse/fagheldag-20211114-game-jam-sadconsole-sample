using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ElectronicFarts
{
    public class PlayerGroup
    {
        public PlayerGroup(int floorValue, int centerValue)
        {
            Players = new List<Player>
            {
                new(Color.Yellow, Color.Transparent, 8, 4, 4),
                new(Color.Yellow, Color.Transparent, 8, 4, 4),
                new(Color.Yellow, Color.Transparent, 8, 4, 4),
                new(Color.Yellow, Color.Transparent, 8, 4, 4),
                new(Color.Yellow, Color.Transparent, 8, 4, 4),
                new(Color.Yellow, Color.Transparent, 8, 4, 4),
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
        
        public int GetBottomValue()
        {
            var value = 1000;
            foreach (var player in Players)
            {
                if (player.Position.Y < value)
                    value = player.Position.Y;
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
}