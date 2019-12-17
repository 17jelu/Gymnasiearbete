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

        private static Cell cell;
        public static Cell SpectatingCell
        {
            get { return cell; }
            set { cell = value; }
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
            _pos = Vector2.Zero;
            zoom = 1f;
            freecam = false;
            cell = null;
        }

        public static Vector2 GetRelativePosition(Vector2 pos)
        {
            return ( pos - _pos ) * zoom +
                new Vector2(
                    StaticGlobal.Screen.Area.Width / 2,
                    StaticGlobal.Screen.Area.Height / 2
                );
        }
        public static Vector3 GetRelativePosition(Vector3 pos)
        {
            Vector3 temp = pos;

            temp.X = (temp.X - _pos.X) * zoom + StaticGlobal.Screen.Area.Width / 2;
            temp.Y = (temp.Y - _pos.Y) * zoom + StaticGlobal.Screen.Area.Height / 2;

            return temp;
        }

        public static void ChangeSpectatingCell(int num, CellManager CM)
        {
            int index = CM.Content.Cells.IndexOf(cell);

            if (CM.Content.Cells.Count > 0)
            {
                cell = CM.Content.Cells[
                    (CM.Content.Cells.Count
                    + index
                    + num)
                    % (CM.Content.Cells.Count)
                ];
            }
        }
    }
}
