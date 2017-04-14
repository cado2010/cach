namespace cach
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMove = new System.Windows.Forms.TextBox();
            this.buttonMove = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelNextToMove = new System.Windows.Forms.Label();
            this.labelGameStatus = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonWhiteView = new System.Windows.Forms.Button();
            this.buttonBlackView = new System.Windows.Forms.Button();
            this.checkBoxAlwaysCurrent = new System.Windows.Forms.CheckBox();
            this.buttonCreateBoard = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 702);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Move/FEN:";
            // 
            // textBoxMove
            // 
            this.textBoxMove.Location = new System.Drawing.Point(67, 700);
            this.textBoxMove.Name = "textBoxMove";
            this.textBoxMove.Size = new System.Drawing.Size(100, 20);
            this.textBoxMove.TabIndex = 1;
            // 
            // buttonMove
            // 
            this.buttonMove.Location = new System.Drawing.Point(173, 698);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(75, 23);
            this.buttonMove.TabIndex = 2;
            this.buttonMove.Text = "Move";
            this.buttonMove.UseVisualStyleBackColor = true;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 703);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "To Move:";
            // 
            // labelNextToMove
            // 
            this.labelNextToMove.AutoSize = true;
            this.labelNextToMove.Location = new System.Drawing.Point(330, 703);
            this.labelNextToMove.Name = "labelNextToMove";
            this.labelNextToMove.Size = new System.Drawing.Size(68, 13);
            this.labelNextToMove.TabIndex = 0;
            this.labelNextToMove.Text = "next to move";
            // 
            // labelGameStatus
            // 
            this.labelGameStatus.AutoSize = true;
            this.labelGameStatus.Location = new System.Drawing.Point(476, 703);
            this.labelGameStatus.Name = "labelGameStatus";
            this.labelGameStatus.Size = new System.Drawing.Size(156, 13);
            this.labelGameStatus.TabIndex = 0;
            this.labelGameStatus.Text = "next to move asdasd asdasd as";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(417, 703);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Game:";
            // 
            // buttonWhiteView
            // 
            this.buttonWhiteView.Location = new System.Drawing.Point(173, 727);
            this.buttonWhiteView.Name = "buttonWhiteView";
            this.buttonWhiteView.Size = new System.Drawing.Size(25, 23);
            this.buttonWhiteView.TabIndex = 2;
            this.buttonWhiteView.Text = "W";
            this.buttonWhiteView.UseVisualStyleBackColor = true;
            this.buttonWhiteView.Click += new System.EventHandler(this.buttonWhiteView_Click);
            // 
            // buttonBlackView
            // 
            this.buttonBlackView.Location = new System.Drawing.Point(204, 727);
            this.buttonBlackView.Name = "buttonBlackView";
            this.buttonBlackView.Size = new System.Drawing.Size(25, 23);
            this.buttonBlackView.TabIndex = 2;
            this.buttonBlackView.Text = "B";
            this.buttonBlackView.UseVisualStyleBackColor = true;
            this.buttonBlackView.Click += new System.EventHandler(this.buttonBlackView_Click);
            // 
            // checkBoxAlwaysCurrent
            // 
            this.checkBoxAlwaysCurrent.AutoSize = true;
            this.checkBoxAlwaysCurrent.Checked = true;
            this.checkBoxAlwaysCurrent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAlwaysCurrent.Location = new System.Drawing.Point(274, 731);
            this.checkBoxAlwaysCurrent.Name = "checkBoxAlwaysCurrent";
            this.checkBoxAlwaysCurrent.Size = new System.Drawing.Size(148, 17);
            this.checkBoxAlwaysCurrent.TabIndex = 3;
            this.checkBoxAlwaysCurrent.Text = "Always show current view";
            this.checkBoxAlwaysCurrent.UseVisualStyleBackColor = true;
            this.checkBoxAlwaysCurrent.CheckedChanged += new System.EventHandler(this.checkBoxAlwaysShowCurrent_CheckedChanged);
            // 
            // buttonCreateBoard
            // 
            this.buttonCreateBoard.Location = new System.Drawing.Point(67, 727);
            this.buttonCreateBoard.Name = "buttonCreateBoard";
            this.buttonCreateBoard.Size = new System.Drawing.Size(100, 23);
            this.buttonCreateBoard.TabIndex = 4;
            this.buttonCreateBoard.Text = "Create Board";
            this.buttonCreateBoard.UseVisualStyleBackColor = true;
            this.buttonCreateBoard.Click += new System.EventHandler(this.buttonCreateBoard_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 775);
            this.Controls.Add(this.buttonCreateBoard);
            this.Controls.Add(this.checkBoxAlwaysCurrent);
            this.Controls.Add(this.buttonBlackView);
            this.Controls.Add(this.buttonWhiteView);
            this.Controls.Add(this.buttonMove);
            this.Controls.Add(this.textBoxMove);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelGameStatus);
            this.Controls.Add(this.labelNextToMove);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cach";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMove;
        private System.Windows.Forms.Button buttonMove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelNextToMove;
        private System.Windows.Forms.Label labelGameStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonWhiteView;
        private System.Windows.Forms.Button buttonBlackView;
        private System.Windows.Forms.CheckBox checkBoxAlwaysCurrent;
        private System.Windows.Forms.Button buttonCreateBoard;
    }
}

