using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classMTVerse
    {
        int wordCount = 0, chapSeq, verseSeq;
        String noteText, chapRef, verseRef;
        SortedDictionary<int, classMTWord> wordIndex = new SortedDictionary<int, classMTWord>();
        classMTVerse nextVerse;

        public int WordCount { get => wordCount; set => wordCount = value; }
        public int ChapSeq { get => chapSeq; set => chapSeq = value; }
        public int VerseSeq { get => verseSeq; set => verseSeq = value; }
        public string NoteText { get => noteText; set => noteText = value; }
        public string ChapRef { get => chapRef; set => chapRef = value; }
        public string VerseRef { get => verseRef; set => verseRef = value; }
        internal classMTVerse NextVerse { get => nextVerse; set => nextVerse = value; }

        public classMTWord addWordToVerse()
        {
            classMTWord newWord;

            newWord = new classMTWord();
            wordIndex.Add(wordCount++, newWord);
            return newWord;
        }

        public classMTWord getWord(int seqNo)
        {
            classMTWord newWord;

            wordIndex.TryGetValue(seqNo, out newWord);
            return newWord;
        }
    }
}
