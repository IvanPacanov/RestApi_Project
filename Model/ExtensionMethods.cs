using RestApi_Dicom.Data.PACSObjectJSON;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestApi_Dicom.Model
{
    public static class ExtensionMethods
    {
        public static Bitmap[] GdcmBitmap2Bitmap(this gdcm.Bitmap bmjpeg2000)
        {
            uint cols = bmjpeg2000.GetDimension(0);
            uint rows = bmjpeg2000.GetDimension(1);
            uint layers = bmjpeg2000.GetDimension(2);

            Bitmap[] ret = new Bitmap[layers];

            // bufor
            byte[] bufor = new byte[bmjpeg2000.GetBufferLength()];
            if (!bmjpeg2000.GetBuffer(bufor))
                throw new Exception("błąd pobrania bufora");

            for (uint l = 0; l < layers; l++)
            {
                System.Drawing.Bitmap X = new System.Drawing.Bitmap((int)cols, (int)rows);
                double[,] Y = new double[cols, rows];
                double m = 0;

                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        int j = ((int)(l * rows * cols) + (int)(r * cols) + (int)c) * 2;
                        Y[r, c] = (double)bufor[j + 1] * 256 + (double)bufor[j];
                        if (Y[r, c] > m)
                            m = Y[r, c];
                    }
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        int f = (int)(255 * (Y[r, c] / m));
                        X.SetPixel(c, r, Color.FromArgb(f, f, f));
                    }
                ret[l] = X;
            }
            return ret;
        }

        public static gdcm.Bitmap Pxmap2jpeg2000(this gdcm.Pixmap px)
        {
            gdcm.ImageChangeTransferSyntax change = new gdcm.ImageChangeTransferSyntax();
            change.SetForce(false);
            change.SetCompressIconImage(false);
            change.SetTransferSyntax(new gdcm.TransferSyntax(gdcm.TransferSyntax.TSType.JPEG2000Lossless));

            change.SetInput(px);
            if (!change.Change())
                throw new Exception("Nie przekonwertowano typu bitmapy na jpeg2000");

            gdcm.Bitmap outimg = change.GetOutputAsBitmap();

            return outimg;
        }

        public static string Handle_CFindPatient(this bool stan, gdcm.DataSetArrayType wynik)
        {
            if (!stan)
            {
                Console.WriteLine("Zapytanie CFind ma błędne dane.");
                return "Zapytanie CFind ma błędne dane.";
            }
            List<PatientModel> list = new List<PatientModel>();
            string send = "";

            if (wynik != null)
                Console.WriteLine("Załadowano listę pacjentów.");
            foreach (gdcm.DataSet x in wynik)
            {
                gdcm.DataElement de = x.GetDataElement(new gdcm.Tag(0x0010, 0x0020));
                gdcm.Value val = de.GetValue();
                string str = val.toString();
                list.Add(new PatientModel() { ID = str });
            }
            Console.WriteLine("Przygotowano do wysłania listę pacjentów.");
            send = JsonSerializer.Serialize(list);
            return send;
        }

        public static List<SeriesModel> Handle_CMoveSeries(this bool stan, String dane)
        {
            Console.WriteLine("MOVE działa.");
            List<SeriesModel> toSend = new List<SeriesModel>();
            List<string> pliki = new List<string>(System.IO.Directory.EnumerateFiles(dane));
            foreach (String plik in pliki)
            {
                Console.WriteLine("pobrano: {0}", plik);
                string[] help = plik.Split('\\');
                toSend.Add(new SeriesModel() { Id = help[3], NameFolder = help[2] });

                gdcm.PixmapReader reader = new gdcm.PixmapReader();
                reader.SetFileName(plik);

                if (!reader.Read())
                {
                    Console.WriteLine("pomijam: {0}", plik);
                    continue;
                }

                gdcm.Bitmap bmjpeg2000 = reader.GetPixmap().Pxmap2jpeg2000();
                Bitmap[] X = bmjpeg2000.GdcmBitmap2Bitmap();
                for (int i = 0; i < X.Length; i++)
                {
                    String name = String.Format("{0}_warstwa{1}.jpg", plik, i);
                    X[i].Save(name);
                    Console.WriteLine("konwersja do: {0}", name);
                }
            }
            return toSend;
        }



        }
}
