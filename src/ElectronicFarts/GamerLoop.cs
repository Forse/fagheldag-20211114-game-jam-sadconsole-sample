﻿using System;
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
        private static PlayerGroup playerGroup;
        private static BossGroup _bossGroup;
        private static List<Asteroid> _asteroids = new();
        private static List<Shot> _shots = new();
        
        private static TileBase[] _tiles;
        private const int roomStartY = 1;
        private const int roomStartX = 1;
        private const int _roomWidth = 55; 
        private const int _roomHeight = 50;

        private const int _floorYValue = 49;

        private static Stopwatch _gameStopWatch;
        private static Stopwatch _asteroidStopWatch;
        public static Console startingConsole;
        private static Console _introConsole;
        private static bool isGameOver = false;
        private static bool isGameStarted = false;

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

            if (isGameStarted)
            {
                HandleGameMechanics(obj);
            }
            HandleKeyboardInput(obj);
        }

        private static void ShowIntro()
        {
            var textConsole = new Console(Width - 4, 30, Global.FontDefault);
            textConsole.Position = new Point(2, 5);
            _introConsole.Children.Add(textConsole);
            textConsole.Print(0, 0, "A long time ago in a galaxy far far away... ", Color.Cyan, Color.Transparent);
            textConsole.Cursor.Position = new Point(0, 3);
            textConsole.Cursor.Print("No wait. It's present and in the same galaxy you are in now. You have escaped from the earth after all the world's police are after you. You decide to go to Mars to start a new life. But as you are about to land on the red planet, you are shot by laser cannons from the little hospitable race called the Marsipans. You have no choice but to shoot back hoping to land on the more peaceful part of the planet.\r\n\r\nYou are pretty much fucked...");
            _introConsole.Fill(new Rectangle(0, 30, Width, 5), Color.Transparent, Color.Yellow, ' ');
            _introConsole.Print(25, 32, $"MARS WARS", ColorAnsi.Black);
            _introConsole.Print(2, 50, "An Electronic Farts game", Color.White);
        }

        private static void HandleGameMechanics(GameTime obj)
        {
            if (_gameStopWatch.IsRunning == false)
                _gameStopWatch.Start();
            
            if (_asteroidStopWatch.IsRunning == false)
                _asteroidStopWatch.Start();
            
            foreach (var asteroid in _asteroids.ToList())
            {
                var asteroidHit = false;
                foreach (var shot in _shots.ToList())
                {
                    if (shot.Position == asteroid.Position)
                    {
                        startingConsole.Children.Remove(asteroid);
                        _asteroids.Remove(asteroid);
                        startingConsole.Children.Remove(shot);
                        _shots.Remove(shot);
                        asteroidHit = true;
                        break;
                    }
                }

                if (!asteroidHit)
                {
                    if (playerGroup.IsHIt(asteroid.Position))
                    {
                        //Collision
                        var deadGroupPlayer = playerGroup.TakeDamage();

                        if (deadGroupPlayer != null)
                        {
                            startingConsole.Children.Remove(deadGroupPlayer);
                        }
                        if (playerGroup.Players.Count == 0)
                        {
                            isGameOver = true;
                        }
                    }
                }
            }
            
            if (IsGameTick().Item1)
            {
                foreach (var shot in _shots.ToList())
                {
                    if (shot.Position.Y == 1)
                    {
                        _shots.Remove(shot);
                        startingConsole.Children.Remove(shot);
                    }
                    else
                    {
                        shot.MoveBy(new Point(0, -1));
                    }
                }

                _gameStopWatch.Restart();
            }

            if (IsAsteroidTick(obj).Item1)
            {
                playerGroup.IsShooting = false;

                if (IsBossTime(obj))
                {
                    //Boss level
                    // Create Boss
                    CreateBoss();

                }

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
                        if (asteroid.HeatSeeking && asteroid.Position.X > playerGroup.GetLeftValue())
                        {
                            asteroid.MoveBy(new Point(-1, 1));
                        }
                        else if (asteroid.HeatSeeking && asteroid.Position.X < playerGroup.GetRightValue())
                        {
                            asteroid.MoveBy(new Point(1, 1));
                        }
                        else
                        {
                            asteroid.MoveBy(new Point(0, 1));
                        }
                    }
                }
                
                if (new Random().Next(1, 100) > IsAsteroidTick(obj).Item2 && !IsBossTime(obj))
                {
                    CreateAsteroid();
                }
                _asteroidStopWatch.Restart();
            }
        }

        private static void HandleKeyboardInput(GameTime gameTime)
        {
            if (Global.KeyboardState.KeysPressed.Any())
            {
                StartGame();
            }
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
            
            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up)
                || Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                playerGroup.MoveBy(new Point(0, -1));
            }
            
            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down)
                || Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                playerGroup.MoveBy(new Point(0, 1));
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space)
                || Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                Shoot();
            }
        }

        private static void StartGame()
        {
            if (!isGameStarted)
            {
                isGameStarted = true;
                Global.CurrentScreen = startingConsole;
                CreateBackgroundMusic();
            }
        }

        private static void Shoot()
        {
            if (playerGroup.IsShooting)
            {
                return;
            }
            var shot = new Shot(Color.YellowGreen, Color.Transparent, 6, 2, 2)
            {
                Position = new Point(playerGroup.GetLeftValue() , playerGroup.GetBottomValue() - 1)
            };
            startingConsole.Children.Add(shot);
            _shots.Add(shot);
            playerGroup.IsShooting = true;
        }


        private static (bool,int) IsAsteroidTick(GameTime gameTime)
        {
            return gameTime.TotalGameTime switch
            {
                { TotalSeconds: < 20 } => (_asteroidStopWatch.Elapsed.TotalMilliseconds > 200, 90),
                { TotalSeconds: < 40 } => (_asteroidStopWatch.Elapsed.TotalMilliseconds > 150, 70),
                { TotalSeconds: < 60 } => (_asteroidStopWatch.Elapsed.TotalMilliseconds > 100, 50),
                _ => (_asteroidStopWatch.Elapsed.TotalMilliseconds > 75, 20)
            };
        }
        
        private static (bool,int) IsGameTick()
        {
            return (_gameStopWatch.Elapsed.TotalMilliseconds > 75, 20);
        }

        private static bool IsBossTime(GameTime gameTime)
        {
            return gameTime.TotalGameTime.TotalSeconds > 10;
        }

        private static void Init()
        {
            CreateBackgroundMusic();
            CreateWalls();
            CreateFloors();
            startingConsole = new ScrollingConsole(Width, Height, Global.FontDefault, new Rectangle(0, 0, Width, Height));
            _introConsole = new Console(Width, Height, Global.FontDefault);
            Global.CurrentScreen = _introConsole;
            CreatePlayer();
            ShowIntro();
            _gameStopWatch = new Stopwatch();
            _asteroidStopWatch = new Stopwatch();
        }

        private static void CreatePlayer()
        {
            playerGroup = new PlayerGroup(_floorYValue, 14);
            foreach (var groupPlayer in playerGroup.Players)
            {
                startingConsole.Children.Add(groupPlayer);
            }
        }
        
        private static void CreateBoss()
        {
            _bossGroup = new BossGroup(1, 14);
            foreach (var segment in _bossGroup.BossSegments)
            {
                startingConsole.Children.Add(segment);
            }
        }

        private static void CreateAsteroid()
        {
            var heatSeeking = new Random().Next(1, 100) >= 80;
            var asteroid = new Asteroid(heatSeeking ? Color.Purple : Color.Red, Color.Transparent, heatSeeking ? 216 : 179, 2, 2)
            {
                HeatSeeking = heatSeeking
            };
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