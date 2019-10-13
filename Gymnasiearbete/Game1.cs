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

        Grid grid;
        Circle circle;

        CellManager CM;
        Cell debugCell;

        Random random;

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

            random = new Random();
            
            grid = new Grid(Window.ClientBounds);

            Window.AllowUserResizing = true;

            circle = new Circle(Circle.UnitCircle.Point16, GraphicsDevice, Color.Red, 50, new Vector2(200, 150));

            debugCell = new Cell(CM, new Vector2(random.Next(0, Window.ClientBounds.Width), random.Next(0, Window.ClientBounds.Height)), 20, 1, 50);
            CM = new CellManager();
            CM.AddObjects
                (
                
                    new GameObject[6]
                    {
                        debugCell,
                        new Cell(CM, new Vector2(random.Next(0, Window.ClientBounds.Width), random.Next(0, Window.ClientBounds.Height)), 40, 0.5f, 10),
                        new Food(new Vector2(random.Next(0, Window.ClientBounds.Width), random.Next(0, Window.ClientBounds.Height))),
                        new Food(new Vector2(random.Next(0, Window.ClientBounds.Width), random.Next(0, Window.ClientBounds.Height))),
                        new Food(new Vector2(random.Next(0, Window.ClientBounds.Width), random.Next(0, Window.ClientBounds.Height))),
                        new Food(new Vector2(random.Next(0, Window.ClientBounds.Width), random.Next(0, Window.ClientBounds.Height)))
                    }
                );

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            /*
            tick = tick >= 360 ? tick - 360 : tick + 0.005f;
            camera.X = (float)Math.Sin(tick) * 500;
            camera.Y = (float)Math.Cos(tick) * 500;
            */

            CM.Update(Window, random);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                CM.DebugSector(Mouse.GetState().X, Mouse.GetState().Y);
            }

            //circle.Update();

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
            spriteBatch.Begin();
            grid.Draw(GraphicsDevice, spriteBatch, camera);


            CM.Draw(GraphicsDevice, camera);

            spriteBatch.End();
            base.Draw(gameTime);
            
            //circle.Render(GraphicsDevice);
        }
    }
}