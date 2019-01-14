using BibliotheekLibrary.Enums;
using ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BibliotheekLibrary.Models
{
    public static class CollectieManager
    {
        private const string collectionFilename = "collection";
        const string membersFilename = "leden";
        const string adminsFilename = "admins";
        private static readonly string _path = Environment.CurrentDirectory;
        private const char catSep = '*';

        public static List<Item> GetCollection()
        {
            if (File.Exists(collectionFilename))
            {
                try
                {
                    return ObjectSerializer<List<Item>>.DeserializeBinaryObject(collectionFilename);
                }
                catch (Exception err)
                {
                    throw;
                }
                return new List<Item>();
            }

            else return new List<Item>();
        }

        public static string SaveCollection(List<Item> list)
        {
            try
            {
                ObjectSerializer<List<Item>>.SerializeBinaryObject(list, collectionFilename);
            }
            catch (Exception err)
            {
                return err.Message;
            }
            return "Successful";
        }


        public static void ImportCollectionFromCSV()
        {
            if (!File.Exists(collectionFilename + ".txt"))
            {
                return;
            }

            using (FileStream fs = new FileStream(collectionFilename + ".txt", FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (!line.Contains("START"))
                        {
                            string[] item = line.Split(catSep);

                            try
                            {

                                int id = int.Parse(item[0]);
                                SoortItem soort = (SoortItem)Enum.Parse(typeof(SoortItem), item[1]);
                                string titel = item[2];
                                string auteur = item[3];
                                int jaartal = int.Parse(item[4]);
                                bool afgevoerd = bool.Parse(item[5]);
                                bool uitgeleend = bool.Parse(item[6]);
                                bool gereserveerd = bool.Parse(item[7]);
                                string reservatienaam = item.Length > 8 ? item[8] : String.Empty;

                                CollectieBibliotheek.ItemsInCollectie.Add(
                                    Factory.CreateItem(id, soort, titel, auteur, jaartal, afgevoerd, uitgeleend, gereserveerd, reservatienaam)
                                    );
                            }
                            catch (Exception)
                            {
                                Debug.WriteLine("Failed import: " + line);
                            }
                        }
                    }
                }
            }
        }



        #region Export

        #region Collection

        public static void ExportCollectionAsCSV()
        {
            var list = CollectieBibliotheek.ItemsInCollectie;
            using (FileStream fs = new FileStream(collectionFilename + ".txt", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("id;soort;titel;auteur;jaartal;afgevoerd;uitgeleend;gereserveerd;reservatienaam");
                    foreach (var item in list)
                    {
                        string id = item.ItemId.ToString();
                        string soort = item.SoortItem.ToString();
                        string titel = item.Titel;
                        string auteur = item.Auteur;
                        string jaartal = item.Jaartal.ToString();
                        string afgevoerd = item.Afgevoerd.ToString();
                        string uitgeleend = item.Uitgeleend.ToString();
                        string gereserveerd = item.Gereserveerd.ToString();
                        string reservatienaam = item.Reservatienaam ?? String.Empty;

                        sw.WriteLine($"{id}{catSep}{soort}{catSep}{titel}{catSep}{auteur}{catSep}{jaartal}{catSep}{afgevoerd}{catSep}{uitgeleend}{catSep}{gereserveerd}{catSep}{reservatienaam}");
                    }
                }
            }
        }
        #endregion
        #region Members

        public static void ExportMembersAsCSV()
        {
            var list = CollectieBibliotheek.Leden;
            using (FileStream fs = new FileStream(CreateFolderIfNecessary(membersFilename) + membersFilename + ".txt", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("STARTfamilienaam;voornaam;geboorteDatum");
                    foreach (var member in list)
                    {

                        sw.WriteLine($"{member.Familienaam}{catSep}{member.Voornaam}{catSep}{member.Geboortedatum.ToShortDateString()}");

                        ExportMemberCheckoutHistory(member, CreateFolderIfNecessary(membersFilename));
                        ExportMemberCheckoutCurrent(member, CreateFolderIfNecessary(membersFilename));
                    }
                }
            }
        }

        public static void ExportMemberCheckoutHistory(IMember member, string folder)
        {
            using (FileStream fs = new FileStream(folder + $"{member.Familienaam}{member.Voornaam}_history.txt", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (var item in member.Uitleenhistoriek)
                    {
                        sw.WriteLine($"{item.Item1.ToString()}{catSep}" +
                            $"{item.Item2.ItemId}{catSep}" +
                            $"{item.Item2.SoortItem}{catSep}" +
                            $"{item.Item2.Titel}{catSep}" +
                            $"{item.Item2.Auteur}{catSep}" +
                            $"{item.Item2.Jaartal}{catSep}" +
                            $"{item.Item2.Afgevoerd}{catSep}" +
                            $"{item.Item2.Uitgeleend}{catSep}" +
                            $"{item.Item2.Gereserveerd}{catSep}" +
                            $"{item.Item2.Reservatienaam}{catSep}");
                    }
                }
            }
        }

        public static void ExportMemberCheckoutCurrent(IMember member, string folder)
        {
            using (FileStream fs = new FileStream(folder + $"{member.Familienaam}{member.Voornaam}_current.txt", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (var item in member.ItemsUitgeleend)
                    {
                        sw.WriteLine(
                            $"{item.ItemId}{catSep}" +
                            $"{item.SoortItem}{catSep}" +
                            $"{item.Titel}{catSep}" +
                            $"{item.Auteur}{catSep}" +
                            $"{item.Jaartal}{catSep}" +
                            $"{item.Afgevoerd}{catSep}" +
                            $"{item.Uitgeleend}{catSep}" +
                            $"{item.Gereserveerd}{catSep}" +
                            $"{item.Reservatienaam}{catSep}");
                    }
                }
            }
        }

        #endregion
        #region Admins

        public static void ExportAdminsAsCSV()
        {
            var list = CollectieBibliotheek.Medewerkers;
            string folder = CreateFolderIfNecessary(adminsFilename);
            using (FileStream fs = new FileStream(folder + adminsFilename + ".txt", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("STARTfamilienaam;voornaam;geboorteDatum");
                    foreach (var member in list)
                    {

                        sw.WriteLine($"{member.Familienaam}{catSep}{member.Voornaam}{catSep}{member.Geboortedatum.ToShortDateString()}");

                        ExportMemberCheckoutHistory(member, folder);
                        ExportMemberCheckoutCurrent(member, folder);
                    }
                }
            }
        }
        #endregion
        #endregion

        public static void ImportAdminsFromCSV()
        {
            string file = _path + $"/{adminsFilename}/" + adminsFilename + ".txt";
            if (!File.Exists(file))
            {
                return;
            }

            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (!line.Contains("START"))
                        {
                            string[] item = line.Split(catSep);

                            try
                            {
                                string familienaam = item[0];
                                string voornaam = item[1];
                                DateTime birthDate = DateTime.Parse(item[2]);

                                CollectieBibliotheek.Medewerkers.Add(
                                    Factory.CreateMedewerker(familienaam, voornaam, birthDate, null, null)
                                    );
                            }
                            catch (Exception)
                            {
                                Debug.WriteLine("Failed import: " + line);
                            }
                        }
                    }
                }
            }

            foreach (var admin in CollectieBibliotheek.Medewerkers)
            {
                admin.ItemsUitgeleend = ImportAdminCheckoutCurrent(admin.Familienaam, admin.Voornaam);
                admin.Uitleenhistoriek = ImportAdminCheckoutHistory(admin.Familienaam, admin.Voornaam);
            }
        }

        public static Item GetItemFromString(string line, bool containsDate = false)
        {
            string[] item = line.Split(catSep);

            if (containsDate)
            {
                try
                {
                    int id = int.Parse(item[1]);
                    SoortItem soort = (SoortItem)Enum.Parse(typeof(SoortItem), item[2]);
                    string titel = item[3];
                    string auteur = item[4];
                    int jaartal = int.Parse(item[5]);
                    bool afgevoerd = bool.Parse(item[6]);
                    bool uitgeleend = bool.Parse(item[7]);
                    bool gereserveerd = bool.Parse(item[8]);
                    string reservatienaam = item.Length > 9 ? item[9] : String.Empty;

                    return Factory.CreateItem(id, soort, titel, auteur, jaartal, afgevoerd, uitgeleend, gereserveerd, reservatienaam);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Failed import: " + line);
                    return null;
                }
            }

            try
            {
                int id = int.Parse(item[0]);
                SoortItem soort = (SoortItem)Enum.Parse(typeof(SoortItem), item[1]);
                string titel = item[2];
                string auteur = item[3];
                int jaartal = int.Parse(item[4]);
                bool afgevoerd = bool.Parse(item[5]);
                bool uitgeleend = bool.Parse(item[6]);
                bool gereserveerd = bool.Parse(item[7]);
                string reservatienaam = item.Length > 8 ? item[8] : String.Empty;

                return Factory.CreateItem(id, soort, titel, auteur, jaartal, afgevoerd, uitgeleend, gereserveerd, reservatienaam);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed import: " + line);
                return null;
            }
        }

        public static List<Item> ImportAdminCheckoutCurrent(string familienaam, string voornaam)
        {
            string file = _path + $"/{adminsFilename}/" + familienaam + voornaam + "_current.txt";
            if (!File.Exists(file))
            {
                return null;
            }

            var list = new List<Item>();

            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (!line.Contains("START"))
                        {
                            try
                            {
                                list.Add(GetItemFromString(line));
                            }
                            catch (Exception)
                            {
                                Debug.WriteLine("Failed import: " + line);
                            }
                        }
                    }

                    return list;
                }
            }
        }

        public static List<(DateTime,Item)> ImportAdminCheckoutHistory(string familienaam, string voornaam)
        {
            string file = _path + $"/{adminsFilename}/" + familienaam + voornaam + "_history.txt";
            if (!File.Exists(file))
            {
                return null;
            }

            var list = new List<(DateTime,Item)>();

            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (!line.Contains("START"))
                        {
                            try
                            {
                                list.Add((DateTime.Parse(line.Split(catSep)[0]), GetItemFromString(line, true)));
                            }
                            catch (Exception)
                            {
                                Debug.WriteLine("Failed import: " + line);
                            }
                        }
                    }

                    return list;
                }
            }
        }


        #region Helper methods
        public static string CreateFolderIfNecessary(string folder)
        {
            if (!Directory.Exists(_path + $"/{folder}/"))
            {
                Directory.CreateDirectory(_path + $"/{folder}/");
            }

            return _path + $"/{folder}/";
        } 
        #endregion
    }
}
