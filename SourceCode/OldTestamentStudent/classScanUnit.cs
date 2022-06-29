using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classScanUnit
    {
        int chapterSeq, verseSeq, wordSeq;
        String chapterRef, verseRef;
        classMTWord scanWord;

        public int ChapterSeq { get => chapterSeq; set => chapterSeq = value; }
        public int VerseSeq { get => verseSeq; set => verseSeq = value; }
        public int WordSeq { get => wordSeq; set => wordSeq = value; }
        public string ChapterRef { get => chapterRef; set => chapterRef = value; }
        public string VerseRef { get => verseRef; set => verseRef = value; }
        public classMTWord ScanWord { get => scanWord; set => scanWord = value; }
    }
}
