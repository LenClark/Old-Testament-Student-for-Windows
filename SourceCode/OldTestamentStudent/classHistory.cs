using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OldTestamentStudent
{
    public class classHistory
    {
        classGlobal globalVars;
        classMTText mtText;
        classLXXText lxxText;

        private delegate void performComboBoxUpdate(ComboBox targetCB, String comboItem);
        private delegate void performComboBoxSelection(ComboBox targetCB, int itemIndex);
        private delegate void performComboBoxClear(ComboBox targetCB);

        private void addComboItem(ComboBox targetCB, String cbItem)
        {
            targetCB.Items.Add(cbItem);
        }

        private void selectComboItem(ComboBox targetCB, int itemIndex)
        {
            if (targetCB.Items.Count > itemIndex)
            {
                targetCB.Text = targetCB.Items[0].ToString();
                //targetCB.SelectedIndex = itemIndex;
            }
        }

        private void clearCombobox(ComboBox targetCB)
        {
            targetCB.Items.Clear();
        }

        public classHistory( classGlobal inGlobal, classMTText inMtText, classLXXText inLxxText)
        {
            globalVars = inGlobal;
            mtText = inMtText;
            lxxText = inLxxText;
        }

        public void loadHistory()
        {
            /*============================================================================================*
             *                                                                                            *
             *                                       loadHistory                                          *
             *                                       ===========                                          *
             *                                                                                            *
             *  This is called only once, at the start of the application.  Because it is called in a     *
             *    background threat, we cannot use addEntryToHistory, which is a method designed for use  *
             *    throughout the life of the application. However, we _can_ assume that the target        *
             *    Combo boxes are currently empty.                                                        *
             *                                                                                            *
             *============================================================================================*/

            String historyFileName, fileBuffer;
            FileInfo fiHistory;
            StreamReader srHistory;
            ComboBox cbHistory;

            historyFileName = globalVars.FullMTNotesPath + @"\History.txt";
            fiHistory = new FileInfo(historyFileName);
            cbHistory = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 6);
            if (fiHistory.Exists)
            {
                srHistory = new StreamReader(historyFileName);
                fileBuffer = srHistory.ReadLine();
                if ((fileBuffer != null) && (fileBuffer[0] == ';')) fileBuffer = srHistory.ReadLine();
                while (fileBuffer != null)
                {
                    cbHistory.Invoke(new performComboBoxUpdate(addComboItem), cbHistory, fileBuffer);
                    fileBuffer = srHistory.ReadLine();
                }
                srHistory.Close();
                srHistory.Dispose();
                if (cbHistory.Items.Count == 0)
                {
                    fileBuffer = "Genesis 1";
                    cbHistory.Invoke(new performComboBoxUpdate(addComboItem), cbHistory, fileBuffer);
                }
                else
                {
                    cbHistory.Invoke(new performComboBoxSelection(selectComboItem), cbHistory, 0);
                }
            }
            else
            {
                cbHistory.Invoke(new performComboBoxUpdate(addComboItem), cbHistory, "Genesis 1");
                cbHistory.Invoke(new performComboBoxSelection(selectComboItem), cbHistory, 0);
                globalVars.MtCurrentBookIndex = 0;
                globalVars.MtCurrentChapter = "";
            }

            historyFileName = globalVars.FullLXXNotesPath + @"\History.txt";
            fiHistory = new FileInfo(historyFileName);
            if (fiHistory.Exists)
            {
                cbHistory = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 7);
                srHistory = new StreamReader(historyFileName);
                fileBuffer = srHistory.ReadLine();
                if ((fileBuffer != null) && (fileBuffer[0] == ';')) fileBuffer = srHistory.ReadLine();
                while (fileBuffer != null)
                {
                    cbHistory.Invoke(new performComboBoxUpdate(addComboItem), cbHistory, fileBuffer);
                    fileBuffer = srHistory.ReadLine();
                }
                srHistory.Close();
                srHistory.Dispose();
                if (cbHistory.Items.Count == 0)
                {
                    fileBuffer = "Genesis 1";
                    cbHistory.Invoke(new performComboBoxUpdate(addComboItem), cbHistory, fileBuffer);
                }
                else
                {
                    cbHistory.Invoke(new performComboBoxSelection(selectComboItem), cbHistory, 0);
                }
            }
            else
            {
                cbHistory = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 7);
                cbHistory.Invoke(new performComboBoxUpdate(addComboItem), cbHistory, "Genesis 1");
                cbHistory.Invoke(new performComboBoxSelection(selectComboItem), cbHistory, 0);
                globalVars.LxxCurrentBookIndex = 0;
                globalVars.LxxCurrentChapter = "";
            }
        }

        public void addEntryToHistory(String newEntry, int actionCode, int languageCode)
        {
            /*======================================================================================================*
             *                                                                                                      *
             *                                           addEntryToHistory                                          *
             *                                           -----------------                                          *
             *                                                                                                      *
             *  This adds an entry to either of the history combo boxes.                                            *
             *                                                                                                      *
             *  Parameters:                                                                                         *
             *  ----------                                                                                          *
             *                                                                                                      *
             *  newEntry          The string that will be entered                                                   *
             *  actionCode        An integer value specifying whether to add the entry at the head of the list or   *
             *                    at the tail;                                                                      *
             *                      0 = head         1 - tail                                                       *
             *  languageCode      Identifies whether the entry is from the MT or LXX                                *
             *                      0 = MT           1 = LXX                                                        *
             *                                                                                                      *
             *  The statement:                                                                                      *
             *             addEntryToHistory(displayString, 0);                                                     *
             *  will remove a pre-existing entry for the book and chapter, if it exists, and, whether it existed    *
             *  before or not, it will add i.  This means that the event method, cbHistory_SelectedIndexChanged,    *
             *  will be invoked and this, in turn, will call mainText.processSelectedHistory.  This will finish by  *
             *  calling displayChapter which, as before, sets up a loop.  So, once again, we can use                *
             *  isChapUpdateActive to prevent the loop occurring                                                    *
             *                                                                                                      *
             *======================================================================================================*/
            ComboBox cbHistory;

            if( languageCode == 0) cbHistory = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 6);
            else cbHistory = (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 7);
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
    }
}
