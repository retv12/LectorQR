using System.IO;

namespace LectorQR.Clases
{
    public class Configuracion
    {
        public static string GetNombreMaquina()
        {
            return System.Environment.MachineName;
        }

        public static string GetPuerta()
        {
            return Properties.Settings.Default.NumeroDePuerta.ToString();
        }

        public static string GetTemporada()
        {
            return Properties.Settings.Default.TemporadaNumero.ToString();
        }

        public static string GetCarpetaFotos()
        {
            string ruta = @"c:\FotosControlAcceso";
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
            return ruta;
        }

        public string GetCarpetaNoAutorizados()
        {
            string ruta = @"c:\NoAutorizados";
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
            return ruta;
        }
    }
}
