using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OldTestamentStudent
{
    public class classNote
    {
        /************************************************
          *                                              *
          *            Key Global references             *
          *            =====================             *
          *                                              *
          ************************************************/

        classGlobal globalVars;
        classMTText mtTextHandler;
        classLXXText lxxTextHandler;
        classRegistry regClass;

        private delegate void performPageTitleModification(TabPage targetPage, String groupName);

        private void modifyPageTitle(TabPage targetPage, String groupName)
        {
            targetPage.Text = "Notes: " + groupName;
        }

        public void initialiseNotes(classGlobal inConfig, classMTText inMT, classLXXText inLXX, classRegistry inReg)
        {
            globalVars = inConfig;
            mtTextHandler = inMT;
            lxxTextHandler = inLXX;
            regClass = inReg;
        }

        public void processOnStartup()
        {
            /*=====================================================================================================*
             *                                                                                                     *
             *                                       processOnStartup                                              *
             *                                       ================                                              *
             *                                                                                                     *
             *  This will be called when the application starts.  It will function as follows:                     *
             *                                                                                                     *
             *    1. It will check for the presence of the notes directory with the name of the notes group        *
             *       currently being used (i.e. in the registry);                                                  *
             *    2. If any stored notes are found:                                                                *
             *       a) each note in turn will be applied to the relevant class store for the verse;               *
             *       b) processing will then pass to the next note, so that each one is applied in turn;           *
             *                                                                                                     *
             *=====================================================================================================*/
            bool isFirst = true;
            int bookId, dirNo = -1, maxDirNo = -1;
            String newNote, fullPath, dirName;
            DirectoryInfo storeDir, subDirStore;
            String[] fileComponents;
            Char[] fileSplit = { '-' };
            DirectoryInfo[] listOfDirectories, subDirList;
            FileInfo[] notesFiles;
            StreamReader srNote, srNoteName;
            StreamWriter swNoteName;
            classMTBook currentMTBook = null;
            classMTChapter currentMTChapter;
            classMTVerse currentMTVerse;
            classLXXBook currentLXXBook = null;
            classLXXChapter currentLXXChapter;
            classLXXVerse currentLXXVerse;
            TabPage currentNotesPage;

            // Before we do anything else, we need to find out whether we have stored any notes
            fullPath = globalVars.FullMTNotesPath;
            // BTW, document the notes tab with the Set name
            storeDir = new DirectoryInfo(fullPath);
            if (storeDir.Exists)  // No stored data; nothing to do
            {
                listOfDirectories = storeDir.GetDirectories();
                if (listOfDirectories.Length > 0)
                {
                    // This will be one directory for each notes set created (including the default)
                    foreach (DirectoryInfo subDir in listOfDirectories)
                    {
                        // First, convert to meaningful integer
                        dirNo = Convert.ToInt32(subDir.Name);
                        if (dirNo > maxDirNo) maxDirNo = dirNo;
                        if ((globalVars.CurrentMTNoteRef == -1) && isFirst)
                        {
                            isFirst = false;
                            globalVars.CurrentMTNoteRef = dirNo;
                        }
                        if (dirNo == globalVars.CurrentMTNoteRef)
                        {
                            if (File.Exists(subDir.FullName + @"\NoteSetName.txt"))
                            {
                                srNoteName = new StreamReader(subDir.FullName + @"\NoteSetName.txt");
                                newNote = srNoteName.ReadLine();
                            }
                            else newNote = "Default";
                            globalVars.MtNotesName = newNote;
                            currentNotesPage = (TabPage)globalVars.getGroupedControl(globalVars.TabPageCode, 2);
                            currentNotesPage.Invoke(new performPageTitleModification(modifyPageTitle), currentNotesPage, newNote);
                            subDirStore = new DirectoryInfo(subDir.FullName);
                            subDirList = subDirStore.GetDirectories();
                            foreach (DirectoryInfo subSubDir in subDirList)
                            {
                                notesFiles = subSubDir.GetFiles();
                                dirName = subSubDir.Name;
                                bookId = Convert.ToInt32(dirName);
                                foreach (FileInfo noteFile in notesFiles)
                                {
                                    if (globalVars.MtBookList.ContainsKey(bookId)) globalVars.MtBookList.TryGetValue(bookId, out currentMTBook);
                                    if (currentMTBook == null) continue;
                                    newNote = noteFile.Name.Substring(0, noteFile.Name.Length - 5);
                                    fileComponents = newNote.Split(fileSplit);
                                    currentMTChapter = currentMTBook.getChapterByChapterNo(fileComponents[0]);
                                    currentMTVerse = currentMTChapter.getVerseByVerseNo(fileComponents[1]);
                                    srNote = new StreamReader(noteFile.FullName);
                                    currentMTVerse.NoteText = srNote.ReadToEnd();
                                    srNote.Close();
                                    srNote.Dispose();
                                }
                            }
                        }
                    }
                    globalVars.MaxMTNoteRef = maxDirNo;
                }
                else
                {
                    dirNo = 0;
                    maxDirNo = 0;
                    globalVars.CurrentMTNoteRef = 0;
                    Directory.CreateDirectory(storeDir.FullName + @"\0000000000");
                    swNoteName = new StreamWriter(storeDir.FullName + @"\0000000000\NoteSetName.txt");
                    swNoteName.WriteLine("Default");
                    swNoteName.Close();
                    globalVars.MtNotesName = "Default";
                }
            }
            else
            {
                storeDir.Create();
                dirNo = 0;
                maxDirNo = 0;
                globalVars.CurrentMTNoteRef = 0;
                Directory.CreateDirectory(storeDir.FullName + @"\0000000000");
                swNoteName = new StreamWriter(storeDir.FullName + @"\0000000000\NoteSetName.txt");
                swNoteName.WriteLine("Default");
                swNoteName.Close();
                globalVars.MtNotesName = "Default";
            }

            // repeat for LXX
            isFirst = true;
            maxDirNo = -1;
            fullPath = globalVars.FullLXXNotesPath;
            // BTW, document the notes tab with the Set name
            storeDir = new DirectoryInfo(fullPath);
            if (storeDir.Exists)  // No stored data; nothing to do
            {
                listOfDirectories = storeDir.GetDirectories();
                if (listOfDirectories.Length > 0)
                {
                    // This will be one directory for each notes set created (including the default)
                    foreach (DirectoryInfo subDir in listOfDirectories)
                    {
                        // First, convert to meaningful integer
                        dirNo = Convert.ToInt32(subDir.Name);
                        if (dirNo > maxDirNo) maxDirNo = dirNo;
                        if ((globalVars.CurrentLXXNoteRef == -1) && isFirst)
                        {
                            isFirst = false;
                            globalVars.CurrentLXXNoteRef = dirNo;
                        }
                        if (dirNo == globalVars.CurrentLXXNoteRef)
                        {
                            if (File.Exists(subDir.FullName + @"\NoteSetName.txt"))
                            {
                                srNoteName = new StreamReader(subDir.FullName + @"\NoteSetName.txt");
                                newNote = srNoteName.ReadLine();
                            }
                            else newNote = "Default";
                            globalVars.LxxNotesName = newNote;
                            currentNotesPage = (TabPage)globalVars.getGroupedControl(globalVars.TabPageCode, 5);
                            currentNotesPage.Invoke(new performPageTitleModification(modifyPageTitle), currentNotesPage, newNote);
                            subDirStore = new DirectoryInfo(subDir.FullName);
                            subDirList = subDirStore.GetDirectories();
                            foreach (DirectoryInfo subSubDir in subDirList)
                            {
                                notesFiles = subSubDir.GetFiles();
                                dirName = subSubDir.Name;
                                bookId = Convert.ToInt32(dirName);
                                foreach (FileInfo noteFile in notesFiles)
                                {
                                    if (globalVars.LxxBookList.ContainsKey(bookId)) globalVars.LxxBookList.TryGetValue(bookId, out currentLXXBook);
                                    if (currentLXXBook == null) continue;
                                    newNote = noteFile.Name.Substring(0, noteFile.Name.Length - 5);
                                    fileComponents = newNote.Split(fileSplit);
                                    currentLXXChapter = currentLXXBook.getChapterByChapterNo(fileComponents[0]);
                                    currentLXXVerse = currentLXXChapter.getVerseByVerseNo(fileComponents[1]);
                                    srNote = new StreamReader(noteFile.FullName);
                                    currentLXXVerse.NoteText = srNote.ReadToEnd();
                                    srNote.Close();
                                    srNote.Dispose();
                                }
                            }
                        }
                    }
                    globalVars.MaxLXXNoteRef = maxDirNo;
                }
                else
                {
                    dirNo = 0;
                    maxDirNo = 0;
                    globalVars.CurrentLXXNoteRef = 0;
                    Directory.CreateDirectory(storeDir.FullName + @"\0000000000");
                    swNoteName = new StreamWriter(storeDir.FullName + @"\0000000000\NoteSetName.txt");
                    swNoteName.WriteLine("Default");
                    swNoteName.Close();
                    globalVars.LxxNotesName = "Default";
                }
            }
            else
            {
                storeDir.Create();
                dirNo = 0;
                maxDirNo = 0;
                globalVars.CurrentLXXNoteRef = 0;
                Directory.CreateDirectory(storeDir.FullName + @"\0000000000");
                swNoteName = new StreamWriter(storeDir.FullName + @"\0000000000\NoteSetName.txt");
                swNoteName.WriteLine("Default");
                swNoteName.Close();
                globalVars.LxxNotesName = "Default";
            }
        }

        public String expandedNumberReference( int sourceNumber)
        {
            /*=====================================================================================================*
             *                                                                                                     *
             *                                      expandedNumberReference                                        *
             *                                      =======================                                        *
             *                                                                                                     *
             *  Converts an integer value into a zero-padded string.  So, for example, the source number, 265,     *
             *    will be converted to the string "0000000265".  This string will form the name of the folder      *
             *    containing the relevant note set.                                                                *
             *                                                                                                     *
             *  The logic of this decision is that the maximum integer value is 2,147,483,647 (i.e. 10 digits) and *
             *    it is considered highly unlikely that anyone will create that number of separate note sets.      *
             *                                                                                                     *
             *=====================================================================================================*/
            const int maxLength = 10;

            int stringLength = 0;
            String workArea = "0000000000", integerAsString;

            integerAsString = sourceNumber.ToString();
            stringLength = integerAsString.Length;
            workArea = workArea.Substring(0, maxLength - stringLength) + integerAsString;
            return workArea;
        }

        public void insertTextIntoNote(String textToInsert)
        {
            int currPstn = 0, versionCode;
            String fullNote = "", beforeCursor, afterCursor;
            RichTextBox rtxtNotes = null;

            versionCode = ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 1)).SelectedIndex;
            switch (versionCode)
            {
                case 0: rtxtNotes = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 2); break;
                case 1: rtxtNotes = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 3); break;
            }
            fullNote = rtxtNotes.Text;
            currPstn = rtxtNotes.SelectionStart;
            if ((fullNote == null) || (fullNote.Length == 0))
            {
                rtxtNotes.Text = textToInsert;
                rtxtNotes.SelectionStart = textToInsert.Length;
                return;
            }
            if (currPstn == 0)
            {
                rtxtNotes.Text = textToInsert + fullNote;
                rtxtNotes.SelectionStart = textToInsert.Length;
                return;
            }
            if (currPstn == fullNote.Length - 1)
            {
                rtxtNotes.Text = fullNote + textToInsert;
                rtxtNotes.SelectionStart = currPstn + textToInsert.Length;
                return;
            }
            beforeCursor = fullNote.Substring(0, currPstn);
            afterCursor = fullNote.Substring(currPstn);
            rtxtNotes.Text = beforeCursor + textToInsert + afterCursor;
            rtxtNotes.SelectionStart = currPstn + textToInsert.Length;
        }

        public void processOldNote(int versionCode)
        {
            /*=====================================================================================================*
             *                                                                                                     *
             *                                       processOldNote                                                *
             *                                       ==============                                                *
             *                                                                                                     *
             *  This will be called when the notes area has lost focus - i.e. when the user has clicked anywhere   *
             *    outside the notes area.  This will include the case where he or she selects a new verse.         *
             *                                                                                                     *
             *  It will also be called when ending the application.                                                *
             *                                                                                                     *
             *  It will perform two tasks:                                                                         *
             *    1  Save the discrete note the current folder                                                     *
             *    2  Update (or create) a copy of the note in the class instance for the verse.                    *
             *                                                                                                     *
             *  The purpose of step 1 will be to provide a durable copy that will be available in the event of     *
             *    unplanned application exit, such as a loss  of power.  These will constitute the saved version,  *
             *    which will be loaded on system startup and retained until changed by the user and application.   *
             *                                                                                                     *
             *  Parameter:                                                                                         *
             *  =========                                                                                          *
             *                                                                                                     *
             *  As with many methods, the integer code tells the method whether the mote is for MT or LXX.         *
             *                                                                                                     *
             *=====================================================================================================*/

            int bkVal, chVal, vsVal, dirNo;
            String newNote, previousNote = "", chapter = "", verse = "", noteGroupName = "", notesName, fullPath = "";
            DirectoryInfo storeDir;
            StreamWriter swNote;
            classMTBook currentMTBook;
            classMTChapter currentMTChapter;
            classMTVerse currentMTVerse = null;
            classLXXBook currentLXXBook;
            classLXXChapter currentLXXChapter;
            classLXXVerse currentLXXVerse = null;
            RichTextBox rtxtNotes;
            ComboBox cbBook = null, cbChapter = null, cbVerse = null;

            switch (versionCode)
            {
                case 1:
                    // Make sure we can see the note
                    rtxtNotes = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 2);  // MT note area
                    // And the current book, chapter and verse references
                    cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 0);
                    cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 1);
                    cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 2);
                    break;
                case 2:
                    // Make sure we can see the note
                    rtxtNotes = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 3);  // LXX note area
                    // And the current book, chapter and verse references
                    cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
                    cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4);
                    cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 5);
                    break;
            }
            // Get the class instance storing the verse data
            bkVal = cbBook.SelectedIndex;
            chVal = cbChapter.SelectedIndex;
            vsVal = cbVerse.SelectedIndex;
            switch (versionCode)
            {
                case 1:
                    globalVars.MtBookList.TryGetValue(bkVal, out currentMTBook);
                    currentMTChapter = currentMTBook.getChapterBySequence(chVal);
                    currentMTVerse = currentMTChapter.getVerseBySequence(vsVal);
                    chapter = currentMTBook.getChapterNoBySequence(chVal);
                    verse = currentMTChapter.getVerseNoBySequence(vsVal);
                    dirNo = globalVars.CurrentMTNoteRef;
                    noteGroupName = expandedNumberReference(dirNo);
                    fullPath = globalVars.FullMTNotesPath + @"\" + noteGroupName;
                    break;
                case 2:
                    globalVars.LxxBookList.TryGetValue(bkVal, out currentLXXBook);
                    currentLXXChapter = currentLXXBook.getChapterBySequence(chVal);
                    currentLXXVerse = currentLXXChapter.getVerseBySequence(vsVal);
                    chapter = currentLXXBook.getChapterNoBySequence(chVal);
                    verse = currentLXXChapter.getVerseNoBySequence(vsVal);
                    dirNo = globalVars.CurrentLXXNoteRef;
                    noteGroupName = expandedNumberReference(dirNo);
                    fullPath = globalVars.FullLXXNotesPath + @"\" + noteGroupName;
                    break;
            }
            // Before we do anything else, we need to find out whether we have stored any temporary notes
            storeDir = new DirectoryInfo(fullPath);
            if (!storeDir.Exists) storeDir.Create();
            fullPath = fullPath + @"\" + bkVal.ToString();
            storeDir = new DirectoryInfo(fullPath);
            if (!storeDir.Exists) storeDir.Create();
            notesName = chapter + "-" + verse + ".note";
            fullPath = fullPath + @"\" + notesName;
            // Whether we do anything depends on whether the new note is a change from the old
            switch (versionCode)
            {
                case 1: previousNote = currentMTVerse.NoteText; break;
                case 2: previousNote = currentLXXVerse.NoteText; break;
            }
            newNote = ((RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, versionCode + 1)).Text;
            if ((previousNote == null) || (previousNote.Length == 0))
            {
                if ((newNote == null) || (newNote.Length == 0)) return;
            }
            else
            {
                if (String.Compare(previousNote, newNote) == 0) return;
            }
            swNote = new StreamWriter(fullPath);
            swNote.Write(newNote);
            swNote.Close();
            swNote.Dispose();
            switch (versionCode)
            {
                case 1: currentMTVerse.NoteText = newNote; break;
                case 2: currentLXXVerse.NoteText = newNote; break;
            }
        }

        public void processNewNote(int versionCode)
        {
            /*=====================================================================================================*
             *                                                                                                     *
             *                                         processNewNote                                              *
             *                                         ==============                                              *
             *                                                                                                     *
             *  This will be called when a new value for cbVerse is selected (or after cbBooks and/or cbChapters   *
             *    has changed.                                                                                     *
             *                                                                                                     *
             *=====================================================================================================*/

            int bkVal, chVal, vsVal;
            String previousNote = "", noteForNewVerse = "", chapter, verse, bookName;
            classMTBook currentMTBook;
            classMTChapter currentMTChapter;
            classMTVerse currentMTVerse = null;
            classLXXBook currentLXXBook;
            classLXXChapter currentLXXChapter;
            classLXXVerse currentLXXVerse = null;
            RichTextBox rtxtNotes = null;
            ComboBox cbBook = null, cbChapter = null, cbVerse = null;

            switch (versionCode)
            {
                case 1:
                    // Make sure we can see the note
                    rtxtNotes = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 2);
                    // And the current book, chapter and verse references
                    cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 0);
                    cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 1);
                    cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 2);
                    break;
                case 2:
                    // Make sure we can see the note
                    rtxtNotes = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 3);
                    // And the current book, chapter and verse references
                    cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
                    cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4);
                    cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 5);
                    break;
            }
            // Get the class instance storing the verse data
            if (cbBook == null) return;
            bkVal = cbBook.SelectedIndex;
            chVal = cbChapter.SelectedIndex;
            vsVal = cbVerse.SelectedIndex;
            if (chVal < 0) return;
            switch (versionCode)
            {
                case 1:
                    globalVars.MtBookList.TryGetValue(bkVal, out currentMTBook);
                    currentMTChapter = currentMTBook.getChapterBySequence(chVal);
                    currentMTVerse = currentMTChapter.getVerseBySequence(vsVal);
                    bookName = currentMTBook.BookName;
                    chapter = currentMTBook.getChapterNoBySequence(chVal);
                    verse = currentMTChapter.getVerseNoBySequence(vsVal);
                    ((TabPage)globalVars.getGroupedControl(globalVars.TabPageCode, 2)).Text = "Notes: " + globalVars.MtNotesName + " - " + bookName + " " + chapter + ":" + verse;
                    // Whether we do anything depends on whether the new note is a change from the old
                    noteForNewVerse = currentMTVerse.NoteText;
                    break;
                case 2:
                    globalVars.LxxBookList.TryGetValue(bkVal, out currentLXXBook);
                    currentLXXChapter = currentLXXBook.getChapterBySequence(chVal);
                    currentLXXVerse = currentLXXChapter.getVerseBySequence(vsVal);
                    bookName = currentLXXBook.CommonName;
                    chapter = currentLXXBook.getChapterNoBySequence(chVal);
                    verse = currentLXXChapter.getVerseNoBySequence(vsVal);
                    ((TabPage)globalVars.getGroupedControl(globalVars.TabPageCode, 5)).Text = "Notes: " + globalVars.LxxNotesName + " - " + bookName + " " + chapter + ":" + verse;
                    // Whether we do anything depends on whether the new note is a change from the old
                    noteForNewVerse = currentLXXVerse.NoteText;
                    break;
            }
            previousNote = rtxtNotes.Text;
            if ((noteForNewVerse == null) || (noteForNewVerse.Length == 0))
            {
                if ((previousNote == null) || (previousNote.Length == 0)) return;
                else noteForNewVerse = "";
            }
            else
            {
                if (String.Compare(noteForNewVerse, previousNote) == 0) return;
            }
            rtxtNotes.Text = noteForNewVerse;
        }

        public void moveNotesAndOthers(FolderBrowserDialog dlgNotes)
        {
            /**********************************************************************************************
             *                                                                                            *
             *                                  moveNotesAndOthers                                        *
             *                                  ==================                                        *
             *                                                                                            *
             *  The aim of this method is to move the current location of all transient files (i.e. Help, *
             *    Notes and Source files) to a new location.                                              *
             *                                                                                            *
             *  The location of the entire fileset to be moved is found in globalVars.baseDirectory.      *
             *  The target folder name will be GBSFiles.  Since this is different from the initial        *
             *    directory name, the base folder (initially "Files") will need to be extracted.          *
             *                                                                                            *
             **********************************************************************************************/
/*            int idx;
            String startingPath, targetFolder = "GBSFiles", endingPath, subDirectory;
            DirectoryInfo diTarget;
            String[] subDirectoryName = { "Source", "Notes", "Help" };

            startingPath = globalVars.BaseDirectory;
            if (startingPath[startingPath.Length - 1] == '\\') startingPath = startingPath.Substring(0, startingPath.Length - 1);
            dlgNotes.SelectedPath = startingPath;
            dlgNotes.Description = "Location of Folder containing Source files, Notes and Help\nThe folders, Source, Notes and Help and the " +
                "parent folder, GBSFiles, will be created *below* the folder that you select or create.";
            if (dlgNotes.ShowDialog() == DialogResult.OK)
            {
                regClass.openRegistry();
                endingPath = dlgNotes.SelectedPath;
                if (String.Compare(endingPath, startingPath) == 0) return;  // i.e. no change
                subDirectory = endingPath + @"\" + targetFolder;
                diTarget = new DirectoryInfo(subDirectory);
                if (!diTarget.Exists) diTarget.Create();
                endingPath = subDirectory;
                for (idx = 0; idx <= 2; idx++)
                {
                    createAndPopulateFolder(startingPath + @"\" + subDirectoryName[idx], endingPath + @"\" + subDirectoryName[idx], subDirectoryName[idx],
                                                idx);
                }
                regClass.closeKeys();
            } */
        }

        private void createAndPopulateFolder(String fromDirectory, String toDirectory, String folderName, int actionCode)
        {
/*            String fileName, fullFileName, finalFileName;
            DirectoryInfo diTarget, diSourceContents, diTemp;
            FileInfo[] directoryContents, sourceContents, notesBottomLevelFiles;
            DirectoryInfo[] notesSubDirectories, notesItems;

            diTarget = new DirectoryInfo(toDirectory);
            if (diTarget.Exists)
            {
                if (MessageBox.Show("The folder " + toDirectory + "already exist\nIf you continue, any data currently in this folder will be erased\n\n" +
                    "Select \"OK\" to continue.", folderName + " Folder", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    directoryContents = diTarget.GetFiles();
                    foreach (FileInfo fiSingleFile in directoryContents)
                    {
                        fiSingleFile.Delete();
                    }
                }
                else return;
            }
            else
            {
                diTarget.Create();
            }
            diSourceContents = new DirectoryInfo(fromDirectory);
            sourceContents = diSourceContents.GetFiles();
            fileName = "";
            foreach (FileInfo fiSourceFile in sourceContents)
            {
                fileName = toDirectory + @"\" + fiSourceFile.Name;
                fiSourceFile.CopyTo(fileName);
            }
            if (actionCode == 1) // i.e. Notes
            {
                notesSubDirectories = diSourceContents.GetDirectories();
                foreach (DirectoryInfo diSubSource in notesSubDirectories)
                {
                    fileName = toDirectory + @"\" + diSubSource.Name;
                    diTemp = new DirectoryInfo(fileName);
                    if (!diTemp.Exists) diTemp.Create();
                    notesItems = diSubSource.GetDirectories();
                    foreach (DirectoryInfo fiFinalDir in notesItems)
                    {
                        fullFileName = fileName + @"\" + fiFinalDir.Name;
                        diTemp = new DirectoryInfo(fullFileName);
                        if (!diTemp.Exists) diTemp.Create();
                        notesBottomLevelFiles = fiFinalDir.GetFiles();
                        foreach (FileInfo fiFinalFile in notesBottomLevelFiles)
                        {
                            finalFileName = fiFinalFile.Name;
                            finalFileName = fullFileName + @"\" + fiFinalFile.Name;
                            fiFinalFile.CopyTo(finalFileName);
                        }
                    }
                }
            } */
        }
    }
}
