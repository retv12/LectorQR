using LectorQR.Clases;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

public static class GraphicsResize
{
    
    public static void Procesar(int width, int Height, string filename)
    {
        string PathFotosControlAcceso = Configuracion.GetCarpetaFotos();
        Image image = Image.FromFile(@PathFotosControlAcceso + "\\" + filename);
        Image resizedImage = ResizeImageToWidth(image, width, Height);
        string outputFileName = Path.Combine(@PathFotosControlAcceso, Path.GetFileName(@PathFotosControlAcceso + "\\" + filename.Replace(".jpg", "_resize.jpg")));
        SaveImage(outputFileName, resizedImage);
        image.Dispose();
        resizedImage.Dispose();
    }

    public static Image ResizeImageToWidth(Image image, int width, int maxHeight)
    {
        int sourceWidth = image.Width;
        int sourceHeight = image.Height;
        int height = width * sourceHeight / sourceWidth;
        if (height > maxHeight)
        {
            width = maxHeight * sourceWidth / sourceHeight;
            height = width * sourceHeight / sourceWidth;
        }
        Bitmap resizedImage = new Bitmap(width, height, PixelFormat.Format32bppRgb);
        resizedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        Graphics grPhoto = Graphics.FromImage(resizedImage);
        grPhoto.Clear(Color.Transparent);
        grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
        grPhoto.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(0, 0, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
        grPhoto.Dispose();
        return resizedImage;
    }

    public static void SaveImage(string fileName, Image image)
    {
        ImageCodecInfo jpeg = null;
        ImageCodecInfo png = null;
        ImageCodecInfo gif = null;
        ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
        foreach (ImageCodecInfo encoder in encoders)
        {
            jpeg = encoder.FormatDescription.Equals("JPEG") ? encoder : jpeg;
            png = encoder.FormatDescription.Equals("PNG") ? encoder : png;
            gif = encoder.FormatDescription.Equals("GIF") ? encoder : gif;
        }
        EncoderParameters encoderParameters = new EncoderParameters();
        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
        if (fileName.ToUpper().EndsWith("JPG") ||
            fileName.ToUpper().EndsWith("JPEG"))
        {
            image.Save(fileName, jpeg, encoderParameters);
        }
        else if (fileName.ToUpper().EndsWith("PNG"))
        {
            image.Save(fileName, png, encoderParameters);
        }
        else if (fileName.ToUpper().EndsWith("GIF"))
        {
            image.Save(fileName, gif, encoderParameters);
        }
        else
        {
            image.Save(fileName);
        }
    }

    public static void LimpiarCarpeta()
    {
        try
        {
            string PathFotoGrande = Configuracion.GetCarpetaFotos();
            Array.ForEach(Directory.GetFiles(@PathFotoGrande), File.Delete);
        }
        catch
        {
        }
    }
}

