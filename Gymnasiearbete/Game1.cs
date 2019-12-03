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
        
        // Martin
        SpriteFont spriteFont;
        CellManager CM;
        public static string debugMessage = "";

        // Jesper
        SpriteBatch spriteBatch;

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
            Window.AllowUserResizing = true;
            if (!graphics.IsFullScreen)
            {
                //graphics.ToggleFullScreen();
            }

            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);

            CM = new CellManager(StaticGlobal.Random);

            SGBasicEffect.Initialize(GraphicsDevice);
            SGScreen.Initialize(Window.ClientBounds);
            Render.Initialize();
            Camera.Initialize();

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
            // grid.LoadContent(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            StaticGlobal.Keyboard.Begin();
            StaticGlobal.Mouse.Begin();

            // Allows the game to exit
            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.Escape))
            {
                this.Exit();
            }

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.R) || CM.simulationEnd)
            {
                //Initialize();
                CM = new CellManager(StaticGlobal.Random);
            }

            int h = (int)(Math.Floor((CM.civilazationTime) / 60) / 60);
            int m = (int)(Math.Floor(CM.civilazationTime) / 60) - (60 * h);
            int s = (int)Math.Floor(CM.civilazationTime) - (60 * m);
            debugMessage = "";
            if (h > 0){debugMessage += " h:" + h;}
            if (m > 0){debugMessage += " m:" + m;}
            debugMessage += " s:" + s;

            if (StaticGlobal.Mouse.IsButtonClicked(CustomMouseButtons.MOUSE_1) || StaticGlobal.Keyboard.IsKeyClicked(Keys.Space))
            {
                CM.pause = true;
                int debugType = 0;
                if (Keyboard.GetState().IsKeyDown(Keys.D1))
                {
                    debugType = 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D2))
                {
                    debugType = 2;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D3))
                {
                    debugType = 3;
                }
                debugMessage = CM.DebugSector(
                    (Camera.Position.X - SGScreen.Area.Width / 2 + Mouse.GetState().X) * Camera.Zoom, 
                    (Camera.Position.Y - SGScreen.Area.Height / 2 + Mouse.GetState().Y) * Camera.Zoom, 
                    debugType);
            }

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.Q))
            {
                CM.pause = false;
            }
            CM.Update(StaticGlobal.Random, gameTime);

            // Jesper
            if (StaticGlobal.Keyboard.IsKeyHeld(Keys.Up))
                Camera.Zoom += 0.005f;
            if (StaticGlobal.Keyboard.IsKeyHeld(Keys.Down))
                Camera.Zoom -= 0.005f;
            if (StaticGlobal.Mouse.ScrollWheelDifference != 0)
            {
                Camera.Zoom += StaticGlobal.Mouse.ScrollWheelDifference * 0.001f;
            }
            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.F))
                Camera.ToggleFreeCam();
            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.W) ||
                StaticGlobal.Keyboard.IsKeyClicked(Keys.A) ||
                StaticGlobal.Keyboard.IsKeyClicked(Keys.S) ||
                StaticGlobal.Keyboard.IsKeyClicked(Keys.D))
                Camera.FreeCam = true;
            if (Camera.FreeCam)
            {
                if (StaticGlobal.Keyboard.IsKeyHeld(Keys.W))
                    Camera.Position += new Vector2(0, -5 / Camera.Zoom);
                if (StaticGlobal.Keyboard.IsKeyHeld(Keys.S))
                    Camera.Position += new Vector2(0, 5 / Camera.Zoom);
                if (StaticGlobal.Keyboard.IsKeyHeld(Keys.A))
                    Camera.Position += new Vector2(-5 / Camera.Zoom, 0);
                if (StaticGlobal.Keyboard.IsKeyHeld(Keys.D))
                    Camera.Position += new Vector2(5 / Camera.Zoom, 0);
            }

            StaticGlobal.Keyboard.End();
            StaticGlobal.Mouse.End();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, debugMessage, new Vector2(4, 10), Color.Gold);
            spriteBatch.DrawString(spriteFont, Camera.Zoom.ToString(), new Vector2(10, 40), Color.Gold);
            if (CM.Content.Cells.Count > 0)
            {
                spriteBatch.DrawString(spriteFont, CM.Content.Cells[0].Energy.ToString(), new Vector2(10, 80), Color.LawnGreen);
                spriteBatch.DrawString(spriteFont, CM.Content.Cells[0].AI.lastMemory.ToString(), new Vector2(10, 80 +16), Color.LawnGreen);
            }

            spriteBatch.End();
            base.Draw(gameTime);

            SGBasicEffect.ApplyCurrentTechnique();

            if (CM.Content.Cells.Count > 0 && !Camera.FreeCam)
            {
                Camera.Position = Vector2.Lerp(Camera.Position, CM.Content.Cells[0].Position, 0.125f);
                //Vector2.Lerp(
                //    Camera.Position,
                //    CM.Content.Cells[0].Position,
                //    0.125f
                //);
                //Camera.Position -= new Vector2(
                //    SGScreen.Area.Width / (2 * Camera.Zoom),
                //    SGScreen.Area.Height / (2 * Camera.Zoom)
                //);
            }

            Render.Draw(CM, GraphicsDevice);
        }

        /// <summary>
        /// Thingy thingy that happens when a user resizes the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnResize(Object sender, EventArgs e)
        {
            Console.WriteLine(sender.ToString());
            SGBasicEffect.Resize(GraphicsDevice);
        }
    }
}