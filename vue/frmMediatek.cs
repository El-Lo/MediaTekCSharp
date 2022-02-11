using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mediatek86.metier;
using Mediatek86.controleur;
using System.Drawing;
using System.Linq;
using System.Data;

using System.Security.Cryptography;
using System.Text;
using Mediatek86.modele.utilisateur;

namespace Mediatek86.vue
{
    public partial class FrmMediatek : Form
    {

        #region Variables globales

        private readonly Controle controle;
        const string ETATNEUF = "00001";


        private readonly BindingSource bdgLivresListe = new BindingSource();
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private readonly BindingSource bdgDocumentAvecCommandes = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private List<Dvd> lesDvd = new List<Dvd>();
        private List<Revue> lesRevues = new List<Revue>();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();

        #endregion


        internal FrmMediatek(Controle controle)
        {
            InitializeComponent();
            this.controle = controle;
            if (Utilisateur.Service == Role.admin)
            {
                VerifierAbonnements();
                pnlNotifAbonRevues.Visible = true;
            }
            else if (Utilisateur.Service == Role.pres)
            {
                tabOngletsApplication.TabPages.Remove(this.tabCommandeDvds);
                tabOngletsApplication.TabPages.Remove(this.tabCommandeLivres);
                tabOngletsApplication.TabPages.Remove(this.tabCommandeRevues);
            }
        }
        /// <summary>
        /// Montrer les abonnements qui vont se terminer sous 30 jours
        /// </summary>
        private void VerifierAbonnements()
        {
            List<string> dit = controle.RecupererRevuesAbonnementTerminant();
            if (dit != null)
            {
                pnlNotifAbonRevues.Visible = true;
                if (dit.Count > 0)
                {
                    lblNotifsAucun.Visible = false;
                    lstbxNotifs.Visible = true;

                    foreach (string str in dit)
                    {
                        lstbxNotifs.Items.Add(str);
                    }
                    lstbxNotifs.SelectionMode = SelectionMode.None;

                }
                else
                {
                    lblNotifsAucun.Visible = true;
                    lstbxNotifs.Visible = false;
                }
            }
            else
            {
                lblNotifsAucun.Visible = true;
                lstbxNotifs.Visible = false;
            }
        }
        #region modules communs

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        #endregion



        #region CommandeDvds
        //-----------------------------------------------------------
        // ONGLET "CommandeDvds"
        //------------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Commande Dvds : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandeDvds_Enter(object sender, EventArgs e)
        {
            if (lesDvd == null || lesDvd.Count == 0)
            {
                lesDvd = controle.GetAllDvd();
            }

            cbxFindDvdByNum.SelectedIndex = -1;
            cbxFindDvdByNum.Items.Clear();
            foreach (Dvd dvd in lesDvd)
            {
                ComboboxItem item = new ComboboxItem(dvd.Id, dvd.Id);
                cbxFindDvdByNum.Items.Add(item);
            }



        }

        /// <summary>
        /// Fermer panel pnlCommandeDvdDetail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseCommandeDvdDetail_Click(object sender, EventArgs e)
        {
            pnlCommandeDvdDetail.Visible = false;
        }
        /// <summary>
        /// Enregistrer une nouvelle etape d'une commande de Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateCommandeDvdEtape_Click(object sender, EventArgs e)
        {
            lblCommandeDvdDetailErreur.Text = "";
            // changer l'etape pour la commande

            ComboboxItem item = (ComboboxItem)cbxCommandeDvdDetailEtape.SelectedItem;
            if (item != null)
            {
                string EtapeID = item.Value;
                if (Int32.TryParse(EtapeID, out int EtapeIDInt))
                {
                    string erreur = controle.UpdateCommandeEtape(lblCommandeDVDDetailID.Text, EtapeIDInt);
                    if (string.IsNullOrWhiteSpace(erreur))
                    {
                        ChercheDvdetCommandes();
                    }
                    else
                    {
                        lblCommandeDvdDetailErreur.Text = erreur;
                    }
                }
            }
        }
        /// <summary>
        /// Montrer commande lorsqu'elle est selectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesdeDvd_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridView dgv = sender as DataGridView;
                if (dgv == null)
                    return;
                // verifier que le row est selectionné
                if (dgv.CurrentRow.Selected)
                {
                    // chercher des valeurs du row selectionné
                    string id = Convert.ToString(dgv.CurrentRow.Cells["id"].Value);
                    string nbExemplaires = Convert.ToString(dgv.CurrentRow.Cells["NbExemplaire"].Value);
                    string montant = Convert.ToString(dgv.CurrentRow.Cells["montant"].Value);
                    string selectedEtape = Convert.ToString(dgv.CurrentRow.Cells["etapesuivi"].Value);
                    string datecommande = Convert.ToString(dgv.CurrentRow.Cells["datecommande"].Value);
                    // remplir les textboxes
                    pnlCommandeDvdDetail.Visible = true;
                    lblCommandeDVDDetailID.Text = id;
                    lblCommandeDVDDetailDateDeCommande.Text = datecommande;
                    txbCommandeDvdDetailMontant.Text = montant;
                    txbCommandeDvdDetailNExemplaires.Text = nbExemplaires;

                    // trouver une liste d'étapes
                    List<KeyValuePair<string, string>> Etapes = controle.GetAllEtapes();

                    // réinitialiser cbxCommandeDvdDetailEtape
                    cbxCommandeDvdDetailEtape.Items.Clear();

                    //ajouter les étapes
                    foreach (KeyValuePair<string, string> uneEtape in Etapes)
                    {
                        if (Int32.TryParse(uneEtape.Key, out int key))
                        {
                            ComboboxItem item = new ComboboxItem(uneEtape.Value, uneEtape.Key);

                            cbxCommandeDvdDetailEtape.Items.Add(item);
                        }
                    }
                    // selectionner la bonne étape 
                    foreach (ComboboxItem item in cbxCommandeDvdDetailEtape.Items)
                    {
                        if (item.Text == selectedEtape)
                            cbxCommandeDvdDetailEtape.SelectedIndex = cbxCommandeDvdDetailEtape.Items.IndexOf(item);
                    }
                    // Commande ne peut plus être supprimé si elle est livrée
                    // 1 En cours
                    // 2 Livrée 
                    // 3 Reglée
                    // 4 Relancée

                    // trouver l'étape
                    ComboboxItem cmbx = (ComboboxItem)cbxCommandeDvdDetailEtape.SelectedItem;
                    Int32.TryParse(cmbx.Value, out int etapeID);

                    // si la commande est livré cacher le bouton pour supprimer et montrer une message sinon faire l'inverse
                    if (etapeID > 1)
                    {
                        btnSupprimeCommandeDvd.Visible = false;
                        lblCommandeDvdNonSupprimable.Visible = true;
                    }
                    else
                    {
                        btnSupprimeCommandeDvd.Visible = true;
                        lblCommandeDvdNonSupprimable.Visible = false;
                    }
                }
            }
        }
        /// <summary>
        /// Supprimer une commande de Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimeCommandeDvd_Click(object sender, EventArgs e)
        {
            string message = "Voulez-vous vraiment supprimer cette commande ?";
            string title = "Suppression de commande";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            if (result.ToString().ToLower() == "yes")
            {
                if (!string.IsNullOrWhiteSpace(lblCommandeDVDDetailID.Text))
                {
                    controle.SupprimerCommandeDvdLivre(lblCommandeDVDDetailID.Text);
                }
                // réinitialiser pnlCommandeDvdDetail
                pnlCommandeDvdDetail.Visible = false;
                lblCommandeDVDDetailID.Text = "";
                lblCommandeDVDDetailDateDeCommande.Text = "";
                txbCommandeDvdDetailMontant.Text = "";
                txbCommandeDvdDetailNExemplaires.Text = "";
                cbxCommandeDvdDetailEtape.Items.Clear();
                // réinitialiser la liste de commandes
                ChercheDvdetCommandes();

            }

        }
        /// <summary>
        /// Enregistrer une commande de Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnregistrerCommandeDvd_Click(object sender, EventArgs e)
        {
            lblErrorAjouteDocumentDvd.Text = "";
            string idDvd = txbCommandeDvdNumero.Text;
            // verifier saisie montant
            if (!Decimal.TryParse(txbNewCommandeDvdMontant.Text, out decimal montant))
            {
                lblErrorAjouteDocumentDvd.Text = "Montant incorrect, veuillez réessayer, merci.";
                return;
            }
            // verifier saisie nbExemplaires
            if (!Int32.TryParse(txbNewCommandeDvdNbExemplaires.Text, out int nbExemplaires))
            {
                lblErrorAjouteDocumentDvd.Text = "Nombre d'exemplaires incorrect, veuillez réessayer, merci.";
                return;
            }
            // verifier enregistrement
            if (!controle.EnregistrerCommandeDocument(idDvd, montant, nbExemplaires))
            {
                lblErrorAjouteDocumentDvd.Text = "Une erreur est survenue, merci de réesayer. (Err : 15d)";

            }
            //si l'enregistrement s'est bien passée, réinitialise les champs
            else
            {
                txbNewCommandeDvdMontant.Text = "";
                txbNewCommandeDvdNbExemplaires.Text = "";
                ChercheDvdetCommandes();
            }


        }
        /// <summary>
        /// btnClick Chercher Dvd avec Commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDVDsEtCommandesSearch_Click(object sender, EventArgs e)
        {

            ChercheDvdetCommandes();
        }
        /// <summary>
        /// Remplir les champs avec les données d'un Dvd 
        /// </summary>
        private void ChercheDvdetCommandes()
        {
            ComboboxItem item = (ComboboxItem)cbxFindDvdByNum.SelectedItem;
            if (item != null)
            {
                string Num = item.Value;
                if (!String.IsNullOrWhiteSpace(Num))
                {
                    Dvd Dvd = lesDvd.Find(x => x.Id.Equals(Num));
                    if (Dvd != null)
                    {

                        txbCommandeDvdTitre.Text = Dvd.Titre;
                        txbCommandeDvdRealisat.Text = Dvd.Realisateur;
                        txbCommandeDvdCheminImage.Text = Dvd.Image;
                        txbCommandeDvdSynopsis.Text = Dvd.Synopsis;
                        txbCommandeDvdGenre.Text = Dvd.Genre;
                        txbCommandeDvdNumero.Text = Dvd.Id;
                        txbCommandeDvdRayon.Text = Dvd.Rayon;
                        txbCommandeDvdPublic.Text = Dvd.Public;
                        txbCommandeDvdDuree.Text = Convert.ToString(Dvd.Duree);
                        UpdateCommandesListPourDvd(Dvd.Id);




                    }
                    else
                    {
                        MessageBox.Show("numéro introuvable");

                    }
                }
            }
        }
        /// <summary>
        /// Recuperation des commandes pour un Dvd
        /// </summary>
        /// <param name="DvdID"></param>
        private void UpdateCommandesListPourDvd(string DvdID)
        {
            List<CommandeDocument> commandes = controle.GetCommandesdeDeDocument(DvdID);
            // enregistrer pour le tri des colonnes 
            lesDvd.Find(x => x.Id.Equals(DvdID)).Commandes = commandes;
            bdgDocumentAvecCommandes.DataSource = commandes;
            dgvCommandesdeDvd.DataSource = bdgDocumentAvecCommandes;
            dgvCommandesdeDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandesdeDvd.Columns["id"].Visible = false;
        }


        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesdeDvd_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesdeDvd.Columns[e.ColumnIndex].HeaderText.ToLower();
            ComboboxItem item = (ComboboxItem)cbxFindDvdByNum.SelectedItem;
            if (item != null)
            {
                List<CommandeDocument> sortedList;
                switch (titreColonne)
                {
                    case "datecommande":
                        sortedList = lesDvd.Find(x => x.Id.Equals(item.Value)).Commandes.OrderBy(o => o.DateCommande).ToList();
                        break;

                    case "nbexemplaire":
                        sortedList = lesDvd.Find(x => x.Id.Equals(item.Value)).Commandes.OrderBy(o => o.NbExemplaire).ToList();
                        break;
                    case "montant":
                        sortedList = lesDvd.Find(x => x.Id.Equals(item.Value)).Commandes.OrderBy(o => o.Montant).ToList();
                        break;
                    case "etapesuivi":
                        sortedList = lesDvd.Find(x => x.Id.Equals(item.Value)).Commandes.OrderBy(o => o.EtapeSuivi).ToList();
                        break;
                    default:
                        sortedList = lesDvd.Find(x => x.Id.Equals(item.Value)).Commandes.OrderBy(o => o.Id).Reverse().ToList();
                        break;
                }
                bdgDocumentAvecCommandes.DataSource = sortedList;
                dgvCommandesdeDvd.DataSource = bdgDocumentAvecCommandes;
                dgvCommandesdeDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvCommandesdeDvd.Columns["id"].Visible = false;
            }
        }

        #endregion

        #region CommandeLivres
        //-----------------------------------------------------------
        // ONGLET "CommandeLivres"
        //------------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Commande Livres : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandeLivres_Enter(object sender, EventArgs e)
        {
            if (lesLivres == null || lesLivres.Count == 0)
            {
                lesLivres = controle.GetAllLivres();
            }

            cbxFindLivreByNum.SelectedIndex = -1;

            foreach (Livre unlivre in lesLivres)
            {
                ComboboxItem item = new ComboboxItem(unlivre.Id, unlivre.Id);

                cbxFindLivreByNum.Items.Add(item);
            }
        }
        /// <summary>
        /// Enregistrer une nouvelle etape d'une commande de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateCommandeLivreEtape_Click(object sender, EventArgs e)
        {
            lblCommandeLivreDetailErreur.Text = "";
            // changer l'etape pour la commande

            ComboboxItem item = (ComboboxItem)cbxCommandeLivreDetailEtape.SelectedItem;
            if (item != null)
            {
                string EtapeID = item.Value;

                if (Int32.TryParse(EtapeID, out int EtapeIDInt))
                {
                    string erreur = controle.UpdateCommandeEtape(lblCommandeLivreDetailID.Text, EtapeIDInt);
                    if (string.IsNullOrWhiteSpace(erreur))
                    {
                        ChercheLivreetCommandes();
                    }
                    else
                    {
                        lblCommandeLivreDetailErreur.Text = erreur;
                    }
                }

            }

        }
        /// <summary>
        /// Fermer panel pnlCommandeLivreDetail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClosCommandeLivreDetails_Click(object sender, EventArgs e)
        {
            pnlCommandeLivreDetail.Visible = false;
        }
        /// <summary>
        /// Montrer commande lorsqu'elle est selectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesdeLivre_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex != -1)
            {
                DataGridView dgv = sender as DataGridView;
                if (dgv == null)
                    return;
                // verifier que le row est selectionné
                if (dgv.CurrentRow.Selected)
                {
                    // chercher des valeurs du row selectionné
                    string id = Convert.ToString(dgv.CurrentRow.Cells["id"].Value);
                    string nbExemplaires = Convert.ToString(dgv.CurrentRow.Cells["NbExemplaire"].Value);
                    string montant = Convert.ToString(dgv.CurrentRow.Cells["montant"].Value);
                    string selectedEtape = Convert.ToString(dgv.CurrentRow.Cells["etapesuivi"].Value);
                    string datecommande = Convert.ToString(dgv.CurrentRow.Cells["datecommande"].Value);
                    // remplir les textboxes
                    pnlCommandeLivreDetail.Visible = true;
                    lblCommandeLivreDetailID.Text = id;
                    lblCommandeLivreDetailDateDeCommande.Text = datecommande;
                    txbCommandeLivreDetailMontant.Text = montant;
                    txbCommandeLivreDetailNExemplaires.Text = nbExemplaires;

                    // trouver une liste d'étapes
                    List<KeyValuePair<string, string>> Etapes = controle.GetAllEtapes();

                    // réinitialiser cbxCommandeLivreDetailEtape
                    cbxCommandeLivreDetailEtape.Items.Clear();

                    //ajouter les étapes
                    foreach (KeyValuePair<string, string> uneEtape in Etapes)
                    {
                        if (Int32.TryParse(uneEtape.Key, out int key))
                        {
                            ComboboxItem item = new ComboboxItem(uneEtape.Value, uneEtape.Key);
                            cbxCommandeLivreDetailEtape.Items.Add(item);
                        }
                    }
                    // selectionner la bonne étape 
                    foreach (ComboboxItem item in cbxCommandeLivreDetailEtape.Items)
                    {
                        if (item.Text == selectedEtape)
                            cbxCommandeLivreDetailEtape.SelectedIndex = cbxCommandeLivreDetailEtape.Items.IndexOf(item);
                    }

                    // Commande ne peut plus être supprimé si elle est livrée
                    // 1 En cours
                    // 2 Livrée 
                    // 3 Reglée
                    // 4 Relancée

                    // trouver l'étape
                    ComboboxItem cmbx = (ComboboxItem)cbxCommandeLivreDetailEtape.SelectedItem;
                    Int32.TryParse(cmbx.Value, out int etapeID);

                    // si la commande est livré cacher le bouton pour supprimer et montrer une message sinon faire l'inverse
                    if (etapeID > 1)
                    {
                        btnSupprimeCommandeLivre.Visible = false;
                        lblCommandeLivreNonSupprimable.Visible = true;
                    }
                    else
                    {
                        btnSupprimeCommandeLivre.Visible = true;
                        lblCommandeLivreNonSupprimable.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// Supprimer une commande de Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimeCommandeLivre_Click(object sender, EventArgs e)
        {
            string message = "Voulez-vous vraiment supprimer cette commande ?";
            string title = "Suppression de commande";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            if (result.ToString().ToLower() == "yes")
            {
                if (!string.IsNullOrWhiteSpace(lblCommandeLivreDetailID.Text))
                {
                    controle.SupprimerCommandeDvdLivre(lblCommandeLivreDetailID.Text);
                }
                // réinitialiser pnlCommandeLivreDetail
                pnlCommandeLivreDetail.Visible = false;
                lblCommandeLivreDetailID.Text = "";
                lblCommandeLivreDetailDateDeCommande.Text = "";
                txbCommandeLivreDetailMontant.Text = "";
                txbCommandeLivreDetailNExemplaires.Text = "";
                cbxCommandeLivreDetailEtape.Items.Clear();
                // réinitialiser la liste de commandes
                ChercheLivreetCommandes();

            }
        }
        /// <summary>
        /// Enregistrer une command de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnregistrerCommandeLivre_Click(object sender, EventArgs e)
        {
            lblErrorAjouteDocumentLivre.Text = "";
            string idLivre = txbCommandeLivreNumero.Text;
            // verifier saisie montant
            if (!Decimal.TryParse(txbNewCommandeLivreMontant.Text, out decimal montant))
            {
                lblErrorAjouteDocumentLivre.Text = "Montant incorrect, veuillez réessayer, merci.";
                return;
            }
            // verifier saisie nbExemplaires
            if (!Int32.TryParse(txbNewCommandeLivreNbExemplaires.Text, out int nbExemplaires))
            {
                lblErrorAjouteDocumentLivre.Text = "Nombre d'exemplaires incorrect, veuillez réessayer, merci.";
                return;
            }
            // verifier enregistrement
            if (!controle.EnregistrerCommandeDocument(idLivre, montant, nbExemplaires))
            {
                lblErrorAjouteDocumentLivre.Text = "Une erreur est survenue, merci de réesayer. (Err : 15d)";
            }
            //si l'enregistrement s'est bien passée, réinitialise les champs
            else
            {
                txbNewCommandeLivreMontant.Text = "";
                txbNewCommandeLivreNbExemplaires.Text = "";
                ChercheLivreetCommandes();
            }


        }

        /// <summary>
        /// Recuperer un livre avec ses commandes
        /// </summary>
        private void ChercheLivreetCommandes()
        {
            ComboboxItem item = (ComboboxItem)cbxFindLivreByNum.SelectedItem;
            if (item != null)
            {
                string Num = item.Value;
                if (!String.IsNullOrWhiteSpace(Num))
                {
                    Livre livre = lesLivres.Find(x => x.Id.Equals(Num));
                    if (livre != null)
                    {

                        txbCommandeLivreTitre.Text = livre.Titre;
                        txbCommandeLivreAuteur.Text = livre.Auteur;
                        txbCommandeLivreCheminImage.Text = livre.Image;
                        txbCommandeLivreCollection.Text = livre.Collection;
                        txbCommandeLivreGenre.Text = livre.Genre;
                        txbCommandeLivreNumero.Text = livre.Id;
                        txbCommandeLivreRayon.Text = livre.Rayon;
                        txbCommandeLivrePublic.Text = livre.Public;
                        txbCommandeLivresISBN.Text = Convert.ToString(livre.Isbn);
                        livre.Commandes = controle.GetCommandesdeDeDocument(livre.Id);

                        bdgDocumentAvecCommandes.DataSource = livre.Commandes;
                        dgvCommandesdeLivres.DataSource = bdgDocumentAvecCommandes;
                        dgvCommandesdeLivres.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        dgvCommandesdeLivres.Columns["id"].Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("numéro introuvable");

                    }
                }
            }
        }
        /// <summary>
        /// btnClick Chercher un livre et remplir les champs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresEtCommandesSearch_Click(object sender, EventArgs e)
        {
            ComboboxItem item = (ComboboxItem)cbxFindLivreByNum.SelectedItem;
            if (item != null)
            {
                string Num = item.Value;
                if (!String.IsNullOrWhiteSpace(Num))
                {

                    Livre livre = lesLivres.Find(x => x.Id.Equals(Num));
                    if (livre != null)
                    {

                        txbCommandeLivreTitre.Text = livre.Titre;
                        txbCommandeLivreAuteur.Text = livre.Auteur;
                        txbCommandeLivreCheminImage.Text = livre.Image;
                        txbCommandeLivreCollection.Text = livre.Collection;
                        txbCommandeLivreGenre.Text = livre.Genre;
                        txbCommandeLivreNumero.Text = livre.Id;
                        txbCommandeLivreRayon.Text = livre.Rayon;
                        txbCommandeLivrePublic.Text = livre.Public;
                        txbCommandeLivresISBN.Text = livre.Isbn;
                        UpdateCommandesListPourLivre(Num);



                    }
                    else
                    {
                        MessageBox.Show("numéro introuvable");

                    }
                }
            }
        }
        /// <summary>
        /// Recuperation des commandes pour un Livre
        /// </summary>
        /// <param name="LivreID"></param>
        private void UpdateCommandesListPourLivre(string LivreID)
        {
            List<CommandeDocument> commandes = controle.GetCommandesdeDeDocument(LivreID);
            // enregistrer pour le tri des colonnes 
            lesLivres.Find(x => x.Id.Equals(LivreID)).Commandes = commandes;
            bdgDocumentAvecCommandes.DataSource = commandes;
            dgvCommandesdeLivres.DataSource = bdgDocumentAvecCommandes;
            dgvCommandesdeLivres.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandesdeLivres.Columns["id"].Visible = false;
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesdeLivres_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesdeLivres.Columns[e.ColumnIndex].HeaderText.ToLower();
            ComboboxItem item = (ComboboxItem)cbxFindLivreByNum.SelectedItem;
            if (item != null)
            {
                List<CommandeDocument> sortedList;
                switch (titreColonne)
                {
                    case "datecommande":
                        sortedList = lesLivres.Find(x => x.Id.Equals(item.Value)).Commandes.OrderBy(o => o.DateCommande).ToList();
                        break;
                    case "nbexemplaire":
                        sortedList = lesLivres.Find(x => x.Id.Equals(item.Value)).Commandes.OrderBy(o => o.NbExemplaire).ToList();
                        break;
                    case "montant":
                        sortedList = lesLivres.Find(x => x.Id.Equals(item.Value)).Commandes.OrderBy(o => o.Montant).ToList();
                        break;
                    case "etapesuivi":
                        sortedList = lesLivres.Find(x => x.Id.Equals(item.Value)).Commandes.OrderBy(o => o.EtapeSuivi).ToList();
                        break;
                    default:
                        sortedList = lesLivres.Find(x => x.Id.Equals(item.Value)).Commandes.OrderBy(o => o.Id).Reverse().ToList();
                        break;

                }
                bdgDocumentAvecCommandes.DataSource = sortedList;
                dgvCommandesdeLivres.DataSource = bdgDocumentAvecCommandes;
                dgvCommandesdeLivres.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvCommandesdeLivres.Columns["id"].Visible = false;
            }
        }
        #endregion

        #region CommandeRevues
        //-----------------------------------------------------------
        // ONGLET "CommandeRevues"
        //------------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Commande Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandeRevues_Enter(object sender, EventArgs e)
        {

            if (lesRevues == null || lesRevues.Count == 0)
            {
                lesRevues = controle.GetAllRevues();
            }

            cbxFindRevueByNum.SelectedIndex = -1;
            cbxFindRevueByNum.Items.Clear();
            foreach (Revue unRevue in lesRevues)
            {
                ComboboxItem item = new ComboboxItem(unRevue.Id, unRevue.Id);
                cbxFindRevueByNum.Items.Add(item);
            }
        }

        /// <summary>
        /// Fermer panel pnlCommandeRevueDetail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClosCommandeRevueDetails_Click(object sender, EventArgs e)
        {
            pnlCommandeRevueDetail.Visible = false;
        }
        /// <summary>
        /// Montrer commande lorsqu'elle est selectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesdeRevue_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex != -1)
            {
                DataGridView dgv = sender as DataGridView;
                if (dgv == null)
                    return;
                // verifier que le row est selectionné
                if (dgv.CurrentRow.Selected)
                {
                    // chercher des valeurs du row selectionné
                    string id = Convert.ToString(dgv.CurrentRow.Cells["id"].Value);
                    string dateFinAbonnement = Convert.ToString(dgv.CurrentRow.Cells["dateFinAbonnement"].Value);
                    string montant = Convert.ToString(dgv.CurrentRow.Cells["montant"].Value);
                    string datecommande = Convert.ToString(dgv.CurrentRow.Cells["datecommande"].Value);

                    // remplir les textboxes
                    pnlCommandeRevueDetail.Visible = true;
                    lblCommandeRevueIdAbonnement.Text = id;
                    lblCommandeRevueDetailDateDeCommande.Text = datecommande;
                    lblCommandeRevueDetailMontant.Text = montant;
                    lblNewCommandeRevueFinAbonnement.Text = dateFinAbonnement;

                    //verfier si commande est supprimable
                    if (Controle.EstAbonnementSupprimable(Convert.ToDateTime(datecommande), Convert.ToDateTime(dateFinAbonnement), lblCommandeRevueNumero.Text))
                    {
                        lblCommandeRevueNonSupprimable.Visible = false;
                        btnSupprimeCommandeRevue.Visible = true;
                    }
                    else
                    {
                        lblCommandeRevueNonSupprimable.Visible = true;
                        btnSupprimeCommandeRevue.Visible = false;
                    }


                }
            }
        }

        /// <summary>
        /// Enregistrer une command de Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnregistrerAbonnementRevue_Click(object sender, EventArgs e)
        {
            lblErrorAjouteDocumentRevue.Text = "";
            string idRevue = lblCommandeRevueNumero.Text;
            // verifier saisie montant
            if (!Decimal.TryParse(txbNewCommandeRevueMontant.Text, out decimal montant))
            {
                lblErrorAjouteDocumentRevue.Text = "Montant incorrect, veuillez réessayer, merci.";
                return;
            }
            // verifier saisie nbExemplaires
            if (!DateTime.TryParse(txbNewAbonnementDateFin.Text, out DateTime dateFinAbonnement))
            {

                lblErrorAjouteDocumentRevue.Text = "Date de fin d'abonnement incorrect, veuillez réessayer, merci.";
                return;
            }
            else if (dateFinAbonnement < DateTime.Now)
            {
                lblErrorAjouteDocumentRevue.Text = "Date de fin d'abonnement incorrect, merci de saisir une date future.";
                return;
            }
            // verifier enregistrement
            if (!controle.EnregistrerRevueAbonnement(idRevue, montant, dateFinAbonnement))
            {
                lblErrorAjouteDocumentRevue.Text = "Une erreur est survenue, merci de réesayer. (Err : 15e)";

            }
            //si l'enregistrement s'est bien passée, réinitialise les champs
            else
            {
                txbNewCommandeRevueMontant.Text = "";
                txbNewAbonnementDateFin.Text = "";
                ChercheRevueetCommandes();
            }


        }

        /// <summary>
        /// Recuperer un Revue avec ses commandes
        /// </summary>
        private void ChercheRevueetCommandes()
        {
            ComboboxItem item = (ComboboxItem)cbxFindRevueByNum.SelectedItem;
            if (item != null)
            {
                string Num = item.Value;
                if (!String.IsNullOrEmpty(Num))
                {
                    Revue Revue = lesRevues.Find(x => x.Id.Equals(Num));
                    if (Revue != null)
                    {

                        txbCommandeRevueTitre.Text = Revue.Titre;
                        txbCommandeRevuePeriodicite.Text = Revue.Periodicite;
                        txbReceptionRevueDelaiMiseADispo.Text = Revue.DelaiMiseADispo.ToString();

                        txbCommandeRevueGenre.Text = Revue.Genre;
                        lblCommandeRevueNumero.Text = Revue.Id;
                        txbCommandeRevueRayon.Text = Revue.Rayon;
                        txbCommandeRevuePublic.Text = Revue.Public;
                        chkCommandeRevuesEmpruntable.Checked = Revue.Empruntable;
                        Revue.Abonnements = controle.GetAbonnementsDeRevue(Revue.Id);

                        bdgDocumentAvecCommandes.DataSource = Revue.Abonnements;
                        dgvCommandesdeRevue.DataSource = bdgDocumentAvecCommandes;
                        dgvCommandesdeRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        dgvCommandesdeRevue.Columns["id"].Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("numéro introuvable");

                    }
                }
            }
        }
        /// <summary>
        /// btnClick Chercher un Revue et remplir les champs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesEtAbonnementsSearch_Click(object sender, EventArgs e)
        {
            ComboboxItem item = (ComboboxItem)cbxFindRevueByNum.SelectedItem;
            if (item != null)
            {
                string Num = item.Value;
                if (!String.IsNullOrEmpty(Num))
                {

                    Revue Revue = lesRevues.Find(x => x.Id.Equals(Num));
                    if (Revue != null)
                    {

                        txbCommandeRevueTitre.Text = Revue.Titre;
                        txbCommandeRevuePeriodicite.Text = Revue.Periodicite;
                        txbReceptionRevueDelaiMiseADispo.Text = Revue.DelaiMiseADispo.ToString();
                        txbCommandeRevueGenre.Text = Revue.Genre;
                        lblCommandeRevueNumero.Text = Revue.Id;
                        txbCommandeRevueRayon.Text = Revue.Rayon;
                        txbCommandeRevuePublic.Text = Revue.Public;
                        txbCommandeRevueImage.Text = Revue.Image;
                        chkCommandeRevuesEmpruntable.Checked = Revue.Empruntable;
                        UpdateCommandesListPourRevue(Num);



                    }
                    else
                    {
                        MessageBox.Show("numéro introuvable");

                    }
                }
            }
        }
        /// <summary>
        /// Recuperation des commandes pour un Revue
        /// </summary>
        /// <param name="RevueID"></param>
        private void UpdateCommandesListPourRevue(string RevueID)
        {
            List<CommandeRevue> commandes = controle.GetAbonnementsDeRevue(RevueID);
            // enregistrer pour le tri des colonnes 
            lesRevues.Find(x => x.Id.Equals(RevueID)).Abonnements = commandes;
            bdgDocumentAvecCommandes.DataSource = commandes;
            dgvCommandesdeRevue.DataSource = bdgDocumentAvecCommandes;
            dgvCommandesdeRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandesdeRevue.Columns["id"].Visible = false;
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesdeRevue_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesdeRevue.Columns[e.ColumnIndex].HeaderText.ToLower();

            ComboboxItem item = (ComboboxItem)cbxFindRevueByNum.SelectedItem;
            if (item != null)
            {
                List<CommandeRevue> sortedList;
                switch (titreColonne)
                {
                    case "datecommande":
                        sortedList = lesRevues.Find(x => x.Id.Equals(item.Value)).Abonnements.OrderBy(o => o.DateCommande).ToList();
                        break;
                    case "datefinabonnement":
                        sortedList = lesRevues.Find(x => x.Id.Equals(item.Value)).Abonnements.OrderBy(o => o.DateFinAbonnement).ToList();
                        break;
                    case "montant":
                        sortedList = lesRevues.Find(x => x.Id.Equals(item.Value)).Abonnements.OrderBy(o => o.Montant).ToList();
                        break;

                    default:
                        sortedList = lesRevues.Find(x => x.Id.Equals(item.Value)).Abonnements.OrderBy(o => o.Id).Reverse().ToList();
                        break;

                }
                bdgDocumentAvecCommandes.DataSource = sortedList;
                dgvCommandesdeRevue.DataSource = bdgDocumentAvecCommandes;
                dgvCommandesdeRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvCommandesdeRevue.Columns["id"].Visible = false;
            }
        }

        /// <summary>
        /// Supprimer un abonnement de Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimeAbonnementRevue_Click(object sender, EventArgs e)
        {
            string message = "Voulez-vous vraiment supprimer cet abonnement ?";
            string title = "Suppression d'Abonnement";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            if (result.ToString().ToLower() == "yes" && !string.IsNullOrWhiteSpace(lblCommandeRevueIdAbonnement.Text))
            {

                // récuperer revue
                Revue revue = lesRevues.Find(x => x.Id.Equals(lblCommandeRevueNumero.Text));
                // récuperer commande
                CommandeRevue cv = revue.Abonnements.Find(x => x.Id.Equals(lblCommandeRevueIdAbonnement.Text));

                if (Controle.EstAbonnementSupprimable(cv.DateCommande, cv.DateFinAbonnement, lblCommandeRevueIdAbonnement.Text))
                {
                    controle.SupprimerAbonnement(lblCommandeRevueIdAbonnement.Text);
                    // réinitialiser pnlCommandeRevueDetail
                    pnlCommandeRevueDetail.Visible = false;
                    lblCommandeRevueIdAbonnement.Text = "";
                    lblCommandeRevueDetailDateDeCommande.Text = "";
                    lblCommandeRevueDetailMontant.Text = "";


                    // réinitialiser la liste de commandes
                    ChercheRevueetCommandes();
                }




            }

        }
        #endregion

        #region Revues
        //-----------------------------------------------------------
        // ONGLET "Revues"
        //------------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controle.GetAllRevues();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["empruntable"].Visible = false;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>();
                    revues.Add(revue);
                    //remplir les textboxes etc. sur le form
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche avec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            chkRevuesEmpruntable.Checked = revue.Empruntable;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            chkRevuesEmpruntable.Checked = false;
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        #endregion


        #region Livres

        //-----------------------------------------------------------
        // ONGLET "LIVRES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controle.GetAllLivres();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();

        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>();
                    livres.Add(livre);
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {

            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        #endregion


        #region Dvd
        //-----------------------------------------------------------
        // ONGLET "DVD"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controle.GetAllDvd();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>();
                    Dvd.Add(dvd);
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        #endregion


        #region Réception Exemplaire de presse
        //-----------------------------------------------------------
        // ONGLET "RECEPTION DE REVUES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet : blocage en saisie des champs de saisie des infos de l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controle.GetAllRevues();
            accesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            bdgExemplairesListe.DataSource = exemplaires;
            dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
            dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
            dgvReceptionExemplairesListe.Columns["idDocument"].Visible = false;
            dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
            dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    VideReceptionRevueInfos();
                }
            }
            else
            {
                VideReceptionRevueInfos();
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            accesReceptionExemplaireGroupBox(false);
            VideReceptionRevueInfos();
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            chkReceptionRevueEmpruntable.Checked = revue.Empruntable;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            afficheReceptionExemplairesRevue();
            // accès à la zone d'ajout d'un exemplaire
            accesReceptionExemplaireGroupBox(true);
        }

        private void afficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controle.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
        }

        /// <summary>
        /// Vide les zones d'affchage des informations de la revue
        /// </summary>
        private void VideReceptionRevueInfos()
        {
            txbReceptionRevuePeriodicite.Text = "";
            chkReceptionRevueEmpruntable.Checked = false;
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            lesExemplaires = new List<Exemplaire>();
            RemplirReceptionExemplairesListe(lesExemplaires);
            accesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de l'exemplaire
        /// </summary>
        private void VideReceptionExemplaireInfos()
        {
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces"></param>
        private void accesReceptionExemplaireGroupBox(bool acces)
        {
            VideReceptionExemplaireInfos();
            grpReceptionExemplaire.Enabled = acces;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controle.CreerExemplaire(exemplaire))
                    {
                        VideReceptionExemplaireInfos();
                        afficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// Sélection d'une ligne complète et affichage de l'image sz l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }

        #endregion




        public class ComboboxItem
        {
            public ComboboxItem(string text, string value)
            {
                this.Text = text;
                this.Value = value;
            }
            public string Text { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                return Text;
            }

        }

        private void btnFermerNotifAbonRevues_Click(object sender, EventArgs e)
        {
            pnlNotifAbonRevues.Visible = false;
        }


    }
}
