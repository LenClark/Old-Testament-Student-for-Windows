using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTestamentStudent
{
    public class classBDBEntry
    {
        int noOfEntries = 0;
        String entryWord = "";
        SortedDictionary<int, String> bdbEntryList = new SortedDictionary<int, string>();

        public int NoOfEntries { get => noOfEntries; set => noOfEntries = value; }
        public string EntryWord { get => entryWord; set => entryWord = value; }

        public void addEntry(int bdbNo, String entryItem, String entryText)
        {
            bdbEntryList.Add(bdbNo, entryText);
            if (noOfEntries == 0) entryWord = entryItem;
            noOfEntries++;
        }

        public String getEntryBySequence(int seqNo)
        {
            KeyValuePair<int, String> returnedPair;

            if (seqNo < noOfEntries)
            {
                returnedPair = bdbEntryList.ElementAt(seqNo);
                return returnedPair.Value;
            }
            return "";
        }
    }
}
