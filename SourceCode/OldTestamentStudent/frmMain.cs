using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OldTestamentStudent
{
    public partial class frmMain : Form
    {
        const int keybrdPanelHeight = 280;

        bool isActivated = false;
        int rightPanesControl = 2;

        /*-----------------------------------------------------------------------------------------------------*
         *                                                                                                     *
         *   Definition of classes                                                                             *
         *   ---------------------                                                                             *
         *                                                                                                     *
         *-----------------------------------------------------------------------------------------------------*/
        classGlobal globalVars;
        frmProgress progressForm;
        classRegistry appRegistry;
        classGreekOrthography greekOrthography;
        classMTText mtText;
        classLXXText lxxText;
        classKeyboard keyboardMethods;
        classNote notesProcessing;
        classHistory historyProcessing;
        classHebLexicon hebrewLexicon;
        classGkLexicon greekLexicon;
        classSearch searchProcs;

        /*-----------------------------------------------------------------------------------------------------*
         *                                                                                                     *
         *   Thread management                                                                                 *
         *   -----------------                                                                                 *
         *                                                                                                     *
         *-----------------------------------------------------------------------------------------------------*/
        Thread initialisationThread;
        private delegate void performProgressAdvance(String mainMessage, String secondaryMessage, bool useSecondary);
        private delegate void performFormClose();
        private delegate void performMessageRemoval(Label targetLabel);
        private delegate void performKeyboardInit(SplitContainer targetSplit, Panel associatedPanel);

        private void setKeyboardPosition(SplitContainer targetSplit, Panel associatedPanel)
        {
            targetSplit.SplitterDistance = targetSplit.Height - associatedPanel.Height;
        }

        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Activated(object sender, EventArgs e)
        {
            /*****************************************************************************************************************************
             *                                                                                                                           *
             *                                                     frmMain_Activated                                                     *
             *                                                     =================                                                     *
             *                                                                                                                           *
             *  This is the controling method for building the form.  We have used form activation rather than the class instantiation   *
             *  because the latter functions *before* the visible form is available and makes the display of progress impossible (or, at *
             *  least, difficult).  We ensure that the form creation elements of the activation method is only called once by means of   *
             *  the variable, isActivated.                                                                                               *
             *                                                                                                                           *
             *****************************************************************************************************************************/

            if (!isActivated)
            {
                this.Visible = false;
                progressForm = new frmProgress();
                progressForm.Show();

                appRegistry = new classRegistry();
                appRegistry.DefaultX = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
                appRegistry.DefaultY = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
                appRegistry.DefaultHeight = this.Height;
                appRegistry.DefaultWidth = this.Width;
                appRegistry.DefaultState = (int) this.WindowState;
                appRegistry.DefaultSplitPosition = splitMain.SplitterDistance;
                initialisationThread = new Thread(new ThreadStart(performInitialisation));
                initialisationThread.IsBackground = true;
                initialisationThread.Start(); 

                isActivated = true;
            }
        }

        private void performInitialisation()
        {

            /*=====================================================================================================*
             *                                                                                                     *
             *   Initialise class for global variables and populate with frmMain elements                          *
             *                                                                                                     *
             *=====================================================================================================*/
            globalVars = new classGlobal();
            globalVars.MasterForm = this;

            /*-----------------------------------------------------------------------------------------------------*
             *  Declare Richtext boxes to global configuration class                                               *
             *                                                                                                     *
             *        Masoretic Side                                      Septuagint Side                          *
             *  Index     Specific Richtext Control                Index     Specific Richtext Control             *
             *                                                                                                     *
             *    0         rtxtMainMTText                           1         rtxtMainLXXText                     *
             *    2         rtxtMTNotes                              3         rtxtLXXNotes                        *
             *                                                       5         rtxtGkLexicon                       *
             *                                                                                                     *
             *                              Combined                                                               *
             *                 Index     Specific Richtext Control                                                 *
             *                                                                                                     *
             *                   4         rtxtParse                                                               *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(rtxtMainMTText, globalVars.RichtextBoxCode);
            globalVars.addGroupedControl(rtxtMainLXXText, globalVars.RichtextBoxCode);
            globalVars.addGroupedControl(rtxtMTNotes, globalVars.RichtextBoxCode);
            globalVars.addGroupedControl(rtxtLXXNotes, globalVars.RichtextBoxCode);
            globalVars.addGroupedControl(rtxtParse, globalVars.RichtextBoxCode);
            globalVars.addGroupedControl(rtxtGkLexicon, globalVars.RichtextBoxCode);

            /*-----------------------------------------------------------------------------------------------------*
             *  Declare Combo boxes to global configuration class                                                  *
             *                                                                                                     *
             *        Masoretic Side                                      Septuagint Side                          *
             *  Index     Specific Combobox                        Index     Specific Combobox                     *
             *                                                                                                     *
             *    0         cbMTBook                                 3         cbLXXBook                           *
             *    1         cbMTChapter                              4         cbLXXChapter                        *
             *    2         cbMTVerse                                5         cbLXXVerse                          *
             *    6         cbMTHistory                              7         cbLXXHistory                        *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(cbMTBook, globalVars.ComboBoxesCode);
            globalVars.addGroupedControl(cbMTChapter, globalVars.ComboBoxesCode);
            globalVars.addGroupedControl(cbMTVerse, globalVars.ComboBoxesCode);
            globalVars.addGroupedControl(cbLXXBook, globalVars.ComboBoxesCode);
            globalVars.addGroupedControl(cbLXXChapter, globalVars.ComboBoxesCode);
            globalVars.addGroupedControl(cbLXXVerse, globalVars.ComboBoxesCode);
            globalVars.addGroupedControl(cbMTHistory, globalVars.ComboBoxesCode);
            globalVars.addGroupedControl(cbLXXHistory, globalVars.ComboBoxesCode);

            /*-----------------------------------------------------------------------------------------------------*
             *  Declare tab controls:                                                                              *
             *                                                                                                     *
             *  Index     Specific Page                                                                            *
             *                                                                                                     *
             *    0         Main Tab Control - Languages                                                           *
             *    1         Secondary Tab Control - Functions                                                      *
             *    2         Hebrew/Aramaic Keyboard Tab Control                                                    *
             *    3         Greek Keyboard Tab Control                                                             *
             *    4         Top Right Tab Control                                                                  *
             *    5         Functions specific to Masoretic Text Tab Control - cf control 1                        *
             *    6         Functions specific to Septuagint Text Tab Control - cf control 1                       *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(tabCtrlLanguage, globalVars.TabControlCode);
            globalVars.addGroupedControl(tabCtrlFunctions, globalVars.TabControlCode);
            globalVars.addGroupedControl(tabCtrlHebKeyboard, globalVars.TabControlCode);
            globalVars.addGroupedControl(tabCtrlGkKeyboard, globalVars.TabControlCode);
            globalVars.addGroupedControl(tabCtrlTop, globalVars.TabControlCode);
            globalVars.addGroupedControl(tabCtrlMTFunctions, globalVars.TabControlCode);
            globalVars.addGroupedControl(tabCtrlLXXFunctions, globalVars.TabControlCode);

            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  Tab control pages      objectTypeCode = 4                                                          *
             *  -----------------                                                                                  *
             *                                                                                                     *
             *  Index     Specific Page                                                                            *
             *                                                                                                     *
             *    0         Main Control - Masoretic Text                                                          *
             *    1         Main Control - Septuagint Text                                                         *
             *    2         Masoretic Text functions - notes page                                                  *
             *    3         Masoretic Text functions - search setup page                                           *
             *    4         Masoretic Text functions - variant readings page                                       *
             *    5         Septuagint functions - notes page                                                      *
             *    6         Septuagint functions - Liddell & Scott appendices                                      *
             *    7         Septuagint functions - search setup page                                               *  
             *    8         tabPgeHebVKeyboard - tabCtrlHebKeyboard, left tab                                      *
             *    9         tabPgeHebKeystrokes - tabCtrlHebKeyboard, right tab                                    *
             *   10         tabPgeGkVKeyboard - tabCtrlGkKeyboard, left tab                                        *
             *   11         tabPgeGkKeystrokes - tabCtrlGkKeyboard, right tab                                      *
             *   12         tabPgeParse - top right, parse (grammatical information) tab                           *
             *   13         tabPgeLexicon - top right, lexical information tab                                     *
             *   14         tabPgeGkLexicon - top right, alternativelexical information tab                        *
             *   15         tabPgeSearchResults - top right, search results tab                                    *
             *                                                                                                     *
             *   The six tabs withing the Liddell & Scott appendices do not need to be stored globally.            *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(tabPgeMT, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeLXX, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeMTNotes, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeMTSearch, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeMTVariants, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeLXXNotes, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeLSAppendices, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeLXXSearch, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeHebVKeyboard, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeHebKeystrokes, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeGkVKeyboard, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeGkKeystrokes, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeParse, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeLexicon, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeGkLexicon, globalVars.TabPageCode);
            globalVars.addGroupedControl(tabPgeSearchResults, globalVars.TabPageCode);

            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  Panels (virtual keyboards)      objectTypeCode = 5                                                 *
             *  --------------------------                                                                         *
             *                                                                                                     *
             *  Index     Specific Page                                                                            *
             *  -----     -------------                                                                            *
             *                                                                                                     *
             *    0         Panel for Hebrew/Aramaic keyboard                                                      *
             *    1         Panel for Greek keyboard                                                               *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(pnlMTKeyboard, globalVars.PanelCode);
            globalVars.addGroupedControl(pnlGkKeyboard, globalVars.PanelCode);


            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  Buttons (needing global access) objectTypeCode = 6                                                 *
             *  --------------------------                                                                         *
             *                                                                                                     *
             *        Masoretic Side                                      Septuagint Side                          *
             *        --------------                                      ---------------                          *
             *  Index     Specific Button                          Index     Specific Button                       *
             *  -----     ---------------                          -----     ---------------                       *
             *                                                                                                     *
             *    0         btnMTAdvanced (Search function)          1         btnLXXAdvanced                      *
             *                                                                                                     *
             *                                 Search Toolbar (Common)                                             *
             *                                 -----------------------                                             *
             *                                Index     Specific Button                                            *
             *                                -----     ---------------                                            *
             *                                                                                                     *
             *                                  2         btnStop                                                  *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(btnMTAdvanced, globalVars.ButtonCode);
            globalVars.addGroupedControl(btnLXXAdvanced, globalVars.ButtonCode);
            globalVars.addGroupedControl(btnStop, globalVars.ButtonCode);

            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  Text boxes (needing global access) objectTypeCode = 7                                              *
             *  --------------------------                                                                         *
             *                                                                                                     *
             *        Masoretic Side                                      Septuagint Side                          *
             *        --------------                                      ---------------                          *
             *  Index     Specific Textbox                         Index     Specific Textbox                      *
             *  -----     ----------------                         -----     ----------------                      *
             *                                                                                                     *
             *    0      txtMTPrimaryWord (Search function)          2         txtLXXPrimaryWord                   *
             *    1      txtMTSecondaryWord (Search function)        3         txtLXXSecondaryWord                 *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(txtMTPrimaryWord, globalVars.TextboxCode);
            globalVars.addGroupedControl(txtMTSecondaryWord, globalVars.TextboxCode);
            globalVars.addGroupedControl(txtLXXPrimaryWord, globalVars.TextboxCode);
            globalVars.addGroupedControl(txtLXXSecondaryWord, globalVars.TextboxCode);

            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  Split Containers objectTypeCode = 8                                                                *
             *  ----------------                                                                                   *
             *                                                                                                     *
             *    0         splitMain (Covers effectively the whole form)                                          *
             *    1         splitMTLeft (within the MT tab, left main area)                                        *
             *    2         splitLXXLeft (within the LXX tab, left main area)                                      *
             *    3         splitRight                                                                             *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(splitMain, globalVars.SplitContainerCode);
            globalVars.addGroupedControl(splitMTLeft, globalVars.SplitContainerCode);
            globalVars.addGroupedControl(splitLXXLeft, globalVars.SplitContainerCode);
            globalVars.addGroupedControl(splitRight, globalVars.SplitContainerCode);

            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  RadioButtons (needing global access)     objectTypeCode = 9                                        *
             *  ------------                                                                                       *
             *                                                                                                     *
             *        Masoretic Side                                      Septuagint Side                          *
             *        --------------                                      ---------------                          *
             *  Index     Specific Textbox                         Index     Specific Textbox                      *
             *  -----     ----------------                         -----     ----------------                      *
             *                                                                                                     *
             *    0      rbtnBdbRefs (under "Search for ...")        3         rbtnLXXRootMatch                    *
             *    1      rbtnMTStrict (under "Search for ...")       4         rbtnLXXExactMatch                   *
             *    2      rbtnMTModerate (under "Search for ...")                                                   *
             *    5      rbtnMTExclude                               6         rbtnLXXExclude                      *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(rbtnBdbRefs, globalVars.RadioButtonCode);
            globalVars.addGroupedControl(rbtnMTStrict, globalVars.RadioButtonCode);
            globalVars.addGroupedControl(rbtnMTModerate, globalVars.RadioButtonCode);
            globalVars.addGroupedControl(rbtnLXXRootMatch, globalVars.RadioButtonCode);
            globalVars.addGroupedControl(rbtnLXXExactMatch, globalVars.RadioButtonCode);
            globalVars.addGroupedControl(rbtnMTExclude, globalVars.RadioButtonCode);
            globalVars.addGroupedControl(rbtnLXXExclude, globalVars.RadioButtonCode);

            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  ToolStripStatusLabels (in the Search Results Status Strip)      objectTypeCode = 10                *
             *  ---------------------                                                                              *
             *                                                                                                     *
             *    0         statLab1                                                                               *
             *    1         statLab2                                                                               *
             *    2         statLab3                                                                               *
             *    3         statLab4                                                                               *
             *    4         statLab5                                                                               *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(statLab1, globalVars.ToolStripLabelCode);
            globalVars.addGroupedControl(statLab2, globalVars.ToolStripLabelCode);
            globalVars.addGroupedControl(statLab3, globalVars.ToolStripLabelCode);
            globalVars.addGroupedControl(statLab4, globalVars.ToolStripLabelCode);
            globalVars.addGroupedControl(statLab5, globalVars.ToolStripLabelCode);

            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  Web Browser controls                                            objectTypeCode = 11                *
             *  ---------------------                                                                              *
             *                                                                                                     *
             *    0         webLexiconEntry                                                                        *
             *    1         webAuthors                                                                             *
             *    2         webEpigraphy                                                                           *
             *    3         webPapyrology                                                                          *
             *    4         webPeriodicals                                                                         *
             *    5         webGeneral                                                                             *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(webLexiconEntry, globalVars.WebBrowserCode);
            globalVars.addGroupedControl(webAuthors, globalVars.WebBrowserCode);
            globalVars.addGroupedControl(webEpigraphy, globalVars.WebBrowserCode);
            globalVars.addGroupedControl(webPapyrology, globalVars.WebBrowserCode);
            globalVars.addGroupedControl(webPeriodicals, globalVars.WebBrowserCode);
            globalVars.addGroupedControl(webGeneral, globalVars.WebBrowserCode);

            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  Labels (needing global access)     objectTypeCode = 12                                        *
             *  ------                                                                                             *
             *                                                                                                     *
             *        Masoretic Side                                      Septuagint Side                          *
             *        --------------                                      ---------------                          *
             *  Index     Specific Textbox                         Index     Specific Textbox                      *
             *  -----     ----------------                         -----     ----------------                      *
             *                                                                                                     *
             *    0      labMTSearchLbl                              3         labLXXSearchLbl                     *
             *    1      labMTWithinLbl                              4         labLXXWithinLbl                     *
             *    2      labMTWordsOfLbl                             5         labLXXWordsOfLbl                    *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(labMTSearchLbl, globalVars.LabelCode);
            globalVars.addGroupedControl(labMTWithinLbl, globalVars.LabelCode);
            globalVars.addGroupedControl(labMTWordsOfLbl, globalVars.LabelCode);
            globalVars.addGroupedControl(labLXXSearchLabel, globalVars.LabelCode);
            globalVars.addGroupedControl(labLXXWithin, globalVars.LabelCode);
            globalVars.addGroupedControl(labLXXWordsOf, globalVars.LabelCode);

            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  Listboxes (needing global access)     objectTypeCode = 13                                          *
             *  ---------                                                                                          *
             *                                                                                                     *
             *        Masoretic Side                                      Septuagint Side                          *
             *        --------------                                      ---------------                          *
             *  Index     Specific Listbox                         Index     Specific Listbox                      *
             *  -----     ----------------                         -----     ----------------                      *
             *                                                                                                     *
             *    0      lbAvailableMTBooks                          1         lbLXXAvailableItems                 *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(lbAvailableMTBooks, globalVars.ListBoxCode);
            globalVars.addGroupedControl(lbLXXAvailableItems, globalVars.ListBoxCode);

            /*-----------------------------------------------------------------------------------------------------*
             *                                                                                                     *
             *  Groupboxes (needing global access)     objectTypeCode = 14                                         *
             *  ----------                                                                                         *
             *                                                                                                     *
             *        Masoretic Side                                      Septuagint Side                          *
             *        --------------                                      ---------------                          *
             *  Index     Specific Listbox                         Index     Specific Listbox                      *
             *  -----     ----------------                         -----     ----------------                      *
             *                                                                                                     *
             *    0      gbMTSourceOptions                           1       gbLXXSourceOptions                    *
             *                                                                                                     *
             *-----------------------------------------------------------------------------------------------------*/
            globalVars.addGroupedControl(gbMTSourceOptions, globalVars.GroupboxCode);
            globalVars.addGroupedControl(gbLXXSourceOptions, globalVars.GroupboxCode);
            globalVars.FlowResults = flowResults;
            globalVars.SearchContextMenu = cMnuSearchResult;
            globalVars.StatSearch = statSearch;
            globalVars.UdMTScan = upDownMTWithin;
            globalVars.UdLXXScan = upDownLXXWithin;
            globalVars.initialiseGlobal();

            /*===================================================================*
             *                                                                   *
             *  Registry Management and cross-session values                     *
             *                                                                   *
             *===================================================================*/

            appRegistry.MainForm = this;
            appRegistry.initialiseRegistry(globalVars, @"software\LFCConsulting\OTBibleStudent");
            appRegistry.initialiseWindowDetails();
            appRegistry.initialiseFontsAndColour();
            //            appRegistry.closeKeys();

            /*===================================================================*
             *                                                                   *
             *  Initialise the methods for manipulating Greek characters         *
             *                                                                   *
             *===================================================================*/

            progressForm.Invoke(new performProgressAdvance(updateProgress), "Preparing Greek characters", "", false);
            greekOrthography = new classGreekOrthography();
            greekOrthography.initialiseGreekOrthography(globalVars);

            // We need to define some classes early so that a non-null value is stored in the text classes - the sequence is vital
            mtText = new classMTText();
            historyProcessing = new classHistory(globalVars, mtText, lxxText);
            hebrewLexicon = new classHebLexicon();
            hebrewLexicon.initialiseLexicon(globalVars, mtText);
            greekLexicon = new classGkLexicon();
            greekLexicon.initialiseLexicon(globalVars, greekOrthography);

            /*===================================================================*
             *                                                                   *
             *  Load the MT text                                                 *
             *                                                                   *
             *===================================================================*/

            mtText.initialiseText(globalVars, progressForm, historyProcessing, hebrewLexicon);
            mtText.loadText();

            /*===================================================================*
             *                                                                   *
             *  Load the LXX text                                                 *
             *                                                                   *
             *===================================================================*/

            lxxText = new classLXXText();
            lxxText.initialiseText(globalVars, progressForm, historyProcessing, greekLexicon);
            lxxText.loadText();

            /*************************************************************************************
             *                                                                                   *
             * Load the Liddell & Scott Appendices                                               *
             *                                                                                   *
             *************************************************************************************/

            greekLexicon = new classGkLexicon();
            greekLexicon.initialiseLexicon(globalVars, greekOrthography);
            greekLexicon.populateAppendices();

            /*===================================================================*
             *                                                                   *
             *  Set up the two virtual keyboards                                 *
             *                                                                   *
             *===================================================================*/
            progressForm.Invoke(new performProgressAdvance(updateProgress), "Creating the Hebrew/Aramaic virtual keyboard", "", false);
            keyboardMethods = new classKeyboard();
            keyboardMethods.initialiseMTKeyboard(globalVars, progressForm, greekOrthography);
            splitMTLeft.Invoke(new performKeyboardInit(setKeyboardPosition), splitMTLeft, pnlMTHistory);
            splitMTLeft.Invoke(new performKeyboardInit(setKeyboardPosition), splitLXXLeft, pnlLXXHistory);

            /*===================================================================*
             *                                                                   *
             *  Initialise the notes facilities                                  *
             *                                                                   *
             *===================================================================*/
            progressForm.Invoke(new performProgressAdvance(updateProgress), "Setting up the notes facility", "", false);
            notesProcessing = new classNote();
            notesProcessing.initialiseNotes(globalVars, mtText, lxxText, appRegistry);
            notesProcessing.processOnStartup();

            /*===================================================================*
             *                                                                   *
             *  Initialise the two history lists                                 *
             *                                                                   *
             *===================================================================*/
            progressForm.Invoke(new performProgressAdvance(updateProgress), "Retrieving and processing history information", "", false);
            historyProcessing.loadHistory();

            searchProcs = new classSearch( this, globalVars, hebrewLexicon, greekLexicon, greekOrthography, mtText, lxxText, notesProcessing);

            /*===================================================================*
             *                                                                   *
             *  Initialise the "remember" flags for copy settings                *
             *                                                                   *
             *===================================================================*/

            globalVars.setCopyOption(3, 0);
            globalVars.setCopyOption(7, 0);
            globalVars.setCopyOption(11, 0);
            globalVars.setCopyOption(15, 0);
            globalVars.setCopyOption(19, 0);
            globalVars.setCopyOption(23, 0);
            globalVars.setCopyOption(27, 0);
            globalVars.setCopyOption(31, 0);

            progressForm.Invoke(new performFormClose(closeForm));
            labInitialisationNotice.Invoke(new performMessageRemoval(removeMessage), labInitialisationNotice);
        }

        private void updateProgress(String mainMessage, String secondaryMessage, bool useSecondary)
        {
            progressForm.incrementProgress(mainMessage, secondaryMessage, useSecondary);
        }

        private void closeForm()
        {
            progressForm.Close();
        }

        private void removeMessage(Label targetLabel)
        {
            targetLabel.Visible = false;
        }

        private void labInitialisationNotice_VisibleChanged(object sender, EventArgs e)
        {
            /*=====================================================================================================*
             *                                                                                                     *
             *                                labInitialisationNotice_VisibleChanged                                              *
             *                                ======================================                               *
             *                                                                                                     *
             *  There is data that was accumulated as part of the startup process - mainly imported from source    *
             *    data files.  These need to be used to populate ComboBoxes and the main text RichText areas.  If  *
             *    we do that directly, we will need to do so through delegate methods because the startup is       *
             *    managed through a background thread (to allow progress reporting).  So, the data is stored and   *
             *    is here retrieved and processed - *outside* the background task in the main task.                *
             *                                                                                                     *
             *=====================================================================================================*/
            int idx, bookId, nPstn;
            Char discriminant = ' ';
            String historyReference, bookName, chapterRef;
            classMTBook currentMtBook;
            classLXXBook currentLxxBook;

            classMTChapter currentMTChapter;
            classMTVerse currentMTVerse;

            if (!labInitialisationNotice.Visible)
            {
                globalVars.activateLexiconTab(1);
                // Set the splitter position
                this.Left = appRegistry.DefaultX;
                this.Top = appRegistry.DefaultY;
                this.Width = appRegistry.DefaultWidth;
                this.Height = appRegistry.DefaultHeight;
                switch( appRegistry.DefaultState)
                {
                    case 0: this.WindowState = FormWindowState.Normal; break;
                    case 1: this.WindowState = FormWindowState.Minimized; break;
                    case 2: this.WindowState = FormWindowState.Maximized; break;
                }
                splitMain.SplitterDistance = appRegistry.DefaultSplitPosition;
                // Populate the book names for the MT tab
                for ( idx = 0; idx < globalVars.NoOfMTBooks; idx++)
                {
                    globalVars.MtBookList.TryGetValue(idx, out currentMtBook);
                    cbMTBook.Items.Add(currentMtBook.BookName);
                }
                // Populate the book names for the LXX tab
                for (idx = 0; idx < globalVars.NoOfLXXBooks; idx++)
                {
                    globalVars.LxxBookList.TryGetValue(idx, out currentLxxBook);
                    cbLXXBook.Items.Add(currentLxxBook.CommonName);
                }

                // Display a suitable passage
                if (cbMTHistory.Items.Count > 0)
                {
                    historyReference = cbMTHistory.SelectedItem.ToString();
                    nPstn = historyReference.LastIndexOf(discriminant);
                    if (nPstn >= 0)
                    {
                        bookName = historyReference.Substring(0, nPstn);
                        chapterRef = historyReference.Substring(nPstn + 1);
                        bookId = -1;
                        currentMtBook = null;
                        for (idx = 0; idx < globalVars.NoOfMTBooks; idx++)
                        {
                            globalVars.MtBookList.TryGetValue(idx, out currentMtBook);
                            if (String.Compare(bookName, currentMtBook.BookName) == 0)
                            {
                                bookId = idx;
                                break;
                            }
                        }
                        if (bookId >= 0)
                        {
                            mtText.displayChapter(bookId, chapterRef);
                            globalVars.IsStartup = true;
                            if (cbMTChapter.Items.Contains(chapterRef)) cbMTChapter.SelectedItem = chapterRef;
                            globalVars.IsStartup = false;
                        }
                    }
                }
                else
                {
                    if (globalVars.MtCurrentChapter.Length == 0) mtText.displayChapter(globalVars.MtCurrentBookIndex, "1"); // cbMTBook.SelectedIndex = globalVars.MtCurrentBookIndex;
                    else mtText.displayChapter(globalVars.MtCurrentBookIndex, globalVars.MtCurrentChapter);
                }
                if (cbLXXHistory.Items.Count > 0)
                {
                    historyReference = cbLXXHistory.SelectedItem.ToString();
                    nPstn = historyReference.LastIndexOf(discriminant);
                    if (nPstn >= 0)
                    {
                        bookName = historyReference.Substring(0, nPstn);
                        chapterRef = historyReference.Substring(nPstn + 1);
                        bookId = -1;
                        currentLxxBook = null;
                        for (idx = 0; idx < globalVars.NoOfLXXBooks; idx++)
                        {
                            globalVars.LxxBookList.TryGetValue(idx, out currentLxxBook);
                            if (String.Compare(bookName, currentLxxBook.CommonName) == 0)
                            {
                                bookId = idx;
                                break;
                            }
                        }
                        if (bookId >= 0)
                        {
                            lxxText.displayChapter(bookId, chapterRef);
                            globalVars.IsStartup = true;
                            if (cbLXXChapter.Items.Contains(chapterRef)) cbLXXChapter.SelectedItem = chapterRef;
                            globalVars.IsStartup = false;
                        }
                    }
                }
                else
                {
                    if (globalVars.LxxCurrentChapter.Length == 0) lxxText.displayChapter(globalVars.LxxCurrentBookIndex, "1");
                    else lxxText.displayChapter(globalVars.LxxCurrentBookIndex, globalVars.LxxCurrentChapter);
                }

                // Since this is a post-startup task, avoid certain updates that will cycle unhelpfully
                globalVars.IsTextUpdateActive = false;

                // Set some base values for the search facility
                searchProcs.searchOptionCheckedChanged(globalVars.ChkMTBooks[0], null);
                searchProcs.searchOptionCheckedChanged(globalVars.ChkLXXBooks[0], null);

                // Update the verse currently active
                bookId = cbMTBook.SelectedIndex;
                globalVars.MtBookList.TryGetValue(bookId, out currentMtBook);
                currentMTChapter = currentMtBook.getChapterByChapterNo(cbMTChapter.SelectedItem.ToString());
                currentMTVerse = currentMTChapter.getVerseByVerseNo(cbMTVerse.SelectedItem.ToString());
                rtxtMTNotes.Text = currentMTVerse.NoteText;

                this.Visible = true;
                this.Focus();
            }
        }

        private void cbBook_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbBook;

            if (globalVars.IsTextUpdateActive) return;
            cbBook = (ComboBox)sender;
            if (cbBook == (ComboBox) globalVars.getGroupedControl( globalVars.ComboBoxesCode, 0)) mtText.respondToNewBook();
//            if (cbBook == globalVars.getComboBoxControlByIndex(0)) mtText.respondToNewBook();
            else lxxText.respondToNewBook();
        }

        private void cbChapter_SelectedValueChanged(object sender, EventArgs e)
        {
            int bookId;
            String chapterId;
            ComboBox cbChapter;

            if (globalVars.IsStartup) return;
            cbChapter = (ComboBox)sender;
            if (cbChapter == (ComboBox) globalVars.getGroupedControl(globalVars.ComboBoxesCode, 1))
            {
                bookId = cbMTBook.SelectedIndex;
                chapterId = cbMTChapter.SelectedItem.ToString();
                if(! globalVars.IsStartup) mtText.displayChapter(bookId, chapterId);
            }
            if (cbChapter == (ComboBox)globalVars.getGroupedControl(globalVars.ComboBoxesCode, 4))
            {
                bookId = cbLXXBook.SelectedIndex;
                chapterId = cbLXXChapter.SelectedItem.ToString();
                if (!globalVars.IsStartup) lxxText.displayChapter(bookId, chapterId);
            }
        }

        private void cMnuTextAnalyse_Click(object sender, EventArgs e)
        {
            if (tabCtrlLanguage.SelectedTab == tabPgeMT) mtText.Analysis();
            else lxxText.Analysis();
        }

        private void searchSetup_Click(object sender, EventArgs e)
        {
            int tagVal;
            ToolStripMenuItem cMnuCurrent = (ToolStripMenuItem)sender;

            tagVal = Convert.ToInt32(cMnuCurrent.Tag);
            searchProcs.searchSetup(tagVal, tabCtrlLanguage.SelectedIndex);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int idx, noOfSelections, tagVal;
            int[] selectionArray;
            Button currentButton;
            ListBox currentListbox = null;
            ListBox.SelectedIndexCollection selectedBooks;
            CheckBox exampleCheckbox = null;

            currentButton = (Button)sender;
            tagVal = Convert.ToInt32(currentButton.Tag);
            switch( tagVal)
            {
                case 1:
                    {
                        currentListbox = lbAvailableMTBooks;
                        exampleCheckbox = globalVars.ChkMTBooks[0];
                    }
                    break;
                case 2:
                    {
                        currentListbox = lbLXXAvailableItems;
                        exampleCheckbox = globalVars.ChkLXXBooks[0];
                    }
                    break;
            }
            if (String.Compare(currentButton.Text, "Remove Selected Books") == 0)
            {
                selectedBooks = currentListbox.SelectedIndices;
                noOfSelections = selectedBooks.Count;
                if (noOfSelections > 0)
                {
                    selectionArray = new int[noOfSelections];
                    selectedBooks.CopyTo(selectionArray, 0);
                    for (idx = noOfSelections - 1; idx >= 0; idx--)
                    {
                        currentListbox.Items.RemoveAt(selectionArray[idx]);
                    }
                }
                currentButton.Text = "Put Removed Books back";
            }
            else
            {
                searchProcs.searchOptionCheckedChanged(exampleCheckbox, null);
                currentButton.Text = "Remove Selected Books";
            }
        }

        private void bookList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tagVal;
            Button currentButton = null;
            ListBox currentListbox = null;

            currentListbox = (ListBox)sender;
            tagVal = Convert.ToInt32(currentListbox.Tag);
            switch( tagVal)
            {
                case 1: currentButton = btnMTRemove; break;
                case 2: currentButton = btnLXXRemove; break;
            }
            currentButton.Text = "Remove Selected Books";
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            int idx, noOfBookGroups = 0, tagVal;
            Button currentButton = null;
            CheckBox[] chkBooks = null;

            currentButton = (Button)sender;
            tagVal = Convert.ToInt32(currentButton.Tag);
            switch( tagVal)
            {
                case 1:
                    {
                        noOfBookGroups = globalVars.NoOfMTBookGroups;
                        chkBooks = globalVars.ChkMTBooks;
                    }
                    break;
                case 2:
                    {
                        noOfBookGroups = globalVars.NoOfLXXBookGroups;
                        chkBooks = globalVars.ChkLXXBooks;
                    }
                    break;
            }
            for (idx = 0; idx < noOfBookGroups; idx++) chkBooks[idx].Checked = false;
            searchProcs.searchOptionCheckedChanged(chkBooks[0], null);
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            int tagVal;
            Button currentButton;

            currentButton = (Button)sender;
            tagVal = Convert.ToInt32(currentButton.Tag);
            searchProcs.setSearchType(tagVal);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
           searchProcs.controlSearch();
        }

        private void advanceOrGoBackOneChapter(object sender, EventArgs e)
        {
            int tagVal;
            Button currentButton;

            currentButton = (Button)sender;
            tagVal = Convert.ToInt32(currentButton.Tag);
            switch (tagVal)
            {
                case 1: mtText.backOrForwardOneChapter(1); break;
                case 2: mtText.backOrForwardOneChapter(2); break;
                case 3: lxxText.backOrForwardOneChapter(1); break;
                case 4: lxxText.backOrForwardOneChapter(2); break;
            }
        }
        private void changeReferenceByHistory(object sender, EventArgs e)
        {
            int tagVal, idx, noOfEntries;
            String fileName = "";
            ComboBox currentCB;
            StreamWriter swHistory;
            
            currentCB = (ComboBox)sender;
            tagVal = Convert.ToInt32(currentCB.Tag);
            if (!globalVars.IsTextUpdateActive)
            {
                switch (tagVal)
                {
                    case 1: mtText.processSelectedHistory(); break;
                    case 2: lxxText.processSelectedHistory(); break;
                }
            }
            switch( tagVal)
            {
                case 1: fileName = globalVars.FullMTNotesPath; break;
                case 2: fileName = globalVars.FullLXXNotesPath; break;
            }
            swHistory = new StreamWriter(fileName + @"\History.txt");
            noOfEntries = currentCB.Items.Count;
            for( idx = 0; idx < noOfEntries; idx++)
            {
                if (idx < noOfEntries - 1) swHistory.WriteLine(currentCB.Items[idx].ToString());
                else swHistory.Write(currentCB.Items[idx].ToString());
            }
            swHistory.Close();
        }

        private void registerMouseDown(object sender, MouseEventArgs e)
        {
            /*========================================================================================================*
             *                                                                                                        *
             *                                           registerMouseDown                                            *
             *                                           =================                                            *
             *                                                                                                        *
             *  Purpose:                                                                                              *
             *  =======                                                                                               *
             *                                                                                                        *
             *  To identify the following whenever *any* mouse click is made:                                         *
             *    a) The text for the verse in which the selection is made                                            *
             *    b) The word clicked                                                                                 *
             *    c) The sequence number of the word in the verse                                                     *
             *  Point c) will allow us to identify the classWord instance for the selected word.                      *
             *                                                                                                        *
             *  Note: if the content of the rich text area has been changed by selecting from one of the combo boxes, *
             *    a) and b) will be set to null values.                                                               *
             *                                                                                                        *
             *========================================================================================================*/
            int nPstn;
            Tuple<String, String, int, classMTWord> mtClickResults;
            Tuple<String, String, int, classLXXWord> lxxClickResults;

            if (((RichTextBox)sender) == rtxtMainMTText)
            {
                nPstn = rtxtMainMTText.GetCharIndexFromPosition(new Point(e.X, e.Y));
                mtClickResults = mtText.identifyClickedWord(nPstn, rtxtMainMTText.Text);
//                if (mtClickResults.Item3 == -1) return;
                globalVars.LastSelectedMTVerse = mtClickResults.Item1;
                globalVars.LastSelectedMTWord = mtClickResults.Item2;
                globalVars.SelectedMTWordSequence = mtClickResults.Item3;
                globalVars.CurrentlySelectedMTWord = mtClickResults.Item4;
                cMnuText.RightToLeft = RightToLeft.No;
                cMnuSearchResult.RightToLeft = RightToLeft.No;
                if (globalVars.CurrentlySelectedMTWord != null)  // It will be null if we click on the verse number (i.e. the non-Hebrew/Aramaic area)
                {
                    if (globalVars.CurrentlySelectedMTWord.HasVariant) mnuTextKethibQere.Enabled = true;
                    else
                    {
                        mnuTextKethibQere.Enabled = false;
                        labKethibMsg.Text = "None";
                        labQereMsg.Text = "None";
                        tabPgeMTVariants.Refresh();
                    }
                }
            }

            if (((RichTextBox)sender) == rtxtMainLXXText)
            {
                nPstn = rtxtMainLXXText.GetCharIndexFromPosition(new Point(e.X, e.Y));
                lxxClickResults = lxxText.identifyClickedWord(nPstn, rtxtMainLXXText.Text);
                globalVars.LastSelectedLXXVerse = lxxClickResults.Item1;
                globalVars.LastSelectedLXXWord = lxxClickResults.Item2;
                globalVars.SelectedLXXWordSequence = lxxClickResults.Item3;
                globalVars.CurrentlySelectedLXXWord = lxxClickResults.Item4;
            }
        }

/*        private void rtxtSearchResults_MouseDown(object sender, MouseEventArgs e)
        { */
            /*========================================================================================================*
             *                                                                                                        *
             *                                      rtxtSearchResults_MouseDown                                       *
             *                                      ===========================                                       *
             *                                                                                                        *
             *  Purpose:                                                                                              *
             *  =======                                                                                               *
             *                                                                                                        *
             *  To identify the following whenever *any* mouse click is made in the search richtext area:             *
             *    a) The integer text location of the point at which the mouse is clicked                             *
             *    b) The text for the verse in which the selection is made                                            *
             *                                                                                                        *
             *========================================================================================================*/
/*            int nPstn, nStart, nEnd;
            String fullText, currentVerseText;

            nPstn = rtxtSearchResults.GetCharIndexFromPosition(new Point(e.X, e.Y));
            globalVars.LastSearchPosition = nPstn;
            fullText = rtxtSearchResults.Text;
            if ((fullText == null) || (fullText.Length == 0)) return;
            // This seems to be true when clicked *beyond* a line - find the start of the current line
            if (fullText[nPstn] == '\n') nStart = fullText.LastIndexOf('\n', nPstn - 1);
            else nStart = fullText.LastIndexOf('\n', nPstn);
            nStart++;
            // Now the end of the line
            if (fullText[nPstn] == '\n') nEnd = nPstn;
            else nEnd = fullText.IndexOf('\n', nPstn);
            if (nEnd == -1) nEnd = fullText.Length;
            // We can now identify the verse and the line of text associated with it
            currentVerseText = fullText.Substring(nStart, nEnd - nStart);
            globalVars.LastSelectedSearchVerse = currentVerseText;
        } */

        private void tmrKeyboard_Tick(object sender, EventArgs e)
        {
            int maxHeight, heightCalculation, tagVal;

            tagVal = Convert.ToInt32(tmrKeyboard.Tag);
            if( tagVal == 1)
            {
                if (btnHebKeyboard.Text == "Show Keyboard")
                {
                    heightCalculation = keybrdPanelHeight;
                    maxHeight = splitMTLeft.Height;
                    splitMTLeft.SplitterDistance -= 8;
                    if (maxHeight - splitMTLeft.SplitterDistance >= heightCalculation + 5)
                    {
                        btnHebKeyboard.Text = "Hide Keyboard";
                        tmrKeyboard.Enabled = false;
//                        rtxtMainText.Refresh();
                    }
                }
                else
                {
                    splitMTLeft.SplitterDistance += 8;
                    if (splitMTLeft.SplitterDistance >= splitMTLeft.Height - pnlMTHistory.Height)
                    {
                        splitMTLeft.SplitterDistance = splitMTLeft.Height - pnlMTHistory.Height;
                        btnHebKeyboard.Text = "Show Keyboard";
                        tmrKeyboard.Enabled = false;
                    }
                }
            }
            else
            {
                if (btnGkKeyboard.Text == "Show Keyboard")
                {
                    heightCalculation = keybrdPanelHeight;
                    maxHeight = splitLXXLeft.Height;
                    splitLXXLeft.SplitterDistance -= 8;
                    if (maxHeight - splitLXXLeft.SplitterDistance >= heightCalculation + 5)
                    {
                        btnGkKeyboard.Text = "Hide Keyboard";
                        tmrKeyboard.Enabled = false;
                        //                        rtxtMainText.Refresh();
                    }
                }
                else
                {
                    splitLXXLeft.SplitterDistance += 8;
                    if (splitLXXLeft.SplitterDistance >= splitLXXLeft.Height - pnlLXXHistory.Height)
                    {
                        splitLXXLeft.SplitterDistance = splitLXXLeft.Height - pnlLXXHistory.Height;
                        btnGkKeyboard.Text = "Show Keyboard";
                        tmrKeyboard.Enabled = false;
                    }
                }
            }
        }

        private void notesMouseDown(object sender, MouseEventArgs e)
        {
            int tagVal;
            RichTextBox activeRTextArea;

            activeRTextArea = (RichTextBox)sender;
            tagVal = Convert.ToInt32(activeRTextArea.Tag);
            switch( tagVal)
            {
                case 1: globalVars.LatestMTNotesCsrPosition = activeRTextArea.GetCharIndexFromPosition(new Point(e.X, e.Y)); break;
                case 2: globalVars.LatestLXXNotesCsrPosition = activeRTextArea.GetCharIndexFromPosition(new Point(e.X, e.Y)); break;
            }
        }

        private void notesMainMenu(object sender, EventArgs e)
        {
            int tagVal;
            ToolStripMenuItem currentMenuItem;
            frmNotes noteManagement;

            currentMenuItem = (ToolStripMenuItem)sender;
            tagVal = Convert.ToInt32(currentMenuItem.Tag);
            noteManagement = new frmNotes();
            noteManagement.initialiseNotesDialog(globalVars, notesProcessing, tagVal);
            noteManagement.ShowDialog();
            noteManagement.Dispose();
        }

        private void leaveNotesArea(object sender, EventArgs e)
        {
            int tagVal;
            RichTextBox activeArea;

            activeArea = (RichTextBox)sender;
            tagVal = Convert.ToInt32(activeArea.Tag);
            notesProcessing.processOldNote(tagVal);
        }

        private void verseChanged(object sender, EventArgs e)
        {
            int tagVal;
            ComboBox activeArea;

            activeArea = (ComboBox)sender;
            tagVal = Convert.ToInt32(activeArea.Tag);
            notesProcessing.processNewNote(tagVal);
        }

        private void btnKeyboard_Click(object sender, EventArgs e)
        {
            Button currentButton;

            currentButton = (Button)sender;
            if (currentButton == btnHebKeyboard) tmrKeyboard.Tag = 1;
            else tmrKeyboard.Tag = 2;
            tmrKeyboard.Enabled = true;
        }
        private void cMnuCopy_Click(object sender, EventArgs e)
        {
            int tagVal, versionFlag;
            ToolStripMenuItem currentMenuItem;
            frmCopyOptions copyOptions;

            currentMenuItem = (ToolStripMenuItem)sender;
            tagVal = Convert.ToInt32(currentMenuItem.Tag);
            versionFlag = ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 0)).SelectedIndex + 1;
            copyOptions = new frmCopyOptions(tagVal, versionFlag, globalVars, hebrewLexicon, greekOrthography, notesProcessing);
            if (globalVars.getCopyOption((tagVal - 1) * 4 + (versionFlag - 1) * 16 + 3) == 2)
            {
                copyOptions.DestCode = globalVars.getCopyOption((tagVal - 1) * 4 + (versionFlag - 1) * 16);
                copyOptions.RefCode = globalVars.getCopyOption((tagVal - 1) * 4 + (versionFlag - 1) * 16 + 1);
                copyOptions.AccentCode = globalVars.getCopyOption((tagVal - 1) * 4 + (versionFlag - 1) * 16 + 2);
                copyOptions.RememberCode = globalVars.getCopyOption((tagVal - 1) * 4 + (versionFlag - 1) * 16 + 3);
                copyOptions.performCopy();
            }
            else copyOptions.ShowDialog();
        }

        private void mnuTextReset_Click(object sender, EventArgs e)
        {
            bool isUseful = false;
            frmCopyReset copyReset;

            copyReset = new frmCopyReset();
            copyReset.IsMTWordChecked = globalVars.getCopyOption(3);
            copyReset.IsMTVerseChecked = globalVars.getCopyOption(7);
            copyReset.IsMTChapterChecked = globalVars.getCopyOption(11);
            copyReset.IsMTSelectionChecked = globalVars.getCopyOption(15);
            copyReset.IsLXXWordChecked = globalVars.getCopyOption(19);
            copyReset.IsLXXVerseChecked = globalVars.getCopyOption(23);
            copyReset.IsLXXChapterChecked = globalVars.getCopyOption(27);
            copyReset.IsLXXSelectionChecked = globalVars.getCopyOption(31);
            if (copyReset.IsMTWordChecked + copyReset.IsMTVerseChecked + copyReset.IsMTChapterChecked + copyReset.IsMTSelectionChecked +
                copyReset.IsLXXWordChecked + copyReset.IsLXXVerseChecked + copyReset.IsLXXChapterChecked + copyReset.IsLXXSelectionChecked > 0) isUseful = true;
            if ( isUseful)
            {
                copyReset.populateCheckboxes();
                if( copyReset.ShowDialog() == DialogResult.OK)
                {
                    globalVars.setCopyOption(3, copyReset.IsMTWordChecked);
                    globalVars.setCopyOption(7, copyReset.IsMTVerseChecked);
                    globalVars.setCopyOption(11, copyReset.IsMTChapterChecked);
                    globalVars.setCopyOption(15, copyReset.IsMTSelectionChecked);
                    globalVars.setCopyOption(19, copyReset.IsLXXWordChecked);
                    globalVars.setCopyOption(23, copyReset.IsLXXVerseChecked);
                    globalVars.setCopyOption(27, copyReset.IsLXXChapterChecked);
                    globalVars.setCopyOption(31, copyReset.IsLXXSelectionChecked);
                }
            }
        }

        private void cMnuResultsCopy_Click(object sender, EventArgs e)
        {
            int tagVal;
            ToolStripMenuItem currentMenuItem;

            currentMenuItem = (ToolStripMenuItem)sender;
            tagVal = Convert.ToInt32(currentMenuItem.Tag);
            switch( tagVal)
            {
                case 1: searchProcs.copyAllResults(true); break;
                case 2: searchProcs.copyAllResults(false); break;
                case 3: searchProcs.copySingleResult(true); break;
                case 4: searchProcs.copySingleResult(false); break;
            }
        }

        private void cMnuResultsUpdateText_Click(object sender, EventArgs e)
        {
            searchProcs.updateTextAreaWithSelectedChapter();
        }

        private void tabCtrlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabCtrlLanguage.SelectedTab == tabPgeMT)
            {
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 1)).SelectedIndex = 0;
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 5)).SelectedIndex = 0;
                globalVars.activateLexiconTab(1);
                mnuTextKethibQere.Visible = true;
            }
            else
            {
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 1)).SelectedIndex = 1;
                ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 6)).SelectedIndex = 0;
                globalVars.activateLexiconTab(2);
                mnuTextKethibQere.Visible = false;
            }
        }

        private void mnuTextKethibQere_Click(object sender, EventArgs e)
        {
            classKethib_Qere kethibQereDetails;

            kethibQereDetails = globalVars.CurrentlySelectedMTWord.WordVariant;
            labKethibMsg.Text = kethibQereDetails.KethibText;
            labQereMsg.Text = kethibQereDetails.QereText;
            ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 1)).SelectedIndex = 0;
            ((TabControl)globalVars.getGroupedControl(globalVars.TabControlCode, 5)).SelectedIndex = 2;
        }

        private void tmrRightPanes_Tick(object sender, EventArgs e)
        {
            int targetSize, headerHeight;

            headerHeight = tabPgeParse.Top;
            switch (rightPanesControl)
            {
                case 0:   // Maximise top pane
                    {
                        targetSize = splitMain.Panel2.Height - headerHeight;
                        splitRight.SplitterDistance += 4;
                        if (splitRight.SplitterDistance >= targetSize)
                        {
                            tmrRightPanes.Enabled = false;
                            rightPanesControl = -1;
                        }
                    }
                    break;
                case 1:
                    {
                        targetSize = pnlTop.Height + headerHeight;
                        splitRight.SplitterDistance -= 4;
                        if (splitRight.SplitterDistance <= targetSize)
                        {
                            tmrRightPanes.Enabled = false;
                            rightPanesControl = -1;
                        }
                    }
                    break;
                case 2:
                    {
                        targetSize = (splitMain.Panel2.Height + pnlTop.Height) / 2;
                        if (splitRight.SplitterDistance < targetSize)
                        {
                            splitRight.SplitterDistance += 4;
                        }
                        else
                        {
                            splitRight.SplitterDistance -= 4;
                        }
                        if (Math.Abs(splitRight.SplitterDistance - targetSize) < 3)
                        {
                            tmrRightPanes.Enabled = false;
                            rightPanesControl = -1;
                        }
                    }
                    break;
                default: break;
            }
        }

        private void cbRightPanes_SelectedIndexChanged(object sender, EventArgs e)
        {
            rightPanesControl = cbRightPanes.SelectedIndex;
            tmrRightPanes.Enabled = true;
        }

        private void splitMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            appRegistry.updateSplitterDistance();
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            appRegistry.updateWindowSize();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            notesProcessing.processOldNote(1);
            notesProcessing.processOldNote(2);
            appRegistry.updateNotesSet();
        }

        private void frmMain_Move(object sender, EventArgs e)
        {
            if( appRegistry != null) appRegistry.updateWindowPosition();
        }

        private void statLab2_VisibleChanged(object sender, EventArgs e)
        {
            ToolStripStatusLabel currentLabel;

            currentLabel = (ToolStripStatusLabel)sender;
            if (String.Compare(currentLabel.Text, "Current task finished") == 0) btnStop.Visible = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            searchProcs.cancelSearch();
            btnStop.Visible = false;
        }

        private void mnuFilePrefs_Click(object sender, EventArgs e)
        {
            int noOfAreas;
            bool[] hasChanged;
            frmPreferences preferences;

            preferences = new frmPreferences(globalVars, appRegistry);
            preferences.initialiseForm();
            if (preferences.ShowDialog() == DialogResult.OK)
            {
                noOfAreas = preferences.NoOfAreas;
                hasChanged = preferences.HasChanged;
                if (hasChanged[0])
                {
                    mtText.displayChapter(cbMTBook.SelectedIndex, cbMTChapter.SelectedItem.ToString());
                    appRegistry.updateSingleAreaFontsAndColour(0);
                }
                if (hasChanged[1])
                {
                    lxxText.displayChapter(cbLXXBook.SelectedIndex, cbLXXChapter.SelectedItem.ToString());
                    appRegistry.updateSingleAreaFontsAndColour(1);
                }
                if (hasChanged[2]) appRegistry.updateSingleAreaFontsAndColour(2);
                if (hasChanged[3]) appRegistry.updateSingleAreaFontsAndColour(3);
                if (hasChanged[4]) appRegistry.updateSingleAreaFontsAndColour(4);
                if (hasChanged[5]) appRegistry.updateSingleAreaFontsAndColour(5);
                if (hasChanged[6]) appRegistry.updateSingleAreaFontsAndColour(6);
                if (hasChanged[7]) appRegistry.updateSingleAreaFontsAndColour(7);
            }
            preferences.Close();
            preferences.Dispose();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            frmAbout aboutForm;

            aboutForm = new frmAbout();
            aboutForm.ShowDialog();
        }

        private void mnuHelpHelp_Click(object sender, EventArgs e)
        {
            frmHelp helpForm;

            helpForm = new frmHelp();
            helpForm.initialiseHelp(globalVars.FullHelpFile);
            helpForm.Show();
        }

        private void relocateApplicationFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMoveFiles relocationForm;

            relocationForm = new frmMoveFiles();
            relocationForm.registerDestination( globalVars.BaseDirectory );
            if (relocationForm.ShowDialog() == DialogResult.OK)
            {
                relocationForm.Dispose();
                if (appRegistry.relocateFiles(relocationForm.SelectedDestination))
                    MessageBox.Show("Application files successfully relocated", "File Relocation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            performClose();
        }

        private void performClose()
        {
            Application.Exit();
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            performClose();
        }
    }
}
