using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotheekLibrary.Models
{
    public static class CollectieBibliotheek
    {
        private static List<Item> _itemsCollection = new List<Item>();
        private static List<Lid> _leden = new List<Lid>();
        private static List<Medewerker> _admins = new List<Medewerker>();
        
        public static List<Item> ItemsInCollectie
        {
            get
            {
                return _itemsCollection;
            }
            private set
            {
                _itemsCollection = value;
                CollectieManager.ExportCollectionAsCSV();
            }
        }
        public static List<Item> AfgevoerdeItems
        {
            get
            {
                return ItemsInCollectie.Where(x => x.Afgevoerd = true).ToList();
            }
        }
        public static List<Lid> Leden
        {
            get
            {
                return _leden;
            }
            private set
            {
                _leden = value;
                CollectieManager.ExportMembersAsCSV();
            }
        }
        public static List<Medewerker> Medewerkers
        {
            get
            {
                return _admins;
            }
            private set
            {
                _admins = value;
                CollectieManager.ExportAdminsAsCSV();
            }
        }

        public static int GetItemsCount() => ItemsInCollectie.Count();
    }
}
