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
    public partial class frmCopyOptions : Form
    {
        /*==================================================================================*
         *                                                                                  *
         *                                 frmCopyOptions                                   *
         *                                 ==============                                   *
         *                                                                                  *
         *  Controls and performs the various copying options, as provided in the context   *
         *    menu of the two main text areas.                                              *
         *                                                                                  *
         *  Parameters:                                                                     *
         *  ==========                                                                      *
         *                                                                                  *
         *  typeOfCopy    Basically, which of the options was selected:                     *
         *                                                                                  *
         *       Code                          Significance                                 *
         *         1         Copy the currently selected word (or the word the user has     *
         *                     clicked on                                                   *
         *         2         Copy the current verse                                         *
         *         3         Copy the whole chapter                                         *
         *         4         Copy the user-selected text (replaces ^C)                      *
         *                                                                                  *
         *  whichVersion  Identifies whether the copy is in the MT or LXX                   *
         *         0         MT                                                             *
         *         1         LXX                                                            *
         *                                                                                  *
         *==================================================================================*/

        int typeOfCopy, whichVersion;

        /*-----------------------------------------------------------------------------------*
         *                                                                                   *
         *                                Controlling codes                                  *
         *                                 ----------------                                  *
         *                                                        Values                     *
         *     Code            Meaning                      1                 2              *
         *                                                                                   *
         *   destCode     copyDestination               Clipboard           Notes            *
         *   refCode      referenceIncluded             Include             Exclude          *
         *   accentCode   AccentsIncluded               Include             Exclude          *
         *   rememberCode Remember and use settings     Don't use           Use              *
         *   whichVersion Flags source as MT or LXX     MT = 0              LXX = 16         *
         *                                                                                   *
         *-----------------------------------------------------------------------------------*/
        int destCode, refCode, accentCode, rememberCode;
        String referenceText, selectedText;

        classGlobal globalVars;
        classHebLexicon hebrewMethods;
        classGreekOrthography greekMethods;
        classNote noteProcs;

        public int TypeOfCopy { get => typeOfCopy; set => typeOfCopy = value; }
        public int WhichVersion { get => whichVersion; set => whichVersion = value; }
        public int DestCode { get => destCode; set => destCode = value; }
        public int RefCode { get => refCode; set => refCode = value; }
        public int AccentCode { get => accentCode; set => accentCode = value; }
        public int RememberCode { get => rememberCode; set => rememberCode = value; }

        public frmCopyOptions( int copyCode, int mtOrLxx, classGlobal inGlobal, classHebLexicon inHebLex, classGreekOrthography inGkProcs, classNote inNote)
        {
            InitializeComponent();
            typeOfCopy = copyCode;
            whichVersion = mtOrLxx;
            globalVars = inGlobal;
            hebrewMethods = inHebLex;
            greekMethods = inGkProcs;
            noteProcs = inNote;
            referenceText = formReference();
            getRelevantText();
            switch( copyCode)
            {
                case 1:
                case 2: this.Text = "Copying " + referenceText + " - " + selectedText; break;
                case 3: this.Text = "Copying " + referenceText; break;
                case 4: this.Text = "Copying a selection from " + referenceText; break;
            }
            
        }

        private String formReference()
        {
            ComboBox cbBook;
            String refText = "", bookName, chapNo, vNo;
            classMTBook currentMTBook;
            classLXXBook currentLXXBook;

            switch ( whichVersion)
            {
                case 0:
                    cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 0);
                    globalVars.MtBookList.TryGetValue(cbBook.SelectedIndex, out currentMTBook);
                    bookName = currentMTBook.BookName;
                    chapNo = ((ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 1)).SelectedItem.ToString();
                    vNo = ((ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 2)).SelectedItem.ToString();
                    switch (typeOfCopy)
                    {
                        case 1:
                        case 2: refText = bookName + " " + chapNo + "." + vNo; break;
                        case 3:
                        case 4: refText = bookName + " " + chapNo; break;
                    }
                    break;
                case 1:
                    cbBook = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 3);
                    globalVars.LxxBookList.TryGetValue(cbBook.SelectedIndex, out currentLXXBook);
                    bookName = currentLXXBook.CommonName;
                    chapNo = ((ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4)).SelectedItem.ToString();
                    vNo = ((ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 5)).SelectedItem.ToString();
                    switch (typeOfCopy)
                    {
                        case 1:
                        case 2: refText = bookName + " " + chapNo + "." + vNo; break;
                        case 3:
                        case 4: refText = bookName + " " + chapNo; break;
                    }
                    break;
            }
            return refText;
        }

        private void getRelevantText()
        {
            int nStart, nLength;
            RichTextBox targetTextArea = null;

            switch ( whichVersion)
            {
                case 0: 
                    targetTextArea = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 0);
                    switch (typeOfCopy)
                    {
                        case 1: selectedText = globalVars.LastSelectedMTWord; break;
                        case 2: 
                            selectedText = globalVars.LastSelectedMTVerse;
                            nStart = selectedText.IndexOf(":");
                            while (selectedText[++nStart] == ' ') ;
                            selectedText = selectedText.Substring(nStart);
                            break;
                        case 3: selectedText = targetTextArea.Text; break;
                        case 4:
                            nStart = targetTextArea.SelectionStart;
                            nLength = targetTextArea.SelectionLength;
                            if (nLength == 0) selectedText = "";
                            else selectedText = targetTextArea.Text.Substring( nStart, nLength ); break;
                    }
                    break;
                case 1: 
                    targetTextArea = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 1);
                    switch (typeOfCopy)
                    {
                        case 1: selectedText = globalVars.LastSelectedLXXWord; break;
                        case 2: 
                            selectedText = globalVars.LastSelectedLXXVerse;
                            nStart = selectedText.IndexOf(":");
                            while (selectedText[++nStart] == ' ') ;
                            selectedText = selectedText.Substring(nStart);
                            break;
                        case 3: selectedText = targetTextArea.Text; break;
                        case 4:
                            nStart = targetTextArea.SelectionStart;
                            nLength = targetTextArea.SelectionLength;
                            if (nLength == 0) selectedText = "";
                            else selectedText = targetTextArea.Text.Substring(nStart, nLength); break;
                    }
                    break;
            }
        }

        private void copyWord( int destCode, int refCode, int accentCode )
        {
            const char zeroWidthSpace = '\u200b', zeroWidthNonJoiner = '\u200d';

            String copyWord = "", modifiedWord, informationMessage = "";

            switch ( whichVersion)
            {
                case 0:
                    {
                        copyWord = globalVars.LastSelectedMTWord;
                        modifiedWord = copyWord.Replace(zeroWidthSpace.ToString(), "");
                        modifiedWord = modifiedWord.Replace(zeroWidthNonJoiner.ToString(), "");
                        if (accentCode == 2) modifiedWord = hebrewMethods.removeAccents(modifiedWord);
                        if( destCode == 1)
                        {
                            if (refCode == 1) Clipboard.SetText(referenceText + ":  " + modifiedWord);
                            else Clipboard.SetText(modifiedWord);
                            informationMessage = referenceText + ", " + modifiedWord + "has been copied to the clipboard";
                            MessageBox.Show(informationMessage, "Copy of " + copyWord + " successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    break;
                case 1:
                    {
                        copyWord = globalVars.LastSelectedMTWord;
                        modifiedWord = copyWord.Replace(zeroWidthSpace.ToString(), "");
                        modifiedWord = modifiedWord.Replace(zeroWidthNonJoiner.ToString(), "");
                        if (accentCode == 2) modifiedWord = greekMethods.reduceToBareGreek(modifiedWord, true);
                        if (destCode == 1)
                        {
                            if (refCode == 1) Clipboard.SetText(referenceText + ":  " + modifiedWord);
                            else Clipboard.SetText(modifiedWord);
                            informationMessage = referenceText + ", " + modifiedWord + "has been copied to the clipboard";
                            MessageBox.Show(informationMessage, "Copy of " + copyWord + " successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    break;
            }
        }

        private void setupCodes()
        {
            if (rbtnCopyToMemory.Checked) destCode = 1;
            else destCode = 2;
            if (rbtnIncludeReference.Checked) refCode = 1;
            else refCode = 2;
            if (rbtnIncludeAccents.Checked) accentCode = 1;
            else accentCode = 2;
            if (chkRemember.Checked) rememberCode = 2;
            else rememberCode = 1;
        }

        public void performCopy()
        {
            const char zeroWidthSpace = '\u200b', zeroWidthNonJoiner = '\u200d';

            int idx, noOfWords;
            String modifiedWord, finalText = "", tempText = "", messageText = "";
            Char[] splitter = { ' ' };
            String[] brokenText;

            if (whichVersion < 0) return;
            globalVars.setCopyOptions((typeOfCopy - 1), 0, destCode, whichVersion);
            globalVars.setCopyOptions((typeOfCopy - 1), 1, refCode, whichVersion);
            globalVars.setCopyOptions((typeOfCopy - 1), 2, accentCode, whichVersion);
            globalVars.setCopyOptions((typeOfCopy - 1), 3, rememberCode, whichVersion);
            modifiedWord = selectedText.Replace(zeroWidthSpace.ToString(), "");
            modifiedWord = modifiedWord.Replace(zeroWidthNonJoiner.ToString(), "");
            switch (accentCode)
            {
                case 1: finalText = modifiedWord; break;
                case 2:
                    brokenText = modifiedWord.Split(splitter);
                    noOfWords = brokenText.Length;
                    for( idx = 0; idx < noOfWords; idx++)
                    {
                        switch( whichVersion )
                        {
                            case 0: tempText = hebrewMethods.removeAccents(brokenText[idx]); break;
                            case 1: tempText = greekMethods.reduceToBareGreek(brokenText[idx], true); break;
                        }
                        if (idx == 0) finalText = tempText;
                        else finalText += " " + tempText;
                    }
                    break;
            }
            if (refCode == 1) finalText = referenceText + ": " + finalText;
            switch( typeOfCopy)
            {
                case 1: messageText = selectedText; break;
                case 2: messageText = "the verse"; break;
                case 3: messageText = "the chapter"; break;
                case 4: messageText = "the selected text"; break;
            }
            switch( destCode)
            {
                case 1:
                    if( finalText == null)
                    {
                        MessageBox.Show("Your copy string was empty\nThis isn't allowed.", "Copy failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Clipboard.SetText(finalText);
                    MessageBox.Show(referenceText + ": " + messageText + " has been copied to the clipboard", "Copy successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case 2: noteProcs.insertTextIntoNote(finalText); break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            setupCodes();
            performCopy();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
