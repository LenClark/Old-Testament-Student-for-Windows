using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classLXXPrimaryResult
    {
        /*================================================================================================================*
         *                                                                                                                *
         *                                              classLXXSearchResult                                              *
         *                                              ====================                                              *
         *                                                                                                                *
         *  This class is specific to the situation where a single word is being sought (i.e. no secondary word is        *
         *    involved.  (Of course, it is also specific to the Septuagint.)                                              *
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

        bool isRepeatInVerse = false;
        int bookId, chapSeq, verseSeq, noOfMatchingWords;
        String chapReference, verseReference;
        SortedList<int, int> matchingWordPositions = new SortedList<int, int>();
        classLXXVerse impactedVerse;

        public bool IsRepeatInVerse { get => isRepeatInVerse; set => isRepeatInVerse = value; }
        public int BookId { get => bookId; set => bookId = value; }
        public int ChapSeq { get => chapSeq; set => chapSeq = value; }
        public int VerseSeq { get => verseSeq; set => verseSeq = value; }
        public int NoOfMatchingWords { get => noOfMatchingWords; set => noOfMatchingWords = value; }
        public string ChapReference { get => chapReference; set => chapReference = value; }
        public string VerseReference { get => verseReference; set => verseReference = value; }
        public SortedList<int, int> MatchingWordPositions { get => matchingWordPositions; set => matchingWordPositions = value; }
        internal classLXXVerse ImpactedVerse { get => impactedVerse; set => impactedVerse = value; }

        public void addWordPosition(int position)
        {
            matchingWordPositions.Add(noOfMatchingWords++, position);
        }

        public int getWordPositionBySeq(int index)
        {
            int retrievedPosition = -1;

            if (matchingWordPositions.ContainsKey(index)) matchingWordPositions.TryGetValue(index, out retrievedPosition);
            return retrievedPosition;
        }
    }
}
