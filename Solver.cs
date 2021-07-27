using System;
using System.Collections.Generic;

namespace SidokuSolver_WinForm
{
    public class Solver
    {
        private Sudoku _inputSudoku;
        Stack<Step> _stepsHistory;
        //public bool isSolved { get; private set; }
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
            Done,
            Canceled
        }

        public Solver()
        {
            _stepsHistory = new Stack<Step>();
            _abort = false;

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
            _inputSudoku = (Sudoku)sudoku.Clone();
            _outputSudoku = new Sudoku(_inputSudoku);

            ChangeState(eSolvingStatus.SudokuLoad);
        }

        private void ApplyStep(Step step)
        {
            _outputSudoku._array[step.X, step.Y] = step.Value;
        }

        private void RemoveStepFromCell(Step step)
        {
            _outputSudoku._array[step.X, step.Y] = Sudoku.BLANK_CELL_VALUE;
        }


        private bool StepForward()
        {
            for (int r = 0; r < Sudoku.MAX_X; r++)
            {
                for (int c = 0; c < Sudoku.MAX_Y; c++)
                {
                    if (_outputSudoku._array[r, c] == Sudoku.BLANK_CELL_VALUE)
                    {
                        Step step = new Step(r, c, 1);
                        _stepsHistory.Push(step);
                        ApplyStep(step);

                        return true;
                    }
                }
            }

            return false;
        }


        private bool StepBackward()
        {

            var currStep = _stepsHistory.Peek();

            if (currStep.Value != Sudoku.MAX_CELL_VAL)
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
            long inc = 0;
            Step currStep;
            bool isAbleToStepForward = false;
            bool isAbleToStepBackward = true;


            ChangeState(eSolvingStatus.InProgress);

            isAbleToStepForward = StepForward();
            //Console.ReadLine();

            while (isAbleToStepForward && isAbleToStepBackward)
            {
                if (_abort)
                {
                    Console.WriteLine("canceling");
                    ChangeState(eSolvingStatus.Canceled);
                    return;
                }

                inc++;
                currStep = _stepsHistory.Peek();

                if (_inputSudoku.IsValid(currStep.X, currStep.Y))
                {
                    isAbleToStepForward = StepForward();
                }
                else
                {
                    isAbleToStepBackward = StepBackward();
                }

            }

            Console.WriteLine("____________________________________________________________________________________________");
            Console.WriteLine("__________________                    no:  " + inc + "                      ___________________________");
            Console.WriteLine();
            Console.WriteLine(_outputSudoku.ToString());
            Console.WriteLine("____________________________________________________________________________________________");

            ChangeState(eSolvingStatus.Done);
            if(OnSolvingDone != null)
                OnSolvingDone();

            return;
        }

        public Sudoku GetSolution()
        {
            return _outputSudoku;
        }
    }
}
