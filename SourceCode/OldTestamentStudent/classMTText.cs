using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace OldTestamentStudent
{
    public class classMTText
    {
        const char zeroWidthSpace = '\u200b', zeroWidthNonJoiner = '\u200d';

        int noOfStoredBooks = 0;

        classGlobal globalVars;
        frmProgress progressForm;
        classHistory historyProcesses;
        classHebLexicon lexicon;

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
        SortedDictionary<int, classMTBook> bookList = new SortedDictionary<int, classMTBook>();

        /*===============================================================================================*
         *                                                                                               *
         *                                   listOfStrongConversions                                     *
         *                                   -----------------------                                     *
         *                                                                                               *
         *  Allows us to access data relating to information from Strong's Concordance.                  *
         *     Key:     The word from the text (with accents removed);                                   *
         *     Value:   The class instance giving Strongs data.                                          *
         *                                                                                               *
         *===============================================================================================*/
        SortedList<String, classWordToStrong> listOfStrongConversions = new SortedList<string, classWordToStrong>();

        /*===============================================================================================*
         *                                                                                               *
         *                                        codeDecode                                             *
         *                                        ----------                                             *
         *                                                                                               *
         *  A simple code/decode list, storing information derived from the file, Codes.txt.  This       *
         *    relates to codes relating to a variety of elements (mainly grammar).  The source text      *
         *    provides this information in terse, abbreviated form; this expands to (hopefully) more     *
         *    meaningful descriptions.                                                                   *
         *                                                                                               *
         *     Key:     The word from the text (with accents removed);                                   *
         *     Value:   The class instance giving Strongs data.                                          *
         *                                                                                               *
         *===============================================================================================*/
        SortedList<String, String> codeDecode = new SortedList<string, string>();

        /*===============================================================================================*
         *                                                                                               *
         *                                      variantList:                                             *
         *                                      -----------                                              *
         *                                                                                               *
         *  Stores references to Kethib-Qere.                                                            *
         *     Key:     The word code of the variant;                                                    *
         *     Value:   The class instance storing the data                                              *
         *                                                                                               *
         *===============================================================================================*/
        SortedList<int, classKethib_Qere> variantList = new SortedList<int, classKethib_Qere>();

        ComboBox cbBooks;

        internal SortedList<int, classKethib_Qere> VariantList { get => variantList; set => variantList = value; }
        internal SortedList<string, classWordToStrong> ListOfStrongConversions { get => listOfStrongConversions; set => listOfStrongConversions = value; }

        private delegate void performProgressAdvance(String primaryMessage, String secondaryMessage, bool useSecondary);
        private delegate void performNewFormTitle(Form formId, String newName);

        public void initialiseText(classGlobal inGlobal, frmProgress inForm, classHistory inHistory, classHebLexicon inLex)
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

        private void createNewFormTitle(Form formId, String newName)
        {
            formId.Text = newName;
        }

        public void loadText()
        {
            int bookNo, prevBookNo = 0, fileWordNo, idx, globalWordId;
            String titlesFileName, sourceFileName, fileBuffer, fileName = globalVars.FullConvertFile, unaccentedWord, bookName, kethib, qere,
                ChapNo = "", prevChapNo = "?", verseNo = "", prevVerseNo = "?";
            String[] fields, strongNos;
            Char[] fieldSeperator = { '\t' }, strongSeperator = { '+' };
            StreamReader srSource, srWordToStrong;
            classMTBook currentBook = null;
            classMTChapter currentChapter = null, prevChapter;
            classMTVerse currentVerse = null, previousVerse = null;
            classMTWord currentWord = null;
            classWordToStrong wordToStrong;
            classKethib_Qere currentVariant;

            /*----------------------------------------------------------------------------------------------------------------*
             *                                                                                                                *
             *  Step 0: Load the Kethib-Qere list                                                                             *
             *  ------  -------------------- ----                                                                             *
             *                                                                                                                *
             *  Load this now so we can integrate it into the word record                                                     *
             *                                                                                                                *
             *----------------------------------------------------------------------------------------------------------------*/
            progressForm.Invoke(new performProgressAdvance(updateProgress), "Loading Kethib/Qere data", "", false);
            sourceFileName = globalVars.FullKethibQereFile;
            srSource = new StreamReader(sourceFileName);
            fileBuffer = srSource.ReadLine();
            while (fileBuffer != null)
            {
                fields = fileBuffer.Split(fieldSeperator);
                if (fields.Length >= 3)
                {
                    fileWordNo = Convert.ToInt32(fields[0]);
                    kethib = fields[1];
                    qere = fields[2];
                    if ((kethib.Length > 0) || (qere.Length > 0))
                    {
                        currentVariant = new classKethib_Qere();
                        currentVariant.KethibText = kethib;
                        currentVariant.QereText = qere;
                        variantList.Add(fileWordNo, currentVariant);
                    }
                }
                fileBuffer = srSource.ReadLine();
            }
            srSource.Close();

            /*--------------------------------------------------------*
             *                                                        *
             *  Step 1: Load the titles                               *
             *  ------  ---------------                               *
             *                                                        *
             *--------------------------------------------------------*/
            progressForm.Invoke(new performProgressAdvance(updateProgress), "Loading the names of OT books", "", false);
            titlesFileName = globalVars.FullMTTitleFile;
            srSource = new StreamReader(titlesFileName);
            fileBuffer = srSource.ReadLine();
            while (fileBuffer != null)
            {
                fields = fileBuffer.Split(fieldSeperator);
                if (fields.Length > 3)
                {
                    bookNo = Convert.ToInt32(fields[0]);
                    currentBook = new classMTBook();
                    bookList.Add(bookNo - 1, currentBook);
                    bookName = fields[2];
                    currentBook.BookName = bookName;
                    currentBook.BookId = bookNo;
                    //                    isChapUpdateActive = false;
                    //                    globalVars.getComboBoxControlByIndex(0).Items.Add(bookName);
                    //                    isChapUpdateActive = true;
                    currentBook.ShortName = fields[1];
                    currentBook.Category = Convert.ToInt32(fields[3]);
                    noOfStoredBooks++;
                }
                fileBuffer = srSource.ReadLine();
            }
            srSource.Close();

            /*------------------------------------------------------------------------------------------------------*
             *                                                                                                      *
             *  Step 2: Load the text data                                                                          *
             *  ------  ------------------                                                                          *
             *                                                                                                      *
             *  Fields:                                                                                             *
             *  ======                                                                                              *
             *                                                                                                      *
             *  1    Global word id    A sequential count of all OT words in book, chapter, verse, occurrence order *
             *  2    Global Verse id   Each verse is allocated a unique integer, in the same order                  *
             *  3    Book no.          Int (1 - 39) These are "keys" to Titles.txt in the Source directory          *
             *  4    Chapter no.       Int (1 - n)                                                                  *
             *                           Note: real numbers, not sequence.  So                                      *
             *                           a) not 0-based                                                             *
             *                           b) requires a seperate sequence number when processing chapters            *
             *  5    Verse no.         Int (1 - n) Similar constraints to field 4                                   *
             *  6    Word sequence no. Int (0 - n) (note: zero-based, unlike the other counts)                      *
             *  7    Word              String.  This is the word as used in the text                                *
             *                           Note: there are a small number of "words" that are actually multiple words *
             *                                 and contain spaces.  These have been replaced by non-breaking space  * 
             *                                 (hex 0x00a0) so that a normal space can be used as a boundary value  *
             *                                 between words.                                                       *
             *  8    Unaccented Word   I.e. the word in field 7 with accents removed but retaining                  *
             *                           a) vowels                                                                  *
             *                           b) sin/shin dots                                                           *
             *                           c) dagesh (forte and line)                                                 *
             *                           d)                                                                         *
             *                           e) any non-breaking spaces embedded in the text                            *
             *  9    Bare Word         I.e. the word in field 7 with *all* pointing removed but retaining any       *
             *                           embedded non-breaking spaces                                               *
             *  10   Affix             String sequence (if it exists, such as a mappeq) affixed (postfixed) to the  *
             *                           word but not an integral part of the word.                                 *
             *  11   Prefix marker     Code to indicate whether the current "word" is seperated from the following  *
             *                           word by a space or not:                                                    *
             *                           1   This word is prefixed to the following word,                           *
             *                           0   A space follows this word                                              *
             *  12   Internal code     In the original source data, this was prefixed with "E"                      *
             *  13   Strong Reference  The identified Strong reference or references.  In the original data this    *
             *                           was prefixed with "H".                                                     *
             *                           Note that there can be more than one code, in which case subsequent values *
             *                           are seperated by "+".  (I think this indicates that the word is composed   *
             *                           of more than one Hebrew word and each Strongs code relates to each word.   *
             *  14   Gloss             A simple meaning to the word, as provided by Strongs (a bit naive really)    *
             *  15   Morphology        I.e. a summarised grammatical analysis. It remains to be seen just how       *
             *                           accurate this summary is                                                   *
             *                                                                                                      *
             *------------------------------------------------------------------------------------------------------*/
            cbBooks = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 0);
            sourceFileName = globalVars.FullMTSourceFile;
            srSource = new StreamReader(sourceFileName);
            fileBuffer = srSource.ReadLine();
            while (fileBuffer != null)
            {
                fields = fileBuffer.Split(fieldSeperator);
                globalWordId = Convert.ToInt32(fields[0]);
                bookNo = Convert.ToInt32(fields[2]);
                if (bookNo != prevBookNo)
                {
                    // New book
                    bookList.TryGetValue(bookNo - 1, out currentBook);
                    progressForm.Invoke(new performProgressAdvance(updateProgress), "Loading source text for the Hebrew and Aramaic text", currentBook.BookName, true);
                    prevChapNo = "?";
                    prevVerseNo = "?";
                    prevBookNo = bookNo;
                    previousVerse = null;
                }
                ChapNo = fields[3];
                if (String.Compare(ChapNo, prevChapNo) != 0)
                {
                    prevChapter = currentChapter;
                    currentChapter = currentBook.addNewChapterToBook(ChapNo);
                    if (prevChapter != null) prevChapter.NextChapter = currentChapter;
                    currentChapter.PreviousChapter = prevChapter;
                    currentChapter.BookNo = bookNo - 1;
                    currentChapter.ChapterRef = ChapNo;
                    currentChapter.ChapterSeqNo = currentBook.getSequenceByChapterNo(ChapNo);
                    prevVerseNo = "?";
                    prevChapNo = ChapNo;
                }
                verseNo = fields[4];
                if (String.Compare(verseNo, prevVerseNo) != 0)
                {
                    currentVerse = currentChapter.addVerseToChapter(verseNo);
                    if (previousVerse != null) previousVerse.NextVerse = currentVerse;
                    previousVerse = currentVerse;
                    currentVerse.ChapSeq = currentChapter.ChapterSeqNo;
                    currentVerse.ChapRef = currentChapter.ChapterRef;
                    currentVerse.VerseSeq = currentChapter.NoOfVersesInChapter - 1;
                    currentVerse.VerseRef = currentChapter.getVerseNoBySequence(currentVerse.VerseSeq);
                    prevVerseNo = verseNo;
                }
                currentWord = currentVerse.addWordToVerse();
                currentWord.ActualWord = fields[6];
                currentWord.UnaccentedWord = fields[7];
                currentWord.BareWord = fields[8];
                if (fields[9].Length > 0) currentWord.Affix = fields[9];
                else currentWord.Affix = "";
                if (String.Compare(currentWord.Affix, "׀") == 0) currentWord.Affix = "";
                idx = Convert.ToInt32(fields[10]);
                if (idx == 1) currentWord.IsPrefix = true;
                else currentWord.IsPrefix = false;
                currentWord.ERef = Convert.ToInt32(fields[11]);
                strongNos = fields[12].Split(strongSeperator);
                fileWordNo = strongNos.Length;
                for (idx = 0; idx < fileWordNo; idx++) currentWord.addStrongRef(Convert.ToInt32(strongNos[idx].Trim()));
                currentWord.Gloss = fields[13];
                currentWord.Morphology = fields[14];
                if (variantList.ContainsKey(globalWordId))
                {
                    currentWord.HasVariant = true;
                    variantList.TryGetValue(globalWordId, out currentVariant);
                    currentWord.WordVariant = currentVariant;
                }
                fileBuffer = srSource.ReadLine();
            }
            srSource.Close();
            globalVars.MtBookList = bookList;
            globalVars.NoOfMTBooks = noOfStoredBooks;
            globalVars.MtCurrentBookIndex = 0;
            globalVars.MtCurrentChapter = "";

            /*------------------------------------------------------------------------------------------------------*
             *                                                                                                      *
             *  Step 3: Load the data for identifying BDB codes                                                     *
             *  ------  ---- --- ---- --- ----------- --- -----                                                     *
             *                                                                                                      *
             *------------------------------------------------------------------------------------------------------*/

            progressForm.Invoke(new performProgressAdvance(updateProgress), "Loading BDB codes", "", false);
            srWordToStrong = new StreamReader(fileName);
            fileBuffer = srWordToStrong.ReadLine();
            while (fileBuffer != null)
            {
                fields = fileBuffer.Split(fieldSeperator);
                unaccentedWord = fields[0];
                if (listOfStrongConversions.ContainsKey(unaccentedWord))
                {
                    listOfStrongConversions.TryGetValue(unaccentedWord, out wordToStrong);
                }
                else
                {
                    wordToStrong = new classWordToStrong();
                    listOfStrongConversions.Add(unaccentedWord, wordToStrong);
                    wordToStrong.UnaccentedWord = unaccentedWord;
                }
                strongNos = fields[1].Split(strongSeperator);
                for (idx = 0; idx < strongNos.Length; idx++) wordToStrong.addAnActualWord(strongNos[idx]);
                strongNos = fields[2].Split(strongSeperator);
                for (idx = 0; idx < strongNos.Length; idx++) wordToStrong.addAStrongRef(Convert.ToInt32(strongNos[idx]));
                fileBuffer = srWordToStrong.ReadLine();
            }
            srWordToStrong.Close();

            /*------------------------------------------------------------------------------------------------------*
             *                                                                                                      *
             *  Step 4: Load the code/decode data for the morphology "codes"                                        *
             *  ------  ---- --- ----------- ---- --- --- ---------- -------                                        *
             *                                                                                                      *
             *  This step will load meaningful expressions for each terse code and store them in the Sorted List    *
             *    codeDecode.                                                                                       *
             *                                                                                                      *
             *------------------------------------------------------------------------------------------------------*/

            progressForm.Invoke(new performProgressAdvance(updateProgress), "Loading morphology datas", "", false);
            srSource = new StreamReader(globalVars.FullCodeFile);
            fileBuffer = srSource.ReadLine();
            while (fileBuffer != null)
            {
                // Split into the main fields 
                fields = fileBuffer.Split(fieldSeperator);
                if (!codeDecode.ContainsKey(fields[0])) codeDecode.Add(fields[0], fields[1]);
                fileBuffer = srSource.ReadLine();
            }
            srSource.Close();
        }

        public void respondToNewBook()
        {
            int bookId, cdx, noOfChapters;
            String chapterRef;
            ComboBox cbBook, cbChapter;
            SortedDictionary<int, classMTBook> bookList;
            classMTBook currentBook;

            cbBook = (ComboBox) globalVars.getGroupedControl(globalVars.ComboBoxesCode, 0);
            bookList = globalVars.MtBookList;
            cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 1);
            bookId = cbBook.SelectedIndex;
            if (bookId == -1) return;
            bookList.TryGetValue(bookId, out currentBook);
            noOfChapters = currentBook.NoOfChaptersInBook;
            cbChapter.Items.Clear();
            for (cdx = 0; cdx < noOfChapters; cdx++)
            {
                chapterRef = currentBook.getChapterNoBySequence(cdx);
                cbChapter.Items.Add(chapterRef);
            }
            if (cbChapter.Items.Count > 0) cbChapter.SelectedIndex = 0;
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
            classMTBook currentBook;
            classMTChapter currentChapter;
            classMTVerse currentVerse;
            classMTWord currentWord;
            RichTextBox targetTextArea;
            ComboBox cbChapter, cbVerse;
            ComboBox cbBook;
            Font engFont, hebFont, altFont;
            Color backgroundColour;

            //Set up the fonts to be used
            engFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(0, 1), globalVars.getDefinedStyleByIndex(0, 1), globalVars.getTextSize(0, 1));
            hebFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(0, 2), globalVars.getDefinedStyleByIndex(0, 2), globalVars.getTextSize(0, 2));
            altFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(0, 3), globalVars.getDefinedStyleByIndex(0, 3), globalVars.getTextSize(0, 3));
            backgroundColour = globalVars.getColourSetting(0, 0);
            // Get the Rich Text area in which the text occurs
            targetTextArea = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 0);
            // Get the combo boxes for the Book, Chapter and Verse
            cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 0);
            cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 1);
            cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 2);
            // If any of them don't exist, abort
            if ((cbBook == null) || (cbChapter == null) || (cbVerse == null) || (targetTextArea == null)) return;
            // Get the name of the new book - and, BTW the class instance for the book
            bookList.TryGetValue(bookIdx, out currentBook);
            newBookName = currentBook.BookName;
            // This is to stop any processing before the books are loaded (at application startup)
            if (cbBook.Items.Count == 0) return;
            /*===================================================================================*
             *                                                                                   *
             * The next statement:                                                               *
             *      cbBook.SelectedIndex = bookIdx;                                              *
             * will kick off cbBook_SelectedIndexChanged in frmMain, which, in turn, will invoke *
             * mainText.respondToNewBook.  Since this will finally invoke displayChapter, we are *
             * going to have an unhelpful loop.  So, let's stop it by disabling the call-back.   *
             * We do this by using globalVars.IsTextUpdateActive                                 *
             *                                                                                   *
             *===================================================================================*/
            globalVars.IsOnHold = true;
            cbBook.SelectedIndex = bookIdx;
            globalVars.IsOnHold = false;
            if (currentBook.NoOfChaptersInBook == 0) return;
            // Get the specified chapter from the current book -
            currentChapter = currentBook.getChapterByChapterNo(chapIdx);
            // Now modify the verse combo box *and* display the new chapter
            /*===================================================================================*
             *                                                                                   *
             * We now want to modify the verse combo box but, given that we're already all set   *
             * populate the text, we don't want it to kick off another round of updating the     *
             * text.  So we will use isChapUpdateActive, much as before.  This time, the 
             * variable, isChapUpdateActive, will be picked up in the frmMain event and handled  *
             * there                                                                             *
             *                                                                                   *
             *===================================================================================*/
            // We *have* to assume that this is a new chapter, so repopulate cbChapter
            cbChapter.Items.Clear();
            noOfChapters = currentBook.NoOfChaptersInBook;
            for ( cdx = 0; cdx < noOfChapters; cdx++)
            {
                realChapNo = currentBook.getChapterNoBySequence(cdx);
                cbChapter.Items.Add(realChapNo);
            }
            globalVars.IsOnHold = true;
            cbChapter.SelectedItem = chapIdx;
            globalVars.IsOnHold = false;
            noOfVerses = currentChapter.NoOfVersesInChapter;
            //            targetTextArea.BackColor = globalVars.getBackColorByIndex(0);
            targetTextArea.BackColor = backgroundColour;
            targetTextArea.Text = "";
            cbVerse.Items.Clear();
            for (idx = 0; idx < noOfVerses; idx++)
            {
                /*--------------------------------------------------------------------------------------------------------------*
                 *                                                                                                              *
                 *                                        The structure of output text                                          *
                 *                                        ----------------------------                                          *
                 *                                                                                                              *
                 *  Each line is of the form:                                                                                   *
                 *      verse number + ": " (in English font and colour)                                                        *
                 *      hebrew word = zeroWidthChar + actual word + zeroWidthNonJoiner + any affix                              *
                 *         if the word is not a prefix and the affix is not a mappeq, a space is added                          *
                 *         if the word has a variant, it is set to red (or option), otherwise black (or option)                 *
                 *                                                                                                              *
                 *  So, each word can be identified by looking for the zeroWidthChar and ending with the zeroWidthNonJoiner.    *
                 *    Note that this makes it easy to identify a word, even when it is part of a larger complex.                *
                 *                                                                                                              *
                 *--------------------------------------------------------------------------------------------------------------*/
                currentVerse = currentChapter.getVerseBySequence(idx);
                if (currentVerse == null) continue;
                realVNo = currentChapter.getVerseNoBySequence(idx);
                cbVerse.Items.Add(realVNo.ToString());
                if (idx > 0)
                {
                    targetTextArea.AppendText("\n");
                }
                targetTextArea.SelectionColor = globalVars.getColourSetting(0, 1);
                targetTextArea.SelectionFont = engFont;
                targetTextArea.SelectedText = realVNo.ToString() + ": ";
                noOfWords = currentVerse.WordCount;
                for (wdx = 0; wdx < noOfWords; wdx++)
                {
                    currentWord = currentVerse.getWord(wdx);
                    //                    if (currentWord.HasVariant) targetTextArea.SelectionColor = globalVars.getForeAltColourByIndex(0);
                    //                    else targetTextArea.SelectionColor = globalVars.getForePrimeColorByIndex(0);
                    if (currentWord.HasVariant)
                    {
                        targetTextArea.SelectionColor = globalVars.getColourSetting(0, 3);
                        targetTextArea.SelectionFont = altFont;
                    }
                    else
                    {
                        targetTextArea.SelectionColor = globalVars.getColourSetting(0, 2);
                        targetTextArea.SelectionFont = hebFont;
                    }
                    targetTextArea.SelectedText = zeroWidthSpace.ToString() + currentWord.ActualWord;
                    if (currentWord.Affix.Length > 0) targetTextArea.SelectedText = zeroWidthNonJoiner + currentWord.Affix;
                    if ((!currentWord.IsPrefix) && (String.Compare(currentWord.Affix, "־") != 0)) targetTextArea.SelectedText = " ";
                }
            }
            cbVerse.SelectedIndex = 0;
            if (((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 0)).SelectedTab == (TabPage)globalVars.getGroupedControl(globalVars.TabPageCode, 0))
            {
                displayString = newBookName + " " + chapIdx;
                globalVars.MasterForm.Text = "Old Testament Student - Masoretic Text: " + displayString;
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

            cbHistory = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 6);
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
            classMTBook currentBook;
            classMTChapter currentChapter, advChapter;

            cbBooks = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 0);
            cbChapters = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 1);
            actualIdx = cbBooks.SelectedIndex;
            chapIdx = cbChapters.SelectedIndex;
            bookList.TryGetValue(actualIdx, out currentBook);
            currentChapter = currentBook.getChapterBySequence(chapIdx);
            if (forwardBack == 2) advChapter = currentChapter.NextChapter;
            else advChapter = currentChapter.PreviousChapter;
            if (advChapter == null) return;
            bookIdx = advChapter.BookNo;
            chapNo = advChapter.ChapterRef;
            globalVars.IsOnHold = true;
            displayChapter(bookIdx, chapNo);
            globalVars.IsOnHold = false;
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
            ComboBox cbHistory;
            classMTBook currentBook;

            cbHistory = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 6);
            if (cbHistory.Items.Count == 0) return;
            historyEntry = cbHistory.SelectedItem.ToString();
            if (String.Compare(globalVars.LastMTHistoryEntry, historyEntry) == 0) return;
            nPstn = historyEntry.LastIndexOf(' ');
            if (nPstn < 0) return;
            bookName = historyEntry.Substring(0, nPstn);
            chapNo = historyEntry.Substring(nPstn + 1);
            bookIdx = -1;
            for (idx = 0; idx < noOfStoredBooks; idx++)
            {
                bookList.TryGetValue(idx, out currentBook);
                if (String.Compare(currentBook.BookName, bookName) == 0)
                {
                    bookIdx = idx;
                    break;
                }
            }
            if (bookIdx > -1)
            {
                globalVars.LastMTHistoryEntry = historyEntry;
                displayChapter(bookIdx, chapNo);
            }
        }

        public Tuple<String, String, int, classMTWord> identifyClickedWord(int charPosition, String subjectText)
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
            classMTBook currentBook = null;
            classMTChapter currentChapter;
            classMTVerse currentVerse;
            classMTWord currentWord;

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
            ((ComboBox) globalVars.getGroupedControl(globalVars.ComboBoxesCode, 2 )).SelectedItem = currentRef;
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
            cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 0);
            cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 1);
            cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 2);
            bookId = cbBook.SelectedIndex;
            if (globalVars.MtBookList.ContainsKey(bookId)) bookList.TryGetValue(bookId, out currentBook);
            currentChapter = currentBook.getChapterByChapterNo(cbChapter.SelectedItem.ToString());
            currentVerse = currentChapter.getVerseByVerseNo(cbVerse.SelectedItem.ToString());
            currentWord = currentVerse.getWord(wordSeq);
            return new Tuple<String, String, int, classMTWord>(currentVerseText, selectedWord, wordSeq, currentWord);
        }

        public void Analysis()
        {
            bool isFirstEntry = true, isNewEntry = true;
            int idx, noOfMorphs, extraCommentCode, bookId, noOfEntries, jdx, noOfDetail;
            String contentText, morphSource, morphologyString = "", ChapNo, VerseNo;
            String[] parseList;
            Char[] parseSplit = { '.' };
            classMTBook currentBook = null;
            classMTChapter currentChapter;
            classMTVerse currentVerse;
            classMTWord currentWord;
            classBDBEntry currentEntry;
            ComboBox cbBook, cbChapter, cbVerse;
            RichTextBox rtxtParse;
            Font largeFont, mainFont;
            Color textColour, headerColour;

            largeFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(2, 2), globalVars.getDefinedStyleByIndex(2, 2), globalVars.getTextSize(2, 2));
            mainFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(2, 1), globalVars.getDefinedStyleByIndex(2, 1), globalVars.getTextSize(2, 1));
            // First, find the word that has been selected
            if (globalVars.LastSelectedMTWord.Length == 0)
            {
                MessageBox.Show("You need to actively select a word before this action", "Analyse Word", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 0);
            cbChapter = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 1);
            cbVerse = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 2);
            bookId = cbBook.SelectedIndex;
            ChapNo = cbChapter.SelectedItem.ToString();
            VerseNo = cbVerse.SelectedItem.ToString();
            if (globalVars.MtBookList.ContainsKey(bookId)) globalVars.MtBookList.TryGetValue(bookId, out currentBook);
            currentChapter = currentBook.getChapterByChapterNo(ChapNo);
            currentVerse = currentChapter.getVerseByVerseNo(VerseNo);
            currentWord = currentVerse.getWord(globalVars.SelectedMTWordSequence);
            // Now we have two tasks.  Intially, get and present the parse details
            rtxtParse = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 4);
            rtxtParse.Clear();
            rtxtParse.BackColor = globalVars.getColourSetting(2, 0);  // Background colour for the parse area
            textColour = globalVars.getColourSetting(2, 1);           // Main text colour for the parse area
            headerColour = globalVars.getColourSetting(2, 2);         // Large text (title) colour for the parse area
            rtxtParse.SelectionFont = mainFont;
            rtxtParse.SelectionColor = headerColour;
            rtxtParse.SelectedText = "\t\t";
            rtxtParse.SelectionFont = largeFont;
            rtxtParse.SelectionColor = headerColour;
            rtxtParse.SelectedText = currentWord.UnaccentedWord;
            rtxtParse.SelectionFont = largeFont;
            rtxtParse.SelectionColor = textColour;
            rtxtParse.SelectedText = "\n";
            rtxtParse.SelectionFont = mainFont;
            rtxtParse.SelectionColor = textColour;
            rtxtParse.SelectedText = "\n";
            parseList = currentWord.Morphology.Split(parseSplit);
            noOfMorphs = parseList.Length;
            for (idx = 0; idx < noOfMorphs; idx++)
            {
                extraCommentCode = 0;
                morphSource = parseList[idx].Trim();
                if (codeDecode.ContainsKey(morphSource)) codeDecode.TryGetValue(morphSource, out morphologyString);
                if (morphologyString.Length > 0)
                {
                    if (morphologyString[0] == '*')
                    {
                        extraCommentCode = Convert.ToInt32(morphologyString[1].ToString());
                        morphologyString = morphologyString.Substring(2);
                    }
                    if (String.Compare(morphologyString, "unknown") == 0) continue;
                    if (isNewEntry)
                    {
                        rtxtParse.SelectionFont = mainFont;
                        rtxtParse.SelectionColor = textColour;
                        rtxtParse.SelectedText = morphologyString + ":";
                        isNewEntry = false;
                    }
                    else rtxtParse.SelectedText = " " + morphologyString;
                    if (extraCommentCode > 0)
                    {
                        rtxtParse.SelectedText = "\n\n";
                        rtxtParse.SelectionColor = textColour;
                        rtxtParse.SelectionFont = mainFont;
                        rtxtParse.SelectedText = "Aramaic verb form";
                    }
                }
            }
            // Now display the lexicon details
            noOfEntries = currentWord.NoOfStrongRefs;
            contentText = "<html><head></head><body>";
            for (idx = 0; idx < noOfEntries; idx++)
            {
                currentEntry = lexicon.getBDBEntryForStrongNo(currentWord.getStrongRefBySeq(idx));
                contentText += "<div align=\"center\">" + currentEntry.EntryWord + "</div><br />";
                noOfDetail = currentEntry.NoOfEntries;
                for (jdx = 0; jdx < noOfDetail; jdx++)
                {
                    if (isFirstEntry)
                    {
                        contentText += currentEntry.getEntryBySequence(jdx);
                        isFirstEntry = false;
                    }
                    else
                    {
                        contentText += "<br /><br /><br />===========================<br /><br /><br />" + currentEntry.getEntryBySequence(jdx);
                    }
                }
            }
            contentText += "</body></html>";
            ((WebBrowser) globalVars.getGroupedControl(11,0)).DocumentText = contentText;
            // finally, make sure the parse page is visible
            ((TabControl) globalVars.getGroupedControl( globalVars.TabControlCode, 4)).SelectedIndex = 0;
        }
    }
}
