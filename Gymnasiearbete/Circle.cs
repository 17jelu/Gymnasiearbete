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
        
        VertexPositionColor[] vertices;

        float radius;
        Vector2 pos;
        Vector2[] preCalc;

        /// <summary>
        /// Used in Render() to tell how many triangles shall be rendered.
        /// </summary>
        int triangles;

        public Circle(UnitCircle type, GraphicsDevice graphicsDevice, Color color, float radius, Vector2 position)
        {
            // Properties
            this.radius = radius;
            this.pos = position;
            
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

        private void UpdateVertices()
        {
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

        public void Render(GraphicsDevice GraphicsDevice, Camera camera)
        {
            Vector2 oldpos = pos;

            pos = pos - camera.Position;

            UpdateVertices();
            pos = oldpos;

            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                vertices,
                0,
                triangles);
        }
    }

    class GraphicRectangle
    {
        BasicEffect effect;
        VertexPositionColor[] vertices;

        Rect rect;
        private struct Rect
        {
            public int X, Y, Width, Height;
            public Rect(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }
        public int X
        {
            get { return rect.X; }
            set { rect.X = value; UpdateVertices(); }
        }
        public int Y
        {
            get { return rect.Y; }
            set { rect.Y = value; UpdateVertices(); }
        }
        public int Width
        {
            get { return rect.Width; }
            set { rect.Width = value; UpdateVertices(); }
        }
        public int Height
        {
            get { return rect.Height; }
            set { rect.Height = value; UpdateVertices(); }
        }

        public GraphicRectangle(BasicEffect effect, GraphicsDevice GraphicsDevice, Color color, int x, int y, int width, int height)
        {
            // Load BasicEffect
            this.effect = effect;

            rect = new Rect(x, y, width, height);

            vertices = new VertexPositionColor[4];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }

            UpdateVertices();
        }

        private void UpdateVertices()
        {
            vertices[0].Position.X = rect.X;
            vertices[0].Position.Y = rect.Y + rect.Height;

            vertices[1].Position.X = rect.X;
            vertices[1].Position.Y = rect.Y;

            vertices[2].Position.X = rect.X + rect.Width;
            vertices[2].Position.Y = rect.Y + rect.Height;

            vertices[3].Position.X = rect.X + rect.Width;
            vertices[3].Position.Y = rect.Y;
        }

        public void Draw(GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                vertices,
                0,
                2);
        }
    }
}
