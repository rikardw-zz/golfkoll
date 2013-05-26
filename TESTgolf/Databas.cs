using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using System.Data;
using Npgsql;
using System.Collections.Specialized; //lägger till denna för att kunna använda Connectionstrings, källa http://stackoverflow.com/questions/1274852/the-name-configurationmanager-does-not-exist-in-the-current-context



namespace TESTgolf
{
    class Databas
    {
        public const string conString = "GOLF";

        //metod som hämtar golfspelare
        public static List<Golfspelare> GetGolfspelarlista()
        {
            List<Golfspelare> golfspelarlista = new List<Golfspelare>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM golfspelare order by efternamn, fornamn", conn);
            NpgsqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                Golfspelare golfspelare = new Golfspelare
                {
                    GolfId = (int)dr["golf_id"],   
                    Fornamn = (string)dr["fornamn"],
                    Efternamn = (string)dr["efternamn"],
                    Status = (int)dr["status_id"],
                    Mobil = (string)dr["mobil"],
                    Adress = (string)dr["gatuadress"],
                    GatuNr = (string)dr["gatunummer"],
                    PostNr = (int)dr["postnummer"],
                    Email = (string)dr["epost"],
                    Medlemsavg = (bool)dr["medlemsavgift"],
              //      Handicap = (string)dr["handicap"]  kan ej visa denna. förmodligen för att vi skickar in den som string till databas
                };
                golfspelarlista.Add(golfspelare);
            }
            conn.Close();
            return golfspelarlista;
        }

   
        public static List<Tävlingar> GetTävlingslista()
        {
    
            List<Tävlingar> tävlingslista = new List<Tävlingar>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            conn.Open();
            NpgsqlCommand command1 = new NpgsqlCommand("SELECT * FROM tavling ORDER BY id", conn);
            NpgsqlDataReader dr = command1.ExecuteReader();
            while (dr.Read())
            {
                Tävlingar aktuelltävling = new Tävlingar
                {
                    TävlingsNamn = (string)(dr["tavlingsnamn"])
                };

                tävlingslista.Add(aktuelltävling);
            }
            conn.Close();
            return tävlingslista;
        }
        //Ska detta vara med?
     /*   public static List<string> GetGolfSpelarInfo(int GolfId)
        {
            List<string> medlemslista = new List<string>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            conn.Open();
            NpgsqlCommand command2 = new NpgsqlCommand(@"SELECT *
                                                        FROM 
                                                          golfspelare
                                                           WHERE 
                                                          golf_id = :GolfId;
                                                        ", conn);

            command2.Parameters.Add(new NpgsqlParameter("GolfId", DbType.Int32));
            command2.Parameters[0].Value = Convert.ToString(GolfId);
            NpgsqlDataReader dr = command2.ExecuteReader();
            while (dr.Read())
            {
                medlemslista.Add((string)dr["*"]);
            }
            conn.Close();
            return medlemslista;
        }*/
        
        public static void AddSpelareTillTävling(string Fornamn, string Efternamn, string Tävlingsnamn)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            NpgsqlTransaction trans = null;
            try
            {
                conn.Open();
                trans = conn.BeginTransaction();

                NpgsqlCommand command3 = new NpgsqlCommand(@"SELECT golf_id
                                                        FROM golfspelare
                                                        WHERE fornamn = :Fornamn
                                                        AND efternamn =:Efternamn", conn);
                command3.Parameters.Add(new NpgsqlParameter("Fornamn", DbType.String));
                command3.Parameters[0].Value = Fornamn;
                command3.Parameters.Add(new NpgsqlParameter("Efternamn", DbType.String));
                command3.Parameters[1].Value = Efternamn;
                command3.Transaction = trans;
                int GolfId = (int)command3.ExecuteScalar();


                NpgsqlCommand command4 = new NpgsqlCommand(@"SELECT id
                                                        FROM tavling
                                                        WHERE tavlingsnamn = :TävlingsNamn", conn);
                command4.Parameters.Add(new NpgsqlParameter("TävlingsNamn", DbType.String));
                command4.Parameters[0].Value = Tävlingsnamn;
                command4.Transaction = trans;
                int TävlingsID = (int)command4.ExecuteScalar();


         /*       NpgsqlCommand command4 = new NpgsqlCommand(@"SELECT id
                                                        FROM tavling
                                                        WHERE tavlingsnamn = :TävlingsNamn", conn);
                command4.Parameters.Add(new NpgsqlParameter("tavlingsnamn", DbType.String));
                command4.Parameters[0].Value = Tävlingsnamn;
                command4.Transaction = trans;
                int TävlingsID = (int)command4.ExecuteScalar(); */

                NpgsqlCommand command5 = new NpgsqlCommand(@"INSERT INTO spelar_resultat (golf_id, tavlings_id)
                                                        VALUES (:GolfId, :TävlingsID)", conn);
                command5.Parameters.Add(new NpgsqlParameter("GolfId", DbType.Int32));
                command5.Parameters[0].Value = GolfId;
                command5.Parameters.Add(new NpgsqlParameter("TävlingsID", DbType.Int32));
                command5.Parameters[1].Value = TävlingsID;
                command5.Transaction = trans;
                int numberOfAffectedRows = command5.ExecuteNonQuery();
                trans.Commit();
            }
            catch (NpgsqlException ex)
            {
                trans.Rollback();
            }
            finally
            {
                conn.Close();
            }
        }


        public static void UppdateraSpelare(int golfid, string nyttFornamn, string nyttEfternamn, string nyMobil, string nyGatuadress, 
            string nyttGatunr, int nyttPostnr, string nyEmail, double nyttHandicap)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand(@"UPDATE golfspelare
                                                        SET fornamn =:Fornamn,
                                                            efternamn =:Efternamn,
                                                            mobil =:Mobil,
                                                            gatuadress =:Gatuadress,
                                                            gatunummer =:Gatunummer,
                                                            postnummer =:Postnummer,
                                                            epost =:Email,
                                                            handicap =:Handicap
                                                            WHERE golf_id = :golfid", conn);
            command.Parameters.Add(new NpgsqlParameter("Fornamn", DbType.String));
            command.Parameters[0].Value = nyttFornamn;
            command.Parameters.Add(new NpgsqlParameter("Efternamn", DbType.String));
            command.Parameters[1].Value = nyttEfternamn;
            command.Parameters.Add(new NpgsqlParameter("Mobil", DbType.String));
            command.Parameters[2].Value = nyMobil;
            command.Parameters.Add(new NpgsqlParameter("Gatuadress", DbType.String));
            command.Parameters[3].Value = nyGatuadress;
            command.Parameters.Add(new NpgsqlParameter("Gatunummer", DbType.String));
            command.Parameters[4].Value = nyttGatunr;
            command.Parameters.Add(new NpgsqlParameter("Postnummer", DbType.Int32));
            command.Parameters[5].Value = Convert.ToInt32(nyttPostnr);
            command.Parameters.Add(new NpgsqlParameter("Email", DbType.String));
            command.Parameters[6].Value = nyEmail;
            command.Parameters.Add(new NpgsqlParameter("Handicap", DbType.Double));
            command.Parameters[7].Value = Convert.ToDouble(nyttHandicap);
            command.Parameters.Add(new NpgsqlParameter("golfid", DbType.Int32));
            command.Parameters[8].Value = Convert.ToInt32(golfid);

            int numberOfRowsAffected = command.ExecuteNonQuery();
            conn.Close();
            }

 /*       public static List<string> TävlingsSpelar(int spelarID)
        {
            List<string> tävlingsspel = new List<string>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            conn.Open();
            NpgsqlCommand command7 = new NpgsqlCommand(@"SELECT golfspelare.fornamn
                                                        FROM
                                                        spelar_resultat,
                                                        golfspelare
                                                        WHERE
                                                        spelar_resultat.golf_id = golfspelare.golf_id
                                                        AND tavlings_id =:tävlingsid", conn); //förmodligen något fel på denna rad

            command7.Parameters.Add(new NpgsqlParameter("tävlingsid", DbType.Int32));
            command7.Parameters[0].Value = Convert.ToInt32(spelarID);
            NpgsqlDataReader dr = command7.ExecuteReader();
            while (dr.Read())
            {
                tävlingsspel.Add((string)dr["fornamn"]);  
            }
            conn.Close();
            return tävlingsspel;
        }
  */
        public static List<string> GetTävlingsFromSpelare(int test)
        {
            List<string> golflista = new List<string>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT tavling.tavlingsnamn
                                                        FROM 
                                                          spelar_resultat, 
                                                          tavling
                                                        WHERE 
                                                          spelar_resultat.tavlings_id = tavling.id
                                                          AND spelar_resultat.golf_id = :hej;", conn);

            command.Parameters.Add(new NpgsqlParameter("hej", DbType.Int32));
            command.Parameters[0].Value = Convert.ToInt32(test);
            NpgsqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                golflista.Add((string)dr["tavlingsnamn"]);
            }
            conn.Close();
            return golflista;
        }


    }
}