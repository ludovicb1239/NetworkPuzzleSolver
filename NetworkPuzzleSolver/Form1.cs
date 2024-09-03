using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace NetworkPuzzleSolver
{
    public partial class Form1 : Form
    {
        static Thread th;
        bool savePuzzle = false;
        public Form1()
        {
            InitializeComponent();

            string folderPath = @"puzzles"; // Replace with your folder path
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                if (!file.EndsWith(".json"))
                    continue;

                Stopwatch profiler = new();
                for (int i = 0;  i < 100; i++)
                {
                    Board board = Board.Load(file);

                    profiler.Start();

                    bool solved = board.Solve();

                    profiler.Stop();

                    if (!solved)
                        throw new Exception("Could not solve");
                }
                double elapsedMicroseconds = (double)profiler.ElapsedTicks / Stopwatch.Frequency * 1_000_000;
                Console.WriteLine($"{file}\t\t Solve Time: {(int)(elapsedMicroseconds / 100f)} µs");
            }
        }
        private static int ExtractNumberFromFileName(string fileName)
        {
            // Regular expression to find the number in the file name
            Match match = Regex.Match(fileName, @"\d+");
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            return 0; // If no number is found, treat it as 0
        }

        void Solve()
        {
            int i = 0;
            while (i < 2)
            {
                float cellSize = 47.8f;
                i++;

                Size size = new Size(5, 5);
                Rectangle rect = new Rectangle(new Point(1403, 472), new Size(239, 239));
                Bitmap bmp = TakeScreenshotRegion(rect);
                //if (SolveAndPlay(rect, size, bmp))
                //{
                //    i = 0;
                //    continue;
                //}
                //
                //size = new Size(6, 6);
                //rect = new Rectangle(new Point(1379, 448), new Size(287, 287));
                //bmp = TakeScreenshotRegion(rect);
                //if (SolveAndPlay(rect, size, bmp))
                //{
                //    i = 0;
                //    continue;
                //}
                //
                //size = new Size(7, 7);
                //rect = new Rectangle(new Point(1355, 438), new Size(335, 335));
                //bmp = TakeScreenshotRegion(rect);
                //if (SolveAndPlay(rect, size, bmp))
                //{
                //    i = 0;
                //    continue;
                //}
                //
                //size = new Size(11, 11);
                //rect = new Rectangle(new Point(1263, 437), new Size(520, 520));
                //bmp = TakeScreenshotRegion(rect);
                //if (SolveAndPlay(rect, size, bmp))
                //{
                //    i = 0;
                //    continue;
                //}
                //
                //size = new Size(12, 12);
                //bmp = TakeScreenshotRegion(rect);
                //if (SolveAndPlay(rect, size, bmp))
                //{
                //    i = 0;
                //    continue;
                //}
                //size = new Size(13, 13);
                //bmp = TakeScreenshotRegion(rect);
                //if (SolveAndPlay(rect, size, bmp))
                //{
                //    i = 0;
                //    continue;
                //}
                size = new Size(25, 25);
                rect = new Rectangle(new Point(620, 289), new Size(702, 702));
                bmp = TakeScreenshotRegion(rect);
                if (SolveAndPlay(rect, size, bmp))
                {
                    i = 0;
                    continue;
                }
                Thread.Sleep(400);
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

            Stopwatch profiler = new();
            profiler.Start();

            Cell[] beforeCells = savePuzzle ? board.cells.ToArray() : null;

            board.box = OutputImageBox;
            bool solved = board.Solve();

            profiler.Stop();
            double elapsedMicroseconds = (double)profiler.ElapsedTicks / Stopwatch.Frequency * 1_000_000;
            Console.WriteLine($"Solve Time: {elapsedMicroseconds:F3} µs");

            if (solved)
            {
                if (savePuzzle)
                {
                    Board boardUnsolved = new(beforeCells, size);
                    boardUnsolved.Save($"puzzles\\{size.Width}x{size.Height}_{(int)elapsedMicroseconds}.json");
                }

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
            if (th == null || th.ThreadState != System.Threading.ThreadState.Running)
            {
                th = new Thread(Solve);
                th.Start();
                th.Name = "Solver";
            }
        }
    }
}
