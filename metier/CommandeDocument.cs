using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    public class CommandeDocument : Commande
    {
        private int nbExemplaire;
        private string etapeSuivi;

        public CommandeDocument(int nbExemplaire, string EtapeSuivi, string id, DateTime dateCommande, decimal montant) : base(id, dateCommande, montant)
        {
            this.nbExemplaire = nbExemplaire;
        }

        public int NbExemplaire { get => nbExemplaire; }
        public string EtapeSuivi { get => etapeSuivi; }
    }
}
