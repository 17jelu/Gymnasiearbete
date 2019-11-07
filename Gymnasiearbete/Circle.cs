using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class Circle
    {
        public enum UnitCircle
        {
            Point8,
            Point16
        }

        BasicEffect effect;
        VertexPositionColor[] vertices;

        float radius;
        Vector2 pos;

        /// <summary>
        /// Used in Render() to tell how many triangles shall be rendered.
        /// </summary>
        int triangles;

        public Circle(UnitCircle type, GraphicsDevice graphicsDevice, Color color, float radius, Vector2 position)
        {
            // Load BasicEffect
            effect = new BasicEffect(graphicsDevice);
            effect.VertexColorEnabled = true;
            effect.Projection = Matrix.CreateOrthographicOffCenter
                (0, graphicsDevice.Viewport.Width,     // left, right
                graphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);
            effect.CurrentTechnique.Passes[0].Apply();

            // Properties
            this.radius = radius;
            this.pos = position;

            Vector2[] preCalc;
            if (type == UnitCircle.Point16)
            {
                vertices = new VertexPositionColor[16];
                triangles = 14;
                preCalc = PreCalc.Point16UnitCircle;
            }
            else
            {
                vertices = new VertexPositionColor[8];
                triangles = 6;
                preCalc = PreCalc.Point8UnitCircle;
            }

            // Give each vertex a color
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }

            // Positionate each vertex
            for (int i = 0, j = 0; i < vertices.Length; i++)
            {
                if (i == 0)
                    vertices[0].Position = new Vector3(pos + preCalc[0] * radius, 0);
                else
                {
                    if (i % 2 == 1)
                    {
                        j++;
                        vertices[i].Position = new Vector3(pos + preCalc[j] * radius, 0);
                    }
                    else
                    {
                        vertices[i].Position = new Vector3(pos + preCalc[preCalc.Length - j] * radius, 0);
                    }
                }
            }
        }

        public void Render(GraphicsDevice GraphicsDevice)
        {
            effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                vertices,
                0,
                triangles);
        }
    }
}
