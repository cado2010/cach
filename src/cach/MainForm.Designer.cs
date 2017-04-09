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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 674);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Move:";
            // 
            // textBoxMove
            // 
            this.textBoxMove.Location = new System.Drawing.Point(44, 672);
            this.textBoxMove.Name = "textBoxMove";
            this.textBoxMove.Size = new System.Drawing.Size(100, 20);
            this.textBoxMove.TabIndex = 1;
            // 
            // buttonMove
            // 
            this.buttonMove.Location = new System.Drawing.Point(150, 670);
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
            this.label2.Location = new System.Drawing.Point(248, 675);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "To Move:";
            // 
            // labelNextToMove
            // 
            this.labelNextToMove.AutoSize = true;
            this.labelNextToMove.Location = new System.Drawing.Point(307, 675);
            this.labelNextToMove.Name = "labelNextToMove";
            this.labelNextToMove.Size = new System.Drawing.Size(68, 13);
            this.labelNextToMove.TabIndex = 0;
            this.labelNextToMove.Text = "next to move";
            // 
            // labelGameStatus
            // 
            this.labelGameStatus.AutoSize = true;
            this.labelGameStatus.Location = new System.Drawing.Point(453, 675);
            this.labelGameStatus.Name = "labelGameStatus";
            this.labelGameStatus.Size = new System.Drawing.Size(156, 13);
            this.labelGameStatus.TabIndex = 0;
            this.labelGameStatus.Text = "next to move asdasd asdasd as";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(394, 675);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Game:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 753);
            this.Controls.Add(this.buttonMove);
            this.Controls.Add(this.textBoxMove);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelGameStatus);
            this.Controls.Add(this.labelNextToMove);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
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
    }
}

