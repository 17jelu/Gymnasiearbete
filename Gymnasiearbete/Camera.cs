using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    /* IDÉER
     * kunna zooma ut så att man ser hela "brädet"
     * följa efter en cell
     */


    class Camera
    {
        private Vector2 pos;
        public Vector2 Position
        {
            get { return pos; }
            set { pos = value; }
        }

        public float X // Temporary? | +2019-10-01 14:54
        {
            get { return pos.X; }
            set { pos.X = value; }
        }
        public float Y // Temporary? | +2019-10-01 14:54
        {
            get { return pos.Y; }
            set { pos.Y = value; }
        }

        public Camera(Vector2 position) // Temporary? | +2019-10-01 14:33
        {
            pos = position;
        }
    }
}
