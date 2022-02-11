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
           return  EnregistrerService(Dao.VerifierLogin(NomUtilisateur, HashPass)); 
        }
        /// <summary>
        /// Enregistrer le service de l'utilisateur
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        public bool EnregistrerService(string Service)
        {
            if (!string.IsNullOrWhiteSpace(Service))
            {
                if (Enum.TryParse(Service, out Role roles))
                {
                    Utilisateur.Service = roles;
                    return true;
                }
                return false;
            }
            return false;
          
        }
    }
}
