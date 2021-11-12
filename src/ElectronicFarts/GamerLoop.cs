using System;
using Microsoft.Xna.Framework;
using SadConsole;
using Console = SadConsole.Console;

namespace ElectronicFarts
{
    public class GameLoop
    {
        public const int Width = 40;
        public const int Height = 40;
        private static Player player;
        
        private static TileBase[] _tiles;
        private const int roomStartY = 29;
        private const int roomStartX = 14;
        private const int _roomWidth = 30; 
        private const int _roomHeight = 30;

        static void Main(string[] args)
        {
            SadConsole.Game.Create(Width, Height);
            SadConsole.Game.OnInitialize = Init;
            SadConsole.Game.OnUpdate = Update;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime obj)
        {
            if (Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                Settings.ToggleFullScreen();
            }

            //if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            //{
            //    player.MoveBy(new Point(0, -1));
            //}

            //if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            //{
            //    player.MoveBy(new Point(0, 1));
            //}

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                player.MoveBy(new Point(-1, 0));
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                player.MoveBy(new Point(1, 0));
            }
        }

        private static void Init()
        {
            CreateWalls();
            CreateFloors();
            Console startingConsole = new ScrollingConsole(Width, Height, Global.FontDefault, new Rectangle(0, 0, Width, Height));
            
            SadConsole.Global.CurrentScreen = startingConsole;
            CreatePlayer();
            startingConsole.Children.Add(player);
        }

        private static void CreatePlayer()
        {
            player = new Player(Color.Yellow, Color.Transparent, 1, 4, 4);
            player.Position = new Point(14, 29);
        }
        
        private static void CreateFloors()
        {
            for (int x = roomStartX; x < _roomWidth; x++)
            {
                for (int y = roomStartY; y < _roomHeight; y++)
                {
                    _tiles[y * Width + x] = new TileFloor();
                }
            }
        }
        
        private static void CreateWalls()
        {
            int wallAlternator = 0;
            string wallCharacters = "$#$%";
            _tiles = new TileBase[Width * Height];

            for (int i = 0; i < _tiles.Length; i++)
            {
                _tiles[i] = new TileWall(wallCharacters[wallAlternator]);
                wallAlternator++;
                if (wallAlternator == wallCharacters.Length)
                {
                    wallAlternator = 0;
                }
            }
        }
        
        public static bool IsTileWalkable(Point location)
        {
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false;
            return !_tiles[location.Y * Width + location.X].IsBlockingMove;
        }
    }
}
