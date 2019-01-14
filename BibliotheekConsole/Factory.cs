using BibliotheekLibrary.Enums;
using BibliotheekLibrary.Models;
using System;
using System.Collections.Generic;

public static class Factory
{
    public static Bezoeker CreateBezoeker(string familienaam, string voornaam) => new Bezoeker(familienaam, voornaam);

    public static Medewerker CreateMedewerker(string familienaam, string voornaam, DateTime geboortedatum, List<(DateTime,Item)> uitleenhistoriek, List<Item> itemsUitgeleend)
        => new Medewerker(familienaam, voornaam, geboortedatum, uitleenhistoriek, itemsUitgeleend);

    public static Lid CreateLid(string familienaam, string voornaam, DateTime geboortedatum)
        => new Lid(familienaam, voornaam, geboortedatum);

    public static Item CreateItem(SoortItem soort, string titel, string auteur, int jaartal)
        => new Item(soort, CollectieBibliotheek.GetItemsCount()+1, titel, auteur, jaartal);

}
