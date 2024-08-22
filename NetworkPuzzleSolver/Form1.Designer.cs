namespace NetworkPuzzleSolver
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SolveButton = new Button();
            InputImageBox = new PictureBox();
            OutputImageBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)InputImageBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OutputImageBox).BeginInit();
            SuspendLayout();
            // 
            // SolveButton
            // 
            SolveButton.Location = new Point(60, 43);
            SolveButton.Name = "SolveButton";
            SolveButton.Size = new Size(75, 23);
            SolveButton.TabIndex = 0;
            SolveButton.Text = "Solve";
            SolveButton.UseVisualStyleBackColor = true;
            SolveButton.Click += SolveButton_Click;
            // 
            // InputImageBox
            // 
            InputImageBox.Location = new Point(241, 12);
            InputImageBox.Name = "InputImageBox";
            InputImageBox.Size = new Size(250, 250);
            InputImageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            InputImageBox.TabIndex = 1;
            InputImageBox.TabStop = false;
            // 
            // OutputImageBox
            // 
            OutputImageBox.Location = new Point(241, 268);
            OutputImageBox.Name = "OutputImageBox";
            OutputImageBox.Size = new Size(250, 250);
            OutputImageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            OutputImageBox.TabIndex = 2;
            OutputImageBox.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(501, 533);
            Controls.Add(OutputImageBox);
            Controls.Add(InputImageBox);
            Controls.Add(SolveButton);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)InputImageBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)OutputImageBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button SolveButton;
        private PictureBox InputImageBox;
        private PictureBox OutputImageBox;
    }
}
