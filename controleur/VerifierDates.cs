using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.controleur
{
    public static class VerifierDates
    {
        /// <summary>
        /// Verifier si une date de parution est entre la date de commande et la date de fin d'abonnement
        /// </summary>
        /// <param name="DateCommande"></param>
        /// <param name="DatefinAbonnement"></param>
        /// <param name="DateDeParution"></param>
        /// <returns>Vrai si DateDeParution est entre les des autre dates</returns>
        public static bool ParutionDansAbonnement(DateTime DateCommande, DateTime DatefinAbonnement, DateTime DateDeParution)
        {

            if (DateDeParution > DateCommande && DateDeParution < DatefinAbonnement)
            {
                return true;
            }
            return false;

        }
    }
}
