namespace INFOIBV
{
    partial class INFOIBV
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

        private void InitializeComponent()
        {
            this.LoadImageButton = new System.Windows.Forms.Button();
            this.openImageDialog = new System.Windows.Forms.OpenFileDialog();
            this.imageFileName = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.applyButton = new System.Windows.Forms.Button();

            this.pipeLineLabel = new System.Windows.Forms.Label();
            this.strucElemShapeLabel = new System.Windows.Forms.Label();
            this.strucElemSizeLabel = new System.Windows.Forms.Label();
            this.morphologicalFilterLabel = new System.Windows.Forms.Label();

            this.pipeLineDropDown = new System.Windows.Forms.ComboBox();
            this.StrucElemShapeDropDown = new System.Windows.Forms.ComboBox();
            this.StrucElemShapeSizeTxtBox = new System.Windows.Forms.TextBox();
            this.morphologicalFilterDropDown = new System.Windows.Forms.ComboBox();

            this.andOrOperationLabel = new System.Windows.Forms.Label();
            this.andOrOperationDropDown = new System.Windows.Forms.ComboBox();

            this.valueCountingCheckBox = new System.Windows.Forms.CheckBox();
            this.boundaryTracingCheckBox = new System.Windows.Forms.CheckBox();

            this.HoughTransformImage = new System.Windows.Forms.CheckBox();
            this.CompleteHoughTransform = new System.Windows.Forms.CheckBox();

            this.EdgeThresholdLabel = new System.Windows.Forms.Label();
            this.EdgeThreshold = new System.Windows.Forms.TextBox();
            this.PeakThresholdLabel = new System.Windows.Forms.Label();
            this.PeakThreshold = new System.Windows.Forms.TextBox();
            this.MinThresholdLabel = new System.Windows.Forms.Label();
            this.MinThreshold = new System.Windows.Forms.TextBox();
            this.LineLengthLabel = new System.Windows.Forms.Label();
            this.LineLength = new System.Windows.Forms.TextBox();
            this.PixelGapLabel = new System.Windows.Forms.Label();
            this.PixelGap = new System.Windows.Forms.TextBox();

            this.LowerAngleLabel = new System.Windows.Forms.Label();
            this.LowerAngle = new System.Windows.Forms.TextBox();
            this.UpperAngleLabel = new System.Windows.Forms.Label();
            this.UpperAngle = new System.Windows.Forms.TextBox();

            this.saveImageDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveButton = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadImageButton
            // 
            this.LoadImageButton.Location = new System.Drawing.Point(12, 12);
            this.LoadImageButton.Name = "LoadImageButton";
            this.LoadImageButton.Size = new System.Drawing.Size(98, 23);
            this.LoadImageButton.TabIndex = 0;
            this.LoadImageButton.Text = "Load image...";
            this.LoadImageButton.UseVisualStyleBackColor = true;
            this.LoadImageButton.Click += new System.EventHandler(this.loadImageButton_Click);
            // 
            // openImageDialog
            // 
            this.openImageDialog.Filter = "Bitmap files (*.bmp;*.gif;*.jpg;*.png;*.tiff;*.jpeg)|*.bmp;*.gif;*.jpg;*.png;*.ti" +
                "ff;*.jpeg";
            this.openImageDialog.InitialDirectory = "..\\..\\images";
            // 
            // imageFileName
            // 
            this.imageFileName.Location = new System.Drawing.Point(116, 14);
            this.imageFileName.Name = "imageFileName";
            this.imageFileName.ReadOnly = true;
            this.imageFileName.Size = new System.Drawing.Size(316, 20);
            this.imageFileName.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 60);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 512);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // applyButton
            //
            this.applyButton.Location = new System.Drawing.Point(650, 40);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(200, 40);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            
            //
            // PipeLineLabel
            //
            this.pipeLineLabel.Location = new System.Drawing.Point(440, 18);
            this.pipeLineLabel.Name = "pipeLineLabel";
            this.pipeLineLabel.Size = new System.Drawing.Size(55, 23);
            this.pipeLineLabel.Text = "PipeLine: ";
            //
            // pipeLineDropDown
            //
            this.pipeLineDropDown.Location = new System.Drawing.Point(500, 14);
            this.pipeLineDropDown.Name = "pipeLineDropDown";
            this.pipeLineDropDown.Size = new System.Drawing.Size(100, 20);
            this.pipeLineDropDown.Items.Add("PipeLine 1");
            this.pipeLineDropDown.Items.Add("PipeLine 2");
            this.pipeLineDropDown.Items.Add("binary image");
            //
            // strucElemShapeLabel
            //
            this.strucElemShapeLabel.Location = new System.Drawing.Point(92, 40);
            this.strucElemShapeLabel.Name = "strucElemShapeLabel";
            this.strucElemShapeLabel.Size = new System.Drawing.Size(140, 18);
            this.strucElemShapeLabel.Text = "Structuring element shape: ";
            //
            // strucElemSizeLabel
            //
            this.strucElemSizeLabel.Location = new System.Drawing.Point(92, 60);
            this.strucElemSizeLabel.Name = "strucElemSizeLabel";
            this.strucElemSizeLabel.Size = new System.Drawing.Size(140, 18);
            this.strucElemSizeLabel.Text = "Structuring element size: ";
            //
            // morphologicalFilterLabel
            //
            this.morphologicalFilterLabel.Location = new System.Drawing.Point(310, 50);
            this.morphologicalFilterLabel.Name = "morphologicalFilterLabel";
            this.morphologicalFilterLabel.Size = new System.Drawing.Size(100, 18);
            this.morphologicalFilterLabel.Text = "Morphological filter: ";
            //
            // StrucElemShapeDropDown
            //
            this.StrucElemShapeDropDown.Location = new System.Drawing.Point(240, 36);
            this.StrucElemShapeDropDown.Name = "StrucElemShapeDropDown";
            this.StrucElemShapeDropDown.Size = new System.Drawing.Size(60, 20);
            this.StrucElemShapeDropDown.Items.Add("plus");
            this.StrucElemShapeDropDown.Items.Add("square");
            //
            // StrucElemShapeSizeTxtBox
            //
            this.StrucElemShapeSizeTxtBox.Location = new System.Drawing.Point(240, 58);
            this.StrucElemShapeSizeTxtBox.Name = "StrucElemShapeSizeTxtBox";
            this.StrucElemShapeSizeTxtBox.Size = new System.Drawing.Size(60, 20);
            //
            // morphologicalFilterDropDown
            //
            this.morphologicalFilterDropDown.Location = new System.Drawing.Point(410, 48);
            this.morphologicalFilterDropDown.Name = "morphologicalFilterDropDown";
            this.morphologicalFilterDropDown.Size = new System.Drawing.Size(60, 20);
            this.morphologicalFilterDropDown.Items.Add("erosion");
            this.morphologicalFilterDropDown.Items.Add("dilation");
            this.morphologicalFilterDropDown.Items.Add("opening");
            this.morphologicalFilterDropDown.Items.Add("closing");
            //
            // andOrOperationLabel
            //
            this.andOrOperationLabel.Location = new System.Drawing.Point(650, 16);
            this.andOrOperationLabel.Name = "andOrOperationLabel";
            this.andOrOperationLabel.Size = new System.Drawing.Size(160, 18);
            this.andOrOperationLabel.Text = "With previous and new output: ";
            //
            // andOrOperationDropDown
            //
            this.andOrOperationDropDown.Location = new System.Drawing.Point(810, 14);
            this.andOrOperationDropDown.Name = "andOrOperationDropDown";
            this.andOrOperationDropDown.Size = new System.Drawing.Size(60, 20);
            this.andOrOperationDropDown.Items.Add("AND");
            this.andOrOperationDropDown.Items.Add("OR");
            //
            //valueCountingCheckBox
            //
            this.valueCountingCheckBox.Text = "Value Counting";
            this.valueCountingCheckBox.Location = new System.Drawing.Point(500, 40);
            this.valueCountingCheckBox.Size = new System.Drawing.Size(110, 20);
            //
            //boundaryTracingCheckBox
            //
            this.boundaryTracingCheckBox.Text = "Boundary Tracing";
            this.boundaryTracingCheckBox.Location = new System.Drawing.Point(500, 60);
            this.boundaryTracingCheckBox.Size = new System.Drawing.Size(110, 20);

            this.HoughTransformImage.Text = "Hough Transform Image";
            this.HoughTransformImage.Location = new System.Drawing.Point(20, 80);
            this.HoughTransformImage.Size = new System.Drawing.Size(130, 20);

            this.CompleteHoughTransform.Text = "Complete Hough lines";
            this.CompleteHoughTransform.Location = new System.Drawing.Point(20, 100);
            this.CompleteHoughTransform.Size = new System.Drawing.Size(130, 20);



            this.EdgeThresholdLabel.Location = new System.Drawing.Point(150, 84);
            this.EdgeThresholdLabel.Name = "EdgeThresholdLabel";
            this.EdgeThresholdLabel.Size = new System.Drawing.Size(90, 18);
            this.EdgeThresholdLabel.Text = "Edge Threshold: ";
            this.EdgeThreshold.Location = new System.Drawing.Point(240, 80);
            this.EdgeThreshold.Name = "EdgeThreshold";
            this.EdgeThreshold.Size = new System.Drawing.Size(60, 20);

            this.PeakThresholdLabel.Location = new System.Drawing.Point(150, 104);
            this.PeakThresholdLabel.Name = "PeakThresholdLabel";
            this.PeakThresholdLabel.Size = new System.Drawing.Size(90, 18);
            this.PeakThresholdLabel.Text = "Peak Threshold: ";
            this.PeakThreshold.Location = new System.Drawing.Point(240, 100);
            this.PeakThreshold.Name = "StrucElemShapeSizeTxtBox";
            this.PeakThreshold.Size = new System.Drawing.Size(60, 20);
            this.PeakThreshold.Text = "250";

            this.MinThresholdLabel.Location = new System.Drawing.Point(300, 84);
            this.MinThresholdLabel.Name = "MinThresholdLabel";
            this.MinThresholdLabel.Size = new System.Drawing.Size(80, 18);
            this.MinThresholdLabel.Text = "MinThreshold: ";
            this.MinThreshold.Location = new System.Drawing.Point(380, 80);
            this.MinThreshold.Name = "MinThreshold";
            this.MinThreshold.Size = new System.Drawing.Size(60, 20);
            this.MinThreshold.Text = "250";

            this.LineLengthLabel.Location = new System.Drawing.Point(300, 104);
            this.LineLengthLabel.Name = "LineLengthLabel";
            this.LineLengthLabel.Size = new System.Drawing.Size(80, 18);
            this.LineLengthLabel.Text = "min line length: ";
            this.LineLength.Location = new System.Drawing.Point(380, 100);
            this.LineLength.Name = "LineLength";
            this.LineLength.Size = new System.Drawing.Size(60, 20);
            this.LineLength.Text = "10";

            this.PixelGapLabel.Location = new System.Drawing.Point(300, 124);
            this.PixelGapLabel.Name = "PixelGapLabel";
            this.PixelGapLabel.Size = new System.Drawing.Size(80, 18);
            this.PixelGapLabel.Text = "max Pixel Gap: ";
            this.PixelGap.Location = new System.Drawing.Point(380, 120);
            this.PixelGap.Name = "PixelGap";
            this.PixelGap.Size = new System.Drawing.Size(60, 20);
            this.PixelGap.Text = "3";

            this.LowerAngleLabel.Location = new System.Drawing.Point(440, 84);
            this.LowerAngleLabel.Name = "LowerAngleLabel";
            this.LowerAngleLabel.Size = new System.Drawing.Size(80, 18);
            this.LowerAngleLabel.Text = "Lower Angle: ";
            this.LowerAngle.Location = new System.Drawing.Point(520, 80);
            this.LowerAngle.Name = "LowerAngle";
            this.LowerAngle.Size = new System.Drawing.Size(60, 20);

            this.UpperAngleLabel.Location = new System.Drawing.Point(440, 104);
            this.UpperAngleLabel.Name = "UpperAngleLabel";
            this.UpperAngleLabel.Size = new System.Drawing.Size(80, 18);
            this.UpperAngleLabel.Text = "Upper Angle: ";
            this.UpperAngle.Location = new System.Drawing.Point(520, 100);
            this.UpperAngle.Name = "UpperAngle";
            this.UpperAngle.Size = new System.Drawing.Size(60, 20);
            //
            // saveImageDialog
            // 
            this.saveImageDialog.Filter = "Bitmap file (*.bmp)|*.bmp";
            this.saveImageDialog.InitialDirectory = "..\\..\\images";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(948, 11);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(95, 23);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save as BMP...";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(531, 60);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(512, 512);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(587, 14);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(276, 20);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;
            // 
            // INFOIBV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 576);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.applyButton);
            //this.Controls.Add(this.applyPipeLine1);
            //this.Controls.Add(this.applyPipeLine2);

            this.Controls.Add(this.pipeLineLabel);
            this.Controls.Add(this.strucElemShapeLabel);
            this.Controls.Add(this.strucElemSizeLabel);
            this.Controls.Add(this.morphologicalFilterLabel);

            this.Controls.Add(this.pipeLineDropDown);
            this.Controls.Add(this.StrucElemShapeDropDown);
            this.Controls.Add(this.StrucElemShapeSizeTxtBox);
            this.Controls.Add(this.morphologicalFilterDropDown);

            this.Controls.Add(this.andOrOperationLabel);
            this.Controls.Add(this.andOrOperationDropDown);

            this.Controls.Add(this.valueCountingCheckBox);
            this.Controls.Add(this.boundaryTracingCheckBox);

            this.Controls.Add(this.HoughTransformImage);
            this.Controls.Add(this.CompleteHoughTransform);
            this.Controls.Add(this.EdgeThresholdLabel);
            this.Controls.Add(this.EdgeThreshold);
            this.Controls.Add(this.PeakThresholdLabel);
            this.Controls.Add(this.PeakThreshold);
            this.Controls.Add(this.MinThresholdLabel);
            this.Controls.Add(this.MinThreshold);
            this.Controls.Add(this.LineLengthLabel);
            this.Controls.Add(this.LineLength);
            this.Controls.Add(this.PixelGapLabel);
            this.Controls.Add(this.PixelGap);
            this.Controls.Add(this.LowerAngleLabel);
            this.Controls.Add(this.LowerAngle);
            this.Controls.Add(this.UpperAngleLabel);
            this.Controls.Add(this.UpperAngle);



            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.imageFileName);
            this.Controls.Add(this.LoadImageButton);
            this.Location = new System.Drawing.Point(10, 10);
            this.Name = "INFOIBV";
            this.ShowIcon = false;
            this.Text = "INFOIBV";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private System.Windows.Forms.Button LoadImageButton;
        private System.Windows.Forms.OpenFileDialog openImageDialog;
        private System.Windows.Forms.TextBox imageFileName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label pipeLineLabel;
        private System.Windows.Forms.Label strucElemShapeLabel;
        private System.Windows.Forms.Label strucElemSizeLabel;
        private System.Windows.Forms.Label morphologicalFilterLabel;
        private System.Windows.Forms.ComboBox pipeLineDropDown;
        private System.Windows.Forms.ComboBox StrucElemShapeDropDown;
        private System.Windows.Forms.TextBox StrucElemShapeSizeTxtBox;
        private System.Windows.Forms.ComboBox morphologicalFilterDropDown;
        private System.Windows.Forms.SaveFileDialog saveImageDialog;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ProgressBar progressBar;

        private System.Windows.Forms.Label EdgeThresholdLabel;
        private System.Windows.Forms.Label PeakThresholdLabel;
        private System.Windows.Forms.Label MinThresholdLabel;
        private System.Windows.Forms.Label LineLengthLabel;
        private System.Windows.Forms.Label PixelGapLabel;
        private System.Windows.Forms.Label LowerAngleLabel;
        private System.Windows.Forms.Label UpperAngleLabel;
        private System.Windows.Forms.TextBox EdgeThreshold;
        private System.Windows.Forms.TextBox PeakThreshold;
        private System.Windows.Forms.TextBox MinThreshold;
        private System.Windows.Forms.TextBox LineLength;
        private System.Windows.Forms.TextBox PixelGap;
        private System.Windows.Forms.TextBox LowerAngle;
        private System.Windows.Forms.TextBox UpperAngle;

        private System.Windows.Forms.CheckBox HoughTransformImage;
        private System.Windows.Forms.CheckBox CompleteHoughTransform;

        private System.Windows.Forms.Label andOrOperationLabel;
        private System.Windows.Forms.ComboBox andOrOperationDropDown;

        private System.Windows.Forms.CheckBox valueCountingCheckBox;
        private System.Windows.Forms.CheckBox boundaryTracingCheckBox;

    }
}
