using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gymnasiearbete
{
    class UIElement
    {
        bool hover, active, selected = false;
        Rectangle collisionBox;

        public bool Hover => hover;
        public bool Selected => selected;
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public Rectangle Bounds
        {
            get { return collisionBox; }
            set { collisionBox = value; }
        }

        public virtual void Update()
        {
            hover = collisionBox.Contains(StaticGlobal.Mouse.Location);
            active = hover && StaticGlobal.Mouse.IsButtonHeld(CustomMouseButtons.LeftButton);
        }
    }

    class Button : UIElement
    {
        bool old_active;

        private Action action;
        public Action Action
        {
            get { return action; }
            set { action = value; }
        }

        public Button()
        {
            old_active = false;
        }

        public override void Update()
        {
            base.Update();

            // When mouse release, act
            if (Hover && action != null && old_active && !Active)
                action();

            old_active = Active;
        }
    }

    class UIElementHandler
    {
        static Button[] buttons;
        static GraphicRectangle body;
        static GraphicRectangle bottom;

        // Implementing "select" aka TAB thingy
        static VertexPositionColor[] vertices;
        static int selectedButtonIndex;
        const int outlineThickness = 5;

        public static void Initialize()
        {
            #region Buttons
            body = new GraphicRectangle(Color.White, 0, 0, 0, 0);
            bottom = new GraphicRectangle(Color.White, 0, 0, 0, 0);

            buttons = new Button[]
            {
                // NULL BUTTON
                new Button
                {
                    Bounds = new Rectangle(-100, -100, 0, 0)
                },

                // Hello
                new Button
                {
                    Bounds = new Rectangle
                    {
                        Width = 150,
                        Height = 50,
                        X = 20,
                        Y = 20
                    },
                    Action = () => Console.WriteLine("Hello")
                },

                // Exit button
                new Button
                {
                    Bounds = new Rectangle
                    {
                        Width = 100,
                        Height = 100,
                        X = 200,
                        Y = 200
                    },
                    Action = () =>
                    {
                        StaticGlobal.SaveModeON = true;
                        StaticGlobal.Shutdown();
                    }
                },

                // Just cause
                new Button
                {
                    Bounds = new Rectangle
                    {
                        Width = 50,
                        Height = 50,
                        X = StaticGlobal.Screen.Area.Width - 100,
                        Y = 100
                    },
                    Action = () =>
                    {
                        Camera.FreeCam = true;
                        Camera.Position = new Vector2(int.MaxValue, int.MinValue);
                    }
                }
            };
            #endregion Buttons

            #region Select Outline
            vertices = new VertexPositionColor[10];
            selectedButtonIndex = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = Color.DodgerBlue;
            }
            #endregion Select Outline
        }

        public static void Update()
        {
            foreach (var button in buttons)
            {
                button.Update();
            }

            if (StaticGlobal.Keyboard.IsKeyClicked(Keys.Tab))
            {
                if (!StaticGlobal.Keyboard.IsKeyDown(Keys.LeftShift))
                { // Next
                    selectedButtonIndex += 1;
                }
                else // Previous
                {
                    selectedButtonIndex -= 1;
                }
                selectedButtonIndex = (buttons.Length + selectedButtonIndex) % buttons.Length;
            }

            if (StaticGlobal.Keyboard.IsKeyHeld(Keys.Enter))
            {
                buttons[selectedButtonIndex].Active = true;
            }
            if (StaticGlobal.Keyboard.IsKeyReleased(Keys.Enter))
            {
                buttons[selectedButtonIndex].Action();
            }
        }

        public static void Render(GraphicsDevice GraphicsDevice)
        {
            foreach (Button button in buttons)
            {
                // Default placement
                bottom.Width = button.Bounds.Width;
                bottom.Height = 5;
                bottom.X = button.Bounds.X;
                bottom.Y = button.Bounds.Y + button.Bounds.Height - bottom.Height;
                body.Width = button.Bounds.Width;
                body.Height = button.Bounds.Height - bottom.Height;
                body.X = button.Bounds.X;
                body.Y = button.Bounds.Y;

                // Mouse is hovering over button
                if (button.Hover)
                {
                    body.Color = new Color(152, 227, 227);
                    bottom.Color = new Color(118, 188, 188);
                }
                else
                {
                    body.Color = Color.LightGray;
                    bottom.Color = Color.Silver;
                }

                // Mouse is being pressed
                if (button.Active)
                {
                    body.Y = button.Bounds.Y + bottom.Height;
                }

                bottom.Render(GraphicsDevice);
                body.Render(GraphicsDevice);
            }

            #region Selected Outline
            vertices[0].Position = new Vector3(
                buttons[selectedButtonIndex].Bounds.X - outlineThickness,
                buttons[selectedButtonIndex].Bounds.Y - outlineThickness,
            0);
            vertices[1].Position = new Vector3(
                buttons[selectedButtonIndex].Bounds.X - outlineThickness * 2,
                buttons[selectedButtonIndex].Bounds.Y - outlineThickness * 2,
            0);
            vertices[2].Position = new Vector3(
                vertices[0].Position.X + buttons[selectedButtonIndex].Bounds.Width + outlineThickness * 2,
                vertices[0].Position.Y,
            0);
            vertices[3].Position = new Vector3(
                vertices[2].Position.X + outlineThickness,
                vertices[2].Position.Y - outlineThickness,
            0);
            vertices[4].Position = new Vector3(
                vertices[2].Position.X,
                vertices[2].Position.Y + buttons[selectedButtonIndex].Bounds.Height + outlineThickness * 2,
            0);
            vertices[5].Position = new Vector3(
                vertices[4].Position.X + outlineThickness,
                vertices[4].Position.Y + outlineThickness,
            0);
            vertices[6].Position = new Vector3(
                vertices[0].Position.X,
                vertices[0].Position.Y + buttons[selectedButtonIndex].Bounds.Height + outlineThickness * 2,
            0);
            vertices[7].Position = new Vector3(
                vertices[6].Position.X - outlineThickness,
                vertices[6].Position.Y + outlineThickness,
            0);
            // Tying the knot
            vertices[8].Position = vertices[0].Position;
            vertices[9].Position = vertices[1].Position;

            // Render Selected Outline
            GraphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleStrip,
                vertices,
                0,
                vertices.Length - 2);
            #endregion Selected Outline
        }
    }
}
