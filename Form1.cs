using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SidokuSolver_WinForm
{
    public partial class Form1 : Form
    {
        SudokuVariantManager varManager;

        private static Solver _solver;
        private static Sudoku _sudokuInput;
        private static Sudoku _sudokuSolution;
        private bool _isInputChanged;

        private Thread _solvingThread;

        public Form1()
        {
            InitializeComponent();

            InitDataGridViewOutput();
            varManager = new SudokuVariantManager();
            varManager.OnRulesChanged += RulesChangedEvent;

            foreach(var variant in varManager.GetAvailableGames())
            {
                comboBox1.Items.Add(variant.Description);
            }
            _solver = new Solver();
            _solver.OnSolvingDone = OnSolvingDone;
            _solver.OnStatusChanged = OnStatusChanged;

            comboBox1.SelectedIndex = 0;
            comboBox1_SelectedIndexChanged(null, null);

            cCancel.Enabled = false;
        }

        public void OnSolvingDone()
        {
            Console.WriteLine("[main] OnSolvingDone");
            _isInputChanged = false;
            ReprintSolution();
        }

        public void OnStatusChanged(Solver.eSolvingStatus stat)
        {
            Console.WriteLine("[main] OnStatusChanged, new status = " + stat.ToString());

            switch (stat)
            {
                case Solver.eSolvingStatus.SudokuNotLoad:
                    break;
                case Solver.eSolvingStatus.SudokuLoad:
                    break;
                case Solver.eSolvingStatus.InProgress:
                    break;
                case Solver.eSolvingStatus.SolvingSuccess:
                case Solver.eSolvingStatus.SolvingFail:
                case Solver.eSolvingStatus.Canceled:
                    cCancel.BeginInvoke((Action)delegate ()
                    {
                        cCancel.Enabled = false;
                    });
                    bSolve.BeginInvoke((Action)delegate ()
                    {
                        bSolve.Enabled = true;
                    });
                    break;
                default:
                    break;
            }

        }

        private void InitDataGridViewOutput()
        {
            dgvSudokuMatrix.Rows.Clear(); 
            dgvSudokuMatrix.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvSudokuMatrix.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvSudokuMatrix.AllowUserToResizeColumns = false;
            dgvSudokuMatrix.AllowUserToResizeRows = false;
            dgvSudokuMatrix.ColumnHeadersVisible = false;
            dgvSudokuMatrix.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvSudokuMatrix.GridColor = Color.Black;
            dgvSudokuMatrix.RowHeadersVisible = false;
            dgvSudokuMatrix.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void PrepareDataGridViewOutput(SudokuFieldRules fieldRules)
        {
            int componentWidth = dgvSudokuMatrix.Width;
            int componentHeight = dgvSudokuMatrix.Height;
            dgvSudokuMatrix.Rows.Clear();

            dgvSudokuMatrix.ColumnCount = fieldRules.MaxCols;

            for (int i = 0; i < fieldRules.MaxCols; i++)
                dgvSudokuMatrix.Columns[i].Width = componentWidth / fieldRules.MaxCols;

            dgvSudokuMatrix.RowTemplate.Height = componentHeight / fieldRules.MaxRows;
            for (int i = 0; i < fieldRules.MaxRows; i++)
                dgvSudokuMatrix.Rows.Add();


            int smallerDim;
            if (componentWidth / fieldRules.MaxCols > componentHeight / fieldRules.MaxRows)
                smallerDim = componentHeight / fieldRules.MaxRows;
            else
                smallerDim = componentWidth / fieldRules.MaxCols;

            dgvSudokuMatrix.DefaultCellStyle.Font = new Font(FontFamily.GenericMonospace, (smallerDim*2)/5, FontStyle.Regular);

            //colouring
            for (int c = 0; c < fieldRules.MaxCols; c++)
                for (int r = 0; r < fieldRules.MaxRows; r++)
                {
                    if((c / varManager.CurrentRules.ChunkSizeCols)%2 == 1){
                        if ((r / varManager.CurrentRules.ChunkSizeRows) % 2 == 1)
                            dgvSudokuMatrix.Rows[r].Cells[c].Style.BackColor = Color.LightSteelBlue;
                        else
                            dgvSudokuMatrix.Rows[r].Cells[c].Style.BackColor = Color.White;
                    }
                    else
                    {
                        if ((r / varManager.CurrentRules.ChunkSizeRows) % 2 == 1)
                            dgvSudokuMatrix.Rows[r].Cells[c].Style.BackColor = Color.White;
                        else
                            dgvSudokuMatrix.Rows[r].Cells[c].Style.BackColor = Color.LightSteelBlue;
                    }
                }
            //dgvSudokuMatrix.Rows.RemoveAt(dgvSudokuMatrix.Rows.Count - 1);
        }

        private void RulesChangedEvent(SudokuFieldRules rules)
        {
            PrepareDataGridViewOutput(rules);
            _solver.Clear();
        }

        private Sudoku CreateSudokuFromTable(SudokuFieldRules rules)
        {
            SudokuSimpleChunk sudoku = new SudokuSimpleChunk(rules);
            string cellText;

            for (int c = 0; c < rules.MaxCols; c++)
                for (int r = 0; r < rules.MaxRows; r++)
                {
                    cellText = (string)dgvSudokuMatrix.Rows[r].Cells[c].Value;

                    if (String.IsNullOrEmpty(cellText))
                    {
                        sudoku.SetValue(r, c, -1);
                        continue;
                    }

                    if (cellText.Length != 1)
                        sudoku.SetValue(r, c, -1);

                    //if digit
                    if ((byte)cellText[0] >= 0x30 && (byte)cellText[0] <= 0x39)
                        sudoku.SetValue(r, c, cellText[0] - 0x30);
                    else
                        sudoku.SetValue(r, c, -1);
                }

            return sudoku;
        }
        public void PrintSudokuToTable(Sudoku sudoku)
        {
            int element;
            for (int c = 0; c < varManager.CurrentRules.MaxCols; c++)
                for (int r = 0; r < varManager.CurrentRules.MaxRows; r++)
                {
                    element = sudoku.DataArray[r, c];
                    if (element == -1)
                        dgvSudokuMatrix.Rows[r].Cells[c].Value = "";
                    else
                        dgvSudokuMatrix.Rows[r].Cells[c].Value = element.ToString();
                }

        }

        private void bSolve_Click(object sender, EventArgs e)
        {
            // avoid multiple executing
            if(_solver.Status == Solver.eSolvingStatus.InProgress)
            {
                Console.WriteLine("Generacja zablokowane, bo watek jeszcze raz");
                return;
            }

            // check if already generated
            if(_isInputChanged == false)
            {
                if (_solver.Status == Solver.eSolvingStatus.SolvingSuccess ||
                _solver.Status == Solver.eSolvingStatus.SolvingFail)
                {
                    Console.WriteLine("[main] Already Solved.");
                    ReprintSolution();
                    return;
                }
            }
            

            _sudokuInput = CreateSudokuFromTable(varManager.CurrentRules);

            _solver.LoadSudoku(_sudokuInput);
            Console.WriteLine("[main] _solvingThread.Start()");

            _solvingThread = new Thread(new ThreadStart(_solver.Solve));
            _solvingThread.Start();

            cCancel.Enabled = true;
            bSolve.Enabled = false;
        }

        private void ReprintSolution()
        {
            if (_isInputChanged == false)
            {
                if (_solver.Status == Solver.eSolvingStatus.SolvingSuccess)
                {
                    _sudokuSolution = _solver.GetSolution();
                    PrintSudokuToTable(_sudokuSolution);
                    return;
                }

                if (_solver.Status == Solver.eSolvingStatus.SolvingFail)
                {
                    MessageBox.Show("Solving error.", "Your sudoku is not able to solve.", MessageBoxButtons.OK);
                }
            }
            
        }

        private void bClearAll_Click(object sender, EventArgs e)
        {
            FillEachCell((r, c) => String.Empty);
        }

        private void bClearSolution_Click(object sender, EventArgs e)
        {
            PrintSudokuToTable(_sudokuInput);
        }

        private void FillEachCell(Func<int, int, string> func)
        {
            for (int c = 0; c < 9; c++)
                for (int r = 0; r < 9; r++)
                {
                    dgvSudokuMatrix.Rows[r].Cells[c].Value = func(r, c);
                }
        }

        private void cCancel_Click(object sender, EventArgs e)
        {
            _solver.Abort();
            //_solvingThread.Abort();
        }

        private void dgvSudokuMatrix_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            _isInputChanged = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            varManager.ChangeGameVariant(comboBox1.SelectedIndex);
        }

        
    }
}
