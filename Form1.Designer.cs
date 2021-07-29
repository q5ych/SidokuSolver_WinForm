
namespace SidokuSolver_WinForm
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvSudokuMatrix = new System.Windows.Forms.DataGridView();
            this.bSolve = new System.Windows.Forms.Button();
            this.bClearAll = new System.Windows.Forms.Button();
            this.bClearSolution = new System.Windows.Forms.Button();
            this.cCancel = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSudokuMatrix)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSudokuMatrix
            // 
            this.dgvSudokuMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSudokuMatrix.Location = new System.Drawing.Point(12, 86);
            this.dgvSudokuMatrix.Name = "dgvSudokuMatrix";
            this.dgvSudokuMatrix.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvSudokuMatrix.Size = new System.Drawing.Size(488, 456);
            this.dgvSudokuMatrix.TabIndex = 0;
            this.dgvSudokuMatrix.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSudokuMatrix_CellValueChanged);
            // 
            // bSolve
            // 
            this.bSolve.Location = new System.Drawing.Point(12, 51);
            this.bSolve.Name = "bSolve";
            this.bSolve.Size = new System.Drawing.Size(215, 29);
            this.bSolve.TabIndex = 1;
            this.bSolve.Text = "Solve Sudoku";
            this.bSolve.UseVisualStyleBackColor = true;
            this.bSolve.Click += new System.EventHandler(this.bSolve_Click);
            // 
            // bClearAll
            // 
            this.bClearAll.Location = new System.Drawing.Point(392, 16);
            this.bClearAll.Name = "bClearAll";
            this.bClearAll.Size = new System.Drawing.Size(108, 29);
            this.bClearAll.TabIndex = 2;
            this.bClearAll.Text = "Clear All";
            this.bClearAll.UseVisualStyleBackColor = true;
            this.bClearAll.Click += new System.EventHandler(this.bClearAll_Click);
            // 
            // bClearSolution
            // 
            this.bClearSolution.Location = new System.Drawing.Point(392, 51);
            this.bClearSolution.Name = "bClearSolution";
            this.bClearSolution.Size = new System.Drawing.Size(108, 29);
            this.bClearSolution.TabIndex = 3;
            this.bClearSolution.Text = "Clear Solution";
            this.bClearSolution.UseVisualStyleBackColor = true;
            this.bClearSolution.Click += new System.EventHandler(this.bClearSolution_Click);
            // 
            // cCancel
            // 
            this.cCancel.Location = new System.Drawing.Point(233, 51);
            this.cCancel.Name = "cCancel";
            this.cCancel.Size = new System.Drawing.Size(93, 29);
            this.cCancel.TabIndex = 4;
            this.cCancel.Text = "Cancel";
            this.cCancel.UseVisualStyleBackColor = true;
            this.cCancel.Click += new System.EventHandler(this.cCancel_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 16);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(314, 21);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 554);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.cCancel);
            this.Controls.Add(this.bClearSolution);
            this.Controls.Add(this.bClearAll);
            this.Controls.Add(this.bSolve);
            this.Controls.Add(this.dgvSudokuMatrix);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSudokuMatrix)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSudokuMatrix;
        private System.Windows.Forms.Button bSolve;
        private System.Windows.Forms.Button bClearAll;
        private System.Windows.Forms.Button bClearSolution;
        private System.Windows.Forms.Button cCancel;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}

