using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    public abstract class Commande
    {
        private readonly string id;
        private readonly DateTime dateCommande;
        private decimal montant;

        protected Commande(string id, DateTime dateCommande, decimal montant)
        {
            this.id = id;
            this.dateCommande = dateCommande;
            this.montant = montant;
        }

        public string Id { get => id; }
        public DateTime DateCommande { get => dateCommande; }
        public decimal Montant { get => montant; }
    }
}
