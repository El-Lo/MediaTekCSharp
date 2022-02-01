using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    class Abonnement : Commande
    {

        private DateTime dateFinAbonnement;

        public Abonnement(DateTime dateFinAbonnement, string id, DateTime dateCommande, decimal montant) :base(id, dateCommande, montant)
        {
            this.dateFinAbonnement = dateFinAbonnement;
        }

        public DateTime DateFinAbonnement { get => dateFinAbonnement; }
    }
}
