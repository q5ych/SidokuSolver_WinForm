using System;
using System.Linq;
using System.Text;

namespace SidokuSolver_WinForm
{
    public class Sudoku : ICloneable
    {
        public const int BLANK_CELL_VALUE = -1;

        public const int MAX_X = 9;
        public const int MAX_Y = 9;
        public const int MAX_CELLS = MAX_X * MAX_Y;
        public const int MAX_CELL_VAL = 9;

        public int[,] _array;

        

        public Sudoku()
        {
            _array = new int[MAX_X, MAX_Y];
        }
        public Sudoku(Sudoku sudoku)
        {
            _array = sudoku._array;
        }

        public Sudoku(int[,] data)
        {
            if (data.GetLength(0) != 9)
                throw new ArgumentOutOfRangeException();
            if (data.GetLength(1) != 9)
                throw new ArgumentOutOfRangeException();

            _array = data;
        }

        public bool IsValid(int row, int col)
        {
            if (HasColumnGotDuplicates(col))
                return false;

            else
            {
                if (HasRowGotDuplicates(row))
                    return false;

                else
                {
                    if (HasChunkGotDuplicates(row, col))
                        return false;
                    else
                        return true;
                }
            }
        }


        private bool HasChunkGotDuplicates(int row, int col)
        {
            return HasChunkGotDuplicates_recurency(row / 3, col / 3, 0); ;
        }

        //pos:
        // row = pos % 3;
        // col = pos / 3;
        private bool HasChunkGotDuplicates_recurency(int chunk_row_idx, int chunk_col_idx, int pos)
        {
            if (pos == 8)
                return false;

            int currCellVall = _array[chunk_row_idx * 3 + (pos % 3), chunk_col_idx * 3 + (pos / 3)];

            if (currCellVall == -1)
                return HasChunkGotDuplicates_recurency(chunk_row_idx, chunk_col_idx, pos + 1);


            for (int toCheckPos = pos + 1; toCheckPos < 9; toCheckPos++)
                if (currCellVall == _array[chunk_row_idx * 3 + (toCheckPos % 3), chunk_col_idx * 3 + (toCheckPos / 3)])
                    return true;

            return HasChunkGotDuplicates_recurency(chunk_row_idx, chunk_col_idx, pos + 1);

        }


        public bool HasColumnGotDuplicates(int col)
        {
            return HasColumnGotDuplicates_recurency(col, 0);
        }

        private bool HasColumnGotDuplicates_recurency(int col, int pos)
        {
            if (pos == MAX_X - 1)
                return false;

            if (_array[pos, col] == -1)
                return HasColumnGotDuplicates_recurency(col, pos + 1);

            for (int row = pos + 1; row < MAX_X; row++)
                if (_array[row, col] == _array[pos, col])
                    return true;

            return HasColumnGotDuplicates_recurency(col, pos + 1);
        }

        public bool HasRowGotDuplicates(int row)
        {
            return HasRowGotDuplicates_recurency(row, 0);
        }

        private bool HasRowGotDuplicates_recurency(int row, int pos)
        {
            if (pos == MAX_Y - 1)
                return false;

            if (_array[row, pos] == -1)
                return HasRowGotDuplicates_recurency(row, pos + 1);

            for (int col = pos + 1; col < MAX_Y; col++)
                if (_array[row, col] == _array[row, pos])
                    return true;

            return HasRowGotDuplicates_recurency(row, pos + 1);
        }




        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("------------------------------------------------------");
            for (int r = 0; r < Sudoku.MAX_X; r++)
            {
                for (int c = 0; c < Sudoku.MAX_Y; c++)
                {
                    sb.Append(" (");
                    if (_array[r, c] != BLANK_CELL_VALUE)
                        sb.Append(_array[r, c].ToString());
                    else
                        sb.Append(" ");
                    sb.Append(") ");
                    sb.Append("|");
                }
                sb.AppendLine();
                sb.AppendLine("------------------------------------------------------");
            }

            return sb.ToString();
        }

        public object Clone()
        {
            Sudoku copy = new Sudoku();

            for (int r = 0; r < Sudoku.MAX_X; r++)
                for (int c = 0; c < Sudoku.MAX_Y; c++)
                    copy._array[r, c] = _array[r, c];

            return (object)copy;
        }
    }
}
