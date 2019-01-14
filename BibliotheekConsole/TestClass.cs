using BibliotheekLibrary.Enums;
using System;
using static BibliotheekLibrary.Models.CollectieBibliotheek;

namespace BibliotheekConsole
{
    public static class TestClass
    {
        public static void InitTests()
        {
            if (GetItemsCount() == 0)
            {
                return;
                ItemsInCollectie.Add(Factory.CreateItem(
                        SoortItem.Boek, "To Kill A Mockingbird", "Harper Lee", 1957
                        ));

                ItemsInCollectie.Add(Factory.CreateItem(
                    SoortItem.CD, "Are We There", "Sharon Van Etten", 2018
                    ));

                ItemsInCollectie.Add(Factory.CreateItem(
                    SoortItem.CD, "In Utero", "Nirvana", 1993
                    ));

                ItemsInCollectie.Add(Factory.CreateItem(
                    SoortItem.Boek, "The Grapes Of Wrath", "John Steinbeck", 1939
                    ));

                ItemsInCollectie.Add(Factory.CreateItem(
                    SoortItem.DVD, "Stranger Things Season 1", "The Duffer Brothers", 2017
                    ));

                ItemsInCollectie.Add(Factory.CreateItem(
                    SoortItem.DVD, "The Hateful Eight", "Quentin Tarantino", 2018
                    ));

                ItemsInCollectie.Add(Factory.CreateItem(
                    SoortItem.Boek, "Extremely Loud and Incredibly Close", "Jonathan Safran Foer", 2006
                    ));

                ItemsInCollectie.Add(Factory.CreateItem(
                    SoortItem.Boek, "I Know Why The Caged Bird Sings", "Maya Angelou", 1969
                    ));

                ItemsInCollectie.Add(Factory.CreateItem(
                    SoortItem.Boek, "The Sun And Her Flowers", "Rupi Kaur", 2017
                    ));

            }

            //Leden.Add(Factory.CreateLid(
            //    "ber", "mem", new DateTime(1962, 05, 13)
            //    ));

            //Leden.Add(Factory.CreateLid(
            //    "Vanparys", "Gilbert", new DateTime(1954, 02, 12)
            //    ));

            //Leden.Add(Factory.CreateLid(
            //    "Verwée", "Jeroen", new DateTime(1987, 03, 03)
            //    ));

            //Leden.Add(Factory.CreateLid(
            //    "Van Aert", "Vanessa", new DateTime(1982, 10, 14)
            //    ));

            //Medewerkers.Add(Factory.CreateMedewerker(
            //    "admin", "admin", new DateTime(1978, 05, 22), null, null
            //    ));

            //Medewerkers.Add(Factory.CreateMedewerker(
            //    "Lambotte", "Peter", new DateTime(1978, 05, 22), null, null
            //    ));

            //Medewerkers.Add(Factory.CreateMedewerker(
            //    "Monballiu", "Alex", new DateTime(1972, 11, 04), null, null
            //    ));

            //Medewerkers.Add(Factory.CreateMedewerker(
            //    "Monballiu", "Sam", new DateTime(1983, 01, 25), null, null
            //    ));

            //Medewerkers.Add(Factory.CreateMedewerker(
            //    "Nimmegeers", "Jerry", new DateTime(1974, 05, 31), null, null
            //    ));

            //var works = File.ReadAllLines("..\\..\\guardianFix.txt");

            //for (int i = 0; i < works.Length; i++)
            //{
            //    string[] item = works[i].Split('*');
            //    if (item.Length > 1)
            //    {
            //        string titel = item[0];
            //        string auteur = item[1];
            //        ItemsInCollectie.Add(Factory.CreateItem(SoortItem.Boek, titel, auteur, 0));
            //    }
            //    if (i % 100 == 0)
            //    {
            //        Console.WriteLine("... ");
            //    }
            //}
        }
    }
}
