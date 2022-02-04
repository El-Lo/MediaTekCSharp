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

        private static readonly string server = "localhost";
        private static readonly string userid = "root";
        private static readonly string password = "";
        private static readonly string database = "mediatek86";
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
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@DocID", idLivreOuDvd);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                DateTime dateDeCommande = new DateTime();
                Decimal montant = 0;
                int nbExemplaires = 0;
                if (curs.Field("dateCommande") != null)
                    DateTime.TryParse(curs.Field("dateCommande").ToString(), out dateDeCommande);
                if (curs.Field("montant") != null)
                {
                    Decimal.TryParse(curs.Field("montant").ToString(), out montant);
                }
                if(curs.Field("nbExemplaires") != null)
                    Int32.TryParse(curs.Field("nbExemplaires").ToString(), out nbExemplaires);
                CommandeDocument Commande = new CommandeDocument(nbExemplaires, curs.Field("titreEtape").ToString(), curs.Field("id").ToString(), dateDeCommande, montant);
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
            string id = DateTime.Now.ToString("ddMMyyHHmmssffff") + DocumentID + montant.ToString();
            string req = "insert into commande(id, dateCommande, montant) ";
            req += "values(@id, @dateTimeNow, @montant)  ";
            
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@dateTimeNow", DateTime.Now);
            parameters.Add("@montant", montant);
            parameters.Add("@id", id);
            BddMySql dbUpdater = BddMySql.GetInstance(connectionString);
            dbUpdater.ReqUpdate(req, parameters);


            req = "insert into commandedocument(nbExemplaire, id, idLivreDvd, idEtapeSuivi) ";
            req += "values(@nbExemplaires, @id, @DocumentID, 1) ";
            Dictionary<string, object> parameters2 = new Dictionary<string, object>();
            parameters2.Add("@id", id);
            parameters2.Add("@nbExemplaires", nbExemplaires);
            parameters2.Add("@DocumentID", DocumentID);
            dbUpdater.ReqUpdate(req, parameters2);
            return true;
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
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@commandeID", CommandeID);
            parameters.Add("@EtapeID", EtapeID);
            BddMySql dbUpdater = BddMySql.GetInstance(connectionString);
            dbUpdater.ReqUpdate(req, parameters);
        }

        /// <summary>
        /// Get Etape d'une commande
        /// </summary>
        /// <param name="CommandeID"></param>
        /// <returns></returns>
        public static short GetEtapeDeCommande(string CommandeID)
        {
            string req = "select idEtapeSuivi from commandedocument where id = @commandeID";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@commandeID", CommandeID);
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

    }
}
