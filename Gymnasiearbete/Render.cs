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
            gridLine = new Line { Color = Color.Gray };
        }

        /// <summary>
        /// Renders the simulation
        /// </summary>
        public static void Draw(CellManager CM, GraphicsDevice GraphicsDevice)
        {
            byte
                gridWidth = (byte)(1+ StaticGlobal.Screen.Area.Width / (StaticGlobal.SectorSize * Camera.Zoom)),
                gridHeight = (byte)(1+ StaticGlobal.Screen.Area.Height / (StaticGlobal.SectorSize * Camera.Zoom));
            float
                offset;

            // Drawing the Grid (vertical)
            for (int y = -(gridHeight >> 1) - 1; y < (gridHeight << 1); y++)
            {
                offset =
                    y * StaticGlobal.SectorSize * Camera.Zoom
                    - (Camera.Position.Y % StaticGlobal.SectorSize * Camera.Zoom)
                    + (StaticGlobal.Screen.Area.Height >> 1);

                gridLine.SetLine(
                    new Vector2(0, offset),
                    new Vector2(StaticGlobal.Screen.Area.Width, offset),
                    false
                );
                gridLine.Render(GraphicsDevice);
            }
            // Drawing the Grid (horizontal)
            for (int x = -(gridWidth >> 1) - 1; x < (gridWidth << 1); x++)
            {
                offset =
                    x * StaticGlobal.SectorSize * Camera.Zoom
                    - (Camera.Position.X % StaticGlobal.SectorSize * Camera.Zoom)
                    + (StaticGlobal.Screen.Area.Width >> 1);

                gridLine.SetLine(
                    new Vector2(offset, 0),
                    new Vector2(offset, StaticGlobal.Screen.Area.Height),
                    false
                );
                gridLine.Render(GraphicsDevice);
            }

            int
                xoff = (int)(Camera.Position.X / StaticGlobal.SectorSize)
                    - (gridWidth >> 1) - 2, // - 2 because out of screen
                yoff = (int)(Camera.Position.Y / StaticGlobal.SectorSize)
                    - (gridHeight >> 1) - 2 // - 2 because out of screen
            ;
            // Drawing content inside of Chunks
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
                            switch (sector.Cells[i].AI.family.Split(' ')[0])
                            {
                                case "Red":
                                    circleCell.Color = Color.Red;
                                    break;
                                case "Blue":
                                    circleCell.Color = Color.Blue;
                                    break;
                                case "Green":
                                    circleCell.Color = Color.Green;
                                    break;
                                case "Cyan":
                                    circleCell.Color = Color.Cyan;
                                    break;
                                case "Magenta":
                                    circleCell.Color = Color.Magenta;
                                    break;
                                case "Yellow":
                                    circleCell.Color = Color.Yellow;
                                    break;
                                case "Black":
                                    circleCell.Color = Color.Black;
                                    break;
                                case "White":
                                    circleCell.Color = Color.White;
                                    break;
                                default: break;
                            };

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

            // END OF Draw()
        }
    }
}