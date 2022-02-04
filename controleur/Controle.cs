﻿using System.Collections.Generic;
using Mediatek86.modele;
using Mediatek86.metier;
using Mediatek86.vue;
using System;

namespace Mediatek86.controleur
{
    internal class Controle
    {
        private readonly List<Livre> lesLivres;
        private readonly List<Dvd> lesDvd;
        private readonly List<Revue> lesRevues;
        private readonly List<Categorie> lesRayons;
        private readonly List<Categorie> lesPublics;
        private readonly List<Categorie> lesGenres;

        /// <summary>
        /// Ouverture de la fenêtre
        /// </summary>
        public Controle()
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
        /// <param name="exemplaire">Le document concerné</param>
        /// <returns>List de commandes</returns>
        public List<CommandeDocument> GetCommandesdeDeDocument(string DocID)
        {
            return Dao.GetCommandesdeDeDocument(DocID);
        }

        /// <summary>
        /// Enregistrer une commandes pour un document (DVD ou Livre)
        /// </summary>
        /// <param name="exemplaire">Le document concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool EnregistrerCommandeDocument(string DocumentID, decimal montant, int nbExemplaires)
        {
            return Dao.EnregistrerCommandeDocument(DocumentID, montant, nbExemplaires);
        }
        public string UpdateCommandeEtape(string ID, int newEtapeID)
        {
            string erreur = "";
            // 1 En cours
            // 2 Livree 
            // 3 Reglee
            // 4 Relancee
            // Une commande livrée (2) ou réglée(3) ne peut pas revenir à une étape précédente : en cours (1) ou relancée (4)
            // Une commande ne peut pas être réglée (3) si elle n'est pas livrée(2)
            int etapeActuelle = Dao.GetEtapeDeCommande(ID);
            if (etapeActuelle != 0)
            {
                if ((etapeActuelle == 2 || etapeActuelle == 4) && (newEtapeID == 1 || newEtapeID == 4))
                {
                    erreur = "Une commande livrée ou réglée ne peut pas revenir à une étape précédente";
                }
                else if (newEtapeID == 3 && etapeActuelle != 2)
                {
                    erreur = "Une commande ne peut pas être réglée si elle n'est pas livrée";
                }
                else
                {
                    Dao.UpdateCommandeEtape(ID, newEtapeID);
                }
            }
            return erreur;
        }
    }

}

