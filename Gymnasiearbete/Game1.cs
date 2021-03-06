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

        const string LORE =
@"                                             As the story goes:
                      War. War, always war. The year is 2151. 
                            The nukes erased life as we know it. 
      Left are only somewhat mindless blobs of radiated mass,
ruled by their primal instinct of; fight or flight, eat or get eaten.
                                      This is survival. This is war.";

        // Martin
        SpriteFont spriteFont;
        public static string debugMessage = "";

        // Jesper
        SpriteBatch spriteBatch;

        GraphicRectangle debugRectangle;
        GraphicRectangle shadowRectangle;

        CustomDrawObject custom;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            
        }

        GraphicRectangle rect;
        float rectWidth;
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
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                //graphics.ToggleFullScreen();
            }

            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);


            // SGBasicEffect.Initialize(GraphicsDevice);
            StaticGlobal.BasicEffect.Initialize(GraphicsDevice);
            StaticGlobal.Screen.Initialize(Window.ClientBounds);
            Render.Initialize();
            Camera.Initialize();

            UIElementHandler.Initialize();

            debugRectangle = new GraphicRectangle(Color.White, 0, 0, 490, 148);

            shadowRectangle = new GraphicRectangle(
                new Color(0, 0, 0, 0.85f),
                0, 0,
                Window.ClientBounds.Width,
                Window.ClientBounds.Height
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

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.R) || StaticGlobal.CM.SimulationEnd)
            {
                StaticGlobal.CM.Initilize();
                Camera.ChangeSpectatingCell(0);
            }
            
            StaticGlobal.CM.Update(gameTime);

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.Space))
            { StaticGlobal.CM.TogglePause(); }

            if (StaticGlobal.Keyboard.IsKeyHeld(Keys.Up))
            { Camera.Zoom += 0.005f; }

            if (StaticGlobal.Keyboard.IsKeyHeld(Keys.Down))
            { Camera.Zoom -= 0.005f; }

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.Left))
            { Camera.ChangeSpectatingCell(-1); }

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.Right))
            { Camera.ChangeSpectatingCell(1); }

            if (StaticGlobal.Mouse.ScrollWheelDifference != 0)
            {
                Console.WriteLine("{0} -> {1}",
                    StaticGlobal.Mouse.ScrollWheelDifference,
                    StaticGlobal.Mouse.ScrollWheelDifference * 0.001f);
                Camera.Zoom -= StaticGlobal.Mouse.ScrollWheelDifference * 0.001f;
            }

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.F))
            { Camera.ToggleFreeCam(); }
            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.W) ||
                StaticGlobal.Keyboard.IsKeyClicked(Keys.A) ||
                StaticGlobal.Keyboard.IsKeyClicked(Keys.S) ||
                StaticGlobal.Keyboard.IsKeyClicked(Keys.D))
            { Camera.FreeCam = true; }

            if (Camera.SpectatingCell.AI.GetAIType == AI.AIType.Player)
                Camera.FreeCam = false;

            if (Camera.FreeCam)
            {
                if (StaticGlobal.Keyboard.IsKeyHeld(Keys.W))
                { Camera.Position += new Vector2(0, -5 / Camera.Zoom); }
                if (StaticGlobal.Keyboard.IsKeyHeld(Keys.S))
                { Camera.Position += new Vector2(0, 5 / Camera.Zoom); }
                if (StaticGlobal.Keyboard.IsKeyHeld(Keys.A))
                { Camera.Position += new Vector2(-5 / Camera.Zoom, 0); }
                if (StaticGlobal.Keyboard.IsKeyHeld(Keys.D))
                { Camera.Position += new Vector2(5 / Camera.Zoom, 0); }
            }

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.F11))
            {
                StaticGlobal.Screen.ToggleFullScreen(graphics, GraphicsDevice);
            }

            if (Camera.SpectatingCell.isMarkedForDelete)
            {
                Camera.ChangeSpectatingCell(0);
            }

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.Delete))
                Camera.SpectatingCell.isMarkedForDelete = true;

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.LeftControl))
            {
                //Camera.SpectatingCell.AI = AI.GetAI(AI.AIType.Player);
                Camera.SpectatingCell.AI
                    = new Player();
            }

            UIElementHandler.Update();


            StaticGlobal.Keyboard.End();
            StaticGlobal.Mouse.End();

            // Check for shutdown
            if (StaticGlobal.ExitInProgress)
                Shutdown();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Needed before Render.Draw();
            StaticGlobal.BasicEffect.ApplyCurrentTechnique();

            // clear screen
            GraphicsDevice.Clear(Color.WhiteSmoke);

            if (Camera.SpectatingCell != null && !Camera.FreeCam)
            {
                Camera.Position = Vector2.Lerp(Camera.Position, Camera.SpectatingCell.Position, 0.125f);
            }

            RasterizerState rasterizerState;
            rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.Solid;
            GraphicsDevice.RasterizerState = rasterizerState;

            Render.Draw(StaticGlobal.CM, GraphicsDevice);

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, debugMessage, new Vector2(10, 35), Color.Black);

            if (Camera.SpectatingCell != null && !Camera.FreeCam)
            {
                spriteBatch.DrawString(spriteFont, "About current Cell:", new Vector2(10, 80 - 20 * 1), Color.Black);
                spriteBatch.DrawString(spriteFont, "Energy: " + Math.Floor(Camera.SpectatingCell.Energy).ToString(), new Vector2(10, 80 + 20 * 0), Color.Black);
                spriteBatch.DrawString(spriteFont, "Family: " + Camera.SpectatingCell.AI.family.ToString(), new Vector2(10, 80 + 20 * 1), Color.Black);
                spriteBatch.DrawString(spriteFont, "FamilyCount: " + StaticGlobal.Family.FamilyCount(Camera.SpectatingCell.AI.family).ToString(), new Vector2(10, 80 + 20 * 2), Color.Black);
                spriteBatch.DrawString(spriteFont, "Memory: " + Camera.SpectatingCell.AI.lastMemory.ToString(), new Vector2(10, 80 + 20 * 3), Color.Black);
                spriteBatch.DrawString(spriteFont, "Points: " + Camera.SpectatingCell.AI.lastMemory.Points.ToString(), new Vector2(10, 80 + 20 * 4), Color.Black);
            }

            if (StaticGlobal.CM.Pause)
            {
                shadowRectangle.Width = Window.ClientBounds.Width;
                shadowRectangle.Height = Window.ClientBounds.Height;
                shadowRectangle.Render(GraphicsDevice);

                spriteBatch.DrawString(spriteFont, LORE, new Vector2(StaticGlobal.Screen.Area.Center.X - 239, StaticGlobal.Screen.Area.Center.Y - 74), Color.Black);

                debugRectangle.X = StaticGlobal.Screen.Area.Center.X - 239 - 6;
                debugRectangle.Y = StaticGlobal.Screen.Area.Center.Y - 74 - 6;

                debugRectangle.Render(GraphicsDevice);
            }

            if (StaticGlobal.CM.Pause)
            {
                spriteBatch.DrawString(spriteFont, 
                    "Press [Spacebar] to resume the simulation",
                    new Vector2(10, 10 + 25 * 0), Color.White);
                spriteBatch.DrawString(spriteFont,
                    "Use WASD to enter free-cam mode and move the Camera",
                    new Vector2(10, 10 + 25 * 1), Color.White);
                spriteBatch.DrawString(spriteFont,
                    "Press [F] to toggle between free-cam mode",
                    new Vector2(10, 10 + 25 * 2), Color.White);
                spriteBatch.DrawString(spriteFont,
                    "Press [Left Arrow] and [Right Arrow] to switch between which cell you're spectating",
                    new Vector2(10, 10 + 25 * 3), Color.White);
                spriteBatch.DrawString(spriteFont,
                    "Press [R] to restart the simulation",
                    new Vector2(10, 10 + 25 * 4), Color.White);
                spriteBatch.DrawString(spriteFont,
                    "Press [ESC] to quit the application",
                    new Vector2(10, Window.ClientBounds.Height - 10 - 25 * 1), Color.White);
                spriteBatch.DrawString(spriteFont,
                    "Press [Up Arrow] and [Down Arrow] or use [Scrollwheel] to zoom in and out in the simulation",
                    new Vector2(10, Window.ClientBounds.Height - 10 - 25 * 2), Color.White);
                spriteBatch.DrawString(spriteFont,
                    "Press [F11] to toggle between fullscreen mode and window mode",
                    new Vector2(10, Window.ClientBounds.Height - 10 - 25 * 3), Color.White);
            }
            else
            {
                spriteBatch.DrawString(spriteFont, "Press [Spacebar] to pause the simulation, see controls, and read the lore", new Vector2(10, 10), Color.Black);
            }

            UIElementHandler.Render(GraphicsDevice);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Thingy thingy that happens when a user resizes the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnResize(Object sender, EventArgs e)
        {
            Console.WriteLine(sender.ToString());
            StaticGlobal.BasicEffect.Resize(GraphicsDevice);
            StaticGlobal.Screen.Resize(Window.ClientBounds);
        }

        /// <summary>
        /// Exits the game the correct way
        /// </summary>
        private void Shutdown()
        {
            Console.WriteLine("SHUTDOWN INITIATED");

            // Implement Save function if save & exit???
            if (StaticGlobal.SaveModeON)
            {

            }

            Exit();
        }
    }
}