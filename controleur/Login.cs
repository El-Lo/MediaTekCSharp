using Mediatek86.modele;
using Mediatek86.modele.utilisateur;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mediatek86.controleur
{
    public class LoginControlleur
    {
        /// <summary>
        /// Verifier l'identification 
        /// </summary>
        /// <param name="NomUtilisateur"></param>
        /// <param name="HashPass"></param>
        /// <returns>Vrai si la personne peut être authentifié</returns>
        public bool VerifierLogin(string NomUtilisateur, string HashPass)
        {
            string service = Dao.VerifierLogin(NomUtilisateur, HashPass);

            if (!string.IsNullOrWhiteSpace(service))
            {
                if (Enum.TryParse(service, out Role roles))
                {
                    Utilisateur.Service = roles;
                    return true;
                }
            }
            return false;

        }
    }
}
