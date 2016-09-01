using Microsoft.VisualStudio.TestTools.UnitTesting;
using EMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMLibrary.Tests
{
    [TestClass()]
    public class NameExtractorTests
    {
        string[] validLastNames = { "Paprocki", "Foller", "Dilliard", "Wieser", "Marrier", "Amigon", "Maclead", "Caldarera", "Ruta", "Albares" };
        string[] validFirstNames = { "James", "Josephine", "Lenna", "Donette", "Simona", "Mitsue", "Leota", "Sage", "Kris", "Minna" };
        string[] validTitles = { "Mr", "Miss", "Ms", "Mrs", "Mx", "Sir", "Madam", "Dame", "Lord", "Lady", "Adv","Dr","Prof","Fr","Pr","Br", "Sr", "Elder", "Rabbi" };
        string[] validSpacers = { " ", "    ", "       ", "\t   \t\t", "\t\t\t\t", "\t", " \t   ", "  ", "\t   \t    \t", "\t\t \t" };

        [TestMethod()]
        public void ExtractTestRule1()
        {
            //If the name contains only one word, it is treated as the last name

            foreach (string lastName in validLastNames)
            {
                runDifferentWays(lastName, new NameExtractionResult() { LastName = lastName });
            }
        }

        [TestMethod()]
        public void ExtractTestRule2()
        {
            //If the name contains two words and the first word is a title, the second word is treated as the last name

            foreach (string spacer in validSpacers)
            {
                foreach(string title in validTitles)
                {
                    foreach (string lastName in validLastNames)
                    {
                        runDifferentWays(title + spacer + lastName, 
                            new NameExtractionResult() { Title = title, LastName = lastName });
                    }
                }
            }
        }

        [TestMethod()]
        public void ExtractTestRule3()
        {
            // If the name contains two or more words and the first word is not a title, the first two words are treated as the first name and the last name

            foreach (string spacer in validSpacers)
            {
                foreach (string firstName in validFirstNames)
                {
                    foreach (string lastName in validLastNames)
                    {
                        runDifferentWays(firstName + spacer + lastName,
                            new NameExtractionResult() { FirstName = firstName, LastName = lastName });
                    }
                }
            }
        }

        [TestMethod()]
        public void ExtractTestRule4()
        {
            //The words have the strict order: the title, the first name and the last name

            foreach (string spacer in validSpacers)
            {
                foreach (string spacer2 in validSpacers)
                {
                    foreach (string title in validTitles)
                    {
                        foreach (string firstName in validFirstNames)
                        {
                            foreach (string lastName in validLastNames)
                            {
                                runDifferentWays(title + spacer + firstName + spacer2 + lastName,
                                    new NameExtractionResult() { Title = title, FirstName = firstName, LastName = lastName });
                            }
                        }
                    }
                }
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void ExtractTestRule5_1()
        {
            //If there are more than three words, an exception is thrown
            MockNameProvider prv = new MockNameProvider("a b c d");
            (new NameExtractor(prv)).Extract();
        }

        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void ExtractTestRule5_2()
        {
            //If there are more than three words, an exception is thrown
            MockNameProvider prv = new MockNameProvider(string.Join(validSpacers[0], validFirstNames));
            (new NameExtractor(prv)).Extract();
        }

        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void ExtractTestRule5_3()
        {
            //If there are more than three words, an exception is thrown
            MockNameProvider prv = new MockNameProvider("Mr John Wood Jr");
            (new NameExtractor(prv)).Extract();
        }

        [TestMethod()]
        public void ExtractTestConstructor()
        {
            //The class' constructor takes an INameProvider instance and will call this interface in order to fetch the name to break down
            //Name should be extracted on construction time, not extraction
            MockNameProvider prv = new MockNameProvider(validTitles[0] + " " + validFirstNames[0] + " " + validLastNames[0]);
            NameExtractor exractor = new NameExtractor(prv);

            prv.replaceMockName(validTitles[1] + " " + validFirstNames[1] + " " + validLastNames[1]);

            CheckResults(exractor.Extract(), new NameExtractionResult()
            {
                Title = validTitles[0],
                FirstName = validFirstNames[0],
                LastName = validLastNames[0]
            });
        }

        #region Helpers
        private void runOneWay(string source, NameExtractionResult expected)
        {
            MockNameProvider prv = new MockNameProvider(source);
            CheckResults((new NameExtractor(prv)).Extract(), expected);
        }

        private void runDifferentWays(string source, NameExtractionResult expected)
        {
            foreach(string spacer in validSpacers)
            {
                runOneWay(spacer + source, expected); // check Leading spacers
                runOneWay(source + spacer, expected); // check Trailing spaces
            }

            foreach (string leadingSpacer in validSpacers)
            {
                foreach (string trailingSpacer in validSpacers)
                {
                    runOneWay(leadingSpacer + source + trailingSpacer, expected); // check allaround
                }
            }

            runOneWay(source, expected); // simple run
        }

        private void CheckResults(NameExtractionResult actual, NameExtractionResult expected)
        {
            if(expected.Title == null)
            {
                Assert.IsTrue(string.IsNullOrEmpty(actual.Title), "Title should be empty.");
            }
            else
            {
                Assert.AreEqual(expected.Title, actual.Title, "Titles does not match.");
            }

            if (expected.FirstName == null)
            {
                Assert.IsTrue(string.IsNullOrEmpty(actual.FirstName), "First name should be empty.");
            }
            else
            {
                Assert.AreEqual(expected.FirstName, actual.FirstName, "First names does not match.");
            }

            if (expected.LastName == null)
            {
                Assert.IsTrue(string.IsNullOrEmpty(actual.LastName), "Last name should be empty.");
            }
            else
            {
                Assert.AreEqual(expected.LastName, actual.LastName, "Last names does not match.");
            }
        }
        #endregion
    }
}