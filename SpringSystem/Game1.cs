using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpringSystem
{

    public static class SpriteBatchExtensions
    {
        private static Texture2D _texture;
        public static Texture2D GetTexture(this SpriteBatch spriteBatch)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _texture.SetData(new[] { Color.White });
            }

            return _texture;
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(spriteBatch.GetTexture(), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Particule[] _particules;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InitModel2();

            //var spring0 = new Spring(50, 70);
            //_particules = new[]
            //{
            //    new Particule(true, 0, new Vector(320, 0, 50)),
            //    new Particule(false, 100, new Vector(320, 0, 150))
            //};

            //_particules[0].Link(_particules[1], spring0);

            base.Initialize();
        }

        private void InitModel1()
        {
            var spring0 = new Spring(6.0, 10.0);
            var spring1 = new Spring(8.0, 20.0);
            var spring2 = new Spring(6.0, 15);
            var spring3 = new Spring(10.0, 25);
            var spring4 = new Spring(15.0, 35);
            _particules = new[]
                              {
                                  new Particule(true, 15.0, new Vector(320, 0, 50)),
                                  new Particule(false, 20.0, new Vector(380, 0, 50)),
                                  new Particule(true, 18.0, new Vector(250, 0, 50)),
                                  new Particule(false, 400, new Vector(200, 0, 150)),
                                  new Particule(true, 400, new Vector(100, 0, 200)),
                                  new Particule(false, 400, new Vector(300, 0, 350))
                              };

            _particules[0].Link(_particules[1], spring0).Link(_particules[3], spring1);
            _particules[1].Link(_particules[2], spring1).Link(_particules[3], spring2);
            _particules[3].Link(_particules[4], spring3).Link(_particules[5], spring0);
            _particules[5].Link(_particules[2], spring4);
        }

        private void InitModel2()
        {
            var spring0 = new Spring(20, 150);

            _particules = new Particule[10*10];
            var rnd = new Random();
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    var fix = (i == 0) || (j == 0) || (j < 3 && i == 9);
                    var weight = 40 + rnd.Next(100);
                    _particules[i + j*10] = new Particule(fix, weight, new Vector(100 + i*70, 0, -10 + (i+j) * 30));
                }
            }

            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    var spring = spring0;
                    if(i < 9)
                        _particules[i + j * 10].Link(_particules[i + 1 + j * 10], spring);
                    if(j < 9)
                        _particules[i + j * 10].Link(_particules[i + j * 10 + 10], spring);
                }
            }
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                InitModel2();
            }

            foreach (var mass in _particules)
            {
                mass.Apply();
            }

            var dt = gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var mass in _particules)
            {
                mass.Move(dt);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            var texture = spriteBatch.GetTexture();
            foreach (var mass in _particules)
            {
                var src = new Vector2((int)mass.Position.X, (int)mass.Position.Z);
                foreach (var target in mass.Links)
                {
                    var dst = new Vector2((int) target.Position.X, (int) target.Position.Z);
                    spriteBatch.DrawLine(src, dst, Color.Gray);
                }
            }

            foreach (var mass in _particules)
            {
                var s = 5;
                var rect = new Rectangle((int)(mass.Position.X + s), (int)(mass.Position.Z + s), -s * 2, -s * 2);
                spriteBatch.Draw(texture, rect, Color.White);
            }


            spriteBatch.End();
        }
    }
}
