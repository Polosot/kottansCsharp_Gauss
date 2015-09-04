using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gauss
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {if (args.Length > 0)
                {
                    double[,] data = Gauss.ReadCSV(args[0]);

                    double[] x = Gauss.Solve(data);

                    Console.WriteLine("Solution is found.\nVector of roots:");
                    for (int i = 0; i < x.Length; ++i)
                        Console.WriteLine("x{0} = {1:0.000000}", i + 1, x[i]);
                    Console.WriteLine();

                    Console.WriteLine("Vector of errors (E = B-A*X):");
                    for (int i = 0; i < data.GetLength(0); ++i)
                    {
                        double e = data[i, data.GetLength(1) - 1];
                        for (int j = 0; j < data.GetLength(1) - 1; ++j)
                            e -= data[i, j] * x[j];
                        Console.WriteLine("e{0} = {1:E}", i + 1, e);
                    }
                }
                else
                    Console.WriteLine("File usage:\nGauss.exe file\nwhere file - CSV-file with ';' delimiter, '.' decimal symbol, without header, which consist augmented matrix (A|B):\n* n rows\n* n+1 columns\n* the last column represents B-vector");
            }
            catch(Gauss.NoSolutionException)
            {
                Console.WriteLine("There is no unique solution");
            }
            catch(Gauss.WrongDataFormatException)
            {
                Console.WriteLine("Wrong file format");
                Console.WriteLine("File usage:\nGauss.exe file\nwhere file - CSV-file with ';' delimiter, '.' decimal symbol, without header, which consist augmented matrix (A|B):\n* n rows\n* n+1 columns\n* the last column represents B-vector");
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("File {0} not found", args[0]);
            }
        }
    }
}
