using System;
using System.Diagnostics;
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
        private static Asteroid _asteroid;
        
        private static TileBase[] _tiles;
        private const int roomStartY = 1;
        private const int roomStartX = 2;
        private const int _roomWidth = 30; 
        private const int _roomHeight = 30;

        private const int _floorYValue = 29;

        private static Stopwatch _stopwatch;
        public static Console startingConsole;

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
            
            if(_stopwatch.IsRunning == false)
                _stopwatch.Start();

            if (_stopwatch.Elapsed.TotalMilliseconds > 200)
            {
                if (_asteroid.Position.Y == _floorYValue)
                {
                    //Remove it
                    startingConsole.Children.Remove(_asteroid);
                    CreateAsteroid();
                    
                }
                _asteroid.MoveBy(new Point(0, 1));
                _stopwatch.Restart();
            }
            
            //_asteroid.MoveBy(new Point(0, 1));
            
            System.Console.WriteLine(_asteroid.Position.X);


        }

        private static void Init()
        {
            CreateWalls();
            CreateFloors();
            
            startingConsole = new ScrollingConsole(Width, Height, Global.FontDefault, new Rectangle(0, 0, Width, Height));

            SadConsole.Global.CurrentScreen = startingConsole;
            CreatePlayer();
            CreateAsteroid();
            
            

            _stopwatch = new Stopwatch();
        }

        private static void CreatePlayer()
        {
            player = new Player(Color.Yellow, Color.Transparent, 1, 4, 4);
            player.Position = new Point(14, _floorYValue);
            startingConsole.Children.Add(player);
        }

        private static void CreateAsteroid()
        {
            _asteroid = new Asteroid(Color.Red, Color.Transparent, 1, 2, 2);
            var rand = new Random();
            var startPosition = rand.Next(1, 29);
            _asteroid.Position = new Point(startPosition, 1);
            startingConsole.Children.Add(_asteroid);
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
