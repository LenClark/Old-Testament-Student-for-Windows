using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classWordToStrong
    {
        /*================================================================================================================*
         *                                                                                                                *
         *                                            classWordToStrong                                                   *
         *                                            =================                                                   *
         *                                                                                                                *
         *  Purpose:                                                                                                      *
         *  =======                                                                                                       *
         *                                                                                                                *
         *  To provide a simple (performant) mechanism of identifying potential Strong References directly from an        *
         *    (unaccented) word.  This will   make the manual input of Hebrew a bit easier and quicker.                   *
         *                                                                                                                *
         *  Variables:                                                                                                    *
         *  =========                                                                                                     *
         *                                                                                                                *
         *  unaccentedWord       This is effectively the key to the class and is not strictly necessary in the instance   *
         *                         itself                                                                                 *
         *  listOfActualWords    This contains the fully pointed word and, at the time of creation, it's not clear        *
         *                         whether it will be needed.  Ideally, this will always be a list of one but it caters   *
         *                         for the possibility that there will be two forms that are morphologically different    *
         *                         but which result in the same unaccented form.                                          *
         *  noOfDistinctWords    A count of listOfActualWords                                                             *
         *  actualWordsProcessed This is a list that reproduces the values of listOfActualWords and is used purely to     *
         *                         check whether we have already entered a given word                                     *
         *  strongRef          The list of Strongs references for the word (or words) in the instance                     *
         *  noOfStrongRefs     A count of items in strongRef                                                              *
         *                                                                                                                *
         *================================================================================================================*/

        int noOfDistinctWords = 0, noOfStrongRefs = 0;
        String unaccentedWord;
        SortedList<int, String> listOfActualWords = new SortedList<int, string>();
        SortedSet<String> actualWordsProcessed = new SortedSet<string>();
        SortedSet<int> strongRef = new SortedSet<int>();

        public int NoOfDistinctWords { get => noOfDistinctWords; set => noOfDistinctWords = value; }
        public int NoOfStrongRefs { get => noOfStrongRefs; set => noOfStrongRefs = value; }
        public string UnaccentedWord { get => unaccentedWord; set => unaccentedWord = value; }

        public void addAnActualWord(String inWord)
        {
            if (actualWordsProcessed.Contains(inWord)) return;  //We've already processed that word
            actualWordsProcessed.Add(inWord);
            listOfActualWords.Add(NoOfDistinctWords++, inWord);
        }

        public void addAStrongRef(int inRef)
        {
            if (!strongRef.Contains(inRef))
            {
                strongRef.Add(inRef);
                noOfStrongRefs++;
            }
        }

        public String getActualWordByIndex(int index)
        {
            String newWord = "";

            if (listOfActualWords.ContainsKey(index))
            {
                listOfActualWords.TryGetValue(index, out newWord);
            }
            return newWord;
        }

        public int getStrongRefByIndex(int index)
        {
            int strongNo = 0;

            if (index < noOfStrongRefs) strongNo = strongRef.ElementAt(index);
            return strongNo;
        }
    }
}
