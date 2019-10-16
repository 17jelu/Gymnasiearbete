using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Gymnasiearbete
{
    /// <summary>
    /// /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BasicEffect effect;

        Circle circle;
        Circle cirkel;
        
        SpriteFont spriteFont;

        Grid grid;

        CellManager CM;
        string debugMessage = "";

        Random random;

        bool restart = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // TODO: Add your initialization logic here
            IsMouseVisible = true;

            // TODO: Add your initialization logic here

            Window.AllowUserResizing = true;

            // Load BasicEffect
            effect = new BasicEffect(GraphicsDevice);
            effect.VertexColorEnabled = true;
            effect.Projection = Matrix.CreateOrthographicOffCenter
                (0, GraphicsDevice.Viewport.Width,     // left, right
                GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);
            effect.CurrentTechnique.Passes[0].Apply();

            grid = new Grid(effect, GraphicsDevice, Window.ClientBounds);

            circle = new Circle(Circle.UnitCircle.Point16, GraphicsDevice, Color.Red, 50, new Vector2(200, 150));
            cirkel = new Circle(Circle.UnitCircle.Point16, GraphicsDevice, Color.Purple, 100, new Vector2(250, 250));
            random = new Random();

            CM = new CellManager();
            CM.AddObjects
                (
                    new GameObject[2]
                    {
                        new Cell(CM, new Vector2(random.Next(0, Window.ClientBounds.Width), random.Next(0, Window.ClientBounds.Height)), 10, 1, 10),
                        new Cell(CM, new Vector2(random.Next(0, Window.ClientBounds.Width), random.Next(0, Window.ClientBounds.Height)), 10, 2, 10)
                    }
                );

            for (int i = 0; i < 10; i++)
            {
                CM.AddObjects
                    (
                        new GameObject[1]
                        {
                            new Food(new Vector2(random.Next(0, Window.ClientBounds.Width), random.Next(0, Window.ClientBounds.Height)))
                        }
                    );
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("FontDefault");

            // TODO: use this.Content to load your game content here
            grid.LoadContent(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        float tick = 0;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R) && restart == false)
            {
                Initialize();
                restart = true;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.R) && restart == true)
            {
                restart = false;
            }

            camera.X = (float)Math.Sin(tick) * 100;
            camera.Y = (float)Math.Cos(tick) * 100;

            int h = (int)(Math.Floor((CM.civilazationTime) / 60) / 60);
            int m = (int)(Math.Floor(CM.civilazationTime) / 60) - (60 * h);
            int s = (int)Math.Floor(CM.civilazationTime) - (60 * m);
            debugMessage = "";
            if (h > 0){debugMessage += " h:" + h;}
            if (m > 0){debugMessage += " m:" + m;}
            debugMessage += " s:" + s;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                CM.pause = true;
                debugMessage = CM.DebugSector(Mouse.GetState().X, Mouse.GetState().Y);
            } else
            {
                CM.pause = false;
            }
            CM.Update(Window, random, gameTime);

            base.Update(gameTime);
        }

        #region TEMPORARY - USED FOR DEBUGGING
        // Comment created 2019-10-01 14:33
        Camera camera = new Camera(Vector2.Zero);
        #endregion

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            //spriteBatch.Begin();

            

            //spriteBatch.End();
            base.Draw(gameTime);

            effect.CurrentTechnique.Passes[0].Apply();

            grid.Draw(GraphicsDevice, camera);
            
            circle.Render(GraphicsDevice, camera);
            cirkel.Render(GraphicsDevice, camera);

            CM.Draw(GraphicsDevice, camera);

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, debugMessage, new Vector2(4, 10), Color.Gold);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}