using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace LectorQR.Clases
{
    public class DBConexion
    {
        public ModeloAcceso ObtenerIdtarjeta(string IdTarjeta)
        {
            ModeloAcceso salida = new ModeloAcceso();
            EstatusAcceso estatusAcceso = new EstatusAcceso();
            List<AccesoRegistrador> accesoRegistrado = new List<AccesoRegistrador>();

            Bd Mybd = new Bd(Properties.Settings.Default.ConexionAbonado);
            DataTable dt1, dt2;

            Mybd.AgregarParametros("Id_Tarjeta", int.Parse(IdTarjeta));
            Mybd.AgregarParametros("Puerta", Properties.Settings.Default.NumeroDePuerta.ToString());

            DataSet ds = Mybd.ExecutarSPDataset("ObtenerAccesoTarjeta");
            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            estatusAcceso.Id_Estatus = int.Parse(dt2.Rows[0]["Id_Estatus"].ToString());
            estatusAcceso.Estatus = dt2.Rows[0]["Estatus"].ToString();
            estatusAcceso.UltimoIdLog = int.Parse(dt2.Rows[0]["UltimoIdLog"].ToString());

            foreach (DataRow dr in dt1.Rows)
            {
                AccesoRegistrador item = new AccesoRegistrador();
                item.FechaHora = dr["FechaHora"].ToString();
                item.Foto = dr["Foto"].ToString();
                item.Puerta = dr["Puerta"].ToString();
                accesoRegistrado.Add(item);
            }

            if (accesoRegistrado.Count == 0)
            {
                estatusAcceso.Acceso = "OK";
            }
            else
            {
                estatusAcceso.Acceso = "Ya Acceso " + accesoRegistrado.Count;
            }

            salida.EstatusAcceso = estatusAcceso;
            salida.Historico = accesoRegistrado;
            return salida;
        }

        public void GuardarImagenBd(string Id_Log, string filename)
        {
            string myPath = Configuracion.GetCarpetaFotos() + "\\" + filename.Replace(".jpg", "_resize.jpg");
            byte[] content = ReadBitmap2ByteArray(@myPath);
            StoreBlob2DataBase(content, Id_Log);
        }

        protected void StoreBlob2DataBase(byte[] content, string Id_Log)
        {
            System.Data.SqlClient.SqlConnection con = new SqlConnection
            {
                ConnectionString = Properties.Settings.Default.ConexionAbonado
            };
            con.Open();
            try
            {
                SqlCommand insert = new SqlCommand("Update LogFotos set stream = @image where Id_Log =" + Id_Log, con);
                SqlParameter imageParameter =
                insert.Parameters.Add("@image", SqlDbType.Binary);
                imageParameter.Value = content;
                imageParameter.Size = content.Length;
                insert.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        protected static byte[] ReadBitmap2ByteArray(string fileName)
        {
            using (Bitmap image = new Bitmap(fileName))
            {
                MemoryStream stream = new MemoryStream();
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }
    }
}
