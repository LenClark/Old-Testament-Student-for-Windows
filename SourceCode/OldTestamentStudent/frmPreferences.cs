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
    public partial class frmPreferences : Form
    {
        const char zeroWidthSpace = '\u200b', zeroWidthNonJoiner = '\u200d';

        int noOfAreas, controlTops = 10;
        bool[] hasChanged;
        ComboBox[,] cbEngFontSizes, cbAltFontSizes, cbPrimaryFontSizes, cbSecondaryFontSizes;
        TextBox txtPrimary, txtSecondary;
        TextBox[] txtText, txtBg;
        RichTextBox[] rtxtExample;
        RichTextBox[,] rtxtReference, rtxtVerse;
        FlowLayoutPanel[] pnlSearch;
        TabPage[] tabCollection;
        TabControl[] tabCtlSubMaster;
        TabPage[,] tabSubGroups;
        PictureBox[,] colourBoxes;
        Font largerFont, smallerFont;
        Button[] btnReset;
        int[] noOfWordsInExample;
        int[,] textFont = { { 12, 18, 18, 12 }, { 12, 18, 18, 12 }, { 12, 18, 18, 12 }, { 12, 18, 18, 12 }, { 12, 18, 18, 18 }, { 12, 12, 12, 12 }, 
                            { 12, 18, 18, 12 }, { 12, 18, 18, 12 } };
//        int[] controlTops = { 10, 42, 74, 106 };
        String[] preferenceAreas = { "Masoretic Hebrew and Aramaic", "Septuagint", "Grammatical Breakdown", "Lexical Area", "Search Results Pane for MT",
                                     "Search Results Pane for LXX", "Notes on the Masoretic Text", "Notes on the Septuagint" };
        SortedList<int, classPreferencesExamples>[] exampleText = new SortedList<int, classPreferencesExamples>[7];


        String[] mtParse = { "׳דְּבָרַי", "", "noun: masculine plural absolute" };
        String[] lxxParse = { "xδέδωκα", "", "1st person Singular Perfect Active Indicative  - root: δίδωμι" };


        String[,] subTabHeaders = { { "Verse Numbers", "Hebrew/Aramaic Text", "Kethib/Qere", ""}, { "Verse Numbers", "Greek Text", "", "" },
                                    { "Main Text", "Heading Text", "", "" }, { "Main Text", "Heading Text", "", "" },
                                    { "Text for Refrerences", "Main Text", "The Primary word sought", "The Secondary word sought" },
                                    { "Text for Refrerences", "Main Text", "The Primary word sought", "The Secondary word sought" },
                                    { "Text", "", "", ""}, { "Text", "", "", ""} };


        /*-----------------------------------------------------------------------------------------------*
         *                                                                                               *
         *                                         Text sizes                                            *
         *                                         ----------                                            *
         *                                                                                               *
         *-----------------------------------------------------------------------------------------------*/
        SortedList<int, float> englishTextSize = new SortedList<int, float>();
        SortedList<int, float> hebrewTextSize = new SortedList<int, float>();
        SortedList<int, float> primaryTextSize = new SortedList<int, float>();
        SortedList<int, float> secondaryTextSize = new SortedList<int, float>();

        /*-----------------------------------------------------------------------------------------------*
         *                                                                                               *
         *                                         Text style                                            *
         *                                         ----------                                            *
         *                                                                                               *
         *-----------------------------------------------------------------------------------------------*/
        SortedList<int, String> englishMainStyle = new SortedList<int, String>();
        SortedList<int, String> foreignMainStyle = new SortedList<int, String>();
        SortedList<int, String> primaryAltStyle = new SortedList<int, String>();
        SortedList<int, String> secondaryAltStyle = new SortedList<int, String>();

        /*-----------------------------------------------------------------------------------------------*
         *                                                                                               *
         *                                         Font name                                             *
         *                                         ---------                                             *
         *                                                                                               *
         *-----------------------------------------------------------------------------------------------*/
        SortedList<int, String> englishFontName = new SortedList<int, String>();
        SortedList<int, String> foreignFontName = new SortedList<int, String>();
        SortedList<int, String> primaryFontName = new SortedList<int, String>();
        SortedList<int, String> secondaryFontName = new SortedList<int, String>();

        /*-----------------------------------------------------------------------------------------------*
         *                                                                                               *
         *                                     Colour collections                                        *
         *                                     ------------------                                        *
         *                                                                                               *
         *-----------------------------------------------------------------------------------------------*/
        SortedList<int, Color> backColourRec = new SortedList<int, Color>();
        SortedList<int, Color> foreMainColourRec = new SortedList<int, Color>();
        SortedList<int, Color> foreAltColour = new SortedList<int, Color>();
        SortedList<int, Color> forePrimeColour = new SortedList<int, Color>();
        SortedList<int, Color> foreSecondColour = new SortedList<int, Color>();

        classGlobal globalVars;
        classRegistry appRegistry;

        public int NoOfAreas { get => noOfAreas; set => noOfAreas = value; }
        public bool[] HasChanged { get => hasChanged; set => hasChanged = value; }

        public  frmPreferences(classGlobal inConfig, classRegistry inRegistry)
        {

            InitializeComponent();

            /*-------------------------------------------------------------------------*
             *  Global Classes used                                                    *
             *-------------------------------------------------------------------------*/

            globalVars = inConfig;
            appRegistry = inRegistry;

        }

        public void initialiseForm()
        {
            /*=====================================================================================================================*
             *                                                                                                                     *
             *                                     frmPreferences - Initialisation                                                 *
             *                                     ===============================                                                 *
             *                                                                                                                     *
             *  The Preferences dialog is created programmatically and is quite fiddly.  The basic architecture is:                *
             *                                                                                                                     *
             *                                             frmPreferences                                                          *
             *                                                   |                                                                 *
             *                                                   v                                                                 *
             *                                            tabCtlPreferences (created in the Designer)                              *
             *                                                   |                                                                 *
             *                                                   v                                                                 *
             *                                              tabCollection (one per area, defined by preferenceAreas)               *
             *                                                   |                                                                 *
             *                                                   v                                                                 *
             *                                            tabCtlSubMaster (tab controls added to each tabCollection)               *
             *                                                   |                                                                 *
             *                                                   v                                                                 *
             *                                              tabSubGroups (pages added to each tabCtlSubMaster)                     *
             *                                                                                                                     *
             *                                                                                                                     *
             *                                                                                                                     *
             *                                                                                                                     *
             *=====================================================================================================================*/

            int idx;
            String[] fontSizes;
            String[] fontStyles;
            Color tempColour = Color.Empty;
            Label fontInfo, textInfo, bgroundInfo;

            /*-------------------------------------------------------------------------*
             *  Get current values from classGlobal                                    *
             *-------------------------------------------------------------------------*/

            populateSourceData();

            /*-------------------------------------------------------------------------*
             *  Set up the actual Dialog                                               *
             *-------------------------------------------------------------------------*/

            noOfAreas = preferenceAreas.Length;
            fontSizes = new string[] { "6", "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "32", "36", "48", "60", "72" };
            fontStyles = new string[] { "Regular", "Bold", "Italic", "Underline", "Strikeout" };
            largerFont = new Font(FontFamily.GenericSansSerif, 12F, FontStyle.Regular);
            smallerFont = new Font(FontFamily.GenericSansSerif, 10F, FontStyle.Regular);
            tabCollection = new TabPage[noOfAreas];
            tabCtlSubMaster = new TabControl[noOfAreas];
            tabSubGroups = new TabPage[noOfAreas, 4];
            rtxtExample = new RichTextBox[noOfAreas];
            rtxtReference = new RichTextBox[2, 25];
            rtxtVerse = new RichTextBox[2, 25];
            pnlSearch = new FlowLayoutPanel[2];
            fontInfo = new Label();
            textInfo = new Label();
            bgroundInfo = new Label();
            cbEngFontSizes = new ComboBox[noOfAreas, 4];
            cbAltFontSizes = new ComboBox[noOfAreas, 4];
            cbPrimaryFontSizes = new ComboBox[noOfAreas, 4];
            cbSecondaryFontSizes = new ComboBox[noOfAreas, 4];
            colourBoxes = new PictureBox[noOfAreas, 5];
            txtText = new TextBox[noOfAreas];
            txtBg = new TextBox[noOfAreas];
            hasChanged = new bool[noOfAreas];
            btnReset = new Button[noOfAreas];

            populateExampleLists();
            buildDialog();
            for (idx = 0; idx < noOfAreas; idx++)
            {
                displayExampleText(idx);
            }
        }

        private void buildDialog()
        {
            int idx, jdx, textLength, newLeft;
            Color tempColour = Color.Empty;
            Label textInfo, exampleLbl, resetLbl;
            PictureBox pboxTemp;
            Button btnFont, btnTemp;

            for (idx = 0; idx < noOfAreas; idx++)
            {
                /*----------------------------------------------------------------------------------------*
                 *  An area-based flag indicating whether one or more elements for that area has/have     *
                 *    changed.                                                                            *
                 *----------------------------------------------------------------------------------------*/
                hasChanged[idx] = false;

                /*----------------------------------------------------------------------------------------*
                 *  Create the main tabs (tabCollection - one for each area)                              *
                 *----------------------------------------------------------------------------------------*/
                tabCollection[idx] = createNewTabPage(preferenceAreas[idx]);
                tabCollection[idx].Height = tabCtlPreferences.ClientRectangle.Height;
                tabCollection[idx].Width = tabCtlPreferences.ClientSize.Width - 4;
                tabCtlPreferences.Controls.Add(tabCollection[idx]);

                /*----------------------------------------------------------------------------------------*
                 *  Create the TabControl for the area tab that will house the sub-tabs                   *
                 *----------------------------------------------------------------------------------------*/
                tabCtlSubMaster[idx] = new TabControl();
                tabCtlSubMaster[idx].Height = TextRenderer.MeasureText("Masoretic", tabCollection[idx].Font).Height + controlTops + 50;
                tabCtlSubMaster[idx].Width = tabCollection[idx].ClientSize.Width - 4;
                tabCollection[idx].Controls.Add(tabCtlSubMaster[idx]);

                /*----------------------------------------------------------------------------------------*
                 *  Create the sub-tabs.                                                                  *
                 *    Each area will have up to four sub-tabs.  The exact number is determined by the     *
                 *    array of sub-tab titles, subTabHeaders.                                             *
                 *----------------------------------------------------------------------------------------*/
                for (jdx = 0; jdx < 4; jdx++)
                {
                    if (subTabHeaders[idx, jdx].Length == 0) continue;
                    tabSubGroups[idx, jdx] = createNewTabPage(subTabHeaders[idx, jdx]);
                    tabSubGroups[idx, jdx].Width = tabCollection[idx].ClientSize.Width - 8;
                    tabSubGroups[idx, jdx].Height = tabCtlSubMaster[idx].ClientSize.Height;
                    tabCtlSubMaster[idx].Controls.Add(tabSubGroups[idx, jdx]);

                    /*----------------------------------------------------------------------------------------*
                     *  We now add the various Controls, starting with the colour label.                      *
                     *----------------------------------------------------------------------------------------*/
                    if (tabSubGroups[idx, jdx] == null) continue;
                    textInfo = new Label();
                    textInfo.Text = "Text Colour:";
                    textInfo.Left = 16;
                    textInfo.Top = controlTops + 3;
                    textInfo.Font = largerFont;
                    textInfo.AutoSize = true;
                    tabSubGroups[idx, jdx].Controls.Add(textInfo);
                    textLength = TextRenderer.MeasureText("Text Colour:", largerFont).Width;
                    newLeft = textLength + 21;  // textInfo.Left plus a small spacing value
                    /*----------------------------------------------------------------------------------------*
                     *  Now create the picture box for that text's colour.                                    *
                     *----------------------------------------------------------------------------------------*/
                    pboxTemp = new PictureBox();
                    pboxTemp.Top = controlTops;
                    pboxTemp.Left = newLeft;
                    pboxTemp.Size = new Size(75, 28);
                    switch (jdx)
                    {
                        case 0:
                            foreMainColourRec.TryGetValue(idx, out tempColour);
                            pboxTemp.BackColor = tempColour;
                            break;
                        case 1:
                            foreAltColour.TryGetValue(idx, out tempColour);
                            pboxTemp.BackColor = tempColour;
                            break;
                        case 2:
                            forePrimeColour.TryGetValue(idx, out tempColour);
                            pboxTemp.BackColor = tempColour;
                            break;
                        case 3:
                            foreSecondColour.TryGetValue(idx, out tempColour);
                            pboxTemp.BackColor = tempColour;
                            break;

                    }
                    pboxTemp.Tag = idx * 10 + jdx + 1;  // A distinctive tag value that allows us to decode it when clicked
                    pboxTemp.MouseClick += pictureBoxMouseClick;
                    tabSubGroups[idx, jdx].Controls.Add(pboxTemp);
                    colourBoxes[idx, jdx + 1] = pboxTemp;

                    /*----------------------------------------------------------------------------------------*
                     *  Now add the label for the background colour, if we are on the zeroth sub-tab.         *
                     *----------------------------------------------------------------------------------------*/
                    newLeft += 90;
                    if (jdx == 0)
                    {
                        textInfo = new Label();
                        textInfo.Text = "Background Colour:";
                        textInfo.Left = newLeft;
                        textInfo.Top = controlTops + 3;
                        textInfo.Font = largerFont;
                        textInfo.AutoSize = true;
                        tabSubGroups[idx, jdx].Controls.Add(textInfo);
                        newLeft += TextRenderer.MeasureText("Background Colour:", largerFont).Width + 5;

                        /*----------------------------------------------------------------------------------------*
                         *  Add the associated picture box for the background colour.                             *
                         *----------------------------------------------------------------------------------------*/
                        pboxTemp = new PictureBox();
                        pboxTemp.Top = controlTops;
                        pboxTemp.Left = newLeft;
                        pboxTemp.Size = new Size(75, 28);
                        backColourRec.TryGetValue(idx, out tempColour);
                        pboxTemp.BackColor = tempColour;
                        pboxTemp.Tag = idx * 10;
                        pboxTemp.MouseClick += pictureBoxMouseClick;
                        tabSubGroups[idx, jdx].Controls.Add(pboxTemp);
                        colourBoxes[idx, 0] = pboxTemp;
                        newLeft += 90;
                    }

                    /*----------------------------------------------------------------------------------------*
                     *  Now add the label and button for modifying the relevant font.                         *
                     *----------------------------------------------------------------------------------------*/

                    textInfo = new Label();
                    textInfo.Text = "Font details:";
                    textInfo.Left = newLeft;
                    textInfo.Top = controlTops + 3;
                    textInfo.Font = smallerFont;
                    textInfo.AutoSize = true;
                    tabSubGroups[idx, jdx].Controls.Add(textInfo);
                    newLeft += TextRenderer.MeasureText("Font details:", smallerFont).Width + 5;

                    btnFont = new Button();
                    btnFont.Left = newLeft;
                    btnFont.Top = controlTops;
                    btnFont.Font = smallerFont;
                    btnFont.Height = 28;
                    btnFont.Text = "Modify";
                    btnFont.Tag = idx * 10 + jdx;
                    btnFont.Click += BtnFont_Click;
                    tabSubGroups[idx, jdx].Controls.Add(btnFont);
                }

                /*----------------------------------------------------------------------------------------*
                 *  We are now targeting the area _below_ the sub-tabs, common to the main tab.           *
                 *  First, the label for the RichTextBox or set of RichTextBoxes, below                   *
                 *----------------------------------------------------------------------------------------*/
                exampleLbl = new Label();
                exampleLbl.Top = controlTops + tabCtlSubMaster[idx].Height + 8;
                exampleLbl.Left = 10;
                exampleLbl.Text = "Example text:";
                exampleLbl.Font = largerFont;
                exampleLbl.AutoSize = true;
                tabCollection[idx].Controls.Add(exampleLbl);

                /*----------------------------------------------------------------------------------------*
                 *  Further over, the label explaining the reset button.                                  *
                 *----------------------------------------------------------------------------------------*/
                textLength = TextRenderer.MeasureText("Example text:", largerFont).Width;
                newLeft = textLength + 225;  // Space the new text well away from the richtextbox label
                resetLbl = new Label();
                resetLbl.Top = controlTops + tabCtlSubMaster[idx].Height + 8;
                resetLbl.Left = newLeft;
                resetLbl.Text = "Restore all colours and font metrics:";
                resetLbl.Font = largerFont;
                resetLbl.AutoSize = true;
                tabCollection[idx].Controls.Add(resetLbl);

                /*----------------------------------------------------------------------------------------*
                 *  And the reset button itself.                                                          *
                 *----------------------------------------------------------------------------------------*/
                newLeft += TextRenderer.MeasureText("Restore all colours and font metrics:", largerFont).Width + 5;
                btnTemp = new Button();
                btnTemp.Left = newLeft;
                btnTemp.Top = controlTops + tabCtlSubMaster[idx].Height + 3;
                btnTemp.Font = largerFont;
                btnTemp.Height = 28;
                btnTemp.Text = "...";
                btnTemp.Enabled = false;
                btnTemp.Tag = idx;
                btnTemp.Click += resetButton;
                tabCollection[idx].Controls.Add(btnTemp);
                btnReset[idx] = btnTemp;

                if ((idx == 4) || (idx == 5))
                {
                    /*----------------------------------------------------------------------------------------*
                     *  For areas 4 and 5, we add a Flow Layout Panel                                         *
                     *----------------------------------------------------------------------------------------*/
                    pnlSearch[idx - 4] = new FlowLayoutPanel();
                    pnlSearch[idx - 4].Top = exampleLbl.Top + TextRenderer.MeasureText("Example text:", largerFont).Height + 16;
                    pnlSearch[idx - 4].Left = 16;
                    pnlSearch[idx - 4].Width = tabCtlPreferences.ClientRectangle.Width - 32;
                    pnlSearch[idx - 4].Height = tabCollection[idx].ClientRectangle.Height - (controlTops + tabCtlSubMaster[idx].Height + 16 + exampleLbl.Height);
                    pnlSearch[idx - 4].AutoScroll = true;
                    tabCollection[idx].Controls.Add(pnlSearch[idx - 4]);
                }
                else
                {
                    /*----------------------------------------------------------------------------------------*
                     *  For all other areas, a simple RichTextBox.                                            *
                     *----------------------------------------------------------------------------------------*/
                    rtxtExample[idx] = new RichTextBox();
                    rtxtExample[idx].Top = exampleLbl.Top + TextRenderer.MeasureText("Example text:", largerFont).Height + 16;
                    rtxtExample[idx].Left = 16;
                    rtxtExample[idx].Width = tabCtlPreferences.ClientRectangle.Width - 32;
                    rtxtExample[idx].Height = tabCtlPreferences.ClientRectangle.Height - (controlTops + 40 + exampleLbl.Height);
                    tabCollection[idx].Controls.Add(rtxtExample[idx]);
                }
            }
        }

        private void populateSourceData()
        {
            /*--------------------------------------------------------------------------------------------------*
             *                                                                                                  *
             *  Get current values from classGlobal                                                             *
             *                                                                                                  *
             *  In all that follows, the indexes representing different application areas function as follows:  *
             *                                                                                                  *
             *                   Value              Refers to                                                   *
             *                   -----              ---------                                                   *
             *                     0            Main MT Text area                                               *
             *                     1            Main LXX Text area                                              *
             *                     2            Parse area (for both MT and LXX)                                *
             *                     3            Lexical Area (for LXX)                                          *
             *                     4            Search Results Area when using MT                               *
             *                     5            Search Results Area when using LXX                              *
             *                     6            Notes Area for MT                                               *
             *                     7            Motes area for LXX                                              *
             *                                                                                                  *
             *--------------------------------------------------------------------------------------------------*/

                englishTextSize.Clear();
            hebrewTextSize.Clear();
            primaryTextSize.Clear();
            secondaryTextSize.Clear();
            englishTextSize.Add(0, globalVars.getTextSize(0, 1));
            englishTextSize.Add(1, globalVars.getTextSize(1, 1));
            englishTextSize.Add(2, globalVars.getTextSize(2, 1));
            englishTextSize.Add(3, globalVars.getTextSize(3, 1));
            englishTextSize.Add(4, globalVars.getTextSize(4, 1));
            englishTextSize.Add(5, globalVars.getTextSize(5, 1));
            englishTextSize.Add(6, globalVars.getTextSize(6, 1));
            englishTextSize.Add(7, globalVars.getTextSize(7, 1));
            hebrewTextSize.Add(0, globalVars.getTextSize(0, 2));
            hebrewTextSize.Add(1, globalVars.getTextSize(1, 2));
            hebrewTextSize.Add(2, globalVars.getTextSize(2, 2));
            hebrewTextSize.Add(3, globalVars.getTextSize(3, 2));
            hebrewTextSize.Add(4, globalVars.getTextSize(4, 2));
            hebrewTextSize.Add(5, globalVars.getTextSize(5, 2));
            primaryTextSize.Add(0, globalVars.getTextSize(0, 3));
            primaryTextSize.Add(4, globalVars.getTextSize(4, 3));
            primaryTextSize.Add(5, globalVars.getTextSize(5, 3));
            secondaryTextSize.Add(4, globalVars.getTextSize(4, 4));
            secondaryTextSize.Add(5, globalVars.getTextSize(5, 4));

            englishMainStyle.Clear();
            foreignMainStyle.Clear();
            primaryAltStyle.Clear();
            secondaryAltStyle.Clear();
            englishMainStyle.Add(0, globalVars.getDefinedStyleByIndex(0, 1));
            englishMainStyle.Add(1, globalVars.getDefinedStyleByIndex(1, 1));
            englishMainStyle.Add(2, globalVars.getDefinedStyleByIndex(2, 1));
            englishMainStyle.Add(3, globalVars.getDefinedStyleByIndex(3, 1));
            englishMainStyle.Add(4, globalVars.getDefinedStyleByIndex(4, 1));
            englishMainStyle.Add(5, globalVars.getDefinedStyleByIndex(5, 1));
            englishMainStyle.Add(6, globalVars.getDefinedStyleByIndex(6, 1));
            englishMainStyle.Add(7, globalVars.getDefinedStyleByIndex(7, 1));
            foreignMainStyle.Add(0, globalVars.getDefinedStyleByIndex(0, 2));
            foreignMainStyle.Add(1, globalVars.getDefinedStyleByIndex(1, 2));
            foreignMainStyle.Add(2, globalVars.getDefinedStyleByIndex(2, 2));
            foreignMainStyle.Add(3, globalVars.getDefinedStyleByIndex(3, 2));
            foreignMainStyle.Add(4, globalVars.getDefinedStyleByIndex(4, 2));
            foreignMainStyle.Add(5, globalVars.getDefinedStyleByIndex(5, 2));
            primaryAltStyle.Add(0, globalVars.getDefinedStyleByIndex(0, 3));
            primaryAltStyle.Add(4, globalVars.getDefinedStyleByIndex(4, 3));
            primaryAltStyle.Add(5, globalVars.getDefinedStyleByIndex(5, 3));
            secondaryAltStyle.Add(4, globalVars.getDefinedStyleByIndex(4, 4));
            secondaryAltStyle.Add(5, globalVars.getDefinedStyleByIndex(5, 4));

            englishFontName.Clear();
            foreignFontName.Clear();
            primaryFontName.Clear();
            secondaryFontName.Clear();
            englishFontName.Add(0, globalVars.getDefinedFontNameByIndex(0, 1));
            englishFontName.Add(1, globalVars.getDefinedFontNameByIndex(1, 1));
            englishFontName.Add(2, globalVars.getDefinedFontNameByIndex(2, 1));
            englishFontName.Add(3, globalVars.getDefinedFontNameByIndex(3, 1));
            englishFontName.Add(4, globalVars.getDefinedFontNameByIndex(4, 1));
            englishFontName.Add(5, globalVars.getDefinedFontNameByIndex(5, 1));
            englishFontName.Add(6, globalVars.getDefinedFontNameByIndex(6, 1));
            englishFontName.Add(7, globalVars.getDefinedFontNameByIndex(7, 1));
            foreignFontName.Add(0, globalVars.getDefinedFontNameByIndex(0, 2));
            foreignFontName.Add(1, globalVars.getDefinedFontNameByIndex(1, 2));
            foreignFontName.Add(2, globalVars.getDefinedFontNameByIndex(2, 2));
            foreignFontName.Add(3, globalVars.getDefinedFontNameByIndex(3, 2));
            foreignFontName.Add(4, globalVars.getDefinedFontNameByIndex(4, 2));
            foreignFontName.Add(5, globalVars.getDefinedFontNameByIndex(5, 2));
            primaryFontName.Add(0, globalVars.getDefinedFontNameByIndex(0, 3));
            primaryFontName.Add(4, globalVars.getDefinedFontNameByIndex(4, 3));
            primaryFontName.Add(5, globalVars.getDefinedFontNameByIndex(5, 3));
            secondaryFontName.Add(4, globalVars.getDefinedFontNameByIndex(4, 4));
            secondaryFontName.Add(5, globalVars.getDefinedFontNameByIndex(5, 4));

            backColourRec.Clear();
            foreMainColourRec.Clear();
            foreAltColour.Clear();
            forePrimeColour.Clear();
            foreSecondColour.Clear();
            backColourRec.Add(0, globalVars.getColourSetting(0, 0));
            backColourRec.Add(1, globalVars.getColourSetting(1, 0));
            backColourRec.Add(2, globalVars.getColourSetting(2, 0));
            backColourRec.Add(3, globalVars.getColourSetting(3, 0));
            backColourRec.Add(4, globalVars.getColourSetting(4, 0));
            backColourRec.Add(5, globalVars.getColourSetting(5, 0));
            backColourRec.Add(6, globalVars.getColourSetting(6, 0));
            backColourRec.Add(7, globalVars.getColourSetting(7, 0));
            foreMainColourRec.Add(0, globalVars.getColourSetting(0, 1));
            foreMainColourRec.Add(1, globalVars.getColourSetting(1, 1));
            foreMainColourRec.Add(2, globalVars.getColourSetting(2, 1));
            foreMainColourRec.Add(3, globalVars.getColourSetting(3, 1));
            foreMainColourRec.Add(4, globalVars.getColourSetting(4, 1));
            foreMainColourRec.Add(5, globalVars.getColourSetting(5, 1));
            foreMainColourRec.Add(6, globalVars.getColourSetting(6, 1));
            foreMainColourRec.Add(7, globalVars.getColourSetting(7, 1));
            foreAltColour.Add(0, globalVars.getColourSetting(0, 2));
            foreAltColour.Add(1, globalVars.getColourSetting(1, 2));
            foreAltColour.Add(2, globalVars.getColourSetting(2, 2));
            foreAltColour.Add(3, globalVars.getColourSetting(3, 2));
            foreAltColour.Add(4, globalVars.getColourSetting(4, 2));
            foreAltColour.Add(5, globalVars.getColourSetting(5, 2));
            forePrimeColour.Add(0, globalVars.getColourSetting(0, 3));
            forePrimeColour.Add(4, globalVars.getColourSetting(4, 3));
            forePrimeColour.Add(5, globalVars.getColourSetting(5, 3));
            foreSecondColour.Add(4, globalVars.getColourSetting(4, 4));
            foreSecondColour.Add(5, globalVars.getColourSetting(5, 4));
        }

        private void populateExampleLists()
        {
            int noOfItems, noOfLines, itemIndex, lineIndex, itemCount, nPstn;
            Char[] textSplitter = { ' ' }, initialSplitter = { ':' };
            String[] individualItem, splitWord;
            String[] mtText = { "19: שִׁמְעִ֣י הָאָ֔רֶץ הִנֵּ֨ה אָנֹכִ֜י מֵבִ֥יא רָעָ֛ה אֶל־הָעָ֥ם הַזֶּ֖ה פְּרִ֣י מַחְשְׁבֹותָ֑ם כִּ֤י עַל־דְּבָרַי֙ לֹ֣א הִקְשִׁ֔יבוּ וְתֹורָתִ֖י וַיִּמְאֲסוּ־בָֽהּ׃",
                            "20: לָמָּה־זֶּ֨ה לִ֤י לְבֹונָה֙ מִשְּׁבָ֣א תָבֹ֔וא וְקָנֶ֥ה הַטֹּ֖וב מֵאֶ֣רֶץ מֶרְחָ֑ק עֹלֹֽותֵיכֶם֙ לֹ֣א לְרָצֹ֔ון וְזִבְחֵיכֶ֖ם לֹא־עָ֥רְבוּ לִֽי׃ ס",
                            "21: לָכֵ֗ן כֹּ֚ה אָמַ֣ר יְהוָ֔ה הִנְנִ֥י נֹתֵ֛ן אֶל־הָעָ֥ם הַזֶּ֖ה מִכְשֹׁלִ֑ים וְכָ֣שְׁלוּ בָ֠ם אָבֹ֨ות וּבָנִ֥ים יַחְדָּ֛ו שָׁכֵ֥ן וְרֵעֹ֖ו ׳וְאָבָֽדוּ׃",
                            "22: כֹּ֚ה אָמַ֣ר יְהוָ֔ה הִנֵּ֛ה עַ֥ם בָּ֖א מֵאֶ֣רֶץ צָפֹ֑ון וְגֹ֣וי גָּדֹ֔ול יֵעֹ֖ור מִיַּרְכְּתֵי־אָֽרֶץ׃",
                            "23: קֶ֣שֶׁת וְכִידֹ֞ון יַחֲזִ֗יקוּ אַכְזָרִ֥י הוּא֙ וְלֹ֣א יְרַחֵ֔מוּ קֹולָם֙ כַּיָּ֣ם יֶהֱמֶ֔ה וְעַל־סוּסִ֖ים יִרְכָּ֑בוּ עָר֗וּךְ כְּאִישׁ֙ לַמִּלְחָמָ֔ה עָלַ֖יִךְ בַּת־צִיֹּֽון׃",
                            "24: שָׁמַ֥עְנוּ אֶת־שָׁמְעֹ֖ו רָפ֣וּ יָדֵ֑ינוּ צָרָה֙ הֶחֱזִיקַ֔תְנוּ חִ֖יל כַּיֹּולֵדָֽה׃",
                            "25: אַל־׳תֵּֽצְאוּ֙ הַשָּׂדֶ֔ה וּבַדֶּ֖רֶךְ אַל־׳תֵּלֵ֑כוּ כִּ֚י חֶ֣רֶב לְאֹיֵ֔ב מָגֹ֖ור מִסָּבִֽיב׃",
                            "26: בַּת־עַמִּ֤י חִגְרִי־שָׂק֙ וְהִתְפַּלְּשִׁ֣י בָאֵ֔פֶר אֵ֤בֶל יָחִיד֙ עֲשִׂ֣י לָ֔ךְ מִסְפַּ֖ד תַּמְרוּרִ֑ים כִּ֣י פִתְאֹ֔ם יָבֹ֥א הַשֹּׁדֵ֖ד עָלֵֽינוּ׃",
                            "27: בָּחֹ֛ון נְתַתִּ֥יךָ בְעַמִּ֖י מִבְצָ֑ר וְתֵדַ֕ע וּבָחַנְתָּ֖ אֶת־דַּרְכָּֽם׃",
                            "28: כֻּלָּם֙ סָרֵ֣י סֹֽורְרִ֔ים הֹלְכֵ֥י רָכִ֖יל נְחֹ֣שֶׁת וּבַרְזֶ֑ל כֻּלָּ֥ם מַשְׁחִיתִ֖ים הֵֽמָּה׃",
                            "29: נָחַ֣ר מַפֻּ֔חַ ׳מֵאֵ֖שׁ ׳תַּ֣ם עֹפָ֑רֶת לַשָּׁוְא֙ צָרַ֣ף צָרֹ֔וף וְרָעִ֖ים לֹ֥א נִתָּֽקוּ׃",
                            "30: כֶּ֣סֶף נִמְאָ֔ס קָרְא֖וּ לָהֶ֑ם כִּֽי־מָאַ֥ס יְהוָ֖ה בָּהֶֽם׃ פ" };

            String[] lxxText = { "1:  Καὶ ἦλθον οἱ υἱοὶ Ισραηλ, πᾶσα ἡ συναγωγή, εἰς τὴν ἔρημον Σιν ἐν τῷ μηνὶ τῷ πρώτῳ, καὶ κατέμεινεν ὁ λαὸς ἐν Καδης, καὶ ἐτελεύτησεν ἐκεῖ Μαριαμ καὶ ἐτάφη ἐκεῖ.",
                             "2:  καὶ οὐκ ἦν ὕδωρ τῇ συναγωγῇ, καὶ ἠθροίσθησαν ἐπὶ Μωυσῆν καὶ Ααρων.",
                             "3:  καὶ ἐλοιδορεῖτο ὁ λαὸς πρὸς Μωυσῆν λέγοντες Ὄφελον ἀπεθάνομεν ἐν τῇ ἀπωλείᾳ τῶν ἀδελφῶν ἡμῶν ἔναντι κυρίου·",
                             "4:  καὶ ἵνα τί ἀνηγάγετε τὴν συναγωγὴν κυρίου εἰς τὴν ἔρημον ταύτην ἀποκτεῖναι ἡμᾶς καὶ τὰ κτήνη ἡμῶν;",
                             "5:  καὶ ἵνα τί τοῦτο ἀνηγάγετε ἡμᾶς ἐξ Αἰγύπτου παραγενέσθαι εἰς τὸν τόπον τὸν πονηρὸν τοῦτον; τόπος, οὗ οὐ σπείρεται οὐδὲ συκαῖ οὐδὲ ἄμπελοι οὐδὲ ῥόαι οὐδὲ ὕδωρ ἐστὶν πιεῖν.",
                             "6:  καὶ ἦλθεν Μωϋσῆς καὶ Ααρων ἀπὸ προσώπου τῆς συναγωγῆς ἐπὶ τὴν θύραν τῆς σκηνῆς τοῦ μαρτυρίου καὶ ἔπεσαν ἐπὶ πρόσωπον, καὶ ὤφθη ἡ δόξα κυρίου πρὸς αὐτούς.",
                             "7:  καὶ ἐλάλησεν κύριος πρὸς Μωυσῆν λέγων",
                             "8:  Λαβὲ τὴν ῥάβδον καὶ ἐκκλησίασον τὴν συναγωγὴν σὺ καὶ Ααρων ὁ ἀδελφός σου καὶ λαλήσατε πρὸς τὴν πέτραν ἔναντι αὐτῶν, καὶ δώσει τὰ ὕδατα αὐτῆς, καὶ ἐξοίσετε αὐτοῖς ὕδωρ ἐκ τῆς πέτρας καὶ ποτιεῖτε τὴν συναγωγὴν καὶ τὰ κτήνη αὐτῶν.",
                             "9:  καὶ ἔλαβεν Μωϋσῆς τὴν ῥάβδον τὴν ἀπέναντι κυρίου, καθὰ συνέταξεν κύριος·",
                             "10:  καὶ ἐξεκκλησίασεν Μωϋσῆς καὶ Ααρων τὴν συναγωγὴν ἀπέναντι τῆς πέτρας καὶ εἶπεν πρὸς αὐτούς Ἀκούσατέ μου, οἱ ἀπειθεῖς· μὴ ἐκ τῆς πέτρας ταύτης ἐξάξομεν ὑμῖν ὕδωρ;",
                             "11:  καὶ ἐπάρας Μωϋσῆς τὴν χεῖρα αὐτοῦ ἐπάταξεν τὴν πέτραν τῇ ῥάβδῳ δίς, καὶ ἐξῆλθεν ὕδωρ πολύ, καὶ ἔπιεν ἡ συναγωγὴ καὶ τὰ κτήνη αὐτῶν." };

            String[] mtParse = { "׳דְּבָרַי", "noun: masculine plural absolute" };

            String[] lxxLex = { "δίδωμι\n\n",
                "Etymology: Redupl. from Root  δΟ , Lat.  do, dare.\n\n",
                "        I Orig. sense, to give,  τί τινι Hom., etc.; in pres.and imperf.to be ready to give, to offer, id= Hom.\n",
                "			2 of the gods, to grant, κῠδος, νίκην, and of evils, δ.ἄλγεα, ἄτας, κήδεα id = Hom.; later,  εὖ διδόναι τινί to provide well for . . , Soph., Eur.",
                "			3 to offer to the gods, Hom., etc.\n",
                "			4 with an inf.added,  δῶκε τεύχεα θεράποντι φορῆναι gave him the arms to carry, Il.; διδοῐ πιεῐν gives to drink, Hdt., etc.\n",
                "			5 Prose phrases, δ. ὅρκον, opp.to λαμβάνειν, to tender an oath;  δ.χάριν,  χαρίζεσθαι, as  ὀργῆι χάριν δούς having indulged his anger, Soph.;" +
                "   λόγον τινὶ δ.to give one leave to speak, Xen.; but,  δ.λόγον ἑαυτῶι to deliberate, Hdt.\n\n",
                "        II c.acc.pers.to give over, deliver up, Hom., etc.\n",
                "			2 of parents, to give their daughter to wife, id= Hom.\n",
                "			3 in attic, διδόναι τινά τινι to grant any one to entreaties, pardon him, Xen.:   διδόναι τινί τι to forgive one a thing, remit its punishment, Eur., Dem.\n",
                "			4  διδόναι ἑαυτόν τινι to give oneself up, Hdt., etc.\n",
                "			5  δδ.ίκην, v.δίκη IV. 3.\n\n",
                "        III in vows and prayers, c.acc.pers.et inf. to grant, allow, bring about that, Hom., Trag.\n",
                "        IV seemingly intr.to give oneself up, devote oneself, τινί Eur." };

            String[] mtSearch = { "2 Chronicles 6.27:",
                              "   וְאַתָּ֣ה תִּשְׁמַ֣ע הַשָּׁמַ֗יִם וְסָ֨לַחְתָּ֜ לְחַטַּ֤את עֲבָדֶ֨יךָ֙ וְעַמְּךָ֣ יִשְׂרָאֵ֔ל כִּ֥י תֹורֵ֛ם אֶל־הַ״דֶּ֥רֶךְ הַ׳טֹּובָ֖ה אֲשֶׁ֣ר יֵֽלְכוּ־בָ֑הּ וְנָתַתָּ֤ה מָטָר֙ עַֽל־אַרְצְךָ֔ אֲשֶׁר־נָתַ֥תָּה לְעַמְּךָ֖ לְנַחֲלָֽה׃ ס",
                              "", "Psalms 25.12:",
                              "   מִי־זֶ֣ה הָ֭אִישׁ יְרֵ֣א יְהוָ֑ה יֹ֝ורֶ֗נּוּ בְּ״דֶ֣רֶךְ יִבְחָֽר׃", "Psalms 25.13:   נַ֭פְשֹׁו בְּ׳טֹ֣וב תָּלִ֑ין וְ֝זַרְעֹ֗ו יִ֣ירַשׁ אָֽרֶץ׃", "",
                              "Psalms 36.4:",
                              "   אָ֤וֶן יַחְשֹׁ֗ב עַֽל־מִשְׁכָּ֫בֹ֥ו יִ֭תְיַצֵּב עַל־״דֶּ֣רֶךְ לֹא־׳טֹ֑וב רָ֝֗ע לֹ֣א יִמְאָֽס׃", "", "Proverbs 2.20:", "   לְמַ֗עַן תֵּ֭לֵךְ בְּ״דֶ֣רֶךְ ׳טֹובִ֑ים וְאָרְחֹ֖ות צַדִּיקִ֣ים תִּשְׁמֹֽר׃", "",
                              "Proverbs 13.15:", "   שֵֽׂכֶל־׳טֹ֭וב יִתֶּן־חֵ֑ן וְ״דֶ֖רֶךְ בֹּגְדִ֣ים אֵיתָֽן׃", "", "Proverbs 16.29:", "   אִ֣ישׁ חָ֭מָס יְפַתֶּ֣ה רֵעֵ֑הוּ וְ֝הֹולִיכֹ֗ו בְּדֶ֣רֶךְ לֹא־׳טֹֽוב׃", "",
                              "Proverbs 16.31:", "   עֲטֶ֣רֶת תִּפְאֶ֣רֶת שֵׂיבָ֑ה בְּ״דֶ֥רֶךְ צְ֝דָקָ֗ה תִּמָּצֵֽא׃", "Proverbs 16.32:", "   ׳טֹ֤וב אֶ֣רֶךְ אַ֭פַּיִם מִגִּבֹּ֑ור וּמֹשֵׁ֥ל בְּ֝רוּחֹ֗ו מִלֹּכֵ֥ד עִֽיר׃" };

            String[] lxxSearch = { "Numbers 20.16:",
                "καὶ ἀνεβοήσαμεν πρὸς κύριον, καὶ εἰσήκουσεν κύριος τῆς yφωνῆς ἡμῶν καὶ ἀποστείλας xἄγγελον ἐξήγαγεν ἡμᾶς ἐξ Αἰγύπτου, " +
                               "καὶ νῦν ἐσμεν ἐν Καδης, πόλει ἐκ μέρους τῶν ὁρίων σου·", "",
                               "Judges(Codex Alexandrinus) 6.10:",
                "καὶ εἶπα ὑμῖν Ἐγὼ κύριος ὁ θεὸς ὑμῶν, οὐ φοβηθήσεσθε τοὺς θεοὺς τοῦ Αμορραίου, " +
                               "ἐν οἷς ὑμεῖς ἐνοικεῖτε ἐν τῇ γῇ αὐτῶν· καὶ οὐκ εἰσηκούσατε τῆς yφωνῆς μου.", "Judges(Codex Alexandrinus) 6.11:",
                "Καὶ ἦλθεν xἄγγελος κυρίου καὶ " +
                               "ἐκάθισεν ὑπὸ τὴν δρῦν τὴν οὖσαν ἐν Εφραθα τὴν τοῦ Ιωας πατρὸς Αβιεζρι, καὶ Γεδεων ὁ υἱὸς αὐτοῦ ἐρράβδιζεν πυροὺς ἐν ληνῷ τοῦ ἐκφυγεῖν ἐκ προσώπου Μαδιαμ.",
                               "", "Judges(Codex Alexandrinus) 13.9:",
                "καὶ ἐπήκουσεν ὁ θεὸς τῆς yφωνῆς Μανωε, καὶ παρεγένετο ὁ xἄγγελος τοῦ θεοῦ ἔτι πρὸς τὴν γυναῖκα αὐτῆς " +
                               "καθημένης ἐν τῷ ἀγρῷ, καὶ Μανωε ὁ ἀνὴρ αὐτῆς οὐκ ἦν μετ αὐτῆς.", "",
                               "Judges(Codex Vaticanus) 6.10:",
                "καὶ εἶπα ὑμῖν Ἐγὼ κύριος ὁ θεὸς ὑμῶν, οὐ φοβηθήσεσθε τοὺς θεοὺς τοῦ Αμορραίου, " +
                               "ἐν οἷς ὑμεῖς καθήσεσθε ἐν τῇ γῇ αὐτῶν· καὶ οὐκ εἰσηκούσατε τῆς yφωνῆς μου.", "Judges(Codex Vaticanus) 6.11:",
                "Καὶ ἦλθεν xἄγγελος κυρίου καὶ ἐκάθισεν " +
                               "ὑπὸ τὴν τερέμινθον τὴν ἐν Εφραθα τὴν Ιωας πατρὸς τοῦ Εσδρι, καὶ Γεδεων υἱὸς αὐτοῦ ῥαβδίζων σῖτον ἐν ληνῷ εἰς ἐκφυγεῖν ἀπὸ προσώπου τοῦ Μαδιαμ." };

            String exampleNote = "My notes tell me that this:\n\n" +
                                        "Τοῦ δὲ Ἰησοῦ χριστοῦ ἡ γένεσις οὕτως ἦν. μνηστευθείσης τῆς μητρὸς αὐτοῦ Μαρίας τῷ Ἰωσήφ, πρὶν ἢ συνελθεῖν αὐτοὺς " +
                                        "εὑρέθη ἐν γαστρὶ ἔχουσα ἐκ πνεύματος ἁγίου.\n\nis a verse which ...";

            exampleText = new SortedList<int, classPreferencesExamples>[noOfAreas];
            noOfWordsInExample = new int[noOfAreas];
            // Area 0: MT text
            exampleText[0] = new SortedList<int, classPreferencesExamples>();
            itemCount = 0;
            noOfLines = mtText.Length;
            for( lineIndex = 0; lineIndex < noOfLines; lineIndex++)
            {
                if( lineIndex > 0) addToClass(0, "\n", 2, itemCount++); // New line
                individualItem = mtText[lineIndex].Split(textSplitter);
                noOfItems = individualItem.Length;
                for( itemIndex = 0; itemIndex < noOfItems; itemIndex++)
                {
                    if( individualItem[itemIndex].Contains( ':')) addToClass(0, individualItem[itemIndex] + " ", 1, itemCount++); // The word is a verse number - use english font
                    else
                    {
                        if( individualItem[itemIndex].Contains('׳')) // The word contains a Kethib/qere
                        {
                            splitWord = individualItem[itemIndex].Split('׳');
                            if( splitWord[0].Length > 0 ) addToClass(0, splitWord[0], 2, itemCount++);
                            addToClass(0, splitWord[1] + " ", 3, itemCount++);
                        }
                        else addToClass(0, individualItem[itemIndex] + " ", 2, itemCount++);  // Normal non-English word
                    }
                }
            }
            noOfWordsInExample[0] = itemCount;
            // Area 1: LXX text
            exampleText[1] = new SortedList<int, classPreferencesExamples>();
            itemCount = 0;
            noOfLines = lxxText.Length;
            for (lineIndex = 0; lineIndex < noOfLines; lineIndex++)
            {
                if (lineIndex > 0) addToClass(1, "\n", 2, itemCount++); // New line
                individualItem = lxxText[lineIndex].Split(textSplitter);
                noOfItems = individualItem.Length;
                for (itemIndex = 0; itemIndex < noOfItems; itemIndex++)
                {
                    if (individualItem[itemIndex].Contains(':')) addToClass(1, individualItem[itemIndex] + "  ", 1, itemCount++); // The word is a verse number - use english font
                    else addToClass(1, individualItem[itemIndex] + " ", 2, itemCount++);  // Normal non-English word
                }
            }
            noOfWordsInExample[1] = itemCount;
            // Area 2: Parse area
            exampleText[2] = new SortedList<int, classPreferencesExamples>();
            itemCount = 0;
            noOfLines = mtParse.Length;
            for (lineIndex = 0; lineIndex < noOfLines; lineIndex++)
            {
                if (lineIndex == 0) addToClass(2, mtParse[0] + "\n\n", 2, itemCount++);
                else addToClass(2, mtParse[lineIndex], 1, itemCount++);
            }
            noOfWordsInExample[2] = itemCount;
            // Area 3: Lexical area (for LXX)
            exampleText[3] = new SortedList<int, classPreferencesExamples>();
            itemCount = 0;
            noOfLines = lxxLex.Length;
            for (lineIndex = 0; lineIndex < noOfLines; lineIndex++)
            {
                if (lineIndex == 0) addToClass(3, lxxLex[0], 2, itemCount++);
                else addToClass(3, lxxLex[lineIndex], 1, itemCount++);
            }
            noOfWordsInExample[3] = itemCount;
            // Area 4: MT search
            exampleText[4] = new SortedList<int, classPreferencesExamples>();
            itemCount = 0;
            noOfLines = mtSearch.Length;
            for (lineIndex = 0; lineIndex < noOfLines; lineIndex++)
            {
                if (mtSearch[lineIndex].Length == 0)
                {
                    addToClass(4, "", 6, itemCount++);
                    continue;
                }
                if (mtSearch[lineIndex].Contains(':'))
                {
                    nPstn = mtSearch[lineIndex].IndexOf(':');
                    addToClass(4, mtSearch[lineIndex].Substring(0, nPstn), 1, itemCount++); // The word is a reference - use english font
                }
                else
                {
                    individualItem = mtSearch[lineIndex].Split(textSplitter);
                    noOfItems = individualItem.Length;
                    for (itemIndex = 0; itemIndex < noOfItems; itemIndex++)
                    {
                        if (individualItem[itemIndex].Contains('׳')) // The word contains a Kethib/qere
                        {
                            splitWord = individualItem[itemIndex].Split('׳');
                            if (splitWord[0].Length > 0)
                            {
                                addToClass(4, splitWord[0], 2, itemCount++);
                                addToClass(4, splitWord[1] + " ", 3, itemCount++);
                            }
                            else addToClass(4, splitWord[0] + " ", 3, itemCount++);
                        }
                        else
                        {
                            if (individualItem[itemIndex].Contains('״')) // The word contains a Kethib/qere
                            {
                                splitWord = individualItem[itemIndex].Split('״');
                                if (splitWord[0].Length > 0)
                                {
                                    addToClass(4, splitWord[0], 2, itemCount++);
                                    addToClass(4, splitWord[1] + " ", 4, itemCount++);
                                }
                                else addToClass(4, splitWord[0] + " ", 4, itemCount++);
                            }
                            else addToClass(4, individualItem[itemIndex] + " ", 2, itemCount++);  // Normal non-English word
                        }
                    }
                    addToClass(4, "", 5, itemCount++);
                }
            }
            noOfWordsInExample[4] = itemCount;
            // Area 5: LXX search
            exampleText[5] = new SortedList<int, classPreferencesExamples>();
            itemCount = 0;
            noOfLines = lxxSearch.Length;
            for (lineIndex = 0; lineIndex < noOfLines; lineIndex++)
            {
                if (lxxSearch[lineIndex].Length == 0)
                {
                    addToClass(5, "", 6, itemCount++);
                    continue;
                }
                if( lxxSearch[lineIndex].Contains( ':' ))
                {
                    nPstn = lxxSearch[lineIndex].IndexOf(':');
                    addToClass(5, lxxSearch[lineIndex].Substring(0, nPstn), 1, itemCount++); // The word is a reference - use english font
                }
                else
                {
                    individualItem = lxxSearch[lineIndex].Split(textSplitter);
                    noOfItems = individualItem.Length;
                    for (itemIndex = 0; itemIndex < noOfItems; itemIndex++)
                    {
                        if (individualItem[itemIndex].Contains('x')) // The word contains a Kethib/qere
                        {
                            splitWord = individualItem[itemIndex].Split('x');
                            if (splitWord[0].Length > 0)
                            {
                                addToClass(5, splitWord[0], 2, itemCount++);
                                addToClass(5, splitWord[1] + " ", 3, itemCount++);
                            }
                            else addToClass(5, splitWord[1] + " ", 3, itemCount++);
                        }
                        else
                        {
                            if (individualItem[itemIndex].Contains('y')) // The word contains a Kethib/qere
                            {
                                splitWord = individualItem[itemIndex].Split('y');
                                if (splitWord[0].Length > 0)
                                {
                                    addToClass(5, splitWord[0], 2, itemCount++);
                                    addToClass(5, splitWord[1] + " ", 4, itemCount++);
                                }
                                else addToClass(5, splitWord[1] + " ", 4, itemCount++);
                            }
                            else addToClass(5, individualItem[itemIndex] + " ", 2, itemCount++);  // Normal non-English word
                        }
                    }
                    addToClass(5, "", 5, itemCount++);
                }
            }
            noOfWordsInExample[5] = itemCount;
            // Area 6: notes
            exampleText[6] = new SortedList<int, classPreferencesExamples>();
            addToClass(6, exampleNote, 1, 0);
            noOfWordsInExample[6] = 1;
            // Area 7: notes
            exampleText[7] = new SortedList<int, classPreferencesExamples>();
            addToClass(7, exampleNote, 1, 0);
            noOfWordsInExample[7] = 1;
        }

        private RichTextBox initialSetupOfRText(Color backgroundColour, int colWidth, RightToLeft rightToLeft)
        {
            RichTextBox rtxtCurrent;

            rtxtCurrent = new RichTextBox();
            rtxtCurrent.BackColor = backgroundColour;
            rtxtCurrent.Width = colWidth;
            rtxtCurrent.Height = 120;
            rtxtCurrent.RightToLeft = rightToLeft;
            rtxtCurrent.Visible = false;
            return rtxtCurrent;
        }

        private int getNewRTXHeight(RichTextBox rtxCurrent, Font fontUsed)
        {
            int noOfLines, noOfRTXLines;
            float fontHeight;

            noOfRTXLines = 0;
            if (rtxCurrent != null)
            {
                noOfLines = rtxCurrent.GetLineFromCharIndex(rtxCurrent.Text.Length) + 1;
                fontHeight = fontUsed.Height;
                noOfRTXLines = noOfLines * ((int)fontHeight) + rtxCurrent.Margin.Vertical;
            }
            return noOfRTXLines;
        }

        private void resetRTX(RichTextBox rtxtCurrent, int newHeight)
        {
            if (rtxtCurrent == null) return;
            rtxtCurrent.Height = newHeight;
            rtxtCurrent.Visible = true;
        }

        private void displayExampleText( int areaCode)
        {
            const int refWidth = 220;

            bool isFirst;
            int idx, typeCode, index, noOfItems, boxHeight, boxWidth;
            float fontSize = 12F;
            String fontName = "", fontStyle;
            Color tempColour = Color.Empty;
            RichTextBox rtxtCurrent;
            FontStyle actualStyle = FontStyle.Regular;
            SortedList<int, classPreferencesExamples> specificText;
            classPreferencesExamples currentExample;

            if ((areaCode == 4) || (areaCode == 5))
            {
                specificText = exampleText[areaCode];
                index = 0;
                noOfItems = specificText.Count;
                idx = 0;
                boxWidth = pnlSearch[areaCode - 4].Width - refWidth - 50;
                pnlSearch[areaCode - 4].Controls.Clear();
                isFirst = true;
                rtxtCurrent = null;
                while (idx < noOfItems)
                {
                    specificText.TryGetValue(idx++, out currentExample);
                    typeCode = currentExample.TypeCode;
                    switch (typeCode)
                    {
                        case 1:
                            backColourRec.TryGetValue(areaCode, out tempColour);
                            rtxtCurrent = initialSetupOfRText(tempColour, refWidth, RightToLeft.No);
                            rtxtReference[areaCode - 4, index] = rtxtCurrent;
                            foreMainColourRec.TryGetValue(areaCode, out tempColour);
                            englishTextSize.TryGetValue(areaCode, out fontSize);
                            englishFontName.TryGetValue(areaCode, out fontName);
                            englishMainStyle.TryGetValue(areaCode, out fontStyle);
                            switch (fontStyle)
                            {
                                case "Regular": actualStyle = FontStyle.Regular; break;
                                case "Bold": actualStyle = FontStyle.Bold; break;
                                case "Italic": actualStyle = FontStyle.Italic; break;
                                case "Underline": actualStyle = FontStyle.Underline; break;
                                case "Strikeout": actualStyle = FontStyle.Strikeout; break;
                            }
                            rtxtCurrent.SelectionColor = tempColour;
                            rtxtCurrent.SelectionFont = new Font(fontName, fontSize, actualStyle);
                            rtxtCurrent.SelectedText = currentExample.Text;
                            pnlSearch[areaCode - 4].Controls.Add(rtxtCurrent);
                            break;
                        case 2:
                            if (isFirst)
                            {
                                backColourRec.TryGetValue(areaCode, out tempColour);
                                if (areaCode == 4) rtxtCurrent = initialSetupOfRText(tempColour, boxWidth, RightToLeft.Yes);
                                else rtxtCurrent = initialSetupOfRText(tempColour, boxWidth, RightToLeft.No);
                                rtxtVerse[areaCode - 4, index] = rtxtCurrent;
                                pnlSearch[areaCode - 4].Controls.Add(rtxtCurrent);
                                isFirst = false;
                            }
                            foreAltColour.TryGetValue(areaCode, out tempColour);
                            hebrewTextSize.TryGetValue(areaCode, out fontSize);
                            foreignFontName.TryGetValue(areaCode, out fontName);
                            foreignMainStyle.TryGetValue(areaCode, out fontStyle);
                            switch (fontStyle)
                            {
                                case "Regular": actualStyle = FontStyle.Regular; break;
                                case "Bold": actualStyle = FontStyle.Bold; break;
                                case "Italic": actualStyle = FontStyle.Italic; break;
                                case "Underline": actualStyle = FontStyle.Underline; break;
                                case "Strikeout": actualStyle = FontStyle.Strikeout; break;
                            }
                            rtxtCurrent.SelectionColor = tempColour;
                            rtxtCurrent.SelectionFont = new Font(fontName, fontSize, actualStyle);
                            rtxtCurrent.SelectedText = currentExample.Text;
                            break;
                        case 3:
                            if (isFirst)
                            {
                                backColourRec.TryGetValue(areaCode, out tempColour);
                                if (areaCode == 4) rtxtCurrent = initialSetupOfRText(tempColour, boxWidth, RightToLeft.Yes);
                                else rtxtCurrent = initialSetupOfRText(tempColour, boxWidth, RightToLeft.No);
                                rtxtVerse[areaCode - 4, index] = rtxtCurrent;
                                pnlSearch[areaCode - 4].Controls.Add(rtxtCurrent);
                                isFirst = false;
                            }
                            forePrimeColour.TryGetValue(areaCode, out tempColour);
                            primaryTextSize.TryGetValue(areaCode, out fontSize);
                            primaryFontName.TryGetValue(areaCode, out fontName);
                            primaryAltStyle.TryGetValue(areaCode, out fontStyle);
                            switch (fontStyle)
                            {
                                case "Regular": actualStyle = FontStyle.Regular; break;
                                case "Bold": actualStyle = FontStyle.Bold; break;
                                case "Italic": actualStyle = FontStyle.Italic; break;
                                case "Underline": actualStyle = FontStyle.Underline; break;
                                case "Strikeout": actualStyle = FontStyle.Strikeout; break;
                            }
                            rtxtCurrent.SelectionColor = tempColour;
                            rtxtCurrent.SelectionFont = new Font(fontName, fontSize, actualStyle);
                            rtxtCurrent.SelectedText = currentExample.Text;
                            break;
                        case 4:
                            if (isFirst)
                            {
                                backColourRec.TryGetValue(areaCode, out tempColour);
                                if (areaCode == 4) rtxtCurrent = initialSetupOfRText(tempColour, boxWidth, RightToLeft.Yes);
                                else rtxtCurrent = initialSetupOfRText(tempColour, boxWidth, RightToLeft.No);
                                rtxtVerse[areaCode - 4, index] = rtxtCurrent;
                                pnlSearch[areaCode - 4].Controls.Add(rtxtCurrent);
                                isFirst = false;
                            }
                            foreSecondColour.TryGetValue(areaCode, out tempColour);
                            secondaryTextSize.TryGetValue(areaCode, out fontSize);
                            secondaryFontName.TryGetValue(areaCode, out fontName);
                            secondaryAltStyle.TryGetValue(areaCode, out fontStyle);
                            switch (fontStyle)
                            {
                                case "Regular": actualStyle = FontStyle.Regular; break;
                                case "Bold": actualStyle = FontStyle.Bold; break;
                                case "Italic": actualStyle = FontStyle.Italic; break;
                                case "Underline": actualStyle = FontStyle.Underline; break;
                                case "Strikeout": actualStyle = FontStyle.Strikeout; break;
                            }
                            rtxtCurrent.SelectionColor = tempColour;
                            rtxtCurrent.SelectionFont = new Font(fontName, fontSize, actualStyle);
                            rtxtCurrent.SelectedText = currentExample.Text;
                            break;
                        case 5:
                            hebrewTextSize.TryGetValue(areaCode, out fontSize);
                            foreignFontName.TryGetValue(areaCode, out fontName);
                            foreignMainStyle.TryGetValue(areaCode, out fontStyle);
                            switch (fontStyle)
                            {
                                case "Regular": actualStyle = FontStyle.Regular; break;
                                case "Bold": actualStyle = FontStyle.Bold; break;
                                case "Italic": actualStyle = FontStyle.Italic; break;
                                case "Underline": actualStyle = FontStyle.Underline; break;
                                case "Strikeout": actualStyle = FontStyle.Strikeout; break;
                            }
                            boxHeight = getNewRTXHeight(rtxtVerse[areaCode - 4, index], new Font(fontName, fontSize, actualStyle));
                            resetRTX(rtxtReference[areaCode - 4, index], boxHeight);
                            resetRTX(rtxtVerse[areaCode - 4, index], boxHeight);
                            if (!isFirst)
                            {
                                isFirst = true;
                                index++;
                            }
                            break;
                        case 6:
                            index++;
                            rtxtCurrent = new RichTextBox();
                            rtxtCurrent.BackColor = Color.Black;
                            rtxtCurrent.Width = pnlSearch[areaCode - 4].Width - 20;
                            rtxtCurrent.Height = 5;
                            rtxtReference[areaCode - 4, index] = rtxtCurrent;
                            pnlSearch[areaCode - 4].Controls.Add(rtxtCurrent);
                            isFirst = true;
                            index++;
                            break;
                    }
                }
            }
            else
            {
                specificText = exampleText[areaCode];
                rtxtCurrent = rtxtExample[areaCode];
                rtxtCurrent.Clear();
                backColourRec.TryGetValue(areaCode, out tempColour);
                rtxtCurrent.BackColor = tempColour;
                foreach (KeyValuePair<int, classPreferencesExamples> egPair in specificText)
                {
                    typeCode = egPair.Value.TypeCode;
                    switch (typeCode)
                    {
                        case 1:
                            foreMainColourRec.TryGetValue(areaCode, out tempColour);
                            englishTextSize.TryGetValue(areaCode, out fontSize);
                            englishFontName.TryGetValue(areaCode, out fontName);
                            englishMainStyle.TryGetValue(areaCode, out fontStyle);
                            switch (fontStyle)
                            {
                                case "Regular": actualStyle = FontStyle.Regular; break;
                                case "Bold": actualStyle = FontStyle.Bold; break;
                                case "Italic": actualStyle = FontStyle.Italic; break;
                                case "Underline": actualStyle = FontStyle.Underline; break;
                                case "Strikeout": actualStyle = FontStyle.Strikeout; break;
                            }
                            break;
                        case 2:
                            foreAltColour.TryGetValue(areaCode, out tempColour);
                            hebrewTextSize.TryGetValue(areaCode, out fontSize);
                            foreignFontName.TryGetValue(areaCode, out fontName);
                            foreignMainStyle.TryGetValue(areaCode, out fontStyle);
                            switch (fontStyle)
                            {
                                case "Regular": actualStyle = FontStyle.Regular; break;
                                case "Bold": actualStyle = FontStyle.Bold; break;
                                case "Italic": actualStyle = FontStyle.Italic; break;
                                case "Underline": actualStyle = FontStyle.Underline; break;
                                case "Strikeout": actualStyle = FontStyle.Strikeout; break;
                            }
                            break;
                        case 3:
                            forePrimeColour.TryGetValue(areaCode, out tempColour);
                            primaryTextSize.TryGetValue(areaCode, out fontSize);
                            primaryFontName.TryGetValue(areaCode, out fontName);
                            primaryAltStyle.TryGetValue(areaCode, out fontStyle);
                            switch (fontStyle)
                            {
                                case "Regular": actualStyle = FontStyle.Regular; break;
                                case "Bold": actualStyle = FontStyle.Bold; break;
                                case "Italic": actualStyle = FontStyle.Italic; break;
                                case "Underline": actualStyle = FontStyle.Underline; break;
                                case "Strikeout": actualStyle = FontStyle.Strikeout; break;
                            }
                            break;
                    }
                    rtxtCurrent.SelectionColor = tempColour;
                    rtxtCurrent.SelectionFont = new Font(fontName, fontSize, actualStyle);
                    rtxtCurrent.SelectedText = egPair.Value.Text;
                }
            }
        }

        private void addToClass( int classIndex, String text, int code, int noOfItems)
        {
            classPreferencesExamples currentExampleClass;

            currentExampleClass = new classPreferencesExamples();
            currentExampleClass.Text = text;
            currentExampleClass.TypeCode = code;
            exampleText[classIndex].Add(noOfItems, currentExampleClass);
        }

        private TabPage createNewTabPage(String name)
        {
            TabPage tempPage;

            tempPage = new TabPage();
            tempPage.Text = name;
            return tempPage;
        }

        private void pictureBoxMouseClick(object sender, MouseEventArgs e)
        {
            int tagVal, areaVal, pboxType;
            Color tempColour = Color.Empty;
            PictureBox currentPBox;
            SortedList<int, Color> selectedPBox = null;

            currentPBox = (PictureBox)sender;
            tagVal = Convert.ToInt32(currentPBox.Tag);
            areaVal = tagVal / 10;
            pboxType = tagVal % 10;
            switch( pboxType)
            {
                case 0: selectedPBox = backColourRec; break;
                case 1: selectedPBox = foreMainColourRec; break;
                case 2: selectedPBox = foreAltColour; break;
                case 3: selectedPBox = forePrimeColour; break;
                case 4: selectedPBox = foreSecondColour; break;
            }
            selectedPBox.TryGetValue(areaVal, out tempColour);
            if( tempColour != Color.Empty)
            {
                dlgBgColour.Color = tempColour;
                if (dlgBgColour.ShowDialog() == DialogResult.OK)
                {
                    selectedPBox.Remove(areaVal);
                    selectedPBox.Add(areaVal, dlgBgColour.Color);
                    currentPBox.BackColor = dlgBgColour.Color;
                    displayExampleText(areaVal);
                    hasChanged[areaVal] = true;
                    btnReset[areaVal].Enabled = true;
                }
            }
        }

        private void BtnFont_Click(object sender, EventArgs e)
        {
            int tagVal, areaVal, subVal;
            float actualSize = 12F;
            String styleName = "", actualFont = "";
            FontStyle actualFontStyle = FontStyle.Regular;
            Font newFont;
            Button btnClicked;
            SortedList<int, float> textSize = null;
            SortedList<int, String> textStyle = null;
            SortedList<int, String> fontName = null;

            btnClicked = (Button)sender;
            tagVal = Convert.ToInt32(btnClicked.Tag);
            areaVal = tagVal / 10;
            subVal = tagVal % 10;
            switch (subVal)
            {
                case 0:
                    textSize = englishTextSize;
                    textStyle = englishMainStyle;
                    fontName = englishFontName;
                    break;
                case 1:
                    textSize = hebrewTextSize;
                    textStyle = foreignMainStyle;
                    fontName = foreignFontName;
                    break;
                case 2:
                    textSize = primaryTextSize;
                    textStyle = primaryAltStyle;
                    fontName = primaryFontName;
                    break;
                case 3:
                    textSize = secondaryTextSize;
                    textStyle = secondaryAltStyle;
                    fontName = secondaryFontName;
                    break;
            }
            textSize.TryGetValue(areaVal, out actualSize);
            textStyle.TryGetValue(areaVal, out styleName);
            fontName.TryGetValue(areaVal, out actualFont);
            switch( styleName)
            {
                case "Regular": actualFontStyle = FontStyle.Regular; break;
                case "Bold": actualFontStyle = FontStyle.Bold; break;
                case "Italic": actualFontStyle = FontStyle.Italic; break;
                case "Underline": actualFontStyle = FontStyle.Underline; break;
                case "Strikeout": actualFontStyle = FontStyle.Strikeout; break;
            }
            dlgFont.Font = new Font(actualFont, actualSize, actualFontStyle);
            if( dlgFont.ShowDialog() == DialogResult.OK)
            {
                newFont = dlgFont.Font;
                actualSize = newFont.Size;
                actualFont = newFont.Name;
                actualFontStyle = newFont.Style;
                switch( actualFontStyle)
                {
                    case FontStyle.Regular: styleName = "Regular"; break;
                    case FontStyle.Bold: styleName = "Bold"; break;
                    case FontStyle.Italic: styleName = "Italic"; break;
                    case FontStyle.Underline: styleName = "Underline"; break;
                    case FontStyle.Strikeout: styleName = "Strikeout"; break;
                }
                textSize.Remove(areaVal);
                textStyle.Remove(areaVal);
                fontName.Remove(areaVal);
                textSize.Add(areaVal, actualSize);
                textStyle.Add(areaVal, styleName);
                fontName.Add(areaVal, actualFont);
                displayExampleText(areaVal);
                hasChanged[areaVal] = true;
                btnReset[areaVal].Enabled = true;
            }
        }

        private void resetButton(object sender, EventArgs e)
        {
            int tagVal, idx, limit = 0;
            Color newColour;
            Button btnDoReset;

            btnDoReset = (Button)sender;
            tagVal = Convert.ToInt32(btnDoReset.Tag);
            backColourRec.Remove(tagVal);
            newColour = globalVars.getColourSetting(tagVal, 0);
            backColourRec.Add(tagVal, newColour);
            colourBoxes[tagVal, 0].BackColor = newColour;
            switch( tagVal)
            {
                case 0: limit = 3; break;
                case 1: limit = 2; break;
                case 2: limit = 2; break;
                case 3: limit = 2; break;
                case 4: limit = 4; break;
                case 5: limit = 4; break;
                case 6: limit = 1; break;
                case 7: limit = 1; break;
            }
            for (idx = 1; idx <= limit; idx++)
            {
                switch (idx)
                {
                    case 1: processReset(tagVal, idx, foreMainColourRec, englishTextSize, englishMainStyle, englishFontName); break;
                    case 2: processReset(tagVal, idx, foreAltColour, hebrewTextSize, foreignMainStyle, foreignFontName); break;
                    case 3: processReset(tagVal, idx, forePrimeColour, primaryTextSize, primaryAltStyle, primaryFontName); break;
                    case 4: processReset(tagVal, idx, foreSecondColour, secondaryTextSize, secondaryAltStyle, secondaryFontName); break;
                }
            }
            displayExampleText(tagVal);
            hasChanged[tagVal] = false;
            btnReset[tagVal].Enabled = false;

        }

        private void processReset(int tagVal, int actionType,
                                  SortedList<int, Color> colourList,
                                  SortedList<int, float> sizeList,
                                  SortedList<int, String> styleList,
                                  SortedList<int, String> fontList)
        {
            Color newColour;

            newColour = globalVars.getColourSetting(tagVal, actionType);
            colourList.Remove(tagVal);
            colourList.Add(tagVal, newColour);
            colourBoxes[tagVal, actionType].BackColor = newColour;
            sizeList.Remove(tagVal);
            sizeList.Add(tagVal, globalVars.getTextSize(tagVal, actionType));
            styleList.Remove(tagVal);
            styleList.Add(tagVal, globalVars.getDefinedStyleByIndex(tagVal, actionType));
            fontList.Remove(tagVal);
            fontList.Add(tagVal, globalVars.getDefinedFontNameByIndex(tagVal, actionType));
        }

        private void populateGlobalData()
        {
            /*--------------------------------------------------------------------------------------------------*
             *                                                                                                  *
             *  Get current values from classGlobal                                                             *
             *                                                                                                  *
             *  In all that follows, the indexes representing different application areas function as follows:  *
             *                                                                                                  *
             *                   Value              Refers to                                                   *
             *                   -----              ---------                                                   *
             *                     0            Main MT Text area                                               *
             *                     1            Main LXX Text area                                              *
             *                     2            Parse area (for both MT and LXX)                                *
             *                     3            Lexical Area (for LXX)                                          *
             *                     4            Search Results Area when using MT                               *
             *                     5            Search Results Area when using LXX                              *
             *                     6            Notes Area for MT                                               *
             *                     7            Motes area for LXX                                              *
             *                                                                                                  *
             *--------------------------------------------------------------------------------------------------*/

            int idx, jdx, limit = 0;
            Color tempColour;

            for (idx = 0; idx < noOfAreas; idx++)
            {
                if (hasChanged[idx])
                {
                    backColourRec.TryGetValue(idx, out tempColour);
                    globalVars.addColourSetting(idx, tempColour, 0);
                    switch (idx)
                    {
                        case 0: limit = 3; break;
                        case 1: limit = 2; break;
                        case 2: limit = 2; break;
                        case 3: limit = 2; break;
                        case 4: limit = 4; break;
                        case 5: limit = 4; break;
                        case 6: limit = 1; break;
                        case 7: limit = 1; break;
                    }
                    for (jdx = 1; jdx <= limit; jdx++)
                    {
                        switch (jdx)
                        {
                            case 1: modifyGlobalData(idx, jdx, foreMainColourRec, englishTextSize, englishMainStyle, englishFontName); break;
                            case 2: modifyGlobalData(idx, jdx, foreAltColour, hebrewTextSize, foreignMainStyle, foreignFontName); break;
                            case 3: modifyGlobalData(idx, jdx, forePrimeColour, primaryTextSize, primaryAltStyle, primaryFontName); break;
                            case 4: modifyGlobalData(idx, jdx, foreSecondColour, secondaryTextSize, secondaryAltStyle, secondaryFontName); break;
                        }
                    }
                }
            }
        }

        private void modifyGlobalData(int tagVal, int actionType,
                                  SortedList<int, Color> colourList,
                                  SortedList<int, float> sizeList,
                                  SortedList<int, String> styleList,
                                  SortedList<int, String> fontList)
        {
            float newSize = 12F;
            String newStyle = "", newFont = "";
            Color newColour = Color.Empty;

            colourList.TryGetValue(tagVal, out newColour);
            globalVars.addColourSetting(tagVal, newColour, actionType);
            sizeList.TryGetValue(tagVal, out newSize);
            globalVars.addTextSize(tagVal, newSize, actionType);
            styleList.TryGetValue(tagVal, out newStyle);
            globalVars.addDefinedStyle(tagVal, newStyle, actionType);
            fontList.TryGetValue(tagVal, out newFont);
            globalVars.addFontName(tagVal, newFont, actionType);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            populateGlobalData();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
