using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    public class classMTWord
    {
        bool isPrefix, hasVariant = false;
        int eRef, noOfStrongRefs = 0;
        String actualWord, unaccentedWord, bareWord, gloss, morphology, affix;
        SortedSet<int> listOfStrongRefs = new SortedSet<int>();
        classKethib_Qere wordVariant;

        public string ActualWord { get => actualWord; set => actualWord = value; }
        public string Morphology { get => morphology; set => morphology = value; }
        public int ERef { get => eRef; set => eRef = value; }
        public bool IsPrefix { get => isPrefix; set => isPrefix = value; }
        public string Affix { get => affix; set => affix = value; }
        public int NoOfStrongRefs { get => noOfStrongRefs; set => noOfStrongRefs = value; }
        public string UnaccentedWord { get => unaccentedWord; set => unaccentedWord = value; }
        public string BareWord { get => bareWord; set => bareWord = value; }
        public string Gloss { get => gloss; set => gloss = value; }
        public bool HasVariant { get => hasVariant; set => hasVariant = value; }
        public classKethib_Qere WordVariant { get => wordVariant; set => wordVariant = value; }

        public void addStrongRef(int strongRef)
        {
            if (!listOfStrongRefs.Contains(strongRef))
            {
                listOfStrongRefs.Add(strongRef);
                noOfStrongRefs++;
            }
        }

        public int getStrongRefBySeq(int seqNo)
        {
            int strongRef = 0;

            if (seqNo < noOfStrongRefs)
            {
                strongRef = listOfStrongRefs.ElementAt(seqNo);
            }
            return strongRef;
        }
    }
}
