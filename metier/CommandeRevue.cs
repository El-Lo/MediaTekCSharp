using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    public class CommandeRevue : Commande
    {
    
        private DateTime dateFinAbonnement;

        public CommandeRevue(DateTime DateFindAbonnement, string id, DateTime dateCommande, decimal montant) : base(id, dateCommande, montant)
        {
            this.dateFinAbonnement = DateFindAbonnement;
        }


        public DateTime DateFinAbonnement { get => dateFinAbonnement; }
    }
}
