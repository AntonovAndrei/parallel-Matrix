using System;
using System.Threading;

namespace ParrallelMatrix
{
    class Program
    {
        private static int _row1 = 2000;
        private static int _column1 = _row1;
        private static int[,] _matrix1 = GetFiledInMatrix(_row1, _column1);
            
        private static int _row2 = _row1;
        private static int _column2 = _row1;
        private static int[,] _matrix2 = GetFiledInMatrix(_row2, _column2);

        private static int _row3 = _row1;
        private static int _column3 = _column2;
        private static int[,] _parallelMultiplyMatrix = new int[_row3, _column3];
        
        private static int _row4 = _row1;
        private static int _column4 = _column2;
        private static int[,] _multiplyMatrix = new int[_row4, _column4];

        private static int _rowPR = _row1;
        private static int _columnPR = _column2;
        private static int[,] _rightMultiplyMatrix = new int[_rowPR, _columnPR];
        private static int allThreadsRight = Environment.ProcessorCount;
        private static int rowsCountInThread = _rowPR / (allThreadsRight*2);
        
        //private static object RowWriteLockObject = new object();
        
        static void Main(string[] args)
        {
            Console.WriteLine($"Количество потоков процессора {allThreadsRight}");
            Console.WriteLine("Программа начала свою работу");
            
            
            _matrix1 = GetFiledInMatrix(_row1, _column1);
            Console.WriteLine($"Матрица1({_row1}x{_column1}) заполнена");
            
            _matrix2 = GetFiledInMatrix(_row2, _column2);
            Console.WriteLine($"Матрица2({_row2}x{_column2}) заполнена");
            Console.WriteLine();

            Console.WriteLine($"Обычное умножение вычисляется");
            DateTime begin=DateTime.Now;
            Multiply();
            DateTime end = DateTime.Now;
            TimeSpan rez = end - begin;
            Console.WriteLine($"Обычное умножение выполнилось за {rez.Minutes}:{rez.Seconds}:{rez.Milliseconds}");
            Console.WriteLine();
            
            Console.WriteLine($"Параллельное умножение вычисляется");
            begin=DateTime.Now;
            ParrallelMuliply();
            end = DateTime.Now;
            rez = end - begin;
            Console.WriteLine($"Параллельное умножение выполнилось за {rez.Minutes}:{rez.Seconds}:{rez.Milliseconds}");

            if (rowsCountInThread > 0)
            {
                Console.WriteLine($"Правильное параллельное умножение вычисляется");
                begin = DateTime.Now;
                RightParallelMultiply();
                end = DateTime.Now;
                rez = end - begin;
                Console.WriteLine(
                    $"Правильное параллельное умножение выполнилось за {rez.Minutes}:{rez.Seconds}:{rez.Milliseconds}");
            }
            
            
            //Код для просмотра вычислений матрицы
            /*Console.WriteLine("\nMatrix1:");
            PrintMatrix(_matrix1);
            
            Console.WriteLine("\nMatrix2:");
            PrintMatrix(_matrix2);

            ParrallelMuliply();
            Console.WriteLine("\nParallel matrix multiplication:");
            PrintMatrix(_parallelMultiplyMatrix);
            
            Multiply();
            Console.WriteLine("\nMatrix multiplication:");
            PrintMatrix(_multiplyMatrix);*/
        }

        private static void RightParallelMultiply()
        {
            //количество потоков два раза больше количества потоков процессора
            int numOfThread = allThreadsRight * 2;
            Thread[] matrixThreads = new Thread[numOfThread];

            for (int i = 0; i < numOfThread; i++)
            {
                matrixThreads[i] = new Thread(RightParallelRowsCalculation);
            }

            for (int i = 0; i < numOfThread; i++)
            {
                matrixThreads[i].Start(i);
            }

            foreach (var t in matrixThreads)
            {
                t.Join();
            }
        }
        
        private static void RightParallelRowsCalculation(object obj)
        {
            int threadNumber = (int) obj;
            for (int k = threadNumber * rowsCountInThread; k < (threadNumber + 1) * rowsCountInThread; k++)
            {
                for (int i = 0; i < _column2; i++)
                {
                    for (int j = 0; j < _column1; j++)
                    {
                        _parallelMultiplyMatrix[k, i] += _matrix1[k, j] * _matrix2[j, i];
                    }
                }
            }
        }

        private static void Multiply()
        {
            for (var i = 0; i < _row1; i++)
            {
                for (var j = 0; j < _column2; j++)
                {
                    for (var k = 0; k < _column1; k++)
                    {
                        _multiplyMatrix[i, j] += _matrix1[i, k] * _matrix2[k, j];
                    }
                }
            }
        }

        private static void ParrallelMuliply()
        {
            //количество потоков равно количеству строк исходной матрицы(то есть за вычисления каждой строки отвечает отдельный поток)
            int numOfThread = _row3;
            Thread[] matrixThreads = new Thread[numOfThread];

            for (int i = 0; i < numOfThread; i++)
            {
                matrixThreads[i] = new Thread(ParallelRowCalculation);
            }

            for (int i = 0; i < numOfThread; i++)
            {
                matrixThreads[i].Start(i);
            }

            foreach (var t in matrixThreads)
            {
                t.Join();
            }
        }
        
        private static void ParallelRowCalculation(object objRow)
        {
            int row = (int) objRow;
            for (int i = 0; i < _column2; i++)
            {
                for (int j = 0; j < _column1; j++)
                { 
                    //если тут добавить локер, то параллельное выполнение будет в любом случае медленнее.
                    _parallelMultiplyMatrix[row, i] += _matrix1[row, j] * _matrix2[j, i];
                }
            }
        }
        
        private static void PrintMatrix(int[,] matrix)
        {
            for (int i = 0; i < _row3; i++)
            {
                for (int j = 0; j < _column3; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        
        public static int[,] GetFiledInMatrix(int row, int column)
        {
            Random random = new Random();
            int[,] matrix = new int[row,column];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    matrix[i, j] = random.Next(0, 10);
                }
            }

            return matrix;
        }
    }
}