using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OldTestamentStudent
{
    public partial class frmNotes : Form
    {
        int actionCode, activeVersion;
        classGlobal globalVars;
        classNote noteProcs;
        TextBox txtNote;
        ComboBox cbNote;

        public frmNotes()
        {
            InitializeComponent();
        }

        public void initialiseNotesDialog(classGlobal inConfig, classNote inNote, int inAction)
        {
            /*=====================================================================================*
             *                                                                                     *
             *                                initialiseNotesDialog                                *
             *                                =====================                                *
             *                                                                                     *
             *  Essentially, handles the four menu options from the main File menu.                *
             *                                                                                     *
             *  Parameters:                                                                        *
             *  ==========                                                                         *
             *                                                                                     *
             *  actionCode                                                                         *
             *   Code                      Meaning                                                 *
             *     1      Create a new note group                                                  *
             *     2      Change to a different (existing) note group                              *
             *     3      Delete an existing note group                                            *
             *     4      Simply display the note group currently in use                           *
             *                                                                                     *
             *  activeVersion                                                                      *
             *   Code                      Meaning                                                 *
             *     0      Currently active version is the Masoretic Text, so all references are to *
             *              that                                                                   *
             *     1      Currently active version is the Septuagint                               *
             *                                                                                     *
             *=====================================================================================*/
            const int okXPstn = 306, noOfLabels = 7;

            int idx, noteCode;
            String fileLocation = "", fullFileName, newName, currentNoteName = "", currentNotesSetLabel, currentNotesSetContent, versionLabel, versionContent = "", codeRef;
            StreamReader srNote;
            DirectoryInfo notesDirectory, baseDir = null;
            DirectoryInfo[] dirList, baseList;
            int[] lblLeft, lblTop;
            String[] dialogText, field1, field2, field3;
            Label[] lblInfo;

            globalVars = inConfig;
            noteProcs = inNote;
            dialogText = new String[4] { "Create a New Note Set", "Select a different, existing Note Set", "Delete an existing Note Set (remove it completely)", "Displaying the current Note Set Name" };
            field1 = new String[4] { "Provide a name for the new Notes Set:", "Select an Existing Notes Set you want to use:", "Select an Existing Notes Set you want to delete:", "" };
            field2 = new String[4] { "This option will not only create a new Note Set but will change to that note set.", "If the combobox (above) is empty, this means that there are currently no alternative ", 
                "(Note that this will remove all notes contained in the Note Set and the action", "This is purely for information." };
            field3 = new String[4] { "", "Note Sets for you to use.", "cannot be reversed.)", "No changes will be made." };
            lblLeft = new int[noOfLabels] { 15, 15, 58, 15, 175, 15, 170 };
            lblTop = new int[noOfLabels] { 18, 38, 58, 78, 78, 98, 98 };
            currentNotesSetLabel = "The current Notes Set name is:";
            versionLabel = "The currently active version is:";
            actionCode = inAction;
            activeVersion = ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 0)).SelectedIndex;
            switch( activeVersion)
            {
                case 0:
                    noteCode = globalVars.CurrentMTNoteRef;
                    codeRef = noteProcs.expandedNumberReference(noteCode);
                    fileLocation = globalVars.FullMTNotesPath + @"\" + codeRef + @"\NoteSetName.txt";
                    srNote = new StreamReader(fileLocation);
                    currentNoteName = srNote.ReadLine();
                    srNote.Close();
                    versionContent = "the Masoretic Text";
                    fileLocation = globalVars.FullMTNotesPath + @"\" + codeRef;
                    break;
                case 1: 
                    noteCode = globalVars.CurrentLXXNoteRef;
                    codeRef = noteProcs.expandedNumberReference(noteCode);
                    fileLocation = globalVars.FullLXXNotesPath + @"\" + codeRef + @"\NoteSetName.txt";
                    srNote = new StreamReader(fileLocation);
                    currentNoteName = srNote.ReadLine();
                    srNote.Close();
                    versionContent = "the Septuagint";
                    fileLocation = globalVars.FullLXXNotesPath + @"\" + codeRef;
                    break;
            }
            currentNotesSetContent = currentNoteName;
            lblInfo = new Label[noOfLabels];
            for (idx = 0; idx < noOfLabels; idx++) lblInfo[idx] = new Label();
            notesDirectory = new DirectoryInfo(fileLocation);
            dirList = notesDirectory.GetDirectories();
            for (idx = 0; idx < noOfLabels; idx++)
            {
                lblInfo[idx].Left = lblLeft[idx];
                lblInfo[idx].Top = lblTop[idx];
                lblInfo[idx].AutoSize = true;
            }
            lblInfo[3].Text = currentNotesSetLabel;
            lblInfo[4].Text = currentNotesSetContent;
            lblInfo[4].ForeColor = Color.Red;
            lblInfo[5].Text = versionLabel;
            lblInfo[6].Text = versionContent;
            btnOK.Left = okXPstn;
            btnCancel.Visible = true;
            switch (inAction)
            {
                case 1:
                    lblInfo[0].Text = field1[0];
                    lblInfo[1].Text = field2[0];
                    lblInfo[2].Text = field3[0];
                    this.Controls.Add(lblInfo[0]);
                    this.Controls.Add(lblInfo[1]);
                    this.Controls.Add(lblInfo[2]);
                    this.Controls.Add(lblInfo[3]);
                    this.Controls.Add(lblInfo[4]);
                    this.Controls.Add(lblInfo[5]);
                    this.Controls.Add(lblInfo[6]);
                    txtNote = new TextBox();
                    txtNote.Left = 211;
                    txtNote.Top = 15;
                    txtNote.Width = 176;
                    txtNote.Height = 20;
                    this.Controls.Add(txtNote);
                    this.Text = "New Note Set";
                    break;
                case 2:
                    lblInfo[0].Text = field1[1];
                    lblInfo[1].Text = field2[1];
                    lblInfo[2].Text = field3[1];
                    this.Controls.Add(lblInfo[0]);
                    this.Controls.Add(lblInfo[1]);
                    this.Controls.Add(lblInfo[2]);
                    this.Controls.Add(lblInfo[3]);
                    this.Controls.Add(lblInfo[4]);
                    this.Controls.Add(lblInfo[5]);
                    this.Controls.Add(lblInfo[6]);
                    cbNote = new ComboBox();
                    cbNote.Left = 239;
                    cbNote.Top = 15;
                    cbNote.Width = 162;
                    cbNote.Height = 21;
                    this.Controls.Add(cbNote);
                    switch( activeVersion )
                    {
                        case 0: baseDir = new DirectoryInfo(globalVars.FullMTNotesPath); break;
                        case 1: baseDir = new DirectoryInfo(globalVars.FullLXXNotesPath); break;
                    }
                    baseList = baseDir.GetDirectories();
                    foreach (DirectoryInfo notesSet in baseList)
                    {
                        fullFileName = notesSet.FullName + @"\NoteSetName.txt";
                        srNote = new StreamReader(fullFileName);
                        newName = srNote.ReadLine();
                        srNote.Close();
                        if (String.Compare(newName, currentNoteName) == 0) continue;
                        cbNote.Items.Add(newName);
                    }
                    if (cbNote.Items.Count > 0) cbNote.SelectedIndex = 0;
                    this.Text = "Change Current Note Set";
                    break;
                case 3:
                    lblInfo[0].Text = field1[2];
                    lblInfo[1].Text = field2[2];
                    lblInfo[2].Text = field3[2];
                    this.Controls.Add(lblInfo[0]);
                    this.Controls.Add(lblInfo[1]);
                    this.Controls.Add(lblInfo[2]);
                    this.Controls.Add(lblInfo[3]);
                    this.Controls.Add(lblInfo[4]);
                    this.Controls.Add(lblInfo[5]);
                    this.Controls.Add(lblInfo[6]);
                    cbNote = new ComboBox();
                    cbNote.Left = 250;
                    cbNote.Top = 15;
                    cbNote.Width = 162;
                    cbNote.Height = 21;
                    this.Controls.Add(cbNote);
                    switch (activeVersion)
                    {
                        case 0: baseDir = new DirectoryInfo(globalVars.FullMTNotesPath); break;
                        case 1: baseDir = new DirectoryInfo(globalVars.FullLXXNotesPath); break;
                    }
                    baseList = baseDir.GetDirectories();
                    foreach (DirectoryInfo notesSet in baseList)
                    {
                        fullFileName = notesSet.FullName + @"\NoteSetName.txt";
                        srNote = new StreamReader(fullFileName);
                        newName = srNote.ReadLine();
                        srNote.Close();
                        if (String.Compare(newName, currentNoteName) == 0) continue;
                        cbNote.Items.Add(newName);
                    }
                    if (cbNote.Items.Count > 0) cbNote.SelectedIndex = 0;
                    this.Text = "Delete an Existing Note Set";
                    break;
                case 4:
                    lblInfo[3].Top = 42;
                    lblInfo[4].Left = 246;
                    lblInfo[4].Top = 42;
                    lblInfo[3].Font = new Font(lblInfo[0].Font.FontFamily, 12);
                    lblInfo[4].Font = new Font(lblInfo[0].Font.FontFamily, 12);
                    this.Controls.Add(lblInfo[1]);
                    this.Controls.Add(lblInfo[2]);
                    this.Controls.Add(lblInfo[3]);
                    this.Controls.Add(lblInfo[4]);
                    this.Controls.Add(lblInfo[5]);
                    this.Controls.Add(lblInfo[6]);
                    btnOK.Left = (this.Width - btnOK.Width) / 2;
                    btnCancel.Visible = false;
                    this.Text = "Current Note Set";
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool isFound;
            int bookId, newDirId;
            String newNoteName, foundNoteName, notePath = "", fullPath = "", chapterAndVerse, chapId, VerseId;
            String[] chapterVerseBreakdown;
            Char[] paramSplit = { '-' };
            StreamReader srNote;
            StreamWriter swNote;
            FileInfo[] listOfFiles;
            DirectoryInfo newNoteDir, subDir;
            DirectoryInfo[] listOfBookDirs;
            ComboBox cbBook, cbChapter, cbVerse;
            RichTextBox rtxtNote;
            classMTBook currentMTBook;
            classMTChapter currentMTChapter;
            classMTVerse currentMTVerse;
            classLXXBook currentLXXBook;
            classLXXChapter currentLXXChapter;
            classLXXVerse currentLXXVerse;

            switch (actionCode)
            {
                case 1:  // Create a new note group
                    newNoteName = txtNote.Text;
                    notePath = globalVars.NotesPath;
                    removeAllNotes();
                    switch (activeVersion)
                    {
                        case 0:
                            globalVars.MtNotesName = newNoteName;
                            ((TabPage)globalVars.getGroupedControl(globalVars.TabPageCode, 2)).Text = "Notes: " + newNoteName;
                            // Does that Note Set already exist?  Let's see
                            newNoteDir = new DirectoryInfo(globalVars.FullMTNotesPath);
                            listOfBookDirs = newNoteDir.GetDirectories();
                            isFound = false;
                            foreach (DirectoryInfo diDir in listOfBookDirs)
                            {
                                fullPath = diDir.FullName + @"\NoteSetName.txt";
                                srNote = new StreamReader(fullPath);
                                foundNoteName = srNote.ReadLine();
                                srNote.Close();
                                if (String.Compare(newNoteName, foundNoteName) == 0)
                                {
                                    isFound = true;
                                    break;
                                }
                            }
                            if (!isFound)
                            {
                                newDirId = globalVars.MaxMTNoteRef + 1;
                                globalVars.MaxMTNoteRef = newDirId;
                                globalVars.CurrentMTNoteRef = newDirId;
                                globalVars.MtNotesName = newNoteName;
                                fullPath = globalVars.FullMTNotesPath + @"\" + noteProcs.expandedNumberReference(newDirId);
                                Directory.CreateDirectory(fullPath);
                                swNote = new StreamWriter(fullPath + @"\NoteSetName.txt");
                                swNote.WriteLine(newNoteName);
                                swNote.Close();
                            }
                            break;
                        case 1:
                            globalVars.LxxNotesName = newNoteName;
                            ((TabPage)globalVars.getGroupedControl(globalVars.TabPageCode, 5)).Text = "Notes: " + newNoteName;
                            // Does that Note Set already exist?  Let's see
                            newNoteDir = new DirectoryInfo(globalVars.FullLXXNotesPath);
                            listOfBookDirs = newNoteDir.GetDirectories();
                            isFound = false;
                            foreach (DirectoryInfo diDir in listOfBookDirs)
                            {
                                fullPath = diDir.FullName + @"\NoteSetName.txt";
                                srNote = new StreamReader(fullPath);
                                foundNoteName = srNote.ReadLine();
                                srNote.Close();
                                if (String.Compare(newNoteName, foundNoteName) == 0)
                                {
                                    isFound = true;
                                    break;
                                }
                            }
                            if (!isFound)
                            {
                                newDirId = globalVars.MaxLXXNoteRef + 1;
                                globalVars.MaxLXXNoteRef = newDirId;
                                globalVars.CurrentLXXNoteRef = newDirId;
                                globalVars.LxxNotesName = newNoteName;
                                fullPath = globalVars.FullLXXNotesPath + @"\" + noteProcs.expandedNumberReference(newDirId);
                                Directory.CreateDirectory(fullPath);
                                swNote = new StreamWriter(fullPath + @"\NoteSetName.txt");
                                swNote.WriteLine(newNoteName);
                                swNote.Close();
                            }
                            break;
                    }
                    break;
                case 2:   // Change to a different (existing) note group
                    newNoteName = cbNote.SelectedItem.ToString();
                    removeAllNotes();
                    switch (activeVersion)
                    {
                        case 0:
                            // Does that Note Set already exist?  Let's see
                            newNoteDir = new DirectoryInfo(globalVars.FullMTNotesPath);
                            listOfBookDirs = newNoteDir.GetDirectories();
                            isFound = false;
                            foreach (DirectoryInfo diDir in listOfBookDirs)
                            {
                                notePath = diDir.Name;
                                fullPath = diDir.FullName;
                                srNote = new StreamReader(fullPath + @"\NoteSetName.txt");
                                foundNoteName = srNote.ReadLine();
                                srNote.Close();
                                if (String.Compare(newNoteName, foundNoteName) == 0)
                                {
                                    isFound = true;
                                    break;
                                }
                            }
                            if( isFound)
                            {
                                newDirId = Convert.ToInt32(notePath);
                                globalVars.CurrentMTNoteRef = newDirId;
                                subDir = new DirectoryInfo(fullPath);
                                listOfBookDirs = subDir.GetDirectories();
                                foreach (DirectoryInfo specificBookDir in listOfBookDirs)
                                {
                                    chapterAndVerse = specificBookDir.Name;
                                    bookId = Convert.ToInt32(chapterAndVerse);
                                    listOfFiles = specificBookDir.GetFiles("*.note");
                                    foreach (FileInfo noteFile in listOfFiles)
                                    {
                                        chapterAndVerse = noteFile.Name.Substring(0, noteFile.Name.Length - 5);
                                        chapterVerseBreakdown = chapterAndVerse.Split(paramSplit);
                                        globalVars.MtBookList.TryGetValue( bookId, out currentMTBook);
                                        currentMTChapter = currentMTBook.getChapterByChapterNo(chapterVerseBreakdown[0]);
                                        currentMTVerse = currentMTChapter.getVerseByVerseNo(chapterVerseBreakdown[1]);
                                        srNote = new StreamReader(noteFile.FullName);
                                        currentMTVerse.NoteText = srNote.ReadToEnd();
                                        srNote.Close();
                                        srNote.Dispose();
                                    }
                                }
                                globalVars.MtNotesName = newNoteName;
                                ((TabPage)globalVars.getGroupedControl( globalVars.TabPageCode, 2 )).Text = "Notes: " + newNoteName;
                                // One more step: if the current verse has a note in the new note set, make sure it's displayed
                                cbBook = (ComboBox)globalVars.getGroupedControl( globalVars.ComboBoxesCode, 0);
                                cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 1);
                                cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 2);
                                bookId = cbBook.SelectedIndex;
                                chapId = cbChapter.SelectedItem.ToString();
                                VerseId = cbVerse.SelectedItem.ToString();
                                globalVars.MtBookList.TryGetValue( bookId, out currentMTBook);
                                currentMTChapter = currentMTBook.getChapterByChapterNo(chapId);
                                currentMTVerse = currentMTChapter.getVerseByVerseNo(VerseId);
                                if ((currentMTVerse.NoteText == null) || (currentMTVerse.NoteText.Length > 0))
                                {
                                    rtxtNote = (RichTextBox) globalVars.getGroupedControl( globalVars.RichtextBoxCode, 2 );
                                    rtxtNote.Text = currentMTVerse.NoteText;
                                }
                            }
                            break;
                        case 1:
                            // Does that Note Set already exist?  Let's see
                            newNoteDir = new DirectoryInfo(globalVars.FullLXXNotesPath);
                            listOfBookDirs = newNoteDir.GetDirectories();
                            isFound = false;
                            foreach (DirectoryInfo diDir in listOfBookDirs)
                            {
                                notePath = diDir.Name;
                                fullPath = diDir.FullName;
                                srNote = new StreamReader(fullPath + @"\NoteSetName.txt");
                                foundNoteName = srNote.ReadLine();
                                srNote.Close();
                                if (String.Compare(newNoteName, foundNoteName) == 0)
                                {
                                    isFound = true;
                                    break;
                                }
                            }
                            if (isFound)
                            {
                                newDirId = Convert.ToInt32(notePath);
                                globalVars.CurrentLXXNoteRef = newDirId;
                                subDir = new DirectoryInfo(fullPath);
                                listOfBookDirs = subDir.GetDirectories();
                                foreach (DirectoryInfo specificBookDir in listOfBookDirs)
                                {
                                    chapterAndVerse = specificBookDir.Name;
                                    bookId = Convert.ToInt32(chapterAndVerse);
                                    listOfFiles = specificBookDir.GetFiles("*.note");
                                    foreach (FileInfo noteFile in listOfFiles)
                                    {
                                        chapterAndVerse = noteFile.Name.Substring(0, noteFile.Name.Length - 5);
                                        chapterVerseBreakdown = chapterAndVerse.Split(paramSplit);
                                        globalVars.LxxBookList.TryGetValue(bookId, out currentLXXBook);
                                        currentLXXChapter = currentLXXBook.getChapterByChapterNo(chapterVerseBreakdown[0]);
                                        currentLXXVerse = currentLXXChapter.getVerseByVerseNo(chapterVerseBreakdown[1]);
                                        srNote = new StreamReader(noteFile.FullName);
                                        currentLXXVerse.NoteText = srNote.ReadToEnd();
                                        srNote.Close();
                                        srNote.Dispose();
                                    }
                                }
                                globalVars.MtNotesName = newNoteName;
                                ((TabPage)globalVars.getGroupedControl(globalVars.TabPageCode, 5)).Text = "Notes: " + newNoteName;
                                // One more step: if the current verse has a note in the new note set, make sure it's displayed
                                cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
                                cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4);
                                cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 5);
                                bookId = cbBook.SelectedIndex;
                                chapId = cbChapter.SelectedItem.ToString();
                                VerseId = cbVerse.SelectedItem.ToString();
                                globalVars.LxxBookList.TryGetValue(bookId, out currentLXXBook);
                                currentLXXChapter = currentLXXBook.getChapterByChapterNo(chapId);
                                currentLXXVerse = currentLXXChapter.getVerseByVerseNo(VerseId);
                                if ((currentLXXVerse.NoteText == null) || (currentLXXVerse.NoteText.Length > 0))
                                {
                                    rtxtNote = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 3);
                                    rtxtNote.Text = currentLXXVerse.NoteText;
                                }
                            }
                            break;
                    }
                    break;
                case 3:  // Delete a note set
                    newNoteName = cbNote.SelectedItem.ToString();
                    switch (activeVersion)
                    {
                        case 0:
                            if (String.Compare(newNoteName, globalVars.MtNotesName) == 0)
                            {
                                MessageBox.Show("You cannot delete the Note Set that you are currently using", "Deletion error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            newNoteDir = new DirectoryInfo(globalVars.FullMTNotesPath);
                            listOfBookDirs = newNoteDir.GetDirectories();
                            isFound = false;
                            foreach (DirectoryInfo diDir in listOfBookDirs)
                            {
                                fullPath = diDir.FullName;
                                srNote = new StreamReader(fullPath + @"\NoteSetName.txt");
                                foundNoteName = srNote.ReadLine();
                                srNote.Close();
                                if (String.Compare(newNoteName, foundNoteName) == 0)
                                {
                                    isFound = true;
                                    break;
                                }
                            }
                            if (isFound)
                            {
                                newNoteDir = new DirectoryInfo(fullPath);
                                if (newNoteDir.Exists) newNoteDir.Delete(true);
                            }
                            break;
                        case 1:
                            if (String.Compare(newNoteName, globalVars.LxxNotesName) == 0)
                            {
                                MessageBox.Show("You cannot delete the Note Set that you are currently using", "Deletion error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            newNoteDir = new DirectoryInfo(globalVars.FullLXXNotesPath);
                            listOfBookDirs = newNoteDir.GetDirectories();
                            isFound = false;
                            foreach (DirectoryInfo diDir in listOfBookDirs)
                            {
                                fullPath = diDir.FullName;
                                srNote = new StreamReader(fullPath + @"\NoteSetName.txt");
                                foundNoteName = srNote.ReadLine();
                                srNote.Close();
                                if (String.Compare(newNoteName, foundNoteName) == 0)
                                {
                                    isFound = true;
                                    break;
                                }
                            }
                            if (isFound)
                            {
                                newNoteDir = new DirectoryInfo(fullPath);
                                if (newNoteDir.Exists) newNoteDir.Delete(true);
                            }
                            break;
                    }
                    break;
                case 4:
                    break;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void removeAllNotes()
        {
            int bIdx, cIdx, vIdx, noOfBooks, noOfChapters, noOfVerses;
            classMTBook currentMTBook;
            classMTChapter currentMTChapter;
            classMTVerse currentMTVerse;
            classLXXBook currentLXXBook;
            classLXXChapter currentLXXChapter;
            classLXXVerse currentLXXVerse;

            switch (activeVersion)
            {
                case 0:
                    noOfBooks = globalVars.NoOfMTBooks;
                    for (bIdx = 0; bIdx < noOfBooks; bIdx++)
                    {
                        globalVars.MtBookList.TryGetValue(bIdx, out currentMTBook);
                        noOfChapters = currentMTBook.NoOfChaptersInBook;
                        for (cIdx = 0; cIdx < noOfChapters; cIdx++)
                        {
                            currentMTChapter = currentMTBook.getChapterBySequence(cIdx);
                            noOfVerses = currentMTChapter.NoOfVersesInChapter;
                            for (vIdx = 0; vIdx < noOfVerses; vIdx++)
                            {
                                currentMTVerse = currentMTChapter.getVerseBySequence(vIdx);
                                if ((currentMTVerse.NoteText != null) && (currentMTVerse.NoteText.Length > 0)) currentMTVerse.NoteText = "";
                            }
                        }
                    }
                    ((RichTextBox)globalVars.getGroupedControl( globalVars.RichtextBoxCode, 2)).Text = "";
                    break;
                case 1:
                    noOfBooks = globalVars.NoOfLXXBooks;
                    for (bIdx = 0; bIdx < noOfBooks; bIdx++)
                    {
                        globalVars.LxxBookList.TryGetValue(bIdx, out currentLXXBook);
                        noOfChapters = currentLXXBook.NoOfChaptersInBook;
                        for (cIdx = 0; cIdx < noOfChapters; cIdx++)
                        {
                            currentLXXChapter = currentLXXBook.getChapterBySequence(cIdx);
                            noOfVerses = currentLXXChapter.NoOfVersesInChapter;
                            for (vIdx = 0; vIdx < noOfVerses; vIdx++)
                            {
                                currentLXXVerse = currentLXXChapter.getVerseBySequence(vIdx);
                                if ((currentLXXVerse.NoteText != null) && (currentLXXVerse.NoteText.Length > 0)) currentLXXVerse.NoteText = "";
                            }
                        }
                    }
                    ((RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 3)).Text = "";
                    break;
            }
        }
    }
}
