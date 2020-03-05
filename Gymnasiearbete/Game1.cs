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
        Menu menu;
        Texture2D infoText;
        Rectangle infoRectangle;

        SpriteBatch spriteBatch;

        GraphicRectangle energyBar;
        GraphicRectangle shadowPauseOverlay;

        CustomDrawObject custom;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            
        }

        float rectWidth;
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            menu = new Menu(Menu.State.MainMenu);

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

            #region menu.ButtonGroups
            // Declare menu.ButtonGroup_MainMenu
            #region ButtonGroup_MainMenu
            menu.ButtonGroup_MainMenu = new UIElementButtonGroup
            {
                Buttons = new Button[]
                {
                    // NULL Button
                    new Button { Bounds = new Rectangle(-50, -50, 0, 0) },
                    // Enter Simulation Button
                    new Button
                    {
                        Text = "Enter Simulation",
                        Bounds = new Rectangle(0, 0, 250, 50),
                        Action =()=> menu.ChangeState(Menu.State.Simulation)
                    },
                    // Read Info Screen
                    new Button
                    {
                        Text = "Info",
                        Bounds = new Rectangle(0, 0, 200, 50),
                        Action =()=> {
                            // reset size
                            infoRectangle.Width = infoText.Width;
                            infoRectangle.Height = infoText.Height;
                            // Center X
                            infoRectangle.X = (StaticGlobal.Screen.Area.Width >> 1) - (infoRectangle.Width >> 1);
                            // Set Y
                            infoRectangle.Y = 70;
                            // Change to InfoScreen
                            menu.ChangeState(Menu.State.InfoScreen);
                        }
                    },
                    // Exit Application
                    new Button
                    {
                        Text = "Exit",
                        Bounds = new Rectangle(0, 0, 150, 50),
                        Action =()=> StaticGlobal.Shutdown()
                    },
                }
            };
            #endregion ButtonGroup_MainMenu
            // Declare menu.ButtonGroup_InfoScreen
            #region ButtonGroup_InfoScreen
            menu.ButtonGroup_InfoScreen = new UIElementButtonGroup
            {
                Buttons = new Button[]
                {
                    // NULL Button
                    new Button { Bounds = new Rectangle(-50, -50, 0, 0) },
                    // Return to main menu
                    new Button
                    {
                        Text = "Back",
                        Bounds = new Rectangle(15, 15, 100, 50),
                        Action =()=> menu.ChangeState(Menu.State.MainMenu)
                    }
                }
            };
            #endregion ButtonGroup_InfoScreen
            // Declare menu.ButtonGroup_PausedSimulation
            #region ButtonGroup_PausedSimulation

            menu.ButtonGroup_PausedSimulation = new UIElementButtonGroup
            {
                Buttons = new Button[]
                {
                    // NULL Button
                    new Button { Bounds = new Rectangle(-50, -50, 0, 0) },
                    // Unpause Simulation
                    new Button
                    {
                        Text = "Resume",
                        Bounds = new Rectangle(0, 0, 200, 50),
                        Action =()=> menu.ChangeState(Menu.State.Simulation)
                    },
                    // Kill / restart simulation # Same thing as pressing R button
                    new Button
                    {
                        Text = "Restart simulation",
                        Bounds = new Rectangle(0, 0, 300, 50),
                        Action =()=>
                        {
                            StaticGlobal.CM.Initilize(10, AI.AIType.TargetingPoints);
                            Camera.ChangeSpectatingCell(0);
                            menu.ChangeState(Menu.State.Simulation);
                        }
                    },
                    // Mainmenu Button
                    new Button
                    {
                        Text = "Return to Main Menu",
                        Bounds = new Rectangle(0, 0, 300, 50),
                        Action =()=> menu.ChangeState(Menu.State.MainMenu)
                    },
                    // Exit Application
                    new Button
                    {
                        Text = "Exit",
                        Bounds = new Rectangle(0, 0, 150, 50),
                        Action =()=>
                        {
                            StaticGlobal.Shutdown();
                        }
                    },
                }
            };
            #endregion ButtonGroup_PausedSimulation
            // Initialize all Menu ButtonGroups
            #region .Initialize();
            menu.ButtonGroup_MainMenu.Initialize();
            menu.ButtonGroup_InfoScreen.Initialize();
            menu.ButtonGroup_PausedSimulation.Initialize();
            #endregion .Initialize();
            // Add custom behaviour on resize
            #region .OnRize =()=> ...
            menu.ButtonGroup_MainMenu.OnResize =()=>
            {
                // Centering all buttons (except NULL button)
                for (int i = 1; i < menu.ButtonGroup_MainMenu.Buttons.Length; i++)
                {
                    Rectangle btn = menu.ButtonGroup_MainMenu.Buttons[i].Bounds;
                    btn = new Rectangle(
                        (StaticGlobal.Screen.Area.Width >> 1) - (btn.Width >> 1),
                        (StaticGlobal.Screen.Area.Height >> 1) - (btn.Height >> 1)
                            + (i * (btn.Height + (UIElementButtonGroup.OutlineMargin * 4)))
                            + (UIElementButtonGroup.OutlineMargin << 1)
                            - (btn.Height * (menu.ButtonGroup_MainMenu.Buttons.Length + 2) >> 1),
                        btn.Width, btn.Height);
                    menu.ButtonGroup_MainMenu.Buttons[i].Bounds = btn;
                }
            };
            menu.ButtonGroup_PausedSimulation.OnResize = () =>
            {
                // Centering all buttons (except NULL button)
                for (int i = 1; i < menu.ButtonGroup_PausedSimulation.Buttons.Length; i++)
                {
                    Rectangle btn = menu.ButtonGroup_PausedSimulation.Buttons[i].Bounds;
                    btn = new Rectangle(
                        (StaticGlobal.Screen.Area.Width >> 1) - (btn.Width >> 1),
                        (StaticGlobal.Screen.Area.Height >> 1) - (btn.Height >> 1)
                            + (i * (btn.Height + (UIElementButtonGroup.OutlineMargin * 4)))
                            + (UIElementButtonGroup.OutlineMargin << 1)
                            - (btn.Height * (menu.ButtonGroup_PausedSimulation.Buttons.Length + 2) >> 1),
                        btn.Width, btn.Height);
                    menu.ButtonGroup_PausedSimulation.Buttons[i].Bounds = btn;
                }
            };
            #endregion .OnResize =()> ...
            #endregion menu.ButtonGroups

            energyBar = new GraphicRectangle(Color.White, 0, 0, 48, 148);

            shadowPauseOverlay = new GraphicRectangle(
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
            spriteFont = Content.Load<SpriteFont>("OpenDyslexic");

            // TODO: use this.Content to load your game content here
            // grid.LoadContent(GraphicsDevice);
            infoText = Content.Load<Texture2D>("info");
            infoRectangle.Width = infoText.Width;
            infoRectangle.Height = infoText.Height;
            infoRectangle.Location = Point.Zero;
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
            menu.FirstUpdate();

            StaticGlobal.Keyboard.Begin();
            StaticGlobal.Mouse.Begin();

            /* START OF switch(meny.CurrentState) */
            switch(menu.CurrentState)
            {
                case Menu.State.MainMenu:
                    /* START OF case Menu.State.MainMenu */
                    menu.ButtonGroup_MainMenu.Update();
                    /* END OF case Menu.State.MainMenu */
                    break;
                case Menu.State.InfoScreen:
                    /* START OF case Menu.State.InfoScreen */

                    // Zooming Scroll
                    if ((StaticGlobal.Keyboard.IsKeyHeld(Keys.LeftControl)
                        || StaticGlobal.Keyboard.IsKeyHeld(Keys.RightControl))
                        && StaticGlobal.Mouse.ScrollWheelDifference != 0)
                    {
                        const float factor = 0.2f;
                        // Bitshifting 1 to right and 1 back to left to remove trailing 1's
                        // 00110101 
                        // 00011010
                        // 00110100
                        infoRectangle.Width += ((int)(StaticGlobal.Mouse.ScrollWheelDifference * factor));
                        infoRectangle.Height += ((int)(StaticGlobal.Mouse.ScrollWheelDifference * factor));
                        // To be able to divide evenly by 2

                        // Cap Zoom
                        if (infoRectangle.Width < (infoText.Width >> 1))
                        {
                            infoRectangle.Width = (infoText.Width >> 1);
                            infoRectangle.Height = (infoText.Height >> 1);
                            break;
                        }

                        float dif = (StaticGlobal.Mouse.Location.X - infoRectangle.X);
                        var ratio = (float)dif / (float)infoRectangle.Width;
                        Console.WriteLine("Dif {0}, Width {1}, 0/1 = {2}", dif, infoRectangle.Width, ratio);
                        
                        //infoRectangle.X -= (int)(StaticGlobal.Mouse.ScrollWheelDifference * ratio);
                        infoRectangle.X -= (int)(factor * ((StaticGlobal.Mouse.Location.X - infoRectangle.X) / (float)infoRectangle.Width) * StaticGlobal.Mouse.ScrollWheelDifference);
                        infoRectangle.Y -= (int)(factor * ((StaticGlobal.Mouse.Location.Y - infoRectangle.Y) / (float)infoRectangle.Height) * StaticGlobal.Mouse.ScrollWheelDifference);
                    }
                    // Horizontal Scroll
                    else if (StaticGlobal.Keyboard.IsKeyHeld(Keys.LeftShift)
                        || StaticGlobal.Keyboard.IsKeyHeld(Keys.RightShift))
                    {
                        infoRectangle.X += StaticGlobal.Mouse.ScrollWheelDifference;
                    }
                    // Vertical Scroll
                    else
                    {
                        infoRectangle.Y += StaticGlobal.Mouse.ScrollWheelDifference;
                    }

                    // Don't let infoText escape the Screen more than it being hidden
                    if (infoRectangle.Right < StaticGlobal.Screen.Area.Left) // Left Side
                        infoRectangle.X = StaticGlobal.Screen.Area.Left - infoRectangle.Width;
                    if (infoRectangle.Left > StaticGlobal.Screen.Area.Right) // Right Side
                        infoRectangle.X = StaticGlobal.Screen.Area.Right;
                    if (infoRectangle.Bottom < StaticGlobal.Screen.Area.Top) // Top Side
                        infoRectangle.Y = StaticGlobal.Screen.Area.Top - infoRectangle.Height;
                    if (infoRectangle.Top > StaticGlobal.Screen.Area.Bottom) // Bottom Side
                        infoRectangle.Y = StaticGlobal.Screen.Area.Bottom;

                    menu.ButtonGroup_InfoScreen.Update();
                    /* END OF case Menu.State.InfoScreen */
                        break;
                case Menu.State.Simulation:
                    /* START OF case Menu.State.Simulation */
                    if (StaticGlobal.Keyboard.IsKeyClicked(Keys.R) || StaticGlobal.CM.SimulationEnd)
                    {
                        StaticGlobal.CM.Initilize(10, AI.AIType.TargetingPoints);
                        Camera.ChangeSpectatingCell(0);
                    }

                    StaticGlobal.CM.Update(gameTime);

                    // Pause
                    if (StaticGlobal.Keyboard.IsKeyClicked(Keys.Space)
                        || StaticGlobal.Keyboard.IsKeyClicked(Keys.Escape)
                        || StaticGlobal.Keyboard.IsKeyClicked(Keys.P))
                    {
                        menu.ChangeState(Menu.State.PausedSimulation);
                    }

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
                        Camera.Zoom += StaticGlobal.Mouse.ScrollWheelDifference * 0.001f;
                    }

                    if (StaticGlobal.Keyboard.IsKeyClicked(Keys.F))
                    { Camera.ToggleFreeCam(); }
                    if (StaticGlobal.Keyboard.IsKeyClicked(Keys.W) ||
                        StaticGlobal.Keyboard.IsKeyClicked(Keys.A) ||
                        StaticGlobal.Keyboard.IsKeyClicked(Keys.S) ||
                        StaticGlobal.Keyboard.IsKeyClicked(Keys.D))
                    { Camera.FreeCam = true; }

                    // Prevent the Camera to move while spectating cell is a Player
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

                    if (Camera.SpectatingCell.isMarkedForDelete)
                    {
                        Camera.ChangeSpectatingCell(0);
                    }
                    /* END OF case Menu.State.Simulation */
                    break;
                case Menu.State.PausedSimulation:
                    /* START OF case Menu.State.PausedSimulation */
                    // Update
                    menu.ButtonGroup_PausedSimulation.Update();
                    // Return to Simulation (Unpause)
                    if (StaticGlobal.Keyboard.IsKeyClicked(Keys.Space)
                        || StaticGlobal.Keyboard.IsKeyClicked(Keys.Escape)
                        || StaticGlobal.Keyboard.IsKeyClicked(Keys.P))
                    {
                        menu.ChangeState(Menu.State.Simulation);
                    }
                    /* END OF case Menu.State.PausedSimulation */
                    break;
                default: break;
            }
            /* END OF switch(menu.CurrentState) */

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.F11))
            {
                StaticGlobal.Screen.ToggleFullScreen(graphics, GraphicsDevice);
            }


            // Check for shutdown
            if (StaticGlobal.ExitInProgress)
                Shutdown();

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
            // Needed before Render.Draw();
            StaticGlobal.BasicEffect.ApplyCurrentTechnique();

            // clear screen
            GraphicsDevice.Clear(Color.WhiteSmoke);


            // Begin spriteBatch with closest neighbour
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            /* START OF switch(meny.CurrentState) */
            switch (menu.CurrentState)
            {
                case Menu.State.MainMenu:
                    /* START OF case Menu.State.MainMenu */
                    menu.ButtonGroup_MainMenu.Render(GraphicsDevice, spriteBatch, spriteFont);
                    /* END OF case Menu.State.MainMenu */
                    break;
                case Menu.State.InfoScreen:
                    /* START OF case Menu.State.InfoScreen */
                    spriteBatch.Draw(infoText, infoRectangle, Color.Black);

                    menu.ButtonGroup_InfoScreen.Render(GraphicsDevice, spriteBatch, spriteFont);
                    /* END OF case Menu.State.InfoScreen */
                    break;
                case Menu.State.Simulation:
                    /* START OF case Menu.State.Simulation */
                    if (Camera.SpectatingCell != null && !Camera.FreeCam)
                    {
                        Camera.Position = Vector2.Lerp(Camera.Position, Camera.SpectatingCell.Position, 0.125f);
                    }

                    Render.Draw(StaticGlobal.CM, GraphicsDevice);

                    spriteBatch.DrawString(spriteFont, debugMessage, new Vector2(10, 35), Color.Black);

                    if (Camera.SpectatingCell != null && !Camera.FreeCam)
                    {
                        spriteBatch.DrawString(spriteFont, "About current Cell:", new Vector2(10, 80 - 20 * 1), Color.Black);
                        spriteBatch.DrawString(spriteFont, "Energy: " + Math.Floor(Camera.SpectatingCell.Energy).ToString(), new Vector2(10, 80 + 20 * 0), Color.Black);
                        spriteBatch.DrawString(spriteFont, "Family: " + Camera.SpectatingCell.AI.family.ToString(), new Vector2(10, 80 + 20 * 1), Color.Black);
                        spriteBatch.DrawString(spriteFont, "FamilyCount: " + StaticGlobal.Family.FamilyCount(Camera.SpectatingCell.AI.family).ToString(), new Vector2(10, 80 + 20 * 2), Color.Black);
                        spriteBatch.DrawString(spriteFont, "Memory: " + Camera.SpectatingCell.AI.lastMemory?.ToString(), new Vector2(10, 80 + 20 * 3), Color.Black);
                        spriteBatch.DrawString(spriteFont, "AI Type: " + Camera.SpectatingCell.AI.GetAIType.ToString(), new Vector2(10, 80 + 20 * 4), Color.Black);

                        float energy = Camera.SpectatingCell.Energy;
                        if (energy > 1500)
                        {
                            energyBar.Color = Color.Lerp(Color.DodgerBlue, Color.Blue, 0.5f * ((float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 2) + 1));
                            energyBar.Height = (int)MathHelper.Lerp(energyBar.Height, 380, 0.5f);
                        }
                        else
                        {
                            energyBar.Height = (int)MathHelper.Lerp(energyBar.Height, (int)energy >> 2, 0.5f);
                            energyBar.Color = Color.Lerp(Color.Red, Color.LawnGreen, energy / 1500);
                        }
                        energyBar.X = StaticGlobal.Screen.Area.Right - (energyBar.Width + 16);
                        energyBar.Y = StaticGlobal.Screen.Area.Bottom - (energyBar.Height + 16);
                        energyBar.Render(GraphicsDevice);
                    }

                    /* END OF case Menu.State.Simulation */
                    break;
                case Menu.State.PausedSimulation:
                    /* START OF case Menu.State.PausedSimulation */
                    Render.Draw(StaticGlobal.CM, GraphicsDevice);

                    shadowPauseOverlay.Width = Window.ClientBounds.Width;
                    shadowPauseOverlay.Height = Window.ClientBounds.Height;
                    shadowPauseOverlay.Render(GraphicsDevice);

                    menu.ButtonGroup_PausedSimulation.Render(GraphicsDevice, spriteBatch, spriteFont);
                    /* END OF case Menu.State.PausedSimulation */
                    break;
                default: break;
            }
            /* END OF switch(menu.CurrentState) */
            
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
            var x = infoRectangle.X;

            var g1 = StaticGlobal.Screen.Location.X + infoRectangle.X;
            Console.WriteLine(((GameWindow)sender).ClientBounds.ToString());

            Console.WriteLine(sender.ToString());
            StaticGlobal.BasicEffect.Resize(GraphicsDevice);
            StaticGlobal.Screen.Resize(Window.ClientBounds);

            infoRectangle.X = g1 - Window.ClientBounds.X;

            menu.ButtonGroup_MainMenu?.Resize();
            menu.ButtonGroup_InfoScreen?.Resize();
            menu.ButtonGroup_Simulation?.Resize();
            menu.ButtonGroup_PausedSimulation?.Resize();
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
 