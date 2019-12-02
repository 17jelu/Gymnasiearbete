using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class Render
    {
        private static Circle circleCell, circleFood;
        private static Line line, gridLine;

        public static void Initialize()
        {
            circleCell = new Circle(Circle.UnitCircle.Point16, Color.Red, 10f, Vector2.Zero);
            circleFood = new Circle(Circle.UnitCircle.Point8, Color.LawnGreen, 10f, Vector2.Zero);
            line = new Line { Color = Color.Orange };
            gridLine = new Line { Color = Color.White };
        }
        public static void Draw(CellManager CM, GraphicsDevice GraphicsDevice)
        {
            byte
                gridWidth = (byte)(1+ SGScreen.Area.Width / (CellManager.SectorSize * Camera.Zoom)),
                gridHeight = (byte)(1+ SGScreen.Area.Height / (CellManager.SectorSize * Camera.Zoom));
            float
                offset;

            // Drawing the Grid (vertical)
            for (int y = -(gridHeight >> 1) - 1; y < (gridHeight << 1); y++)
            {
                offset =
                    y * CellManager.SectorSize * Camera.Zoom
                    - (Camera.Position.Y % CellManager.SectorSize * Camera.Zoom)
                    + (SGScreen.Area.Height >> 1);

                gridLine.SetLine(
                    new Vector2(0, offset),
                    new Vector2(SGScreen.Area.Width, offset),
                    false
                );
                gridLine.Render(GraphicsDevice);
            }
            // Drawing the Grid (horizontal)
            for (int x = -(gridWidth >> 1) - 1; x < (gridWidth << 1); x++)
            {
                offset =
                    x * CellManager.SectorSize * Camera.Zoom
                    - (Camera.Position.X % CellManager.SectorSize * Camera.Zoom)
                    + (SGScreen.Area.Width >> 1);

                gridLine.SetLine(
                    new Vector2(offset, 0),
                    new Vector2(offset, SGScreen.Area.Height),
                    false
                );
                gridLine.Render(GraphicsDevice);
            }

            int
                xoff = (int)(Camera.Position.X / CellManager.SectorSize)
                    - (gridWidth >> 1) - 2, // - 2 because out of screen
                yoff = (int)(Camera.Position.Y / CellManager.SectorSize)
                    - (gridHeight >> 1) - 2 // - 2 because out of screen
            ;
            // Drawing content inside of Chunks
            // TODO: add offset to points
            for (int y = yoff; y < gridHeight + yoff + 3; y++) // + 3 because out of screen
            {
                for (int x = xoff; x < gridWidth + xoff + 3; x++) // + 3 because out of screen
                {
                    Point p = new Point(
                        x,
                        y
                    );
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
                    else
                    {
                        line.SetLine(
                            new Vector2(
                                x * CellManager.SectorSize,
                                y * CellManager.SectorSize),
                            new Vector2(
                                x * CellManager.SectorSize + CellManager.SectorSize,
                                y * CellManager.SectorSize + CellManager.SectorSize
                            ));
                        line.Render(GraphicsDevice);

                        line.SetLine(
                            new Vector2(
                                x * CellManager.SectorSize + CellManager.SectorSize,
                                y * CellManager.SectorSize),
                            new Vector2(
                                x * CellManager.SectorSize,
                                y * CellManager.SectorSize + CellManager.SectorSize
                            ));
                        line.Render(GraphicsDevice);
                    }
                }
            }
        }
    }
}