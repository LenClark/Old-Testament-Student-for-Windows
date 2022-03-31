using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classLXXBook
    {
        /*============================================================================================================*
         *                                                                                                            *
         *                                              classLXXBook                                                  *
         *                                              ------------                                                  *
         *                                                                                                            *
         *  In essence, we want to identify each chapter that belongs to a given book.  At root, information about    *
         *    the chapter is provided by the class classLXXChapter.  However, we need to cater for the possibility    *
         *    that the chapter number is _not_ a simple integer but may contain alphanumerics (e.g. 12a).  So, we     *
         *    a) key the list of class instances on a sequential integer.  The sequence has no significance other     *
         *       than ensuring uniqueness.  (It will actually be generated in the sequence the chapters are           *
         *       encountered in the source data.)                                                                     *
         *    b) we separately provide a lookup of this sequence number which gives the String-based version of the   *
         *       chapter "number" (which we will call a "chapter reference"), which is recorded in the sequence       *
         *       listed in the source data.  This means that numbers may also be out of strict sequence.              *
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
         *  This reflects the categorisation used for MT books: the books of LXX are placed in the same category groups *
         *    as their Hebrew (and Aramaic) equivalents.  However, we also need an additional category for those books  *
         *    not found in the Hebrew and Aramaic canon.                                                                *
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
         *      7       The pseudo-canonical books: 1 Esdras, Judith, Tobit, 1-4 Macabees, Odes, Wisdom of Solomon,     *
         *                 Ecclesiasticus, Psalms of Solomon, Baruch, Epistle of Jeremiah, Bel and the Dragon, Susanna  *
         *                                                                                                              *
         *--------------------------------------------------------------------------------------------------------------*/
        int noOfChaptersInBook = 0, category, bookId;
        String shortName, commonName, lxxName, fileName;

        /*--------------------------------------------------------------------------------------------------------------*
         *                                                                                                              *
         *                                                chaptersBySequence                                            *
         *                                                ------------------                                            *
         *                                                                                                              *
         *  A look-up list of chapter class instances, keyed by a sequence no.                                          *
         *      Key:   chapter Sequence                                                                                 *
         *      Value: the class instance address                                                                       *
         *                                                                                                              *
         *--------------------------------------------------------------------------------------------------------------*/
        SortedDictionary<int, classLXXChapter> chaptersBySequence = new SortedDictionary<int, classLXXChapter>();

        /*--------------------------------------------------------------------------------------------------------------*
         *                                                                                                              *
         *                                            chapterReferencesBySequence                                       *
         *                                            ---------------------------                                       *
         *                                                                                                              *
         *  A list that will convert the simple chapter sequence to the chapter reference, as given in the data         *
         *      Key:   chapter sequence                                                                                 *
         *      Value: the chapter "number" provided from data (which can be e.g. 12a)                                  *
         *                                                                                                              *
         *--------------------------------------------------------------------------------------------------------------*/
        SortedDictionary<int, String> chapterReferencesBySequence = new SortedDictionary<int, String>();

        /*--------------------------------------------------------------------------------------------------------------*
         *                                                                                                              *
         *                                            sequenceForChapterReference                                       *
         *                                            ---------------------------                                       *
         *                                                                                                              *
         *  A reverse lookup to chapterReferencesBySequence - i.e. given a data chapter reference, this will give us    *
         *    the internal sequence number                                                                              *
         *      Key:   chapter number (from data)                                                                       *
         *      Value: chapter sequence                                                                                 *
         *                                                                                                              *
         *--------------------------------------------------------------------------------------------------------------*/
        SortedDictionary<String, int> sequenceForChapterReference = new SortedDictionary<String, int>();

        /*--------------------------------------------------------------------------------------------------------------*
         *                                                                                                              *
         *                              secondLowerBound                                                                *
         *                              ----------------                                                                *
         *                                                                                                              *
         *  This is used specifically for those books where a chapter may be separated by another, out-of-sequence      *
         *    chapter.  For example, we may find chapter 24 followed by chapter 30 and later the rest of chapter 24.    *
         *                                                                                                              *
         *    Key:   the chapter, as provided within the text.                                                          *
         *    Value: an array of all verse values that are found in the *second* occurrence of the chapter.             *
         *                                                                                                              *
         *--------------------------------------------------------------------------------------------------------------*/
        SortedDictionary<String, String[]> secondLowerBound = new SortedDictionary<String, String[]>();

        public int NoOfChaptersInBook { get => noOfChaptersInBook; set => noOfChaptersInBook = value; }
        public int BookId { get => bookId; set => bookId = value; }
        public string ShortName { get => shortName; set => shortName = value; }
        public string CommonName { get => commonName; set => commonName = value; }
        public string LxxName { get => lxxName; set => lxxName = value; }
        public string FileName { get => fileName; set => fileName = value; }
        public int Category { get => category; set => category = value; }

        public classLXXBook()
        {
            String[] chap24 = { "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34" };
            String[] chap30 = { "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33" };
            String[] chap31 = { "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                                "30", "31" };

            secondLowerBound.Add("24", chap24);
            secondLowerBound.Add("30", chap30);
            secondLowerBound.Add("31", chap31);
        }

        public classLXXChapter addNewChapterToBook(String chapterRef)
        {
            /*==========================================================================================================*
             *                                                                                                          *
             *                                           addNewChapterToBook                                            *
             *                                           ===================                                            *
             *                                                                                                          *
             *  This method will:                                                                                       *
             *  a) create a new instance of the chapter;                                                                *
             *  b) accept the text chapter designation of the source file (= chapterId);                                *
             *  c) generate a sequential chapter reference number (= noOfChaptersInBook);                               *
             *  d) add the chapter instance to a List, keyed on the sequential value;                                   *
             *  e) add the text chapter designation to a list, keyed by the sequential value;                           *
             *  f) create a reverse reference list to the sequential value so that we can retrieve the sequence number  *
             *       if we know the chapter designation                                                                 *
             *                                                                                                          *
             *  This approach has been used because the LXX sometimes has some non-sequential chapters (and even some   *
             *       chapters that aren't actual numbers).  However, there is a problem: in Proverbs, Rahlfs chapter    *
             *       designation is as follows:                                                                         *
             *           1 - 24 (v22e), 30 (vv 1-14), 24 again (vv 23-34), 30 again (vv 15-33), 31 (vv 1-9), 32 - 36,   *
             *           31 (vv 11-31).                                                                                 *
             *       As a result, the reverse list (keyed on the chapter designation) is not unique for chapters 24, 30 *
             *       and 31.                                                                                            *
             *                                                                                                          *
             *  To enable this, we have added a wierd extra:                                                            *
             *    occurrenceList will be keyed on the chapter reference with a value of the last occurrence of the      *
             *    chapter.  So, most will have a value of 0 but, for example, the second time we hit 24, the occurrence *
             *    will be adjusted up to 1. chapterSequence will then *not* be keyed on the chapter designation alone   *
             *    but a concatenation of chapter designation and occurrence (seperated by "-").  This will ensure       *
             *    uniqueness.  Retrieving the chapter designation will require the removal of this occurrence value.    *
             *                                                                                                          *
             *==========================================================================================================*/
            int sequenceNo;
            String substituteChapterId;
            classLXXChapter newChapter = null;

            if (sequenceForChapterReference.ContainsKey(chapterRef))
            {
                sequenceForChapterReference.TryGetValue(chapterRef, out sequenceNo);
                chaptersBySequence.TryGetValue(sequenceNo, out newChapter);
            }
            else
            {
                newChapter = new classLXXChapter();
                chaptersBySequence.Add(noOfChaptersInBook, newChapter);
                if (BookId == 28)
                {
                    substituteChapterId = chapterRef;
                    if (sequenceForChapterReference.ContainsKey(chapterRef))
                    {
                        substituteChapterId += "b";
                    }
                    chapterReferencesBySequence.Add(noOfChaptersInBook, substituteChapterId);
                    sequenceForChapterReference.Add(substituteChapterId, noOfChaptersInBook);
                }
                else
                {
                    chapterReferencesBySequence.Add(noOfChaptersInBook, chapterRef);
                    sequenceForChapterReference.Add(chapterRef, noOfChaptersInBook);
                }
                noOfChaptersInBook++;
            }
            return newChapter;
        }

        public classLXXChapter getChapterBySequence(int seqNo)
        {
            classLXXChapter newChapter = null;

            chaptersBySequence.TryGetValue(seqNo, out newChapter);
            return newChapter;
        }

        public classLXXChapter getChapterByChapterNo(String chapterRef)
        {
            int seqNo = -1;

            sequenceForChapterReference.TryGetValue(chapterRef, out seqNo);
            if (seqNo == -1) return null;
            return getChapterBySequence(seqNo);
        }

        public int getSequenceByChapterNo(String chapterRef)
        {
            int seqNo = -1;

            sequenceForChapterReference.TryGetValue(chapterRef, out seqNo);
            return seqNo;
        }

        public String getChapterNoBySequence(int seqNo)
        {
            String chapterRef = "";

            chapterReferencesBySequence.TryGetValue(seqNo, out chapterRef);
            return chapterRef;
        }
    }
}
