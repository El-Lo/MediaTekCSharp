using System.Collections.Generic;
using Mediatek86.modele;
using Mediatek86.metier;
 
using System;

using Mediatek86.modele.utilisateur;
using Mediatek86.vue;
using System.Linq;

namespace Mediatek86.controleur
{
    internal class Controle
    {
        private List<Livre> lesLivres;
        private List<Dvd> lesDvd;
        private List<Revue> lesRevues;
        private List<Categorie> lesRayons;
        private List<Categorie> lesPublics;
        private List<Categorie> lesGenres;

        /// <summary>
        /// Ouverture de la fenêtre authentification
        /// </summary>
        public Controle()
        {
            FrmLogin frmLogin = new FrmLogin(this, new LoginControlleur());
            frmLogin.ShowDialog();
        }
        public void showFrmMediaTek(Role? roleType)
        {
            lesLivres = Dao.GetAllLivres();
            lesDvd = Dao.GetAllDvd();
            lesRevues = Dao.GetAllRevues();
            lesGenres = Dao.GetAllGenres();
            lesRayons = Dao.GetAllRayons();
            lesPublics = Dao.GetAllPublics();
            FrmMediatek frmMediatek = new FrmMediatek(this);
            frmMediatek.ShowDialog();
        }
        /// <summary>
        /// Retourner une liste de revues avec des abonnement qui se terminera sous 30 jours 
        /// </summary>
        /// <returns>List de chaines Revue titre plus la date de fin d'abonnement</returns>
        public List<string> RecupererRevuesAbonnementTerminant()
        {
            return Dao.RecupererRevuesAbonnementTerminant();
        }
        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Collection d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return lesGenres;
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return lesLivres;
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Collection d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return lesDvd;
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Collection d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return lesRevues;
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return lesRayons;
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return lesPublics;
        }

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <returns>Collection d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return Dao.GetExemplairesRevue(idDocuement);
        }
        /// <summary>
        /// récupère les étapes possibles d'une commande
        /// </summary>
        /// <returns>Collection de KeyValuePair</returns>
        public List<KeyValuePair<string, string>> GetAllEtapes()
        {
            return Dao.GetEtapesdeCommande();
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return Dao.CreerExemplaire(exemplaire);
        }
         
        /// <summary>
        /// Retourne une list de commandes pour un document (DVD ou Livre)
        /// </summary>
        /// <param name="DocID">Le document concerné</param>
        /// <returns>List de commandes</returns>
        public List<CommandeDocument> GetCommandesdeDeDocument(string DocID)
        {
            return Dao.GetCommandesdeDeDocument(DocID);
        }
        /// <summary>
        /// Retourne une list d'abonnements pour une revue
        /// </summary>
        /// <param name="RevueID"></param>
        /// <returns>List de revues</returns>
        public List<CommandeRevue> GetAbonnementsDeRevue(string RevueID)
        {
            return Dao.GetAbonnementsDeRevue(RevueID);
        }



        /// <summary>
        ///  Enregistrer une commande pour un document (DVD ou Livre)
        /// </summary>
        /// <param name="DocumentID"></param>
        /// <param name="montant"></param>
        /// <param name="nbExemplaires"></param>
        /// <returns>True si la création a pu se faire</returns>
        public bool EnregistrerCommandeDocument(string DocumentID, decimal montant, int nbExemplaires)
        {
            return Dao.EnregistrerCommandeDocument(DocumentID, montant, nbExemplaires);
        }

        /// <summary>
        /// Enregistrer une abonnement pour une revue
        /// </summary>
        /// <param name="DocumentID"></param>
        /// <param name="montant"></param>
        /// <param name="DateFinAbonnement"></param>
        /// <returns>True si la création a pu se faire</returns>
        public bool EnregistrerRevueAbonnement(string DocumentID, decimal montant, DateTime DateFinAbonnement)
        {
            return Dao.EnregistrerAbonnement(DocumentID, montant, DateFinAbonnement);
        }
        /// <summary>
        /// Mis a jour d'une étape
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="newEtapeID"></param>
        /// <returns>Indicateur de succès</returns>
        public string UpdateCommandeEtape(string ID, int newEtapeID)
        {
            string message = "";
            // 1 En cours
            // 2 Livrée 
            // 3 Reglée
            // 4 Relancée
            // Une commande livrée (2) ou réglée(3) ne peut pas revenir à une étape précédente : 
            // en cours (1) ou relancée (4)
            // Une commande ne peut pas être réglée (3) si elle n'est pas livrée(2)

            // Récuperer l'étape actuelle de la commande pour vérifier la validité de la demande
            int etapeActuelle = Dao.GetEtapeDeCommande(ID);
            if (etapeActuelle != 0)
            {
                if ((etapeActuelle == 2 || etapeActuelle == 3) && (newEtapeID == 1 || newEtapeID == 4))
                {
                    // Une commande livrée ou réglée ne peut pas revenir à une étape précédente
                    message = "Une commande livrée ou réglée ne peut pas revenir à une étape précédente";
                }
                else if (newEtapeID == 2 && etapeActuelle == 3)
                {
                    // Une commande réglée ne peut pas revenir à une étape précédente
                    message = "Une commande réglée ne peut pas revenir à une étape précédente";
                }
                else if (newEtapeID == 3 && etapeActuelle != 2)
                {
                    // Une commande ne peut pas être réglée si elle n'est pas livrée
                    message = "Une commande ne peut pas être réglée si elle n'est pas livrée";
                }
                else
                {
                    // Mise a jour de la commande
                    Dao.UpdateCommandeEtape(ID, newEtapeID);
                }
            }
            // Récuperer la nouvelle étape dans la BDD pour vérifier que la changement à bien été effectué
            int NewEtapeDansBDD = Dao.GetEtapeDeCommande(ID);
            // Vérifier que l'étape venue de la BDD est égale a l'étape souhaité
            if (NewEtapeDansBDD == newEtapeID)
            {
                // "1" egale succès
                message = "1";
            }
            return message;
        }
        /// <summary>
        /// Référence a DAO
        /// </summary>
        /// <param name="DocID"></param>
        public void SupprimerCommandeDvdLivre(string DocID)
        {
            Dao.SupprimerCommandeDvdLivre(DocID);
        }
        /// <summary>
        /// Lien à DAO pour supprimer un abonnement
        /// </summary>
        /// <param name="IDAbonnement"></param>
        public void SupprimerAbonnement(string IDAbonnement)
        {
            Dao.SupprimerAbonnement(IDAbonnement);
        }
         
        /// <summary>
        /// Verifier si une commande peut être supprimée. 
        /// </summary>
        /// <param name="DateCommande"></param>
        /// <param name="DatefinAbonnement"></param>
        /// <param name="RevueID"></param>
        /// <returns>True si abonnement peut être supprimé, sinon false</returns>
        public static bool EstAbonnementSupprimable(DateTime DateCommande, DateTime DatefinAbonnement, string RevueID)
        {
            List<DateTime> DatesDeParution = Dao.GetDateDesExemplairesdeRevue(RevueID);
            DateTime a = DatesDeParution.FirstOrDefault(x => VerifierDates.ParutionDansAbonnement(DateCommande, DatefinAbonnement, x));
            if (a != DateTime.MinValue)
            { 
                // une parution exist, donc la commande ne peut pas être supprimée, donc retourner faux
                return false;
            }
            return true;
        }
        


    }
}
