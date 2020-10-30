using System;
using System.Collections.Generic;
using System.Text;

namespace TH1
{
    class Matrix
    {
        
        static void Input(ref int[,] arr ,int row,int column)
        {
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    arr[i, j] = Convert.ToInt32(Console.ReadLine());
        }
        static void Print(int [,] arr,int row,int column)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                    Console.Write(arr[i, j]);
                Console.WriteLine();
            }
        }
        static int MinValue(int [,] arr)
        {
            int min = arr[0, 0];
            foreach(int val in arr)
                    if (val < min) min = val;
            return min;
        }
        static int MaxValue(int [,] arr)
        {
            int max = arr[0, 0];
            foreach(int val in arr)
                    if (val > max) max = val;
            return max;
        }
        static int FindRowMax(int[,]arr,int m,int n)
        {
            int indexOfRow = 0;
            int maxValue = 0;
            for(int i=0;i<m;i++)
            {
                int totalOfRow = 0;
                for (int j = 0; j < m; j++)
                    totalOfRow += arr[i, j];
                if (totalOfRow > maxValue)
                {
                    maxValue = totalOfRow;
                    indexOfRow = i;
                }
            }
            return indexOfRow;
        }
        static int TotalNonPrimeNumber(int[,] arr)
        {
            int total = 0;
            foreach(int temp in arr)
            {
                if (Cau1.isPrime(temp) == false) total += temp;
            }
            return total;
        }
        static void DeleteRow(ref int [,] arr,ref int row, int column,int index)
        {
            for (int i = index; i < row - 1; i++)
                for (int j = 0; j < column; j++)
                    arr[i, j] = arr[i + 1, j];
            row--;
        }
        static int FindCoumnHasMaxNumber(int[,] arr, int row, int column)
        {
            int max = arr[0, 0];
            int maxColumn=0;
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    if (arr[i, j] > max)
                    {
                        max = arr[i, j];
                        maxColumn = j;
                    }
            return maxColumn;
        }
        static void DeleteColumm(ref int [,] arr,int row, ref int column)
        {
            for (int i = 0; i < row; i++)
                for (int j = FindCoumnHasMaxNumber(arr,row,column); j < column-1; j++)
                    arr[i, j] = arr[i, j+1];
            column--;
        }
        
        public static void call()
        {
            Console.WriteLine("===Cau 6===");   

            int row, column;

            Console.WriteLine("Number of row: ");
            row = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Number of column: ");
            column = Convert.ToInt32(Console.ReadLine());

            int[,] arr = new int[row, column];

            Input(ref arr, row, column);
            Print(arr, row, column);

            Console.WriteLine("Min value of the matrix= " + MinValue(arr));
            Console.WriteLine("Max value of the matrix= " + MaxValue(arr));
            Console.WriteLine("The row has max total: " + FindRowMax(arr,row,column));
            Console.WriteLine("Total of non-prime numbers= " + TotalNonPrimeNumber(arr));

            Console.WriteLine("Select the row you want to delete from 0 to "+(row-1));
            int indexRow = Convert.ToInt32(Console.ReadLine());
            DeleteRow(ref arr, ref row, column, indexRow);
            Print(arr, row, column);

            Console.WriteLine("Matrix after delete the row has max number");
            DeleteColumm(ref arr, row, ref column);
            Print(arr, row, column);


        }
        
    }
}
