namespace LectorQR.Clases
{
    public class ParceoTexto
    {
        public static ModeloAbonado RetornaAbonado(string str)
        {
            ModeloAbonado Abonado = new ModeloAbonado();
            string[] Temporal = str.Split('|');
            if (Temporal.Length == 7)
            {
                Abonado.Temporada = Temporal[0];
                Abonado.Zona  = Temporal[1];
                Abonado.Fila = Temporal[2];
                Abonado.Silla = Temporal[3];
                Abonado.IdAbonado = Temporal[4];
                Abonado.Nombre = Temporal[5];
                Abonado.IdTarjeta = Temporal[6];
            }
            return Abonado;
        }
    }
}
