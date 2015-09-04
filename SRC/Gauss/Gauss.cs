using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace Gauss
{
    public static class Gauss
    {
        public class NoSolutionException : ApplicationException
        {
            public NoSolutionException()
            {
            }
        }

        public class WrongDataFormatException : ApplicationException
        {
            public WrongDataFormatException()
            {
            }
        }

        private static void swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        private class GeneralElement
        {
            private int _row; //row index [0, ..., n)
            private int _col; // column index [0, ..., n)
            private double _val = 0; //value
            private bool _isSet = false;
            public void setPoint(int row, int col, double val)
            {
                this._col = col;
                this._row = row;
                this._val = val;
                this._isSet = true;
            }
            public int row { get { return this._row; } }
            public int col { get { return this._col; } }
            public double val { get { return this._val; } }
            public double absVal { get { return Math.Abs(this.val); } }
            public bool isSet { get { return this._isSet; } }
            public override string ToString()
            {
                return this._isSet ? String.Format("General element: a[{0},{1}] = {2:0.0}", this.row + 1, this.col + 1, this.val) : 
                    "General element is not found";
            }
        }

        public static double[,] ReadCSV(string fileName)
        {
            try
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(fileName)))
                {
                    double[,] result = null;
                    int r = 0;
                    NumberFormatInfo provider = new NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";
                    provider.NumberGroupSeparator = "";
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        double[] row = line.Split(';').Select(x => Convert.ToDouble(x, provider)).ToArray();

                        if (result == null)
                            result = new double[row.Length - 1, row.Length];
                        if (row.Length != result.GetLength(1))
                            throw new WrongDataFormatException();
                        for (int c = 0; c < row.Length; ++c)
                            result[r, c] = row[c];

                        ++r;
                    }
                    if (r != result.GetLength(0))
                        throw new WrongDataFormatException();
                    return result;
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                WrongDataFormatException argEx = new WrongDataFormatException();
                throw argEx;
            }
            catch (System.FormatException)
            {
                WrongDataFormatException argEx = new WrongDataFormatException();
                throw argEx;
            }
        }

        private static void printEqs(double[,] data, int[] varOrder, string caption)
        {
            Console.WriteLine(caption);
            for (int i = 0; i < data.GetLength(0); ++i)
            {
                for (int j = 0; j < data.GetLength(1); ++j)
                {
                    if (j == 0)
                        Console.Write("{0,6:0.0} * x{1} ", data[i, j], varOrder[j] + 1);
                    else if (j == data.GetLength(0))
                        Console.Write("= {0,6:0.0}", data[i, j]);
                    else
                        Console.Write("+ {0,6:0.0} * x{1} ", data[i, j], varOrder[j] + 1);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static double[] Solve(double[,] data)
        {
            data = (double[,])data.Clone(); //The method mustn't change an argument

            // varOrder - array for variables' order
            int[] varOrder = new int[data.GetLength(1) - 1];
            for (int i = 0; i < varOrder.Length; ++i)
                varOrder[i] = i;
            
            // Print equations
            printEqs(data, varOrder, "Initial system of linear equations:");
            Console.WriteLine("Gaussian elimination with the general element in rows\n");
            
            // Main loop
            for (int i = 0; i < data.GetLength(0); ++i)
            {
                Console.WriteLine("Step #{0}", i + 1);

                //Looking for the general element
                GeneralElement generalElement = new GeneralElement();
                for (int j = i; j < data.GetLength(1) - 1; ++j) 
                    if (Math.Abs(data[i, j]) - generalElement.absVal > 1e-10)
                        generalElement.setPoint(i, j, data[i, j]);

                Console.WriteLine(generalElement);
                if (!generalElement.isSet) // Its mean every a-coefficient in row = 0
                    throw new NoSolutionException();

                // Row reduction
                for (int k = 0; k < data.GetLength(0); ++k)
                    if (k != generalElement.row)
                    {
                        double m = -data[k, generalElement.col] / generalElement.val;
                        for (int j = 0; j < data.GetLength(1); ++j)
                            data[k, j] += data[generalElement.row, j] * m;
                    }

                // Print intermediate results
                printEqs(data, varOrder, "Row reduction:");

                // Change variables' order
                if (i != generalElement.col)
                {
                    swap<int>(ref varOrder[generalElement.col], ref varOrder[i]);
                    for (int k = 0; k < data.GetLength(0); ++k)
                        swap<double>(ref data[k, generalElement.col], ref data[k, i]);

                    printEqs(data, varOrder, "Changing variables' order:");
                }
            }
            // Getting variables' values
            double[] answers = new double[data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); ++i)
                answers[varOrder[i]] = data[i, data.GetLength(1) - 1] / data[i, i];
            return answers;
        }
    }
}
