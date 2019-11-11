using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class Grid
    {
        // XNA.Framework stuff
        Texture2D texture; // Used to draw lines
        Rectangle horizontalR;
        Rectangle verticalR;

        // Constants
        const int tileWidth = 100;
        const int lineWidth = 2;

        // Colors
        Color BACKGROUND_COLOR = Color.Black;
        Color LINE_COLOR = Color.WhiteSmoke;

        public Grid(Rectangle clientBounds)
        {
            horizontalR = new Rectangle(0, 0, clientBounds.Width, lineWidth);
            verticalR = new Rectangle(0, 0, lineWidth, clientBounds.Height);
        }

        /// <summary>
        /// Uses Game.GraphicsDevice to create neccessary texture used by Grid.cs
        /// <para>This has to be called prior to Draw()</para>
        /// </summary>
        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            // Creates 1x1 texture using GraphicsDevice
            texture = new Texture2D(graphicsDevice, 1, 1);
            // Sets the pixel data in texture to be 1 Color.White
            texture.SetData(new Color[] { Color.White });
        }

        /*|                    ==:TODO:=                        |*\
        |*|    The Draw method must support different Camera    |*|
        \*| properties, such as Angle (rotate) and Zoom (scale) |*/
        /// <summary>
        /// Clears the screen and draws grid lines in relation to the current Camera position.
        /// </summary>
        public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Camera camera)
        {
            graphicsDevice.Clear(BACKGROUND_COLOR);

            horizontalR.Y = (horizontalR.Height - (int)camera.Y % tileWidth);
            for (int i = 0; i < (verticalR.Height / tileWidth) + 2; i++)
            {
                spriteBatch.Draw(texture, horizontalR, LINE_COLOR);
                horizontalR.Y += tileWidth;
            }

            verticalR.X = (verticalR.Width - (int)camera.X % tileWidth);
            for (int i = 0; i < (horizontalR.Width / tileWidth) + 2; i++)
            {
                spriteBatch.Draw(texture, verticalR, LINE_COLOR);
                verticalR.X += tileWidth;
            }
        }
    }
}