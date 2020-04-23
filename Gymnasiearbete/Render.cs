using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Gymnasiearbete
{
    class Render
    {
        private static Circle circleCell, circleFood;
        private static Line line;
        private static GraphicRectangle rectangle;

        private static List<float> fps_stat;
        private const float MS_15_FPS = 1000f / 15f; // ~ 66.667...
        private const float MS_30_FPS = 1000f / 30f; // ~ 33.333...
        private const int FPS_LINE_WIDTH = 2;
        private const int HUD_PADDING = 16;
        private const int ENERGYBAR_WIDTH = 25;
        private const int ENERGYBAR_MAXHEIGHT = 200;
        private const int RELATIONBAR_HEIGHT = 100;
        private const int RELATIONBAR_WIDTH = 5;

        public static void Initialize()
        {
            circleCell = new Circle(Circle.UnitCircle.Point16, Color.Red, 10f, Vector2.Zero);
            circleFood = new Circle(Circle.UnitCircle.Point8, Color.LawnGreen, 10f, Vector2.Zero);
            line = new Line { Color = Color.Orange };
            rectangle = new GraphicRectangle(Color.White, 0, 0, 0, 0);

            fps_stat = new List<float>();
            for (int i = 0; i < 200; i++)
            {
                fps_stat.Add(0);
            }
        }

        /// <summary>
        /// Renders the simulation
        /// </summary>
        public static void Draw(CellManager CM, GraphicsDevice GraphicsDevice, GameTime gameTime)
        {
            byte
                gridWidth = (byte)(1+ StaticGlobal.Screen.Area.Width / (StaticGlobal.SectorSize * Camera.Zoom)),
                gridHeight = (byte)(1+ StaticGlobal.Screen.Area.Height / (StaticGlobal.SectorSize * Camera.Zoom));
            float
                offset;

            // Drawing the Grid (vertical)
            line.Color = Color.Gray;
            for (int y = -(gridHeight >> 1) - 1; y < (gridHeight << 1); y++)
            {
                offset =
                    y * StaticGlobal.SectorSize * Camera.Zoom
                    - (Camera.Position.Y % StaticGlobal.SectorSize * Camera.Zoom)
                    + (StaticGlobal.Screen.Area.Height >> 1);

                line.SetLine(
                    new Vector2(0, offset),
                    new Vector2(StaticGlobal.Screen.Area.Width, offset),
                    false
                );
                line.Render(GraphicsDevice);
            }
            // Drawing the Grid (horizontal)
            for (int x = -(gridWidth >> 1) - 1; x < (gridWidth << 1); x++)
            {
                offset =
                    x * StaticGlobal.SectorSize * Camera.Zoom
                    - (Camera.Position.X % StaticGlobal.SectorSize * Camera.Zoom)
                    + (StaticGlobal.Screen.Area.Width >> 1);

                line.SetLine(
                    new Vector2(offset, 0),
                    new Vector2(offset, StaticGlobal.Screen.Area.Height),
                    false
                );
                line.Render(GraphicsDevice);
            }

            int
                xoff = (int)(Camera.Position.X / StaticGlobal.SectorSize)
                    - (gridWidth >> 1) - 0, // - 2 because out of screen
                yoff = (int)(Camera.Position.Y / StaticGlobal.SectorSize)
                    - (gridHeight >> 1) - 0 // - 2 because out of screen
            ;

            // Drawing content inside of Chunks
            line.Color = Color.Orange;
            for (int y = yoff; y < gridHeight + yoff + 1; y++) // + 3 because out of screen
            {
                for (int x = xoff; x < gridWidth + xoff + 1; x++) // + 3 because out of screen
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
                            switch (sector.Cells[i].AI.family)
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
                                sector.Cells[i].Position + (sector.Cells[i].AI.DEBUG_Direction)
                            );
                            line.Render(GraphicsDevice);
                        }
                    }
                }
            }

            // Draw HUD
            // If spectating a cell and not in freecam
            if (Camera.SpectatingCell != null && !Camera.FreeCam)
            {
                // Background
                rectangle.Color = new Color(0, 0, 0, 0.7f);
                rectangle.Width = 200 + HUD_PADDING * 4;
                rectangle.Height = 200 + HUD_PADDING * 4;
                rectangle.X = StaticGlobal.Screen.Area.Right - rectangle.Width;
                rectangle.Y = StaticGlobal.Screen.Area.Bottom - rectangle.Height;
                rectangle.Render(GraphicsDevice);

                

                // EnergyBar
                float energy = Camera.SpectatingCell.Energy;
                if (energy > EnergyControlls.CellEnergyRequirement)
                {
                    rectangle.Color = Color.Lerp(Color.DodgerBlue, Color.Blue, 0.5f * ((float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4) + 1));
                    rectangle.Height = (int)MathHelper.Lerp(rectangle.Height, ENERGYBAR_MAXHEIGHT, 0.5f);
                }
                else
                {
                    rectangle.Height = (int)MathHelper.Lerp(rectangle.Height, energy / ((float)EnergyControlls.CellEnergyRequirement / ENERGYBAR_MAXHEIGHT), 0.5f);
                    rectangle.Color = Color.Lerp(Color.Red, Color.LawnGreen, energy / (float)EnergyControlls.CellEnergyRequirement);
                }
                rectangle.Width = ENERGYBAR_WIDTH;
                rectangle.X = StaticGlobal.Screen.Area.Right - (rectangle.Width + HUD_PADDING);
                rectangle.Y = StaticGlobal.Screen.Area.Bottom - (rectangle.Height + HUD_PADDING);
                rectangle.Render(GraphicsDevice);

                // FamilyColor
                switch (Camera.SpectatingCell.AI.family)
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

                circleCell.Radius = 25;
                circleCell.Position = new Vector2(
                    StaticGlobal.Screen.Area.Right - (circleCell.Radius + ENERGYBAR_WIDTH) - 2 * HUD_PADDING,
                    StaticGlobal.Screen.Area.Bottom - circleCell.Radius - HUD_PADDING);
                circleCell.UpdateVertices();
                circleCell.Render(GraphicsDevice);

                // Draw Attributes
                float sum, factor;
                sum = Camera.SpectatingCell.Speed + Camera.SpectatingCell.Size + Camera.SpectatingCell.Detectionrange;
                factor = sum / (float)RELATIONBAR_HEIGHT;

                rectangle.Color = Color.LightGreen;
                rectangle.Width = RELATIONBAR_WIDTH;
                rectangle.Height = (int)(Camera.SpectatingCell.Speed / factor);
                rectangle.X = StaticGlobal.Screen.Area.Right - HUD_PADDING - 180;
                rectangle.Y = StaticGlobal.Screen.Area.Bottom - RELATIONBAR_HEIGHT - HUD_PADDING;
                rectangle.Render(GraphicsDevice);

                rectangle.Color = Color.LightCoral;
                rectangle.Y += rectangle.Height;
                rectangle.Height = (int)(Camera.SpectatingCell.Size / factor);
                rectangle.Render(GraphicsDevice);

                rectangle.Color = Color.LightBlue;
                rectangle.Y += rectangle.Height;
                rectangle.Height = (int)(Camera.SpectatingCell.Detectionrange / factor);
                rectangle.Render(GraphicsDevice);

                rectangle.Color = Color.LightCoral;
                rectangle.Height = (int)Camera.SpectatingCell.AI.lastIntresst.Speed;
                rectangle.X += rectangle.Width + 5;
                rectangle.Y = StaticGlobal.Screen.Area.Bottom - rectangle.Height - HUD_PADDING;
                rectangle.Render(GraphicsDevice);
                rectangle.Height = (int)Camera.SpectatingCell.AI.lastIntresst.Size;
                rectangle.X += rectangle.Width + 5;
                rectangle.Y = StaticGlobal.Screen.Area.Bottom - rectangle.Height - HUD_PADDING;
                rectangle.Render(GraphicsDevice);
                if (Camera.SpectatingCell.AI.lastIntresst.GetType() == typeof(Cell))
                {
                    rectangle.Height = (int)((Cell)Camera.SpectatingCell.AI.lastIntresst).Detectionrange;
                }
                else
                {
                    rectangle.Height = 0;
                }
                rectangle.X += rectangle.Width + 5;
                rectangle.Y = StaticGlobal.Screen.Area.Bottom - rectangle.Height - HUD_PADDING;
                rectangle.Render(GraphicsDevice);
            }
            // END OF Draw()
        }

        public static void UpdateFPS(GameTime gameTime)
        {
            var framerate = gameTime.ElapsedGameTime.TotalMilliseconds;

            fps_stat.Add((float)framerate);
            fps_stat.RemoveAt(0);
        }
        
        public static void DrawFPS(GraphicsDevice GraphicsDevice)
        {
            // Draw 30 fps and 15 fps marker
            rectangle.Color = Color.White;
            rectangle.Height = (int)Math.Round(MS_15_FPS);
            rectangle.Width = (fps_stat.Count - 2) * FPS_LINE_WIDTH + 1;
            rectangle.X = -1;
            rectangle.Y = StaticGlobal.Screen.Area.Bottom - rectangle.Height;
            rectangle.Render(GraphicsDevice);
            line.Color = Color.Black;
            line.SetLine(
                new Vector2(0, StaticGlobal.Screen.Area.Bottom - MS_30_FPS),
                new Vector2((fps_stat.Count - 2) * FPS_LINE_WIDTH, StaticGlobal.Screen.Area.Bottom - MS_30_FPS)
            , false);
            line.Render(GraphicsDevice);

            for (int i = 1; i < fps_stat.Count - 1; i++)
            {
                // If fps is worse than 30 FPS, lerp it between yellow & red
                if (fps_stat[i - 1] > MS_30_FPS)
                {
                    line.Color = Color.Lerp(Color.Yellow, Color.Red, fps_stat[i - 1] / MS_15_FPS);
                }
                // If fps is better than 30 FPS, lerp it between green & yellow
                else
                {
                    line.Color = Color.Lerp(Color.Green, Color.Yellow, (fps_stat[i - 1]) / MS_30_FPS);
                }

                // Draw lines
                for (int j = 0; j < FPS_LINE_WIDTH; j++)
                {
                    line.SetLine(
                        new Vector2(
                            (i - 1) * FPS_LINE_WIDTH + j,//((float)(i - 1) / fps_stat.Count) * StaticGlobal.Screen.Area.Width,
                            StaticGlobal.Screen.Area.Bottom - fps_stat[i - 1]
                        ),
                        new Vector2(
                            (i - 1) * FPS_LINE_WIDTH + j,//((float)(i) / fps_stat.Count) * StaticGlobal.Screen.Area.Width,
                            StaticGlobal.Screen.Area.Bottom //- (float)fps_stat[i]
                        ),
                        false);
                    line.Render(GraphicsDevice);
                }
            }
        }
    }
}