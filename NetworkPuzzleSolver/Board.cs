using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkPuzzleSolver
{
    public struct Cell
    {
        public bool CanN;
        public bool CanE;
        public bool CanS;
        public bool CanW;
        public int RotateCount;
        public Cell(bool CanN,  bool CanE, bool CanS, bool CanW)
        {
            this.CanN = CanN;
            this.CanE = CanE;
            this.CanS = CanS;
            this.CanW = CanW;
            RotateCount = 0;
        }
        public void Rotate()
        {
            bool t = CanW;
            CanW = CanS;
            CanS = CanE;
            CanE = CanN;
            CanN = t;
            RotateCount++;
            if (RotateCount == 4)
                RotateCount = 0;
        }
        public void Reset()
        {
            while (RotateCount > 0)
                Rotate();
        }
    }
    public class Board
    {
        public PictureBox box;
        public readonly Cell[] cells;
        public readonly Size size;
        readonly int[] patternCells;
        public readonly int[] maxRotationCells;

        private HashSet<string> visitedStates = new();
        public Board(Cell[] cells, Size size)
        {
            if (cells.Length != size.Width * size.Height)
            {
                throw new Exception("Grid is not the right lenght");
            }
            this.cells = cells;
            this.size = size;
            this.patternCells = GenerateZigzagPath(size);
            maxRotationCells = new int[cells.Length];
            for (int i = 0; i <  patternCells.Length; i++)
            {
                //Optimises for straights 
                int s1 = (cells[i].CanN ? 1 : 0) + (cells[i].CanE ? 2 : 0);
                int s2 = (cells[i].CanS ? 1 : 0) + (cells[i].CanW ? 2 : 0);
                maxRotationCells[i] = s1 == s2 ? 2 : 4;
            }
        }

        public bool Solve()
        {
            return CheckConnection(0);
        }
        bool CheckConnection(int i)
        {
            if (i == size.Width * size.Height)
                return Done();
            int pos = this.patternCells[i];

            //Try with empty cell
            for (int n = 0; n < maxRotationCells[pos]; n++)
            {
                if (IsValid(pos))
                {
                    string currentState = GetBoardState(i);
                    if (!visitedStates.Contains(currentState))
                    {
                        visitedStates.Add(currentState);
                        if (CheckConnection(i + 1))
                            return true;
                    }
                }
                cells[pos].Rotate();
            }
            return false;
        }
        static int[] GenerateZigzagPath(Size size)
        {
            List<int> path = new();
            List<(int, int)> starts = new();

            for (int i = 0; i < size.Width; i++)
            {
                starts.Add((i, 0));
            }
            for (int j = 1; j < size.Height; j++)
            {
                starts.Add((size.Width-1, j));
            }
            foreach (var pos1 in starts)
            {
                var pos = pos1;
                while (pos.Item1 >= 0 && pos.Item2 < size.Height)
                {
                    path.Add(pos.Item1 + pos.Item2*size.Width);
                    pos.Item1 -= 1;
                    pos.Item2 += 1;
                }
            }

            return path.ToArray();
        }
        bool IsValid(int i) //Only checks if it connects W and N
        {
            if (i % size.Width != 0)
            {
                if (cells[i - 1].CanE != cells[i].CanW) 
                    return false;
            }
            else
            {
                if (cells[i].CanW)
                    return false;
            }
            if (i >= size.Width)
            {
                if (cells[i - size.Width].CanS != cells[i].CanN)
                    return false;
            }
            else
            {
                if (cells[i].CanN)
                    return false;
            }
            if ((i + 1) % size.Width == 0)
            {
                if (cells[i].CanE)
                    return false;
            }
            if (i >= cells.Length - size.Width)
            {
                if (cells[i].CanS)
                    return false;
            }
            return true;
        }
        bool Done()
        {
            bool[] grid = new bool[cells.Length];

            FloodFill(grid, 0, 0);

            for (int i = 0; i < cells.Length; i++)
            {
                if (!grid[i])
                {
                    return false;
                }
            }
            return true;
        }
        private void FloodFill(bool[] grid, int row, int col)
        {
            int index = row * size.Width + col;
            grid[index] = true;
            Cell cell = cells[index];

            // Check Right
            if (col < size.Width - 1 && !grid[index + 1] && cell.CanE)
            {
                FloodFill(grid, row, col + 1);
            }

            // Check Down
            if (row < size.Height - 1 && !grid[index + size.Width] && cell.CanS)
            {
                FloodFill(grid, row + 1, col);
            }

            // Check Left
            if (col > 0 && !grid[index - 1] && cell.CanW)
            {
                FloodFill(grid, row, col - 1);
            }

            // Check Up
            if (row > 0 && !grid[index - size.Width] && cell.CanN)
            {
                FloodFill(grid, row - 1, col);
            }
        }
        public Bitmap Draw()
        {
            int cellSize = 40;
            Size imageSize = new Size(cellSize * size.Width, cellSize * size.Height);
            Bitmap bitmap = new Bitmap(imageSize.Width, imageSize.Height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Set background color to white
                graphics.Clear(Color.White);

                Pen linePen = new Pen(Color.RebeccaPurple, 5);

                for (int i = 0; i < cells.Length; i++)
                {
                    int row = i / size.Width;
                    int col = i % size.Width;
                    int centerX = (col * cellSize) + cellSize / 2;
                    int centerY = (row * cellSize) + cellSize / 2;
                    if (cells[i].CanN)
                        graphics.DrawLine(linePen, centerX, centerY, centerX, centerY - cellSize/2);
                    if (cells[i].CanS)
                        graphics.DrawLine(linePen, centerX, centerY, centerX, centerY + cellSize / 2);
                    if (cells[i].CanE)
                        graphics.DrawLine(linePen, centerX, centerY, centerX + cellSize / 2, centerY);
                    if (cells[i].CanW)
                        graphics.DrawLine(linePen, centerX, centerY, centerX - cellSize / 2, centerY);
                }
                // Draw the grid
                Pen gridPen = new Pen(Color.Gray, 2);

                // Draw the grid lines
                for (int i = 0; i <= size.Width; i++)
                {
                    graphics.DrawLine(gridPen, i * cellSize, 0, i * cellSize, imageSize.Height);
                }
                for (int i = 0; i <= size.Height; i++)
                {
                    graphics.DrawLine(gridPen, 0, i * cellSize, imageSize.Width, i * cellSize);
                }

            }
            return bitmap;
        }
        private string GetBoardState(int i)
        {
            StringBuilder state = new();
            for(int n = 0; n <= i; n++)
            {
                int pos = patternCells[n];
                state.Append(cells[pos].RotateCount); // or any other properties that represent the state
            }
            return state.ToString();
        }
    }
}
