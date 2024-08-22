using System.Drawing;
using System.Runtime.CompilerServices;

namespace NetworkPuzzleSolver
{
    public partial class Form1 : Form
    {
        static Thread th;
        public Form1()
        {
            InitializeComponent();
        }

        void Solve()
        {
            for (int n = 0; n < 2; n++)
            {
                for (int i = 0; i < 10; i++)
                {
                    float cellSize = 47.8f;

                    Size size = new Size(5, 5);
                    Rectangle rect = new Rectangle(new Point(1403, 472), new Size(239, 239));
                    Bitmap bmp = TakeScreenshotRegion(rect);
                    if (SolveAndPlay(rect, size, bmp))
                    {
                        i = 0;
                        n = 0;
                        continue;
                    }

                    size = new Size(6, 6);
                    rect = new Rectangle(new Point(1379, 448), new Size(287, 287));
                    bmp = TakeScreenshotRegion(rect);
                    if (SolveAndPlay(rect, size, bmp))
                    {
                        i = 0;
                        n = 0;
                        continue;
                    }

                    size = new Size(7, 7);
                    rect = new Rectangle(new Point(1355, 438), new Size(335, 335));
                    bmp = TakeScreenshotRegion(rect);
                    if (SolveAndPlay(rect, size, bmp))
                    {
                        i = 0;
                        n = 0;
                        continue;
                    }

                    size = new Size(11, 11);
                    rect = new Rectangle(new Point(1263, 437), new Size(520, 520));
                    bmp = TakeScreenshotRegion(rect);
                    if (SolveAndPlay(rect, size, bmp))
                    {
                        i = 0;
                        n = 0;
                        continue;
                    }

                    size = new Size(12, 12);
                    bmp = TakeScreenshotRegion(rect);
                    if (SolveAndPlay(rect, size, bmp))
                    {
                        i = 0;
                        n = 0;
                        continue;
                    }
                    size = new Size(13, 13);
                    bmp = TakeScreenshotRegion(rect);
                    if (SolveAndPlay(rect, size, bmp))
                    {
                        i = 0;
                        n = 0;
                        continue;
                    }
                    Thread.Sleep(100);
                }

                Thread.Sleep(200);
                Player.SimulateMouseClick(1432, 602);
                Thread.Sleep(200);
            }

        }

        bool SolveAndPlay(Rectangle rect, Size size, Bitmap bitmap)
        {
            Console.WriteLine("\n­\n\nTrying " + size.ToString());
            if (!Player.FromBitmap(bitmap, size, out Board board))
            {
                return false;
            }
            Bitmap notSolvedImage = board.Draw();
            Console.WriteLine("Scanned Board" + board.ToString());

            board.box = OutputImageBox;
            bool solved = board.Solve();

            if (solved)
            {
                Console.WriteLine("Solved" + board.ToString());

                Bitmap solvedImage = board.Draw();
                OutputImageBox.Invoke(new Action(() =>
                {
                    InputImageBox.Image = bitmap;
                    OutputImageBox.Image = solvedImage;
                    //outLabel.Text = "Solved puzzle";
                }));

                Player.Play(board, rect);
                Thread.Sleep(100);
                Player.SimulateMouseClick(1346, 760);
                Thread.Sleep(400);
            }
            else
            {
                OutputImageBox.Invoke(new Action(() =>
                {
                    InputImageBox.Image = bitmap;
                    OutputImageBox.Image = notSolvedImage;
                    //outLabel.Text = "Solved puzzle";
                }));
                Console.WriteLine("Did not solve");
            }
            return solved;
        }

        static Bitmap TakeScreenshotRegion(Rectangle rect)
        {
            Point topLeft = rect.Location;
            int width = rect.Width;
            int height = rect.Height;
            Bitmap bitmap = new Bitmap(width, height);
            // Use the Graphics object to copy the pixel from the screen
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen((int)topLeft.X, (int)topLeft.Y, 0, 0, rect.Size);
            }
            return bitmap;
        }

        private void SolveButton_Click(object sender, EventArgs e)
        {
            if (th == null || th.ThreadState != ThreadState.Running)
            {
                th = new Thread(Solve);
                th.Start();
                th.Name = "Solver";
            }
        }
    }
}
