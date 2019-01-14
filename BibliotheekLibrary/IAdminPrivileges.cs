using BibliotheekLibrary.Models;
using System.Collections.Generic;

namespace BibliotheekLibrary
{
    public interface IAdmin : IUser
    {
        void PromoveerLidNaarMedewerker(Lid lid);
        void VoerItemAf(Item item);
        void VoegItemToe(Item item);
        IEnumerable<Lid> GeefOverzichtLeden();
    }
}
