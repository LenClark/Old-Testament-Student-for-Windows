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
    public class classGkLexicon
    {
        /*=======================================================================================================*
         *                                                                                                       *
         *                                          classGkLexicon                                               *
         *                                          ==============                                               *
         *                                                                                                       *
         *  The Liddell & Scott Intermediate Lexicon forms the heart of this class.  However, we will also use   *
         *    it for:                                                                                            *
         *    a) parse related activity, and                                                                     *
         *    b) processing related to the lexicon - notably, the appendices                                     *
         *                                                                                                       *
         *=======================================================================================================*/

        classGlobal globalVars;
        classGreekOrthography greekProcessing;

        /*--------------------------------------------------------------------------------------------*
         *                                                                                            *
         *  lexiconEntry stores word and meaning:                                                     *
         *                                                                                            *
         *  Key:   the root Greek word                                                                *
         *  Value: the full meaning                                                                   *
         *                                                                                            *
         *--------------------------------------------------------------------------------------------*/

        Dictionary<String, String> lexiconEntry;
        Dictionary<String, classGkLexiconExtras> UnaccentedLookup = new Dictionary<string, classGkLexiconExtras>();
        SortedList<String, String> alternativeCharacters = new SortedList<string, string>();

        private delegate void performWebBrowserUpdate(WebBrowser targetBrowser, String fileName);

        private void updateWebBrowser( WebBrowser targetBrowser, String fileName)
        {
            if (fileName.Contains(@"\\")) fileName = fileName.Replace(@"\\", @"\");
            targetBrowser.Navigate(fileName);
        }

        public void initialiseLexicon(classGlobal inConfig, classGreekOrthography inGkProcs)
        {
            bool doesExist = false;
            int idx, wordLength, letterValue;
            Char firstInString, secondInString, finalChar;
            String lexBuffer, keyWord, altWord, lexMeaning, retrievedText, flatWord = "";
            String[] problemChars = { "ά", "έ", "ή", "ί", "ό", "ύ", "ώ", "Ά", "Έ", "Ή", "Ί", "Ό", "Ύ", "Ώ", "ΐ", "ΰ" };
            String[] replacementChars = { "ά", "έ", "ή", "ί", "ό", "ύ", "ώ", "Ά", "Έ", "Ή", "Ί", "Ό", "Ύ", "Ώ", "ΐ", "ΰ" };
            StreamReader srLexicon;
            classGkLexiconExtras lexiconExtras;

            globalVars = inConfig;
            greekProcessing = inGkProcs;
            lexiconEntry = new Dictionary<string, string>();
            lexMeaning = "";
            keyWord = "";
            altWord = "";
            // Replace characters that have two forms un the Unicode specification
            wordLength = problemChars.Length;
            for (idx = 0; idx < wordLength; idx++) alternativeCharacters.Add(problemChars[idx], replacementChars[idx]);
            // Get the lexicon data
            srLexicon = new StreamReader(globalVars.FullGkLexiconFile);
            lexBuffer = srLexicon.ReadLine();
            while (lexBuffer != null)
            {
                if (lexBuffer.Trim().Length == 0) { lexBuffer = srLexicon.ReadLine(); continue; }
                firstInString = (lexBuffer.TrimStart())[0];
                switch (firstInString)
                {
                    case '+':
                        // This is used as a key for the entry.
                        // Note that there are some entries that are identical but with two seperate entries (e.g. ἀγός and ἄγος1)
                        // Note also there are values for this field that vary only in accent (e.g. ἄγη1 and ἀγή1)
                        altWord = lexBuffer.Substring(1, lexBuffer.Length - 1);
                        break; // Discard because includes wierd additions
                    case '=':
                        // This contains the word which is actually the lexicon entry
                        if (lexBuffer.Length == 1)
                        {
                            keyWord = altWord;
                            lexBuffer = srLexicon.ReadLine();
                            continue;
                        }
                        keyWord = lexBuffer.Substring(1, lexBuffer.Length - 1);
                        finalChar = keyWord[keyWord.Length - 1];
                        letterValue = finalChar;
                        // The keyWord may contain a digit; in which case, remove it
                        if ((letterValue >= 0x30) && (letterValue <= 0x39))
                        {
                            keyWord = keyWord.Substring(0, keyWord.Length - 1);
                        }
                        // Do we already have an entry
                        if (lexiconEntry.ContainsKey(keyWord))
                        {
                            doesExist = true;
                        }
                        else
                        {
                            doesExist = false;
                        }
                        break;
                    case ';':
                        // This adds the version of the word with no accents, etc.
                        // It follows the keyWord entry and uses the same logic
                        flatWord = lexBuffer.Substring(1, lexBuffer.Length - 1);
                        break;
                    case '>':
                        // This is used for equivalence references
                        secondInString = (lexBuffer.TrimStart())[1];
                        lexBuffer = lexBuffer.TrimStart().Substring(2, lexBuffer.Length - 2).Trim();
                        switch (secondInString)
                        {
                            case '1':
                                // There is a related word (and no specified meaning for this version
                                lexMeaning = "Related word: " + lexBuffer + "\n";
                                wordLength = lexBuffer.Length + 14;
                                for (idx = 0; idx < wordLength; idx++)
                                {
                                    lexMeaning += "=";
                                }
                                lexMeaning += "\n";
                                if (lexiconEntry.ContainsKey(lexBuffer))
                                {
                                    lexiconEntry.TryGetValue(lexBuffer, out retrievedText);
                                    if (retrievedText != null)
                                    {
                                        lexMeaning += retrievedText;
                                    }
                                    else
                                    {
                                        lexMeaning += "Unable to retrieve a related meaning.";
                                    }
                                }
                                else
                                {
                                    lexMeaning += "No related meaning found.";
                                }
                                break;
                            case '2':
                                // There is a different word that is used with exactly the same meaning
                                lexMeaning = "Equivalent word to: " + lexBuffer + "\n";
                                wordLength = lexBuffer.Length + 20;
                                for (idx = 0; idx < wordLength; idx++)
                                {
                                    lexMeaning += "=";
                                }
                                lexMeaning += "\n";
                                if (lexiconEntry.ContainsKey(lexBuffer))
                                {
                                    lexiconEntry.TryGetValue(lexBuffer, out retrievedText);
                                    if (retrievedText != null)
                                    {
                                        lexMeaning += retrievedText;
                                    }
                                    else
                                    {
                                        lexMeaning += "Unable to retrieve a related meaning.";
                                    }
                                }
                                else
                                {
                                    lexMeaning += "No related meaning found.";
                                }
                                break;
                        }
                        break;
                    case '-':
                        if (doesExist)
                        {
                            lexiconEntry.TryGetValue(keyWord, out retrievedText);
                            if (retrievedText != null)
                            {
                                retrievedText += "\n\nAdditional Meaning:\n==================\n\n" + lexMeaning;
                                lexiconEntry.Remove(keyWord);
                                lexiconEntry.Add(keyWord, retrievedText);
                                if (!UnaccentedLookup.ContainsKey(flatWord))
                                {
                                    lexiconExtras = new classGkLexiconExtras();
                                    UnaccentedLookup.Add(flatWord, lexiconExtras);
                                }
                                else UnaccentedLookup.TryGetValue(flatWord, out lexiconExtras);
                                lexiconExtras.addAKey(keyWord);
                                lexMeaning = "";
                                break;
                            }
                            else
                            {
                                if (lexiconEntry.ContainsKey(keyWord))
                                {
                                    lexiconEntry.Remove(keyWord);
                                    UnaccentedLookup.Remove(keyWord);
                                }
                            }
                        }
                        if ((String.Compare(keyWord, "-i") == 0) || (lexMeaning.Length == 0))
                        {
                            lexMeaning = "";
                            break;
                        }
                        lexiconEntry.Add(keyWord, lexMeaning);
                        if (!UnaccentedLookup.ContainsKey(flatWord))
                        {
                            lexiconExtras = new classGkLexiconExtras();
                            UnaccentedLookup.Add(flatWord, lexiconExtras);
                        }
                        else UnaccentedLookup.TryGetValue(flatWord, out lexiconExtras);
                        lexiconExtras.addAKey(keyWord);
                        lexMeaning = "";
                        break;
                    default:
                        lexMeaning += lexBuffer + "\n";
                        break;
                }
                lexBuffer = srLexicon.ReadLine();
            }
            srLexicon.Close();
            srLexicon.Dispose();
        }

        /***********************************************************************
         *                                                                     *
         * Liddell & Scott Appendices                                          *
         *                                                                     *
         ***********************************************************************/

        public void populateAppendices()
        {
            int idx, noOfAppendices;
            String pathName, fileName = "";
            WebBrowser activeWeb;

            noOfAppendices = globalVars.NoOfWebBrowsers;
            pathName = globalVars.FullGkLexiconFolder;
            for (idx = 1; idx < noOfAppendices; idx++)
            {
                activeWeb = (WebBrowser) globalVars.getGroupedControl(globalVars.WebBrowserCode, idx);
                switch( idx)
                {
                    case 1: fileName = globalVars.FullGkLexiconFolder + @"\" + globalVars.LsApp1; break;
                    case 2: fileName = globalVars.FullGkLexiconFolder + @"\" + globalVars.LsApp2; break;
                    case 3: fileName = globalVars.FullGkLexiconFolder + @"\" + globalVars.LsApp3; break;
                    case 4: fileName = globalVars.FullGkLexiconFolder + @"\" + globalVars.LsApp4; break;
                    case 5: fileName = globalVars.FullGkLexiconFolder + @"\" + globalVars.LsApp5; break;
                }
                fileName = "file://" + fileName;
                activeWeb.Invoke( new performWebBrowserUpdate(updateWebBrowser), activeWeb, fileName);
            }
        }

        public void getLexiconEntry(String wordToExplain)
        {
            bool isModified = false, isNoUse = false;
            int idx, wordLength;
            String sourceLetter, targetLetter;
            String retrievedMeaning, workingWord, alternativeWorkingWord, finalLetter;
            classGkLexiconExtras lexiconExtras;
            SortedSet<String> setOfKeys;
            Font largeFont, mainFont;
            Color textColour, headerColour, backgroundColour;
            RichTextBox rtxtLexicon;

            largeFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(3, 2), globalVars.getDefinedStyleByIndex(3, 2), globalVars.getTextSize(3, 2));
            mainFont = globalVars.configureFont(globalVars.getDefinedFontNameByIndex(3, 1), globalVars.getDefinedStyleByIndex(3, 1), globalVars.getTextSize(3, 1));
            backgroundColour = globalVars.getColourSetting(3, 0);
            workingWord = wordToExplain.Trim();
            rtxtLexicon = (RichTextBox)globalVars.getGroupedControl(globalVars.RichtextBoxCode, 5);
            rtxtLexicon.Clear();
            rtxtLexicon.BackColor = backgroundColour;  // Background colour for the lexical area
            textColour = globalVars.getColourSetting(3, 1);           // Main text colour for the lexical area
            headerColour = globalVars.getColourSetting(3, 2);         // Large text (title) colour for the lexical area
            rtxtLexicon.SelectionFont = largeFont;
            rtxtLexicon.SelectionColor = headerColour;
            rtxtLexicon.SelectedText = "\t\t";
            rtxtLexicon.SelectionFont = largeFont;
            rtxtLexicon.SelectionColor = headerColour;
            rtxtLexicon.SelectedText = workingWord;
            rtxtLexicon.SelectionFont = largeFont;
            rtxtLexicon.SelectionColor = textColour;
            rtxtLexicon.SelectedText = "\n";
            rtxtLexicon.SelectionFont = mainFont;
            rtxtLexicon.SelectionColor = textColour;
            rtxtLexicon.SelectedText = "\n";
            if (lexiconEntry.ContainsKey(workingWord))
            {
                // The simple solution: we've found an entry
                lexiconEntry.TryGetValue(workingWord, out retrievedMeaning);
                if (retrievedMeaning != null)
                {
                    rtxtLexicon.SelectionFont = mainFont;
                    rtxtLexicon.SelectionColor = textColour;
                    rtxtLexicon.SelectedText = retrievedMeaning;
                }
                else
                {
                    rtxtLexicon.SelectionFont = mainFont;
                    rtxtLexicon.SelectionColor = textColour;
                    rtxtLexicon.SelectedText = "Meaning not found";
                }
            }
            else
            {
                // So, no immediate entry - try changing accented characters to second Unicode form
                isNoUse = true;
                alternativeWorkingWord = workingWord;
                wordLength = alternativeWorkingWord.Length;
                for (idx = 0; idx < wordLength; idx++)
                {
                    sourceLetter = alternativeWorkingWord.Substring(idx, 1);
                    if (alternativeCharacters.ContainsKey(sourceLetter))
                    {
                        alternativeCharacters.TryGetValue(sourceLetter, out targetLetter);
                        alternativeWorkingWord = alternativeWorkingWord.Replace(sourceLetter, targetLetter);
                        isModified = true;
                    }
                }
                if (isModified)
                {
                    if (lexiconEntry.ContainsKey(alternativeWorkingWord))
                    {
                        // Ah!  That worked
                        lexiconEntry.TryGetValue(alternativeWorkingWord, out retrievedMeaning);
                        if (retrievedMeaning != null)
                        {
                            rtxtLexicon.SelectionFont = mainFont;
                            rtxtLexicon.SelectionColor = textColour;
                            rtxtLexicon.SelectedText = retrievedMeaning;
                        }
                        else
                        {
                            rtxtLexicon.SelectionFont = mainFont;
                            rtxtLexicon.SelectionColor = textColour;
                            rtxtLexicon.SelectedText = "Meaning not found";
                        }
                        isNoUse = false;
                    }
                }
                if (isNoUse)
                {
                    // We're now faced with fiddling around - first, see if we can find a flat text solution
                    alternativeWorkingWord = greekProcessing.reduceToBareGreek(workingWord, true);
                    if (UnaccentedLookup.ContainsKey(alternativeWorkingWord))
                    {
                        // Good news!  We seem to have found the word
                        UnaccentedLookup.TryGetValue(alternativeWorkingWord, out lexiconExtras);
                        setOfKeys = lexiconExtras.SetOfKeys;
                        foreach (String foundKeyWord in setOfKeys)
                        {
                            lexiconEntry.TryGetValue(foundKeyWord, out retrievedMeaning);
                            if (retrievedMeaning != null)
                            {
                                rtxtLexicon.SelectionFont = mainFont;
                                rtxtLexicon.SelectionColor = textColour;
                                rtxtLexicon.SelectedText = retrievedMeaning;
                            }
                            else
                            {
                                rtxtLexicon.SelectionFont = mainFont;
                                rtxtLexicon.SelectionColor = textColour;
                                rtxtLexicon.SelectedText = "Meaning not found";
                            }
                        }
                    }
                    else
                    {
                        if (alternativeWorkingWord.Length < 5)
                        {
                            finalLetter = "xxxx";
                        }
                        else
                        {
                            finalLetter = alternativeWorkingWord.Substring(alternativeWorkingWord.Length - 4);
                        }
                        if (String.Compare(finalLetter, "ομαι") == 0)
                        {
                            alternativeWorkingWord = alternativeWorkingWord.Substring(0, alternativeWorkingWord.Length - 4) + "ω";
                            if (UnaccentedLookup.ContainsKey(alternativeWorkingWord))
                            {
                                // Good news!  We seem to have found the word
                                UnaccentedLookup.TryGetValue(alternativeWorkingWord, out lexiconExtras);
                                setOfKeys = lexiconExtras.SetOfKeys;
                                foreach (String foundKeyWord in setOfKeys)
                                {
                                    lexiconEntry.TryGetValue(foundKeyWord, out retrievedMeaning);
                                    if (retrievedMeaning != null)
                                    {
                                        rtxtLexicon.SelectionFont = mainFont;
                                        rtxtLexicon.SelectionColor = textColour;
                                        rtxtLexicon.SelectedText = retrievedMeaning;
                                    }
                                    else
                                    {
                                        rtxtLexicon.SelectionFont = mainFont;
                                        rtxtLexicon.SelectionColor = textColour;
                                        rtxtLexicon.SelectedText = "Meaning not found";
                                    }
                                }
                            }
                            else
                            {
                                rtxtLexicon.SelectionFont = mainFont;
                                rtxtLexicon.SelectionColor = textColour;
                                rtxtLexicon.SelectedText = "Meaning not found";
                            }
                        }
                        else
                        {
                            rtxtLexicon.SelectionFont = mainFont;
                            rtxtLexicon.SelectionColor = textColour;
                            rtxtLexicon.SelectedText = "Meaning not found";
                        }
                    }
                }
            }
        }

        public String parseGrammar(String codes1, String codes2)
        {
            bool isPtcpl = false;
            Char currentCode, decodeChar;
            String outputText;

            if ((codes1 == null) || (codes2 == null)) return "";
            currentCode = codes1[0];
            outputText = "";
            switch (currentCode)
            {
                case 'V': // The word is a verb
                    {
                        outputText = "Verb: ";
                        decodeChar = codes2[2];
                        if (decodeChar == 'P') isPtcpl = true;
                        if (isPtcpl)
                        {
                            decodeChar = codes2[3];
                            switch (decodeChar)
                            {
                                case 'N': outputText += "Nominative "; break;
                                case 'V': outputText += "Vocative "; break;
                                case 'A': outputText += "Accusative "; break;
                                case 'G': outputText += "Genitive "; break;
                                case 'D': outputText += "Dative "; break;
                                default: break;
                            }
                        }
                        decodeChar = codes2[3];
                        switch (decodeChar)
                        {
                            case '1': outputText = "1st person "; break;
                            case '2': outputText = "2nd person "; break;
                            case '3': outputText = "3rd person "; break;
                            default: // do nothing
                                break;
                        }
                        if (codes2.Length > 4)
                        {
                            decodeChar = codes2[4];
                            switch (decodeChar)
                            {
                                case 'S': outputText += "Singular "; break;
                                case 'D': outputText += "Dual"; break;
                                case 'P': outputText += "Plural "; break;
                                default: break;
                            }
                        }
                        if (codes2.Length > 5)
                        {
                            decodeChar = codes2[5];
                            switch (decodeChar)
                            {
                                case 'M': outputText += "Masculine "; break;
                                case 'F': outputText += "Feminine "; break;
                                case 'N': outputText += "Neuter "; break;
                                default: break;
                            }
                        }
                        decodeChar = codes2[0];
                        switch (decodeChar)
                        {
                            case 'P': outputText += "Present "; break;
                            case 'I': outputText += "Imperfect "; break;
                            case 'F': outputText += "Future "; break;
                            case 'A': outputText += "Aorist "; break;
                            case 'X': outputText += "Perfect "; break;
                            case 'Y': outputText += "Pluperfect "; break;
                            default: break;
                        }
                        decodeChar = codes2[1];
                        switch (decodeChar)
                        {
                            case 'A': outputText += "Active "; break;
                            case 'M': outputText += "Middle "; break;
                            case 'P': outputText += "Passive "; break;
                            case 'E': outputText += "Middle or Passive "; break;
                            case 'D': outputText += "Middle deponent "; break;
                            case 'O': outputText += "Passive deponent "; break;
                            case 'N': outputText += "Middle or Passive deponent "; break;
                            case 'Q': outputText += "Impersonal active "; break;
                            default: break;
                        }
                        decodeChar = codes2[2];
                        switch (decodeChar)
                        {
                            case 'I': outputText += "Indicative "; break;
                            case 'S': outputText += "Subjunctive "; break;
                            case 'O': outputText += "Optative "; break;
                            case 'M': outputText += "Imperative "; break;
                            case 'D': outputText += "Imperative "; break;
                            case 'N': outputText += "Infinitive "; break;
                            case 'P': outputText += "Participle "; break;
                            case 'R': outputText += "Imperative participle "; break;
                            default: break;
                        }
                    }
                    break;
                case 'N':
                case 'A':
                    {
                        if (currentCode == 'A')
                        {
                            outputText = "Adjective: ";
                        }
                        else
                        {
                            outputText = "Noun: ";
                        }
                        decodeChar = codes2[0];
                        switch (decodeChar)
                        {
                            case 'N': outputText += "Nominative "; break;
                            case 'V': outputText += "Vocative "; break;
                            case 'A': outputText += "Accusative "; break;
                            case 'G': outputText += "Genitive "; break;
                            case 'D': outputText += "Dative "; break;
                            default: break;
                        }
                        decodeChar = codes2[1];
                        switch (decodeChar)
                        {
                            case 'S': outputText += "Singular "; break;
                            case 'D': outputText += "Dual "; break;
                            case 'P': outputText += "Plural "; break;
                            default: break;
                        }
                        decodeChar = codes2[2];
                        switch (decodeChar)
                        {
                            case 'M': outputText += "Masculine "; break;
                            case 'F': outputText += "Feminine "; break;
                            case 'N': outputText += "Neuter "; break;
                            default: break;
                        }
                        if (codes2.Length > 3)
                        {
                            decodeChar = codes2[3];
                            switch (decodeChar)
                            {
                                case 'C': outputText += "Comparative "; break;
                                case 'S': outputText += "Superlative "; break;
                                default: break;
                            }
                        }
                    }
                    break;
                case 'P': outputText = "Preposition"; break;
                case 'C': outputText = "Conjunction"; break;
                case 'D': outputText = "Adverb"; break;
                case 'X': outputText = "Particle"; break;
                case 'I': outputText = "Interjection"; break;
                case 'M': outputText = "Indeclinable number"; break;
                case 'R':
                    {
                        decodeChar = codes2[0];
                        switch (decodeChar)
                        {
                            case 'N': outputText += "Nominative "; break;
                            case 'V': outputText += "Vocative "; break;
                            case 'A': outputText += "Accusative "; break;
                            case 'G': outputText += "Genitive "; break;
                            case 'D': outputText += "Dative "; break;
                            default: break;
                        }
                        decodeChar = codes2[1];
                        switch (decodeChar)
                        {
                            case 'S': outputText += "Singular "; break;
                            case 'D': outputText += "Dual "; break;
                            case 'P': outputText += "Plural "; break;
                            default: break;
                        }
                        decodeChar = codes2[2];
                        switch (decodeChar)
                        {
                            case 'M': outputText += "Masculine "; break;
                            case 'F': outputText += "Feminine "; break;
                            case 'N': outputText += "Neuter "; break;
                            default: break;
                        }
                        switch (codes1[1])
                        {
                            case 'A': outputText += " Article "; break;
                            case 'P': outputText += " Personal Pronoun "; break;
                            case 'I': outputText += " Interrogative Pronoun "; break;
                            case 'R': outputText += " Relative Pronoun "; break;
                            case 'D': outputText += " Demonstrative Pronoun "; break;
                            case 'X': outputText += "ὅστις"; break;
                            default: outputText += " Unknown: " + codes1; break;
                        }
                    }
                    break;
            }
            return outputText;
        }
    }
}
