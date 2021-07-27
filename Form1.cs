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
        private static Solver _solver;
        private static Sudoku _sudokuInput;
        private static Sudoku _sudokuSolution;

        private bool _isInputChanged;

        private Thread _solvingThread;

        public Form1()
        {
            InitializeComponent();
            DataGridViewSudokuMatrixSetup();

            _solver = new Solver();
            _isInputChanged = true;

            _solver.OnStatusChanged = OnStatusChanged;
            _solver.OnSolvingDone = OnSolvingDone;

            cCancel.Enabled = false;
        }

        public void OnSolvingDone()
        {
            Console.WriteLine("[main] OnSolvingDone");
            
            cCancel.Enabled = false;
            bSolve.Enabled = true;
            _isInputChanged = false;

            ReprintSolution();
        }
        public void OnStatusChanged(Solver.eSolvingStatus stat)
        {
            Console.WriteLine("[main] OnStatusChanged, new status = " + stat.ToString());
        }

        private void DataGridViewSudokuMatrixSetup()
        {
            dgvSudokuMatrix.Rows.Clear();

            dgvSudokuMatrix.ColumnCount = 9;

            for (int i = 0; i < 9; i++)
                dgvSudokuMatrix.Columns[i].Width = 30;


            dgvSudokuMatrix.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dgvSudokuMatrix.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSudokuMatrix.ColumnHeadersDefaultCellStyle.Font = new Font(dgvSudokuMatrix.Font, FontStyle.Bold);

            dgvSudokuMatrix.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvSudokuMatrix.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvSudokuMatrix.AllowUserToResizeColumns = false;
            dgvSudokuMatrix.AllowUserToResizeRows = false;
            dgvSudokuMatrix.ColumnHeadersVisible = false;
            dgvSudokuMatrix.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvSudokuMatrix.GridColor = Color.Black;
            dgvSudokuMatrix.RowHeadersVisible = false;

            dgvSudokuMatrix.RowTemplate.Height = 30;
            for (int i = 0; i < 9; i++)
                dgvSudokuMatrix.Rows.Add();
        }

        private Sudoku CreateSudokuFromTable()
        {
            int[,] tempArray = new int[9, 9];
            string cellText;

            for(int c = 0; c <9; c++)
                for (int r = 0; r < 9; r++)
                {
                    cellText = (string) dgvSudokuMatrix.Rows[r].Cells[c].Value;

                    if (String.IsNullOrEmpty(cellText))
                    {
                        tempArray[r, c] = -1;
                        continue;
                    }

                    if (cellText.Length != 1)
                        //throw netempArray[c, r] = -1;w ArgumentOutOfRangeException();
                        tempArray[r, c] = -1;

                    //if digit
                    if ((byte)cellText[0] >= 0x30 && (byte)cellText[0] <= 0x39)
                        tempArray[r, c] = cellText[0] - 0x30;
                    else
                        tempArray[r, c] = 0;
                }

            return new Sudoku(tempArray);
        }

        public void PrintSudokuToTable(Sudoku sudoku)
        {
            int element;
            for (int c = 0; c < 9; c++)
                for (int r = 0; r < 9; r++)
                {
                    element = sudoku._array[r, c];
                    if(element == -1)
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
            

            _sudokuInput = CreateSudokuFromTable();

            _solver.LoadSudoku(_sudokuInput);
            Console.WriteLine("[main] _solvingThread.Start()");

            _solvingThread = new Thread(new ThreadStart(_solver.Solve));
            _solvingThread.Start();

            cCancel.Enabled = false;
            bSolve.Enabled = true;
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
    }
}
