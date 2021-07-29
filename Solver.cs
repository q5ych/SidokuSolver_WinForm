using System;
using System.Collections.Generic;

namespace SidokuSolver_WinForm
{
    public class Solver
    {
        private Sudoku _inputSudoku;
        Stack<Step> _stepsHistory;
        private Sudoku _outputSudoku;
        private bool _abort;

        public delegate void StatusChanged(eSolvingStatus status);
        public StatusChanged OnStatusChanged;
        public delegate void SolvingDone();
        public SolvingDone OnSolvingDone;
        public eSolvingStatus Status { get; private set; }
        public enum eSolvingStatus
        {
            SudokuNotLoad,
            SudokuLoad,
            InProgress,
            SolvingSuccess,
            SolvingFail,
            Canceled
        }

        public Solver()
        {
            _stepsHistory = new Stack<Step>();
            _abort = false;

            ChangeState(eSolvingStatus.SudokuNotLoad);
        }

        public void Clear()
        {
            _stepsHistory = new Stack<Step>();
            _abort = false;
            _inputSudoku = null;
            _outputSudoku = null;

            ChangeState(eSolvingStatus.SudokuNotLoad);
        }

        public void Abort()
        {
            _abort = true;
        }

        private void ChangeState(eSolvingStatus newState)
        {
            Status = newState;
            if (OnStatusChanged != null)
                OnStatusChanged(newState);
        }

        public void LoadSudoku(Sudoku sudoku)
        {
            _abort = false;
            if(sudoku == null) throw new NullReferenceException(nameof(sudoku));
            _inputSudoku = sudoku;
            _outputSudoku = new SudokuSimpleChunk(_inputSudoku);
            _stepsHistory.Clear();

            ChangeState(eSolvingStatus.SudokuLoad);
        }

        private void ApplyStep(Step step)
        {
            _outputSudoku.SetValue(step.X, step.Y, step.Value);
        }

        private void RemoveStepFromCell(Step step)
        {
            _outputSudoku.SetValue(step.X, step.Y, _outputSudoku.Rules.BlankCellValue);
        }


        private bool StepForward()
        {
            for (int r = 0; r < _outputSudoku.Rules.MaxRows; r++)
            {
                for (int c = 0; c < _outputSudoku.Rules.MaxCols; c++)
                {
                    if (_outputSudoku.DataArray[r, c] == _outputSudoku.Rules.BlankCellValue)
                    {
                        Step step = new Step(r, c, 1);
                        _stepsHistory.Push(step);
                        //Console.WriteLine("pushing step R: " + step.X + "  C: " + step.Y + "  VAL: " + step.Value);
                        ApplyStep(step);

                        return true;
                    }
                }
            }

            return false;
        }


        private bool StepBackward()
        {
            //is not able to solve condition:
            if (_stepsHistory.Count == 0)
                return false;

            var currStep = _stepsHistory.Peek();

            if (currStep.Value != _outputSudoku.Rules.MaxCellVal)
            {
                currStep.Value += 1;
                ApplyStep(currStep);
                return true;
            }
            else
            {
                //if current step is exhaused (reached max incrementation level), need to be skipped
                RemoveStepFromCell(currStep);
                try
                {
                    _stepsHistory.Pop();
                }
                catch (InvalidOperationException)
                {
                    return false;
                }

                return StepBackward();
            }
        }

        public void Solve()
        {
            Step currStep;
            bool isAbleToStepForward;
            bool isAbleToStepBackward = true;

            ChangeState(eSolvingStatus.InProgress);

            isAbleToStepForward = StepForward();

            while (isAbleToStepForward && isAbleToStepBackward)
            {
                if (_abort)
                {
                    Console.WriteLine("canceling");
                    ChangeState(eSolvingStatus.Canceled);
                    return;
                }

                currStep = _stepsHistory.Peek();

                if (_outputSudoku.PerformCellValidation(currStep.X, currStep.Y))
                    isAbleToStepForward = StepForward();
                else
                    isAbleToStepBackward = StepBackward();

            }

            //loop eskaped because no more backwardsteps => unsolvable
            if (!isAbleToStepBackward)
                ChangeState(eSolvingStatus.SolvingFail);
            else
                ChangeState(eSolvingStatus.SolvingSuccess);
            
            if (OnSolvingDone != null)
                OnSolvingDone();

            //Console.WriteLine("____________________________________________________________________________________________");
            //Console.WriteLine("__________________                    no:  " + inc + "                      ___________________________");
            //Console.WriteLine();
            //Console.WriteLine(_outputSudoku.ToString());
            //Console.WriteLine("____________________________________________________________________________________________");

            return;
        }

        public Sudoku GetSolution()
        {
            return _outputSudoku;
        }
    }
}
