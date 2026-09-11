using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSC09
{
    public class cnn
    {
        public static string db = ConfigurationManager.ConnectionStrings["sistemaFacturacion"].ConnectionString;
    }

    public class Item
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public Item(string _name, int _value)
        {
            Name = _name; 
            Value = _value;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Busco
    {
        public static string BuscaUltimoNumero(string nmId)
        {
            string stQuery = "SELECT secuencia + 1 AS ultimo_numero FROM SECUENCIA WHERE ID = @id";

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            SqlCommand cmd = new SqlCommand(stQuery, cnx);
            cmd.Parameters.AddWithValue("@id", nmId);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.Read())
            {
                return sdr["ultimo_numero"].ToString();
            }

            cmd.Dispose();
            cnx.Close();

            return null;
        }
    }
}
