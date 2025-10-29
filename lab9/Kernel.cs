using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab9
{
    public struct Kernel
    {
        private double[][] cells;
        private double summ;
        public double[][] Cells { get { return cells; } set { cells = value; } }
        public double Summ { get { return summ; } set { summ = value; } }
        public Kernel(int columns, int rows)
        {
            summ = 1;
            cells = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                cells[i] = new double[columns];
                for (int j = 0; j < columns; j++)
                {
                    cells[i][j] = 0;
                }
            }
        }
    }
}
