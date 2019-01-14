using System;
using System.Collections.Generic;
using System.Linq;

namespace BibliotheekLibrary.Models
{
    public class Bezoeker : IUser
    {
        public Bezoeker(string familienaam, string voornaam)
        {
            Familienaam = familienaam;
            Voornaam = voornaam;
        }

        public string Familienaam { get; private set; }
        public string Voornaam { get; private set; }

        public void RegistreerLid(DateTime geboorteDatum)
        {
            CollectieBibliotheek.Leden.Add(Factory.CreateLid(Familienaam, Voornaam, geboorteDatum));
        }

        public IEnumerable<Item> ZoekItem(string zoekterm)
        {
            return CollectieBibliotheek.ItemsInCollectie.Where(
                x => x.Titel.ToUpper().Contains(zoekterm.ToUpper()) || x.ItemId.ToString().Contains(zoekterm)
                || x.Auteur.ToUpper().Contains(zoekterm.ToUpper())
                );
        }

        public IEnumerable<Item> ToonOverzichtCollectie(Func<Item, bool> filter = null)
        {
            if (filter is null)
            {
                return CollectieBibliotheek.ItemsInCollectie.Where(x => x.Afgevoerd == false);
            }
            return CollectieBibliotheek.ItemsInCollectie.Where(filter);
        }

        public override string ToString()
        {
            return $"{Voornaam} {Familienaam}";
        }


    }
}
