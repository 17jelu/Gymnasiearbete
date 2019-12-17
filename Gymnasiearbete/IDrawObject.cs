using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    interface IDrawObject
    {
        void Render(GraphicsDevice GraphicsDevice);
    }

    class DrawObject : IDrawObject
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
        // Outline
        VertexPositionColor[] lines;

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
                lines = new VertexPositionColor[17];
                vertices = new VertexPositionColor[16];
                triangles = 14;
                preCalc = PreCalc.Point16UnitCircle;
            }
            else
            {
                lines = new VertexPositionColor[9];
                vertices = new VertexPositionColor[8];
                triangles = 6;
                preCalc = PreCalc.Point8UnitCircle;
            }

            // Give each vertex a color
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }
            // Give each line vertex the color black
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].Color = Color.Black;
            }

            // Positionate each vertex
            UpdateVertices();
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

            // Position each vertex (Outline)
            for (int i = 0; i < lines.Length; i++)
            {
                if (i < preCalc.Length)
                    lines[i].Position = new Vector3(pos + preCalc[i] * radius, 0);
                else
                    lines[i].Position = new Vector3(pos + preCalc[0] * radius, 0);
            }
        }

        private void UpdateColor(Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }
        }

        new public void Render(GraphicsDevice GraphicsDevice)
        {
            base.Render(GraphicsDevice);
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.LineStrip,
                lines,
                0,
                triangles + 2);
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
            vertices = new VertexPositionColor[2];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new VertexPositionColor(Vector3.Zero, Color.Black);
            }
        }

        public void SetLine(Vector2 vector1, Vector2 vector2)
        {
            SetLine(vector1, vector2, true);
        }
        public void SetLine(Vector2 vector1, Vector2 vector2, bool relativeToCamera)
        {
            /*
            // 1 3 2 0
            if (vector1.X < vector2.X)
            {
                vertices[1].Position.X = vector1.X;
                vertices[1].Position.Y = vector1.Y - GAP;
                vertices[0].Position.X = vector1.X;
                vertices[0].Position.Y = vector1.Y + GAP;

                vertices[3].Position.X = vector2.X;
                vertices[3].Position.Y = vector2.Y - GAP;
                vertices[2].Position.X = vector2.X;
                vertices[2].Position.Y = vector2.Y + GAP;
            }
            else
            {
                vertices[3].Position.X = vector1.X;
                vertices[3].Position.Y = vector1.Y - GAP;
                vertices[2].Position.X = vector1.X;
                vertices[2].Position.Y = vector1.Y + GAP;

                vertices[1].Position.X = vector2.X;
                vertices[1].Position.Y = vector2.Y - GAP;
                vertices[0].Position.X = vector2.X;
                vertices[0].Position.Y = vector2.Y + GAP;
            }
            /*/
            vertices[0].Position.X = vector1.X;
            vertices[0].Position.Y = vector1.Y;
            vertices[1].Position.X = vector2.X;
            vertices[1].Position.Y = vector2.Y;

            if (relativeToCamera)
            // Align pos with Camera
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position = Camera.GetRelativePosition(vertices[i].Position);
            }
        }

        private void UpdateColor(Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }
        }

        new public void Render(GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.LineStrip,
                vertices,
                0,
                1);
        }
    }

    class GraphicRectangle : DrawObject
    {
        _Rectangle rect;
        private struct _Rectangle
        {
            public int X, Y, Width, Height;
            public _Rectangle(int x, int y, int width, int height)
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

        // Outline
        VertexPositionColor[] lines;

        public GraphicRectangle(Color color, int x, int y, int width, int height)
        {
            rect = new _Rectangle(x, y, width, height);

            vertices = new VertexPositionColor[4];
            lines = new VertexPositionColor[5];
            triangles = 2;

            Color = color;

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].Color = Microsoft.Xna.Framework.Color.Black;
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

            // Outline
            lines[0].Position.X = rect.X;
            lines[0].Position.Y = rect.Y;

            lines[1].Position.X = rect.X + rect.Width;
            lines[1].Position.Y = rect.Y;

            lines[2].Position.X = rect.X + rect.Width;
            lines[2].Position.Y = rect.Y + rect.Height;

            lines[3].Position.X = rect.X;
            lines[3].Position.Y = rect.Y + rect.Height;

            lines[4].Position = lines[0].Position;
        }

        public void SetCustomColor(Color color1, Color color2, Color color3, Color color4)
        {
            // Top Right Bottom Left
            vertices[1].Color = color1;
            vertices[3].Color = color2;
            vertices[2].Color = color3;
            vertices[0].Color = color4;
        }

        public object Color
        {
            set
            {
                if (value.GetType() == typeof(Color))
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        vertices[i].Color = (Color)value;
                    }
                }
                else if (value.GetType() == typeof(Color[]))
                {
                    Color[] color = (Color[])value;
                    for (int i = 0; i < 4; i++)
                    {
                        vertices[i].Color =
                            color[i % color.Length];
                    }
                }
            }
        }

        new public void Render(GraphicsDevice GraphicsDevice)
        {
            base.Render(GraphicsDevice);
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.LineStrip,
                lines,
                0,
                triangles + 2);
        }
    }

    class CustomDrawObject : DrawObject
    {
        Vector2[] initVertices;

        private float scale;
        public float Scale
        {
            get { return scale; }
            set { scale = value; UpdateVertices(); }
        }

        new public void Render(GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleList,
                vertices,
                0,
                triangles);
        }

        public CustomDrawObject(Vector2[] vertices, Vector2[] outline)
        {
            if (vertices.Length % 3 != 0)
                throw new System.ArgumentException("A CustomDrawObject must have a length dividable by 3", "vertices");

            scale = 1f;

            initVertices = new Vector2[vertices.Length];
            this.vertices = new VertexPositionColor[initVertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                initVertices[i] = vertices[i];
            }
            UpdateVertices();
            triangles = this.vertices.Length / 3;
        }

        private void UpdateVertices()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position = new Vector3(initVertices[i] * scale, 0);
                vertices[i].Color = Color.Black;
            }
        }
    }
}
