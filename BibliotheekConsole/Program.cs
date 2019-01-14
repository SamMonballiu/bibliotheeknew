using BibliotheekLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotheekLibrary.Enums;
using BibliotheekLibrary;
using System.Threading;

namespace BibliotheekConsole
{
    class Program
    {
        public const int waitTime = 500;

        public static IUser activeUser;
        public enum UserType { Bezoeker, Member, Admin, None }
        public static UserType userType;

        static void Main(string[] args)
        {
            Console.WindowWidth = Console.LargestWindowWidth - 20;

            userType = UserType.None;

            CollectieManager.ImportCollectionFromCSV();
            CollectieManager.ImportAdminsFromCSV();
            TestClass.InitTests();

            while (true)
            {
                Console.Clear();

                Console.WriteLine(String.Join("\n", CollectieBibliotheek.Leden));
                Console.WriteLine(String.Join("\n", CollectieBibliotheek.Medewerkers));

                Console.WriteLine("1. Log in");
                Console.WriteLine("0. Exit");

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.NumPad0:
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.NumPad1:
                        DoLogIn();
                        break;
                    case ConsoleKey.E:
                        CollectieManager.ExportMembersAsCSV();
                        break;
                    case ConsoleKey.A:
                        CollectieManager.ExportAdminsAsCSV();
                        break;
                    default:
                        break;
                }

                switch (userType)
                {
                    case UserType.Bezoeker:
                        ShowBezoekerMenu();
                        break;
                    case UserType.Member:
                        ShowMemberMenu();
                        break;
                    case UserType.Admin:
                        ShowAdminMenu();
                        break;
                    case UserType.None:
                        break;
                    default:
                        break;
                }
            }

        }

        public static void DoLogIn()
        {
            string voornaam;
            string familienaam;

            while (true)
            {
                Console.Write("Voornaam: ");
                voornaam = Console.ReadLine().Trim();
                if (!String.IsNullOrEmpty(voornaam)) break;
            }

            while (true)
            {
                Console.Write("Familienaam: ");
                familienaam = Console.ReadLine().Trim();
                if (!String.IsNullOrEmpty(familienaam)) break;
            }

            if (ListContainsName<Lid>(CollectieBibliotheek.Leden, familienaam, voornaam))
            {
                userType = UserType.Member;
                activeUser = CollectieBibliotheek.Leden.First(x => x.Familienaam == familienaam && x.Voornaam == voornaam);
            }

            else if (ListContainsName(CollectieBibliotheek.Medewerkers, familienaam, voornaam))
            {
                activeUser = CollectieBibliotheek.Medewerkers.First(x => x.Familienaam == familienaam && x.Voornaam == voornaam);
                userType = UserType.Admin;
            }

            else
            {
                activeUser = Factory.CreateBezoeker(familienaam, voornaam);
                userType = UserType.Bezoeker;
            }
        }

        public static bool ListContainsName<T>(List<T> list, string familienaam, string voornaam) where T : Bezoeker
        {
            return list.Any(x => x.Familienaam == familienaam && x.Voornaam == voornaam);
        }

        public static void ShowBezoekerMenu()
        {
            bool continueMenu = true;
            while (continueMenu)
            {
                Console.WriteLine($"\n[{activeUser.Voornaam} {activeUser.Familienaam}]");
                Console.WriteLine("1. Registreer je als lid");
                Console.WriteLine("2. Zoek een item op titel of itemId");
                Console.WriteLine("3. Toon overzichten");
                Console.WriteLine("0. Log out");

                var key = Console.ReadKey(true).Key;

                Dictionary<ConsoleKey, Action> visitorKeyMap = new Dictionary<ConsoleKey, Action>()
                {
                    { ConsoleKey.NumPad1, RegisterMember },
                    { ConsoleKey.NumPad2, SearchCollection },
                    { ConsoleKey.NumPad0, new Action(() => { LogOut(); continueMenu =false; })},
                    { ConsoleKey.NumPad3, new Action(() => { ShowCollection(); Console.ReadKey(); }) }
                };

                if (visitorKeyMap.ContainsKey(key))
                {
                    visitorKeyMap[key]();
                }
                Thread.Sleep(waitTime);
            }

            void RegisterMember()
            {
                while (true)
                {

                    Console.Write("Geboortedatum: ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime gebDatum))
                    {
                        activeUser.RegistreerLid(gebDatum);
                        continueMenu = false;
                        Console.WriteLine("U bent nu geregistreerd als lid. Gelieve opnieuw in te loggen.");
                        Console.ReadKey(true);
                        LogOut();
                        break;
                    }
                }
            }
        }

        public static void ShowMemberMenu()
        {
            bool continueMenu = true;
            while (continueMenu)
            {
                Console.Clear();
                Console.WriteLine($"\n[{activeUser.Voornaam} {activeUser.Familienaam}]");
                Console.WriteLine("Uitgeleende items: " + (activeUser as IMember).ItemsUitgeleend.Count);
                Console.WriteLine("1. Zoek een item op titel of itemId");
                Console.WriteLine("2. Toon overzichten");
                Console.WriteLine("3. Leen een item uit");
                Console.WriteLine("T. Breng een item terug");
                Console.WriteLine("4. Reserveer een item");
                Console.WriteLine("5. Bekijk uitleenhistoriek");
                Console.WriteLine("6. Bekijk uitgeleende items");
                Console.WriteLine("0. Log out");

                var key = Console.ReadKey(true).Key;

                Dictionary<ConsoleKey, Action> memberKeyMap = new Dictionary<ConsoleKey, Action>()
                {
                    { ConsoleKey.NumPad0, new Action(() => { LogOut(); continueMenu =false; })},
                    { ConsoleKey.NumPad1, SearchCollection },
                    { ConsoleKey.NumPad2, new Action(() => { ShowCollection(); Console.ReadKey(); }) },
                    { ConsoleKey.NumPad3, CheckItemOut },
                    { ConsoleKey.T, ReturnItem },
                    { ConsoleKey.NumPad4, new Action(() => ReserveItem(SelectFromCollection(CollectieBibliotheek.ItemsInCollectie.Where(x => !x.Afgevoerd)))) },
                    { ConsoleKey.NumPad5, new Action(() => { ShowCheckoutHistory(activeUser); Console.ReadKey(true);})},
                    { ConsoleKey.NumPad6, new Action(() => { ShowActiveCheckouts(activeUser); Console.ReadKey(true);})}
                };

                if (memberKeyMap.ContainsKey(key))
                {
                    memberKeyMap[key]();
                }

                Thread.Sleep(waitTime);
            }
        }

        public static void ShowAdminMenu()
        {
            bool continueMenu = true;
            while (continueMenu)
            {
                Console.Clear();
                Console.WriteLine($"\n[{activeUser.Voornaam} {activeUser.Familienaam}] ADMIN MENU");
                Console.WriteLine("Uitgeleende items: " + (activeUser as IMember).ItemsUitgeleend.Count);
                Console.WriteLine("1. Zoek een item op titel of itemId");
                Console.WriteLine("2. Toon overzichten");
                Console.WriteLine("3. Leen een item uit");
                Console.WriteLine("T. Breng een item terug");
                Console.WriteLine("4. Reserveer een item");
                Console.WriteLine("5. Bekijk uitleenhistoriek");
                Console.WriteLine("6. Bekijk uitgeleende items");
                Console.WriteLine("7. Promoveer een lid naar medewerker");
                Console.WriteLine("8. Voeg een item toe aan de collectie");
                Console.WriteLine("0. Log out");

                var key = Console.ReadKey(true).Key;

                Dictionary<ConsoleKey, Action> memberKeyMap = new Dictionary<ConsoleKey, Action>()
                {
                    { ConsoleKey.NumPad0, new Action(() => { LogOut(); continueMenu =false; })},
                    { ConsoleKey.NumPad1, SearchCollection },
                    { ConsoleKey.NumPad2, new Action(() => { ShowCollection(); Console.ReadKey(); }) },
                    { ConsoleKey.NumPad3, CheckItemOut },
                    { ConsoleKey.T, ReturnItem },
                    { ConsoleKey.NumPad4, new Action(() => ReserveItem(SelectFromCollection(CollectieBibliotheek.ItemsInCollectie.Where(x => !x.Afgevoerd)))) },
                    { ConsoleKey.NumPad5, new Action(() => { ShowCheckoutHistory(activeUser); Console.ReadKey(true);})},
                    { ConsoleKey.NumPad6, new Action(() => { ShowActiveCheckouts(activeUser); Console.ReadKey(true);})},
                    { ConsoleKey.NumPad7, new Action(() => PromoteToAdmin(SelectFromCollection<Lid>(CollectieBibliotheek.Leden.ToArray(), "Selecteer een gebruiker:")))},
                    { ConsoleKey.NumPad8, new Action(() => AddToCollection()) },
                };

                if (memberKeyMap.ContainsKey(key))
                {
                    memberKeyMap[key]();
                }

                Thread.Sleep(waitTime);
            }
        }

        private static void ReserveItem(Item item)
        {
            var member = activeUser as IMember;
            member.Reserveren(item);
        }

        private static void AddToCollection()
        {
            SoortItem itemSoort;

            Console.WriteLine("Type:");
            int counter = 0;
            var choices = Enum.GetValues(typeof(SoortItem));

            foreach (var item in choices)
            {
                Console.WriteLine($"{++counter} {item}");
            }

            while (true)
            {
                Console.Write("? ");
                if (int.TryParse(Console.ReadLine(), out int soort))
                {
                    if (soort <= choices.Length && soort > 0)
                    {
                        itemSoort = (SoortItem)Enum.Parse(typeof(SoortItem), (soort - 1).ToString());
                        break;
                    }
                }
            }

            Console.Write("Titel: ");
            string titel = Console.ReadLine();

            Console.Write("Auteur: ");
            string auteur = Console.ReadLine();

            int jaartal;
            while (true)
            {
                Console.Write("Jaartal: ");
                if (int.TryParse(Console.ReadLine(), out jaartal))
                {
                    break;
                }
            }

            CollectieBibliotheek.ItemsInCollectie.Add(Factory.CreateItem(itemSoort, titel, auteur, jaartal));
            var result = CollectieManager.SaveCollection(CollectieBibliotheek.ItemsInCollectie);
            Console.WriteLine(result);
        }

        private static T SelectFromCollection<T>(IEnumerable<T> list, string message = "")
        {
            Console.WriteLine(message);
            var listArray = list.ToArray();
            int counter = 0;
            for (int i = 0; i < listArray.Length; i++)
            {
                Console.WriteLine((++counter).ToString() + "\t" + listArray[i]);
            }

            while (true)
            {
                Console.Write("? ");
                if (int.TryParse(Console.ReadLine(), out int selection))
                {
                    if (listArray.Length >= selection && selection > 0)
                    {
                        return listArray[selection - 1];
                    }
                }
            }
        }

        private static void PromoteToAdmin(Lid member)
        {
            if (member is null) return;
            (activeUser as IAdmin).PromoveerLidNaarMedewerker(member);
        }

        private static void CheckItemOut()
        {
            Console.WriteLine();
            DisplayCollection(activeUser.ToonOverzichtCollectie());

            while (true)
            {
                Console.Write("\nID: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    bool result = false;
                    if (CollectieBibliotheek.ItemsInCollectie.Any(x => x.ItemId == id && x.Uitgeleend == false && x.Afgevoerd == false))
                    {
                        if (userType == UserType.Member)
                        {
                            result = (activeUser as IMember).Uitlenen(CollectieBibliotheek.ItemsInCollectie.Where(x => x.ItemId == id).FirstOrDefault());
                        }

                        else if (userType == UserType.Admin)
                        {
                            result = (activeUser as IMember).Uitlenen(CollectieBibliotheek.ItemsInCollectie.Where(x => x.ItemId == id).FirstOrDefault());
                        }
                    }

                    Console.WriteLine(result);
                    break;
                }
            }
        }

        private static void ReturnItem()
        {
            var member = activeUser as IMember;
            if (member.ItemsUitgeleend.Count == 0)
            {
                return;
            }

            var item = SelectFromCollection(member.ItemsUitgeleend, "Selecteer een item:");

            member.Terugbrengen(item);
        }

        private static void ShowCheckoutHistory(IUser user)
        {
            var member = user as IMember;
            Console.WriteLine($"\nUitleenhistoriek voor {member.Voornaam} {member.Familienaam}:");
            DisplayCollection(member.Uitleenhistoriek);
        }

        private static void ShowActiveCheckouts(IUser user)
        {
            var member = user as IMember;
            Console.WriteLine($"\nUitleenhistoriek voor {member.Voornaam} {member.Familienaam}:");
            DisplayCollection(member.ItemsUitgeleend);
        }

        private static void LogOut()
        {
            activeUser = null;
            userType = UserType.None;
        }

        private static void ShowCollection()
        {
            Console.WriteLine("\nCOLLECTIE");
            Console.WriteLine("1. Toon alles");
            Console.WriteLine("2. Toon afgevoerde media");
            Console.WriteLine("3. Toon beschikbare media");
            Console.WriteLine("4. Toon uitgeleende media");

            var key = Console.ReadKey(true).Key;

            Dictionary<ConsoleKey, Func<Item, bool>> collectionKeyMap = new Dictionary<ConsoleKey, Func<Item, bool>>()
            {
                { ConsoleKey.NumPad1, null },
                { ConsoleKey.NumPad2, new Func<Item, bool> (x => x.Afgevoerd == true) },
                { ConsoleKey.NumPad3, new Func<Item, bool> (x => x.Uitgeleend == false && x.Afgevoerd == false) },
                { ConsoleKey.NumPad4, new Func<Item, bool> (x => x.Uitgeleend == true && x.Afgevoerd == false) }
            };

            DisplayCollection(activeUser.ToonOverzichtCollectie(collectionKeyMap[key]));
        }

        private static void SearchCollection()
        {
            Console.Write("Zoekterm: ");
            string searchTerm = Console.ReadLine();
            DisplayCollection(activeUser.ZoekItem(searchTerm));
            Console.ReadKey(true);
        }

        private static void DisplayCollection(IEnumerable<Item> collection)
        {
            Console.WriteLine(String.Join("\n", collection));
        }

        private static void DisplayCollection<T, Y>(IEnumerable<(T, Y)> collection)
        {
            Console.WriteLine(String.Join("\n", collection));
        }

    }

}
