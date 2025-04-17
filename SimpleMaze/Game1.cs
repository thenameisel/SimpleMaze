using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;

namespace SimpleMaze
{
    enum Scene
    {
        MAIN,
        GAME,
        PAUSE,
        END
    };

    public class Game1 : Game
    {

        private TimeSpan _gameTimeElapsed;
        private bool _timerRunning;
        private SpriteFont _timerFont;

        bool winCondition = false;

        private float _speed = 100f;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Scene _scene = Scene.MAIN;

        private Texture2D tileAtlas;
        int _playerTextureSize = 55;
        int _enviroSize = 64;

        private Camera _camera;
        private Sprite player;
        private SpriteFont menuFont;

        private Dictionary<Vector2, int> tileMap;
        private Dictionary<Vector2, int> tileMapCol;
        private List<Rectangle> textureStore;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            int baseWidth = 800;
            int baseHeight = 480;

            int scale = 1;

            _graphics.PreferredBackBufferWidth = baseWidth * scale;
            _graphics.PreferredBackBufferHeight = baseHeight * scale;

            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.GraphicsProfile = GraphicsProfile.Reach;
            _graphics.HardwareModeSwitch = false;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _gameTimeElapsed = TimeSpan.FromMinutes(2);
            _timerRunning = false;

            var map02 = Path.Combine(Content.RootDirectory, "map02.csv");
            tileMap = LoadMap(map02);
            var map02Col = Path.Combine(Content.RootDirectory, "mapCol02.csv");
            tileMapCol = LoadMap(map02Col);
            textureStore = new()
            {
                //floor tiles
                new Rectangle(0, 0, _enviroSize,_enviroSize),
                new Rectangle(_enviroSize, 0, _enviroSize,_enviroSize),
                new Rectangle(_enviroSize*2, 0, _enviroSize,_enviroSize),
                new Rectangle(_enviroSize*3, 0, _enviroSize,_enviroSize),
                new Rectangle(0, _enviroSize, _enviroSize,_enviroSize),
                new Rectangle(),
                new Rectangle(),
                new Rectangle(),
                //wall tiles
                new Rectangle(0, _enviroSize*2, _enviroSize,_enviroSize),
                new Rectangle(_enviroSize, _enviroSize*2, _enviroSize,_enviroSize),
                new Rectangle(),
                new Rectangle(),
                //misc tiles
                new Rectangle(0, _enviroSize*3, _enviroSize,_enviroSize),
                new Rectangle(),
                new Rectangle(),
                new Rectangle(),
            };
        }


        protected override void Initialize()
        {
            _camera = new Camera(_graphics.GraphicsDevice.Viewport);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {

            menuFont = Content.Load<SpriteFont>("File");
            
            tileAtlas = Content.Load<Texture2D>("enviromentatlas"); 
            
            player = new Player(Content.Load<Texture2D>("shroomcatatlas"), new Vector2(64, 64), _speed, _playerTextureSize, (bounds) => IsColliding(bounds));
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (_timerRunning && _scene == Scene.GAME)
            {
                _gameTimeElapsed -= gameTime.ElapsedGameTime;
            }


            switch (_scene)
            {
                case Scene.MAIN:
                    // Main menu logic
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        _scene = Scene.GAME;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

                    _timerRunning = false; 
                    break;

                case Scene.GAME:
                    // Game logic

                    if (Keyboard.GetState().IsKeyDown(Keys.P)) _scene = Scene.PAUSE;
                    
                    _timerRunning = true;

                    player.Update(gameTime);

                    _camera.Update(player.position, player.textureSize);

                    //TODO: this needs to be easily adjutable per map, maybe tie into the collison map?
                    //37/16 
                    if (player.position.X >= 2304 && player.position.X <= 2324 && player.position.Y >= 960 && player.position.Y <= 980)
                    {
                        winCondition = true;
                        _scene = Scene.END;
                    }
                    else if (_gameTimeElapsed.TotalSeconds <= 0)
                    {
                        winCondition = false;
                        _scene = Scene.END;
                    }

                    break;

                case Scene.PAUSE:
                    // Pause logic
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
                    else if (Keyboard.GetState().IsKeyDown(Keys.Space)) _scene = Scene.GAME;
                    _timerRunning = false;
                    break;

                case Scene.END:
                    // End game logic
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
                    _timerRunning = false;
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        RestartGame();
                    }
                    break;
            }

            
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {

            switch (_scene)
            {
                case Scene.MAIN:
                    // Main menu Draw

                    _spriteBatch.Begin();

                    GraphicsDevice.Clear(Color.DarkSeaGreen);
                    string title = "SHROOMY DUNGEON MAZE";
                    string prompt = "Press SPACE to Start, P to pause!";
                    string startInstructions = "Find the other green square before the timer runs out!";

                    Vector2 titleSize = menuFont.MeasureString(title);
                    Vector2 promptSize = menuFont.MeasureString(prompt);
                    Vector2 instructSize = menuFont.MeasureString(startInstructions);


                    _spriteBatch.DrawString(menuFont, title,
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - titleSize.X / 2, 100),
                        Color.Black);

                    _spriteBatch.DrawString(menuFont, prompt,
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - promptSize.X / 2, 300),
                        Color.Black);

                    _spriteBatch.DrawString(menuFont, startInstructions, new Vector2(_graphics.PreferredBackBufferWidth / 2 - instructSize.X / 2, 350),
                        Color.Black);

                    _spriteBatch.End();

                    break;

                case Scene.GAME:
                    // Game Draw

                    _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

                    GraphicsDevice.Clear(Color.Black);

                    //Draw background
                    foreach (var item in tileMap)
                    {
                        Rectangle dest = new Rectangle(
                            (int)item.Key.X * _enviroSize,
                            (int)item.Key.Y * _enviroSize,
                            _enviroSize,
                            _enviroSize);

                        Rectangle src = textureStore[item.Value];
                        _spriteBatch.Draw(tileAtlas, dest, src, Color.White);
                        //Debug.WriteLine(dest);
                    }

              
                    //Player Draw
                    _spriteBatch.Draw(
                        player.texture, 
                        player.position, 
                        new Rectangle(
                            player.textureFrame * player.textureSize,
                            player.textureFrameRow * player.textureSize, 
                            player.textureSize, 
                            player.textureSize), 
                        Color.White);

                    string timerText = FormatTime(_gameTimeElapsed);
                    Vector2 timerPosition = new Vector2(_camera.Position.X, _camera.Position.Y); // Top-left corner
                    _spriteBatch.DrawString(menuFont, timerText, timerPosition, Color.White);

                    _spriteBatch.End();

                    break;

                case Scene.PAUSE:
                    // Pause Draw
                    _spriteBatch.Begin();

                    GraphicsDevice.Clear(Color.DarkSeaGreen);
                    string pauseTitle = "Tea time!";
                    string pausePrompt = "Press SPACE to resume, ESC to quit!";
                    string pauseInstructions = "        WSAD to move\n\n El Wisman - w0497862";


                    Vector2 pTitleSize = menuFont.MeasureString(pauseTitle);
                    Vector2 pPromptSize = menuFont.MeasureString(pausePrompt);
                    Vector2 pInstructSize = menuFont.MeasureString(pauseInstructions);


                    _spriteBatch.DrawString(menuFont, pauseTitle,
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - pTitleSize.X / 2, 100),
                        Color.Black);

                    _spriteBatch.DrawString(menuFont, pausePrompt,
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - pPromptSize.X / 2, 300),
                        Color.Black);

                    _spriteBatch.DrawString(menuFont, pauseInstructions, new Vector2(_graphics.PreferredBackBufferWidth / 2 - pInstructSize.X / 2, 350),
                        Color.Black);

                    _spriteBatch.End();

                    break;

                case Scene.END:
                    // End game Draw
                    _spriteBatch.Begin();

                    if (winCondition)
                    {
                        GraphicsDevice.Clear(Color.DarkGreen);
                        
                        string endTitle = "Congratulations! You Win!";
                        string endPrompt = "Press ESC to quit! or SPACE to restart!";

                        Vector2 eTitleSize = menuFont.MeasureString(endTitle);
                        Vector2 ePromptSize = menuFont.MeasureString(endPrompt);


                        _spriteBatch.DrawString(menuFont, endTitle,
                            new Vector2(_graphics.PreferredBackBufferWidth / 2 - eTitleSize.X / 2, 100),
                            Color.White);

                        _spriteBatch.DrawString(menuFont, endPrompt,
                            new Vector2(_graphics.PreferredBackBufferWidth / 2 - ePromptSize.X / 2, 300),
                            Color.White);
                    }
                    else
                    {
                        GraphicsDevice.Clear(Color.DarkRed);
                        string endTitle = "Game Over! You Lose!";
                        string endPrompt = "Press ESC to quit!";

                        Vector2 eTitleSize = menuFont.MeasureString(endTitle);
                        Vector2 ePromptSize = menuFont.MeasureString(endPrompt);


                        _spriteBatch.DrawString(menuFont, endTitle,
                            new Vector2(_graphics.PreferredBackBufferWidth / 2 - eTitleSize.X / 2, 100),
                            Color.White);

                        _spriteBatch.DrawString(menuFont, endPrompt,
                            new Vector2(_graphics.PreferredBackBufferWidth / 2 - ePromptSize.X / 2, 300),
                            Color.White);
                    }

                    _spriteBatch.End();

                    break;
            }
           

            base.Draw(gameTime);
        }

        //map loading method
        private Dictionary<Vector2, int> LoadMap(string filepath) {
            Dictionary<Vector2, int> result = new();

            StreamReader reader = new (filepath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        result[new Vector2(x,y)] = value;
                    }
                }
                y++;
            }
            return result;
        }

        //DeepSeek provided collision method - works about the same as my original buuuuut, well, actualy works.
        public Rectangle GetPlayerBounds()
        {
            return new Rectangle(
                (int)player.position.X,
                (int)player.position.Y,
                _playerTextureSize,
                _playerTextureSize);
        }
        //public List<Rectangle> GetCollidableTiles(Rectangle bounds)
        //{
        //    List<Rectangle> collidableTiles = new List<Rectangle>();

        //    // Convert bounds to tile coordinates
        //    int leftTile = bounds.Left / _enviroSize;
        //    int rightTile = bounds.Right / _enviroSize;
        //    int topTile = bounds.Top / _enviroSize;
        //    int bottomTile = bounds.Bottom / _enviroSize;

        //    // Check all potentially colliding tiles
        //    for (int y = topTile; y <= bottomTile; y++)
        //    {
        //        for (int x = leftTile; x <= rightTile; x++)
        //        {
        //            if (tileMapCol.TryGetValue(new Vector2(x, y), out int tileValue) && tileValue >= 0)
        //            {
        //                collidableTiles.Add(new Rectangle(
        //                    x * _enviroSize,
        //                    y * _enviroSize,
        //                    _enviroSize,
        //                    _enviroSize));
        //            }
        //        }
        //    }
        //    return collidableTiles;
        //}
        public List<Rectangle> GetIntersectingTiles(Rectangle bounds)
        {
            List<Rectangle> intersectingTiles = new();

            // Convert player bounds to tile coordinates
            int leftTile = bounds.Left / _enviroSize;
            int rightTile = bounds.Right / _enviroSize;
            int topTile = bounds.Top / _enviroSize;
            int bottomTile = bounds.Bottom / _enviroSize;

            // Check all tiles the player overlaps with
            for (int y = topTile; y <= bottomTile; y++)
            {
                for (int x = leftTile; x <= rightTile; x++)
                {
                    // Only add tiles that are actually collidable
                    if (tileMapCol.TryGetValue(new Vector2(x, y), out int tileValue) && tileValue >= 0)
                    {
                        intersectingTiles.Add(new Rectangle(
                            x * _enviroSize,
                            y * _enviroSize,
                            _enviroSize,
                            _enviroSize));
                    }
                }
            }

            return intersectingTiles;
        }

        public bool IsColliding(Rectangle bounds)
        {
            List<Rectangle> collidingTiles = GetIntersectingTiles(bounds);
            return collidingTiles.Count > 0;
        }

        private string FormatTime(TimeSpan time)
        {
            return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}.{time.Milliseconds:000}";
        }

        private void RestartGame()
        {
            _gameTimeElapsed = TimeSpan.FromMinutes(2);
            _timerRunning = false;
            player.position = new Vector2(193, 193);
            winCondition = false;
            _scene = Scene.MAIN;
        }

    }
}
