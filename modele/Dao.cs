using Mediatek86.metier;
using System.Collections.Generic;
using Mediatek86.bdd;
using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Mediatek86.modele
{
    public static class Dao
    {

        private static readonly string server = "n1nlmysql63plsk.secureserver.net";
        private static readonly string userid = "readyplayerone";
        private static readonly string password = "gR2Fu8f8aFbR6gdFg8716sdfgPqMzNxEK";
        private static readonly string database = "ph12504474619_mediatek86";
        private static readonly string connectionString = "server=" + server + ";user id=" + userid + ";password=" + password + ";database=" + database + ";SslMode=none";

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public static List<Categorie> GetAllGenres()
        {
            List<Categorie> lesGenres = new List<Categorie>();
            string req = "Select * from genre order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Genre genre = new Genre((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesGenres.Add(genre);
            }
            curs.Close();
            return lesGenres;
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public static List<Categorie> GetAllRayons()
        {
            List<Categorie> lesRayons = new List<Categorie>();
            string req = "Select * from rayon order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Rayon rayon = new Rayon((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesRayons.Add(rayon);
            }
            curs.Close();
            return lesRayons;
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public static List<Categorie> GetAllPublics()
        {
            List<Categorie> lesPublics = new List<Categorie>();
            string req = "Select * from public order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Public lePublic = new Public((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesPublics.Add(lePublic);
            }
            curs.Close();
            return lesPublics;
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public static List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = new List<Livre>();
            string req = "Select l.id, l.ISBN, l.auteur, d.titre, d.image, l.collection, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from livre l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                string isbn = (string)curs.Field("ISBN");
                string auteur = (string)curs.Field("auteur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string collection = (string)curs.Field("collection");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon);
                lesLivres.Add(livre);
            }
            curs.Close();

            return lesLivres;
        }


        /// <summary>
        /// Retourne les étapes possibles d'une commande
        /// </summary>
        /// <returns>Collection de KeyValuePair</returns>
        public static List<KeyValuePair<string, string>> GetEtapesdeCommande()
        {

            string req = "Select es.idEtapeSuivi as id, es.titre as titre from etapesuivi es order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);
            List<KeyValuePair<string, string>> listeEtapes = new List<KeyValuePair<string, string>>();
            while (curs.Read())
            {
                KeyValuePair<string, string> etape = new KeyValuePair<string, string>(curs.Field("id").ToString(), curs.Field("titre").ToString());
                listeEtapes.Add(etape);
            }
            curs.Close();

            return listeEtapes;
        }

        /// <summary>
        /// Retourne Livre/Dvd avec commandes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public static List<CommandeDocument> GetCommandesdeDeDocument(string idLivreOuDvd)
        {
            List<CommandeDocument> lesCommandes = new List<CommandeDocument>();
            string req = "Select cmd.id, suiv.titre as titreEtape, cmd.dateCommande as dateCommande,  cmd.montant, cdoc.nbExemplaire as nbExemplaires from commandedocument cdoc ";
            req += "join commande cmd on cdoc.id=cmd.id ";
            req += "join etapesuivi suiv on cdoc.idEtapeSuivi = suiv.idEtapeSuivi ";
            req += "where cdoc.idLivreDvd = @DocID ";

            req += "order by dateCommande desc ";


            BddMySql curs = BddMySql.GetInstance(connectionString);
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@DocID", idLivreOuDvd }
            };

            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {

                DateTime dateDeCommande = new DateTime();
                Decimal montant = 0;
                int nbExemplaires = 0;
                if (curs.Field("dateCommande") != null)
                    DateTime.TryParse(curs.Field("dateCommande").ToString(), out dateDeCommande);
                if (curs.Field("montant") != null)
                {
                    Decimal.TryParse(curs.Field("montant").ToString(), out montant);
                }
                if (curs.Field("nbExemplaires") != null)
                    Int32.TryParse(curs.Field("nbExemplaires").ToString(), out nbExemplaires);
                CommandeDocument Commande = new CommandeDocument(nbExemplaires, curs.Field("titreEtape").ToString(), curs.Field("id").ToString(), dateDeCommande, montant);
                lesCommandes.Add(Commande);
            }
            curs.Close();

            return lesCommandes;
        }
        /// <summary>
        /// Retourner les commandes de revue
        /// </summary>
        /// <param name="idRevue"></param>
        /// <returns>List de commande de revue</returns>
        public static List<CommandeRevue> GetAbonnementsDeRevue(string idRevue)
        {
            List<CommandeRevue> lesCommandes = new List<CommandeRevue>();
            string req = "Select cmd.id, cmd.dateCommande as dateCommande,  cmd.montant, abnmt.dateFinAbonnement as dateFinAbonnement from abonnement abnmt ";
            req += "join commande cmd on abnmt.id=cmd.id ";

            req += "where abnmt.idRevue = @RevueID ";

            req += "order by dateCommande desc ";


            BddMySql curs = BddMySql.GetInstance(connectionString);
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@RevueID", idRevue }
            };


            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                DateTime dateDeCommande = new DateTime();
                DateTime dateFinAbonnement = new DateTime();
                Decimal montant = 0;
                if (curs.Field("dateCommande") != null)
                    DateTime.TryParse(curs.Field("dateCommande").ToString(), out dateDeCommande);
                if (curs.Field("montant") != null)
                {
                    Decimal.TryParse(curs.Field("montant").ToString(), out montant);
                }
                if (curs.Field("datefinabonnement") != null)
                    DateTime.TryParse(curs.Field("dateFinAbonnement").ToString(), out dateFinAbonnement);
                CommandeRevue Commande = new CommandeRevue(dateFinAbonnement, curs.Field("id").ToString(), dateDeCommande, montant);
                lesCommandes.Add(Commande);
            }
            curs.Close();

            return lesCommandes;
        }
        /// <summary>
        /// Enregistrer une nouvelle commande de document (DVD ou Livre)
        /// </summary>
        /// <returns>Vrai si l'enregistrement s'est fait correctement</returns>
        public static bool EnregistrerCommandeDocument(string DocumentID, decimal montant, int nbExemplaires)
        {
            // Creer l'ID du document
            string id = creerCommandeID(DocumentID, montant);
            // enregistrer la commande dans table commande
            EnregistrerCommande(id, montant);
            // enregistrer les données particulieres a une commande de livre ou dvd
            string req = "insert into commandedocument(nbExemplaire, id, idLivreDvd, idEtapeSuivi) ";
            req += "values(@nbExemplaires, @id, @DocumentID, 1) ";
            Dictionary<string, object> parameters2 = new Dictionary<string, object> {
                {"@id", id},
                {"@nbExemplaires", nbExemplaires},
                {"@DocumentID", DocumentID}
            };
            BddMySql dbUpdater = BddMySql.GetInstance(connectionString);
            dbUpdater.ReqUpdate(req, parameters2);
            return true;
        }
        /// <summary>
        /// Enregistrer une nouvelle abonnement ou rénouveller une abonnement
        /// </summary>
        /// <returns>Vrai si l'enregistrement s'est fait correctement</returns>
        public static bool EnregistrerAbonnement(string DocumentID, decimal montant, DateTime dateFinAbonnement)
        {
            // Creer l'ID du document
            string id = creerCommandeID(DocumentID, montant);
            // enregistrer la commande dans table commande
            EnregistrerCommande(id, montant);
            // enregistrer les données particulieres a une commande de livre ou dvd
            string req = "insert into abonnement(dateFinAbonnement, id, idRevue) ";
            req += "values(@dateFinAbonnement, @id, @DocumentID) ";
            Dictionary<string, object> parameters2 = new Dictionary<string, object>
            {
                {"@id", id },
                {"@dateFinAbonnement", dateFinAbonnement},
                {"@DocumentID", DocumentID}
            };

            BddMySql dbUpdater = BddMySql.GetInstance(connectionString);
            dbUpdater.ReqUpdate(req, parameters2);
            return true;
        }
        /// <summary>
        /// Creer un ID pour un commande, que ce soit pour une abonnement ou une commande de dvd ou livre
        /// </summary>
        /// <param name="DocumentID"></param>
        /// <param name="montant"></param>
        /// <returns>ID pour la commande, construit en utilisant la date, ID Document et le montant</returns>
        private static string creerCommandeID(string DocumentID, decimal montant)
        {
            return DateTime.Now.ToString("ddMMyyHHmmssffff") + DocumentID + montant.ToString();
        }
        /// <summary>
        /// Premiere enregistrement lorsqu'on crée une commande de document ou une abonnement : Creerla commande dans la table commande. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="montant"></param>
        private static void EnregistrerCommande(string id, decimal montant)
        {

            string req = "insert into commande(id, dateCommande, montant) ";
            req += "values(@id, @dateTimeNow, @montant)  ";

            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                {"@dateTimeNow", DateTime.Now },
                {"@montant", montant },
                {"@id", id }
            };
            BddMySql dbUpdater = BddMySql.GetInstance(connectionString);
            dbUpdater.ReqUpdate(req, parameters);
        }
        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public static List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = new List<Dvd>();
            string req = "Select l.id, l.duree, l.realisateur, d.titre, d.image, l.synopsis, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from dvd l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                int duree = (int)curs.Field("duree");
                string realisateur = (string)curs.Field("realisateur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string synopsis = (string)curs.Field("synopsis");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon);
                lesDvd.Add(dvd);
            }
            curs.Close();

            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public static List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = new List<Revue>();
            string req = "Select l.id, l.empruntable, l.periodicite, d.titre, d.image, l.delaiMiseADispo, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from revue l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                bool empruntable = (bool)curs.Field("empruntable");
                string periodicite = (string)curs.Field("periodicite");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                int delaiMiseADispo = (int)curs.Field("delaimiseadispo");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Revue revue = new Revue(id, titre, image, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon, empruntable, periodicite, delaiMiseADispo);
                lesRevues.Add(revue);
            }
            curs.Close();

            return lesRevues;
        }

        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <returns>Liste d'objets Exemplaire</returns>
        public static List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            List<Exemplaire> lesExemplaires = new List<Exemplaire>();
            string req = "Select e.id, e.numero, e.dateAchat, e.photo, e.idEtat ";
            req += "from exemplaire e join document d on e.id=d.id ";
            req += "where e.id = @id ";
            req += "order by e.dateAchat DESC";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", idDocument}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                string idDocuement = (string)curs.Field("id");
                int numero = (int)curs.Field("numero");
                DateTime dateAchat = (DateTime)curs.Field("dateAchat");
                string photo = (string)curs.Field("photo");
                string idEtat = (string)curs.Field("idEtat");
                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocuement);
                lesExemplaires.Add(exemplaire);
            }
            curs.Close();

            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static bool CreerExemplaire(Exemplaire exemplaire)
        {
            try
            {
                string req = "insert into exemplaire values (@idDocument,@numero,@dateAchat,@photo,@idEtat)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idDocument", exemplaire.IdDocument},
                    { "@numero", exemplaire.Numero},
                    { "@dateAchat", exemplaire.DateAchat},
                    { "@photo", exemplaire.Photo},
                    { "@idEtat",exemplaire.IdEtat}
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Changer l'etape d'une commande
        /// </summary>
        /// <param name="CommandeID"></param>
        /// <param name="EtapeID"></param>
        public static void UpdateCommandeEtape(string CommandeID, int EtapeID)
        {
            string req = "update commandedocument set idEtapeSuivi = @EtapeID where id = @commandeID";
            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                {"@commandeID", CommandeID},
                {"@EtapeID", EtapeID}
            };
            BddMySql dbUpdater = BddMySql.GetInstance(connectionString);
            dbUpdater.ReqUpdate(req, parameters);
        }

        /// <summary>
        /// Get Etape d'une commande
        /// </summary>
        /// <param name="CommandeID"></param>
        /// <returns>ID de l'étape liée a la commande</returns>
        public static short GetEtapeDeCommande(string CommandeID)
        {
            string req = "select idEtapeSuivi from commandedocument where id = @commandeID";
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"@commandeID", CommandeID}
            };
            BddMySql curs = BddMySql.GetInstance(connectionString);

            curs.ReqSelect(req, parameters);
            short idEtape = 0;
            while (curs.Read())
            {
                string id = curs.Field("idEtapeSuivi").ToString();
                Int16.TryParse(id, out idEtape);
            }
            curs.Close();
            return idEtape;
        }

        /// <summary>
        /// Supprimer une commande de Dvd ou Livre si elle n'est pas déjà livrée
        /// </summary>
        /// <param name="DocID"></param>
        public static void SupprimerCommandeDvdLivre(string DocID)
        {

            string req = "delete from commandedocument cmd where cmd.id = @id and idEtapeSuivi = 1; ";

            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                {"@id", DocID}
            };
            BddMySql dbUpdater = BddMySql.GetInstance(connectionString);
            dbUpdater.ReqUpdate(req, parameters);

            req = "delete from commande c where c.id = @id and exists(select 1 from commandedocument cmd where cmd.id = c.id and idEtapeSuivi = 1) ; ";
            dbUpdater.ReqUpdate(req, parameters);
        }

        /// <summary>
        /// Retourner les dates de parution des exemplaires d'une revue
        /// </summary>
        /// <returns>Liste d'objets Exemplaire</returns>
        public static List<DateTime> GetDateDesExemplairesdeRevue(string idDocument)
        {
            List<DateTime> lesDatesDesExemplaires = new List<DateTime>();
            string req = "Select e.dateAchat from exemplaire e where e.id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", idDocument}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                if (DateTime.TryParse(curs.Field("dateAchat").ToString(), out DateTime dt))
                {
                    lesDatesDesExemplaires.Add(dt);
                }
            }
            curs.Close();

            return lesDatesDesExemplaires;
        }

        /// <summary>
        /// Supprimer un abonnement si il n'y a aucun exemplaire rataché
        /// </summary>
        /// <param name="idAbonnement"></param>
        public static void SupprimerAbonnement(string idAbonnement)
        {
            // supprimer l'element child en verifiant qu'il n'y a aucun exemplaire rataché à l'abonnement
            string req = "delete ab, cmd from abonnement ab join commande cmd on cmd.id = ab.id where ab.id = @idAbonnement and not exists(select 1 from exemplaire ex where ex.dateAchat > cmd.dateCommande and ex.dateAchat < ab.dateFinAbonnement and ex.id = ab.idRevue)  ";

            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"@idAbonnement", idAbonnement}
            };
            BddMySql dbUpdater = BddMySql.GetInstance(connectionString);
            dbUpdater.ReqUpdate(req, parameters);


        }
        /// <summary>
        /// Recuperer les abonnements qui termineront sous 30 jours
        /// </summary>
        /// <returns>List de documetns avec la date de fin d'abonnement</returns>
        public static List<string> RecupererRevuesAbonnementTerminant()
        {

            List<string> dDates = new List<string>();
            string req = "Select doc.titre as titre, ab.dateFinAbonnement as dateFin from abonnement ab join document doc on ab.idrevue = doc.id where ab.dateFinAbonnement between curdate()  AND DATE_ADD(curdate(), INTERVAL 30 DAY);";


            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                dDates.Add((string)curs.Field("titre") + " : " + ((DateTime)curs.Field("dateFin")).ToString("dd/MM/yyyy"));
            }
            curs.Close();

            return dDates;
        }
        /// <summary>
        /// Retourner le service dans lequel une personne travaille si le mot de passe et nom d'utilisateur sont correctes. 
        /// </summary>
        /// <param name="NomUtilisateur"></param>
        /// <param name="MotDePasse"></param>
        /// <returns>Service de l'utilisateur</returns>
        public static string VerifierLogin(string NomUtilisateur, string MotDePasse)
        {

            string Service = null;
            string req = "Select s.titre as Service from utilisateurs u join services s on s.id=u.idService where u.nomUtilisateur = @nomUtilisateur and u.motDePasse = @motDePasse;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@nomUtilisateur", NomUtilisateur},
                {"@motDePasse",  MotDePasse}
            };
            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                Service = curs.Field("Service").ToString();
            }
            curs.Close();

            return Service;
        }
    }
}
