using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.modele.utilisateur
{
    /// <summary>
    /// Informations sur le/la utilisateur(trice) actuel(le)
    /// </summary>
    public static class Utilisateur
    {
        /// <summary>
        /// Service de l'utilisateur(trice) actuel(le)
        /// </summary>
        public static Role? Service { get; set; }
    }
    /// <summary>
    /// Des Roles d'utilisateur(trice)
    /// </summary>
    public enum Role { 
        /// <summary>
        /// Admin a accès à toute l'application
        /// </summary>
        admin, 
        /// <summary>
        /// pres a accès à une partie de l'application
        /// </summary>
        pres, 
        /// <summary>
        /// culture n'apas accès à l'application
        /// </summary>
        culture
    }
}
