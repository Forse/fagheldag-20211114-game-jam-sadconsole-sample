using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ElectronicFarts
{
    public class SpaceShip
    {
        public SpaceShip(int floorValue, int centerValue)
        {
            Players = new List<Player>
            {
                new(Color.Yellow, Color.Transparent, 215, 4, 4), // 1
                new(Color.Yellow, Color.Transparent, 215, 4, 4), // 4
                new(Color.Yellow, Color.Transparent, 199, 4, 4), // 0
                new(Color.Yellow, Color.Transparent, 182, 4, 4), // 2
                new(Color.Yellow, Color.Transparent, 214, 4, 4), // 3
                new(Color.Yellow, Color.Transparent, 183, 4, 4), // 5
            };

            Players[0].Position = new Point(centerValue, floorValue); // 1
            Players[1].Position = new Point(centerValue, floorValue-1); // 4
            Players[2].Position = new Point(centerValue - 1, floorValue); // 0
            Players[3].Position = new Point(centerValue + 1, floorValue); // 2
            Players[4].Position = new Point(centerValue - 1, floorValue-1); // 3
            Players[5].Position = new Point(centerValue + 1, floorValue-1); // 5
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
            return Players.Any(s => s.Position == target);
        }

        public int GetLeftValue()
        {
            return Players.Select(player => player.Position.X).Prepend(1000).Min();
        }
        
        public int GetBottomValue()
        {
            return Players.Select(player => player.Position.Y).Prepend(1000).Min();
        }
        
        public int GetRightValue()
        {
            return Players.Select(player => player.Position.X).Prepend(0).Max();
        }
    }
}