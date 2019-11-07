using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class Camera
    {
        private Vector2 pos;
        public Vector2 Position
        {
            get { return pos; }
            set { pos = value; }
        }

        private float zoom;
        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }

        public Camera(Vector2 position) // Temporary? | +2019-10-01 14:33
        {
            pos = position;
            zoom = 1f;
        }
    }
}
