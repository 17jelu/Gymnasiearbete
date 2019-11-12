using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class DrawObject
    {
        protected VertexPositionColor[] vertices;
        /// <summary>
        /// Used in Render() to tell how many triangles shall be rendered.
        /// </summary>
        protected int triangles;

        public void Render(GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                vertices,
                0,
                triangles);
        }
    }

    class Circle : DrawObject
    {
        public enum UnitCircle
        {
            Point8,
            Point16
        }

        float radius;
        Vector2 pos;
        Vector2[] preCalc;

        public Vector2 Position
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public Color Color
        {
            get { return vertices[0].Color; }
            set { UpdateColor(value); }
        }

        public Circle(UnitCircle type, Color color, float radius, Vector2 position)
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

        public void UpdateVertices()
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

        private void UpdateColor(Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }
        }
    }

    /// <summary>
    /// An abomination of a so called "line"
    /// </summary>
    class Line : DrawObject
    {
        public Color Color
        {
            get { return vertices[0].Color; }
            set { UpdateColor(value); }
        }

        public Line()
        {
            triangles = 2;

            vertices = new VertexPositionColor[4];
            for (int i = 0; i < 4; i++)
            {
                vertices[i] = new VertexPositionColor(Vector3.Zero, Color.Black);
            }
        }

        public void SetLine(Vector2 vector1, Vector2 vector2, Camera camera)
        {
            // 1 3 2 0
            if (vector1.X < vector2.X)
            {
                vertices[1].Position.X = vector1.X;
                vertices[1].Position.Y = vector1.Y - 1;
                vertices[0].Position.X = vector1.X;
                vertices[0].Position.Y = vector1.Y + 1;

                vertices[3].Position.X = vector2.X;
                vertices[3].Position.Y = vector2.Y - 1;
                vertices[2].Position.X = vector2.X;
                vertices[2].Position.Y = vector2.Y + 1;
            }
            else
            {
                vertices[3].Position.X = vector1.X;
                vertices[3].Position.Y = vector1.Y - 1;
                vertices[2].Position.X = vector1.X;
                vertices[2].Position.Y = vector1.Y + 1;

                vertices[1].Position.X = vector2.X;
                vertices[1].Position.Y = vector2.Y - 1;
                vertices[0].Position.X = vector2.X;
                vertices[0].Position.Y = vector2.Y + 1;
            }

            // Align pos with Camera
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position.X -= camera.Position.X;
                vertices[i].Position.X *= camera.Zoom;
                vertices[i].Position.Y -= camera.Position.Y;
                vertices[i].Position.Y *= camera.Zoom;
            }
        }

        private void UpdateColor(Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }
        }
    }

    class GraphicRectangle : DrawObject
    {
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

        public GraphicRectangle(BasicEffect effect, Color color, int x, int y, int width, int height)
        {
            rect = new Rect(x, y, width, height);

            vertices = new VertexPositionColor[4];
            triangles = 2;

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
    }
}
