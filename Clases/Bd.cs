using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace LectorQR.Clases
{
    class Bd
    {
        string ConnectionString = string.Empty;

        int CommandTimeout = 1200;
        List<SqlParameter> sqlPc = null;

        public Bd(string myConnectionString)
        {
            ConnectionString = myConnectionString;
            sqlPc = new List<SqlParameter>();
        }

        public System.Data.DataTable ExecutarSPDatatable(string StoredProcedure)
        {
            DataTable myDatatable = new DataTable();
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (SqlDataAdapter oDA = new SqlDataAdapter(StoredProcedure, cnn))
                {
                    oDA.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    oDA.SelectCommand.CommandTimeout = CommandTimeout;
                    oDA.SelectCommand.Parameters.Clear();

                    foreach (SqlParameter MyParametro in sqlPc) // Loop through List with foreach
                        oDA.SelectCommand.Parameters.Add(MyParametro);

                    oDA.Fill(myDatatable);
                    oDA.Dispose();
                }
                cnn.Close();
                cnn.Dispose();
            }
            return myDatatable;
        }

        public DataSet ExecutarSPDataset(string StoredProcedure)
        {
            DataSet myDataSet = new DataSet();
            myDataSet.EnforceConstraints = false; //
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (SqlDataAdapter oDA = new SqlDataAdapter(StoredProcedure, ConnectionString))
                {
                    oDA.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    oDA.SelectCommand.CommandTimeout = CommandTimeout;
                    oDA.SelectCommand.Parameters.Clear();

                    foreach (SqlParameter MyParametro in sqlPc) // Loop through List with foreach
                        oDA.SelectCommand.Parameters.Add(MyParametro);

                    oDA.Fill(myDataSet);
                    oDA.Dispose();
                }
                cnn.Close();
                cnn.Dispose();
            }
            return myDataSet;
        }

        public System.Data.SqlClient.SqlDataReader ExecutarSPDataReader(string StoredProcedure)
        {
            SqlConnection cnn = new SqlConnection(ConnectionString);
            cnn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = cnn;
            cmd.CommandTimeout = CommandTimeout;
            cmd.CommandText = StoredProcedure;
            cmd.Parameters.Clear();
            foreach (SqlParameter MyParametro in sqlPc) // Loop through List with foreach
                cmd.Parameters.Add(MyParametro);

            SqlDataReader dr = cmd.ExecuteReader();

            cmd = null;
            return dr;
        }

        public void AgregarParametros(string Nombre, System.Object Valor)
        {
            sqlPc.Add(new SqlParameter("@" + Nombre, Valor));
        }
    }
}
