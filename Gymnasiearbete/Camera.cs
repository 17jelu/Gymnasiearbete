using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    struct Camera
    {
        private static Vector2 _pos;
        public static Vector2 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        private static float zoom;
        public static float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                zoom = zoom < 0.125f ? 0.125f : zoom;
            }
        }

        private static bool freecam;
        public static bool FreeCam
        {
            get { return freecam; }
            set { freecam = value; }
        }
        public static bool ToggleFreeCam()
        {
            freecam =! freecam;
            return freecam;
        }

        public static void Initialize()
        {
            Camera.Position = Vector2.Zero;
            Camera.Zoom = 1f;
            Camera.FreeCam = false;
        }

        public static Vector2 GetRelativePosition(Vector2 pos)
        {
            return ( pos - _pos ) * zoom +
                new Vector2(
                    SGScreen.Area.Width / 2,
                    SGScreen.Area.Height / 2
                );
        }
        public static Vector3 GetRelativePosition(Vector3 pos)
        {
            Vector3 temp = pos;

            temp.X = (temp.X - _pos.X) * zoom + SGScreen.Area.Width / 2;
            temp.Y = (temp.Y - _pos.Y) * zoom + SGScreen.Area.Height / 2;

            return temp;
        }
    }
}
