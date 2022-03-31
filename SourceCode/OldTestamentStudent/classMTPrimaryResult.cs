using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classMTPrimaryResult
    {
        /*================================================================================================================*
         *                                                                                                                *
         *                                              classMTPrimaryResult                                              *
         *                                              ====================                                              *
         *                                                                                                                *
         *  This class is specific to the situation where a single word is being sought (i.e. no secondary word is        *
         *    involved.  (Of course, it is also specific to the Masoretic text.)                                          *
         *                                                                                                                *
         *  Instance members:                                                                                             *
         *  ================                                                                                              *
         *                                                                                                                *
         *  isRepeatInVerse                                                                                               *
         *              An indicator for the display stage that this is the first of connected "records"                  *
         *  bookId      The normal reference to the book from which the verse comes                                       *
         *  chapReference                                                                                                 *
         *              The meaningful chapter number of the reference.                                                   *
         *                (Note: this is *not* the sequence number for the chapter.)                                      *
         *  chapSeq     This _is_ the internal sequence                                                                   *
         *  verseReference                                                                                                *
         *              The verse number, as provided by the source data                                                  *
         *  verseSeq    The internal sequence                                                                             *
         *  noOfMatchingWords                                                                                             *
         *              The total number of entires in the list, matchingWordPositions                                    *
         *  matchingWordPositions                                                                                         *
         *              This allows for multiple occurrences of matched primary words in a given verse                    *
         *  ImpactedVerse                                                                                                 *
         *              The address of the verse itself                                                                   *
         *                                                                                                                *
         *================================================================================================================*/

        bool isRepeatInVerse = false;
        int bookId, chapSeq, verseSeq, noOfMatchingWords;
        String chapReference, verseReference;
        SortedList<int, int> matchingWordPositions = new SortedList<int, int>();
        classMTVerse impactedVerse;

        public bool IsRepeatInVerse { get => isRepeatInVerse; set => isRepeatInVerse = value; }
        public int BookId { get => bookId; set => bookId = value; }
        public int ChapSeq { get => chapSeq; set => chapSeq = value; }
        public int VerseSeq { get => verseSeq; set => verseSeq = value; }
        public int NoOfMatchingWords { get => noOfMatchingWords; set => noOfMatchingWords = value; }
        public string ChapReference { get => chapReference; set => chapReference = value; }
        public string VerseReference { get => verseReference; set => verseReference = value; }
        public SortedList<int, int> MatchingWordPositions { get => matchingWordPositions; set => matchingWordPositions = value; }
        public classMTVerse ImpactedVerse { get => impactedVerse; set => impactedVerse = value; }

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
