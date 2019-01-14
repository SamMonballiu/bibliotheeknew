using BibliotheekLibrary.Models;
using System;
using System.Collections.Generic;

namespace BibliotheekLibrary
{
    public interface IMember : IUser
    {
        List<(DateTime,Item)> Uitleenhistoriek { get; }
        List<Item> ItemsUitgeleend { get; }

        bool Uitlenen(Item item);
        bool Terugbrengen(Item item);
        bool Reserveren(Item item);
    }
}
