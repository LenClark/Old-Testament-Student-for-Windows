using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    class classLXXVerse
    {
        int wordCount = 0, chapSeq, verseSeq;
        String noteText, chapRef, verseRef;
        SortedDictionary<int, classLXXWord> wordIndex = new SortedDictionary<int, classLXXWord>();
        classLXXVerse previousVerse, nextVerse;

        public int WordCount { get => wordCount; set => wordCount = value; }
        public int ChapSeq { get => chapSeq; set => chapSeq = value; }
        public int VerseSeq { get => verseSeq; set => verseSeq = value; }
        public string NoteText { get => noteText; set => noteText = value; }
        public string ChapRef { get => chapRef; set => chapRef = value; }
        public string VerseRef { get => verseRef; set => verseRef = value; }
        internal classLXXVerse PreviousVerse { get => previousVerse; set => previousVerse = value; }
        internal classLXXVerse NextVerse { get => nextVerse; set => nextVerse = value; }

        public classLXXWord addWordToVerse()
        {
            classLXXWord newWord;

            newWord = new classLXXWord();
            wordIndex.Add(wordCount++, newWord);
            return newWord;
        }

        public classLXXWord getWord(int seqNo)
        {
            classLXXWord newWord;

            wordIndex.TryGetValue(seqNo, out newWord);
            return newWord;
        }
    }
}
