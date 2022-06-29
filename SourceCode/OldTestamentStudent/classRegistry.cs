using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace OldTestamentStudent
{
    public class classRegistry
    {
        const int noOfAreas = 8;

        bool isInitialising = false;
        int defaultSplitPosition, defaultState, defaultHeight, defaultWidth, defaultX, defaultY;
        String keyString, cautionFileName = "deferredAction.txt";
        String[] keyColourNames = { "MT Text Background Colour", "MT Verse No. Colour", "MT Text Colour", "MT Kethib/Qere Colour", "",
                                        "LXX Text Background Colour", "LXX Verse No. Colour", "LXX Text Colour", "", "",
                                          "Parse Area Background Colour", "Parse Area Text Colour", "Parse header Colour", "", "",
                                          "Lexical Area Background Colour", "Lexical Area Text Colour", "Lexical Area header Colour", "", "",
                                          "Search Results Background Colour MT", "Search Reference Colour", "Search Results Text Colour MT", "Search Results Alt Colour MT", "Search Results Secondary Colour MT",
                                          "Search Results Background Colour LXX", "Search Reference Colour", "Search Results Text Colour LXX", "Search Results Alt Colour LXX", "Search Results Secondary Colour LXX",
                                          "MT Notes Background Colour", "MT Notes Text Colour", "", "", "",
                                          "LXX Notes Background Colour", "LXX Notes Text Colour", "", "", "" };
        String[] keyFontSizeNames = { "MT English Text Font Size", "MT Hebrew Text Font Size", "Kethib/Qere Text Font Size", "",
                                          "LXX English Text Font Size", "LXX Greek Text Font Size", "", "",
                                          "Parse Area Font Size", "Parse Title Font Size", "", "",
                                          "Lexical Area Font Size", "Lexical Title Font Size", "", "",
                                          "Search Results English Font Size MT", "Search Results Hebrew Font Size MT", "Search Primary Match Size MT", "Search Secondary Match Size MT",
                                          "Search Results English Font Size LXX", "Search Results Hebrew Font Size LXX", "Search Primary Match Size LXX", "Search Secondary Match Size LXX",
                                          "MT Notes Font Size", "", "", "",
                                          "LXX Notes Font Size",  "", "", "" };
        String[] keyFontStyleNames = { "MT English Text Style", "MT Hebrew Text Style", "Kethib/Qere Text Style", "",
                                          "LXX English Text Style", "LXX Greek Text Style", "", "",
                                          "Parse Area Style", "Parse Title Style", "", "",
                                          "Lexical Area Style", "Lexical Title Style", "", "",
                                          "Search Results English Style MT", "Search Results Hebrew Style MT", "Search Primary Match Style MT", "Search Secondary Match Style MT",
                                          "Search Results English Style LXX", "Search Results Hebrew Style LXX", "Search Primary Match Style LXX", "Search Secondary Match Style LXX",
                                          "MT Notes Style", "", "", "", 
                                          "LXX Notes Style", "", "", "" };
        String[] keyFontNameNames = { "MT English Text Font Name", "MT Hebrew Text Font Name", "Kethib/Qere Text Font Name", "",
                                          "LXX English Text Font Name", "LXX Greek Text Font Name", "", "",
                                          "Parse Area Font Name", "Parse Title Font Name", "", "",
                                          "Lexical Area Font Name", "Lexical Title Font Name", "", "",
                                          "Search Results English Font Name MT", "Search Results Hebrew Font Name MT", "Search Primary Font Name MT", "Search Secondary Font Name MT",
                                          "Search Results English Font Name LXX", "Search Results Hebrew Font Name LXX", "Search Primary Font Name LXX", "Search Secondary Font Name LXX",
                                          "MT Notes Font Name", "", "", "", 
                                          "LXX Notes Font Name", "", "", "" };
        String[] keyNames = { "Base Directory", "MT Titles", "LXX Titles", "MT Text Source", "LXX Source Folder",
                "MT Notes Folder", "LXX Notes Folder", "MT Notes Name", "LXX Notes Name", "Help File" };
        RegistryKey baseKey;
        Form mainForm;

        classGlobal globalVars;

        public Form MainForm { get => mainForm; set => mainForm = value; }
        public int DefaultSplitPosition { get => defaultSplitPosition; set => defaultSplitPosition = value; }
        public int DefaultState { get => defaultState; set => defaultState = value; }
        public int DefaultHeight { get => defaultHeight; set => defaultHeight = value; }
        public int DefaultWidth { get => defaultWidth; set => defaultWidth = value; }
        public int DefaultX { get => defaultX; set => defaultX = value; }
        public int DefaultY { get => defaultY; set => defaultY = value; }
        public string KeyString { get => keyString; set => keyString = value; }

        public void initialiseRegistry(classGlobal inConfig, String inKey)
        {
            globalVars = inConfig;
            keyString = inKey;
            openRegistry();
            initialisePaths();
        }

        public void openRegistry()
        {
            baseKey = Registry.CurrentUser.OpenSubKey(keyString, true);
        }

        public void closeKeys()
        {
            baseKey.Close();
            baseKey.Dispose();
        }

        private void initialisePaths()
        {
            /*===================================================================================*
             *                                                                                   *
             *   Manage File location details                                                    *
             *   ----------------------------                                                    *
             *                                                                                   *
             * Note: The actual directory names (Notes, Source) are "hard coded"                 *
             *                                                                                   *
             *===================================================================================*/

            String registryString;
            String baseDirectory;
            String mtTitlesFile = "Titles.txt", lxxTitlesFile = "LXX_Titles.txt";
            String mtSourceFile = "OTText.txt", lxxTextFolder = "LXX_Text";
            String notesPath = "Notes", mtNotesFolder = "MT", lxxNotesFolder = "LXX", mtNotesName = "Default", lxxNotesName = "Default", fullMTNotesName, fullLXXNotesName;
            String helpPath = "Help", helpFile = "Help.html";
            String lexiconFile = "BDB.csv", convertFile = "WordToBdb.txt", codeFile = "Codes.txt", kethibQereFile = "Kethib_Qere.txt";
            String gkLexiconFolder = "GkLexicon", mainLexFile = "LandSSummary.txt";

            String keyboardFolder = "Keyboard", greekControlFolder = "Greek";
            String gkAccute = "accuteAccents.txt", gkCircumflex = "circumflexAccents.txt", gkDiaereses = "diaereses.txt", gkGrave = "graveAccents.txt",
                gkIota = "iotaSubscripts.txt", gkRough = "roughBreathings.txt", gkSmooth = "smoothBreathings.txt",
                gkConv1 = "breathingConversion1.txt", gkConv2 = "breathingConversion2.txt";

            String fullMTTitleFile, fullLXXTitleFile, fullMTSourceFile, fullLXXTextFolder, fullMTNotesPath, fullLXXNotesPath, fullHelpFile, fileBuffer;
            StreamReader srCaution;

            isInitialising = true;
            // Now get file location details from the registry - and create if they don't exist
            if (baseKey == null)
            {
                // Go here if the base registry key doesn't exist
                baseDirectory = Path.GetFullPath(@"..\Source");
                globalVars.FullMTTitleFile = baseDirectory + @"\" + mtTitlesFile;
                globalVars.FullLXXTitleFile = baseDirectory + @"\" + lxxTitlesFile;
                globalVars.FullMTSourceFile = baseDirectory + @"\" + mtSourceFile;
                globalVars.FullLXXTextFolder = baseDirectory + @"\" + lxxTextFolder;
                globalVars.FullMTNotesPath = baseDirectory + @"\" + notesPath + @"\" + mtNotesFolder;
                globalVars.FullLXXNotesPath = baseDirectory + @"\" + notesPath + @"\" + lxxNotesFolder;
                globalVars.MtNotesName = mtNotesName;
                globalVars.LxxNotesName = lxxNotesName;
                globalVars.FullHelpFile = baseDirectory + @"\" + helpPath + @"\" + helpFile;

                baseKey = Registry.CurrentUser.CreateSubKey(keyString);
                baseKey.SetValue(keyNames[0], baseDirectory, RegistryValueKind.String);
                baseKey.SetValue(keyNames[1], globalVars.FullMTTitleFile, RegistryValueKind.String);
                baseKey.SetValue(keyNames[2], globalVars.FullLXXTitleFile, RegistryValueKind.String);
                baseKey.SetValue(keyNames[3], globalVars.FullMTSourceFile, RegistryValueKind.String);
                baseKey.SetValue(keyNames[4], globalVars.FullLXXTextFolder, RegistryValueKind.String);
                baseKey.SetValue(keyNames[5], globalVars.FullMTNotesPath, RegistryValueKind.String);
                baseKey.SetValue(keyNames[6], globalVars.FullLXXNotesPath, RegistryValueKind.String);
                baseKey.SetValue(keyNames[7], globalVars.MtNotesName, RegistryValueKind.String);
                baseKey.SetValue(keyNames[8], globalVars.LxxNotesName, RegistryValueKind.String);
                baseKey.SetValue(keyNames[9], globalVars.FullHelpFile, RegistryValueKind.String);
            }
            else
            {
                registryString = registrySetting(keyNames[0], globalVars.BaseDirectory);
                if (registryString.Length > 0)
                {
                    baseDirectory = registryString;
                }
                else
                {
                    baseDirectory = Path.GetFullPath(@"..\Source");
                    baseKey.SetValue(keyNames[0], baseDirectory, RegistryValueKind.String);
                }
                globalVars.BaseDirectory = baseDirectory;
                fullMTTitleFile = baseDirectory + @"\" + mtTitlesFile;
                registryString = registrySetting(keyNames[1], fullMTTitleFile);
                if (registryString.Length > 0)
                {
                    fullMTTitleFile = registryString;
                }
                else baseKey.SetValue(keyNames[1], fullMTTitleFile, RegistryValueKind.String);
                globalVars.FullMTTitleFile = fullMTTitleFile;
                fullLXXTitleFile = baseDirectory + @"\" + lxxTitlesFile;
                registryString = registrySetting(keyNames[2], fullLXXTitleFile);
                if (registryString.Length > 0)
                {
                    fullLXXTitleFile = registryString;
                }
                else baseKey.SetValue(keyNames[2], fullLXXTitleFile, RegistryValueKind.String);
                globalVars.FullLXXTitleFile = fullLXXTitleFile;
                fullMTSourceFile = baseDirectory + @"\" + mtSourceFile;
                registryString = registrySetting(keyNames[3], fullMTSourceFile);
                if (registryString.Length > 0)
                {
                    fullMTSourceFile = registryString;
                }
                else baseKey.SetValue(keyNames[3], fullMTSourceFile, RegistryValueKind.String);
                globalVars.FullMTSourceFile = fullMTSourceFile;
                fullLXXTextFolder = baseDirectory + @"\" + lxxTextFolder;
                registryString = registrySetting(keyNames[4], fullLXXTextFolder);
                if (registryString.Length > 0)
                {
                    fullLXXTextFolder = registryString;
                }
                else baseKey.SetValue(keyNames[4], fullLXXTextFolder, RegistryValueKind.String);
                globalVars.FullLXXTextFolder = fullLXXTextFolder;
                fullMTNotesPath = baseDirectory + @"\" + notesPath + @"\" + mtNotesFolder;
                registryString = registrySetting(keyNames[5], fullMTNotesPath);
                if (registryString.Length > 0)
                {
                    fullMTNotesPath = registryString;
                }
                else baseKey.SetValue(keyNames[5], fullMTNotesPath, RegistryValueKind.String);
                globalVars.FullMTNotesPath = fullMTNotesPath;
                fullLXXNotesPath = baseDirectory + @"\" + notesPath + @"\" + lxxNotesFolder;
                registryString = registrySetting(keyNames[6], fullLXXNotesPath);
                if (registryString.Length > 0)
                {
                    fullLXXNotesPath = registryString;
                }
                else baseKey.SetValue(keyNames[6], fullLXXNotesPath, RegistryValueKind.String);
                globalVars.FullLXXNotesPath = fullLXXNotesPath;
                fullMTNotesName = lxxNotesName;
                registryString = registrySetting(keyNames[7], mtNotesName);
                if (registryString.Length > 0)
                {
                    mtNotesName = registryString;
                }
                globalVars.MtNotesName = mtNotesName;
                registryString = registrySetting(keyNames[8], lxxNotesName);
                if (registryString.Length > 0)
                {
                    lxxNotesName = registryString;
                }
                globalVars.LxxNotesName = lxxNotesName;
                fullHelpFile = baseDirectory + @"\" + helpPath + @"\" + helpFile;
                registryString = registrySetting(keyNames[9], fullHelpFile);
                if (registryString.Length > 0)
                {
                    fullHelpFile = registryString;
                }
                globalVars.FullHelpFile = fullHelpFile;
            }
            globalVars.FullLexiconFile = baseDirectory + @"\" + lexiconFile;
            globalVars.FullGkLexiconFolder = baseDirectory + @"\" + gkLexiconFolder;
            globalVars.FullGkLexiconFile = globalVars.FullGkLexiconFolder + @"\" + mainLexFile;
            globalVars.FullConvertFile = baseDirectory + @"\" + convertFile;
            globalVars.FullKethibQereFile = baseDirectory + @"\" + kethibQereFile;
            globalVars.FullCodeFile = baseDirectory + @"\" + codeFile;
            globalVars.FullHelpFile = baseDirectory + @"\" + helpPath + @"\" + helpFile;
            globalVars.FullKeyboardFolder = baseDirectory + @"\" + keyboardFolder;
            globalVars.FullGkAccute = baseDirectory + @"\" + greekControlFolder + @"\" + gkAccute;
            globalVars.FullGkCircumflex = baseDirectory + @"\" + greekControlFolder + @"\" + gkCircumflex;
            globalVars.FullGkDiaereses = baseDirectory + @"\" + greekControlFolder + @"\" + gkDiaereses;
            globalVars.FullGkGrave = baseDirectory + @"\" + greekControlFolder + @"\" + gkGrave;
            globalVars.FullGkIota = baseDirectory + @"\" + greekControlFolder + @"\" + gkIota;
            globalVars.FullGkRough = baseDirectory + @"\" + greekControlFolder + @"\" + gkRough;
            globalVars.FullGkSmooth = baseDirectory + @"\" + greekControlFolder + @"\" + gkSmooth;
            globalVars.FullGkConv1 = baseDirectory + @"\" + greekControlFolder + @"\" + gkConv1;
            globalVars.FullGkConv2 = baseDirectory + @"\" + greekControlFolder + @"\" + gkConv2;
            if (!Directory.Exists(globalVars.FullMTNotesPath)) Directory.CreateDirectory(globalVars.FullMTNotesPath);
            if (!Directory.Exists(globalVars.FullLXXNotesPath)) Directory.CreateDirectory(globalVars.FullLXXNotesPath);

            /*---------------------------------------------------------------------------------------------*
             *                                                                                             *
             *  What follows is quite fiddly and not very elegant.  It is the result of                    *
             *  a) the facility to move the application data (see code for reasons), and                   *
             *  b) the fact that some elements are used by the application when it loads -                 *
             *       which means we had to find a mechanism to defer the deletion until the next time the  *
             *       application is loaded, when it will be using data from the new location and the old   *
             *       one can now be removed.                                                               *
             *                                                                                             *
             *---------------------------------------------------------------------------------------------*/

            if (File.Exists(globalVars.BaseDirectory + @"\" + cautionFileName))
            {
                srCaution = new StreamReader(globalVars.BaseDirectory + @"\" + cautionFileName);
                fileBuffer = srCaution.ReadToEnd();
                srCaution.Close();
                srCaution.Dispose();
                Directory.Delete(fileBuffer, true);
                File.Delete(globalVars.BaseDirectory + @"\" + cautionFileName);
            }
            isInitialising = false;
        }

        private String registrySetting(String keyName, String fromRegistry)
        {
            String outcomeString;
            Object regValue;

            regValue = baseKey.GetValue(keyName);
            if (regValue != null)
            {
                outcomeString = regValue.ToString();
                return outcomeString;
            }
            else
            {
                baseKey.SetValue(keyName, fromRegistry, RegistryValueKind.String);
                return "";
            }
        }

        public void initialiseWindowDetails()
        {
            /*===================================================================================*
             *                                                                                   *
             * Step 2: Some window parameters                                                    *
             * ------------------------------                                                    *
             *                                                                                   *
             *===================================================================================*/
            String[] windowDetails = { "Window Position X", "Window Position Y", "Window Width", "Window Height", "Window State", "Main Splitter Distance" };
            String[] historySettings = { "Maximum History Entries" };
            String[] colourSettings = { };

            Object regValue;
            SplitContainer splitMain;
            Screen mainScreen = Screen.PrimaryScreen;

            isInitialising = true;
            regValue = baseKey.GetValue(windowDetails[0]);
            if (regValue == null)
            {
                baseKey.SetValue(windowDetails[0], defaultX, RegistryValueKind.DWord);
            }
            else
            {
                defaultX = (int)regValue;
            }
            globalVars.WindowX = defaultX;
            regValue = baseKey.GetValue(windowDetails[1]);
            if (regValue == null)
            {
                baseKey.SetValue(windowDetails[1], defaultY, RegistryValueKind.DWord);
            }
            else
            {
                defaultY = (int)regValue;
            }
            globalVars.WindowY = defaultY;
            regValue = baseKey.GetValue(windowDetails[2]);
            if (regValue == null)
            {
                baseKey.SetValue(windowDetails[2], defaultWidth, RegistryValueKind.DWord);
            }
            else
            {
                defaultWidth = (int)regValue;
            }
            globalVars.WindowWidth = defaultWidth;
            regValue = baseKey.GetValue(windowDetails[3]);
            if (regValue == null)
            {
                baseKey.SetValue(windowDetails[3], defaultHeight, RegistryValueKind.DWord);
            }
            else
            {
                defaultHeight = (int)regValue;
            }
            globalVars.WindowHeight = defaultHeight;
            regValue = baseKey.GetValue(windowDetails[4]);
            if (regValue == null)
            {
                baseKey.SetValue(windowDetails[4], defaultState, RegistryValueKind.DWord);
            }
            else
            {
                defaultState = (int)regValue;
            }
            globalVars.WindowState = defaultState;
            regValue = baseKey.GetValue(windowDetails[5]);
            splitMain = (SplitContainer)globalVars.getGroupedControl( globalVars.SplitContainerCode, 0 );
            if (regValue == null)
            {
                baseKey.SetValue(windowDetails[5], defaultSplitPosition, RegistryValueKind.DWord);
            }
            else
            {
                defaultSplitPosition = (int)regValue;
            }
            regValue = baseKey.GetValue(historySettings[0]);
            if (regValue == null)
            {
                baseKey.SetValue(historySettings[0], globalVars.HistoryMax, RegistryValueKind.DWord);
            }
            else
            {
                globalVars.HistoryMax = (int)regValue;
            }
            isInitialising = false;
        }

        private void processColour(String registryName, Color defaultColour, int globalIndex, int colourType)
        {
            /*==================================================================================================*
             *                                                                                                  *
             *                                        processColour                                             *
             *                                        =============                                             *
             *                                                                                                  *
             *  A method for the generic processing of colours.                                                 *
             *                                                                                                  *
             *  Parameters:                                                                                     *
             *  ==========                                                                                      *
             *                                                                                                  *
             *  registryName   The name of the subKey                                                           *
             *  defaultColour  Used if not found in the registry                                                *
             *                                                                                                  *
             *  globalIndex:     Value              Refers to                                                   *
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
             *  colourType       Value              Refers to                                                   *
             *                   -----              ---------                                                   *
             *                     0           Background colour                                                *
             *                     1           Main text colour                                                 *
             *                     2           Alternative text colour                                          *
             *                     3           Primary alternative text colour                                  *
             *                     4           Second alternative text colour                                   *
             *                                                                                                  *
             *==================================================================================================*/

            Object regValue;
            Color currentColour;

            regValue = baseKey.GetValue(registryName);
            if (regValue == null)
            {
                currentColour = defaultColour;
                baseKey.SetValue(registryName, currentColour.ToArgb(), RegistryValueKind.DWord);
            }
            else
            {
                currentColour = Color.FromArgb((int)regValue);
            }
            globalVars.addColourSetting(globalIndex, currentColour, colourType);

        }

        private void processFontSize(String registryName, float defaultSize, int globalIndex, int fontType)
        {
            /*==================================================================================================*
             *                                                                                                  *
             *                                       processFontSize                                            *
             *                                       ===============                                            *
             *                                                                                                  *
             *  A method for the generic processing of font sizes.                                              *
             *                                                                                                  *
             *  Parameters:                                                                                     *
             *  ==========                                                                                      *
             *                                                                                                  *
             *  registryName   The name of the subKey                                                           *
             *  defaultSize    Used if not found in the registry                                                *
             *                                                                                                  *
             *  globalIndex:     Value              Refers to                                                   *
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
             *  fontType         Value              Refers to                                                   *
             *                   -----              ---------                                                   *
             *                     1            Size of main text (normally English)                            *
             *                     2            Size of alternative text (normally Hebrew or, perhaps, Greek)   *
             *                     3            Size of primary match word(s)                                   *
             *                     4            Size of secondary match word(s)                                 *
             *                                                                                                  *
             *==================================================================================================*/

            Object regValue;
            float currentFontSize;

            if (registryName.Length == 0) return;
            regValue = baseKey.GetValue(registryName);
            if (regValue == null)
            {
                currentFontSize = defaultSize;
                baseKey.SetValue(registryName, currentFontSize, RegistryValueKind.DWord);
            }
            else
            {
                currentFontSize = (float)Convert.ToDecimal(regValue.ToString());
            }
            globalVars.addTextSize(globalIndex, currentFontSize, fontType);

        }

        private void manageFontStyle(String registryName, String defaultStyle, int globalIndex, int styleType)
        {
            /*==================================================================================================*
             *                                                                                                  *
             *                                       manageFontStyle                                            *
             *                                       ===============                                            *
             *                                                                                                  *
             *  Can be any .Net recognised text style (e.g. bold, italics)                                      *
             *                                                                                                  *
             *  Parameters:                                                                                     *
             *  ==========                                                                                      *
             *                                                                                                  *
             *  registryName   The name of the subKey                                                           *
             *  defaultStyle   Used if not found in the registry                                                *
             *                                                                                                  *
             *  globalIndex:     Value              Refers to                                                   *
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
             *  fontType         Value              Refers to                                                   *
             *                   -----              ---------                                                   *
             *                     1            English text style                                              *
             *                     2            Foreign text style                                              *
             *                     3            Alternative style (e.g. Kethib/Qere or word searched for)       *
             *                     4            Second alternative style (used for search results)              *
             *                                                                                                  *
             *==================================================================================================*/

            Object regValue;
            String currentStyle;

            if (registryName.Length == 0) return;
            regValue = baseKey.GetValue(registryName);
            if (regValue == null)
            {
                currentStyle = defaultStyle;
                baseKey.SetValue(registryName, currentStyle, RegistryValueKind.String);
            }
            else
            {
                currentStyle = regValue.ToString();
            }
            globalVars.addDefinedStyle(globalIndex, currentStyle, styleType);
        }

        private void manageFontName(String registryName, String defaultName, int globalIndex, int fontType)
        {
            /*==================================================================================================*
             *                                                                                                  *
             *                                       manageFontName                                             *
             *                                       ==============                                             *
             *                                                                                                  *
             *  Parameters:                                                                                     *
             *  ==========                                                                                      *
             *                                                                                                  *
             *  registryName   The name of the subKey                                                           *
             *  defaultName    Used if not found in the registry                                                *
             *                                                                                                  *
             *  globalIndex:     Value              Refers to                                                   *
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
             *  fontType         Value              Refers to                                                   *
             *                   -----              ---------                                                   *
             *                     1            English text                                                    *
             *                     2            Foreign text                                                    *
             *                     3            Alternative text                                                *
             *                     4            Second alternative text                                         *
             *                                                                                                  *
             *==================================================================================================*/

            Object regValue;
            String currentName;

            if (registryName.Length == 0) return;
            regValue = baseKey.GetValue(registryName);   // *** Font name ***
            if (regValue == null)
            {
                currentName = defaultName;
                baseKey.SetValue(registryName, currentName, RegistryValueKind.String);
            }
            else
            {
                currentName = regValue.ToString();
            }
            globalVars.addFontName(globalIndex, currentName, fontType);
        }

        public void initialiseFontsAndColour()
        {
            /*************************************************************************************
             *                                                                                   *
             *   Initialisation of Font colours and sizes and rich text area background colours  *
             *                                                                                   *
             *************************************************************************************/

            int idx, jdx, index;

            isInitialising = true;

            /*.......................................................................*
             *                                                                       *
             *                                 Colours                               *
             *                                 -------                               *
             *                                                                       *
             *  idx   each area (main MT text, main LXX text, etc)                   *
             *  jdx   each colour (background, non-Heb/Gk, Heb/Gk text, etc.         *
             *                                                                       *
             *.......................................................................*/

            processColour(keyColourNames[0], Color.White, 0, 0);
            processColour(keyColourNames[5], Color.White, 1, 0);
            processColour(keyColourNames[10], Color.White, 2, 0);
            processColour(keyColourNames[15], Color.White, 3, 0);
            processColour(keyColourNames[20], Color.White, 4, 0);
            processColour(keyColourNames[25], Color.White, 5, 0);
            processColour(keyColourNames[30], Color.White, 6, 0);
            processColour(keyColourNames[35], Color.White, 7, 0);

            processColour(keyColourNames[1], Color.Black, 0, 1);
            processColour(keyColourNames[6], Color.Black, 1, 1);
            processColour(keyColourNames[11], Color.Black, 2, 1);
            processColour(keyColourNames[16], Color.Black, 3, 1);
            processColour(keyColourNames[21], Color.Black, 4, 1);
            processColour(keyColourNames[26], Color.Black, 5, 1);
            processColour(keyColourNames[31], Color.Black, 6, 1);
            processColour(keyColourNames[36], Color.Black, 7, 1);

            processColour(keyColourNames[2], Color.Black, 0, 2);
            processColour(keyColourNames[7], Color.Black, 1, 2);
            processColour(keyColourNames[12], Color.Black, 2, 2);
            processColour(keyColourNames[17], Color.Black, 3, 2);
            processColour(keyColourNames[22], Color.Black, 4, 2);
            processColour(keyColourNames[27], Color.Black, 5, 2);

            processColour(keyColourNames[3], Color.Red, 0, 3);
            processColour(keyColourNames[23], Color.Red, 4, 3);
            processColour(keyColourNames[28], Color.Red, 5, 3);

            processColour(keyColourNames[24], Color.Blue, 4, 4);
            processColour(keyColourNames[29], Color.Blue, 5, 4);

            /*.......................................................................*
             *                                                                       *
             *                                Font Sizes                             *
             *                                ----------                             *
             *                                                                       *
             *  idx   each area (main MT text, main LXX text, etc)                   *
             *  jdx   each colour (background, non-Heb/Gk, Heb/Gk text, etc.         *
             *                                                                       *
             *.......................................................................*/

            processFontSize(keyFontSizeNames[0], 12F, 0, 1);
            processFontSize(keyFontSizeNames[4], 12F, 1, 1);
            processFontSize(keyFontSizeNames[8], 12F, 2, 1);
            processFontSize(keyFontSizeNames[12], 12F, 3, 1);
            processFontSize(keyFontSizeNames[16], 12F, 4, 1);
            processFontSize(keyFontSizeNames[20], 12F, 5, 1);
            processFontSize(keyFontSizeNames[24], 12F, 6, 1);
            processFontSize(keyFontSizeNames[28], 12F, 7, 1);

            processFontSize(keyFontSizeNames[1], 18F, 0, 2);
            processFontSize(keyFontSizeNames[5], 12F, 1, 2);
            processFontSize(keyFontSizeNames[9], 18F, 2, 2);
            processFontSize(keyFontSizeNames[13], 18F, 3, 2);
            processFontSize(keyFontSizeNames[17], 18F, 4, 2);
            processFontSize(keyFontSizeNames[21], 12F, 5, 2);

            processFontSize(keyFontSizeNames[2], 18F, 0, 3);
            processFontSize(keyFontSizeNames[18], 18F, 4, 3);
            processFontSize(keyFontSizeNames[22], 12F, 5, 3);

            processFontSize(keyFontSizeNames[19], 18F, 4, 4);
            processFontSize(keyFontSizeNames[23], 12F, 5, 4);

            /*.......................................................................*
             *                                                                       *
             *                          Font Style Names                             *
             *                          ----------------                             *
             *                                                                       *
             *  idx   each area (main MT text, main LXX text, etc)                   *
             *  jdx   each colour (background, non-Heb/Gk, Heb/Gk text, etc.         *
             *                                                                       *
             *.......................................................................*/

            index = 0;
            for (idx = 0; idx < noOfAreas; idx++)
            {
                for (jdx = 1; jdx < 5; jdx++)
                {
                    manageFontStyle(keyFontStyleNames[index++], "Regular", idx, jdx); 
                }
            }

            /*.......................................................................*
             *                                                                       *
             *                          Font Family Names                            *
             *                          -----------------                            *
             *                                                                       *
             *  idx   each area (main MT text, main LXX text, etc)                   *
             *  jdx   each colour (background, non-Heb/Gk, Heb/Gk text, etc.         *
             *                                                                       *
             *.......................................................................*/

            index = 0;
            for (idx = 0; idx < noOfAreas; idx++)
            {
                for (jdx = 1; jdx < 5; jdx++)
                {
                    manageFontName(keyFontNameNames[index++], "Times New Roman", idx, jdx);
                }
            }

            isInitialising = false;
        }

        public void updateFontsAndColour()
        {
            int idx;

            for (idx = 0; idx < noOfAreas; idx++)
            {
                updateSingleAreaFontsAndColour(idx);
            }
        }

        public void updateSingleAreaFontsAndColour( int areaCode)
        {
            int jdx, index, indexBase;

            indexBase = areaCode * 5;
            index = indexBase;
            openRegistry();
            for (jdx = 0; jdx < 5; jdx++)
            {
                if (keyColourNames[index].Length > 0) baseKey.SetValue(keyColourNames[index], globalVars.getColourSetting(areaCode, jdx).ToArgb(), RegistryValueKind.DWord);
                index++;
            }
            index = indexBase;
            for (jdx = 1; jdx < 5; jdx++)
            {
                if (keyFontSizeNames[index].Length > 0) baseKey.SetValue(keyFontSizeNames[index], globalVars.getTextSize(areaCode, jdx), RegistryValueKind.DWord);
                index++;
            }
            index = indexBase;
            for (jdx = 1; jdx < 5; jdx++)
            {
                if (keyFontStyleNames[index].Length > 0) baseKey.SetValue(keyFontStyleNames[index], globalVars.getDefinedStyleByIndex(areaCode, jdx), RegistryValueKind.String);
                index++;
            }
            index = indexBase;
            for (jdx = 1; jdx < 5; jdx++)
            {
                if (keyFontNameNames[index].Length > 0) baseKey.SetValue(keyFontNameNames[index], globalVars.getDefinedFontNameByIndex(areaCode, jdx), RegistryValueKind.String);
                index++;
            }
        }

        public void updateSplitterDistance()
        {
            int newSplitterDistance;
            SplitContainer targetSplitter;

            if (isInitialising) return;
            targetSplitter = (SplitContainer)globalVars.getGroupedControl( globalVars.SplitContainerCode, 0);
            newSplitterDistance = targetSplitter.SplitterDistance;
            openRegistry();
            baseKey.SetValue("Main Splitter Distance", newSplitterDistance, RegistryValueKind.DWord);
            closeKeys();
            globalVars.SplitPstn = newSplitterDistance;
        }

        public void updateWindowPosition()
        {
            int windowX, windowY;
            FormWindowState windowState;
            Form targetForm;

            if (isInitialising) return;
            targetForm = globalVars.MasterForm;
            windowX = targetForm.Left;
            windowY = targetForm.Top;
            windowState = targetForm.WindowState;
            if ((windowState == FormWindowState.Normal) || (windowState == FormWindowState.Minimized))
            {
                openRegistry();
                baseKey.SetValue("Window Position X", windowX, RegistryValueKind.DWord);
                baseKey.SetValue("Window Position Y", windowY, RegistryValueKind.DWord);
                closeKeys();
                globalVars.WindowX = windowX;
                globalVars.WindowY = windowY;
            }
        }

        public void updateWindowSize()
        {
            int windowWidth, windowHeight;
            FormWindowState windowState;
            Form targetForm;

            if (isInitialising) return;
            targetForm = globalVars.MasterForm;
            windowWidth = targetForm.Width;
            windowHeight = targetForm.Height;
            windowState = targetForm.WindowState;
            openRegistry();
            if ((windowState == FormWindowState.Normal) || (windowState == FormWindowState.Minimized))
            {
                baseKey.SetValue("Window Width", windowWidth, RegistryValueKind.DWord);
                baseKey.SetValue("Window Height", windowHeight, RegistryValueKind.DWord);
                globalVars.WindowWidth = windowWidth;
                globalVars.WindowHeight = windowHeight;
            }
            baseKey.SetValue("Window State", windowState, RegistryValueKind.DWord);
            closeKeys();
            globalVars.WindowState = (int)windowState;
        }

        public void updateNotesSet()
        {
/*            if (isInitialising) return;
            openRegistry();
            baseKey.SetValue("Notes Name", globalVars.NotesName, RegistryValueKind.String);
            closeKeys(); */
        }

        public bool relocateFiles(String newLocation)
        {
            String originalRoot;
            DirectoryInfo diSource, diTarget;
            StreamWriter swCaution;

            originalRoot = globalVars.BaseDirectory;
            if (!Directory.Exists(newLocation)) Directory.CreateDirectory(newLocation);
            // Make sure the target directory is empty
            diTarget = new DirectoryInfo(newLocation);
            try
            {
                foreach (DirectoryInfo diName in diTarget.GetDirectories()) diName.Delete(true);
            }
            catch
            {
                MessageBox.Show("You are not able to move the files to that location", "Application Files Relocation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            foreach (FileInfo fiFile in diTarget.GetFiles()) fiFile.Delete();
            // Now copy data from the current location
            diSource = new DirectoryInfo(originalRoot);
            foreach (DirectoryInfo diName in diSource.GetDirectories()) cloneSingleDirectory(diTarget, diName);
            // Now get any files in the current directory
            foreach (FileInfo fiFile in diSource.GetFiles()) fiFile.CopyTo(diTarget.FullName + @"\" + fiFile.Name);
            globalVars.BaseDirectory = newLocation;
            openRegistry();
            if (baseKey != null) baseKey.SetValue("Base Directory", newLocation, RegistryValueKind.String);
            globalVars.FullMTTitleFile = revampRegString(globalVars.FullMTTitleFile, 1);
            globalVars.FullLXXTitleFile = revampRegString(globalVars.FullLXXTitleFile, 1);
            globalVars.FullMTSourceFile = revampRegString(globalVars.FullMTSourceFile, 1);
            globalVars.FullLXXTextFolder = revampRegString(globalVars.FullLXXTextFolder, 1);
            globalVars.FullMTNotesPath = revampRegString(globalVars.FullMTNotesPath, 2);
            globalVars.FullLXXNotesPath = revampRegString(globalVars.FullLXXNotesPath, 2);
            globalVars.FullHelpFile = revampRegString(globalVars.FullHelpFile, 2);
            baseKey.SetValue(keyNames[1], globalVars.FullMTTitleFile, RegistryValueKind.String);
            baseKey.SetValue(keyNames[2], globalVars.FullLXXTitleFile, RegistryValueKind.String);
            baseKey.SetValue(keyNames[3], globalVars.FullMTSourceFile, RegistryValueKind.String);
            baseKey.SetValue(keyNames[4], globalVars.FullLXXTextFolder, RegistryValueKind.String);
            baseKey.SetValue(keyNames[5], globalVars.FullMTNotesPath, RegistryValueKind.String);
            baseKey.SetValue(keyNames[6], globalVars.FullLXXNotesPath, RegistryValueKind.String);
            baseKey.SetValue(keyNames[9], globalVars.FullHelpFile, RegistryValueKind.String);


            closeKeys();
            try
            {
                Directory.Delete(originalRoot, true);
            }
            catch
            {
                swCaution = new StreamWriter(newLocation + @"\" + cautionFileName);
                swCaution.Write(originalRoot);
                swCaution.Close();
            }
            return true;
        }

        public void cloneSingleDirectory(DirectoryInfo diTargetDirectory, DirectoryInfo diSourceDirectory)
        {
            DirectoryInfo diTarget;

            // Create the new directry
            if (!Directory.Exists(diTargetDirectory.FullName + @"\" + diSourceDirectory.Name)) Directory.CreateDirectory(diTargetDirectory.FullName + @"\" + diSourceDirectory.Name);
            // Get info for the new directory
            diTarget = new DirectoryInfo(diTargetDirectory.FullName + @"\" + diSourceDirectory.Name);
            // Now go down a level and do the same
            foreach (DirectoryInfo diName in diSourceDirectory.GetDirectories()) cloneSingleDirectory(diTarget, diName);
            // Now get any files in the current directoy
            foreach (FileInfo fiFile in diSourceDirectory.GetFiles()) fiFile.CopyTo(diTargetDirectory.FullName + @"\" + diSourceDirectory.Name + @"\" + fiFile.Name);
        }

        private String revampRegString( String oldString, int noOfLevels)
        {
            int nPstn;
            String fileName;

            nPstn = oldString.LastIndexOf('\\');
            if (noOfLevels > 1) nPstn = oldString.LastIndexOf('\\', nPstn - 1);
            fileName = oldString.Substring(nPstn + 1);
            return globalVars.BaseDirectory + @"\" + fileName;
        }
    }
}
