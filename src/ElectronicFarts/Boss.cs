using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ElectronicFarts
{
    public class BossSegment : Actor
    {
        public BossSegment(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph, width, height)
        {
        }
    }

    public class BossGroup
    {
        public BossGroup(int floorValue, int centerValue)
        {
            BossSegments = new List<BossSegment>
            {
                new BossSegment(Color.Blue, Color.Transparent, 8, 4, 4),
                new BossSegment(Color.Blue, Color.Transparent, 8, 4, 4),
                new BossSegment(Color.Blue, Color.Transparent, 8, 4, 4),
                new BossSegment(Color.Blue, Color.Transparent, 8, 4, 4),
                new BossSegment(Color.Blue, Color.Transparent, 8, 4, 4),
                new BossSegment(Color.Blue, Color.Transparent, 8, 4, 4),
            };

            BossSegments[0].Position = new Point(centerValue - 1, floorValue);
            BossSegments[1].Position = new Point(centerValue, floorValue);
            BossSegments[2].Position = new Point(centerValue + 1, floorValue);
            BossSegments[3].Position = new Point(centerValue - 1, floorValue-1);
            BossSegments[4].Position = new Point(centerValue, floorValue-1);
            BossSegments[5].Position = new Point(centerValue + 1, floorValue-1);
        }
        
        public List<BossSegment> BossSegments { get; private set; }
        
        public bool MoveBy(Point p)
        {
            foreach (var segment in BossSegments)
            {
                if (!GameLoop.IsTileWalkable(segment.Position + p))
                {
                    return false;
                }
            }
            foreach (var segment in BossSegments)
            {
                segment.MoveBy(p);
            }
            return true;
        }
        
        public BossSegment TakeDamage()
        {
            var playerToRemove = BossSegments.LastOrDefault();
            if (playerToRemove == null) return null;
            BossSegments.Remove(playerToRemove);
            return playerToRemove;
        }
        
        public bool IsHIt(Point target)
        {
            if (BossSegments.Any(s => s.Position == target))
                return true;
            return false;
        }
    }
}