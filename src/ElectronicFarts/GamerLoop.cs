using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using SadConsole;
using Console = SadConsole.Console;

namespace ElectronicFarts
{
    public class GameLoop
    {
        public const int Width = 60;
        public const int Height = 60;
        private static Player player;
        private static PlayerGroup playerGroup;
        private static List<Asteroid> _asteroids = new();
        
        private static TileBase[] _tiles;
        private const int roomStartY = 1;
        private const int roomStartX = 1;
        private const int _roomWidth = 50; 
        private const int _roomHeight = 50;

        private const int _floorYValue = 49;

        private static Stopwatch _stopwatch;
        public static Console startingConsole;
        private static List<HealthEntity> health;
        private static bool isGameOver = false;

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
            if (isGameOver)
            {
                var score = obj.TotalGameTime.TotalSeconds.ToString("0000");
                startingConsole.Print(1, 12, $"Game over dickhead.", ColorAnsi.CyanBright);
                return;
            }
            
            HandleGameMechanics(obj);
            HandleKeyboardInput();
        }

        private static void HandleGameMechanics(GameTime obj)
        {
            if (_stopwatch.IsRunning == false)
                _stopwatch.Start();

            if (IsGameTick(obj).Item1)
            {
                foreach (var asteroid in _asteroids.ToList())
                {
                    if (asteroid.Position.Y == _floorYValue)
                    {
                        //Remove it
                        startingConsole.Children.Remove(asteroid);
                        _asteroids.Remove(asteroid);
                    }
                    else
                    {
                        asteroid.MoveBy(new Point(0, 1));
                        if (playerGroup.IsHIt(asteroid.Position))
                        {
                            //Collision
                            var deadGroupPlayer = playerGroup.TakeDamage();

                            if (deadGroupPlayer != null)
                            {
                                startingConsole.Children.Remove(deadGroupPlayer);
                            }
                            if(playerGroup.Players.Count== 0)
                            {
                                isGameOver = true;
                            }
                        }
                    }
                }

                if (new Random().Next(1, 100) > IsGameTick(obj).Item2)
                {
                    CreateAsteroid();
                }

                _stopwatch.Restart();
            }
        }

        private static void HandleKeyboardInput()
        {
            if (Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                Settings.ToggleFullScreen();
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left) 
                || Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                playerGroup.MoveBy(new Point(-1, 0));
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right)
                || Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                playerGroup.MoveBy(new Point(1, 0));
            }
        }

        private static (bool,int) IsGameTick(GameTime gameTime)
        {
            return gameTime.TotalGameTime switch
            {
                { TotalSeconds: < 20 } => (_stopwatch.Elapsed.TotalMilliseconds > 200, 90),
                { TotalSeconds: < 40 } => (_stopwatch.Elapsed.TotalMilliseconds > 150, 70),
                { TotalSeconds: < 60 } => (_stopwatch.Elapsed.TotalMilliseconds > 100, 50),
                _ => (_stopwatch.Elapsed.TotalMilliseconds > 75, 20)
            };
        }

        private static void Init()
        {
            CreateBackgroundMusic();
            CreateWalls();
            CreateFloors();
            startingConsole = new ScrollingConsole(Width, Height, Global.FontDefault, new Rectangle(0, 0, Width, Height));
            Global.CurrentScreen = startingConsole;
            CreatePlayer();
            CreateHealth();
            _stopwatch = new Stopwatch();
        }

        private static void CreatePlayer()
        {
            // player = new Player(Color.Yellow, Color.Transparent, 30, 4, 4);
            // player.Position = new Point(14, _floorYValue);
            // startingConsole.Children.Add(player);

            playerGroup = new PlayerGroup(_floorYValue, 14);
            foreach (var groupPlayer in playerGroup.Players)
            {
                startingConsole.Children.Add(groupPlayer);
            }
        }
        
        private static void CreateHealth()
        {
            health = new List<HealthEntity>
            {
                new HealthEntity(Color.Red, Color.Transparent,3,2,2),
                new HealthEntity(Color.Red, Color.Transparent,3,2,2),
                new HealthEntity(Color.Red, Color.Transparent,3,2,2),
                new HealthEntity(Color.Red, Color.Transparent,3,2,2),
                new HealthEntity(Color.Red, Color.Transparent,3,2,2),
            };
            for (int i = 0; i < health.Count; i++)
            {
                health[i].Position = new Point(i, _floorYValue+5);
                startingConsole.Children.Add(health[i]);
            }
        }

        private static void CreateAsteroid()
        {
            var assRand = new Random().Next(1, 32);
            var asteroid = new Asteroid(Color.Red, Color.Transparent, assRand, 2, 2);
            var startPosition = new Random().Next(roomStartX, _roomWidth-1);
            asteroid.Position = new Point(startPosition, 1);
            startingConsole.Children.Add(asteroid);
            _asteroids.Add(asteroid);
        }
        
        private static void CreateFloors()
        {
            for (var x = roomStartX; x < _roomWidth; x++)
            {
                for (var y = roomStartY; y < _roomHeight; y++)
                {
                    _tiles[y * Width + x] = new TileFloor();
                }
            }
        }
        
        private static void CreateWalls()
        {
            var wallAlternator = 0;
            var wallCharacters = "$#$%";
            _tiles = new TileBase[Width * Height];

            for (var i = 0; i < _tiles.Length; i++)
            {
                _tiles[i] = new TileWall(wallCharacters[wallAlternator]);
                wallAlternator++;
                if (wallAlternator == wallCharacters.Length)
                {
                    wallAlternator = 0;
                }
            }
        }

        private static void CreateBackgroundMusic()
        {
            var musicPlaybackService = new DefaultBackgroundMusicPlaybackService(SadConsole.Game.Instance.Content);
            musicPlaybackService.StartBackgroundMusic("gamemusic");
        }

        public static bool IsTileWalkable(Point location)
        {
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false;
            return !_tiles[location.Y * Width + location.X].IsBlockingMove;
        }
    }
}