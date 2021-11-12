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
        private static Player player;
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
                startingConsole.Print(1, 12, "Game over dickhead", ColorAnsi.CyanBright);
                return;
            }
            
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
                        if (asteroid.Position == player.Position)
                        {
                            //Collision
                            player.TakeDamage();
                            
                            startingConsole.Children.Remove(health[player.Health]);
                            health.RemoveAt(player.Health);
                            //startingConsole.Print(1, 1, $"Health: {player.Health}", ColorAnsi.CyanBright);

                            if (player.IsDead) isGameOver = true;
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

        private static (bool,int) IsGameTick(GameTime gameTime)
        {
            switch (gameTime.TotalGameTime)
            {
                case var s when s.TotalSeconds < 30:
                    return (_stopwatch.Elapsed.TotalMilliseconds > 200,90);
                case var s when s.TotalSeconds < 60:
                    return (_stopwatch.Elapsed.TotalMilliseconds > 150,70);
                default:
                    return (_stopwatch.Elapsed.TotalMilliseconds > 100,50);
                    
            }
        }

        private static void Init()
        {
            CreateWalls();
            CreateFloors();
            
            startingConsole = new ScrollingConsole(Width, Height, Global.FontDefault, new Rectangle(0, 0, Width, Height));

            Global.CurrentScreen = startingConsole;
            CreatePlayer();
            CreateHealth();
            _stopwatch = new Stopwatch();
            //startingConsole.Print(1, 1, $"Health: {player.Health}", ColorAnsi.CyanBright);
        }

        private static void CreatePlayer()
        {
            player = new Player(Color.Yellow, Color.Transparent, 1, 4, 4);
            player.Position = new Point(14, _floorYValue);
            startingConsole.Children.Add(player);
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
                health[i].Position = new Point(0, i);
                startingConsole.Children.Add(health[i]);
            }
        }

        private static void CreateAsteroid()
        {
            var asteroid = new Asteroid(Color.Red, Color.Transparent, 1, 2, 2);
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
        
        public static bool IsTileWalkable(Point location)
        {
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false;
            return !_tiles[location.Y * Width + location.X].IsBlockingMove;
        }
    }
}