using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkPuzzleSolver
{
    public class Player
    {
        public static bool FromBitmap(Bitmap bmp, Size size, out Board b) {

            float cellW = (float)bmp.Width / (float)size.Width;
            float cellH = (float)bmp.Height / (float)size.Height;
            float edge = 0.4f; // 0.1 to .5
            Cell[] cells = new Cell [size.Width * size.Height];

            for (int i = 0; i < size.Width * size.Height; i++)
            {
                int row = i / size.Width;
                int col = i % size.Height;
                int centerX = (int)(col * cellW + cellW / 2);
                int centerY = (int)(row * cellH + cellH / 2);

                // North
                for (int n = centerX - (int)(cellW * edge); n < centerX + (int)(cellW * edge); n++)
                {
                    if (bmp.GetPixel(n, centerY - (int)(cellH * edge)).R < 200) //Pixel is black
                    {
                        cells[i].CanN = true;
                    }
                    bmp.SetPixel(n, centerY - (int)(cellH * edge), Color.Red);
                }

                // South
                for (int n = centerX - (int)(cellW * edge); n < centerX + (int)(cellW * edge); n++)
                {
                    if (bmp.GetPixel(n, centerY + (int)(cellH * edge)).R < 200) //Pixel is black
                    {
                        cells[i].CanS = true;
                    }
                    bmp.SetPixel(n, centerY + (int)(cellH * edge), Color.Red);
                }

                // East
                for (int n = centerY - (int)(cellH * edge); n < centerY + (int)(cellH * edge); n++)
                {
                    if (bmp.GetPixel(centerX + (int)(cellW * edge), n).R < 200) //Pixel is black
                    {
                        cells[i].CanE = true;
                    }
                    bmp.SetPixel(centerX + (int)(cellW * edge), n, Color.Red);
                }

                // Weast
                for (int n = centerY - (int)(cellH * edge); n < centerY + (int)(cellH * edge); n++)
                {
                    if (bmp.GetPixel(centerX - (int)(cellW * edge), n).R < 200) //Pixel is Dark
                    {
                        cells[i].CanW = true;
                    }
                    bmp.SetPixel(centerX - (int)(cellW * edge), n, Color.Red);
                }
            }
            b = new(cells, size);
            return true;
        }

        public static void Play(Board board, Rectangle rect)
        {
            Size size = board.size;
            for (int i = 0; i < size.Width * size.Height; i++)
            {
                int row = i / size.Width;
                int col = i % size.Height;

                int cellW = (int)rect.Size.Width / size.Width;
                int cellH = (int)rect.Size.Height / size.Height;

                int x = (int)rect.Location.X + cellW * col + cellW / 2;
                int y = (int)rect.Location.Y + cellH * row + cellH / 2;

                for (int n = 0; n < board.cells[i].RotateCount; n++)
                {
                    SimulateMouseClick(x, y);

                    Thread.Sleep(5);
                }
            }
        }




        // Constants for mouse event flags
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        public static void SimulateMouseClick(int x, int y)
        {
            // Move the cursor to the specified coordinates
            SetCursorPos(x, y);

            // Simulate mouse click
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);
    }
}
