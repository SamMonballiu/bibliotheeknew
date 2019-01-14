using BibliotheekLibrary.Enums;
using System.Collections.Generic;

namespace BibliotheekLibrary.Models
{
    public class Item
    {
        public Item(SoortItem soortItem, int itemId, string titel, string auteur, int jaartal)
        {
            SoortItem = soortItem;
            ItemId = itemId;
            Titel = titel;
            Auteur = auteur;
            Jaartal = jaartal;
        }

        public SoortItem SoortItem { get; private set; }
        public int ItemId { get; private set; }
        public string Titel { get; private set; }
        public string Auteur { get; private set; }
        public int Jaartal { get; private set; }
        public bool Uitgeleend { get; set; } = false;
        public bool Afgevoerd { get; set; } = false;
        public bool Gereserveerd { get; set; } = false;
        public string Reservatienaam { get; set; }

        public override string ToString()
        {
            Dictionary<SoortItem, string> uitvoerder = new Dictionary<SoortItem, string>()
            {
                { SoortItem.Boek, "Auteur" },
                { SoortItem.Strip, "Auteur" },
                { SoortItem.DVD, "Regisseur" },
                { SoortItem.CD, "Uitvoerder" }
            };

            return $"{ItemId.ToString().PadLeft(4)}\t{SoortItem.ToString().PadRight(5)} {Titel.PadRight(40)} {Jaartal.ToString()} {(uitvoerder[this.SoortItem] + ": " + Auteur).PadRight(35)} Uitgel: {(Uitgeleend ? "j" : "n")} Afg: {(Afgevoerd ? "j" : "n")} R: {Reservatienaam}";
        }

        public Item() { }

        public Item(SoortItem soortItem, int itemId, string titel, string auteur, int jaartal, bool uitgeleend, bool afgevoerd, bool gereserveerd, string reservatienaam) : this(soortItem, itemId, titel, auteur, jaartal)
        {
            Uitgeleend = uitgeleend;
            Afgevoerd = afgevoerd;
            Gereserveerd = gereserveerd;
            Reservatienaam = reservatienaam;
        }
    }
}
