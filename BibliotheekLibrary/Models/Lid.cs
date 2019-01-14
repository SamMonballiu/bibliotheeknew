using System;
using System.Collections.Generic;

namespace BibliotheekLibrary.Models
{
    public class Lid : Bezoeker, IMember
    {
        public Lid(string familienaam, string voornaam, DateTime geboorteDatum) : base(familienaam, voornaam)
        {
            Geboortedatum = geboorteDatum;
        }

        public DateTime Geboortedatum { get; private set; }
        public List<(DateTime, Item)> Uitleenhistoriek { get; internal set; } = new List<(DateTime, Item)>();
        public List<Item> ItemsUitgeleend { get; internal set; } = new List<Item>();

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
            ItemsUitgeleend.Remove(item);
            if (item.Reservatienaam == Voornaam + " " + Familienaam)
            {
                item.Reservatienaam = String.Empty;
            }
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
