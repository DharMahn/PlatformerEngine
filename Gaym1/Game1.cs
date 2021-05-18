using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Forms = System.Windows.Forms;
using System.IO;

namespace Gaym1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class FrameCounter
    {
        public FrameCounter()
        {
        }

        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        private Queue<float> _sampleBuffer = new Queue<float>();

        public bool Update(float deltaTime)
        {
            CurrentFramesPerSecond = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
            return true;
        }

    }
    public class Game1 : Game
    {
        private FrameCounter _frameCounter = new FrameCounter();
        SpriteFont SpriteFont;
        KeyboardState lastKeyState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D idle;
        Texture2D climb;

        MouseState previousMouseState;
        Vector2 start;
        bool IsDrawingPlatform;
        public static int ScreenHeight;
        public static int ScreenWidth;

        public List<Entity> platforms = new List<Entity>();

        static Texture2D pixel;

        List<Player> players = new List<Player>();

        static Camera _camera;
        Vector2 MouseWithCameraPos = new Vector2();
        Random r = new Random();

        NonSelectableButton loadbutton = new NonSelectableButton
        {
            Top = 10,
            Left = 230,
            Width = 100,
            Height = 40,
            Text = "Load map from TXT",
            Visible = false,
            Enabled = false,
        };

        NonSelectableButton connectbutton = new NonSelectableButton
        {
            Top = 10,
            Left = 10,
            Width = 100,
            Height = 40,
            Text = "Connect",
            Visible = false,
            Enabled = false,
        };

        NonSelectableButton savebutton = new NonSelectableButton
        {
            Top = 10,
            Left = 120,
            Width = 100,
            Height = 40,
            Text = "Save map to .txt",
            Visible = false,
            Enabled = false,

        };

        Forms.TextBox ipadd = new Forms.TextBox
        {
            Top = 70,
            Left = 10,
            Width = 200,
            Height = 20,
            Visible = false,
            Enabled = false,
        };
        Forms.Form form;
        private void SwapButtons()
        {
            foreach (Forms.Control item in form.Controls)
            {
                item.Visible = !item.Visible;
                item.Enabled = !item.Enabled;
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IntPtr hwnd = Window.Handle;
            //graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;
            Forms.Control ctrl = Forms.Control.FromHandle(hwnd);
            form = ctrl.FindForm();
            form.AcceptButton = null;
            form.CancelButton = null;
            savebutton.Click += new EventHandler(delegate
              {
                  SaveToTxt("map.txt");
              });
            form.KeyUp += OnKeyUp;
            loadbutton.Click += new EventHandler(delegate
              {
                  LoadFromTxt();
              });
            form.Controls.Add(connectbutton);
            form.Controls.Add(ipadd);
            form.Controls.Add(savebutton);
            form.Controls.Add(loadbutton);
        }
        private void OnKeyUp(object sender, Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Forms.Keys.Space || e.KeyCode == Forms.Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
        private void LoadFromTxt()
        {

            platforms = new List<Entity>();
            using (StreamReader sr = new StreamReader("map.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string asd = sr.ReadLine();
                    string[] anyad = asd.Split(';');
                    platforms.Add(new Entity
                    {
                        pos = new Vector2(float.Parse(anyad[0]), float.Parse(anyad[1])),
                        HitboxSize = new Vector2(float.Parse(anyad[2]), float.Parse(anyad[3])),
                        hp = 100,
                        color = Color.Brown,
                    });
                }
                sr.Close();
            }
            SwapButtons();
        }

        private void SaveToTxt(string v)
        {
            StreamWriter sw = new StreamWriter(v);
            foreach (var item in platforms)
            {
                sw.WriteLine(item.Left + ";" + item.Top + ";" + item.HitboxSize.X + ";" + item.HitboxSize.Y);
            }
            sw.Close();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "Anyád";
            // TODO: Add your initialization logic here
            //Window.AllowUserResizing = true;
            //graphics.PreferMultiSampling = true;
            //GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            //Window.ClientSizeChanged += new EventHandler<EventArgs>(delegate { RemakeBorders(); });
            
            IsMouseVisible = true;
            base.Initialize();
            previousMouseState = Mouse.GetState();
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            ScreenWidth = graphics.PreferredBackBufferWidth;
            ScreenHeight = graphics.PreferredBackBufferHeight;
            graphics.ApplyChanges();
            _camera = new Camera();
            RemakeBorders();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteFont = Content.Load<SpriteFont>("File");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            idle = Content.Load<Texture2D>("idle");
            climb = Content.Load<Texture2D>("climb");
            RemakeBorders();
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            Color[] c = new Color[1];
            c[0] = Color.White;
            pixel.SetData(c);
            players.Add(new Player(100, new Vector2(60, 60), Content, "smol", new Input() { Up = Keys.Space, Down = Keys.S, Left = Keys.A, Right = Keys.D, Shoot = Keys.K }, Color.Blue, new Vector2(75, 100), new Vector2(100, 100)));

            players.Add(new Player(100, new Vector2(60, 60), Content, "rekt", new Input() { Up = Keys.Up, Down = Keys.Down, Left = Keys.Left, Right = Keys.Right }, new Color(0, 255, 0)));
            //npcs.Add(new NPC(100, new Vector2(60, 60), Content, "rekt"));
            // TODO: use this.Content to load your game content here

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !lastKeyState.IsKeyDown(Keys.Escape))
            {
                SwapButtons();
            }
            if (IsActive)
            {
                MouseWithCameraPos = new Vector2(Mouse.GetState().Position.X - _camera.Transform.Translation.X, Mouse.GetState().Position.Y - _camera.Transform.Translation.Y);

                if (previousMouseState.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    start = new Vector2(MouseWithCameraPos.X, MouseWithCameraPos.Y);
                    IsDrawingPlatform = true;
                }
                else if (previousMouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    IsDrawingPlatform = false;

                    if (Math.Abs(start.X - MouseWithCameraPos.X) * Math.Abs(start.Y - MouseWithCameraPos.Y) > 1)
                    {
                        Entity temp = new Entity
                        {
                            pos = new Vector2(Math.Min(start.X, MouseWithCameraPos.X), Math.Min(start.Y, MouseWithCameraPos.Y)),
                            HitboxSize = new Vector2(Math.Abs(start.X - MouseWithCameraPos.X), Math.Abs(start.Y - MouseWithCameraPos.Y)),
                            color=Color.Brown,
                        };
                        if (temp.HitboxSize.X > 2 && temp.HitboxSize.Y > 2)
                        {
                            platforms.Add(temp);
                        }
                    }
                }
                if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
                {
                    foreach (var item in platforms.ToList())
                    {
                        if (MouseWithCameraPos.X > item.pos.X && MouseWithCameraPos.Y > item.pos.Y && MouseWithCameraPos.X < item.Right && MouseWithCameraPos.Y < item.Bottom)
                        {
                            platforms.Remove(item);
                        }
                    }
                }
                foreach (var item in platforms.ToList())
                {
                    
                    if (item.hp <= 0)
                    {
                        //platforms.Remove(item);
                        if (!item.exploded)
                        {
                            item.Explode();
                            item.exploded = true;
                        }
                        else
                        {
                            if (item.explosionParticles.Count == 0)
                            {
                                platforms.Remove(item);
                                Console.WriteLine(platforms.Count + " platforms remaining");
                            }
                        }
                    }
                }
                // TODO: Add your update logic here
                previousMouseState = Mouse.GetState();
                foreach (var item in players)
                {
                    item.Update(platforms, gameTime, MouseWithCameraPos);
                }
                _camera.Follow(players.First());
            }
            lastKeyState = Keyboard.GetState();
            base.Update(gameTime);
        }
        void RemakeBorders()
        {
            foreach (var item in players)
            {
                item.pos = new Vector2(60, 60);
            }
            platforms = new List<Entity>();
            platforms.Add(new Entity
            {
                pos = new Vector2(0, 0),
                HitboxSize = new Vector2(graphics.PreferredBackBufferWidth, 50),
                color=Color.Brown,
            });
            platforms.Add(new Entity
            {
                pos = new Vector2(0, 50),
                HitboxSize = new Vector2(50, graphics.PreferredBackBufferHeight - 100),
                color = Color.Brown,

            });
            platforms.Add(new Entity
            {
                pos = new Vector2(0, graphics.PreferredBackBufferHeight - 50),
                HitboxSize = new Vector2(graphics.PreferredBackBufferWidth, 50),
                color = Color.Brown,

            });
            platforms.Add(new Entity
            {
                pos = new Vector2(graphics.PreferredBackBufferWidth - 50, 50),
                HitboxSize = new Vector2(50, graphics.PreferredBackBufferHeight - 100),
                color = Color.Brown,

            });
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: _camera.Transform);
            
            if (IsDrawingPlatform)
            {
                DrawRectangle(new Rectangle(Math.Min((int)start.X, (int)MouseWithCameraPos.X),
                                            Math.Min((int)start.Y, (int)MouseWithCameraPos.Y),
                                            Math.Abs((int)start.X - (int)MouseWithCameraPos.X),
                                            Math.Abs((int)start.Y - (int)MouseWithCameraPos.Y)), 1, Color.Red);
            }

            foreach (var item in platforms)
            {
                item.Draw(spriteBatch, pixel);
                //spriteBatch.Draw(pixel, new Rectangle((int)item.pos.X, (int)item.pos.Y, (int)item.HitboxSize.X, (int)item.HitboxSize.Y), Color.Brown);
            }

            foreach (var item in players)
            {
                if (item.touchLeft && !item.touchTop)
                {
                    spriteBatch.Draw(climb, new Rectangle((int)item.pos.X, (int)item.pos.Y, (int)item.TextureSize.X, (int)item.TextureSize.Y), new Rectangle(0, 0, 320, 320), Color.White, 0f, new Vector2(80, 0), SpriteEffects.None, 0f);
                }
                else if (item.touchRight && !item.touchTop)
                {
                    spriteBatch.Draw(climb, new Rectangle((int)item.pos.X, (int)item.pos.Y, (int)item.TextureSize.X, (int)item.TextureSize.Y), new Rectangle(0, 0, 320, 320), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
                }
                else
                {
                    if (Math.Abs(item.velocity.X) <= 1 && !item.moving && item.velocity.Y >= 0)
                    {
                        if (!item.flipped)
                        {
                            spriteBatch.Draw(idle, new Rectangle((int)item.pos.X, (int)item.pos.Y, (int)item.TextureSize.X, (int)item.TextureSize.Y), new Rectangle(0, 0, 320, 320), Color.White, 0f, new Vector2(80, 0), SpriteEffects.FlipHorizontally, 0f);
                        }
                        else
                        {
                            spriteBatch.Draw(idle, new Rectangle((int)item.pos.X, (int)item.pos.Y, (int)item.TextureSize.X, (int)item.TextureSize.Y), new Rectangle(0, 0, 320, 320), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                        }
                    }
                    else
                    {
                        if (!item.flipped)
                        {
                            spriteBatch.Draw(item.texture, new Rectangle((int)item.pos.X, (int)item.pos.Y, (int)item.TextureSize.X, (int)item.TextureSize.Y), new Rectangle(item.frame * 320, 0, 320, 320), Color.White, 0f, new Vector2(80, 0), SpriteEffects.FlipHorizontally, 0f);
                        }
                        else
                        {
                            spriteBatch.Draw(item.texture, new Rectangle((int)item.pos.X, (int)item.pos.Y, (int)item.TextureSize.X, (int)item.TextureSize.Y), new Rectangle(item.frame * 320, 0, 320, 320), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                        }
                    }
                }

                if (item.hook.hooked || item.hook.inAir)
                {
                    Vector2 edge = item.hook.pos - new Vector2(item.pos.X + (item.HitboxSize.X / 2), item.pos.Y + (item.HitboxSize.Y / 2));
                    float angle = (float)Math.Atan2(edge.Y, edge.X);
                    spriteBatch.Draw(pixel, new Rectangle((int)item.pos.X + ((int)item.HitboxSize.X / 2), (int)item.pos.Y + ((int)item.HitboxSize.Y / 2), (int)edge.Length(), 3), null, item.color, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                //DrawRectangle(new Rectangle((int)item.pos.X, (int)item.pos.Y, (int)item.HitboxSize.X, (int)item.HitboxSize.Y), 3, Color.Red);
                foreach (var item2 in item.Projectiles.ToList())
                {
                    if (!item2.isExploded)
                    {
                        spriteBatch.Draw(pixel, new Rectangle((int)item2.pos.X, (int)item2.pos.Y, (int)item2.HitboxSize.X, (int)item2.HitboxSize.Y), Color.Aquamarine);
                        //Console.WriteLine(item2.isExploded.ToString() + "; " + item2.HitboxSize.X + "-" + item2.HitboxSize.Y + "-" + item2.pos.X + "-" + item2.pos.Y);
                    }
                    else
                    {
                        item2.explosion.Draw(spriteBatch, pixel);
                        //Console.WriteLine(item.Projectiles.Count + item2.isExploded.ToString());
                    }
                }
            }

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _frameCounter.Update(deltaTime);

            var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

            spriteBatch.DrawString(SpriteFont, fps, new Vector2(-_camera.Transform.Translation.X, -_camera.Transform.Translation.Y), Color.Black);

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void DrawRectangle(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }
    }
}
