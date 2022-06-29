using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classMTBook
    {
        /*============================================================================================================*
         *                                                                                                            *
         *                                              classMTBook                                                   *
         *                                              -----------                                                   *
         *                                                                                                            *
         *  In essence, we want to identify each chapter that belongs to a given book.  At root, information about    *
         *    the chapter is provided by the class classMTChapter.  However, we need to cater for the possibility     *
         *    that the chapter number is _not_ a simple integer but may contain alphanumerics (e.g. 12a).  So, we     *
         *    a) key the list of class instances on a sequential integer.  The sequence has no significance other     *
         *       than ensuring uniqueness.  (It will actually be generated in the sequence the chapters are           *
         *       encounteredin the source data.)                                                                      *
         *    b) we separately provide a lookup of this sequence number which gives the String-based version of the   *
         *       chapter "number" (which we will call a chapter reference"), which is recorded in the sequence listed *
         *       in the source data.  This means that numbers may also be out of strict sequence.                     *
         *    c) we also provide an inverse lookup that allows us to find the sequence number, if we know the string- *
         *       based chapter reference.  This is important because it allows us to find the chapter details (the    *
         *       class instance) from the chapter reference provided by the source data.                              *
         *                                                                                                            *
         *  Note: 1. chapter sequences will be zero-based (i.e. start at zero) while chapter references are           *
         *           (ostensibly) meaningful and normally start at 1 (although they can be zero).                     *
         *        2. noOfChaptersInBook will count the *sequence* of chapters                                         *
         *                                                                                                            *
         *============================================================================================================*/
        /*--------------------------------------------------------------------------------------------------------------*
         *                                                                                                              *
         *                                                  category                                                    *
         *                                                  --------                                                    *
         *                                                                                                              *
         *  This is a categorisation that nods in the direction of the traditional Jewish categories but also attempts  *
         *    to reflect popular Christian usage.  It also has the benefit that most categories contain fewer           *
         *    individual books.                                                                                         *
         *                                                                                                              *
         *  The categories are as follows:                                                                              *
         *                                                                                                              *
         *   Category                                            Content                                                *
         *   --------                                            -------                                                *
         *      1       The books of Moses: Genesis, Exodus, Numbers, Leviticus, Deuteronomy                            *
         *      2       The "former prophets", which we would categorise as "history": Joshua, Judges, 1 & 2 Samuel,    *
         *                 1 & 2 Kings                                                                                  *
         *      3       The major prophets - a group within the "latter prophets": Isaiah, Jeremiah and Ezekiel         *
         *      4       The minor prophets - also in the "latter prophets": Hosea, Joel, Amos, Obadiah, Jonah, Micah,   *
         *                 Nahum, Habakkuk,	Zephaniah, Haggai, Zechariah, Malachi                                       *
         *      5       The poetical books in the Kethubim ("the rest"/ "the [other] writings"): Job, Psalms, Proverbs, *
         *                 Ecclesiastes, Song of Solomon, Lamentations                                                  *
         *      6       The "historical" books of the Kethubim: Ruth, 1 & 2 Chronicles, Ezra, Nehemiah, Esther, Daniel  *
         *                                                                                                              *
         *--------------------------------------------------------------------------------------------------------------*/
        int noOfChaptersInBook = 0, category, bookId;
        String bookName, shortName;

        /*--------------------------------------------------------------------------------------------------------------*
         *                                                                                                              *
         *                                                chaptersBySequence                                            *
         *                                                ------------------                                            *
         *                                                                                                              *
         *  A look-up list of chapter class instances, keyed by a sequence no.                                          *
         *     Key:   chapter Sequence                                                                                  *
         *     Value: the class instance address                                                                        *
         *                                                                                                              *
         *--------------------------------------------------------------------------------------------------------------*/
        SortedDictionary<int, classMTChapter> chaptersBySequence = new SortedDictionary<int, classMTChapter>();

        /*--------------------------------------------------------------------------------------------------------------*
         *                                                                                                              *
         *                                             chapterReferencesBySequence                                      *
         *                                             ---------------------------                                      *
         *                                                                                                              *
         *  A list that will convert the simple chapter sequence to the chapter, as given in the data                   *
         *     Key:   chapter sequence                                                                                  *
         *     Value: the chapter reference provided from data                                                          *
         *                                                                                                              *
         *--------------------------------------------------------------------------------------------------------------*/
        SortedDictionary<int, String> chapterReferencesBySequence = new SortedDictionary<int, String>();

        /*--------------------------------------------------------------------------------------------------------------*
         *                                                                                                              *
         *                                             sequenceForChapterReference                                      *
         *                                             ---------------------------                                      *
         *                                                                                                              *
         *  A reverse lookup to chapterReferencesBySequence - i.e. given a chapter reference, this will give us the     *
         *  internal sequence number                                                                                    *
         *     Key:   chapter reference (from data)                                                                     *
         *     Value: chapter sequence                                                                                  *
         *                                                                                                              *
         *--------------------------------------------------------------------------------------------------------------*/
        SortedDictionary<String, int> sequenceForChapterReference = new SortedDictionary<String, int>();

        public int NoOfChaptersInBook { get => noOfChaptersInBook; set => noOfChaptersInBook = value; }
        public int Category { get => category; set => category = value; }
        public int BookId { get => bookId; set => bookId = value; }
        public string BookName { get => bookName; set => bookName = value; }
        public string ShortName { get => shortName; set => shortName = value; }

        public classMTChapter addNewChapterToBook(String chapterId)
        {
            int sequenceNo;
            classMTChapter newChapter = null;

            if (sequenceForChapterReference.ContainsKey(chapterId))
            {
                sequenceForChapterReference.TryGetValue(chapterId, out sequenceNo);
                chaptersBySequence.TryGetValue(sequenceNo, out newChapter);
            }
            else
            {
                newChapter = new classMTChapter();
                sequenceForChapterReference.Add(chapterId, noOfChaptersInBook);
                chapterReferencesBySequence.Add(noOfChaptersInBook, chapterId);
                chaptersBySequence.Add(noOfChaptersInBook++, newChapter);
            }
            return newChapter;
        }

        public classMTChapter getChapterBySequence(int seqNo)
        {
            classMTChapter newChapter = null;

            chaptersBySequence.TryGetValue(seqNo, out newChapter);
            return newChapter;
        }

        public classMTChapter getChapterByChapterNo(String chapterId)
        {
            int seqNo = -1;

            if( chapterId == null) return null;
            sequenceForChapterReference.TryGetValue(chapterId, out seqNo);
            if (seqNo == -1) return null;
            return getChapterBySequence(seqNo);
        }

        public int getSequenceByChapterNo(String chapterId)
        {
            int seqNo = -1;

            sequenceForChapterReference.TryGetValue(chapterId, out seqNo);
            return seqNo;
        }

        public String getChapterNoBySequence(int seqNo)
        {
            String chapNo = "";

            chapterReferencesBySequence.TryGetValue(seqNo, out chapNo);
            return chapNo;
        }
    }
}
