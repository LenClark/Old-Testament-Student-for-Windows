
namespace OldTestamentStudent
{
    partial class frmCopyReset
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
            this.pnlBase = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.labExplanationLbl = new System.Windows.Forms.Label();
            this.chkMTWord = new System.Windows.Forms.CheckBox();
            this.chkMTVerse = new System.Windows.Forms.CheckBox();
            this.chkMTChapter = new System.Windows.Forms.CheckBox();
            this.chkMTSelection = new System.Windows.Forms.CheckBox();
            this.labExplanationFull = new System.Windows.Forms.Label();
            this.labMTLbl = new System.Windows.Forms.Label();
            this.labLXXLbl = new System.Windows.Forms.Label();
            this.chkLXXSelection = new System.Windows.Forms.CheckBox();
            this.chkLXXChapter = new System.Windows.Forms.CheckBox();
            this.chkLXXVerse = new System.Windows.Forms.CheckBox();
            this.chkLXXWord = new System.Windows.Forms.CheckBox();
            this.pnlBase.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBase
            // 
            this.pnlBase.Controls.Add(this.btnCancel);
            this.pnlBase.Controls.Add(this.btnOK);
            this.pnlBase.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBase.Location = new System.Drawing.Point(0, 192);
            this.pnlBase.Name = "pnlBase";
            this.pnlBase.Size = new System.Drawing.Size(519, 30);
            this.pnlBase.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(12, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(432, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // labExplanationLbl
            // 
            this.labExplanationLbl.AutoSize = true;
            this.labExplanationLbl.Location = new System.Drawing.Point(13, 13);
            this.labExplanationLbl.Name = "labExplanationLbl";
            this.labExplanationLbl.Size = new System.Drawing.Size(171, 13);
            this.labExplanationLbl.TabIndex = 1;
            this.labExplanationLbl.Text = "Use the stored, default settings for:";
            // 
            // chkMTWord
            // 
            this.chkMTWord.AutoSize = true;
            this.chkMTWord.Location = new System.Drawing.Point(39, 57);
            this.chkMTWord.Name = "chkMTWord";
            this.chkMTWord.Size = new System.Drawing.Size(128, 17);
            this.chkMTWord.TabIndex = 2;
            this.chkMTWord.Text = "copying a single word";
            this.chkMTWord.UseVisualStyleBackColor = true;
            // 
            // chkMTVerse
            // 
            this.chkMTVerse.AutoSize = true;
            this.chkMTVerse.Location = new System.Drawing.Point(39, 80);
            this.chkMTVerse.Name = "chkMTVerse";
            this.chkMTVerse.Size = new System.Drawing.Size(101, 17);
            this.chkMTVerse.TabIndex = 3;
            this.chkMTVerse.Text = "copying a verse";
            this.chkMTVerse.UseVisualStyleBackColor = true;
            // 
            // chkMTChapter
            // 
            this.chkMTChapter.AutoSize = true;
            this.chkMTChapter.Location = new System.Drawing.Point(39, 103);
            this.chkMTChapter.Name = "chkMTChapter";
            this.chkMTChapter.Size = new System.Drawing.Size(149, 17);
            this.chkMTChapter.TabIndex = 4;
            this.chkMTChapter.Text = "copying the entire chapter";
            this.chkMTChapter.UseVisualStyleBackColor = true;
            // 
            // chkMTSelection
            // 
            this.chkMTSelection.AutoSize = true;
            this.chkMTSelection.Location = new System.Drawing.Point(39, 126);
            this.chkMTSelection.Name = "chkMTSelection";
            this.chkMTSelection.Size = new System.Drawing.Size(193, 17);
            this.chkMTSelection.TabIndex = 5;
            this.chkMTSelection.Text = "copying the selection portion of text";
            this.chkMTSelection.UseVisualStyleBackColor = true;
            // 
            // labExplanationFull
            // 
            this.labExplanationFull.Location = new System.Drawing.Point(13, 155);
            this.labExplanationFull.Name = "labExplanationFull";
            this.labExplanationFull.Size = new System.Drawing.Size(482, 35);
            this.labExplanationFull.TabIndex = 6;
            this.labExplanationFull.Text = "If a check box (above) is \"checked\", then that menu option will use previously co" +
    "nfigured options.  To enable you to reset those options, uncheck the box.";
            // 
            // labMTLbl
            // 
            this.labMTLbl.AutoSize = true;
            this.labMTLbl.Location = new System.Drawing.Point(71, 36);
            this.labMTLbl.Name = "labMTLbl";
            this.labMTLbl.Size = new System.Drawing.Size(77, 13);
            this.labMTLbl.TabIndex = 7;
            this.labMTLbl.Text = "Masoretic Text";
            // 
            // labLXXLbl
            // 
            this.labLXXLbl.AutoSize = true;
            this.labLXXLbl.Location = new System.Drawing.Point(353, 36);
            this.labLXXLbl.Name = "labLXXLbl";
            this.labLXXLbl.Size = new System.Drawing.Size(82, 13);
            this.labLXXLbl.TabIndex = 8;
            this.labLXXLbl.Text = "Septuagint Text";
            // 
            // chkLXXSelection
            // 
            this.chkLXXSelection.AutoSize = true;
            this.chkLXXSelection.Location = new System.Drawing.Point(292, 126);
            this.chkLXXSelection.Name = "chkLXXSelection";
            this.chkLXXSelection.Size = new System.Drawing.Size(193, 17);
            this.chkLXXSelection.TabIndex = 12;
            this.chkLXXSelection.Text = "copying the selection portion of text";
            this.chkLXXSelection.UseVisualStyleBackColor = true;
            // 
            // chkLXXChapter
            // 
            this.chkLXXChapter.AutoSize = true;
            this.chkLXXChapter.Location = new System.Drawing.Point(292, 103);
            this.chkLXXChapter.Name = "chkLXXChapter";
            this.chkLXXChapter.Size = new System.Drawing.Size(149, 17);
            this.chkLXXChapter.TabIndex = 11;
            this.chkLXXChapter.Text = "copying the entire chapter";
            this.chkLXXChapter.UseVisualStyleBackColor = true;
            // 
            // chkLXXVerse
            // 
            this.chkLXXVerse.AutoSize = true;
            this.chkLXXVerse.Location = new System.Drawing.Point(292, 80);
            this.chkLXXVerse.Name = "chkLXXVerse";
            this.chkLXXVerse.Size = new System.Drawing.Size(101, 17);
            this.chkLXXVerse.TabIndex = 10;
            this.chkLXXVerse.Text = "copying a verse";
            this.chkLXXVerse.UseVisualStyleBackColor = true;
            // 
            // chkLXXWord
            // 
            this.chkLXXWord.AutoSize = true;
            this.chkLXXWord.Location = new System.Drawing.Point(292, 57);
            this.chkLXXWord.Name = "chkLXXWord";
            this.chkLXXWord.Size = new System.Drawing.Size(128, 17);
            this.chkLXXWord.TabIndex = 9;
            this.chkLXXWord.Text = "copying a single word";
            this.chkLXXWord.UseVisualStyleBackColor = true;
            // 
            // frmCopyReset
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(519, 222);
            this.ControlBox = false;
            this.Controls.Add(this.chkLXXSelection);
            this.Controls.Add(this.chkLXXChapter);
            this.Controls.Add(this.chkLXXVerse);
            this.Controls.Add(this.chkLXXWord);
            this.Controls.Add(this.labLXXLbl);
            this.Controls.Add(this.labMTLbl);
            this.Controls.Add(this.labExplanationFull);
            this.Controls.Add(this.chkMTSelection);
            this.Controls.Add(this.chkMTChapter);
            this.Controls.Add(this.chkMTVerse);
            this.Controls.Add(this.chkMTWord);
            this.Controls.Add(this.labExplanationLbl);
            this.Controls.Add(this.pnlBase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmCopyReset";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reset default copy option";
            this.pnlBase.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlBase;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label labExplanationLbl;
        private System.Windows.Forms.CheckBox chkMTWord;
        private System.Windows.Forms.CheckBox chkMTVerse;
        private System.Windows.Forms.CheckBox chkMTChapter;
        private System.Windows.Forms.CheckBox chkMTSelection;
        private System.Windows.Forms.Label labExplanationFull;
        private System.Windows.Forms.Label labMTLbl;
        private System.Windows.Forms.Label labLXXLbl;
        private System.Windows.Forms.CheckBox chkLXXSelection;
        private System.Windows.Forms.CheckBox chkLXXChapter;
        private System.Windows.Forms.CheckBox chkLXXVerse;
        private System.Windows.Forms.CheckBox chkLXXWord;
    }
}