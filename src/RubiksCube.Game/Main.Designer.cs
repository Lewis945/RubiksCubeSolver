﻿using RubiksCubeSolver.Cube.Rendering.Controls;

namespace RubiksCube.Game
{
    partial class Main
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
            this.radioButton3dCube = new System.Windows.Forms.RadioButton();
            this.radioButton2dCube = new System.Windows.Forms.RadioButton();
            this.groupBoxDimensions = new System.Windows.Forms.GroupBox();
            this.nextButton = new System.Windows.Forms.Button();
            this.solveButton = new System.Windows.Forms.Button();
            this.rubicsCubeControl = new RubiksCubeSolver.Cube.Rendering.Controls.RubicsCubeControl();
            this.shuffleButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            this.groupBoxDimensions.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButton3dCube
            // 
            this.radioButton3dCube.AutoSize = true;
            this.radioButton3dCube.Checked = true;
            this.radioButton3dCube.Location = new System.Drawing.Point(6, 19);
            this.radioButton3dCube.Name = "radioButton3dCube";
            this.radioButton3dCube.Size = new System.Drawing.Size(39, 17);
            this.radioButton3dCube.TabIndex = 1;
            this.radioButton3dCube.TabStop = true;
            this.radioButton3dCube.Text = "3D";
            this.radioButton3dCube.UseVisualStyleBackColor = true;
            this.radioButton3dCube.CheckedChanged += new System.EventHandler(this.radioButton3dCube_CheckedChanged);
            // 
            // radioButton2dCube
            // 
            this.radioButton2dCube.AutoSize = true;
            this.radioButton2dCube.Location = new System.Drawing.Point(6, 42);
            this.radioButton2dCube.Name = "radioButton2dCube";
            this.radioButton2dCube.Size = new System.Drawing.Size(39, 17);
            this.radioButton2dCube.TabIndex = 2;
            this.radioButton2dCube.TabStop = true;
            this.radioButton2dCube.Text = "2D";
            this.radioButton2dCube.UseVisualStyleBackColor = true;
            this.radioButton2dCube.CheckedChanged += new System.EventHandler(this.radioButton2dCube_CheckedChanged);
            // 
            // groupBoxDimensions
            // 
            this.groupBoxDimensions.Controls.Add(this.radioButton3dCube);
            this.groupBoxDimensions.Controls.Add(this.radioButton2dCube);
            this.groupBoxDimensions.Location = new System.Drawing.Point(399, 12);
            this.groupBoxDimensions.Name = "groupBoxDimensions";
            this.groupBoxDimensions.Size = new System.Drawing.Size(176, 66);
            this.groupBoxDimensions.TabIndex = 3;
            this.groupBoxDimensions.TabStop = false;
            this.groupBoxDimensions.Text = "Dimensions";
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(481, 113);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 4;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // solveButton
            // 
            this.solveButton.Location = new System.Drawing.Point(399, 113);
            this.solveButton.Name = "solveButton";
            this.solveButton.Size = new System.Drawing.Size(75, 23);
            this.solveButton.TabIndex = 5;
            this.solveButton.Text = "Solve";
            this.solveButton.UseVisualStyleBackColor = true;
            this.solveButton.Click += new System.EventHandler(this.solveButton_Click);
            // 
            // rubicsCubeControl
            // 
            this.rubicsCubeControl.DrawingMode = RubiksCube.Game.GraphicsEngine.DrawingMode.Mode3D;
            this.rubicsCubeControl.Location = new System.Drawing.Point(12, 12);
            this.rubicsCubeControl.MaxFps = 30D;
            this.rubicsCubeControl.Name = "rubicsCubeControl";
            this.rubicsCubeControl.RotationSpeed = 250;
            this.rubicsCubeControl.Size = new System.Drawing.Size(381, 348);
            this.rubicsCubeControl.TabIndex = 0;
            this.rubicsCubeControl.Zoom = 1D;
            // 
            // shuffleButton
            // 
            this.shuffleButton.Location = new System.Drawing.Point(481, 84);
            this.shuffleButton.Name = "shuffleButton";
            this.shuffleButton.Size = new System.Drawing.Size(75, 23);
            this.shuffleButton.TabIndex = 6;
            this.shuffleButton.Text = "Shuffle";
            this.shuffleButton.UseVisualStyleBackColor = true;
            this.shuffleButton.Click += new System.EventHandler(this.shuffleButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(399, 84);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 7;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 372);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.shuffleButton);
            this.Controls.Add(this.solveButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.groupBoxDimensions);
            this.Controls.Add(this.rubicsCubeControl);
            this.Name = "Main";
            this.Text = "Form1";
            this.groupBoxDimensions.ResumeLayout(false);
            this.groupBoxDimensions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private RubicsCubeControl rubicsCubeControl;
        private System.Windows.Forms.RadioButton radioButton3dCube;
        private System.Windows.Forms.RadioButton radioButton2dCube;
        private System.Windows.Forms.GroupBox groupBoxDimensions;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button solveButton;
        private System.Windows.Forms.Button shuffleButton;
        private System.Windows.Forms.Button resetButton;
    }
}

