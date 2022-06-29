using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace OldTestamentStudent
{
    public class classLXXText
    {
        const char zeroWidthSpace = '\u200b', zeroWidthNonJoiner = '\u200d';

        int noOfStoredBooks = 0;

        ComboBox cbBooks;

        classGlobal globalVars;
        frmProgress progressForm;
        classHistory historyProcesses;
        classGkLexicon lexicon;

        /*===============================================================================================*
         *                                                                                               *
         *                                        bookList                                               *
         *                                        --------                                               *
         *                                                                                               *
         *  Stores references to book names and associated data.                                         *
         *     Key:     Integer sequence (starting from zero);                                           *
         *     Value:   The class instance for Hebrew books                                              *
         *                                                                                               *
         *===============================================================================================*/
        SortedDictionary<int, classLXXBook> bookList = new SortedDictionary<int, classLXXBook>();

//        Thread initialisationThread;
        private delegate void performProgressAdvance(String primaryMessage, String secondaryMessage, bool useSecondary);
//        private delegate void performComboBoxUpdate(ComboBox targetCB, String comboItem);
//        private delegate void performComboBoxSelection(ComboBox targetCB, int selectionIndex);
//        private delegate void performClearComboBox(ComboBox targetCB);
        private delegate void performNewFormTitle(Form formId, String newName);

        public void initialiseText(classGlobal inGlobal, frmProgress inForm, classHistory inHistory, classGkLexicon inLex)
        {
            globalVars = inGlobal;
            progressForm = inForm;
            historyProcesses = inHistory;
            lexicon = inLex;
        }

        private void updateProgress(String mainMessage, String secondaryMessage, bool useSecondary)
        {
            progressForm.incrementProgress(mainMessage, secondaryMessage, useSecondary);
        }

/*        private void addComboItem(ComboBox targetCB, String cbItem)
        {
            targetCB.Items.Add(cbItem);
        }

        private void selectComboItem(ComboBox targetCB, int selectionIndex)
        {
            if (targetCB.Items.Count > selectionIndex) targetCB.SelectedIndex = selectionIndex;
        }

        private void clearComboBox(ComboBox targetCB)
        {
            targetCB.Items.Clear();
        } */

        private void createNewFormTitle(Form formId, String newName)
        {
            formId.Text = newName;
        }

        public void loadText()
        {
            /*---------------------------------------------------------------------------------------------------*
             *                                                                                                   *
             *   Step 1: data load for LXX data                                                                 *
             *   =======                                                                                         *
             *                                                                                                   *
             *   Step 1 will handle information about Septuagint book names and the files containing text data.  *
             *   It is more complex than the equivalent step for NT data because the LXX text is held in         *
             *   files (one for each book).                                                                      *
             *                                                                                                   *
             *   Note also that LXX chapters and verses are not always logically structured and we need to cope  *
             *   with the possibility of:                                                                        *
             *                                                                                                   *
             *     - chapters out of sequence;                                                                   *
             *     - pre-chapter verses (indexed as verse zero in our data;                                      *
             *     - verses identified as e.g. 12a, 12b and so on.                                               *
             *                                                                                                   *
             *---------------------------------------------------------------------------------------------------*/

            int bdx;
            String titlesFileName, fileBuffer, chapterRef, prevChapRef = "?", verseRef, prevVerseRef = "?", fullFileName;
            Char[] fieldSeparator = { '\t' };
            String[] fields, lineContents;
            StreamReader srBookNames, srLxxText;
            classLXXBook currentBook = null;
            classLXXChapter currentChapter = null, prevChapter;
            classLXXVerse currentVerse = null, previousVerse = null;
            classLXXWord currentWord = null;

//            initialisationThread = progressThread;
            progressForm.Invoke(new performProgressAdvance(updateProgress), "Loading the names of LXX books", "", false);
            // Firstly, get a simple list of book names - primarily for references
            titlesFileName = globalVars.FullLXXTitleFile;
            srBookNames = new StreamReader(titlesFileName);
            fileBuffer = srBookNames.ReadLine();
            while (fileBuffer != null)
            {
                if (fileBuffer[0] != ';')
                {
                    fields = fileBuffer.Split(fieldSeparator);
                    currentBook = new classLXXBook();
                    currentBook.ShortName = fields[0];
                    currentBook.CommonName = fields[1];
                    currentBook.LxxName = fields[2];
                    currentBook.FileName = fields[3];
                    currentBook.Category = Convert.ToInt32(fields[4]);
                    bookList.Add(noOfStoredBooks, currentBook);
                    noOfStoredBooks++;
                }
                fileBuffer = srBookNames.ReadLine();
            }
            srBookNames.Close();
            srBookNames.Dispose();

            /*---------------------------------------------------------------------------------------------------*
             *                                                                                                   *
             *   Step 2:                                                                                         *
             *   ======                                                                                          *
             *                                                                                                   *
             *   Now load the relevant LXX text data.                                                            *
             *                                                                                                   *
             *---------------------------------------------------------------------------------------------------*/

            // Now for the  text data. Let's process and store it
            cbBooks = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
            fullFileName = globalVars.FullLXXTextFolder;
            for (bdx = 0; bdx < noOfStoredBooks; bdx++)
            {
                bookList.TryGetValue(bdx, out currentBook);
                progressForm.Invoke(new performProgressAdvance(updateProgress), "Loading source text for the Septuagint", currentBook.CommonName, true);
//                cbBooks.Invoke(new performComboBoxUpdate(addComboItem), cbBooks, currentBook.CommonName);
                currentChapter = null;
                currentVerse = null;
                prevChapRef = "?";
                srLxxText = new StreamReader(fullFileName + @"\" + currentBook.FileName);
                fileBuffer = srLxxText.ReadLine();
                while (fileBuffer != null)
                {
                    /**************************************************************************************
                     *                                                                                    *
                     *  Split the line as follows:                                                        *
                     *                                                                                    *
                     *   Field                    Contents                                                *
                     *   -----  ------------------------------------------------------------------------  *
                     *     1	Chapter number                                                            *
                     *     2	Verse number (note: may = 0 or 12b)                                       *
                     *     3	Initial Parse code                                                        *
                     *     4	Detailed Parse code                                                       *
                     *     5	A unique grammatical identifier                                           *
                     *     6	Word as it is to be displayed in the text                                 *
                     *     7	Word a) all lower case, b) stripped of accents and related furniture      *
                     *     8	Word, as in field 7 but also with breathings and iota subscripts removed  *
                     *     9	Immediate root of word in field 6                                         *
                     *     10	Pre-word characters                                                       *
                     *     11	Post-word non-punctuation characters                                      *
                     *     12	Punctuation                                                               *
                     *     13	Transliterated version of field 6                                         *
                     *     14+	Transliteration of root (with prefixed prepositions separated             *
                     *                                                                                    *
                     *  However, fields 1 and 2 are as supplied by the source file.  In addition, we will *
                     *  create a simple, sequential index for chapters and verses.  This will allow for:  *
                     *  a) out-of-sequence chapters (in a few books, there are gaps and, even, chapters   *
                     *     transposed;                                                                    *
                     *  b) verses that include text as well as digits (e.g. 19b);                         *
                     *  c) unnumbered verses (in our data, given the index 0)                             *
                     *                                                                                    *
                     **************************************************************************************/
                    lineContents = fileBuffer.Split(fieldSeparator);
                    chapterRef = lineContents[0];
                    // Handle the chapter
                    if (String.Compare(chapterRef, prevChapRef) != 0)
                    {
                        prevChapter = currentChapter;
                        currentChapter = currentBook.addNewChapterToBook(chapterRef);
                        if (prevChapter != null) prevChapter.NextChapter = currentChapter;
                        currentChapter.PreviousChapter = prevChapter;
                        currentChapter.BookNo = bdx;
                        currentChapter.ChapterRef = chapterRef;
                        currentChapter.ChapterNo = currentBook.NoOfChaptersInBook - 1;
                        prevChapRef = chapterRef;
                        prevVerseRef = "?";
                    }
                    // Handle the verse
                    verseRef = lineContents[1];
                    if (String.Compare(verseRef, prevVerseRef) != 0)
                    {
                        currentVerse = currentChapter.addVerseToChapter(verseRef);
                        if (previousVerse != null)
                        {
                            previousVerse.NextVerse = currentVerse;
                        }
                        currentVerse.PreviousVerse = previousVerse;
                        prevVerseRef = verseRef;
                        previousVerse = currentVerse;
                        currentVerse.ChapSeq = currentChapter.ChapterNo;
                        currentVerse.ChapRef = currentChapter.ChapterRef;
                        currentVerse.VerseSeq = currentChapter.NoOfVersesInChapter - 1;
                        currentVerse.VerseRef = currentChapter.getVerseNoBySequence(currentVerse.VerseSeq);
                    }
                    currentWord = currentVerse.addWordToVerse();
                    currentWord.CatString = lineContents[2];
                    currentWord.ParseString = lineContents[3];
                    currentWord.UniqueValue = lineContents[4];
                    currentWord.TextWord = lineContents[5];
                    currentWord.AccentlessTextWord = lineContents[6];
                    currentWord.BareTextWord = lineContents[7];
                    currentWord.RootWord = lineContents[8];
                    currentWord.Punctuation = lineContents[11];
                    currentWord.PreWordChars = lineContents[9];
                    currentWord.PostWordChars = lineContents[10];
                    fileBuffer = srLxxText.ReadLine();
                }
                srLxxText.Close();
                srLxxText.Dispose();
            }
            globalVars.LxxBookList = bookList;
            globalVars.NoOfLXXBooks = noOfStoredBooks;
            globalVars.LxxCurrentBookIndex = 0;
            globalVars.LxxCurrentChapter = "";
//            cbBooks.Invoke(new performComboBoxSelection(selectComboItem), cbBooks, 0);
        }

        public void respondToNewBook()
        {
//            bool isInThread = false;
            int bookId, cdx, noOfChapters;
            String chapterRef;
            ComboBox cbBook, cbChapter;
            SortedDictionary<int, classLXXBook> bookList;
            classLXXBook currentBook;

            //            if (initialisationThread.IsAlive) isInThread = true;
            cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
            bookList = globalVars.LxxBookList;
            noOfChapters = globalVars.NoOfLXXBooks;
            cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4);
            bookId = cbBook.SelectedIndex;
            bookList.TryGetValue(bookId, out currentBook);
            noOfChapters = currentBook.NoOfChaptersInBook;
            cbChapter.Items.Clear();
            for (cdx = 0; cdx < noOfChapters; cdx++)
            {
                chapterRef = currentBook.getChapterNoBySequence(cdx);
                cbChapter.Items.Add(chapterRef);
            }
            if (cbChapter.Items.Count > 0) cbChapter.SelectedIndex = 0;
            //            if (isChapUpdateActive) displayChapter(bookId, 1);
        }

        public void displayChapter(int bookIdx, String chapIdx)
        {
            /*================================================================================================*
             *                                                                                                *
             *                                        displayChapter                                          *
             *                                        ==============                                          *
             *                                                                                                *
             *  Controls the display of a specified chapter                                                   *
             *                                                                                                *
             *  Parameters:                                                                                   *
             *    bookIdx:      the book index (a zero based index)                                           *
             *    chapIdx:      chapter number (real chapter, which must be converted to it's equivalent      *
             *                    sequence number                                                             *
             *                                                                                                *
             *================================================================================================*/

            int idx, cdx, wdx, noOfChapters, noOfVerses, noOfWords;
            String newBookName, displayString = "", realChapNo, realVNo;
            classLXXBook currentBook;
            classLXXChapter currentChapter;
            classLXXVerse currentVerse;
            classLXXWord currentWord;
            RichTextBox targetTextArea;
            ComboBox cbChapter, cbVerse;
            ComboBox cbBook;
            Font engFont, greekFont;
            Color backgroundColour;

            greekFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(3, 2), globalVars.getDefinedStyleByIndex(3, 2), globalVars.getTextSize(3, 2));
            engFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(3, 1), globalVars.getDefinedStyleByIndex(3, 1), globalVars.getTextSize(3, 1));
            backgroundColour = globalVars.getColourSetting(1, 0);
            // Get the Rich Text area in which the text occurs
            targetTextArea = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 1);
            // Get the combo boxes for the Book, Chapter and Verse
            cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
            cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4);
            cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 5);
            // If any of them don't exist, abort
            if ((cbBook == null) || (cbChapter == null) || (cbVerse == null) || (targetTextArea == null)) return;
            // Get the name of the new book - and, BTW the class instance for the book
            bookList.TryGetValue(bookIdx, out currentBook);
            newBookName = currentBook.CommonName;
            // This is to stop any processing before the books are loaded (at application startup)
            if (cbBook.Items.Count == 0) return;
            /*===================================================================================*
             *                                                                                   *
             * The next statement:                                                               *
             *      cbBook.SelectedIndex = bookIdx - 1;                                          *
             * will kick off cbBook_SelectedIndexChanged in frmMain, which, in turn, will invoke *
             * mainText.respondToNewBook.  Since this will finally invoke displayChapter, we are *
             * going to have an unhelpful loop.  So, let's stop it by disabling the call-back.   *
             * We do this by using isChapUpdateActive                                            *
             *                                                                                   *
             *===================================================================================*/
            //            isChapUpdateActive = false;
            cbBook.SelectedIndex = bookIdx;
            //            isChapUpdateActive = true;
            if (currentBook.NoOfChaptersInBook == 0) return;
            // Get the specified chapter from the current book -
            currentChapter = currentBook.getChapterByChapterNo(chapIdx);
            // Now modify the verse combo box *and* display the new chapter
            /*===================================================================================*
             *                                                                                   *
             * We now want to modify the verse combo box but, given that we're already all set   *
             * populate the text, we don't want it to kick off another round of updating the     *
             * text.  So we will use isChapUpdateActive, much as before.  This time, the         *
             * variable, globalVars.IsTextUpdateActive, will be picked up in the frmMain event   *
             * and handled there                                                                 *
             *                                                                                   *
             *===================================================================================*/
            // We *have* to assume that this is a new chapter, so repopulate cbChapter
            cbChapter.Items.Clear();
            noOfChapters = currentBook.NoOfChaptersInBook;
            for (cdx = 0; cdx < noOfChapters; cdx++)
            {
                realChapNo = currentBook.getChapterNoBySequence(cdx);
                cbChapter.Items.Add(realChapNo);
            }
            globalVars.IsOnHold = true;
            cbChapter.SelectedItem = chapIdx.ToString();
            globalVars.IsOnHold = false;
            noOfVerses = currentChapter.NoOfVersesInChapter;
            targetTextArea.BackColor = backgroundColour;
            targetTextArea.Text = "";
            cbVerse.Items.Clear();
            for (idx = 0; idx < noOfVerses; idx++)
            {
                currentVerse = currentChapter.getVerseBySequence(idx);
                if (currentVerse == null) continue;
                realVNo = currentChapter.getVerseNoBySequence(idx);
                cbVerse.Items.Add(realVNo.ToString());
                if (idx > 0)
                {
                    targetTextArea.AppendText("\n");
                }
                targetTextArea.SelectionColor = globalVars.getColourSetting(3, 1);
                targetTextArea.SelectionFont = engFont;
                targetTextArea.SelectedText = realVNo.ToString() + ": ";
                noOfWords = currentVerse.WordCount;
                for (wdx = 0; wdx < noOfWords; wdx++)
                {
                    currentWord = currentVerse.getWord(wdx);
                    targetTextArea.SelectedText = " " + currentWord.PreWordChars;
                    targetTextArea.SelectedText = zeroWidthSpace.ToString() + currentWord.TextWord;
                    targetTextArea.SelectedText = zeroWidthNonJoiner + currentWord.PostWordChars + currentWord.Punctuation;
                }
            }
            cbVerse.SelectedIndex = 0;
            if (((TabControl)globalVars.getGroupedControl( globalVars.TabControlCode, 0)).SelectedTab == (TabPage) globalVars.getGroupedControl( globalVars.TabPageCode, 1))
            {
                displayString = newBookName + " " + chapIdx;
                globalVars.MasterForm.Text = "Old Testament Student - Septuagint: " + displayString;
                addEntryToHistory(displayString, 0);
            }
        }

        private void addEntryToHistory(String newEntry, int actionCode)
        {
            /*================================================================================================*
             *                                                                                                *
             *                                     addEntryToHistory                                          *
             *                                     -----------------                                          *
             *                                                                                                *
             *  This adds an entry to either of the history combo boxes.                                      *
             *                                                                                                *
             *  Parameters:                                                                                   *
             *  ----------                                                                                    *
             *                                                                                                *
             *  newEntry          The string that will be entered                                             *
             *  actionCode        An integer value specifying whether to add the entry at the head of the     *
             *                    list or at the tail;                                                        *
             *                      0 = head         1 - tail                                                 *
             *                                                                                                *
             *================================================================================================*/
            ComboBox cbHistory;

            cbHistory = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 7);
            if (cbHistory.Items.Contains(newEntry))
            {
                cbHistory.Items.Remove(newEntry);
            }
            if (cbHistory.Items.Count >= globalVars.HistoryMax)
            {
                cbHistory.Items.RemoveAt(cbHistory.Items.Count - 1);
            }
            if (actionCode == 0) cbHistory.Items.Insert(0, newEntry);
            else cbHistory.Items.Add(newEntry);
            cbHistory.SelectedIndex = 0;
        }

        public void backOrForwardOneChapter(int forwardBack)
        {
            /*================================================================================================*
             *                                                                                                *
             *                                    backOrForwardOneChapter                                     *
             *                                    =======================                                     *
             *                                                                                                *
             *  Simply handles moving backwards or forwards from the present chapter.                         *
             *                                                                                                *
             *  Parameters:                                                                                   *
             *  ==========                                                                                    *
             *    forwardBack: 1 = previous chapter                                                           *
             *                 2 = next chapter                                                               *
             *                                                                                                *
             *================================================================================================*/

            int bookIdx = 0, actualIdx, chapIdx;
            String chapNo;
            ComboBox cbBooks, cbChapters;
            classLXXBook currentBook;
            classLXXChapter currentChapter, advChapter;

            cbBooks = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
            cbChapters = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4);
            actualIdx = cbBooks.SelectedIndex;
            chapIdx = cbChapters.SelectedIndex;
            bookList.TryGetValue(actualIdx, out currentBook);
            currentChapter = currentBook.getChapterBySequence(chapIdx);
            if (forwardBack == 2) advChapter = currentChapter.NextChapter;
            else advChapter = currentChapter.PreviousChapter;
            if (advChapter == null) return;
            bookIdx = advChapter.BookNo;
            chapNo = advChapter.ChapterRef;
            displayChapter(bookIdx, chapNo);
        }

        public void processSelectedHistory()
        {
            /*================================================================================================*
             *                                                                                                *
             *                                    processSelectedHistory                                      *
             *                                    ======================                                      *
             *                                                                                                *
             *  Called when a book/chapter is changed using the history combo box.                            *
             *                                                                                                *
             *================================================================================================*/

            int idx, bookIdx, nPstn;
            String historyEntry, bookName, chapNo;
            ComboBox cbBooks, cbChapters, cbHistory;
            classLXXBook currentBook;

            cbBooks = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
            cbChapters = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4);
            cbHistory = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 7);
            if (cbHistory.Items.Count == 0) return;
            historyEntry = cbHistory.SelectedItem.ToString();
            if (String.Compare(globalVars.LastLXXHistoryEntry, historyEntry) == 0) return;
            nPstn = historyEntry.LastIndexOf(' ');
            if (nPstn < 0) return;
            bookName = historyEntry.Substring(0, nPstn);
            chapNo = historyEntry.Substring(nPstn + 1);
            bookIdx = -1;
            for (idx = 0; idx < noOfStoredBooks; idx++)
            {
                bookList.TryGetValue(idx, out currentBook);
                if (String.Compare(currentBook.CommonName, bookName) == 0)
                {
                    bookIdx = idx;
                    break;
                }
            }
            if (bookIdx > -1)
            {
                globalVars.LastLXXHistoryEntry = historyEntry;
                displayChapter(bookIdx, chapNo);
            }
        }

        public Tuple<String, String, int, classLXXWord> identifyClickedWord(int charPosition, String subjectText)
        {
            /*===========================================================================================================*
             *                                                                                                           *
             *                                          identifyClickedWord                                              *
             *                                          ===================                                              *
             *                                                                                                           *
             *  Purpose:                                                                                                 *
             *  =======                                                                                                  *
             *                                                                                                           *
             *  To provide all the various pieces of information that might be needed when the main text is clicked on.  *
             *                                                                                                           *
             *  Parameters:                                                                                              *
             *  ==========                                                                                               *
             *                                                                                                           *
             *  charPosition   The integer character position in the complete text on which the mouse was clicked.       *
             *  subjectText    The compete text                                                                          *
             *                                                                                                           *
             *  Returned values:                                                                                         *
             *  ===============                                                                                          *
             *                                                                                                           *
             *  Item1    The complete line for the verse containing the selection                                        *
             *  Item2    The word selected. (Note: if an appended character is clicked, the preceding word will be       *
             *             returned)                                                                                     *
             *  Item3    The word sequence value (allowing us to navigate to the stored classWord)                       *
             *  Item4    The instance of the word (for Kethib/Qere purposes)                                             *
             *                                                                                                           *
             *===========================================================================================================*/
            int nStart, nPstn = 0, nEnd, verseCharPosition, wordSeq = 0, bookId;
            String currentVerseText, selectedWord, currentRef;
            ComboBox cbBook, cbChapter, cbVerse;
            classLXXBook currentBook = null;
            classLXXChapter currentChapter;
            classLXXVerse currentVerse;
            classLXXWord currentWord;

            // This seems to be true when clicked *beyond* a line - find the start of the current line
            if (subjectText[charPosition] == '\n') nStart = subjectText.LastIndexOf('\n', charPosition - 1);
            else nStart = subjectText.LastIndexOf('\n', charPosition);
            nStart++;
            // Now the end of the line
            if (subjectText[charPosition] == '\n') nEnd = charPosition;
            else nEnd = subjectText.IndexOf('\n', charPosition);
            if (nEnd == -1) nEnd = subjectText.Length;
            // We can now identify the verse and the line of text associated with it
            currentVerseText = subjectText.Substring(nStart, nEnd - nStart);
            // Now to get the verse number
            nEnd = currentVerseText.IndexOf(':');
            currentRef = currentVerseText.Substring(0, nEnd);
            // Update cbVerse
            ((ComboBox)globalVars.getGroupedControl( globalVars.ComboBoxesCode, 5 )).SelectedItem = currentRef;
            // What about the specific word?
            // Let's adjust the charPosition so that it relates to the single line of text, currentVerseText
            verseCharPosition = charPosition - nStart;
            if (verseCharPosition > currentVerseText.Length - 1) verseCharPosition = currentVerseText.Length - 1;
            if (currentVerseText[verseCharPosition] == zeroWidthSpace) nStart = verseCharPosition;
            else nStart = currentVerseText.LastIndexOf(zeroWidthSpace, verseCharPosition);
            if (currentVerseText[verseCharPosition] == '\n') nEnd = verseCharPosition - 1;
            else nEnd = currentVerseText.IndexOf(zeroWidthSpace, verseCharPosition);
            if (nEnd == -1) nEnd = currentVerseText.Length - 1;
            // Count the words up to the current position
            nPstn = currentVerseText.IndexOf(zeroWidthSpace, nPstn);
            while (nPstn > -1)
            {
                if (nPstn > verseCharPosition)
                {
                    // The previous word is the word in which we clicked
                    break;
                }
                // So, at the *start* of the first word, wordSeq = 1, at the *start* of the 2nd word, it = 2, and so on
                wordSeq++;
                nPstn = currentVerseText.IndexOf(zeroWidthSpace, ++nPstn);
            }
            // But wordSeq would be too great:
            //   If we clicked on the verse no, wordSeq would = 0;
            //   If we clicked on the first word, wordSeq would = 1, etc.  Sp ...
            wordSeq--;
            selectedWord = "";
            if (nStart > -1)
            {
                selectedWord = currentVerseText.Substring(nStart, nEnd - nStart).Trim();
                if (selectedWord.Contains(zeroWidthNonJoiner.ToString()))
                {
                    nEnd = selectedWord.IndexOf(zeroWidthNonJoiner);
                    selectedWord = selectedWord.Substring(0, nEnd).Trim();
                }
            }
            cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
            cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4);
            cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 5);
            bookId = cbBook.SelectedIndex;
            if (globalVars.LxxBookList.ContainsKey(bookId)) bookList.TryGetValue(bookId, out currentBook);
            currentChapter = currentBook.getChapterByChapterNo(cbChapter.SelectedItem.ToString());
            currentVerse = currentChapter.getVerseByVerseNo(cbVerse.SelectedItem.ToString());
            currentWord = currentVerse.getWord(wordSeq);
            return new Tuple<String, String, int, classLXXWord>(currentVerseText, selectedWord, wordSeq, currentWord);
        }

        public void Analysis()
        {
            int bookId;
            String ChapNo, VerseNo, rootWord, parseString;
            Char[] parseSplit = { '.' };
            ComboBox cbBook, cbChapter, cbVerse;
            RichTextBox rtxtParse;
            classLXXBook currentBook = null;
            classLXXChapter currentChapter;
            classLXXVerse currentVerse;
            classLXXWord currentWord;
            Font largeFont, mainFont;
            Color textColour, headerColour, backgroundColour;

            largeFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(2, 2), globalVars.getDefinedStyleByIndex(2, 2), globalVars.getTextSize(2, 2));
            mainFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(2, 1), globalVars.getDefinedStyleByIndex(2, 1), globalVars.getTextSize(2, 1));
            backgroundColour = globalVars.getColourSetting(2, 0);
            // First, find the word that has been selected
            if (globalVars.LastSelectedLXXWord.Length == 0)
            {
                MessageBox.Show("You need to actively select a word before this action", "Analyse Word", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
            cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4);
            cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 5);
            bookId = cbBook.SelectedIndex;
            ChapNo = cbChapter.SelectedItem.ToString();
            VerseNo = cbVerse.SelectedItem.ToString();
            if (globalVars.LxxBookList.ContainsKey(bookId)) globalVars.LxxBookList.TryGetValue(bookId, out currentBook);
            currentChapter = currentBook.getChapterByChapterNo(ChapNo);
            currentVerse = currentChapter.getVerseByVerseNo(VerseNo);
            currentWord = currentVerse.getWord(globalVars.SelectedLXXWordSequence);
            // Now we have two tasks.  Intially, get and present the parse details
            rtxtParse = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 4);
            rtxtParse.Clear();
            rtxtParse.BackColor = backgroundColour;
            textColour = globalVars.getColourSetting(2, 1);
            headerColour = globalVars.getColourSetting(2, 2);
            rtxtParse.SelectionFont = mainFont;
            rtxtParse.SelectionColor = headerColour;
            rtxtParse.SelectedText = "\t\t";
            rtxtParse.SelectionFont = largeFont;
            rtxtParse.SelectionColor = headerColour;
            rtxtParse.SelectedText = currentWord.TextWord;
            rtxtParse.SelectionFont = mainFont;
            rtxtParse.SelectionColor = textColour;
            rtxtParse.SelectedText = "\n\n";
            rootWord = currentWord.RootWord;
            parseString = lexicon.parseGrammar(currentWord.CatString, currentWord.ParseString);
            if (parseString.Length > 0)
            {
                rtxtParse.SelectionFont = mainFont;
                rtxtParse.SelectionColor = textColour;
                rtxtParse.SelectedText = parseString;
                rtxtParse.SelectedText = " - root: " + rootWord;
            }
            lexicon.getLexiconEntry(rootWord);
            // finally, make sure the parse page is visible
            ((TabControl) globalVars.getGroupedControl(globalVars.TabControlCode, 4)).SelectedIndex = 0;
        }
    }
}
