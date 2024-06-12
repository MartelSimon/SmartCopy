namespace SDCardTool
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
            this.buttonSDCardSelect = new System.Windows.Forms.Button();
            this.textBoxSDPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSourcePath = new System.Windows.Forms.TextBox();
            this.buttonFolderSelect = new System.Windows.Forms.Button();
            this.buttonScan = new System.Windows.Forms.Button();
            this.checkBoxUpdate = new System.Windows.Forms.CheckBox();
            this.checkBoxAdd = new System.Windows.Forms.CheckBox();
            this.checkBoxRemove = new System.Windows.Forms.CheckBox();
            this.treeViewChanges = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelToRemove = new System.Windows.Forms.Label();
            this.labelToAdd = new System.Windows.Forms.Label();
            this.labelToUpdate = new System.Windows.Forms.Label();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSDCardSelect
            // 
            this.buttonSDCardSelect.Location = new System.Drawing.Point(118, 9);
            this.buttonSDCardSelect.Name = "buttonSDCardSelect";
            this.buttonSDCardSelect.Size = new System.Drawing.Size(75, 23);
            this.buttonSDCardSelect.TabIndex = 0;
            this.buttonSDCardSelect.Text = "Select Path";
            this.buttonSDCardSelect.UseVisualStyleBackColor = true;
            this.buttonSDCardSelect.Click += new System.EventHandler(this.buttonSDCardSelect_Click);
            // 
            // textBoxSDPath
            // 
            this.textBoxSDPath.Location = new System.Drawing.Point(199, 9);
            this.textBoxSDPath.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.textBoxSDPath.MaximumSize = new System.Drawing.Size(1000, 23);
            this.textBoxSDPath.MinimumSize = new System.Drawing.Size(100, 23);
            this.textBoxSDPath.Name = "textBoxSDPath";
            this.textBoxSDPath.ReadOnly = true;
            this.textBoxSDPath.Size = new System.Drawing.Size(100, 20);
            this.textBoxSDPath.TabIndex = 1;
            this.textBoxSDPath.TextChanged += new System.EventHandler(this.textBoxSDPath_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "SD Card :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "Folder :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // textBoxSourcePath
            // 
            this.textBoxSourcePath.Location = new System.Drawing.Point(199, 38);
            this.textBoxSourcePath.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.textBoxSourcePath.MaximumSize = new System.Drawing.Size(1000, 23);
            this.textBoxSourcePath.MinimumSize = new System.Drawing.Size(100, 23);
            this.textBoxSourcePath.Name = "textBoxSourcePath";
            this.textBoxSourcePath.ReadOnly = true;
            this.textBoxSourcePath.Size = new System.Drawing.Size(100, 20);
            this.textBoxSourcePath.TabIndex = 4;
            this.textBoxSourcePath.TextChanged += new System.EventHandler(this.textBoxSourcePath_TextChanged);
            // 
            // buttonFolderSelect
            // 
            this.buttonFolderSelect.Location = new System.Drawing.Point(118, 38);
            this.buttonFolderSelect.Name = "buttonFolderSelect";
            this.buttonFolderSelect.Size = new System.Drawing.Size(75, 23);
            this.buttonFolderSelect.TabIndex = 3;
            this.buttonFolderSelect.Text = "Select Path";
            this.buttonFolderSelect.UseVisualStyleBackColor = true;
            this.buttonFolderSelect.Click += new System.EventHandler(this.buttonFolderSelect_Click);
            // 
            // buttonScan
            // 
            this.buttonScan.Location = new System.Drawing.Point(12, 76);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(75, 23);
            this.buttonScan.TabIndex = 6;
            this.buttonScan.Text = "Scan";
            this.buttonScan.UseVisualStyleBackColor = true;
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            // 
            // checkBoxUpdate
            // 
            this.checkBoxUpdate.AutoSize = true;
            this.checkBoxUpdate.Location = new System.Drawing.Point(93, 76);
            this.checkBoxUpdate.MaximumSize = new System.Drawing.Size(0, 23);
            this.checkBoxUpdate.MinimumSize = new System.Drawing.Size(0, 23);
            this.checkBoxUpdate.Name = "checkBoxUpdate";
            this.checkBoxUpdate.Size = new System.Drawing.Size(61, 23);
            this.checkBoxUpdate.TabIndex = 7;
            this.checkBoxUpdate.Text = "Update";
            this.checkBoxUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxUpdate.UseVisualStyleBackColor = true;
            this.checkBoxUpdate.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBoxAdd
            // 
            this.checkBoxAdd.AutoSize = true;
            this.checkBoxAdd.Location = new System.Drawing.Point(160, 76);
            this.checkBoxAdd.MaximumSize = new System.Drawing.Size(0, 23);
            this.checkBoxAdd.MinimumSize = new System.Drawing.Size(0, 23);
            this.checkBoxAdd.Name = "checkBoxAdd";
            this.checkBoxAdd.Size = new System.Drawing.Size(45, 23);
            this.checkBoxAdd.TabIndex = 8;
            this.checkBoxAdd.Text = "Add";
            this.checkBoxAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxAdd.UseVisualStyleBackColor = true;
            this.checkBoxAdd.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBoxRemove
            // 
            this.checkBoxRemove.AutoSize = true;
            this.checkBoxRemove.Location = new System.Drawing.Point(211, 76);
            this.checkBoxRemove.MaximumSize = new System.Drawing.Size(0, 23);
            this.checkBoxRemove.MinimumSize = new System.Drawing.Size(0, 23);
            this.checkBoxRemove.Name = "checkBoxRemove";
            this.checkBoxRemove.Size = new System.Drawing.Size(66, 23);
            this.checkBoxRemove.TabIndex = 9;
            this.checkBoxRemove.Text = "Remove";
            this.checkBoxRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxRemove.UseVisualStyleBackColor = true;
            // 
            // treeViewChanges
            // 
            this.treeViewChanges.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewChanges.Location = new System.Drawing.Point(12, 105);
            this.treeViewChanges.Name = "treeViewChanges";
            this.treeViewChanges.Size = new System.Drawing.Size(287, 262);
            this.treeViewChanges.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.labelToRemove);
            this.groupBox1.Controls.Add(this.labelToAdd);
            this.groupBox1.Controls.Add(this.labelToUpdate);
            this.groupBox1.Location = new System.Drawing.Point(12, 373);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(287, 35);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // labelToRemove
            // 
            this.labelToRemove.AutoSize = true;
            this.labelToRemove.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelToRemove.Location = new System.Drawing.Point(211, 16);
            this.labelToRemove.Name = "labelToRemove";
            this.labelToRemove.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.labelToRemove.Size = new System.Drawing.Size(73, 13);
            this.labelToRemove.TabIndex = 2;
            this.labelToRemove.Text = "0 to remove";
            this.labelToRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelToAdd
            // 
            this.labelToAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.labelToAdd.AutoSize = true;
            this.labelToAdd.Location = new System.Drawing.Point(118, 16);
            this.labelToAdd.Name = "labelToAdd";
            this.labelToAdd.Size = new System.Drawing.Size(46, 13);
            this.labelToAdd.TabIndex = 1;
            this.labelToAdd.Text = "0 to add";
            this.labelToAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelToUpdate
            // 
            this.labelToUpdate.AutoSize = true;
            this.labelToUpdate.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelToUpdate.Location = new System.Drawing.Point(3, 16);
            this.labelToUpdate.Margin = new System.Windows.Forms.Padding(0);
            this.labelToUpdate.Name = "labelToUpdate";
            this.labelToUpdate.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.labelToUpdate.Size = new System.Drawing.Size(71, 13);
            this.labelToUpdate.TabIndex = 0;
            this.labelToUpdate.Text = "0 to update";
            this.labelToUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonExecute
            // 
            this.buttonExecute.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonExecute.Location = new System.Drawing.Point(110, 415);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(75, 23);
            this.buttonExecute.TabIndex = 13;
            this.buttonExecute.Text = "Execute";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(318, 450);
            this.Controls.Add(this.buttonExecute);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.treeViewChanges);
            this.Controls.Add(this.checkBoxRemove);
            this.Controls.Add(this.checkBoxAdd);
            this.Controls.Add(this.checkBoxUpdate);
            this.Controls.Add(this.buttonScan);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxSourcePath);
            this.Controls.Add(this.buttonFolderSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSDPath);
            this.Controls.Add(this.buttonSDCardSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "SDCardTool";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSDCardSelect;
        private System.Windows.Forms.TextBox textBoxSDPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSourcePath;
        private System.Windows.Forms.Button buttonFolderSelect;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.CheckBox checkBoxUpdate;
        private System.Windows.Forms.CheckBox checkBoxAdd;
        private System.Windows.Forms.CheckBox checkBoxRemove;
        private System.Windows.Forms.TreeView treeViewChanges;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelToAdd;
        private System.Windows.Forms.Label labelToUpdate;
        private System.Windows.Forms.Label labelToRemove;
        private System.Windows.Forms.Button buttonExecute;
    }
}

