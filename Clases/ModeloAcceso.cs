using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LectorQR.Clases
{
    public class ModeloAcceso
    {
        public EstatusAcceso EstatusAcceso { get; set; }
        public List<AccesoRegistrador> Historico { get; set; }
    }

    public class EstatusAcceso
    {
        public string Estatus { get; set; }
        public int Id_Estatus { get; set; }
        public int UltimoIdLog { get; set; }
        public string Acceso { get; set; }
    }

    public class AccesoRegistrador
    {
        public string FechaHora { get; set; }
        public string Foto { get; set; }
        public string Puerta { get; set; }
    }
}
