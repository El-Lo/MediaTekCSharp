namespace Mediatek86.vue
{
    partial class FrmLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txbNomUtilisateur = new System.Windows.Forms.TextBox();
            this.txbMotDePasse = new System.Windows.Forms.TextBox();
            this.btnAuthentifier = new System.Windows.Forms.Button();
            this.lblEchecAuthentification = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(89, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(213, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Authentification";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Nom d\'Utilisateur";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Mot de Passe";
            // 
            // txbNomUtilisateur
            // 
            this.txbNomUtilisateur.Location = new System.Drawing.Point(179, 130);
            this.txbNomUtilisateur.Name = "txbNomUtilisateur";
            this.txbNomUtilisateur.Size = new System.Drawing.Size(172, 22);
            this.txbNomUtilisateur.TabIndex = 3;
            // 
            // txbMotDePasse
            // 
            this.txbMotDePasse.Location = new System.Drawing.Point(179, 179);
            this.txbMotDePasse.Name = "txbMotDePasse";
            this.txbMotDePasse.PasswordChar = '*';
            this.txbMotDePasse.Size = new System.Drawing.Size(172, 22);
            this.txbMotDePasse.TabIndex = 4;
            // 
            // btnAuthentifier
            // 
            this.btnAuthentifier.BackColor = System.Drawing.Color.SteelBlue;
            this.btnAuthentifier.ForeColor = System.Drawing.SystemColors.Control;
            this.btnAuthentifier.Location = new System.Drawing.Point(45, 242);
            this.btnAuthentifier.Name = "btnAuthentifier";
            this.btnAuthentifier.Size = new System.Drawing.Size(325, 62);
            this.btnAuthentifier.TabIndex = 5;
            this.btnAuthentifier.Text = "Authentifier";
            this.btnAuthentifier.UseVisualStyleBackColor = false;
            this.btnAuthentifier.Click += new System.EventHandler(this.btnAuthentifier_Click);
            // 
            // lblEchecAuthentification
            // 
            this.lblEchecAuthentification.AutoSize = true;
            this.lblEchecAuthentification.Location = new System.Drawing.Point(42, 345);
            this.lblEchecAuthentification.Name = "lblEchecAuthentification";
            this.lblEchecAuthentification.Size = new System.Drawing.Size(158, 17);
            this.lblEchecAuthentification.TabIndex = 6;
            this.lblEchecAuthentification.Text = "Echec d\'authentification";
            this.lblEchecAuthentification.Visible = false;
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 431);
            this.Controls.Add(this.lblEchecAuthentification);
            this.Controls.Add(this.btnAuthentifier);
            this.Controls.Add(this.txbMotDePasse);
            this.Controls.Add(this.txbNomUtilisateur);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FrmLogin";
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txbNomUtilisateur;
        private System.Windows.Forms.TextBox txbMotDePasse;
        private System.Windows.Forms.Button btnAuthentifier;
        private System.Windows.Forms.Label lblEchecAuthentification;
    }
}