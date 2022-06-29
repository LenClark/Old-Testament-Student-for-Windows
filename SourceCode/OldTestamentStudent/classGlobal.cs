using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OldTestamentStudent
{
    public class classGlobal
    {
        /*========================================================================*
         *                                                                        * 
         *               Basic Variables                                          * 
         *               ---------------                                          * 
         *                                                                        *
         *========================================================================*/

        /*------------------------------------------------------------------------*
         *                                                                        * 
         *                     Controls from the main form                        * 
         *                     ---------------------------                        * 
         *                                                                        *
         *------------------------------------------------------------------------*/

        Form masterForm;  // I.e. the main form itself

        public Form MasterForm { get => masterForm; set => masterForm = value; }

        /*------------------------------------------------------------------------*
         *                                                                        * 
         *  isOnHold                                                              * 
         *  --------                                                              * 
         *                                                                        * 
         *  If isOnHold is set to true, any change to the chapters ComboBox will  * 
         *    not result in updating the main text area.  This is used in startup * 
         *    and when history is used to change text.                            *
         *                                                                        *
         *------------------------------------------------------------------------*/

        bool isOnHold;
        public bool IsOnHold { get => isOnHold; set => isOnHold = value; }

        /*........................................................................*
         *                                                                        * 
         *                  Groups of controls of the same type                   * 
         *                  -----------------------------------                   * 
         *                                                                        *
         *........................................................................*/

        int richtextBoxCode = 1, comboBoxesCode = 2, tabControlCode = 3, tabPageCode = 4, panelCode = 5, buttonCode = 6, textboxCode = 7, splitContainerCode = 8,
            radioButtonCode = 9, toolStripLabelCode = 10, webBrowserCode = 11, labelCode = 12, listBoxCode = 13, groupboxCode = 14;

        public int RichtextBoxCode { get => richtextBoxCode; set => richtextBoxCode = value; }
        public int ComboBoxesCode { get => comboBoxesCode; set => comboBoxesCode = value; }
        public int TabControlCode { get => tabControlCode; set => tabControlCode = value; }
        public int TabPageCode { get => tabPageCode; set => tabPageCode = value; }
        public int PanelCode { get => panelCode; set => panelCode = value; }
        public int ButtonCode { get => buttonCode; set => buttonCode = value; }
        public int TextboxCode { get => textboxCode; set => textboxCode = value; }
        public int SplitContainerCode { get => splitContainerCode; set => splitContainerCode = value; }
        public int RadioButtonCode { get => radioButtonCode; set => radioButtonCode = value; }
        public int ToolStripLabelCode { get => toolStripLabelCode; set => toolStripLabelCode = value; }
        public int WebBrowserCode { get => webBrowserCode; set => webBrowserCode = value; }
        public int LabelCode { get => labelCode; set => labelCode = value; }
        public int ListBoxCode { get => listBoxCode; set => listBoxCode = value; }
        public int GroupboxCode { get => groupboxCode; set => groupboxCode = value; }

        /*-----------------------------------------------------------------------------------------------------*
         *                                                                                                     *
         *  Richtext boxes     objectTypeCode = 1                                                              *
         *  --------------                                                                                     *
         *                                                                                                     *
         *        Masoretic Side                                      Septuagint Side                          *
         *        --------------                                      ---------------                          *
         *  Index     Specific Richtext Control                Index     Specific Richtext Control             *
         *  -----     -------------------------                -----     -------------------------             *
         *                                                                                                     *
         *    0         rtxtMainMTText                           1         rtxtMainLXXText                     *
         *    2         rtxtMTNotes                              3         rtxtLXXNotes                        *
         *                                                                                                     *
         *                              Combined                                                               *
         *                              --------                                                               *
         *                 Index     Specific Richtext Control                                                 *
         *                 -----     -------------------------                                                 *
         *                                                                                                     *
         *                   4         rtxtParse          (Note: the Lexicon results are html-based)           *
         *                   5         rtxtSearchResults                                                       *
         *                                                                                                     *
         *-----------------------------------------------------------------------------------------------------*/
        int noOfRichtextBoxes = 0;
        SortedList<int, RichTextBox> rtxtCollection = new SortedList<int, RichTextBox>();
        public int NoOfRichtextBoxes { get => noOfRichtextBoxes; set => noOfRichtextBoxes = value; }

        /*-----------------------------------------------------------------------------------------------------*
         *                                                                                                     *
         *  Combo Boxes      objectTypeCode = 2                                                                *
         *  -----------                                                                                        *
         *                                                                                                     *
         *        Masoretic Side                                      Septuagint Side                          *
         *        --------------                                      ---------------                          *
         *  Index     Specific Combobox                        Index     Specific Combobox                     *
         *  -----     -----------------                        -----     -----------------                     *
         *                                                                                                     *
         *    0         cbMTBook                                 3         cbLXXBook                           *
         *    1         cbMTChapter                              4         cbLXXChapter                        *
         *    2         cbMTVerse                                5         cbLXXVerse                          *
         *    6         cbMTHistory                              7         cbLXXHistory                        *
         *                                                                                                     *
         *-----------------------------------------------------------------------------------------------------*/
        int noOfComboBoxes = 0;
        SortedList<int, ComboBox> cbCollection = new SortedList<int, ComboBox>();
        public int NoOfComboBoxes { get => noOfComboBoxes; set => noOfComboBoxes = value; }

        /*-----------------------------------------------------------------------------------------------------*
         *                                                                                                     *
         *  Tab Controls     objectTypeCode = 3                                                                *
         *  ------------                                                                                       *
         *                                                                                                     *
         *    0         tabCtrlLangiage (at the head of the main text area)                                    *
         *    1         tabCtrlFunctions (head of bottom right area, echoing tabCtrlLanguage)                  *
         *    2         tabCtrlHebKeyboard (specific to the MT section of the main area)                       *
         *    3         tabCtrlGkKeyboard (the LXX counterpart of tabCtrlHebKeyboard)                          *
         *    4         tabCtrlTop (the top right tab area)                                                    *
         *    5         tabCtrlMTFunctions (bottom right sub-tabs - only MT)                                   *
         *    6         tabCtrlLXXFunctions (bottom right sub-tabs - only LXX)                                 *
         *    7         tabCtrlLSAppendices (specific tothe Liddell & Scott appendix area)                     *
         *                                                                                                     *
         *-----------------------------------------------------------------------------------------------------*/
        int noOfTabControls = 0;
        SortedList<int, TabControl> tabControlCollection = new SortedList<int, TabControl>();
        public int NoOfTabControls { get => noOfTabControls; set => noOfTabControls = value; }

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
        int noOfTabPages = 0;
        SortedList<int, TabPage> tabPageCollection = new SortedList<int, TabPage>();
        public int NoOfTabPages { get => noOfTabPages; set => noOfTabPages = value; }

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
        int noOfPanels = 0;
        SortedList<int, Panel> panelCollection = new SortedList<int, Panel>();
        public int NoOfPanels { get => noOfPanels; set => noOfPanels = value; }

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
         *                                  2         btnClose                                                 *
         *                                                                                                     *
         *-----------------------------------------------------------------------------------------------------*/
        int noOfButtons = 0;
        SortedList<int, Button> buttonCollection = new SortedList<int, Button>();
        public int NoOfButtons { get => noOfButtons; set => noOfButtons = value; }

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
        int noOfTextboxes = 0;
        SortedList<int, TextBox> textboxCollection = new SortedList<int, TextBox>();
        public int NoOfTextboxes { get => noOfTextboxes; set => noOfTextboxes = value; }

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
        int noOfSplitContainers = 0;
        SortedList<int, SplitContainer> splitContainerCollection = new SortedList<int, SplitContainer>();
        public int NoOfSplitContainers { get => noOfSplitContainers; set => noOfSplitContainers = value; }

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
        int noOfRadioButtons = 0;
        SortedList<int, RadioButton> radioButtonCollection = new SortedList<int, RadioButton>();
        public int NoOfRadioButtons { get => noOfRadioButtons; set => noOfRadioButtons = value; }

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
        int noOfToolStripLabels = 0;
        SortedList<int, ToolStripStatusLabel> toolStripLabelCollection = new SortedList<int, ToolStripStatusLabel>();
        public int NoOfToolStripLabels { get => noOfToolStripLabels; set => noOfToolStripLabels = value; }

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
        int noOfWebBrowsers = 0;
        SortedList<int, WebBrowser> webBrowserCollection = new SortedList<int, WebBrowser>();

        public int NoOfWebBrowsers { get => noOfWebBrowsers; set => noOfWebBrowsers = value; }

        /*-----------------------------------------------------------------------------------------------------*
         *                                                                                                     *
         *  Labels (needing global access)     objectTypeCode = 12                                             *
         *  ------                                                                                             *
         *                                                                                                     *
         *        Masoretic Side                                      Septuagint Side                          *
         *        --------------                                      ---------------                          *
         *  Index     Specific Textbox                         Index     Specific Textbox                      *
         *  -----     ----------------                         -----     ----------------                      *
         *                                                                                                     *
         *    0      labMTSearchLbl                              3         labLXXSearchLabel                   *
         *    1      labMTWithinLbl                              4         labLXXWithin                        *
         *    2      labMTWordsOfLbl                             5         labLXXWordsOf                       *
         *                                                                                                     *
         *-----------------------------------------------------------------------------------------------------*/
        int noOfLabels = 0;
        SortedList<int, Label> labelCollection = new SortedList<int, Label>();

        public int NoOfLabels { get => noOfLabels; set => noOfLabels = value; }

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
        int noOfListBoxes = 0;
        SortedList<int, ListBox> listBoxCollection = new SortedList<int, ListBox>();

        public int NoOfListBoxes { get => noOfListBoxes; set => noOfListBoxes = value; }

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
        int noOfGroupBoxes = 0;
        SortedList<int, GroupBox> groupBoxCollection = new SortedList<int, GroupBox>();

        public int NoOfGroupBoxes { get => noOfGroupBoxes; set => noOfGroupBoxes = value; }

        public void addGroupedControl(object newControl, int objectTypeCode)
        {
            switch (objectTypeCode)
            {
                case 1: rtxtCollection.Add(noOfRichtextBoxes++, (RichTextBox)newControl); break;  // Rich text boxes
                case 2: cbCollection.Add(noOfComboBoxes++, (ComboBox)newControl); break;           // Combo boxes
                case 3: tabControlCollection.Add(noOfTabControls++, (TabControl)newControl); break;
                case 4: tabPageCollection.Add(noOfTabPages++, (TabPage)newControl); break;
                case 5: panelCollection.Add(noOfPanels++, (Panel)newControl); break;
                case 6: buttonCollection.Add(noOfButtons++, (Button)newControl); break;
                case 7: textboxCollection.Add(noOfTextboxes++, (TextBox)newControl); break;
                case 8: splitContainerCollection.Add(noOfSplitContainers++, (SplitContainer)newControl); break;
                case 9: radioButtonCollection.Add(noOfRadioButtons++, (RadioButton)newControl); break;
                case 10: toolStripLabelCollection.Add(noOfToolStripLabels++, (ToolStripStatusLabel)newControl); break;
                case 11: webBrowserCollection.Add(noOfWebBrowsers++, (WebBrowser)newControl); break;
                case 12: labelCollection.Add(noOfLabels++, (Label)newControl); break;
                case 13: listBoxCollection.Add(noOfListBoxes++, (ListBox)newControl); break;
                case 14: groupBoxCollection.Add(noOfGroupBoxes++, (GroupBox)newControl); break;
            }
        }

        public Object getGroupedControl(int objectTypeCode, int Index)
        {
            switch (objectTypeCode)
            {
                case 1: // Rich text boxes
                    RichTextBox newVal1;
                    rtxtCollection.TryGetValue(Index, out newVal1);
                    return newVal1;
                case 2:            // Combo boxes
                    ComboBox newVal2;
                    cbCollection.TryGetValue(Index, out newVal2);
                    return newVal2;
                case 3:
                    TabControl newVal3;
                    tabControlCollection.TryGetValue(Index, out newVal3);
                    return newVal3;
                case 4:
                    TabPage newVal4;
                    tabPageCollection.TryGetValue(Index, out newVal4);
                    return newVal4;
                case 5:
                    Panel newVal5;
                    panelCollection.TryGetValue(Index, out newVal5);
                    return newVal5;
                case 6:
                    Button newVal6;
                    buttonCollection.TryGetValue(Index, out newVal6);
                    return newVal6;
                case 7:
                    TextBox newVal7;
                    textboxCollection.TryGetValue(Index, out newVal7);
                    return newVal7;
                case 8:
                    SplitContainer newVal8;
                    splitContainerCollection.TryGetValue(Index, out newVal8);
                    return newVal8;
                case 9:
                    RadioButton newVal9;
                    radioButtonCollection.TryGetValue(Index, out newVal9);
                    return newVal9;
                case 10:
                    ToolStripStatusLabel newVal10;
                    toolStripLabelCollection.TryGetValue(Index, out newVal10);
                    return newVal10;
                case 11:
                    WebBrowser newVal11;
                    webBrowserCollection.TryGetValue(Index, out newVal11);
                    return newVal11;
                case 12:
                    Label newVal12;
                    labelCollection.TryGetValue(Index, out newVal12);
                    return newVal12;
                case 13:
                    ListBox newVal13;
                    listBoxCollection.TryGetValue(Index, out newVal13);
                    return newVal13;
                case 14:
                    GroupBox newVal14;
                    groupBoxCollection.TryGetValue(Index, out newVal14);
                    return newVal14;
                default: return null;
            }
        }

        /*-----------------------------------------------------------------------------------------------------*
         *                                                                                                     *
         *  Specific elements of other types:                                                                  *
         *  --------------------------------                                                                   *
         *                                                                                                     *
         *-----------------------------------------------------------------------------------------------------*/

        WebBrowser lexiconWebPage;
        ToolStripMenuItem parentMenuItem;
        StatusStrip statSearch;
        ContextMenuStrip searchContextMenu;

        public WebBrowser LexiconWebPage { get => lexiconWebPage; set => lexiconWebPage = value; }
        public ToolStripMenuItem ParentMenuItem { get => parentMenuItem; set => parentMenuItem = value; }
        public StatusStrip StatSearch { get => statSearch; set => statSearch = value; }
        public ContextMenuStrip SearchContextMenu { get => searchContextMenu; set => searchContextMenu = value; }

        /*==================================================================================================================================================*/

        /*------------------------------------------------------------------------*
         *                                                                        * 
         *               Variables that are passed between classes                * 
         *               -----------------------------------------                * 
         *                                                                        *
         *------------------------------------------------------------------------*/

        int historyMax, selectedMTWordSequence, selectedLXXWordSequence, lastSearchPosition;
        String lastSelectedMTVerse, lastSelectedMTWord, lastSelectedLXXVerse, lastSelectedLXXWord, lastMTHistoryEntry = "", lastLXXHistoryEntry = "", lastSelectedSearchVerse;
        classMTWord currentlySelectedMTWord;
        classLXXWord currentlySelectedLXXWord;

        public int HistoryMax { get => historyMax; set => historyMax = value; }
        public int SelectedMTWordSequence { get => selectedMTWordSequence; set => selectedMTWordSequence = value; }
        public int SelectedLXXWordSequence { get => selectedLXXWordSequence; set => selectedLXXWordSequence = value; }
        public int LastSearchPosition { get => lastSearchPosition; set => lastSearchPosition = value; }
        public string LastSelectedMTVerse { get => lastSelectedMTVerse; set => lastSelectedMTVerse = value; }
        public string LastSelectedMTWord { get => lastSelectedMTWord; set => lastSelectedMTWord = value; }
        public string LastSelectedLXXVerse { get => lastSelectedLXXVerse; set => lastSelectedLXXVerse = value; }
        public string LastSelectedLXXWord { get => lastSelectedLXXWord; set => lastSelectedLXXWord = value; }
        public string LastMTHistoryEntry { get => lastMTHistoryEntry; set => lastMTHistoryEntry = value; }
        public string LastLXXHistoryEntry { get => lastLXXHistoryEntry; set => lastLXXHistoryEntry = value; }
        public string LastSelectedSearchVerse { get => lastSelectedSearchVerse; set => lastSelectedSearchVerse = value; }
        internal classMTWord CurrentlySelectedMTWord { get => currentlySelectedMTWord; set => currentlySelectedMTWord = value; }
        internal classLXXWord CurrentlySelectedLXXWord { get => currentlySelectedLXXWord; set => currentlySelectedLXXWord = value; }

        /*------------------------------------------------------------------------*
         *                                                                        * 
         *               Window position (and other controls)                     * 
         *               ------------------------------------                     * 
         *                                                                        *
         *------------------------------------------------------------------------*/

        int windowX, windowY, windowWidth, windowHeight, windowState;
        int splitPstn;
        int workingInt = 0, noOfColours;
        int latestMousePosition = 0, latestMTNotesCsrPosition = 0, latestLXXNotesCsrPosition = 0;

        public int WindowX { get => windowX; set => windowX = value; }
        public int WindowY { get => windowY; set => windowY = value; }
        public int WindowWidth { get => windowWidth; set => windowWidth = value; }
        public int WindowHeight { get => windowHeight; set => windowHeight = value; }
        public int WindowState { get => windowState; set => windowState = value; }
        public int SplitPstn { get => splitPstn; set => splitPstn = value; }
        public int WorkingInt { get => workingInt; set => workingInt = value; }
        public int LatestMousePosition { get => latestMousePosition; set => latestMousePosition = value; }
        public int LatestMTNotesCsrPosition { get => latestMTNotesCsrPosition; set => latestMTNotesCsrPosition = value; }
        public int LatestLXXNotesCsrPosition { get => latestLXXNotesCsrPosition; set => latestLXXNotesCsrPosition = value; }
        public int NoOfColours { get => noOfColours; set => noOfColours = value; }

        /*------------------------------------------------------------------------------------------------------------*
         *                                                                                                            * 
         *                                               File definitions                                             *
         *                                               ----------------                                             *
         *                                                                                                            *
         *  Note: the folder names are _not_ generally needed in other classes.  The fully composed path name (i.e.   *
         *    base directory + one or more folder names) is.  Hence, the public encapsulations are generally only     *
         *    provided for "full..." path variables.                                                                  *
         *                                                                                                            *
         *------------------------------------------------------------------------------------------------------------*/
        int maxMTNoteRef = -1, maxLXXNoteRef = -1, currentMTNoteRef = -1, currentLXXNoteRef = -1;
        String baseDirectory = @"..\Source";
        String notesPath = "Notes", mtNotesName = "Default", lxxNotesName = "Default";
        String lsApp1 = "L1_authors_and_works.html", lsApp2 = "L2_epigraphical_publications.html",
            lsApp3 = "L3_papyrological_publications.html", lsApp4 = "L4_periodicals.html", lsApp5 = "L5_general_abbreviations.html";
        /*        String mtTitlesFile = "Titles.txt", lxxTitlesFile = "LXX_Titles.txt";
                String mtSourceFile = "OTText.txt", lxxTextFolder = "LXX_Text";
                String lexiconFile = "BDB.csv", convertFile = "WordToBdb.txt", codeFile = "Codes.txt", kethibQereFile = "Kethib_Qere.txt";
                String gkLexiconFolder = "GkLexicon", mainLexFile = "LandSSummary.txt", lsApp1 = "L1_authors_and_works.html", lsApp2 = "L2_epigraphical_publications.html",
                    lsApp3 = "L3_papyrological_publications.html", lsApp4 = "L4_periodicals.html", lsApp5 = "L5_general_abbreviations.html";

                String helpPath = "Help", helpFile = "Help.html";
                String mtNotesFolder = "MT", lxxNotesFolder = "LXX";
                String keyboardFolder = "Keyboard", greekControlFolder = "Greek";
                String gkAccute = "accuteAccents.txt", gkCircumflex = "circumflexAccents.txt", gkDiaereses = "diaereses.txt", gkGrave = "graveAccents.txt",
                    gkIota = "iotaSubscripts.txt", gkRough = "roughBreathings.txt", gkSmooth = "smoothBreathings.txt", 
                    gkConv1 = "breathingConversion1.txt", gkConv2 = "breathingConversion2.txt"; */

        String fullMTTitleFile, fullLXXTitleFile, fullMTSourceFile, fullLXXTextFolder;
        String fullMTNotesPath, fullLXXNotesPath, fullLexiconFile, fullGkLexiconFile, fullGkLexiconFolder, fullConvertFile, fullKethibQereFile, fullCodeFile, fullHelpFile, fullKeyboardFolder;
        String fullGkAccute, fullGkCircumflex, fullGkDiaereses, fullGkGrave, fullGkIota, fullGkRough, fullGkSmooth, fullGkConv1, fullGkConv2;

        public int MaxMTNoteRef { get => maxMTNoteRef; set => maxMTNoteRef = value; }
        public int MaxLXXNoteRef { get => maxLXXNoteRef; set => maxLXXNoteRef = value; }
        public int CurrentMTNoteRef { get => currentMTNoteRef; set => currentMTNoteRef = value; }
        public int CurrentLXXNoteRef { get => currentLXXNoteRef; set => currentLXXNoteRef = value; }
        public string BaseDirectory { get => baseDirectory; set => baseDirectory = value; }
        public string NotesPath { get => notesPath; set => notesPath = value; }
        public string MtNotesName { get => mtNotesName; set => mtNotesName = value; }
        public string LxxNotesName { get => lxxNotesName; set => lxxNotesName = value; }
        public string FullMTTitleFile { get => fullMTTitleFile; set => fullMTTitleFile = value; }
        public string FullLXXTitleFile { get => fullLXXTitleFile; set => fullLXXTitleFile = value; }
        public string FullMTSourceFile { get => fullMTSourceFile; set => fullMTSourceFile = value; }
        public string FullLXXTextFolder { get => fullLXXTextFolder; set => fullLXXTextFolder = value; }
        public String FullMTNotesPath { get => fullMTNotesPath; set => fullMTNotesPath = value; }
        public String FullLXXNotesPath { get => fullLXXNotesPath; set => fullLXXNotesPath = value; }
        public string FullLexiconFile { get => fullLexiconFile; set => fullLexiconFile = value; }
        public string FullGkLexiconFolder { get => fullGkLexiconFolder; set => fullGkLexiconFolder = value; }
        public string FullGkLexiconFile { get => fullGkLexiconFile; set => fullGkLexiconFile = value; }
        public string LsApp1 { get => lsApp1; set => lsApp1 = value; }
        public string LsApp2 { get => lsApp2; set => lsApp2 = value; }
        public string LsApp3 { get => lsApp3; set => lsApp3 = value; }
        public string LsApp4 { get => lsApp4; set => lsApp4 = value; }
        public string LsApp5 { get => lsApp5; set => lsApp5 = value; }
        public string FullConvertFile { get => fullConvertFile; set => fullConvertFile = value; }
        public string FullKethibQereFile { get => fullKethibQereFile; set => fullKethibQereFile = value; }
        public string FullCodeFile { get => fullCodeFile; set => fullCodeFile = value; }
        public string FullHelpFile { get => fullHelpFile; set => fullHelpFile = value; }
        public string FullKeyboardFolder { get => fullKeyboardFolder; set => fullKeyboardFolder = value; }
        public string FullGkAccute { get => fullGkAccute; set => fullGkAccute = value; }
        public string FullGkCircumflex { get => fullGkCircumflex; set => fullGkCircumflex = value; }
        public string FullGkDiaereses { get => fullGkDiaereses; set => fullGkDiaereses = value; }
        public string FullGkGrave { get => fullGkGrave; set => fullGkGrave = value; }
        public string FullGkIota { get => fullGkIota; set => fullGkIota = value; }
        public string FullGkRough { get => fullGkRough; set => fullGkRough = value; }
        public string FullGkSmooth { get => fullGkSmooth; set => fullGkSmooth = value; }
        public string FullGkConv1 { get => fullGkConv1; set => fullGkConv1 = value; }
        public string FullGkConv2 { get => fullGkConv2; set => fullGkConv2 = value; }

        /*------------------------------------------------------------------------*
         *                                                                        * 
         *             Variables relating to the virtual keyboards                * 
         *             -------------------------------------------                * 
         *                                                                        *
         *------------------------------------------------------------------------*/
        RadioButton[] rbtnMTDestination, rbtnLXXDestination;

        public RadioButton[] RbtnMTDestination { get => rbtnMTDestination; set => rbtnMTDestination = value; }
        public RadioButton[] RbtnLXXDestination { get => rbtnLXXDestination; set => rbtnLXXDestination = value; }

        /*------------------------------------------------------------------------*
         *                                                                        * 
         *               Variables relating to search processing                  * 
         *               ---------------------------------------                  * 
         *                                                                        *
         *------------------------------------------------------------------------*/
        int primaryMTBookId, primaryMTWordSeq, secondaryMTBookId, secondaryMTWordSeq;
        int primaryLXXBookId, primaryLXXWordSeq, secondaryLXXBookId, secondaryLXXWordSeq;
        int noOfMTBookGroups = 6, noOfLXXBookGroups = 7, noOfSearchObjects = 0, noOfMTResultsItems, noOfLXXResultsItems, mtRowIndex, lxxRowIndex;
        String primaryMTWord, secondaryMTWord, primaryMTChapNo, primaryMTVNo, secondaryMTChapNo, secondaryMTVNo;
        String primaryLXXWord, secondaryLXXWord, primaryLXXChapNo, primaryLXXVNo, secondaryLXXChapNo, secondaryLXXVNo;
        String[] categoryName = { "Pentateuch", "History\nFormer Prophets", "Major Prophets", "Minor Prophets", "Kethubim - Poetry", "Kethubim - History", "Pseudepigrapha" };
        NumericUpDown udMTScan, udLXXScan;
        Button btnMTSearchType, btnLXXSearchType;
        CheckBox[] chkMTBooks, chkLXXBooks;
        RichTextBox[,] mtSearchText, lxxSearchText;
        FlowLayoutPanel flowResults;

        public int PrimaryMTBookId { get => primaryMTBookId; set => primaryMTBookId = value; }
        public int PrimaryMTWordSeq { get => primaryMTWordSeq; set => primaryMTWordSeq = value; }
        public int SecondaryMTBookId { get => secondaryMTBookId; set => secondaryMTBookId = value; }
        public int SecondaryMTWordSeq { get => secondaryMTWordSeq; set => secondaryMTWordSeq = value; }
        public int PrimaryLXXBookId { get => primaryLXXBookId; set => primaryLXXBookId = value; }
        public int PrimaryLXXWordSeq { get => primaryLXXWordSeq; set => primaryLXXWordSeq = value; }
        public int SecondaryLXXBookId { get => secondaryLXXBookId; set => secondaryLXXBookId = value; }
        public int SecondaryLXXWordSeq { get => secondaryLXXWordSeq; set => secondaryLXXWordSeq = value; }
        public int NoOfMTBookGroups { get => noOfMTBookGroups; set => noOfMTBookGroups = value; }
        public int NoOfLXXBookGroups { get => noOfLXXBookGroups; set => noOfLXXBookGroups = value; }
        public int NoOfSearchObjects { get => noOfSearchObjects; set => noOfSearchObjects = value; }
        public int NoOfMTResultsItems { get => noOfMTResultsItems; set => noOfMTResultsItems = value; }
        public int NoOfLXXResultsItems { get => noOfLXXResultsItems; set => noOfLXXResultsItems = value; }
        public int MtRowIndex { get => mtRowIndex; set => mtRowIndex = value; }
        public int LxxRowIndex { get => lxxRowIndex; set => lxxRowIndex = value; }
        public string PrimaryMTWord { get => primaryMTWord; set => primaryMTWord = value; }
        public string SecondaryMTWord { get => secondaryMTWord; set => secondaryMTWord = value; }
        public string PrimaryMTChapNo { get => primaryMTChapNo; set => primaryMTChapNo = value; }
        public string PrimaryMTVNo { get => primaryMTVNo; set => primaryMTVNo = value; }
        public string SecondaryMTChapNo { get => secondaryMTChapNo; set => secondaryMTChapNo = value; }
        public string SecondaryMTVNo { get => secondaryMTVNo; set => secondaryMTVNo = value; }
        public string PrimaryLXXWord { get => primaryLXXWord; set => primaryLXXWord = value; }
        public string SecondaryLXXWord { get => secondaryLXXWord; set => secondaryLXXWord = value; }
        public string PrimaryLXXChapNo { get => primaryLXXChapNo; set => primaryLXXChapNo = value; }
        public string PrimaryLXXVNo { get => primaryLXXVNo; set => primaryLXXVNo = value; }
        public string SecondaryLXXChapNo { get => secondaryLXXChapNo; set => secondaryLXXChapNo = value; }
        public string SecondaryLXXVNo { get => secondaryLXXVNo; set => secondaryLXXVNo = value; }
        public string[] CategoryName { get => categoryName; set => categoryName = value; }
        public NumericUpDown UdMTScan { get => udMTScan; set => udMTScan = value; }
        public NumericUpDown UdLXXScan { get => udLXXScan; set => udLXXScan = value; }
        public Button BtnMTSearchType { get => btnMTSearchType; set => btnMTSearchType = value; }
        public Button BtnLXXSearchType { get => btnLXXSearchType; set => btnLXXSearchType = value; }
        public CheckBox[] ChkMTBooks { get => chkMTBooks; set => chkMTBooks = value; }
        public CheckBox[] ChkLXXBooks { get => chkLXXBooks; set => chkLXXBooks = value; }
        public FlowLayoutPanel FlowResults { get => flowResults; set => flowResults = value; }
        public RichTextBox[,] MtSearchText { get => mtSearchText; set => mtSearchText = value; }
        public RichTextBox[,] LxxSearchText { get => lxxSearchText; set => lxxSearchText = value; }

        public void initialiseGlobal()
        {
            historyMax = 99;
            chkMTBooks = new CheckBox[noOfMTBookGroups];
            chkLXXBooks = new CheckBox[noOfLXXBookGroups];
        }

        /*===============================================================================================*
         *                                                                                               *
         *                                        mtBookList                                             *
         *                                        ----------                                             *
         *                                                                                               *
         *  The definitive repository for Hebrew/Aramaic (Massoretic) books.                             *
         *                                                                                               *
         *     Key:     Integer sequence (starting from zero);                                           *
         *     Value:   The class instance for Hebrew books                                              *
         *                                                                                               *
         *===============================================================================================*/
        int noOfMTBooks;
        SortedDictionary<int, classMTBook> mtBookList = new SortedDictionary<int, classMTBook>();

        /*===============================================================================================*
         *                                                                                               *
         *                                        lxxBookList                                            *
         *                                        -----------                                            *
         *                                                                                               *
         *  The definitive repository for Septuagint (LXX) books.                                        *
         *                                                                                               *
         *     Key:     Integer sequence (starting from zero);                                           *
         *     Value:   The class instance for Hebrew books                                              *
         *                                                                                               *
         *===============================================================================================*/
        int noOfLXXBooks;
        SortedDictionary<int, classLXXBook> lxxBookList = new SortedDictionary<int, classLXXBook>();

        /*===============================================================================================*
         *                                                                                               *
         *                            mtCurrentBookIndex and lxxCurrentBookIndex                         *
         *                            ------------------------------------------                         *
         *                                                                                               *
         *  Each variable stores the ComboBox index (cbMtBook, cbLxxBook) for the initially selected     *
         *    book.  It is used in the setup process, allowing us to avoid updating the ComboBoxes       *
         *    within the initialisation thread.                                                          *
         *                                                                                               *
         *===============================================================================================*/
        int mtCurrentBookIndex, lxxCurrentBookIndex;

        /*===============================================================================================*
         *                                                                                               *
         *                              mtCurrentChapter and lxxCurrentChapter                           *
         *                              --------------------------------------                           *
         *                                                                                               *
         *  As mtCurrentBookIndex and lxxCurrentBookIndex but recording the related chapters.            *
         *                                                                                               *
         *===============================================================================================*/
        String mtCurrentChapter, lxxCurrentChapter;

        public int NoOfMTBooks { get => noOfMTBooks; set => noOfMTBooks = value; }
        internal SortedDictionary<int, classMTBook> MtBookList { get => mtBookList; set => mtBookList = value; }
        public int NoOfLXXBooks { get => noOfLXXBooks; set => noOfLXXBooks = value; }
        internal SortedDictionary<int, classLXXBook> LxxBookList { get => lxxBookList; set => lxxBookList = value; }
        public int MtCurrentBookIndex { get => mtCurrentBookIndex; set => mtCurrentBookIndex = value; }
        public int LxxCurrentBookIndex { get => lxxCurrentBookIndex; set => lxxCurrentBookIndex = value; }
        public string MtCurrentChapter { get => mtCurrentChapter; set => mtCurrentChapter = value; }
        public string LxxCurrentChapter { get => lxxCurrentChapter; set => lxxCurrentChapter = value; }

        /*===============================================================================================*
         *                                                                                               *
         *                                   Items managed by Preferences                                *
         *                                   ----------------------------                                *
         *                                                                                               *
         *  Each item is handled by a SortedList.  The key will be the index of the tab in Preferences.  *
         *                                                                                               *
         *===============================================================================================*/

        /*-----------------------------------------------------------------------------------------------*
         *                                                                                               *
         *                                         Text sizes                                            *
         *                                         ----------                                            *
         *                                                                                               *
         *  Each richtext box is capable of displaying text of varying size.  We will only distinguish   *
         *    two sizes:                                                                                 *
         *      Normal text (i.e. English)                                                               *
         *      Enlarged text (i.e. Hebrew)                                                              *
         *                                                                                               *
         *  However, we can choose different sizes for both text types in different richtext areas.  For *
         *    each list:                                                                                 *
         *                                                                                               *
         *      Key:   The coded value for a rich text area.  (These will be the same values as the keys *
         *               of rtxtCollection                                                               *
         *      Value: The font size for that text area                                                  *
         *                                                                                               *
         *-----------------------------------------------------------------------------------------------*/
        SortedList<int, float> englishTextSize = new SortedList<int, float>();
        SortedList<int, float> hebrewTextSize = new SortedList<int, float>();
        SortedList<int, float> primaryTextSize = new SortedList<int, float>();
        SortedList<int, float> secondaryTextSize = new SortedList<int, float>();

        public void addTextSize(int index, float newSize, int textType)
        {
            /*===========================================================================*
             *                                                                           *
             *                            addTextSize                                    *
             *                                                                           *
             *  Generic addition method.  The parameter, textType is:                    *
             *                                                                           *
             *     Code           Meaning                                                *
             *      1           English Text Size                                        *
             *      2           Hebrew (or Greek) Text Size                              *
             *      3           Search text - Primary match text size                    *
             *      4           Search text - Secondary match text size                  *
             *                                                                           *
             *===========================================================================*/

            SortedList<int, float> genericTextSize;

            switch (textType)
            {
                case 1: genericTextSize = englishTextSize; break;
                case 2: genericTextSize = hebrewTextSize; break;
                case 3: genericTextSize = primaryTextSize; break;
                case 4: genericTextSize = secondaryTextSize; break;
                default: genericTextSize = null; break;
            }
            if (genericTextSize == null) return;
            if (genericTextSize.ContainsKey(index)) genericTextSize.Remove(index);
            genericTextSize.Add(index, newSize);
        }

        public float getTextSize(int index, int textType)
        {
            float currentSize;
            SortedList<int, float> genericTextSize;

            switch (textType)
            {
                case 1: genericTextSize = englishTextSize; break;
                case 2: genericTextSize = hebrewTextSize; break;
                case 3: genericTextSize = primaryTextSize; break;
                case 4: genericTextSize = secondaryTextSize; break;
                default: genericTextSize = null; break;
            }
            if (genericTextSize == null) return 0;
            if (!genericTextSize.ContainsKey(index)) return 0;
            genericTextSize.TryGetValue(index, out currentSize);
            return currentSize;
        }

        /*-----------------------------------------------------------------------------------------------*
         *                                                                                               *
         *                                     Colour collections                                        *
         *                                     ------------------                                        *
         *                                                                                               *
         *  Each richtext box is capable of displaying text (and backgrounds) of various colours.  Any   *
         *    text area may display any of the following colours:                                        *
         *                                                                                               *
         *      0  Background colour;                                                                    *
         *      1  Main (prime) text colour (for normal text)                                            *
         *      2  An alternative colour (e.g. for Hebrew, Greek, Headings, etc.)                        *
         *      3  An alternative colour (e.g. for primary word matches and Kethib/Qere markers)         *
         *      4  A second alternative colour (e.g. for a secondary search word)                        *
         *                                                                                               *
         *  These colours may vary for each text area so the index represents the relevant text area to  *
         *    which the colour applies.  The colourCode identifies the above colour use.                 *
         *                                                                                               *
         *-----------------------------------------------------------------------------------------------*/
        SortedList<int, Color> backColourRec = new SortedList<int, Color>();
        SortedList<int, Color> foreMainColourRec = new SortedList<int, Color>();
        SortedList<int, Color> foreAltColour = new SortedList<int, Color>();
        SortedList<int, Color> forePrimeColour = new SortedList<int, Color>();
        SortedList<int, Color> foreSecondColour = new SortedList<int, Color>();

        public SortedList<int, Color> BackColourRec { get => backColourRec; set => backColourRec = value; }
        public SortedList<int, Color> ForeMainColourRec { get => foreMainColourRec; set => foreMainColourRec = value; }
        public SortedList<int, Color> ForeAltColour { get => foreAltColour; set => foreAltColour = value; }
        public SortedList<int, Color> ForePrimeColour { get => forePrimeColour; set => forePrimeColour = value; }
        public SortedList<int, Color> ForeSecondColour { get => foreSecondColour; set => foreSecondColour = value; }

        public void addColourSetting(int index, Color newColour, int colourType)
        {
            /*===========================================================================*
             *                                                                           *
             *                          addColourSetting                                 *
             *                                                                           *
             *  Generic addition method.  The parameter, textType is:                    *
             *                                                                           *
             *     Code           Meaning                                                *
             *      0           Background colour                                        *
             *      1           Main text colour                                         *
             *      2           Alternative text colour                                  *
             *      3           Primary alternative text colour                          *
             *      4           Second alternative text colour                           *
             *                                                                           *
             *===========================================================================*/

            SortedList<int, Color> genericColour;

            switch (colourType)
            {
                case 0: genericColour = backColourRec; break;
                case 1: genericColour = foreMainColourRec; break;
                case 2: genericColour = foreAltColour; break;
                case 3: genericColour = forePrimeColour; break;
                case 4: genericColour = foreSecondColour; break;
                default: genericColour = null; break;
            }
            if (genericColour == null) return;
            if (genericColour.ContainsKey(index)) genericColour.Remove(index);
            genericColour.Add(index, newColour);
        }

        public Color getColourSetting(int index, int colourType)
        {
            Color colourItem;
            SortedList<int, Color> genericColour;

            switch (colourType)
            {
                case 0: genericColour = backColourRec; break;
                case 1: genericColour = foreMainColourRec; break;
                case 2: genericColour = foreAltColour; break;
                case 3: genericColour = forePrimeColour; break;
                case 4: genericColour = foreSecondColour; break;
                default: genericColour = null; break;
            }
            if (genericColour == null) return Color.Empty;
            if (!genericColour.ContainsKey(index)) return Color.Empty;
            genericColour.TryGetValue(index, out colourItem);
            return colourItem;
        }

        SortedList<int, String> englishMainStyle = new SortedList<int, String>();
        SortedList<int, String> foreignMainStyle = new SortedList<int, String>();
        SortedList<int, String> primaryAltStyle = new SortedList<int, String>();
        SortedList<int, String> secondaryAltStyle = new SortedList<int, String>();

        public void addDefinedStyle(int index, String newStyle, int textType)
        {
            /*===========================================================================*
             *                                                                           *
             *                           addDefinedStyle                                 *
             *                                                                           *
             *  By style is meant bold, italic and strikethrough text.                   *
             *                                                                           *
             *===========================================================================*/

            SortedList<int, String> genericStyle = null;

            switch (textType)
            {
                case 1: genericStyle = englishMainStyle; break;
                case 2: genericStyle = foreignMainStyle; break;
                case 3: genericStyle = primaryAltStyle; break;
                case 4: genericStyle = secondaryAltStyle; break;
            }
            if (genericStyle == null) return;
            if (genericStyle.ContainsKey(index)) genericStyle.Remove(index);
            genericStyle.Add(index, newStyle);
        }

        public String getDefinedStyleByIndex(int index, int textType)
        {
            String newStyle;
            SortedList<int, String> genericStyle = null;

            switch (textType)
            {
                case 1: genericStyle = englishMainStyle; break;
                case 2: genericStyle = foreignMainStyle; break;
                case 3: genericStyle = primaryAltStyle; break;
                case 4: genericStyle = secondaryAltStyle; break;
            }
            if (genericStyle == null) return "";
            if (!genericStyle.ContainsKey(index)) return "";
            genericStyle.TryGetValue(index, out newStyle);
            return newStyle;
        }

        SortedList<int, String> englishFontName = new SortedList<int, String>();
        SortedList<int, String> foreignFontName = new SortedList<int, String>();
        SortedList<int, String> primaryFontName = new SortedList<int, String>();
        SortedList<int, String> secondaryFontName = new SortedList<int, String>();

        public void addFontName(int index, String fontName, int textType)
        {
            /*===========================================================================*
             *                                                                           *
             *                             addFontName                                   *
             *                                                                           *
             *  Font name such as "Times New Roman" or "Arial".  (The options wil be     *
             *    limited because they have to be Unicode compliant.)                    *
             *                                                                           *
             *===========================================================================*/

            SortedList<int, String> genericFontName = null;

            switch (textType)
            {
                case 1: genericFontName = englishFontName; break;
                case 2: genericFontName = foreignFontName; break;
                case 3: genericFontName = primaryFontName; break;
                case 4: genericFontName = secondaryFontName; break;
            }
            if (genericFontName == null) return;
            if (genericFontName.ContainsKey(index)) genericFontName.Remove(index);
            genericFontName.Add(index, fontName);
        }

        public String getDefinedFontNameByIndex(int index, int textType)
        {
            String newFontName;
            SortedList<int, String> genericFontName = null;

            switch (textType)
            {
                case 1: genericFontName = englishFontName; break;
                case 2: genericFontName = foreignFontName; break;
                case 3: genericFontName = primaryFontName; break;
                case 4: genericFontName = secondaryFontName; break;
            }
            if (genericFontName == null) return "";
            if (!genericFontName.ContainsKey(index)) return "";
            genericFontName.TryGetValue(index, out newFontName);
            return newFontName;
        }

        public void activateLexiconTab(int tabCode)
        {
            /*==================================================================================*
             *                                                                                  *
             *                              activateLexiconTab                                  *
             *                              ==================                                  *
             *                                                                                  *
             *  Purpose:                                                                        *
             *  =======                                                                         *
             *                                                                                  *
             *  To display one right-hand lexicon tab (either MT or LXX) and hide the other.    *
             *    Which will be displayed and which will be hidden is determined by the         *
             *    tabCode, as follows:                                                          *
             *                                                                                  *
             *     Code           display            hide                                       *
             *     ----           -------            ----                                       *
             *                                                                                  *
             *      1          the MT lexicon     the LXX lexicon                               *
             *      2          the LXX lexicon    the MT lexicon                                *
             *                                                                                  *
             *==================================================================================*/

            switch (tabCode)
            {
                case 1:
                    if (((TabControl)getGroupedControl(TabControlCode, 4)).TabPages[1] == (TabPage)getGroupedControl(tabPageCode, 13))
                    {
                        // i.e. if the 2nd tab page is the MT lexicon
                        if (((TabControl)getGroupedControl(TabControlCode, 4)).TabPages[2] == (TabPage)getGroupedControl(tabPageCode, 14))
                        {
                            // i.e. if the 3rd page is also the LXX lexicon
                            ((TabControl)getGroupedControl(TabControlCode, 4)).TabPages.Remove((TabPage)getGroupedControl(tabPageCode, 14));
                        }
                        // No action if it isn't there
                    }
                    else
                    {
                        if (((TabControl)getGroupedControl(TabControlCode, 4)).TabPages[1] == (TabPage)getGroupedControl(tabPageCode, 14))
                        {
                            // That means the current tab 2 is LXX lexicon rather than MT lexicon
                            ((TabControl)getGroupedControl(TabControlCode, 4)).TabPages.Remove((TabPage)getGroupedControl(tabPageCode, 14));
                            ((TabControl)getGroupedControl(TabControlCode, 4)).TabPages.Insert(1, (TabPage)getGroupedControl(tabPageCode, 13));
                        }
                    }
                    break;
                case 2:
                    if (((TabControl)getGroupedControl(TabControlCode, 4)).TabPages[1] == (TabPage)getGroupedControl(tabPageCode, 13))
                    {
                        // i.e. if the 2nd tab page is the MT lexicon
                        if (((TabControl)getGroupedControl(TabControlCode, 4)).TabPages[2] == (TabPage)getGroupedControl(tabPageCode, 14))
                        {
                            // i.e. if the 3rd page is also the LXX lexicon
                            ((TabControl)getGroupedControl(TabControlCode, 4)).TabPages.Remove((TabPage)getGroupedControl(tabPageCode, 13));
                        }
                        else
                        {
                            ((TabControl)getGroupedControl(TabControlCode, 4)).TabPages.Remove((TabPage)getGroupedControl(tabPageCode, 13));
                            ((TabControl)getGroupedControl(TabControlCode, 4)).TabPages.Insert(1, (TabPage)getGroupedControl(tabPageCode, 14));
                        }
                    }
                    else
                    {
                        if (((TabControl)getGroupedControl(TabControlCode, 4)).TabPages[1] != (TabPage)getGroupedControl(tabPageCode, 14))
                        {
                            // That means the current tab 2 is LXX lexicon rather than MT lexicon
                            ((TabControl)getGroupedControl(TabControlCode, 4)).TabPages.Insert(1, (TabPage)getGroupedControl(tabPageCode, 14));
                        }
                    }
                    break;
            }
        }

        int noOfCopyOptions = 0;
        SortedList<int, int> copyDestinationOptions = new SortedList<int, int>();
        SortedList<int, int> copyReferenceOptions = new SortedList<int, int>();
        SortedList<int, int> copyAccentsOptions = new SortedList<int, int>();
        SortedList<int, int> copyRememberOptions = new SortedList<int, int>();

        public int NoOfCopyOptions { get => noOfCopyOptions; set => noOfCopyOptions = value; }

        public void setCopyOptions(int optionCode, int copyTypeCode, int optionValue, int versionCode)
        {
            /*================================================================================================================================*
             *                                                                                                                                *
             *                                                        setCopyOptions                                                          *
             *                                                        --------------                                                          *
             *                                                                                                                                *
             *  When a user opts to copy a word, verse, etc. from the text, he has the option to elect for his latest options (e.g. whether   *
             *    to include accents or Bible references) so that the same options will be used automatically next time.  This method will    *
             *    manage the storage of those option.  The specific options are as follows                                                    *
             *                                                                                                  Possible values and meaning   *
             *          Dictionary                                Meaning                                             1             2         *
             *          ----------                                -------                                       ------------- ------------    *
             *     copyDestinationOptions   Where the copy will be directed                                     Clipboard     Relevent note   *
             *     copyReferenceOptions     Whether to include or exclude the Bible reference                   Include       Exclude         *
             *     copyAccentsOptions       Whether to include accents with the copy (or not)                   Include       Don't include   *
             *                                                                                                                                *
             *  If the user elects to remember any one of these options for future use an additional dictionary is populated:                 *
             *                                                                                                                                *
             *     copyRememberOptions      Indicates whether a specific copy is to be reused or not            Don't reuse   Reuse           *
             *                                                                                                                                *
             *  Each of the above dictionaries are constructed as:                                                                            *
             *     Key:   A copy option or type (specifically a word, verse, entire chapter or selection of text)                             *
             *     Value: The possible value (and meaning) _for that copy option_, as defined above                                           *
             *                                                                                                                                *
             *  Key values are as follows:                                                                                                    *
             *                                       MT                                 LXX                                                   *
             *                                                                                                                                *
             *                           Word  Verse  Chapter  Selection    Word  Verse  Chapter  Selection                                   *
             *                           ----  -----  -------  ---------    ----  -----  -------  ---------                                   *
             *                                                                                                                                *
             *   dictionary:              0      1       2         3         10     11      12       13                                       *
             *                                                                                                                                *
             *  Parameters:                                                                                                                   *
             *  ==========                                                                                                                    *
             *                                                                                                                                *
             *  optionCode     whether a word, verse, chapter or verse.  (These will be set to 0 to 3 for both MT and LXX and adjusted in the *
             *                   method itself.)                                                                                              *
             *  copyTypeCode   whether it is a value for destination (0), reference (1), accents (2) or for that option Code to be remembered *
             *                   (3)                                                                                                          *
             *  optionValue    1 or 2, asdefined in "Possible values and meaning", above                                                      *
             *  versionCode    if for MT, 0; if for LXX, 1                                                                                    *
             *                                                                                                                                *
             *================================================================================================================================*/

            int keyCode;

            keyCode = optionCode;
            if (versionCode == 1) keyCode += 10;
            switch (copyTypeCode)
            {
                case 0:
                    if (copyDestinationOptions.ContainsKey(keyCode)) copyDestinationOptions.Remove(keyCode);
                    copyDestinationOptions.Add(keyCode, optionValue);
                    break;
                case 1:
                    if (copyReferenceOptions.ContainsKey(keyCode)) copyReferenceOptions.Remove(keyCode);
                    copyReferenceOptions.Add(keyCode, optionValue);
                    break;
                case 2:
                    if (copyAccentsOptions.ContainsKey(keyCode)) copyAccentsOptions.Remove(keyCode);
                    copyAccentsOptions.Add(keyCode, optionValue);
                    break;
                case 3:
                    if (copyRememberOptions.ContainsKey(keyCode)) copyRememberOptions.Remove(keyCode);
                    copyRememberOptions.Add(keyCode, optionValue);
                    break;
            }
        }
    

        public Tuple< int, int, int, int> getCopyOption(int optionCode, int versionCode)
        {
            /*================================================================================================================================*
             *                                                                                                                                *
             *                                                        getCopyOptions                                                          *
             *                                                        --------------                                                          *
             *                                                                                                                                *
             *  See setCopyOptions for a detailed description of the workings of this and the previous method.                                *
             *  This method allows the user to submit an optionCode (e.g. 0 for word, 1 for verse, etc.)  and a value for whether it refers   *
             *    to the Hebrew/Aramaic text (versionCode = 0) or LXX (versionCode = 1) and the method will return a Tuple as follows:        *
             *                                                                                                                                *
             *                                                                     Possible values and meaning                                *
             *    items     significance                    value                        1             2                                      *
             *    -----     ------------                    -----                  ------------- ------------                                 *
             *      1     Destination                       1 or 2                 Clipboard     Relevent note                                *
             *      2     Reference                         1 or 2                 Include       Exclude                                      *
             *      3     Accents                           1 or 2                 Include       Don't include                                *
             *      4     Remember                          1 or 2                 Don't reuse   Reuse                                        *
             *                                                                                                                                *
             *  If no entry is found, a value of -1 is returned for that item.                                                                *
             *                                                                                                                                *
             *================================================================================================================================*/
            int keyCode, destVal, refVal, accVal, remVal ;

            keyCode = optionCode;
            if (versionCode == 1) keyCode += 10;
            if (copyDestinationOptions.ContainsKey(keyCode)) copyDestinationOptions.TryGetValue(keyCode, out destVal);
            else destVal = -1;
            if (copyReferenceOptions.ContainsKey(keyCode)) copyReferenceOptions.TryGetValue(keyCode, out refVal);
            else refVal = -1;
            if (copyAccentsOptions.ContainsKey(keyCode)) copyAccentsOptions.TryGetValue(keyCode, out accVal);
            else accVal = -1;
            if (copyRememberOptions.ContainsKey(keyCode)) copyRememberOptions.TryGetValue(keyCode, out remVal);
            else remVal = -1;
            return new Tuple<int, int, int, int>(destVal, refVal, accVal, remVal);
        }

        public Font configureFont(String fontName, String styleName, float fontSize)
        {
            FontStyle selectedStyle = FontStyle.Regular;

            switch (styleName)
            {
                case "Regular": selectedStyle = FontStyle.Regular; break;
                case "Bold": selectedStyle = FontStyle.Bold; break;
                case "Italic": selectedStyle = FontStyle.Italic; break;
                case "Underline": selectedStyle = FontStyle.Underline; break;
                case "Strikeout": selectedStyle = FontStyle.Strikeout; break;
            }
            return new Font(fontName, fontSize, selectedStyle);
        }
    }
}
