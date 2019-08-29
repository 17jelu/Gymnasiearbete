using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class Circle
    {
        BasicEffect effect;
        VertexPositionColor[] vertices;

        float radius;
        Vector2 pos;

        public Circle(GraphicsDevice GraphicsDevice, Color color, int radius, int x, int y)
        {
            // Load BasicEffect
            effect = new BasicEffect(GraphicsDevice);
            effect.VertexColorEnabled = true;
            effect.Projection = Matrix.CreateOrthographicOffCenter
                (0, GraphicsDevice.Viewport.Width,     // left, right
                GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);
            effect.CurrentTechnique.Passes[0].Apply();
            
            // Settings
            this.radius = radius;
            pos = new Vector2(x, y);

            // This circle will have 16 vertices
            vertices = new VertexPositionColor[16];

            // Give each vertex a color
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }

            // Positionate each vertex
            for (int i = 0, j = 0; i < vertices.Length; i++)
            {
                if (i == 0)
                    vertices[0].Position = new Vector3(pos + PreCalc.Point16UnitCircle[0] * radius, 0);
                else
                {
                    if (i % 2 == 1)
                    {
                        j++;
                        vertices[i].Position = new Vector3(pos + PreCalc.Point16UnitCircle[j] * radius, 0);
                    }
                    else
                    {
                        vertices[i].Position = new Vector3(pos + PreCalc.Point16UnitCircle[16 - j] * radius, 0);
                    }
                }
            }
        }

        public void Render(GraphicsDevice GraphicsDevice)
        {
            effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertices, 0, 14);
        }

        /* LATER USE FOR JESPER
        public void Update(GameTime gameTime, bool isFixed)
        {
            // Positionate each vertex
            for (int i = 0, j = 0; i < vertices.Length; i++)
            {
                if (i == 0)
                    vertices[0].Position = new Vector3(pos + PreCalc.Point16UnitCircle[0] * radius, 0);
                else
                {
                    if (i % 2 == 1)
                    {
                        j++;
                        vertices[i].Position = new Vector3(pos + PreCalc.Point16UnitCircle[j] * radius, 0);
                    }
                    else
                    {
                        vertices[i].Position = new Vector3(pos + PreCalc.Point16UnitCircle[16 - j] * radius, 0);
                    }
                }
            }
        }
        */
    }
}
