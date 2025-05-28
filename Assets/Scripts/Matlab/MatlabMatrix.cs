using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;

/// <summary>
/// c# version of a matlab matrix, methods are as similar as possible to matlab for an easy translation process
/// </summary>
public struct MatlabMatrix
{
    public Matrix<double> Values { get; private set; }

    private readonly int valueSize0;
    private readonly int valueSize1;

    public int Size(int index)
    {
        if (index == 0)
            return valueSize0;
        else
            return valueSize1;
    }

    public MatlabMatrix(Matrix<double> values)
    {
        valueSize0 = values.RowCount;
        valueSize1 = values.ColumnCount;

        Values = values;
    }

    public MatlabMatrix(double[,] values)
    {
        valueSize0 = values.GetLength(0);
        valueSize1 = values.GetLength(1);

        Values = Matrix<double>.Build.Dense(valueSize0, valueSize1);

        for (int i = 0; i < valueSize0; i++)
        {
            for (int j = 0; j < valueSize1; j++)
            {
                Values[i, j] = values[i, j];
            }
        }
    }

    public static MatlabMatrix Create(int size0, int size1)
    {
        return new MatlabMatrix(Matrix<double>.Build.Dense(size0, size1));
    }

    /// <summary>
    /// replacement to: [Re3;RL3] (2,1)
    /// input can be MatlabMatrix or double
    /// </summary>
    public static MatlabMatrix ArrayVertical(MatlabMatrixOrDouble value1, MatlabMatrixOrDouble value2, MatlabMatrixOrDouble? value3 = null, MatlabMatrixOrDouble? value4 = null)
    {
        MatlabMatrixOrDouble result = ExpandMatrixBelow(value1, value2);

        if (value3 != null)
            result = ExpandMatrixBelow(result, (MatlabMatrixOrDouble)value3);
        if (value4 != null)
            result = ExpandMatrixBelow(result, (MatlabMatrixOrDouble)value4);

        return result.Matrix;
    }

    /// <summary>
    /// replacement to: [146,147]
    /// input can be MatlabMatrix or double
    /// </summary>
    public static MatlabMatrix ArrayHorizontal(MatlabMatrixOrDouble value1, MatlabMatrixOrDouble value2, MatlabMatrixOrDouble? value3 = null, MatlabMatrixOrDouble? value4 = null)
    {
        MatlabMatrixOrDouble result = ExpandMatrixRight(value1, value2);

        if (value3 != null)
            result = ExpandMatrixRight(result, (MatlabMatrixOrDouble)value3);
        if (value4 != null)
            result = ExpandMatrixRight(result, (MatlabMatrixOrDouble)value4);

        return result.Matrix;
    }

    public MatlabMatrix Clone()
    {
        return new MatlabMatrix(Values.Clone());
    }

    /// <summary>
    /// changes x- and y-Axis of a matrix
    /// replacement to: M'
    /// </summary>
    public MatlabMatrix Transpose()
    {
        return new MatlabMatrix(Values.Transpose());
    }

    /// <summary>
    /// get only middle values (diagonal) and create new matrix with them OR
    /// create new matrix with his values as middle values (rest is 0)
    /// examples: https://www.mathworks.com/help/matlab/ref/diag.html
    /// replacement to: diag(1./Rr)
    /// </summary>
    public MatlabMatrix Diag()
    {
        if (valueSize0 == 1 || valueSize1 == 1)
        {
            var vector = Values.Enumerate().ToArray();

            var diagMatrix = Matrix<double>.Build.Dense(vector.Length, vector.Length, 0);
            for (int i = 0; i < vector.Length; i++)
            {
                diagMatrix[i, i] = vector[i];
            }
            return new MatlabMatrix(diagMatrix);
        }
        else
        {
            var diagVector = Values.Diagonal();
            return new MatlabMatrix(Matrix<double>.Build.DenseOfColumnArrays(diagVector.ToArray()));
        }
    }


    /// <summary>
    /// Combine 2 matrices into one. matrix2 is right to matrix1.
    /// </summary>
    public static MatlabMatrix ExpandMatrixRight(MatlabMatrixOrDouble matlabMatrixOrDouble1, MatlabMatrixOrDouble matlabMatrixOrDouble2)
    {
        MatlabMatrix result;

        if (matlabMatrixOrDouble1.IsMatrix)
        {
            MatlabMatrix matrix1 = matlabMatrixOrDouble1.Matrix;

            if (matlabMatrixOrDouble2.IsMatrix)
            {
                MatlabMatrix matrix2 = matlabMatrixOrDouble2.Matrix;
                result = ExpandMatrixRight(matrix1, matrix2);
            }
            else
            {
                double v2 = matlabMatrixOrDouble2.Value;
                result = ExpandMatrixRight(matrix1, v2);
            }
        }
        else
        {
            double v1 = matlabMatrixOrDouble1.Value;

            if (matlabMatrixOrDouble2.IsMatrix)
            {
                MatlabMatrix matrix2 = matlabMatrixOrDouble2.Matrix;
                result = ExpandMatrixRight(v1, matrix2);
            }
            else
            {
                double v2 = matlabMatrixOrDouble2.Value;
                result = ExpandMatrixRight(v1, v2);
            }
        }
        return result;
    }

    /// <summary>
    /// Combine 2 matrices into one. matrix2 is right to matrix1.
    /// </summary>
    public static MatlabMatrix ExpandMatrixRight(MatlabMatrix matrix1, MatlabMatrix matrix2)
    {
        int matrix1Size0 = matrix1.Size(0);
        int matrix1Size1 = matrix1.Size(1);
        int matrix2Size0 = matrix2.Size(0);
        int matrix2Size1 = matrix2.Size(1);

        int size0 = matrix1Size0 > matrix2Size0 ? matrix1Size0 : matrix2Size0;
        int size1 = matrix1Size1 + matrix2Size1;
        MatlabMatrix result = Create(size0, size1);

        for (int i = 0; i < matrix1Size0; i++)
        {
            for (int j = 0; j < matrix1Size1; j++)
            {
                result[i, j] = matrix1[i, j];
            }
        }

        for (int i = 0; i < matrix2Size0; i++)
        {
            for (int j = 0; j < matrix2Size1; j++)
            {
                result[i + matrix1Size0, j] = matrix2[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// Combine double and matrix. matrix is right to value.
    /// </summary>
    public static MatlabMatrix ExpandMatrixRight(double value, MatlabMatrix matrix)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);

        int newSize1 = matrixSize1 + 1;
        MatlabMatrix result = Create(matrixSize0, newSize1);

        result[0, 0] = value;

        for (int i = 0; i < matrixSize0; i++)
        {
            for (int j = 0; j < matrixSize1; j++)
            {
                result[i + 1, j] = matrix[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// Combine double and matrix. value is right to matrix.
    /// </summary>
    public static MatlabMatrix ExpandMatrixRight(MatlabMatrix matrix, double value)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);

        if (matrixSize0 == 0 && matrixSize1 == 0)
        {
            matrix = Create(1, 1);
            matrix[0, 0] = value;
            return matrix;
        }

        int newSize1 = matrixSize1 + 1;
        MatlabMatrix result = Create(matrixSize0, newSize1);

        for (int i = 0; i < matrixSize0; i++)
        {
            for (int j = 0; j < matrixSize1; j++)
            {
                result[i, j] = matrix[i, j];
            }
        }

        result[0, newSize1 - 1] = value;

        return result;
    }

    /// <summary>
    /// Combine 2 doubles into a matrix. value2 is right to value1.
    /// </summary>
    public static MatlabMatrix ExpandMatrixRight(double value1, double value2)
    {
        MatlabMatrix result = Create(1, 2);
        result[0, 0] = value1;
        result[0, 1] = value2;

        return result;
    }


    /// <summary>
    /// Combine 2 matrices into one. matrix2 is bellow matrix1.
    /// </summary>
    public static MatlabMatrix ExpandMatrixBelow(MatlabMatrixOrDouble matlabMatrixOrDouble1, MatlabMatrixOrDouble matlabMatrixOrDouble2)
    {
        MatlabMatrix result;

        if (matlabMatrixOrDouble1.IsMatrix)
        {
            MatlabMatrix matrix1 = matlabMatrixOrDouble1.Matrix;

            if (matlabMatrixOrDouble2.IsMatrix)
            {
                MatlabMatrix matrix2 = matlabMatrixOrDouble2.Matrix;
                result = ExpandMatrixBelow(matrix1, matrix2);
            }
            else
            {
                double v2 = matlabMatrixOrDouble2.Value;
                result = ExpandMatrixBelow(matrix1, v2);
            }
        }
        else
        {
            double v1 = matlabMatrixOrDouble1.Value;

            if (matlabMatrixOrDouble2.IsMatrix)
            {
                MatlabMatrix matrix2 = matlabMatrixOrDouble2.Matrix;
                result = ExpandMatrixBelow(v1, matrix2);
            }
            else
            {
                double v2 = matlabMatrixOrDouble2.Value;
                result = ExpandMatrixBelow(v1, v2);
            }
        }
        return result;
    }

    /// <summary>
    /// Combine 2 matrices into one. matrix2 is bellow matrix1.
    /// </summary>
    public static MatlabMatrix ExpandMatrixBelow(MatlabMatrix matrix1, MatlabMatrix matrix2)
    {
        int matrix1Size0 = matrix1.Size(0);
        int matrix1Size1 = matrix1.Size(1);
        int matrix2Size0 = matrix2.Size(0);
        int matrix2Size1 = matrix2.Size(1);

        int size0 = matrix1Size0 + matrix2Size0;
        int size1 = matrix1Size1 > matrix2Size1 ? matrix1Size1 : matrix2Size1;
        MatlabMatrix result = Create(size0, size1);

        for (int i = 0; i < matrix1Size0; i++)
        {
            for (int j = 0; j < matrix1Size1; j++)
            {
                result[i, j] = matrix1[i, j];
            }
        }

        for (int i = 0; i < matrix2Size0; i++)
        {
            for (int j = 0; j < matrix2Size1; j++)
            {
                result[i + matrix1Size0, j] = matrix2[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// Combine double and matrix. matrix is bellow value.
    /// </summary>
    public static MatlabMatrix ExpandMatrixBelow(double value, MatlabMatrix matrix)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);

        int newSize0 = matrixSize0 + 1;
        MatlabMatrix result = Create(newSize0, matrixSize1);

        result[0, 0] = value;

        for (int i = 0; i < matrixSize0; i++)
        {
            for (int j = 0; j < matrixSize1; j++)
            {
                result[i + 1, j] = matrix[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// Combine double and matrix. value is bellow matrix.
    /// </summary>
    public static MatlabMatrix ExpandMatrixBelow(MatlabMatrix matrix, double value)
    {
        int matrixSize0 = matrix.Size(0);
        int matrixSize1 = matrix.Size(1);

        if (matrixSize0 == 0 && matrixSize1 == 0)
        {
            matrix = Create(1, 1);
            matrix[0, 0] = value;
            return matrix;
        }

        int newSize0 = matrixSize0 + 1;
        MatlabMatrix result = Create(newSize0, matrixSize1);

        for (int i = 0; i < matrixSize0; i++)
        {
            for (int j = 0; j < matrixSize1; j++)
            {
                result[i, j] = matrix[i, j];
            }
        }

        result[newSize0 - 1, 0] = value;

        return result;
    }

    /// <summary>
    /// Combine 2 doubles into a matrix. value2 is bellow value1.
    /// </summary>
    public static MatlabMatrix ExpandMatrixBelow(double value1, double value2)
    {
        MatlabMatrix result = Create(2, 1);
        result[0, 0] = value1;
        result[1, 0] = value2;

        return result;
    }

    /// <summary>
    /// Get a single row out of a matrix.
    /// </summary>
    public MatlabMatrix GetRow(int number)
    {
        int size1 = Size(1);
        MatlabMatrix result = Create(1, size1);

        for (int i = 0; i < size1; i++)
        {
            result[0, i] = this[number, i];
        }

        return result;
    }

    /// <summary>
    /// Set a single row out of a matrix. No new matrix will be created!
    /// </summary>
    /// <param name="matrix">only single row allowed (e.g. [1,2])</param>
    public void SetRow(int number, MatlabMatrix matrix)
    {
        int size1 = Size(1);

        for (int i = 0; i < size1; i++)
        {
            this[number, i] = matrix[0, i];
        }
    }

    /// <summary>
    /// Set a single row out of a matrix. No new matrix will be created!
    /// </summary>
    public void SetRow(int number, double value)
    {
        int size1 = Size(1);

        for (int i = 0; i < size1; i++)
        {
            this[number, i] = value;
        }
    }

    /// <summary>
    /// Get a single column out of a matrix.
    /// </summary>
    public MatlabMatrix GetColumn(int number)
    {
        int size0 = Size(0);
        MatlabMatrix result = Create(size0, 1);

        for (int i = 0; i < size0; i++)
        {
            result[i, 0] = this[i, number];
        }

        return result;
    }

    /// <summary>
    /// Set a single column out of a matrix. No new matrix will be created!
    /// </summary>
    /// <param name="matrix">only single column allowed (e.g. [2,1])</param>
    public void SetColumn(int number, MatlabMatrix matrix)
    {
        int size0 = Size(0);

        for (int i = 0; i < size0; i++)
        {
            this[i, number] = matrix[i, 0];
        }
    }

    /// <summary>
    /// Set a single column out of a matrix. No new matrix will be created!
    /// </summary>
    /// <param name="number"></param>
    public void SetColumn(int number, double value)
    {
        int size0 = Size(0);

        for (int i = 0; i < size0; i++)
        {
            this[i, number] = value;
        }
    }

    /// <summary>
    /// convert first row to float array
    /// </summary>
    public float[] ConvertToFloatArray()
    {
        float[] result = new float[Size(1)];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (float)this[0, i];
        }

        return result;
    }

    /// <summary>
    /// convert first row to int array
    /// </summary>
    public int[] ConvertToIntArray()
    {
        int[] result = new int[Size(1)];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (int)Math.Round(this[0, i]);
        }

        return result;
    }

    /// <summary>
    /// removes all indices given by the matrix (has to be one row!)
    /// replacement to: idxRemove(idxNeurons) = [];
    /// </summary>
    public MatlabMatrix Remove(MatlabMatrix matrix)
    {
        if (matrix.valueSize0 != 1)
            throw new ArgumentException("The matrix must be a single row.");

        int[] remove = matrix.ConvertToIntArray();
        MatlabMatrix result = Create(valueSize0, valueSize1 - remove.Length);
        int resultCount = 0;

        for (int i = 0; i < valueSize1; i++)
        {
            if (remove.Contains(i))
                continue;

            result.SetColumn(resultCount, GetColumn(i));
            resultCount++;
        }

        return result;
    }

    public void SetValues(MatlabMatrix idxInputNeurons, int max, double value)
    {
        if (idxInputNeurons.Size(0) != 1 && idxInputNeurons.Size(1) != 1)
        {
            throw new ArgumentException("idxInputNeurons must be a 1D matrix (either a single row or column).");
        }

        var inputNeuronIndices = idxInputNeurons.Size(0) == 1
            ? idxInputNeurons.Values.Row(0).ToArray()       //single row
            : idxInputNeurons.Values.Column(0).ToArray();   //single column

        foreach (int neuronIndex in inputNeuronIndices)
        {
            if (neuronIndex >= 0 && neuronIndex < valueSize1)
            {
                for (int i = 0; i < max; i++)
                {
                    Values[i, neuronIndex] = value;
                }
            }
            else
            {
                throw new IndexOutOfRangeException($"Neuron index {neuronIndex} is out of bounds.");
            }
        }
    }


    public static MatlabMatrix operator +(MatlabMatrix matrix1, MatlabMatrix matrix2)
    {
        return new MatlabMatrix(matrix1.Values + matrix2.Values);
    }

    public static MatlabMatrix operator +(double value, MatlabMatrix matrix)
    {
        return new MatlabMatrix(matrix.Values + value);
    }

    public static MatlabMatrix operator +(MatlabMatrix matrix, double value)
    {
        return new MatlabMatrix(matrix.Values + value);
    }

    public static MatlabMatrix operator -(MatlabMatrix matrix1, MatlabMatrix matrix2)
    {
        return new MatlabMatrix(matrix1.Values - matrix2.Values);
    }

    public static MatlabMatrix operator -(MatlabMatrix matrix, double value)
    {
        return new MatlabMatrix(matrix.Values - value);
    }

    public static MatlabMatrix operator -(double value, MatlabMatrix matrix)
    {
        return new MatlabMatrix(value - matrix.Values);
    }

    public static MatlabMatrix operator -(MatlabMatrix matrix)
    {
        return new MatlabMatrix(-matrix.Values);
    }

    /// <summary>
    /// matrix multiplication
    /// </summary>
    public static MatlabMatrix operator *(MatlabMatrix matrix1, MatlabMatrix matrix2)
    {
        return new MatlabMatrix(matrix1.Values * matrix2.Values);
    }

    /// <summary>
    /// no matrix multiplication
    /// </summary>
    public static MatlabMatrix operator *(MatlabMatrix matrix1, MatlabUtil.Dot dotMatrix2)
    {
        MatlabMatrix matrix2 = dotMatrix2.Matrix;
        return new MatlabMatrix(Matrix<double>.op_DotMultiply(matrix1.Values, matrix2.Values));
    }

    public static MatlabMatrix DotMultiply(MatlabMatrix matrix1, MatlabMatrix matrix2)
    {
        return new MatlabMatrix(Matrix<double>.op_DotMultiply(matrix1.Values, matrix2.Values));
    }

    /// <summary>
    /// replacement to: x*M and x.*M
    /// </summary>
    public static MatlabMatrix operator *(double value, MatlabMatrix matrix)
    {
        return new MatlabMatrix(matrix.Values * value);
    }

    /// <summary>
    /// replacement to: M*x and M.*x
    /// </summary>
    public static MatlabMatrix operator *(MatlabMatrix matrix, double value)
    {
        return new MatlabMatrix(matrix.Values * value);
    }

    /// <summary>
    /// divides value and matrix element wise into a new matrix
    /// replacement to: x./M
    /// </summary>
    public static MatlabMatrix operator /(double value, MatlabMatrix matrix)
    {
        return new MatlabMatrix(value / matrix.Values);
    }

    /// <summary>
    /// divides matrix and value element wise into a new matrix
    /// replacement to: M./x
    /// </summary>
    public static MatlabMatrix operator /(MatlabMatrix matrix, double value)
    {
        return new MatlabMatrix(matrix.Values / value);
    }

    /// <summary>
    /// divides values of both matrices element wise into a new matrix
    /// replacement to: M./M
    /// </summary>
    public static MatlabMatrix operator /(MatlabMatrix matrix1, MatlabMatrix matrix2)
    {
        if (matrix1.valueSize0 != matrix2.valueSize0)
        {
            MatlabMatrix matrix1Original = matrix1.Clone();
            MatlabMatrix matrix2Original = matrix2.Clone();

            while (matrix1.valueSize0 < matrix2.valueSize0)
            {
                matrix1 = ExpandMatrixBelow(matrix1, matrix1Original);
            }

            while (matrix2.valueSize0 < matrix1.valueSize0)
            {
                matrix2 = ExpandMatrixBelow(matrix2, matrix2Original);
            }
        }

        return new MatlabMatrix(Matrix<double>.op_DotDivide(matrix1.Values, matrix2.Values));
    }

    public double this[int value0, int value1]
    {
        get
        {
            return Values[value0,value1];
        }
        set 
        { 
            Values[value0,value1] = value;
        }
    }

    public MatlabMatrix this[int value0, MatlabMatrix value1]
    {
        get
        {
            int ySize1 = value1.Size(1);
            MatlabMatrix result = Create(1, ySize1);

            for (int i = 0; i < ySize1; i++)
            {
                result[0, i] = Values[value0, (int)Math.Round(value1[0, i])];
            }

            return result;
        }
    }

    public double this[int value1]
    {
        get
        {
            return Values[0,value1];
        }
        set 
        { 
            Values[0,value1] = value;
        }
    }

    public struct MatlabMatrixOrDouble
    {
        private readonly MatlabMatrix? matrix;
        private readonly double? value;

        public double Value => (double)value; 
        public MatlabMatrix Matrix => (MatlabMatrix)matrix; 
        public bool IsMatrix { get; }

        private MatlabMatrixOrDouble(double value)
        {
            this.value = value;
            this.matrix = null;
            IsMatrix = false;
        }
        private MatlabMatrixOrDouble(MatlabMatrix matrix)
        {
            this.value = null;
            this.matrix = matrix;
            IsMatrix = true;
        }

        public static implicit operator MatlabMatrixOrDouble(double value)
        {
            return new MatlabMatrixOrDouble(value);
        }

        public static implicit operator MatlabMatrixOrDouble(MatlabMatrix matrix)
        {
            return new MatlabMatrixOrDouble(matrix);
        }
    }

}
