using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using LectorQR.Clases;
using ZXing;

namespace LectorQR
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;
        public string key = "mYq3t6w9z$C&E)H@McQfTjWnZr4u7x!A";
        public string ModelCamara = "";
        public string IdTarjeta = "";
        public string IdLog = "";
        private System.Windows.Forms.Timer TimerAcceso;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Closing += MyClosedHandler;

            TimerFoto.Interval = 1000 * 1;



            GraphicsResize.LimpiarCarpeta();
            IniCamara();
            CleanTextBox();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (!ModelCamara.Equals(""))
            {
                FinalFrame = new VideoCaptureDevice(CaptureDevice[0].MonikerString);
                FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
                FinalFrame.Start();
                timer1.Start();
                TimerFoto.Stop();
            }
        }

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            eventArgs.Frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Timer1_click(object sender, EventArgs e)
        {
            CleanTextBox();
            if (pictureBox1.Image != null)
            {
                BarcodeReader Reader = new BarcodeReader();
                Result result = Reader.Decode((Bitmap)pictureBox1.Image);
                if (result != null)
                {
                    string decoded = result.ToString().Trim();
                    if (decoded != "")
                    {
                        string decryptedString = AesOperation.DecryptString(key, decoded);
                        ModeloAbonado Abonado = ParceoTexto.RetornaAbonado(decryptedString);
                        TxtAbonado.Text = Abonado.Nombre;
                        TxtZona.Text = Abonado.Zona;
                        TxtFila.Text = Abonado.Fila;
                        TxtSilla.Text = Abonado.Silla;
                        TxtTemporada.Text = Abonado.Temporada;
                        if (!Abonado.Temporada.Equals(Configuracion.GetTemporada()))
                        {
                            return;
                        }
                        CargaConfig(Abonado.IdAbonado);
                        ChequeaAcceso(Abonado.IdTarjeta);
                        IdTarjeta = Abonado.IdTarjeta.ToString();
                        timer1.Stop();
                        TimerFoto.Start();
                    }
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
        }

        private void CargaConfig(string IdTarjeta)
        {
            toolStripStatusLabel1.Text = "Maquina: " + Configuracion.GetNombreMaquina() + ", Puerta: " + Configuracion.GetPuerta() + ", Temporada: " + Configuracion.GetTemporada() + ", Camara: " + ModelCamara + " " + IdTarjeta;
        }

        private void CleanTextBox()
        {
            foreach (var c in this.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Text = String.Empty;
                }
            }
            TxtEstatus.Visible = false;
            CargaConfig("");
            IdTarjeta = "";
            IdLog = "";
        }

        public void IniCamara()
        {
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
            {
                ModelCamara = Device.Name;
            }
            FinalFrame = new VideoCaptureDevice();
        }

        public void ChequeaAcceso(string IdTarjeta)
        {
            DBConexion dBConexion = new DBConexion();
            ModeloAcceso modeloAcceso = dBConexion.ObtenerIdtarjeta(IdTarjeta);
            TxtEstatus.Visible = true;
            TxtEstatus.Text = modeloAcceso.EstatusAcceso.Acceso;
            IdLog = modeloAcceso.EstatusAcceso.UltimoIdLog.ToString();
            if (!modeloAcceso.EstatusAcceso.Acceso.Equals("OK")){
                TxtEstatus.BackColor = Color.Red;
            }
            else
            {
                TxtEstatus.BackColor = Color.LimeGreen;
            }
        }

        private void TimerFoto_Tick(object sender, EventArgs e)
        {
            TimerFoto.Stop();
            GraphicsResize.LimpiarCarpeta();
            string ruta = Configuracion.GetCarpetaFotos() + "\\"+ IdTarjeta + ".jpg";
            pictureBox1.Image.Save(ruta, ImageFormat.Jpeg);
            GraphicsResize.Procesar(426, 240, IdTarjeta + ".jpg");
            TxtEstatus.Visible = true;
            TxtEstatus.Text = "Foto Capturada";
            TxtEstatus.BackColor = Color.Yellow;
            DBConexion dBConexion = new DBConexion();
            dBConexion.GuardarImagenBd(IdLog, IdTarjeta + ".jpg");
            //Aqui debo de chequear si dio error
            if (!TxtEstatus.Text.Equals("OK"))
            {
                TimerAcceso = new Timer();
                TimerAcceso.Tick += new EventHandler(TimerAcceso_Tick);
                TimerAcceso.Interval = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                TimerAcceso.Enabled = true;
                TimerAcceso.Start();
            }
            else
            {
                timer1.Start();
            }
        }
        protected void MyClosedHandler(object sender, EventArgs e)
        {
            if (FinalFrame.IsRunning == true)
            {
                FinalFrame.Stop();
            }
            Application.Exit();
        }

        private void TimerAcceso_Tick(object sender, EventArgs e)
        {
            TimerAcceso.Stop();
            //CleanTextBox();
            timer1.Start();
        }
    }
}
