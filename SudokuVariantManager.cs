using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace SidokuSolver_WinForm
{
    public class SudokuVariant
    {
        public string Description;
        public SudokuFieldRules Rules;
        public SudokuVariant(string description, SudokuFieldRules rules)
        {
            Description = description ?? throw new NullReferenceException();
            Rules = rules ?? throw new NullReferenceException();
        }
    }

    public class SudokuVariantManager
    {
        List<SudokuVariant> _availableTypes;
        private int _currVariantIndex;
        public SudokuFieldRules CurrentRules
        {
            get
            {
                return _availableTypes[_currVariantIndex].Rules;
            }
        }

        public delegate void RulesChanged(SudokuFieldRules rules);
        public RulesChanged OnRulesChanged;

        public SudokuVariantManager()
        {
            _availableTypes = new List<SudokuVariant>();

            _availableTypes.Add(new SudokuVariant("6x6, 2x2 chunks, max value = 6", new SudokuFieldRules(6, 6, 6, 2, 2)));
            _availableTypes.Add(new SudokuVariant("9x9, 3x3 chunks, max value = 9", new SudokuFieldRules(9, 9, 9, 3, 3)));
            _availableTypes.Add(new SudokuVariant("12x9, 4x3 chunks, max value = 12", new SudokuFieldRules(12, 9, 12, 4, 3)));
            _availableTypes.Add(new SudokuVariant("16x16, 4x4 chunks, max value = 16", new SudokuFieldRules(16, 16, 16, 4, 4)));
            //_availableTypes.Add(new SudokuVariant("25x25, 5x5 chunks, max value = 25", new SudokuFieldRules(25, 25, 25, 5, 5)));

            _currVariantIndex = 0;

            if (OnRulesChanged != null) OnRulesChanged(CurrentRules);

            //ChangeGameVariant(_currVariantIndex);

            
            //_isInputChanged = true;

            //_solver.OnStatusChanged = OnStatusChanged;
            //_solver.OnSolvingDone = OnSolvingDone;
        }


        public bool ChangeGameVariant(int variantIndex)
        {
            if(variantIndex < 0 || variantIndex >= _availableTypes.Count)
                return false;

            if (_currVariantIndex == variantIndex)
                return true;

            _currVariantIndex = variantIndex;

            if (OnRulesChanged != null) OnRulesChanged(CurrentRules);


            //_sudokuInput;
            //_sudokuSolution;
            //_solver = new Solver();

            return true;
        }


        //public bool LoadSudokuFromDGV()
        //{
        //    //if (_outputRef == null) throw new NullReferenceException();

        //    _solver.LoadSudoku(
        //        CreateSudokuFromTable(_availableTypes[_currVariantIndex])
        //        );



        //    return true;
        //}


        

        ////public void OnSolvingDone()
        ////{
        ////    Console.WriteLine("[main] OnSolvingDone");

        ////    cCancel.Enabled = false;
        ////    bSolve.Enabled = true;
        ////    _isInputChanged = false;

        ////    ReprintSolution();
        ////}
        ////public void OnStatusChanged(Solver.eSolvingStatus stat)
        ////{
        ////    Console.WriteLine("[main] OnStatusChanged, new status = " + stat.ToString());
        ////}


        //public void Solve()
        //{
        //    // avoid multiple executing
        //    if (_solver.Status == Solver.eSolvingStatus.InProgress)
        //    {
        //        Console.WriteLine("Generacja zablokowane, bo watek jeszcze raz");
        //        return;
        //    }

        //    // check if already generated
        //    if (_isInputChanged == false)
        //    {
        //        if (_solver.Status == Solver.eSolvingStatus.SolvingSuccess ||
        //        _solver.Status == Solver.eSolvingStatus.SolvingFail)
        //        {
        //            Console.WriteLine("[main] Already Solved.");
        //            ReprintSolution();
        //            return;
        //        }
        //    }


        //    LoadSudokuFromDGV();
        //    Console.WriteLine("[main] _solvingThread.Start()");

        //    _solvingThread = new Thread(new ThreadStart(_solver.Solve));
        //    _solvingThread.Start();

        //    cCancel.Enabled = false;
        //    bSolve.Enabled = true;
        //}

        //public void ReprintSolution()
        //{
        //    if (_isInputChanged == false)
        //    {
        //        if (_solver.Status == Solver.eSolvingStatus.SolvingSuccess)
        //        {
        //            _sudokuSolution = _solver.GetSolution();
        //            PrintSudokuToTable(_sudokuSolution);
        //            return;
        //        }

        //        if (_solver.Status == Solver.eSolvingStatus.SolvingFail)
        //        {
        //            MessageBox.Show("Solving error.", "Your sudoku is not able to solve.", MessageBoxButtons.OK);
        //        }
        //    }

        //}

        public List<SudokuVariant> GetAvailableGames()
        {
            return _availableTypes;
        }

    }
}
