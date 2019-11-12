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
            line = new Line();
        }
        public static void Draw(CellManager CM, GraphicsDevice GraphicsDevice, Camera camera)
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
                            circleFood.Radius = sector.Foods[i].Size * camera.Zoom;
                            circleFood.Position = (sector.Foods[i].Position - camera.Position) * camera.Zoom;
                            circleFood.UpdateVertices();

                            circleFood.Render(GraphicsDevice);
                        }
                        // Draw Cells
                        for (int i = 0; i < sector.Cells.Count; i++)
                        {
                            circleCell.Color = Color.Red;
                            circleCell.Radius = sector.Cells[i].Size * camera.Zoom;
                            circleCell.Position = (sector.Cells[i].Position - camera.Position) * camera.Zoom;
                            circleCell.UpdateVertices();

                            circleCell.Render(GraphicsDevice);
                        }
                        // Drawing Cells "destination"/"direction"
                        for (int i = 0; i < sector.Cells.Count; i++)
                        {
                            if (true)//sector.Cells[i].AI.lastIntresst != null)
                            {
                                circleCell.Color = Color.Black;
                                circleCell.Radius = 2.5f * camera.Zoom;
                                circleCell.Position = (sector.Cells[i].Position + (sector.Cells[i].AI.Direction) - camera.Position) * camera.Zoom;
                                circleCell.UpdateVertices();
                                circleCell.Render(GraphicsDevice);

                                line.Color = Color.Orange;
                                line.SetLine(
                                    sector.Cells[i].Position,
                                    sector.Cells[i].Position + (sector.Cells[i].AI.Direction),
                                    camera);
                                line.Render(GraphicsDevice);
                            }
                        }
                    }
                }
            }
        }
    }
}
