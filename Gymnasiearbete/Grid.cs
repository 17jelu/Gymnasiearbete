using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class Grid
    {
        // Used to draw lines
        Texture2D texture;

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

        /// <summary>
        /// Clears the screen and draws grid lines in relation to the current Camera position.
        /// </summary>
        public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, float x, float y)
        {
            graphicsDevice.Clear(Color.White);
        }
    }
}