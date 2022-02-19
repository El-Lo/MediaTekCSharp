using Mediatek86.controleur;
using Mediatek86.modele.utilisateur;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace Mediatek86.vue
{
    public partial class FrmLogin : Form
    {
        private readonly Controle controle;
        private readonly LoginControlleur login;
        internal FrmLogin(Controle controle, LoginControlleur login)
        {
            InitializeComponent();
            this.controle = controle;
            this.login = login;
        }
        /// <summary>
        /// Authentifier l'utilisateur 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAuthentifier_Click(object sender, EventArgs e)
        {
            // Réinitialiser le message d'erreur
            lblEchecAuthentification.Visible = false;
            // Vérifier que l'utilisateur a saisie un mot de passe et un nom d'utilisateur
            if (String.IsNullOrWhiteSpace(txbMotDePasse.Text) || String.IsNullOrWhiteSpace(txbNomUtilisateur.Text))
            {
                lblEchecAuthentification.Visible = true;
            }
            else
            {
                lblEchecAuthentification.Visible = false;
                string NomUtilisateur = txbNomUtilisateur.Text;
                // créer hash du mot de passe 
                byte[] HashValue = null;
                using (SHA512Managed sha = new SHA512Managed())
                {
                    HashValue = sha.ComputeHash(ASCIIEncoding.UTF8.GetBytes(txbMotDePasse.Text));
                }
                // Si authentification reussi
                if (login.VerifierLogin(NomUtilisateur, Convert.ToBase64String(HashValue)))
                {
                    if (Utilisateur.Service == Role.admin || Utilisateur.Service == Role.pres)
                    { 
                        // Fermer FrmLogin
                        this.Dispose();
                        // Montrer la fenêtre principale
                        controle.showFrmMediaTek(Utilisateur.Service);
                    }
                    else
                    {
                        // La personne n'a pas le droit d'utiliser l'application, donc fermer l'application après un message d'information
                        string message = "Vos droits ne sont pas suffisants pour accéder à cette application, donc l'application se fermera.";
                        string title = "Erreur d'authentificatoin";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
                        // Fermer l'application
                        System.Windows.Forms.Application.Exit();
                    }
                }
                else
                {
                    // Montrer le message échec d'authentification
                    lblEchecAuthentification.Visible = true;
                }
            }
        }
    }
}
