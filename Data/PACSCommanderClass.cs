using Microsoft.AspNetCore.Mvc;
using RestApi_Dicom.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace RestApi_Dicom.Data.PACSObjectJSON
{
    public class PACSCommanderClass : ICommanderPACS
    {

        private readonly object _locker = new object();

        public string Find(LoginModel patientModel)
        {
            gdcm.KeyValuePairArrayType klucze = new gdcm.KeyValuePairArrayType();
            gdcm.KeyValuePairType klucz1 = new gdcm.KeyValuePairType(new gdcm.Tag(0x0010, 0x0010), "*");
            klucze.Add(klucz1);
            klucze.Add(new gdcm.KeyValuePairType(new gdcm.Tag(0x0010, 0x0020), "*"));
            gdcm.BaseRootQuery zapytanie = gdcm.CompositeNetworkFunctions.ConstructQuery(gdcm.ERootType.ePatientRootType, gdcm.EQueryLevel.ePatient, klucze);

            // sprawdź, czy zapytanie spełnia kryteria
            if (!zapytanie.ValidateQuery())
            {
                Console.WriteLine("Błędne zapytanie");
                return "Błędne zapytanie";
            }

            gdcm.DataSetArrayType wynik = new gdcm.DataSetArrayType();

            bool stan = gdcm.CompositeNetworkFunctions.CFind(patientModel.Ip_address,
                Convert.ToUInt16(patientModel.Port_server), 
                zapytanie, wynik, 
                patientModel.Aet_client, 
                patientModel.Aet_server);
           return stan.Handle_CFindPatient(wynik);

        }

        public  IEnumerable<SeriesModel> Series(PatientModel patientModel)
        {
            gdcm.KeyValuePairArrayType klucze = new gdcm.KeyValuePairArrayType();
            gdcm.KeyValuePairType klucz1 = new gdcm.KeyValuePairType(new gdcm.Tag(0x0010, 0x0020), patientModel.ID);

            klucze.Add(klucz1);

            gdcm.BaseRootQuery zapytanie = gdcm.CompositeNetworkFunctions.ConstructQuery(gdcm.ERootType.ePatientRootType, gdcm.EQueryLevel.ePatient, klucze, gdcm.EQueryType.eMove);

            if (!zapytanie.ValidateQuery())
            {
                Console.WriteLine("MOVE błędne zapytanie!");
                return null;
            }

            String odebrane = Path.Combine(".", "odebrane");
            if (!Directory.Exists(odebrane)) 
                Directory.CreateDirectory(odebrane); 

            String dane = Path.Combine(odebrane, Path.GetRandomFileName()); 
            Directory.CreateDirectory(dane);

            bool stan = gdcm.CompositeNetworkFunctions.CMove(patientModel.loginModel.Ip_address, Convert.ToUInt16(patientModel.loginModel.Port_server), zapytanie, Convert.ToUInt16(patientModel.loginModel.Port_client), patientModel.loginModel.Aet_client, patientModel.loginModel.Aet_server, dane);

            if (!stan)
            {
                Console.WriteLine("MOVE nie działa!");
                return null;
            }
            return stan.Handle_CMoveSeries(dane);
        }     

        public byte[] TakeImage(SeriesModel seriesModel)
        {
            lock (_locker)
            {
                FileInfo fileInfo = new FileInfo($".\\odebrane\\{seriesModel.NameFolder}\\{seriesModel.Id.Remove(seriesModel.Id.Length - 4)}.dcm_warstwa0.jpg");
                gdcm.ERootType typ = gdcm.ERootType.ePatientRootType;
                byte[] data = new byte[fileInfo.Length];
                using (FileStream fs = fileInfo.OpenRead())
                {
                    fs.Read(data, 0, data.Length);
                }
                fileInfo.Delete();
                return data;
            }
        }
    }
}
