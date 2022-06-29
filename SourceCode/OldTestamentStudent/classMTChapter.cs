using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classMTChapter
    {
        /*============================================================================================================*
         *                                                                                                            *
         *                                             classMTChapter                                                 *
         *                                             --------------                                                 *
         *                                                                                                            *
         *  In essence, we want to identify each verse that belongs to a given chapter.  At root, information about   *
         *    the verse is provided by the class classMTVerse.  However, we need to cater for the possibility that    *
         *    verse number is _not_ a simple integer but may contain alphanumerics (e.g. 12a).  So, we                *
         *    a) key the list of class instances on a sequential integer.  The sequence has no significance other     *
         *       than ensuring uniqueness.  (It will actually be generated in the sequence the verses are encountered *
         *       in the source data.)                                                                                 *
         *    b) we separately provide a lookup of this sequence number which gives the String-based version of the   *
         *       verse "number" (which we will call a verse reference"), which is recorded in the sequence listed in  *
         *       the source data.  This means that numbers may also be out of strict sequence.                        *
         *    c) we also provide an inverse lookup that allows us to find the sequence number, if we know the string- *
         *       based verse reference.  This is important because it allows us to find the verse details (the class  *
         *       instance) from the verse reference provided by the source data.                                      *
         *                                                                                                            *
         *  Note: 1. verse sequences will be zero-based (i.e. start at zero) while  verse references are (ostensibly) *
         *           meaningful and normally start at 1 (although they can be zero).                                  *
         *        2. noOfVersesInChapter will count the *sequence* of verses                                          *
         *                                                                                                            *
         *============================================================================================================*/

        int noOfVersesInChapter = 0, bookNo, chapterSeqNo;
        String chapterRef;
        /*============================================================================================================*
         *                                                                                                            *
         *                                             versesBySequence                                               *
         *                                             ----------------                                               *
         *                                                                                                            *
         *  a look-up list of verse class instances, keyed by a sequence no.                                          *
         *     Key:   verse Sequence                                                                                  *
         *     Value: the class instance address                                                                      *
         *                                                                                                            *
         *============================================================================================================*/
        SortedDictionary<int, classMTVerse> versesBySequence = new SortedDictionary<int, classMTVerse>();

        /*============================================================================================================*
         *                                                                                                            *
         *                                           VerseReferenceBySequence                                         *
         *                                           ------------------------                                         *
         *                                                                                                            *
         *  A list that will convert the simple verse sequence to the verse "reference", as given in the data         *
         *     Key:   verse sequence                                                                                  *
         *     Value: the verse number provided from data                                                             *
         *                                                                                                            *
         *============================================================================================================*/
        SortedDictionary<int, String> VerseReferenceBySequence = new SortedDictionary<int, String>();

        /*============================================================================================================*
         *                                                                                                            *
         *                                           sequenceForVerseReference                                        *
         *                                           -------------------------                                        *
         *                                                                                                            *
         *  A reverse lookup to providedVersesBySequence - i.e. given a verse reference, this will give us the        *
         *    internal sequence number                                                                                *
         *     Key:   verse number (from data)                                                                        *
         *     Value: verse sequence                                                                                  *
         *                                                                                                            *
         *============================================================================================================*/
        SortedDictionary<String, int> sequenceForVerseReference = new SortedDictionary<String, int>();

        classMTChapter previousChapter, nextChapter;

        public int NoOfVersesInChapter { get => noOfVersesInChapter; set => noOfVersesInChapter = value; }
        public int BookNo { get => bookNo; set => bookNo = value; }
        public int ChapterSeqNo { get => chapterSeqNo; set => chapterSeqNo = value; }
        public string ChapterRef { get => chapterRef; set => chapterRef = value; }
        public classMTChapter PreviousChapter { get => previousChapter; set => previousChapter = value; }
        public classMTChapter NextChapter { get => nextChapter; set => nextChapter = value; }

        public classMTVerse addVerseToChapter(String verseId)
        {
            int seqNo = -1;
            classMTVerse newVerse;

            if (sequenceForVerseReference.ContainsKey(verseId))
            {
                sequenceForVerseReference.TryGetValue(verseId, out seqNo);
                versesBySequence.TryGetValue(seqNo, out newVerse);
            }
            else
            {
                newVerse = new classMTVerse();
                sequenceForVerseReference.Add(verseId, noOfVersesInChapter);
                VerseReferenceBySequence.Add(noOfVersesInChapter, verseId);
                versesBySequence.Add(noOfVersesInChapter++, newVerse);
            }
            return newVerse;
        }

        public classMTVerse getVerseBySequence(int seqNo)
        {
            classMTVerse newVerse = null;

            versesBySequence.TryGetValue(seqNo, out newVerse);
            return newVerse;
        }

        public classMTVerse getVerseByVerseNo(String verseId)
        {
            int seqNo = -1;

            sequenceForVerseReference.TryGetValue(verseId, out seqNo);
            if (seqNo == -1) return null;
            return getVerseBySequence(seqNo);
        }

        public int getSequenceByVerseNo(String verseId)
        {
            int seqNo = -1;

            sequenceForVerseReference.TryGetValue(verseId, out seqNo);
            return seqNo;
        }

        public String getVerseNoBySequence(int seqNo)
        {
            String verseId = "";

            VerseReferenceBySequence.TryGetValue(seqNo, out verseId);
            return verseId;
        }
    }
}
