using Microsoft.Xna.Framework;

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
}
