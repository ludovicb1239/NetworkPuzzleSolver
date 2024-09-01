using System.Diagnostics;
using System.Drawing;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NetworkPuzzleSolver
{
    public partial class Form1 : Form
    {
        static Thread th;
        static List<Player> players;
        static List<Thread> threads;
        public Form1()
        {
            InitializeComponent();

            string folderPath = @"puzzles"; // Replace with your folder path
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                Stopwatch profiler = new();
                for (int i = 0; i < 10; i++)
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

            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);


            threads = new List<Thread>();
            players = new();

            for (int i = 0; i < 4; i++)
            {
                Player p = new Player
                {
                    id = i
                };

                Thread thread = new Thread(new ThreadStart(p.Start));
                threads.Add(thread);

                players.Add(p);
                thread.Start();
            }

            // Wait for all threads to finish
            foreach (Thread thread in threads)
            {
                thread.Join();
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

        private void SolveButton_Click(object sender, EventArgs e)
        {
            threads = new();
            foreach (Player p in players)
            {
                Thread thread = new Thread(new ThreadStart(p.Run));
                threads.Add(thread);
                thread.Start();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        private void CloseBtt_Click(object sender, EventArgs e)
        {
            Stop();
        }
        void Stop()
        {
            // Wait for all threads to finish
            foreach (Thread thread in threads)
            {
                thread.Interrupt();
                thread.Join();
            }
            foreach (Player p in players)
                p.Destroy();
        }
    }
}
