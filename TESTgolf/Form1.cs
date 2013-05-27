using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using System.Configuration;
using System.Collections.Specialized;

namespace TESTgolf
{
    public partial class TESTGOLF : Form
    {
        private Golfspelare markeradSpelare;
        private Tävlingar markeradTävling;
    
        public TESTGOLF()
        {
            InitializeComponent();          
            //test
            
        }
        private void TESTGOLF_Load(object sender, EventArgs e)
        {            
            lbMedlemsregister.DataSource = Databas.GetGolfspelarlista();
            lbTävlingar.DataSource = Databas.GetTävlingslista();
        }
        private void btnRegistrera_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();    //Används till att skapa ett unikt golfid
            int idnummer = rnd.Next(100, 999);
            int status = 0;
            bool medlemsavg;

            if (rdoAktiv.Checked)
            {
                status = 1;
            }
            else if (rdoVilande.Checked)
            {
                status = 2;
            }
            else if (rdoJunior.Checked)
            {
                status = 3;
            }
            else if (rdoGreenfee.Checked)
            {
                status = 4;
            }
            else if (rdoEjklubbmedlem.Checked)
            {
                status = 5;
            }

            if (cbMedlemsavgift.Checked)
            {
                medlemsavg = true;
            }
            else
            {
                medlemsavg = false;
            }

            Golfspelare nySpelare = new Golfspelare { GolfId = Convert.ToInt32(txtPersonnr.Text), Fornamn = txtFornamn.Text, Efternamn = txtEfternamn.Text, Mobil = txtMobil.Text, Adress = txtGatuadress.Text, GatuNr = txtGatunummer.Text, PostNr = Convert.ToInt32(txtPostnummer.Text), Email = txtEmail.Text, Medlemsavg = medlemsavg, Handicap = txtHandicap.Text, Status = status };
            
            string golfid = nySpelare.GolfId.ToString() + idnummer.ToString(); //lägger över talen till sträng så jag kan lägga ihop dem
            nySpelare.GolfId = Convert.ToInt32(golfid); //konverterar om till int så det går in i databasen                        
            
            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=golfkolltest;User Id=patrick;Password=patrick;");
            try
            {               
                conn.Open();
                string insert = "INSERT INTO golfspelare (golf_id, status_id, fornamn, efternamn, mobil, epost,gatuadress,gatunummer, handicap, medlemsavgift, postnummer) VALUES (" + nySpelare.GolfId + "," + nySpelare.Status + ",'" + nySpelare.Fornamn + "','" + nySpelare.Efternamn + "','" + nySpelare.Mobil + "','" + nySpelare.Email +"','" + nySpelare.Adress +"','" + nySpelare.GatuNr + "'," + nySpelare.Handicap +"," + nySpelare.Medlemsavg +",'" + nySpelare.PostNr +"')";
                NpgsqlCommand command = new NpgsqlCommand(insert, conn);
                //används när man kör INSERT fråga
               int antal = command.ExecuteNonQuery();  
            }
            catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            finally
            {
                conn.Close();
            }
            lbMedlemsregister.DataSource = Databas.GetGolfspelarlista();
            }

        private void btnSkapa_tävling_Click(object sender, EventArgs e)
        {
            
            string tavlingsnamn = txtTävlingsnamn.Text;
            DateTime startdatum = Convert.ToDateTime(txtStarttid.Text);
            DateTime slutdatum = Convert.ToDateTime(txtStopptid.Text);
            DateTime sAnmalningsdatum = Convert.ToDateTime(txtAnmälningsdatum.Text);
            DateTime sAvbokningsdatum = Convert.ToDateTime(txtAvbokningsdatum.Text);            
            string klassA = txtKlassA.Text;
            string klassB = txtKlassB.Text;
            string klassC = txtKlassC.Text;
            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=golfkolltest;User Id=patrick;Password=patrick;");
            try
            {               
                conn.Open();
                string insert = "INSERT INTO tavling (tavlingsnamn, startdatum, slutdatum, sista_anmalningsdatum, sista_avbokningsdatum, klass_a, klass_b, klass_c) VALUES ('" + tavlingsnamn + "','" + startdatum + "','" + slutdatum + "','" + sAnmalningsdatum + "','" + sAvbokningsdatum + "'," + klassA +"," + klassB +"," + klassC + ")";
                NpgsqlCommand command = new NpgsqlCommand(insert, conn);
                //används när man kör INSERT fråga
               int antal = command.ExecuteNonQuery();  
            }
            catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            finally
            {
                lbTävlingar.DataSource = Databas.GetTävlingslista();
                conn.Close();   
            }
            txtTävlingsnamn.Clear();
            txtStarttid.Clear();
            txtStopptid.Clear();
            txtAnmälningsdatum.Clear();
            txtAvbokningsdatum.Clear();
            txtKlassA.Clear();
            txtKlassB.Clear();
            txtKlassC.Clear();
            }

        private void lbMedlemsregister_SelectedIndexChanged(object sender, EventArgs e)
        {
            markeradSpelare = (Golfspelare)lbMedlemsregister.SelectedItem;
            
         //   lbMedlemsregister.DataSource = Databas.GetGolfSpelarInfo(markeradSpelare.GolfId);
            
            txtFornamn.Text = markeradSpelare.Fornamn;
            txtEfternamn.Text = markeradSpelare.Efternamn;
            txtMobil.Text = markeradSpelare.Mobil;
            txtGatuadress.Text = markeradSpelare.Adress;
            txtGatunummer.Text = markeradSpelare.GatuNr;
            txtPostnummer.Text = markeradSpelare.PostNr.ToString();
            txtEmail.Text = markeradSpelare.Email;
    //        txtHandicap.Text = markeradSpelare.Handicap.ToString();
            
            //nytt tillägg:
            //lbGolfspelare.DataSource = Databas.GetTävlingsFromSpelare(markeradSpelare.GolfId);
        }

        private void btnTavlingar_Click(object sender, EventArgs e)
        {
            
        }
        private void lbTävlingar_SelectedIndexChanged(object sender, EventArgs e)
        {
            markeradTävling = (Tävlingar)lbTävlingar.SelectedItem;
            lbGolfspelare.DataSource = Databas.TävlingsSpelar(markeradTävling.tavlingId);
        } 

        private void btnLägg_till_i_tävling_Click(object sender, EventArgs e)
        {       
            Databas.AddSpelareTillTävling(txtFornamn.Text, txtEfternamn.Text, Convert.ToString(lbTävlingar.SelectedValue));
        }

        private void btnTa_bort_spelare_Click(object sender, EventArgs e)
        {
            taBortSpelare();
        }

        private void taBortSpelare()
        {
            string golfspelarid = txtTaBortId.Text;

            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=golfkolltest;User Id=patrick;Password=patrick");
            try
            {
                string sql = "DELETE FROM golfspelare WHERE golf_id = '" + golfspelarid + "'";
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                int antal = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            txtTaBortId.Clear();
            lbMedlemsregister.DataSource = Databas.GetGolfspelarlista();          

        }

        private void btnUppdatera_golfspelare_Click(object sender, EventArgs e)
        {
            Databas.UppdateraSpelare(markeradSpelare.GolfId, txtFornamn.Text, txtEfternamn.Text, txtMobil.Text, txtGatuadress.Text,
                txtGatunummer.Text, Convert.ToInt32(txtPostnummer.Text), txtEmail.Text, Convert.ToDouble(txtHandicap.Text));
        }

        private void txtVisaMedlemmar_Click(object sender, EventArgs e)
        {
            lbMedlemsregister.DataSource = Databas.GetGolfspelarlista();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void lbGolfspelare_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnUppdatera_resultat_Click(object sender, EventArgs e)
        {
            ResultatPåSPelare();
        }

        
   /*     private void spelarTävling()
        {
            string selectionTemp = Convert.ToString(txtTävlingsnamn.Text);

            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=golfkolltest;User Id=patrick;Password=patrick;");
            try
            {
                string sql = "SELECT golf_id FROM spelar_resultat WHERE tavlings_id = '" + selectionTemp + "'";
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                int antal = command.ExecuteNonQuery();
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    lbGolfspelare.Items.Add(dr[0] + " " + dr[1] + " " + dr[2]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
    */
        private void ResultatPåSPelare()
        {
            int resultat = Convert.ToInt32(txtResultat.Text);
            int golfid = Convert.ToInt32(txtPersonnr.Text);
            int tavlingsid = Convert.ToInt32(txtResultatId.Text);


            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=golfkolltest;User Id=patrick;Password=patrick");
            try
            {
                string sql = "UPDATE spelar_resultat SET resultat = " + resultat + " WHERE golf_id = " + golfid + " AND tavlings_id = " + tavlingsid + "";
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                int antal = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }

        }
    }
        
}
        


        
    

