using gdcm;
using Microsoft.AspNetCore.Mvc;
using RestApi_Dicom.Data;
using RestApi_Dicom.Data.PACSObjectJSON;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestApi_Dicom.Controllers
{
    [Route("api/PACS")]
    [ApiController]
    public class PACSController : Controller, ICommanderPACS
    {
        public PACSController()
        {

        }
        [HttpGet]
        [Route("tt")]
        public ActionResult Test()
        {   
                return Ok("Cześć Eniu");
        }
        [HttpGet]
        [Route("all")]
        public ActionResult Find()
        {
            bool stan = false;
            gdcm.DataSetArrayType wynik = new gdcm.DataSetArrayType();
            try
            {
           //     stan = gdcm.CompositeNetworkFunctions.CFind("127.0.0.1", 10100, null, wynik, "KLIENT", "ARCHIWUM");
            }
            catch
            {
            }
            if (stan)
                return Ok(wynik);
            else
                return NotFound("Sorry");

        }

        public static gdcm.Bitmap pxmap2jpeg2000(gdcm.Pixmap px)
        {
            gdcm.ImageChangeTransferSyntax change = new gdcm.ImageChangeTransferSyntax();
            change.SetForce(false);
            change.SetCompressIconImage(false);
            change.SetTransferSyntax(new gdcm.TransferSyntax(gdcm.TransferSyntax.TSType.JPEG2000Lossless));

            change.SetInput(px);
            if (!change.Change())
                throw new Exception("Nie przekonwertowano typu bitmapy na jpeg2000");

            gdcm.Bitmap outimg = change.GetOutputAsBitmap(); // dla GDCM.3.0.4

            return outimg; //change.GetOutput(); // tak było w starszych wersjach
        }


        [HttpPost]
        [Route("move")]
        public ActionResult Find(PatientModel patient)
        {
            // typ wyszukiwania (rozpoczynamy od pacjenta)
            gdcm.ERootType typ = gdcm.ERootType.ePatientRootType;

            // do jakiego poziomu wyszukujemy 
            gdcm.EQueryLevel poziom = gdcm.EQueryLevel.ePatient; // zobacz inne 

            // klucze (filtrowanie lub określenie, które dane są potrzebne)
            gdcm.KeyValuePairArrayType klucze = new gdcm.KeyValuePairArrayType();
            gdcm.KeyValuePairType klucz1 = new gdcm.KeyValuePairType(new gdcm.Tag(0x0010, 0x0020), patient.ID);
            // NIE WOLNO TU STOSOWAC *; w tym przykladzie patientID="01" ale docelowo to musi być przekazane jako arg. metody, 
            // patrz tez uwagi w Find() dot. wyboru badania/serii
            klucze.Add(klucz1);

            // skonstruuj zapytanie
            gdcm.BaseRootQuery zapytanie = gdcm.CompositeNetworkFunctions.ConstructQuery(typ, poziom, klucze, gdcm.EQueryType.eMove);

            // sprawdź, czy zapytanie spełnia kryteria
            if (!zapytanie.ValidateQuery())
            {
                Console.WriteLine("MOVE błędne zapytanie!");
               // return;
            }

            // przygotuj katalog na wyniki
            String odebrane = System.IO.Path.Combine(".", "odebrane"); // podkatalog odebrane w bieżącym katalogu
            if (!System.IO.Directory.Exists(odebrane)) // jeśli nie istnieje
                System.IO.Directory.CreateDirectory(odebrane); // utwórz go

            String dane = System.IO.Path.Combine(odebrane, System.IO.Path.GetRandomFileName()); // wygeneruj losową nazwę podkatalogu
            System.IO.Directory.CreateDirectory(dane); // i go utwórz

            // wykonaj zapytanie - pobierz do katalogu jak w zmiennej 'dane'
            bool stan = gdcm.CompositeNetworkFunctions.CMove(ipPACS, portPACS, zapytanie, portMove, myAET, callAET, dane);

            // sprawdź stan
            if (!stan)
            {
                Console.WriteLine("MOVE nie działa!");
             //   return;
            }

            Console.WriteLine("MOVE działa.");

            List<string> pliki = new List<string>(System.IO.Directory.EnumerateFiles(dane));
            foreach (String plik in pliki)
            {
                Console.WriteLine("pobrano: {0}", plik);

                // MOVE + konwersja
                // przeczytaj pixele
                gdcm.PixmapReader reader = new gdcm.PixmapReader();
                reader.SetFileName(plik);
                if (!reader.Read())
                {
                    // najpewniej nie jest to obraz
                    Console.WriteLine("pomijam: {0}", plik);
                    continue;
                }

                // przekonwertuj na "znany format"
                gdcm.Bitmap bmjpeg2000 = pxmap2jpeg2000(reader.GetPixmap());
                // przekonwertuj na .NET bitmapę
                System.Drawing.Bitmap[] X = gdcmBitmap2Bitmap(bmjpeg2000);
                // zapisz
                for (int i = 0; i < X.Length; i++)
                {
                    String name = String.Format("{0}_warstwa{1}.jpg", plik, i);
                    X[i].Save(name);
                    Console.WriteLine("konwersja do: {0}", name);
                }
            }



            if (stan)
                return Ok(true);
            else
                return NotFound();

        }
        static string myAET = "KLIENTL";       // moj AET - ustaw zgodnie z konfiguracją serwera PACS
        static string callAET = "ARCHIWUM";    // AET serwera - j.w.
        static string ipPACS = "127.0.0.1";    // IP serwera - j.w.
        static ushort portPACS = 10100;        // port serwera - j.w.
        static ushort portMove = 10104;        // port zwrotny dla MOVE - j.w.


        [HttpPost]
        [Route("store")]
        public ActionResult Store(TestObject store)
        {
            //FIND
            // najpewniej bedzie potrzeba kilka przeciazanych wersji aby pozyskac:
            // - liste pacjentow
            // - liste badan wybranego pacjenta
            // - liste serii wybranego badania wybranego pacjenta
            // ... i taka wybrana serie (SeriesUID) dopiero pobieramy z serwera

            // typ wyszukiwania (rozpoczynamy od pacjenta)
            gdcm.ERootType typ = gdcm.ERootType.ePatientRootType;

            // do jakiego poziomu wyszukujemy 
            gdcm.EQueryLevel poziom = gdcm.EQueryLevel.ePatient; // zobacz tez inne 

            // klucze (filtrowanie lub określenie, które dane są potrzebne)
            gdcm.KeyValuePairArrayType klucze = new gdcm.KeyValuePairArrayType();

            gdcm.Tag tag = new gdcm.Tag(0x0010, 0x0010); // 10,10 == PATIENT_NAME
            gdcm.KeyValuePairType klucz1 = new gdcm.KeyValuePairType(tag, "*"); // * == dowolne imię
            klucze.Add(klucz1);
            klucze.Add(new gdcm.KeyValuePairType(new gdcm.Tag(0x0010, 0x0040), "*"));
            // zwrotnie oczekujemy wypełnionego 10,20 czyli PATIENT_ID

            // skonstruuj zapytanie
            gdcm.BaseRootQuery zapytanie = gdcm.CompositeNetworkFunctions.ConstructQuery(typ, poziom, klucze);

            // sprawdź, czy zapytanie spełnia kryteria
            if (!zapytanie.ValidateQuery())
            {
                Console.WriteLine("FIND błędne zapytanie!");
             //   return;
            }

            // kontener na wyniki
            gdcm.DataSetArrayType wynik = new gdcm.DataSetArrayType();

            // wykonaj zapytanie
            bool stan = gdcm.CompositeNetworkFunctions.CFind(ipPACS, portPACS, zapytanie, wynik, myAET, callAET);

            // sprawdź stan
            if (!stan)
            {
                Console.WriteLine("FIND nie działa!");
               // return;
            }

            Console.WriteLine("FIND działa.");

            // pokaż wyniki
            foreach (gdcm.DataSet x in wynik)
            {
                Console.Write(x.toString()); // cała odpowiedź jako wielolinijkowy napis
                // UWAGA: toString() vs ToString() !!!

                // + DOSTEP DO METADANYCH
                //for (var iter = x.Begin(); iter != x.End(); ++iter) { } // brak wrapowania iteratorów...

                // jeden element pary klucz-wartość
                gdcm.DataElement de = x.GetDataElement(new gdcm.Tag(0x0010, 0x0020)); // konkretnie 10,20 = PATIENT_ID

                // dostęp jako string
                gdcm.Value val = de.GetValue(); // pobierz wartość dla wskazanego klucza...
                string str = val.toString();    // ...jako napis, UWAGA! MAŁE "T": _t_oString()
                Console.WriteLine("ID Pacjenta: {0}", str);

                // dostęp jako tablica bajtów
                gdcm.ByteValue bval = de.GetByteValue(); // pobierz jako daną binarną
                byte[] buff = new byte[bval.GetLength().GetValueLength()]; // przygotuj tablicę bajtów
                bval.GetBuffer(buff, (uint)buff.Length); // skopiuj zawartość
                // a co z tym dalej zrobić to już inna kwestia...

                Console.WriteLine();
            }







            return Ok(true);
        }

        public static System.Drawing.Bitmap[] gdcmBitmap2Bitmap(gdcm.Bitmap bmjpeg2000)
        {
            // przekonwertuj teraz na bitmapę C#
            uint cols = bmjpeg2000.GetDimension(0);
            uint rows = bmjpeg2000.GetDimension(1);
            uint layers = bmjpeg2000.GetDimension(2);

            // wartość zwracana - tyle obrazków, ile warstw
            System.Drawing.Bitmap[] ret = new System.Drawing.Bitmap[layers];

            // bufor
            byte[] bufor = new byte[bmjpeg2000.GetBufferLength()];
            if (!bmjpeg2000.GetBuffer(bufor))
                throw new Exception("błąd pobrania bufora");

            // w strumieniu na każdy piksel 2 bajty; tutaj LittleEndian (mnie znaczący bajt wcześniej)
            for (uint l = 0; l < layers; l++)
            {
                System.Drawing.Bitmap X = new System.Drawing.Bitmap((int)cols, (int)rows);
                double[,] Y = new double[cols, rows];
                double m = 0;

                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        // współrzędne w strumieniu
                        int j = ((int)(l * rows * cols) + (int)(r * cols) + (int)c) * 2;
                        Y[r, c] = (double)bufor[j + 1] * 256 + (double)bufor[j];
                        // przeskalujemy potem do wartości max.
                        if (Y[r, c] > m)
                            m = Y[r, c];
                    }

                // wolniejsza metoda tworzenia bitmapy
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        int f = (int)(255 * (Y[r, c] / m));
                        X.SetPixel(c, r, Color.FromArgb(f, f, f));
                    }
                // kolejna bitmapa
                ret[l] = X;
            }
            return ret;
        }

    }
}
