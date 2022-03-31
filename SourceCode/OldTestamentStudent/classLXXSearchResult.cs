using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classLXXSearchResult
    {
        /*================================================================================================================*
         *                                                                                                                *
         *                                              classLXXSearchResult                                              *
         *                                              ====================                                              *
         *                                                                                                                *
         *  The key element of this class is the listOfVerses.  This is a list of verse with the key of between 1 and 5.  *
         *    The significance of these values is as follows:                                                             *
         *                                                                                                                *
         *  key                                        Significance                                                       *
         *  ===                                        ============                                                       *
         *   2          The current verse which contains the primary match.  It is the *only* one of the five verses that *
         *                *can* contain a primary match                                                                   *
         *   1          The verse previous to that in key record 2.  If the secondary matches a word backwards across the *
         *                verse boundary, then it will be this verse                                                      *
         *   0          The verse one earlier.  This will normally not be needed but is included to cater for very short  *
         *                verses                                                                                          *
         *   3          The next verse from the primary match verse, analogous to key = 1 but moving forwards rather than *
         *                backwards                                                                                       *
         *   4          Analogous to key = 0 for forward searches.                                                        *
         *                                                                                                                *
         *   Note that key = 2 is the only verse that can contain the primary match but the secondary match can be found  *
         *     in any of the five (including key = 2).                                                                    *
         *                                                                                                                *
         *  Instance members:                                                                                             *
         *  ================                                                                                              *
         *                                                                                                                *
         *  isValid     For secondary searches, the process will occur in two stages:                                     *
         *              a) a primary search, as normal, potentially returning a substantial number of records;            *
         *              b) a further scan through these primary matches to find those that also have a secondary match.   *
         *              We would expect the majority *not* to have a secondary match.  In these case, we need to remove   *
         *                the record but, rather than deleting it, we mark it as invalid.                                 *
         *  bookId      The normal reference to the book from which the verse comes                                       *
         *  chapReference   The meaningful chapter number of the reference.                                               *
         *                (Note: this is *not* the sequence number for the chapter.)                                      *
         *  chapSeq     This _is_ the internal sequence                                                                   *
         *  verseReference  The verse number                                                                              *
         *  verseSeq    The internal sequence                                                                             *
         *  noOfSearchCandidates                                                                                          *
         *              If we had to go back or forward verses when performing a secondary search, then this tells us     *
         *                whether we need to go back or forward 1 or 2 verses.  So the value be 0, 1 or 2.                *
         *  preOrPostCandidates                                                                                           *
         *              If noOfSearchCandidates > 0, then this will tell us whether the additional verses are *before*    *
         *                the main match or *after*.  Values will be:                                                     *
         *                  0   No additional verses accessed                                                             *
         *                 -n  Additional verse(s) is (are) before the main match                                         *
         *                  n   Additional verse(s) is (are) after the main match                                         *
         *  wordNo      This is the word sequence number of the matched word in the verse                                 *
         *  allWords    A list of all the words in the verse, in sequence:                                                *
         *                Key    The sequence number for the word                                                         *
         *                Value  The word (including accents, etc.)                                                       *
         *  prefixList  This list contains a flag to indicate whether the matching word is a prefix or full word          *
         *                Key    The word sequence (matching the key to allWords                                          *
         *                Value  A boolean: true, if it *is* a prefix                                                     *
         *                                  false, if it is a full word                                                   *
         *  affixList   This is a list of affixes.                                                                        *
         *                Key    The word sequence (matching the key to allWords                                          *
         *                Value  The affix, if there is one.  If the word has no affix, then Value = ""                   *
         *  candidateList                                                                                                 *
         *              The class instance(s) of additional verses, as described for noOfSearchCandidates.                *
         *                                                                                                                *
         *================================================================================================================*/

        bool isValid = false;
        int bookId, chapSeq, verseSeq, primaryWordMatched, noOfSearchCandidates = 0, preOrPostCandidates;
        String chapReference, verseReference;
        SortedList<int, classLXXSearchVerse> listOfVerses = new SortedList<int, classLXXSearchVerse>();

        public int BookId { get => bookId; set => bookId = value; }
        public int ChapSeq { get => chapSeq; set => chapSeq = value; }
        public int VerseSeq { get => verseSeq; set => verseSeq = value; }
        public string ChapReference { get => chapReference; set => chapReference = value; }
        public string VerseReference { get => verseReference; set => verseReference = value; }
        public int PrimaryWordMatched { get => primaryWordMatched; set => primaryWordMatched = value; }
        public int NoOfSearchCandidates { get => noOfSearchCandidates; set => noOfSearchCandidates = value; }
        public int PreOrPostCandidates { get => preOrPostCandidates; set => preOrPostCandidates = value; }
        public bool IsValid { get => isValid; set => isValid = value; }
        public SortedList<int, classLXXSearchVerse> ListOfVerses { get => listOfVerses; set => listOfVerses = value; }

        public void addWord(int seqNo, classLXXVerse impactedVerse, int chapNo, int verseNo, String chapRef, String verseRef)
        {
            classLXXSearchVerse currentSearchVerse;

            if (listOfVerses.ContainsKey(seqNo))
            {
                listOfVerses.TryGetValue(seqNo, out currentSearchVerse);
            }
            else
            {
                currentSearchVerse = new classLXXSearchVerse();
                listOfVerses.Add(seqNo, currentSearchVerse);
            }
            currentSearchVerse.ImpactedVerse = impactedVerse;
            if (chapNo > -1) currentSearchVerse.ChapterNumber = chapNo;
            currentSearchVerse.ChapterReference = chapRef;
            if (verseNo > -1) currentSearchVerse.VerseNumber = verseNo;
            currentSearchVerse.VerseReference = verseRef;
        }

        public classLXXSearchVerse getResultVerseForIndex(int index)
        {
            classLXXSearchVerse currentVerse = null;

            if (listOfVerses.ContainsKey(index))
            {
                listOfVerses.TryGetValue(index, out currentVerse);
                return currentVerse;
            }
            else return null;
        }
    }
}
