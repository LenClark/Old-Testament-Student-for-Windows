using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classMTSearchVerse
    {
        /*===========================================================================================================*
         *                                                                                                           *
         *                                                classSearchVerse                                           *
         *                                                ================                                           *
         *                                                                                                           *
         *  A list of up to five verses can be defined as a SearchResult.  This record gives the following           *
         *    information:                                                                                           *
         *                                                                                                           *
         *  Variables:                                                                                               *
         *  =========                                                                                                *
         *                                                                                                           *
         *  isFollowed            This is set to true of there should be no bold line between this and the next      *
         *                          record                                                                           *
         *  chapterNumber         The real chapter number containing the verse                                       *
         *  verseNumber           The real verse number                                                              *
         *  noOfMatchingWords     Used to control the list of matchingWordPositions                                  *
         *  matchingWordPosition  The sequence value of the word matching the primary search word, if it exists      *
         *  impactedVerse         The instance of the verse, from which each word can be derived                     *
         *  matchingWordPositions The sequence numbewr of any primary or secondary matches in the verse              *
         *  matchingWordType      Indicates whether the match is Primary or secondary:                               *
         *                          Key:   The same integer index as used in matchingWordPositions                   *
         *                          Value: 1 = Primary, 2 = Secondary                                                *
         *                                                                                                           *
         *===========================================================================================================*/

        bool isFollowed = false;
        int bookId, chapterNumber, verseNumber, noOfMatchingWords = 0;
        String chapterReference, verseReference;
        classMTVerse impactedVerse;
        SortedList<int, int> matchingWordPositions = new SortedList<int, int>();
        SortedList<int, int> matchingWordType = new SortedList<int, int>();

        public bool IsFollowed { get => isFollowed; set => isFollowed = value; }
        public int BookId { get => bookId; set => bookId = value; }
        public int ChapterNumber { get => chapterNumber; set => chapterNumber = value; }
        public int VerseNumber { get => verseNumber; set => verseNumber = value; }
        public int NoOfMatchingWords { get => noOfMatchingWords; set => noOfMatchingWords = value; }
        public string ChapterReference { get => chapterReference; set => chapterReference = value; }
        public string VerseReference { get => verseReference; set => verseReference = value; }
        public classMTVerse ImpactedVerse { get => impactedVerse; set => impactedVerse = value; }
        public SortedList<int, int> MatchingWordPositions { get => matchingWordPositions; set => matchingWordPositions = value; }
        public SortedList<int, int> MatchingWordType { get => matchingWordType; set => matchingWordType = value; }

        public void addWordPosition( int position, int wordType)
        {
            matchingWordPositions.Add(noOfMatchingWords, position);
            MatchingWordType.Add(noOfMatchingWords++, wordType);
        }

        public int getWordPositionBySeq( int index)
        {
            int retrievedPosition = -1;

            if (matchingWordPositions.ContainsKey(index)) matchingWordPositions.TryGetValue(index, out retrievedPosition);
            return retrievedPosition;
        }

        public int getWordTypeBySeq(int index)
        {
            int retrievedType = -1;

            if (MatchingWordType.ContainsKey(index)) MatchingWordType.TryGetValue(index, out retrievedType);
            return retrievedType;
        }
    }
}
