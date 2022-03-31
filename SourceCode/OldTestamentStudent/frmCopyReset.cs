using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OldTestamentStudent
{
    public partial class frmCopyReset : Form
    {
        int isMTWordChecked = 0, isMTVerseChecked = 0, isMTChapterChecked = 0, isMTSelectionChecked = 0, 
            isLXXWordChecked = 0, isLXXVerseChecked = 0, isLXXChapterChecked = 0, isLXXSelectionChecked = 0;

        public int IsMTWordChecked { get => isMTWordChecked; set => isMTWordChecked = value; }
        public int IsMTVerseChecked { get => isMTVerseChecked; set => isMTVerseChecked = value; }
        public int IsMTChapterChecked { get => isMTChapterChecked; set => isMTChapterChecked = value; }
        public int IsMTSelectionChecked { get => isMTSelectionChecked; set => isMTSelectionChecked = value; }
        public int IsLXXWordChecked { get => isLXXWordChecked; set => isLXXWordChecked = value; }
        public int IsLXXVerseChecked { get => isLXXVerseChecked; set => isLXXVerseChecked = value; }
        public int IsLXXChapterChecked { get => isLXXChapterChecked; set => isLXXChapterChecked = value; }
        public int IsLXXSelectionChecked { get => isLXXSelectionChecked; set => isLXXSelectionChecked = value; }

        public frmCopyReset()
        {
            InitializeComponent();
        }

        public void populateCheckboxes()
        {
            if (isMTWordChecked == 0) chkMTWord.Enabled = false;
            else
            {
                chkMTWord.Enabled = true;
                chkMTWord.Checked = (isMTWordChecked == 2);
            }
            if (isMTVerseChecked == 0) chkMTVerse.Enabled = false;
            else
            {
                chkMTVerse.Enabled = true;
                chkMTVerse.Checked = (isMTVerseChecked == 2);
            }
            if (isMTChapterChecked == 0) chkMTChapter.Enabled = false;
            else
            {
                chkMTChapter.Enabled = true;
                chkMTChapter.Checked = (isMTChapterChecked == 2);
            }
            if (isMTSelectionChecked == 0) chkMTSelection.Enabled = false;
            else
            {
                chkMTSelection.Enabled = true;
                chkMTSelection.Checked = (isMTSelectionChecked == 2);
            }
            if (isLXXWordChecked == 0) chkLXXWord.Enabled = false;
            else
            {
                chkLXXWord.Enabled = true;
                chkLXXWord.Checked = (isLXXWordChecked == 2);
            }
            if (isLXXVerseChecked == 0) chkLXXVerse.Enabled = false;
            else
            {
                chkLXXVerse.Enabled = true;
                chkLXXVerse.Checked = (isLXXVerseChecked == 2);
            }
            if (isLXXChapterChecked == 0) chkLXXChapter.Enabled = false;
            else
            {
                chkLXXChapter.Enabled = true;
                chkLXXChapter.Checked = (isLXXChapterChecked == 2);
            }
            if (isLXXSelectionChecked == 0) chkLXXSelection.Enabled = false;
            else
            {
                chkLXXSelection.Enabled = true;
                chkLXXSelection.Checked = (isLXXSelectionChecked == 2);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (chkMTWord.Enabled)
            {
                if (chkMTWord.Checked) isMTWordChecked = 2;
                else isMTWordChecked = 1;
            }
            else isMTWordChecked = 0;
            if (chkMTVerse.Enabled)
            {
                if (chkMTVerse.Checked) isMTVerseChecked = 2;
                else isMTVerseChecked = 1;
            }
            else isMTVerseChecked = 0;
            if (chkMTChapter.Enabled)
            {
                if (chkMTChapter.Checked) isMTChapterChecked = 2;
                else isMTChapterChecked = 1;
            }
            else isMTChapterChecked = 0;
            if (chkMTSelection.Enabled)
            {
                if (chkMTSelection.Checked) isMTSelectionChecked = 2;
                else isMTSelectionChecked = 1;
            }
            else isMTSelectionChecked = 0;
            if (chkLXXWord.Enabled)
            {
                if (chkLXXWord.Checked) isLXXWordChecked = 2;
                else isLXXWordChecked = 1;
            }
            else isLXXWordChecked = 0;
            if (chkLXXVerse.Enabled)
            {
                if (chkLXXVerse.Checked) isLXXVerseChecked = 2;
                else isLXXVerseChecked = 1;
            }
            else isLXXVerseChecked = 0;
            if (chkLXXChapter.Enabled)
            {
                if (chkLXXChapter.Checked) isLXXChapterChecked = 2;
                else isLXXChapterChecked = 1;
            }
            else isLXXChapterChecked = 0;
            if (chkLXXSelection.Enabled)
            {
                if (chkLXXSelection.Checked) isLXXSelectionChecked = 2;
                else isLXXSelectionChecked = 1;
            }
            else isLXXSelectionChecked = 0;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
