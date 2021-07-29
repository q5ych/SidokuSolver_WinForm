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
            _availableTypes.Add(new SudokuVariant("8x8, 2x2 chunks, max value = 8", new SudokuFieldRules(8, 8, 8, 2, 2)));
            _availableTypes.Add(new SudokuVariant("9x9, 3x3 chunks, max value = 9", new SudokuFieldRules(9, 9, 9, 3, 3)));
            _availableTypes.Add(new SudokuVariant("9x9, 3x1 chunks, max value = 9", new SudokuFieldRules(9, 9, 9, 3, 1)));
            _availableTypes.Add(new SudokuVariant("12x9, 4x3 chunks, max value = 12", new SudokuFieldRules(12, 9, 12, 4, 3)));
            _availableTypes.Add(new SudokuVariant("12x12, 3x3 chunks, max value = 12", new SudokuFieldRules(12, 12, 12, 3, 3)));
            _availableTypes.Add(new SudokuVariant("16x16, 4x4 chunks, max value = 16", new SudokuFieldRules(16, 16, 16, 4, 4)));

            _currVariantIndex = -1;

            if (OnRulesChanged != null) OnRulesChanged(CurrentRules);
        }


        public bool ChangeGameVariant(int variantIndex)
        {
            if(variantIndex < 0 || variantIndex >= _availableTypes.Count)
                return false;

            if (_currVariantIndex == variantIndex)
                return true;

            _currVariantIndex = variantIndex;
            if (OnRulesChanged != null) OnRulesChanged(CurrentRules);

            return true;
        }

        public List<SudokuVariant> GetAvailableGames()
        {
            return _availableTypes;
        }

    }
}
