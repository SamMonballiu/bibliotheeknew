using BibliotheekLibrary.Models;
using System;
using System.Collections.Generic;

namespace BibliotheekLibrary
{
    public interface IUser
    {
        string Voornaam { get; }
        string Familienaam { get; }

        void RegistreerLid(DateTime geboorteDatum);
        IEnumerable<Item> ZoekItem(string zoekterm);
        IEnumerable<Item> ToonOverzichtCollectie(Func<Item, bool> filter = null);
    }
}
