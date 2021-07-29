using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SidokuSolver_WinForm
{
    public class SudokuFieldRules
    {
        public int BlankCellValue { get; private set; }
        public int MaxRows { get; private set; }
        public int MaxCols { get; private set; }
        public int MaxCellsQty { get; private set; }
        public int MaxCellVal { get; private set; }
        public int ChunkSizeRows { get; private set; }
        public int ChunkSizeCols { get; private set; }
        public int ChunkSize { get; private set; }

        public SudokuFieldRules(int maxRows, int maxCols, int maxCellVal,
            int chunkSizeRows, int chunkSizeCols)
        {
            BlankCellValue = -1;

            MaxRows = maxRows;
            MaxCols = maxCols;
            MaxCellsQty = maxCols * maxRows;
            MaxCellVal = maxCellVal;

            ChunkSizeRows = chunkSizeRows;
            ChunkSizeCols = chunkSizeCols;

            ChunkSize = ChunkSizeRows * ChunkSizeCols;
        }
    }

    public abstract class Sudoku
    {
        public SudokuFieldRules Rules { get; private set; }
        public int[,] DataArray { get; private set; }


        public Sudoku(SudokuFieldRules rules)
        {
            Rules = rules ?? throw new NullReferenceException(nameof(rules));
            DataArray = new int[Rules.MaxRows, Rules.MaxCols];
        }

        public Sudoku(Sudoku sudoku)
        {
            if (sudoku == null) throw new NullReferenceException(nameof(sudoku));

            Rules = sudoku.Rules;

            DataArray = new int[Rules.MaxRows, Rules.MaxCols];

            for (int r = 0; r < Rules.MaxRows; r++)
                for (int c = 0; c < Rules.MaxCols; c++)
                    DataArray[r, c] = sudoku.DataArray[r, c];
        }

        public abstract bool PerformCellValidation(int row, int col);
        public abstract bool SetValue(int row, int col, int val);


    }

    public class SudokuSimpleChunk : Sudoku
    {
        

        public SudokuSimpleChunk(SudokuFieldRules rules) 
            : base(rules) 
        {
        }

        public SudokuSimpleChunk(Sudoku sudoku) 
            : base(sudoku)
        {
        }

        public override bool PerformCellValidation(int row, int col)
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

        public override bool SetValue(int row, int col, int val)
        {
            if (row > Rules.MaxRows) return false;
            if (col > Rules.MaxCols) return false;
            if (val > Rules.MaxCellVal) return false;

            DataArray[row, col] = val;

            return true;
        }



        private bool HasChunkGotDuplicates(int row, int col)
        {
            return HasChunkGotDuplicates_recurency(row / Rules.ChunkSizeRows, col / Rules.ChunkSizeCols, 0); ;
        }

        //pos:
        // row = pos % 3;
        // col = pos / 3;
        private bool HasChunkGotDuplicates_recurency(int chunk_row_idx, int chunk_col_idx, int pos)
        {
            if (pos == Rules.ChunkSize -1)
                return false;

            int currCellVall = DataArray[
                chunk_row_idx * Rules.ChunkSizeRows + (pos % Rules.ChunkSizeRows), 
                chunk_col_idx * Rules.ChunkSizeCols + (pos / Rules.ChunkSizeRows)];

            //skip unknown/unfilled cells
            if (currCellVall == -1)
                return HasChunkGotDuplicates_recurency(chunk_row_idx, chunk_col_idx, pos + 1);


            for (int toCheckPos = pos + 1; toCheckPos < Rules.ChunkSize; toCheckPos++)
                if (currCellVall == DataArray[
                    chunk_row_idx * Rules.ChunkSizeRows + (toCheckPos % Rules.ChunkSizeRows),
                    chunk_col_idx * Rules.ChunkSizeCols + (toCheckPos / Rules.ChunkSizeRows)])
                    return true;

            return HasChunkGotDuplicates_recurency(chunk_row_idx, chunk_col_idx, pos + 1);
        }


        public bool HasColumnGotDuplicates(int col)
        {
            return HasColumnGotDuplicates_recurency(col, 0);
        }

        private bool HasColumnGotDuplicates_recurency(int col, int pos)
        {
            if (pos == Rules.MaxRows - 1)
                return false;

            if (DataArray[pos, col] == -1)
                return HasColumnGotDuplicates_recurency(col, pos + 1);

            for (int row = pos + 1; row < Rules.MaxRows; row++)
                if (DataArray[row, col] == DataArray[pos, col])
                    return true;

            return HasColumnGotDuplicates_recurency(col, pos + 1);
        }

        public bool HasRowGotDuplicates(int row)
        {
            return HasRowGotDuplicates_recurency(row, 0);
        }

        private bool HasRowGotDuplicates_recurency(int row, int pos)
        {
            if (pos == Rules.MaxCols - 1)
                return false;

            if (DataArray[row, pos] == -1)
                return HasRowGotDuplicates_recurency(row, pos + 1);

            for (int col = pos + 1; col < Rules.MaxCols; col++)
                if (DataArray[row, col] == DataArray[row, pos])
                    return true;

            return HasRowGotDuplicates_recurency(row, pos + 1);
        }




        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("------------------------------------------------------");
            for (int r = 0; r < Rules.MaxRows; r++)
            {
                for (int c = 0; c < Rules.MaxCols; c++)
                {
                    sb.Append(" (");
                    if (DataArray[r, c] != Rules.BlankCellValue)
                        sb.Append(DataArray[r, c].ToString());
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




    }

    public class Sudoku_old2 
    {
        public const int BLANK_CELL_VALUE = -1;

        public const int MAX_X = 9;
        public const int MAX_Y = 9;
        public const int MAX_CELLS = MAX_X * MAX_Y;
        public const int MAX_CELL_VAL = 9;

        public int[,] _array;

        

        public Sudoku_old2()
        {
            _array = new int[MAX_X, MAX_Y];
        }
        public Sudoku_old2(Sudoku_old2 sudoku)
        {
            _array = sudoku._array;
        }

        public Sudoku_old2(int[,] data)
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




        //public override string ToString()
        //{
        //    var sb = new StringBuilder();

        //    sb.AppendLine("------------------------------------------------------");
        //    for (int r = 0; r < Sudoku_old.MAX_X; r++)
        //    {
        //        for (int c = 0; c < Sudoku_old.MAX_Y; c++)
        //        {
        //            sb.Append(" (");
        //            if (_array[r, c] != BLANK_CELL_VALUE)
        //                sb.Append(_array[r, c].ToString());
        //            else
        //                sb.Append(" ");
        //            sb.Append(") ");
        //            sb.Append("|");
        //        }
        //        sb.AppendLine();
        //        sb.AppendLine("------------------------------------------------------");
        //    }

        //    return sb.ToString();
        //}

        //public object Clone()
        //{
        //    Sudoku_old copy = new Sudoku_old();

        //    for (int r = 0; r < Sudoku_old.MAX_X; r++)
        //        for (int c = 0; c < Sudoku_old.MAX_Y; c++)
        //            copy._array[r, c] = _array[r, c];

        //    return (object)copy;
        //}
    }
}
