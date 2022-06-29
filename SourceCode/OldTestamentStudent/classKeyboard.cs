using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OldTestamentStudent
{
    class classKeyboard
    {
        bool isGkMiniscule, isCarriageReturnDown = false, isShiftDown = false;
        Panel mtKeyboardPanel, lxxKeyboardPanel;
        Button[,] hebKeys, gkKeys;
        SortedList<int, String> minisculeKeyFace = new SortedList<int, string>();
        SortedList<int, String> majisculeKeyFace = new SortedList<int, string>();
        Label labEnteredTextLbl;
        Button btnHebUse, btnGkUse;
        TextBox txtHebEnteredText, txtGkEnteredText;
        GroupBox gbTextDestination;

        private delegate void performPanelAddition(Panel targetPanel, Button currentButton);
        private delegate void performPanelGroup(Panel targetPanel, GroupBox currentGB);
        private delegate void performAddLabel(Panel targetPanel, Label currentLabel);
        private delegate void performAddTextbox(Panel targetPanel, TextBox currentTextBox);
        private delegate void performGroupAddition(GroupBox currentGB, RadioButton currentButton);
        private delegate void performToolTipAddition(ToolTip ttTarget, Control ctrlTarget, String hint);

        classGlobal globalVars;
        frmProgress progressForm;
        classGreekOrthography greekOrthography;

        private void addToPanel(Panel targetPanel, Button currentButton)
        {
            targetPanel.Controls.Add(currentButton);
        }

        private void addGroupToPanel(Panel targetPanel, GroupBox currentGB)
        {
            targetPanel.Controls.Add(currentGB);
        }

        private void addLabelToPanel(Panel targetPanel, Label currentLabel)
        {
            targetPanel.Controls.Add(currentLabel);
        }

        private void addTextboxToPanel(Panel targetPanel, TextBox currentTextBox)
        {
            targetPanel.Controls.Add(currentTextBox);
        }

        private void updateTooltip(ToolTip ttTarget, Control ctrlTarget, String hint)
        {
            ttTarget.SetToolTip(ctrlTarget, hint);
        }

        private void addRButtonToGroup( GroupBox currentGB, RadioButton currentButton )
        {
            currentGB.Controls.Add(currentButton);
        }

        public void initialiseMTKeyboard(classGlobal inConfig, frmProgress inProgress, classGreekOrthography inGkOrthog)
        {
            globalVars = inConfig;
            progressForm = inProgress;
            greekOrthography = inGkOrthog;
            mtKeyboardPanel = (Panel)globalVars.getGroupedControl(globalVars.PanelCode, 0);
            lxxKeyboardPanel = (Panel)globalVars.getGroupedControl(globalVars.PanelCode, 1);
            setupHebKeyboard();
            setupGkKeyboard();
        }

        public void setupHebKeyboard()
        {
            /*=============================================================================================================*
             *                                                                                                             *
             *                                              setupHebKeyboard                                               *
             *                                              ================                                               *
             *                                                                                                             *
             *  Specific to the virtual keyboard in the Masoretic Text section.                                            *
             *                                                                                                             *
             *=============================================================================================================*/
            const int noOfRows = 4, noOfCols = 13, keyGap = 4, keyHeight = 30;

            int idx, keyRow, keyCol, maxForRow, keyWidth, accummulativeWidth, tagCount = 1, baseHeight, leftRBtn = 15;
            String keyFaceName = "hebKeyFace.txt", keyHintName = "hebKeyHint.txt", keyWidthName = "keyWidths.txt", fullFileName;
            int[,] keyWidths, hebKeyCode;
            int rbtnTop = 20;
//            int[] rbtnLeft = { 15, 90, 224, 300 };
            int[] rbtnLeft;
            String[] radioButtonText = { "Notes", "Primary Search Word", "Secondary Search Word" };
            String[,] hebKeyFace, hebKeyHint, hebKeyVal;
            ToolTip[,] hebToolTips = new ToolTip[noOfRows, noOfCols];
            RadioButton rbtnTemp;
            Font typicalFont;

            fullFileName = globalVars.FullKeyboardFolder + @"\" + keyWidthName;
            keyWidths = loadKeyWidths(fullFileName, noOfRows, noOfCols);
            /***************************************
             * 
             * hebKeys: a global array containing all references to each key
             */
            hebKeys = new Button[noOfRows, noOfCols];

            /***************************************
             * 
             * keyCode: a global array containing the physical key data for each key (if scanned)
             */
            hebKeyCode = new int[noOfRows, noOfCols];
            hebKeyVal = new String[noOfRows, noOfCols];
            /****************************************
             * 
             * Now actually create the two sets of keys
             */
            fullFileName = globalVars.FullKeyboardFolder + @"\" + keyFaceName;
            hebKeyFace = loadFileData(fullFileName, noOfRows, noOfCols);
            fullFileName = globalVars.FullKeyboardFolder + @"\" + keyHintName;
            hebKeyHint = loadFileData(fullFileName, noOfRows, noOfCols);
            initialiseKeyCode(noOfRows, noOfCols, ref hebKeyCode, ref hebKeyVal);
            maxForRow = 0;
            baseHeight = 8;
            for (keyRow = 0; keyRow < noOfRows; keyRow++)
            {
                switch (keyRow)
                {
                    case 0:
                    case 1:
                    case 2: maxForRow = noOfCols; break;
                    case 3: maxForRow = 13; break;
                    case 4: maxForRow = 8; break;
                }
                accummulativeWidth = 16;
                for (keyCol = 0; keyCol < maxForRow; keyCol++)
                {
                    keyWidth = keyWidths[keyRow, keyCol];
                    hebKeys[keyRow, keyCol] = new Button();
                    if ((keyRow == 0) && (keyCol < maxForRow - 1)) hebKeys[keyRow, keyCol].Text = "\u25cc" + hebKeyFace[keyRow, keyCol];
                    else hebKeys[keyRow, keyCol].Text = hebKeyFace[keyRow, keyCol];
                    hebKeys[keyRow, keyCol].TextAlign = ContentAlignment.MiddleCenter;
                    hebKeys[keyRow, keyCol].Left = accummulativeWidth;
                    hebKeys[keyRow, keyCol].Top = baseHeight + (keyRow * (keyHeight + keyGap));
                    hebKeys[keyRow, keyCol].Height = keyHeight;
                    hebKeys[keyRow, keyCol].Width = keyWidth;
                    if ((keyRow < noOfRows - 1) && (keyCol == maxForRow - 1)) hebKeys[keyRow, keyCol].Font = new Font("Times New Roman", 10);
                    else hebKeys[keyRow, keyCol].Font = new Font("Times New Roman", 14);
                    hebKeys[keyRow, keyCol].Tag = tagCount;
                    hebKeys[keyRow, keyCol].Click += hebKeyboard_button_click;
                    mtKeyboardPanel.Invoke(new performPanelAddition(addToPanel), mtKeyboardPanel, hebKeys[keyRow, keyCol]); 

                    hebToolTips[keyRow, keyCol] = new ToolTip();
                    hebToolTips[keyRow, keyCol].AutomaticDelay = 200;
                    hebToolTips[keyRow, keyCol].AutoPopDelay = 2147483647;
                    hebToolTips[keyRow, keyCol].ToolTipTitle = "Key value";
//                    hebToolTips[keyRow, keyCol].SetToolTip(hebKeys[keyRow, keyCol], hebKeyHint[keyRow, keyCol]);
                    mtKeyboardPanel.Invoke(new performToolTipAddition(updateTooltip), hebToolTips[keyRow, keyCol], hebKeys[keyRow, keyCol], hebKeyHint[keyRow, keyCol]);

                    accummulativeWidth += keyWidth + keyGap;
                    tagCount++;
                }
            }
            rbtnLeft = new int[4];
            typicalFont = new Font( "Microsoft Sans Serif", 8.25F );
            for (idx = 0; idx < 3; idx++)
            {
                rbtnLeft[idx] = leftRBtn;
                leftRBtn += TextRenderer.MeasureText(radioButtonText[idx], typicalFont).Width + 25;
            }
            gbTextDestination = new GroupBox();
            gbTextDestination.Left = 44;
            gbTextDestination.Top = baseHeight + (noOfRows * (keyHeight + keyGap));
            gbTextDestination.Height = 50;
            gbTextDestination.Width = leftRBtn;
            gbTextDestination.Text = "Direct Hebrew text to: ";
            mtKeyboardPanel.Invoke(new performPanelGroup(addGroupToPanel), mtKeyboardPanel, gbTextDestination);
            leftRBtn = 15;
            globalVars.RbtnMTDestination = new RadioButton[3];
            for (idx = 0; idx < 3; idx++)
            {
                rbtnTemp = new RadioButton();
                rbtnTemp.Left = rbtnLeft[idx];
                rbtnTemp.Top = rbtnTop;
                rbtnTemp.AutoSize = true;
                rbtnTemp.Text = radioButtonText[idx];
                if (idx == 0) rbtnTemp.Checked = true;
                gbTextDestination.Invoke(new performGroupAddition(addRButtonToGroup), gbTextDestination, rbtnTemp);
                globalVars.RbtnMTDestination[idx] = rbtnTemp;
            }
            labEnteredTextLbl = new Label();
            labEnteredTextLbl.Left = gbTextDestination.Left + gbTextDestination.Width + 10; ;
            labEnteredTextLbl.Top = gbTextDestination.Top + (gbTextDestination.Height / 2) - 4;
            labEnteredTextLbl.AutoSize = true;
            labEnteredTextLbl.Text = "Entered text:";
            mtKeyboardPanel.Invoke(new performAddLabel(addLabelToPanel), mtKeyboardPanel, labEnteredTextLbl);
            txtHebEnteredText = new TextBox();
            txtHebEnteredText.Left = labEnteredTextLbl.Left + labEnteredTextLbl.Width + 5;
            txtHebEnteredText.Top = labEnteredTextLbl.Top - 10;
            txtHebEnteredText.Width = 120;
            txtHebEnteredText.Font = new Font( "Times New Roman", 18F, FontStyle.Regular );
            mtKeyboardPanel.Invoke( new performAddTextbox(addTextboxToPanel), mtKeyboardPanel, txtHebEnteredText);
            btnHebUse = new Button();
            btnHebUse.Left = labEnteredTextLbl.Left + labEnteredTextLbl.Width + 25;
            btnHebUse.Top = labEnteredTextLbl.Top - keyHeight - 15;
            btnHebUse.Height = keyHeight;
            btnHebUse.Width = keyWidths[3, 11];
            btnHebUse.Text = "Use word";
            btnHebUse.Tag = 1;
            btnHebUse.Click += respondToUseButton;
            mtKeyboardPanel.Invoke(new performPanelAddition(addToPanel), mtKeyboardPanel, btnHebUse);
        }

        private void initialiseKeyCode(int x, int y, ref int[,] hebKeyCode, ref String[,] hebKeyVal)
        {
            /*=============================================================================================================*
             *                                                                                                             *
             *                                              initialiseKeyCode                                              *
             *                                              ================                                               *
             *                                                                                                             *
             *  An integral part of the method, setupHebKeyboard (above).                                                  *
             *                                                                                                             *
             *=============================================================================================================*/
            int idx, jdx;

            hebKeyCode = new int[x, y];
            hebKeyVal = new string[x, y];
            for (idx = 0; idx < x; idx++)
            {
                for (jdx = 0; jdx < y; jdx++)
                {
                    hebKeyCode[idx, jdx] = -1;
                    hebKeyVal[idx, jdx] = "";
                }
            }
        }

        private String[,] loadFileData(String fileName, int noOfRows, int noOfCols)
        {
            /*=============================================================================================================*
             *                                                                                                             *
             *                                                loadFileData                                                 *
             *                                                ============                                                 *
             *                                                                                                             *
             *  An integral part of the method, setupHebKeyboard (above).  Used as a somewhat generic mechanism for        *
             *    loading:                                                                                                 *
             *                                                                                                             *
             *    a) Gk and Heb characters;                                                                                *
             *    b) Information for key hints                                                                             *
             *    c) Heb accents                                                                                           *
             *                                                                                                             *
             *  It can be used in a number of files (hence the parameter, fileName).                                       *
             *                                                                                                             *
             *=============================================================================================================*/
            int idx, jdx;
            String fileBuffer;
            String[,] targetArray;
            StreamReader srSource;

            srSource = new StreamReader(fileName);
            targetArray = new String[noOfRows, noOfCols];
            fileBuffer = srSource.ReadLine();
            idx = 0; jdx = 0;
            while (fileBuffer != null)
            {
                if ((fileBuffer.Length > 0) && (fileBuffer[0] == '\\'))
                {
                    fileBuffer = String.Format("{0:C}", (char)int.Parse(fileBuffer.Substring(2), System.Globalization.NumberStyles.HexNumber));
                }
                targetArray[idx, jdx] = fileBuffer;
                if (++jdx == noOfCols)
                {
                    jdx = 0;
                    if (++idx == noOfRows) break;
                }
                fileBuffer = srSource.ReadLine();
            }
            srSource.Close();
            return targetArray;
        }

        private int[,] loadKeyWidths(String fullFileName, int noOfRows, int noOfCols)
        {
            /*=============================================================================================================*
             *                                                                                                             *
             *                                               loadKeyWidths                                                 *
             *                                               =============                                                 *
             *                                                                                                             *
             *  An integral part of the method, setupHebKeyboard (above).  Much as loadFileData (above) but it has to be   *
             *    a different method because the values stored are of a different data type (integer rather than string).  *
             *                                                                                                             *
             *=============================================================================================================*/
            int idx, jdx;
            String fileBuffer;
            int[,] targetArray;
            StreamReader srSource;

            srSource = new StreamReader(fullFileName);
            targetArray = new int[noOfRows, noOfCols];
            fileBuffer = srSource.ReadLine();
            idx = 0; jdx = 0;
            while (fileBuffer != null)
            {
                targetArray[idx, jdx] = Convert.ToInt32(fileBuffer);
                fileBuffer = srSource.ReadLine();
                if (++jdx == noOfCols)
                {
                    jdx = 0;
                    if (++idx == noOfRows) break;
                }
            }
            srSource.Close();
            return targetArray;
        }

        private void hebKeyboard_button_click(object sender, EventArgs e)
        {
            /*=============================================================================================================*
             *                                                                                                             *
             *                                          hebKeyboard_button_click                                           *
             *                                          ========================                                           *
             *                                                                                                             *
             *  This is the callback method, called by the click event for each of the buttons of the viryual keyboard.    *
             *                                                                                                             *
             *                                                                                                             *
             *  An integral part of the method, setupHebKeyboard (above).                                                  *
             *                                                                                                             *
             *=============================================================================================================*/
            int clickedTag;
            String keyVal = "";
            Button clickedButton;

            clickedButton = (Button)sender;
            clickedTag = Convert.ToInt32(clickedButton.Tag.ToString());
            switch (clickedTag)
            {
                case 1: keyVal = "\u05B1"; break; // Various accents
                case 2: keyVal = "\u05B3"; break;
                case 3: keyVal = "\u05B2"; break;
                case 4: keyVal = "\u05B6"; break;
                case 5: keyVal = "\u05BC"; break;
                case 6: keyVal = "\u05B5"; break;
                case 7: keyVal = "\u05B4"; break;
                case 8: keyVal = "\u05B0"; break;
                case 9: keyVal = "\u05B8"; break;
                case 10: keyVal = "\u05B7"; break;
                case 11: keyVal = "\u05BB"; break;
                case 12: keyVal = "\u05B9"; break;
                case 13: keyVal = "\u0008"; break;  // Backspace
                case 14: keyVal = "\u05C1"; break;  // Sin/Shin dots
                case 15: keyVal = "\u05C2"; break;
                case 16: keyVal = "\u05E7"; break;  // First row of normal characters
                case 17: keyVal = "\u05E8"; break;
                case 18: keyVal = "\u05D0"; break;
                case 19: keyVal = "\u05D8"; break;
                case 20: keyVal = "\u05D5"; break;
                case 21: keyVal = "\u05DF"; break;
                case 22: keyVal = "\u05DD"; break;
                case 23: keyVal = "\u05E4"; break;
                case 24: keyVal = "\u05BE"; break;  // Mappeq
                case 25: keyVal = "\u05ab"; break;  // Generic accent
                case 26: keyVal = "\u0008"; break;  // Del = backspace

                case 27: keyVal = "\u05E9"; break;  // Second row of normal characters
                case 28: keyVal = "\u05D3"; break;
                case 29: keyVal = "\u05D2"; break;
                case 30: keyVal = "\u05DB"; break;
                case 31: keyVal = "\u05E2"; break;
                case 32: keyVal = "\u05D9"; break;
                case 33: keyVal = "\u05D7"; break;
                case 34: keyVal = "\u05DC"; break;
                case 35: keyVal = "\u05DA"; break;
                case 36: keyVal = "\u05E3"; break;
                case 37: keyVal = "\u05C3"; break;
                case 39: keyVal = "\u000d"; break;  // Clear

                case 41: keyVal = "\u05D6"; break;  // Third row of normal characters (key 40 is inactive)
                case 42: keyVal = "\u05E1"; break;
                case 43: keyVal = "\u05D1"; break;
                case 44: keyVal = "\u05D4"; break;
                case 45: keyVal = "\u05E0"; break;
                case 46: keyVal = "\u05DE"; break;
                case 47: keyVal = "\u05E6"; break;
                case 48: keyVal = "\u05EA"; break;
                case 49: keyVal = "\u05E5"; break;
                case 51: keyVal = " "; break;
                default: break;
            }
            updateHebTextbox(keyVal, clickedTag);
        }

        public void setupGkKeyboard()
        {
            /*=============================================================================================================*
             *                                                                                                             *
             *                                              setupGkKeyboard                                                *
             *                                              ===============                                                *
             *                                                                                                             *
             *  Specific to the virtual keyboard for Greek.  Because we have to manage the change of characters as accents *
             *    or other non-alphabetic elements are added, the processing is more complex than for Hebrew.              *
             *                                                                                                             *
             *=============================================================================================================*/
            const int noOfRows = 4, noOfCols = 13, keyGap = 4, keyHeight = 30;

            int idx, keyRow, keyCol, maxForRow, keyWidth, accummulativeWidth, tagCount = 1, baseHeight, rbtnTop = 20, leftRBtn = 15;
            String keyFaceMinName = "gkKeyFaceMin.txt", keyFaceMajName = "gkKeyFaceMaj.txt", keyHintMinName = "gkKeyHintMin.txt", keyHintMajName = "gkKeyHintMaj.txt", 
                keyWidthName = "keyWidths.txt", fullFileName, fileBuffer;
            int[,] keyWidths, gkKeyCode;
//            int[] rbtnLeft = { 15, 90, 224, 300 };
            int[] rbtnLeft;
            String[] radioButtonText = { "Notes", "Primary Search Word", "Secondary Search Word" };
            String[,] gkKeyFace, gkKeyHint, gkKeyVal;
            StreamReader srSource;
            ToolTip[,] gkToolTips = new ToolTip[noOfRows, noOfCols];
            RadioButton rbtnTemp;
            Font typicalFont;

            isGkMiniscule = true;
            fullFileName = globalVars.FullKeyboardFolder + @"\" + keyWidthName;
            keyWidths = loadKeyWidths(fullFileName, noOfRows, noOfCols);
            /***************************************
             * 
             * gkKeys: a global array containing all references to each key
             */
            gkKeys = new Button[noOfRows, noOfCols];

            /***************************************
             * 
             * keyCode: a global array containing the physical key data for each key (if scanned)
             */
            gkKeyCode = new int[noOfRows, noOfCols];
            gkKeyVal = new String[noOfRows, noOfCols];
            /****************************************
             * 
             * Now actually create the two sets of keys
             */
            fullFileName = globalVars.FullKeyboardFolder + @"\" + keyFaceMinName;
            gkKeyFace = loadFileData(fullFileName, noOfRows, noOfCols);
            fullFileName = globalVars.FullKeyboardFolder + @"\" + keyHintMinName;
            gkKeyHint = loadFileData(fullFileName, noOfRows, noOfCols);
            initialiseGkKeyCode(noOfRows, noOfCols, ref gkKeyCode, ref gkKeyVal);
            maxForRow = 0;
            baseHeight = 8;
            for (keyRow = 0; keyRow < noOfRows; keyRow++)
            {
                switch (keyRow)
                {
                    case 0:
                    case 1:
                    case 2: maxForRow = noOfCols; break;
                    case 3: maxForRow = 13; break;
                    case 4: maxForRow = 8; break;
                }
                accummulativeWidth = 16;
                for (keyCol = 0; keyCol < maxForRow; keyCol++)
                {
                    keyWidth = keyWidths[keyRow, keyCol];
                    gkKeys[keyRow, keyCol] = new Button();
                    gkKeys[keyRow, keyCol].Text = gkKeyFace[keyRow, keyCol];
                    minisculeKeyFace.Add(tagCount, gkKeyFace[keyRow, keyCol]);
                    gkKeys[keyRow, keyCol].TextAlign = ContentAlignment.MiddleCenter;
                    gkKeys[keyRow, keyCol].Left = accummulativeWidth;
                    gkKeys[keyRow, keyCol].Top = baseHeight + (keyRow * (keyHeight + keyGap));
                    gkKeys[keyRow, keyCol].Height = keyHeight;
                    gkKeys[keyRow, keyCol].Width = keyWidth;
                    gkKeys[keyRow, keyCol].Font = new Font("Times New Roman", 10);
                    gkKeys[keyRow, keyCol].Tag = tagCount;
                    gkKeys[keyRow, keyCol].Click += gkKeyboard_button_click;
                    lxxKeyboardPanel.Invoke(new performPanelAddition(addToPanel), lxxKeyboardPanel, gkKeys[keyRow, keyCol]);

                    gkToolTips[keyRow, keyCol] = new ToolTip();
                    gkToolTips[keyRow, keyCol].AutomaticDelay = 200;
                    gkToolTips[keyRow, keyCol].AutoPopDelay = 2147483647;
                    gkToolTips[keyRow, keyCol].ToolTipTitle = "Key value";
                    lxxKeyboardPanel.Invoke(new performToolTipAddition(updateTooltip), gkToolTips[keyRow, keyCol], gkKeys[keyRow, keyCol], gkKeyHint[keyRow, keyCol]);

                    accummulativeWidth += keyWidth + keyGap;
                    tagCount++;
                }
            }
            rbtnLeft = new int[4];
            typicalFont = new Font("Microsoft Sans Serif", 8.25F);
            for (idx = 0; idx < 3; idx++)
            {
                rbtnLeft[idx] = leftRBtn;
                leftRBtn += TextRenderer.MeasureText(radioButtonText[idx], typicalFont).Width + 25;
            }
            gbTextDestination = new GroupBox();
            gbTextDestination.Left = 44;
            gbTextDestination.Top = baseHeight + (noOfRows * (keyHeight + keyGap));
            gbTextDestination.Height = 50;
            gbTextDestination.Width = leftRBtn;
            gbTextDestination.Text = "Direct Greek text to: ";
            //            mtKeyboardPanel.Controls.Add(gbTextDestination);
            lxxKeyboardPanel.Invoke(new performPanelGroup(addGroupToPanel), lxxKeyboardPanel, gbTextDestination);
            leftRBtn = 15;
            globalVars.RbtnLXXDestination = new RadioButton[3];
            for (idx = 0; idx < 3; idx++)
            {
                rbtnTemp = new RadioButton();
                rbtnTemp.Left = rbtnLeft[idx];
                rbtnTemp.Top = rbtnTop;
                rbtnTemp.AutoSize = true;
                rbtnTemp.Text = radioButtonText[idx];
                if (idx == 0) rbtnTemp.Checked = true;
                gbTextDestination.Invoke(new performGroupAddition(addRButtonToGroup), gbTextDestination, rbtnTemp);
                globalVars.RbtnLXXDestination[idx] = rbtnTemp;
                leftRBtn += TextRenderer.MeasureText(radioButtonText[idx], rbtnTemp.Font).Width + 25;
            }
            labEnteredTextLbl = new Label();
            labEnteredTextLbl.Left = gbTextDestination.Left + gbTextDestination.Width + 10; ;
            labEnteredTextLbl.Top = gbTextDestination.Top + (gbTextDestination.Height / 2) - 4;
            labEnteredTextLbl.AutoSize = true;
            labEnteredTextLbl.Text = "Entered text:";
            lxxKeyboardPanel.Invoke(new performAddLabel(addLabelToPanel), lxxKeyboardPanel, labEnteredTextLbl);
            txtGkEnteredText = new TextBox();
            txtGkEnteredText.Left = labEnteredTextLbl.Left + labEnteredTextLbl.Width + 5;
            txtGkEnteredText.Top = labEnteredTextLbl.Top - 5;
            txtGkEnteredText.Width = 120;
            txtGkEnteredText.Font = new Font("Times New Roman", 12F, FontStyle.Regular);
            lxxKeyboardPanel.Invoke(new performAddTextbox(addTextboxToPanel), lxxKeyboardPanel, txtGkEnteredText);
            btnGkUse = new Button();
            btnGkUse.Left = labEnteredTextLbl.Left + labEnteredTextLbl.Width + 25;
            btnGkUse.Top = labEnteredTextLbl.Top - keyHeight - 15;
            btnGkUse.Height = keyHeight;
            btnGkUse.Width = keyWidths[3,11];
            btnGkUse.Text = "Use word";
            btnGkUse.Tag = 2;
            btnGkUse.Click += respondToUseButton;
            lxxKeyboardPanel.Invoke(new performPanelAddition(addToPanel), lxxKeyboardPanel, btnGkUse);
            // Load the majiscule key-face text, ready;
            tagCount = 1;
            fullFileName = globalVars.FullKeyboardFolder + @"\" + keyFaceMajName;
            srSource = new StreamReader(fullFileName);
            fileBuffer = srSource.ReadLine();
            while (fileBuffer != null)
            {
                if ((fileBuffer.Length > 0) && (fileBuffer[0] == '\\'))
                {
                    fileBuffer = String.Format("{0:C}", (char)int.Parse(fileBuffer.Substring(2), System.Globalization.NumberStyles.HexNumber));
                }
                majisculeKeyFace.Add(tagCount++, fileBuffer);
                fileBuffer = srSource.ReadLine();
            }
            srSource.Close();

        }

        private void respondToUseButton(object sender, EventArgs e)
        {
            int tagVal;
            Button currentButton;

            currentButton = (Button)sender;
            tagVal = Convert.ToInt32(currentButton.Tag);
            if (tagVal == 1)
            {
                if (globalVars.RbtnMTDestination[0].Checked) updateNotes(tagVal, (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 2));
                if (globalVars.RbtnMTDestination[1].Checked) updateSearchBox(tagVal, (TextBox)globalVars.getGroupedControl(globalVars.TextboxCode, 0), false);
                if (globalVars.RbtnMTDestination[2].Checked) updateSearchBox(tagVal, (TextBox)globalVars.getGroupedControl(globalVars.TextboxCode, 1), true);
            }
            else
            {
                if (globalVars.RbtnLXXDestination[0].Checked) updateNotes(tagVal, (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 3));
                if (globalVars.RbtnLXXDestination[1].Checked) updateSearchBox(tagVal, (TextBox)globalVars.getGroupedControl(globalVars.TextboxCode, 2), false);
                if (globalVars.RbtnLXXDestination[2].Checked) updateSearchBox(tagVal, (TextBox)globalVars.getGroupedControl(globalVars.TextboxCode, 3), true);
            }
        }


        private void initialiseGkKeyCode(int x, int y, ref int[,] gkKeyCode, ref String[,] gkKeyVal)
        {
            /*=============================================================================================================*
             *                                                                                                             *
             *                                             initialiseGkKeyCode                                             *
             *                                             ===================                                             *
             *                                                                                                             *
             *  An integral part of the method, setupGkKeyboard (above).                                                   *
             *                                                                                                             *
             *=============================================================================================================*/
            int idx, jdx;

            gkKeyCode = new int[x, y];
            gkKeyVal = new string[x, y];
            for (idx = 0; idx < x; idx++)
            {
                for (jdx = 0; jdx < y; jdx++)
                {
                    gkKeyCode[idx, jdx] = -1;
                    gkKeyVal[idx, jdx] = "";
                }
            }
        }
        private void gkKeyboard_button_click(object sender, EventArgs e)
        {
            /*==========================================================================================================*
             *                                                                                                          *
             *                                        gkKeyboard_button_click                                           *
             *                                        =======================                                           *
             *                                                                                                          *
             *  Specifically handles key presses from the Greek virtual keyboard.  (see hebKeyboard_button_click for    *
             *    responses to the Hebrew keyboard).                                                                    *
             *                                                                                                          *
             *==========================================================================================================*/
            bool isNormalChar = false, isModifyingChar = false, isNormalNonGk = false;
            int clickedTag;
            String keyVal = "";
            Button clickedButton;

            clickedButton = (Button)sender;
            clickedTag = Convert.ToInt32(clickedButton.Tag.ToString());
            if (isGkMiniscule)
            {
                if ((clickedTag > 0) && (clickedTag < 11)) isNormalNonGk = true;
                if ((clickedTag == 11) || (clickedTag == 12) || (clickedTag == 14 )) isModifyingChar = true;
                if ((clickedTag > 14) && (clickedTag < 24)) isNormalChar = true;
                if ((clickedTag == 24) || (clickedTag == 25)) isModifyingChar = true;
                if ((clickedTag > 27) && (clickedTag < 37)) isNormalChar = true;
                if ((clickedTag == 37) || (clickedTag == 38)) isModifyingChar = true;
                if ((clickedTag > 40) && (clickedTag < 48)) isNormalChar = true;
                if ((clickedTag == 48) || (clickedTag == 49) || (clickedTag == 51 )) isNormalChar = true;
                if (isNormalChar || isNormalNonGk) keyVal = clickedButton.Text;
                if (clickedTag == 51) keyVal = " ";
            }
            if ((clickedTag == 27) || (clickedTag == 40) || (clickedTag == 50)) handleShiftAndCapsLoc(clickedTag);
            else
            {
                updateTextbox(keyVal, clickedTag, isModifyingChar, isNormalChar);
        /*        if (globalVars.RbtnLXXDestination[0].Checked)
                    updateGkNotes(keyVal, clickedTag, isModifyingChar, isNormalChar, (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 3));
                if (globalVars.RbtnLXXDestination[1].Checked)
                    updateGkSearchBox(keyVal, clickedTag, isModifyingChar, isNormalNonGk, (TextBox)globalVars.getGroupedControl(globalVars.TextboxCode, 2), false);
                if (globalVars.RbtnLXXDestination[2].Checked)
                    updateGkSearchBox(keyVal, clickedTag, isModifyingChar, isNormalNonGk, (TextBox)globalVars.getGroupedControl(globalVars.TextboxCode, 3), true); */
            }
            if ( isNormalChar)
            {
                if( isShiftDown && ( ! isCarriageReturnDown ) )
                {
                    resetGkKeyboard(true);
                    isShiftDown = false;
                }
            }
        }

        private void handleShiftAndCapsLoc( int keyCode)
        {
            if( keyCode == 27)
            {
                if( isCarriageReturnDown)
                {
                    resetGkKeyboard(true);
                    isCarriageReturnDown = false;
                }
                else
                {
                    resetGkKeyboard(false);
                    isCarriageReturnDown = true;
                }
            }
            else
            {
                if( ! isCarriageReturnDown)
                {
                    if( isShiftDown)
                    {
                        resetGkKeyboard(true);
                        isShiftDown = false;
                    }
                    else
                    {
                        resetGkKeyboard(false);
                        isShiftDown = true;
                    }
                }
            }
        }

        private void resetGkKeyboard(bool isMiniscule)
        {
            const int noOfRows = 4, noOfCols = 13;

            int keyRow, keyCol, maxForRow = 0, tagCount = 1;
            String keyText;

            for (keyRow = 0; keyRow < noOfRows; keyRow++)
            {
                switch (keyRow)
                {
                    case 0:
                    case 1:
                    case 2: maxForRow = noOfCols; break;
                    case 3: maxForRow = 13; break;
                    case 4: maxForRow = 8; break;
                }
                for (keyCol = 0; keyCol < maxForRow; keyCol++)
                {
                    if (isMiniscule) minisculeKeyFace.TryGetValue(tagCount, out keyText);
                    else majisculeKeyFace.TryGetValue(tagCount, out keyText);
                    gkKeys[keyRow, keyCol].Text = keyText;
                    tagCount++;
                }
            }
        }

        private void updateTextbox(String keyVal, int clickCode, bool isModifyingChar, bool isNormalChar)
        {
            int currPstn;
            String fullNote, previousChar, replacementChar = null, beforeCursor, afterCursor;

            fullNote = txtGkEnteredText.Text;
            currPstn = txtGkEnteredText.SelectionStart;
            if (isModifyingChar)
            {
                if ((fullNote == null) || (fullNote.Length == 0)) return;
                if (currPstn == 0) return;
                if (currPstn == fullNote.Length - 1)
                {
                    previousChar = txtGkEnteredText.Text.Substring(txtGkEnteredText.Text.Length - 1);
                    replacementChar = modifyCharacter(previousChar, clickCode);
                    txtGkEnteredText.Text = txtGkEnteredText.Text.Substring(0, txtGkEnteredText.Text.Length - 1) + replacementChar;
                }
                else
                {
                    beforeCursor = fullNote.Substring(0, currPstn);
                    afterCursor = fullNote.Substring(currPstn);
                    previousChar = beforeCursor.Substring(beforeCursor.Length - 1);
                    replacementChar = modifyCharacter(previousChar, clickCode);
                    txtGkEnteredText.Text = beforeCursor.Substring(0, beforeCursor.Length - 1) + replacementChar + afterCursor;
                    currPstn = beforeCursor.Length;
                    txtGkEnteredText.SelectionStart = currPstn;
                }
            }
            else
            {
                if (isNormalChar)
                {
                    if ((fullNote == null) || (fullNote.Length == 0))
                    {
                        txtGkEnteredText.Text = keyVal;
                        txtGkEnteredText.SelectionStart = 1;
                        return;
                    }
                    if (currPstn == 0)
                    {
                        txtGkEnteredText.Text = keyVal + fullNote;
                        txtGkEnteredText.SelectionStart = 1;
                        return;
                    }
                    if (currPstn == fullNote.Length - 1)
                    {
                        txtGkEnteredText.Text = fullNote + keyVal;
                        txtGkEnteredText.SelectionStart = ++currPstn;
                        return;
                    }
                    beforeCursor = fullNote.Substring(0, currPstn);
                    afterCursor = fullNote.Substring(currPstn);
                    txtGkEnteredText.Text = beforeCursor + keyVal + afterCursor;
                    txtGkEnteredText.SelectionStart = ++currPstn;
                }
                else
                {
                    switch (clickCode)
                    {
                        case 13: // Backspace
                        case 26: // Delete - treated as backspace
                            if ((fullNote == null) || (fullNote.Length == 0)) return;
                            if (currPstn == 0) return;
                            if (currPstn == fullNote.Length - 1)
                            {
                                txtGkEnteredText.Text = fullNote.Substring(0, fullNote.Length - 1);
                                txtGkEnteredText.SelectionStart = --currPstn;
                            }
                            else
                            {
                                beforeCursor = fullNote.Substring(0, currPstn);
                                afterCursor = fullNote.Substring(currPstn);
                                txtGkEnteredText.Text = beforeCursor.Substring(0, beforeCursor.Length - 1) + afterCursor;
                                currPstn = beforeCursor.Length - 1;
                                txtGkEnteredText.SelectionStart = currPstn;
                            }
                            break;
                        case 39: txtGkEnteredText.Text = ""; break;
                    }
                }
            }
        }

/*        private void updateGkNotesXX( String keyVal, int clickCode, bool isModifyingChar, bool isNormalChar, RichTextBox rtxtNotes)
        {
            int currPstn;
            String fullNote, previousChar, replacementChar = null, beforeCursor, afterCursor;

            ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 1)).SelectedIndex = 1;
            ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 6)).SelectedIndex = 0;
            fullNote = rtxtNotes.Text;
            currPstn = rtxtNotes.SelectionStart;
            if (isModifyingChar)
            {
                if ((fullNote == null) || (fullNote.Length == 0)) return;
                if (currPstn == 0) return;
                if (currPstn == fullNote.Length - 1)
                {
                    previousChar = rtxtNotes.Text.Substring(rtxtNotes.Text.Length - 1);
                    replacementChar = modifyCharacter(previousChar, clickCode);
                    rtxtNotes.Text = rtxtNotes.Text.Substring(0, rtxtNotes.Text.Length - 1) + replacementChar;
                }
                else
                {
                    beforeCursor = fullNote.Substring(0, currPstn);
                    afterCursor = fullNote.Substring(currPstn);
                    previousChar = beforeCursor.Substring(beforeCursor.Length - 1);
                    replacementChar = modifyCharacter(previousChar, clickCode);
                    rtxtNotes.Text = beforeCursor.Substring(0, beforeCursor.Length - 1) + replacementChar + afterCursor;
                    currPstn = beforeCursor.Length;
                    rtxtNotes.SelectionStart = currPstn;
                }
            }
            else
            {
                if (isNormalChar)
                {
                    if ((fullNote == null) || (fullNote.Length == 0))
                    {
                        rtxtNotes.Text = keyVal;
                        rtxtNotes.SelectionStart = 1;
                        return;
                    }
                    if (currPstn == 0)
                    {
                        rtxtNotes.Text = keyVal + fullNote;
                        rtxtNotes.SelectionStart = 1;
                        return;
                    }
                    if (currPstn == fullNote.Length - 1)
                    {
                        rtxtNotes.Text = fullNote + keyVal;
                        rtxtNotes.SelectionStart = ++currPstn;
                        return;
                    }
                    beforeCursor = fullNote.Substring(0, currPstn);
                    afterCursor = fullNote.Substring(currPstn);
                    rtxtNotes.Text = beforeCursor + keyVal + afterCursor;
                    rtxtNotes.SelectionStart = ++currPstn;
                }
                else
                {
                    switch( clickCode )
                    {
                        case 13: // Backspace
                        case 26: // Delete - treated as backspace
                            if ((fullNote == null) || (fullNote.Length == 0)) return;
                            if (currPstn == 0) return;
                            if (currPstn == fullNote.Length - 1)
                            {
                                rtxtNotes.Text = fullNote.Substring(0, fullNote.Length - 1);
                                rtxtNotes.SelectionStart = --currPstn;
                            }
                            else
                            {
                                beforeCursor = fullNote.Substring(0, currPstn);
                                afterCursor = fullNote.Substring(currPstn);
                                rtxtNotes.Text = beforeCursor.Substring( 0, beforeCursor.Length - 1) + afterCursor;
                                currPstn = beforeCursor.Length - 1;
                                rtxtNotes.SelectionStart = currPstn;
                            }
                            break;
                        case 39: break; // ignore clear
                    }
                }
            }
        } */

        private String modifyCharacter( String startingCharacter, int clickCode)
        {
            String replacementChar = null;

            switch (clickCode)
            {
                case 11: replacementChar = greekOrthography.getCharacterWithDieresis(startingCharacter); break;
                case 12: replacementChar = greekOrthography.getCharacterWithAccuteAccent(startingCharacter); break;
                case 14: replacementChar = greekOrthography.getCharacterWithIotaSubscript(startingCharacter); break;
                case 24: replacementChar = greekOrthography.getCharacterWithCircumflexAccent(startingCharacter); break;
                case 25: replacementChar = greekOrthography.getCharacterWithGraveAccent(startingCharacter); break;
                case 37: replacementChar = greekOrthography.getCharacterWithRoughBreathing(startingCharacter); break;
                case 38: replacementChar = greekOrthography.getCharacterWithSmoothBreathing(startingCharacter); break;
            }
            if (replacementChar == null) return startingCharacter;
            return replacementChar;
        }

/*        private void updateGkSearchBox(String keyVal, int clickCode, bool isModifyingChar, bool isNonGreek, TextBox txtTarget, bool isSecondary)
        {
            int currPstn;
            String fullText, previousChar, replacementChar = null, beforeCursor, afterCursor;
            Label lblFirst, lblSecond;
            Button btnSecondarySearch;
            NumericUpDown udSearch;

            if (isNonGreek) return;  //Digits and punctuation are not suitable here.
            if ((clickCode == 48) || (clickCode == 49) || (clickCode == 51)) return;  // The same is true of space and punctuation that is marked as normal
            ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 1)).SelectedIndex = 1;
            ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 6)).SelectedIndex = 2;
            if (isSecondary)
            {
                lblFirst = (Label)globalVars.getGroupedControl(globalVars.LabelCode, 4);  // within
                lblSecond = (Label)globalVars.getGroupedControl(globalVars.LabelCode, 5);  // wordsOf
                udSearch = (NumericUpDown)globalVars.UdLXXScan;
                btnSecondarySearch = (Button)globalVars.getGroupedControl(globalVars.ButtonCode, 1);
                lblFirst.Visible = true;
                lblSecond.Visible = true;
                udSearch.Visible = true;
                txtTarget.Visible = true;
                btnSecondarySearch.Text = "Basic Search";
            }
            fullText = txtTarget.Text;
            currPstn = txtTarget.SelectionStart;
            if (isModifyingChar)
            {
                if ((fullText == null) || (fullText.Length == 0)) return;
                if (currPstn == 0) return;
                if (currPstn == fullText.Length - 1)
                {
                    previousChar = fullText.Substring(fullText.Length - 1);
                    replacementChar = modifyCharacter(previousChar, clickCode);
                    txtTarget.Text = fullText.Substring(0, fullText.Length - 1) + replacementChar;
                }
                else
                {
                    beforeCursor = fullText.Substring(0, currPstn);
                    afterCursor = fullText.Substring(currPstn);
                    previousChar = beforeCursor.Substring(beforeCursor.Length - 1);
                    replacementChar = modifyCharacter(previousChar, clickCode);
                    txtTarget.Text = beforeCursor.Substring(0, beforeCursor.Length - 1) + replacementChar + afterCursor;
                    currPstn = beforeCursor.Length;
                    txtTarget.SelectionStart = currPstn;
                }
            }
            else
            {
                if ((clickCode == 13) || (clickCode == 26) || (clickCode == 39) )
                {
                    if ((clickCode == 13) || (clickCode == 26))
                    {
                        if ((fullText == null) || (fullText.Length == 0)) return;
                        if (currPstn == 0) return;
                        if (currPstn == fullText.Length - 1)
                        {
                            txtTarget.Text = fullText.Substring(0, fullText.Length - 1);
                            txtTarget.SelectionStart = --currPstn;
                        }
                        else
                        {
                            beforeCursor = fullText.Substring(0, currPstn);
                            afterCursor = fullText.Substring(currPstn);
                            txtTarget.Text = beforeCursor.Substring(0, beforeCursor.Length - 1) + afterCursor;
                            currPstn = beforeCursor.Length - 1;
                            txtTarget.SelectionStart = currPstn;
                        }
                    }
                    else txtTarget.Text = "";
                }
                else
                {
                    // Process normal Greek character
                    if ((fullText == null) || (fullText.Length == 0))
                    {
                        txtTarget.Text = keyVal;
                        txtTarget.SelectionStart = 1;
                        return;
                    }
                    if (currPstn == 0)
                    {
                        txtTarget.Text = keyVal + fullText;
                        txtTarget.SelectionStart = 1;
                        return;
                    }
                    if (currPstn == fullText.Length - 1)
                    {
                        txtTarget.Text = fullText + keyVal;
                        txtTarget.SelectionStart = ++currPstn;
                        return;
                    }
                    beforeCursor = fullText.Substring(0, currPstn);
                    afterCursor = fullText.Substring(currPstn);
                    txtTarget.Text = beforeCursor + keyVal + afterCursor;
                    txtTarget.SelectionStart = ++currPstn;
                }
            }
        } */

        private void updateHebTextbox(String unicodeRepresentation, int tagValue)
        {
            bool isNormalChar = false;
            int currPstn;
            String fullNote, beforeCursor, afterCursor;

            fullNote = txtHebEnteredText.Text;
            currPstn = txtHebEnteredText.SelectionStart;
            if ((tagValue >= 1) && (tagValue <= 12)) isNormalChar = true;
            if ((tagValue >= 14) && (tagValue <= 25)) isNormalChar = true;
            if ((tagValue >= 27) && (tagValue <= 37)) isNormalChar = true;
            if ((tagValue >= 41) && (tagValue <= 51)) isNormalChar = true;
            if (isNormalChar)
            {
                if ((fullNote == null) || (fullNote.Length == 0))
                {
                    txtHebEnteredText.Text = unicodeRepresentation;
                    txtHebEnteredText.SelectionStart = 1;
                    return;
                }
                if (currPstn == 0)
                {
                    txtHebEnteredText.Text = unicodeRepresentation + fullNote;
                    txtHebEnteredText.SelectionStart = 1;
                    return;
                }
                if (currPstn == fullNote.Length - 1)
                {
                    txtHebEnteredText.Text = fullNote + unicodeRepresentation;
                    txtHebEnteredText.SelectionStart = ++currPstn;
                    return;
                }
                beforeCursor = fullNote.Substring(0, currPstn);
                afterCursor = fullNote.Substring(currPstn);
                txtHebEnteredText.Text = beforeCursor + unicodeRepresentation + afterCursor;
                txtHebEnteredText.SelectionStart = ++currPstn;
            }
            else
            {
                switch (tagValue)
                {
                    case 13:
                    case 26:
                        if ((fullNote == null) || (fullNote.Length == 0)) break; // do nothing
                        if (currPstn == 0) break; // do nothing
                        beforeCursor = fullNote.Substring(0, currPstn);
                        afterCursor = fullNote.Substring(currPstn);
                        txtHebEnteredText.Text = beforeCursor.Substring(0, beforeCursor.Length - 1) + afterCursor;
                        txtHebEnteredText.SelectionStart = --currPstn;
                        break;
                    case 39: txtHebEnteredText.Text = ""; break;
                }
            }
        }

        private void updateNotes(int tagValue, RichTextBox rtxtNotes)
        {
            int currPstn;
            String newWord, fullNote, beforeCursor, afterCursor;

            if (tagValue == 1)
            {
                newWord = txtHebEnteredText.Text;
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 1)).SelectedIndex = 0;
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 5)).SelectedIndex = 0;
            }
            else
            {
                newWord = txtGkEnteredText.Text;
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 1)).SelectedIndex = 1;
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 6)).SelectedIndex = 0;
            }
            fullNote = rtxtNotes.Text;
            currPstn = rtxtNotes.SelectionStart;
            //                simpleRtxtUpdate(rtxtNotes, fullNote, currPstn, unicodeRepresentation);

            if ((fullNote == null) || (fullNote.Length == 0))
            {
                rtxtNotes.Text = newWord;
                rtxtNotes.SelectionStart = rtxtNotes.Text.Length;
                return;
            }
            if (currPstn == 0)
            {
                rtxtNotes.Text = newWord + fullNote;
                rtxtNotes.SelectionStart = newWord.Length;
                return;
            }
            if (currPstn == fullNote.Length - 1)
            {
                rtxtNotes.Text = fullNote + newWord;
                rtxtNotes.SelectionStart = rtxtNotes.Text.Length;
                return;
            }
            beforeCursor = fullNote.Substring(0, currPstn);
            afterCursor = fullNote.Substring(currPstn);
            rtxtNotes.Text = beforeCursor + newWord + afterCursor;
            rtxtNotes.SelectionStart = currPstn + newWord.Length;

        }

/*        private void simpleRtxtUpdate(RichTextBox rtxtNotes, String fullNote, int currPstn, String unicodeRepresentation)
        {
            String beforeCursor, afterCursor;

            if ((fullNote == null) || (fullNote.Length == 0))
            {
                rtxtNotes.Text = unicodeRepresentation;
                rtxtNotes.SelectionStart = 1;
                return;
            }
            if (currPstn == 0)
            {
                rtxtNotes.Text = unicodeRepresentation + fullNote;
                rtxtNotes.SelectionStart = 1;
                return;
            }
            if (currPstn == fullNote.Length - 1)
            {
                rtxtNotes.Text = fullNote + unicodeRepresentation;
                rtxtNotes.SelectionStart = ++currPstn;
                return;
            }
            beforeCursor = fullNote.Substring(0, currPstn);
            afterCursor = fullNote.Substring(currPstn);
            rtxtNotes.Text = beforeCursor + unicodeRepresentation + afterCursor;
            rtxtNotes.SelectionStart = ++currPstn;
            return;
        }

        private void deleteRtxt(RichTextBox rtxtNotes, String fullNote, int currPstn)
        {
            String beforeCursor, afterCursor;

            if ((fullNote == null) || (fullNote.Length == 0)) return; // do nothing
            if (currPstn == 0) return; // do nothing
            beforeCursor = fullNote.Substring(0, currPstn);
            afterCursor = fullNote.Substring(currPstn);
            rtxtNotes.Text = beforeCursor.Substring(0, beforeCursor.Length - 1) + afterCursor;
            rtxtNotes.SelectionStart = --currPstn;
        } */

        private void updateSearchBox(int tagValue, TextBox txtTarget, bool isSecondary)
        {
            Label lblFirst = null, lblSecond = null;
            Button btnSecondarySearch = null;
            NumericUpDown udSearch = null;

            if (tagValue == 1)
            {
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 1)).SelectedIndex = 0;
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 5)).SelectedIndex = 1;
                if (isSecondary)
                {
                    lblFirst = (Label)globalVars.getGroupedControl(globalVars.LabelCode, 1);  // within
                    lblSecond = (Label)globalVars.getGroupedControl(globalVars.LabelCode, 2);  // wordsOf
                    udSearch = (NumericUpDown)globalVars.UdMTScan;
                    btnSecondarySearch = (Button)globalVars.getGroupedControl(globalVars.ButtonCode, 0);
                    lblFirst.Visible = true;
                    lblSecond.Visible = true;
                    udSearch.Visible = true;
                    txtTarget.Visible = true;
                    btnSecondarySearch.Text = "Basic Search";
                    globalVars.SecondaryMTBookId = -1;
                    globalVars.SecondaryMTChapNo ="";
                    globalVars.SecondaryMTVNo = "";
                    globalVars.SecondaryMTWordSeq = -1;
                    globalVars.SecondaryMTWord = txtHebEnteredText.Text;
                }
                else
                {
                    globalVars.PrimaryMTBookId = -1;
                    globalVars.PrimaryMTChapNo = "";
                    globalVars.PrimaryMTVNo = "";
                    globalVars.PrimaryMTWordSeq = -1;
                    globalVars.PrimaryMTWord = txtHebEnteredText.Text;
                }
                txtTarget.Text = txtHebEnteredText.Text;
            }
            else
            {
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 1)).SelectedIndex = 1;
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 6)).SelectedIndex = 2;
                if (isSecondary)
                {
                    lblFirst = (Label)globalVars.getGroupedControl(globalVars.LabelCode, 4);  // within
                    lblSecond = (Label)globalVars.getGroupedControl(globalVars.LabelCode, 5);  // wordsOf
                    udSearch = (NumericUpDown)globalVars.UdLXXScan;
                    btnSecondarySearch = (Button)globalVars.getGroupedControl(globalVars.ButtonCode, 1);
                    lblFirst.Visible = true;
                    lblSecond.Visible = true;
                    udSearch.Visible = true;
                    txtTarget.Visible = true;
                    btnSecondarySearch.Text = "Basic Search";
                    globalVars.SecondaryLXXBookId = -1;
                    globalVars.SecondaryLXXChapNo = "";
                    globalVars.SecondaryLXXVNo = "";
                    globalVars.SecondaryLXXWordSeq = -1;
                    globalVars.SecondaryLXXWord = txtGkEnteredText.Text;
                }
                else
                {
                    globalVars.PrimaryLXXBookId = -1;
                    globalVars.PrimaryLXXChapNo = "";
                    globalVars.PrimaryLXXVNo = "";
                    globalVars.PrimaryLXXWordSeq = -1;
                    globalVars.PrimaryLXXWord = txtGkEnteredText.Text;
                }
                txtTarget.Text = txtGkEnteredText.Text;
            }
        }

/*        private void simpleSearchBoxUpdate(TextBox txtEntry, String fullText, int currPstn, String unicodeRepresentation)
        {
            String beforeCursor, afterCursor;

            if ((fullText == null) || (fullText.Length == 0))
            {
                txtEntry.Text = unicodeRepresentation;
                txtEntry.SelectionStart = 1;
                return;
            }
            if (currPstn == 0)
            {
                txtEntry.Text = unicodeRepresentation + fullText;
                txtEntry.SelectionStart = 1;
                return;
            }
            if (currPstn == fullText.Length - 1)
            {
                txtEntry.Text = fullText + unicodeRepresentation;
                txtEntry.SelectionStart = ++currPstn;
                return;
            }
            beforeCursor = fullText.Substring(0, currPstn);
            afterCursor = fullText.Substring(currPstn);
            txtEntry.Text = beforeCursor + unicodeRepresentation + afterCursor;
            txtEntry.SelectionStart = ++currPstn;
            return;
        } */

        public bool getDestinationButtonStatus(int whichButton)
        {
            return globalVars.RbtnLXXDestination[whichButton].Checked;
        }
    }
}
