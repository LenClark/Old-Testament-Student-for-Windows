using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OldTestamentStudent
{
    public class classHebLexicon
    {
        /*========================================================================================*
         *                                                                                        *
         *                                   classLexicon                                         *
         *                                   ============                                         *
         *                                                                                        *
         *  This class will also handle some methods for processing words (in order to avoid      *
         *    creating yet another orthography class.                                             *
         *                                                                                        *
         *========================================================================================*/
        int noOfMatchesReturned;
        classGlobal globalVars;
        classMTText mainText;
        SortedDictionary<int, classBDBEntry> bdbEntryList = new SortedDictionary<int, classBDBEntry>();
        SortedList<int, classMTSearchVerse> listOfSearchResults = new SortedList<int, classMTSearchVerse>();
        ListBox lbBooksToSearch;

        public int NoOfMatchesReturned { get => noOfMatchesReturned; set => noOfMatchesReturned = value; }
        public ListBox LbBooksToSearch { get => lbBooksToSearch; set => lbBooksToSearch = value; }

        public void initialiseLexicon(classGlobal inGlobal, classMTText inText)
        {
            globalVars = inGlobal;
            mainText = inText;
            loadLexiconData();
        }

        private void loadLexiconData()
        {
            int idx, noOfStrongNos, bdbNo, strongRef;
            String fileBuffer, workArea;
            String[] bdbContent, strongRefSource;
            Char[] splitParams = { '\t' }, strongSeperator = { '_' };
            StreamReader srBDB;
            classBDBEntry currentEntry;
            Tuple<String, String> renderResults;

            srBDB = new StreamReader(globalVars.FullLexiconFile);
            fileBuffer = srBDB.ReadLine();
            fileBuffer = srBDB.ReadLine();
            while (fileBuffer != null)
            {
                bdbContent = fileBuffer.Split(splitParams);
                workArea = bdbContent[0].Substring(3);
                bdbNo = Convert.ToInt32(workArea);
                strongRefSource = bdbContent[1].Split(strongSeperator);
                noOfStrongNos = strongRefSource.Length;
                for (idx = 0; idx < noOfStrongNos; idx++)
                {
                    if (strongRefSource[idx].Length == 0) continue;
                    workArea = strongRefSource[idx].Substring(1);
                    strongRef = Convert.ToInt32(workArea);
                    if (bdbEntryList.ContainsKey(strongRef))
                    {
                        bdbEntryList.TryGetValue(strongRef, out currentEntry);
                    }
                    else
                    {
                        currentEntry = new classBDBEntry();
                        bdbEntryList.Add(strongRef, currentEntry);
                    }
                    renderResults = renderLexiconText(bdbContent[2]);
                    if (workArea.Length > 0) currentEntry.addEntry(bdbNo, renderResults.Item1, renderResults.Item2);
                }
                fileBuffer = srBDB.ReadLine();
            }
            srBDB.Close();
        }

        private Tuple<String, String> renderLexiconText(String source)
        {
            int nPstn, nPstn2, nEnd;
            String lexiconText = "", bdbEntry = "";

            nPstn = source.IndexOf("</entry></div>");
            if (nPstn > -1)
            {
                lexiconText = source.Substring(nPstn + 14);
                nPstn = lexiconText.IndexOf("<bdbheb>");
                nPstn2 = lexiconText.IndexOf("<bdbarc>");
                if ((nPstn > -1) || (nPstn2 > -1))
                {
                    if ((nPstn == -1) && (nPstn2 > -1)) nPstn = nPstn2;
                    else
                    {
                        if ((nPstn > -1) && (nPstn2 > -1))
                        {
                            if (nPstn > nPstn2) nPstn = nPstn2;
                        }
                    }
                }
                if (nPstn > -1)
                {
                    nPstn = lexiconText.IndexOf('>', nPstn);
                    nEnd = lexiconText.IndexOf('<', nPstn);
                    bdbEntry = lexiconText.Substring(nPstn + 1, nEnd - nPstn - 1);
                    if (nEnd == nPstn + 1)
                    {
                        nPstn = lexiconText.IndexOf('>', nEnd);
                        nEnd = lexiconText.IndexOf('<', nPstn);
                        bdbEntry = lexiconText.Substring(nPstn + 1, nEnd - nPstn - 1);
                    }
                    if (bdbEntry.Contains("ᵑ7"))
                    {
                        return new Tuple<string, string>("", "");
                    }
                }
            }
            return new Tuple<string, string>(bdbEntry, lexiconText);
        }

        public classBDBEntry getBDBEntryForStrongNo(int strongNo)
        {
            classBDBEntry acquiredEntry = null;

            if (bdbEntryList.ContainsKey(strongNo)) bdbEntryList.TryGetValue(strongNo, out acquiredEntry);
            return acquiredEntry;
        }

        public String removeAccents(String sourceWord)
        {
            /*========================================================================================*
             *                                                                                        *
             *                                     removeAccents                                      *
             *                                     =============                                      *
             *                                                                                        *
             *  Purpose: to remove all except                                                         *
             *           a) core Hebrew characters                                                    *
             *           b) vowel pointing                                                            *
             *           c) sin/shin points                                                           *
             *           d) dagesh (forte and line)                                                   *
             *                                                                                        *
             *  Parameter:                                                                            *
             *  =========                                                                             *
             *                                                                                        *
             *  sourceWord   may be a word, sentence or entire verse, so can includes spaces.         *
             *                                                                                        *
             *========================================================================================*/

            int idx, wordLength;
            String resultingWord = "";

            wordLength = sourceWord.Length;
            for (idx = 0; idx < wordLength; idx++)
            {
                // Is the character a standard Hebrew consonant?
                if (((int)sourceWord[idx] >= 0x5d0) && ((int)sourceWord[idx] <= 0x5ea))
                {
                    resultingWord += sourceWord.Substring(idx, 1);
                    continue;
                }
                // Is the character a vowel or acceptable pointing character?
                if (((int)sourceWord[idx] >= 0x5b0) && ((int)sourceWord[idx] <= 0x5bc))
                {
                    resultingWord += sourceWord.Substring(idx, 1);
                    continue;
                }
                // Is the character a sin/shin dot, end of verse Sof Pasuq or mark dot ?
                if (((int)sourceWord[idx] >= 0x5c1) && ((int)sourceWord[idx] <= 0x5c5))
                {
                    resultingWord += sourceWord.Substring(idx, 1);
                    continue;
                }
                // Is the character a mappeq?
                if (sourceWord[idx] == '\u05be')
                {
                    resultingWord += '\u05be'.ToString();
                    continue;
                }
                // Is the character a low order ASCII character, including space and non-break space
                if ((sourceWord[idx] >= 0x0020) & (sourceWord[idx] <= 0x00a0))
                {
                    resultingWord += sourceWord.Substring(idx, 1);
                    continue;
                }
                // Is the character a carriage return?
                if (sourceWord[idx] == '\n')
                {
                    resultingWord += "\n";
                    continue;
                }
            }
            return resultingWord;
        }
    }
}

