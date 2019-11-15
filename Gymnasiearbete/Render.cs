using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class Render
    {
        private static Circle circleCell, circleFood;
        private static Line line;

        public static void Initialize()
        {
            circleCell = new Circle(Circle.UnitCircle.Point16, Color.Red, 10f, Vector2.Zero);
            circleFood = new Circle(Circle.UnitCircle.Point8, Color.LawnGreen, 10f, Vector2.Zero);
            line = new Line { Color = Color.Orange };
        }
        public static void Draw(CellManager CM, GraphicsDevice GraphicsDevice)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Point p = new Point(x, y);
                    if (CM.Sectors.ContainsKey(p))
                    {
                        SectorContent sector = CM.Sectors[p];
                        // draw code ...
                        // Draw Food
                        for (int i = 0; i < sector.Foods.Count; i++)
                        {
                            circleFood.Radius = sector.Foods[i].Size * Camera.Zoom;
                            circleFood.Position = Camera.GetRelativePosition(sector.Foods[i].Position);
                            circleFood.UpdateVertices();

                            circleFood.Render(GraphicsDevice);
                        }
                        // Draw Cells
                        for (int i = 0; i < sector.Cells.Count; i++)
                        {
                            circleCell.Radius = sector.Cells[i].Size * Camera.Zoom;
                            circleCell.Position = Camera.GetRelativePosition(sector.Cells[i].Position);
                            circleCell.UpdateVertices();

                            circleCell.Render(GraphicsDevice);
                        }
                        // Drawing Cells "destination"/"direction"
                        for (int i = 0; i < sector.Cells.Count; i++)
                        {
                            line.SetLine(
                                sector.Cells[i].Position,
                                sector.Cells[i].Position + (sector.Cells[i].AI.Direction)
                            );
                            line.Render(GraphicsDevice);
                        }
                    }
                }
            }
        }
    }
}
