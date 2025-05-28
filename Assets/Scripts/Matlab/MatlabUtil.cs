using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// c# version of different matlab function, methods are as similar as possible to matlab for an easy translation process
/// </summary>
public class MatlabUtil
{

    public static void Log(object message, bool error = false)
    {
        if (error)
            Debug.LogError(message);
        else
            Debug.Log(message);
    }

    public static void Log(MatlabMatrix matrix, bool error = false)
    {
        for (int i = 0; i < matrix.Size(0); i++)
        {
            string combinedMessage = "";

            for (int j = 0; j < matrix.Size(1); j++)
            {
                combinedMessage += j + ": " + matrix[i, j] + " | ";
            }
            Log(i + ": " + combinedMessage, error);
        }
    }

    /// <summary>
    /// replacement to: readmatrix('...');
    /// </summary>
    public static MatlabMatrix ReadMatrix(string path)
    {
        StringReader reader = new(path);
        
        List<string> lines = new();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            lines.Add(line);
        }

        MatlabMatrix result = MatlabMatrix.Create(lines.Count, lines[0].Split(',').Length);
        for (int i = 0; i < lines.Count; i++)
        {
            string[] values = lines[i].Split(",");
            for (int j = 0; j < values.Length; j++)
            {
                result[i, j] = double.Parse(values[j], CultureInfo.InvariantCulture);
            }
        }

        reader.Close();

        return result;
    }

    /// <summary>
    /// replacement to: writematrix('...');
    /// </summary>
    public static void WriteMatrix(string path, MatlabMatrix matrix)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);

        using StreamWriter writer = new(path, false, Encoding.UTF8);

        for (int i = 0; i < matrixSize0; i++)
        {
            StringBuilder line = new();

            for (int j = 0; j < matrixSize1; j++)
            {
                if (j > 0)
                    line.Append(',');

                line.Append(matrix[i, j].ToString(CultureInfo.InvariantCulture));
            }

            writer.WriteLine(line.ToString());
        }
    }

    public static int Size(MatlabMatrix matrix, int index)
    {
        return matrix.Size(index);
    }

    public static int Length(MatlabMatrix matrix)
    {
        return matrix.Size(1);
    }

    private static MatlabMatrix Fill(double start, int size1, double stepSize, double? startValue)
    {
        MatlabMatrix result = MatlabMatrix.Create(1, size1);

        if (startValue == null)
        {
            for (int i = 0; i < size1; i++)
            {
                result[i] = start + stepSize * i;
            }
        }
        else
        {
            if (startValue != 0)
            {
                for (int i = 0; i < size1; i++)
                {
                    result[i] = (double)startValue;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// replacement to: x = 0:T:21;
    /// </summary>
    /// <param name="start">inclusive</param>
    /// <param name="end">inclusive (if possible)</param>
    public static MatlabMatrix Fill(double start, double stepSize, double end)
    {
        int arraySize = (int)Math.Floor((end - start) / stepSize) + 1;
        return Fill(start, arraySize, stepSize, null);
    }

    /// <summary>
    /// replacement to: x = 1:24
    /// </summary>
    /// <param name="start">inclusive</param>
    /// <param name="end">inclusive (if possible)</param>
    public static MatlabMatrix Fill(double start, double end)
    {
        return Fill(start, 1, end);
    }

    public static MatlabMatrix Zeros(int size0, int size1)
    {
        return MatlabMatrix.Create(size0, size1);
    }

    public static MatlabMatrix Ones(int size0)
    {
        return Ones(size0, size0);
    }

    public static MatlabMatrix Ones(int size0, int size1)
    {
        return new MatlabMatrix(Matrix<double>.Build.Dense(size0, size1, 1));
    }

    /// <summary>
    /// the sum of all matrix values
    /// </summary>
    public static double Sum(MatlabMatrix matrix)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);

        double result = 0;

        for (int i = 0; i < matrixSize0; i++)
        {
            for (int j = 0; j < matrixSize1; j++)
            {
                result += matrix[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// the sum of all matrix values
    /// </summary>
    public static MatlabMatrix SumMatrix(MatlabMatrix matrix)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);

        MatlabMatrix result = MatlabMatrix.Create(1, matrixSize1);
        for (int i = 0; i < matrixSize1; i++)
        {
            double colSum = 0;
            for (int j = 0; j < matrixSize0; j++)
            {
                colSum += matrix[j, i];
            }
            result[0, i] = colSum;
        }
        return result;
    }

    /// <summary>
    /// sum a matrix but each true predicate get a 1
    /// replacement to: sum(sum(A ~= 0))
    /// </summary>
    /// <param name="predicate">A function to check each source item for a condition</param>
    public static int SumWithOnes(MatlabMatrix matrix, Func<double, bool> predicate)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);
        int result = 0;

        for (int i = 0; i < matrixSize0; i++)
        {
            for (int j = 0; j < matrixSize1; j++)
            {
                result += predicate(matrix[i, j]) ? 1 : 0;
            }
        }

        return result;
    }

    /// <summary>
    /// replacement to: A\B
    /// </summary>
    public static MatlabMatrix MlDivide(MatlabMatrix matrix1, MatlabMatrix matrix2)
    {
        return new MatlabMatrix(matrix1.Values.Solve(matrix2.Values));
    }

    /// <summary>
    /// replacement to: A.^B
    /// </summary>
    public static MatlabMatrix Power(MatlabMatrix matrix, double power)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);
        MatlabMatrix result = new(Matrix<double>.Build.Dense(matrixSize0, matrixSize1, (i, j) => Math.Pow(matrix[i, j], power)));
        
        return result;
    }

    /// <summary>
    /// replacement to: exp(A)
    /// </summary>
    public static MatlabMatrix Exp(MatlabMatrix matrix)
    {
        return new MatlabMatrix(Matrix<double>.Exp(matrix.Values));
    }


    public readonly struct Dot
    {
        public MatlabMatrix Matrix { get; }

        public Dot(MatlabMatrix matrix)
        {
            Matrix = matrix;
        }

        public static implicit operator Dot(MatlabMatrix matrix)
        {
            return new Dot(matrix);
        }
    }



    /// <summary>
    /// auxiliary function for calculating incidence matrix from adjacency matrix
    /// </summary>
    public static MatlabMatrix IncidenceFromAdjacency(MatlabMatrix A)
    {
        int n = Size(A, 0);
        int m = SumWithOnes(A, x => x != 0) / 2;
        MatlabMatrix N = Zeros(n, m);
        int ctr = 0;
        for (int mu = 0; mu < n; mu++)
        {
            for (int nu = mu + 1; nu < n; nu++)
            {
                if (A[mu, nu] == 0)
                {
                    continue;
                }
                else
                {
                    N[mu, ctr] = 1;
                    N[nu, ctr] = -1;
                    ctr++;
                }
            }
        }

        return N;
    }

    /// <summary>
    /// replacement to: tanh(y)
    /// </summary>
    public static MatlabMatrix Tanh(MatlabMatrix matrix)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);
        MatlabMatrix result = MatlabMatrix.Create(matrixSize0, matrixSize1);

        for (int i = 0; i < matrixSize0; i++)
        {
            for (int j = 0; j < matrixSize1; j++)
            {
                result[i,j] = Math.Tanh(matrix[i,j]);
            }
        }

        return result;
    }

    /// <summary>
    /// replacement to: cosh(y)
    /// </summary>
    public static MatlabMatrix Cosh(MatlabMatrix matrix)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);
        MatlabMatrix result = MatlabMatrix.Create(matrixSize0, matrixSize1);

        for (int i = 0; i < matrixSize0; i++)
        {
            for (int j = 0; j < matrixSize1; j++)
            {
                result[i,j] = Math.Cosh(matrix[i,j]);
            }
        }

        return result;
    }
}
