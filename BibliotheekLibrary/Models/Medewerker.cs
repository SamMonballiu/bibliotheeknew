using System;
using System.Collections.Generic;

namespace BibliotheekLibrary.Models
{
    public class Medewerker : Bezoeker, IAdmin, IMember
    {
        public List<(DateTime, Item)> Uitleenhistoriek { get; set; } = new List<(DateTime, Item)>();
        public List<Item> ItemsUitgeleend { get; set; } = new List<Item>();
        public DateTime Geboortedatum { get; private set; }

        public Medewerker(string familienaam, string voornaam, DateTime geboorteDatum, List<(DateTime,Item)> uitleenhistoriek = null, List<Item> itemsUitgeleend = null ) : base(familienaam, voornaam)
        {
            Uitleenhistoriek = uitleenhistoriek ?? new List<(DateTime,Item)>();
            ItemsUitgeleend = itemsUitgeleend ?? new List<Item>();
            Geboortedatum = geboorteDatum;
        }
        
        public void PromoveerLidNaarMedewerker(Lid lid)
        {
            CollectieBibliotheek.Medewerkers.Add(
                Factory.CreateMedewerker(lid.Familienaam, lid.Voornaam, lid.Geboortedatum,
                                            lid.Uitleenhistoriek, lid.ItemsUitgeleend));
            CollectieBibliotheek.Leden.Remove(lid);
        }

        public void VoerItemAf(Item item)
        {
            item.Afgevoerd = false;
        }

        public void VoegItemToe(Item item)
        {
            CollectieBibliotheek.ItemsInCollectie.Add(item);
        }

        public IEnumerable<Lid> GeefOverzichtLeden()
        {
            return CollectieBibliotheek.Leden;
        }

        public bool Uitlenen(Item item)
        {
            if (item.Gereserveerd)
            {
                if (item.Reservatienaam == Voornaam + " " + Familienaam)
                {
                    item.Gereserveerd = false;
                    item.Reservatienaam = String.Empty;
                }

                else
                {
                    return false;
                }
            }

            if (ItemsUitgeleend.Count < 5)
            {
                item.Uitgeleend = true;
                ItemsUitgeleend.Add(item);
                Uitleenhistoriek.Add((DateTime.Now, item));
                return true;
            }

            else
            {
                return false;
            }
        }

        public bool Terugbrengen(Item item)
        {
            item.Uitgeleend = false;
            if (item.Reservatienaam == Voornaam + " " + Familienaam)
            {
                item.Reservatienaam = String.Empty;
            }
            ItemsUitgeleend.Remove(item);
            return true;
        }

        public bool Reserveren(Item item)
        {
            if (item.Uitgeleend)
            {
                if (ItemsUitgeleend.Contains(item))
                {
                    return false;
                }
            }

            if (item.Gereserveerd)
            {
                return false;
            }

            item.Reservatienaam = this.Voornaam + " " + this.Familienaam;
            item.Gereserveerd = true;
            return true;
        }

        public override string ToString()
        {
            return base.ToString() + $" ({Geboortedatum.ToShortDateString()}) {this.GetType().Name.ToUpper()}";
        }

    }
}
