# Old Testament Student for Windows
OldTestamentStudent was originally focused primarily on the Hebrew/Aramaic text, as provided by Biblia Hebraica Stuttgartensia (BUS).  It provides the following utilities in a single, desktop application:
a) the text of BHS (often also referred to as the Masoretic Text - MT);
b) visual presentation of the Qere form of Kethib/Qeres with a facility to view both the Kethib and the Qere;
c) parsing and lexical information on any selected word;
d) a fairly substantial search facility;
e) a utility for taking notes on a verse by verse basis.

A parallel implementation of the Septuagint (LXX; Rhalfs edition) is also provided with similar utilities.

The lexican used for the Hebrew and Aramaic is a fairly late version of Brown, Driver and Briggs (BDB).  Most internat implementations of the OT seem to use Strong's Concordance but this is less well received in academic circles than BDB.  Hence our focus. A few caveats need to be added, however:
a) The text of the lexican entries are missing some elements (e.g. words that cannot be represented by latin text);
b) The indexing of our source data _is_ based on Strong and, as a result, may be questionable at times;
c) There are often more than one possible entry that relates to a given word, so all are provided and the user must decide which is appropriate.

In sunnary, the lexican is usually adequate for a working translation and normally points to the correct entry in BDB but doesn't replace the need for a full edition.

The lexican used for the LXX is Liddell & Scott's Intermediate lexicon.  Quite a lot of work has gone into matching a given word to a suitable entry but it is not always successful.

Installation Files
----------------

These have been provided for use as-is.  Note that it is provided for use on a single user basis: multiple installations on a single server have not been tested.  You may also need administrator rights to complete the installation.

Note also that, by default, the data files and notes are placed in the AppData folder under Roaming.  You can move them from there to a more visible location within the application itself.

Source Code
--------------

The application was developed in C# in Visual Studio 2019.  The data is simply a dump of all files from the project.  We have included the Setup Project but this will probably cause an error if you try using the project directly.  All you need to do, however, is delete that project and it should work.

Note that the Help documentation functions as a kind of tutorial for the application and you will find a contact email address there.  Feel free to contact me if you have any problems or questions.

Len Clark
March 2022