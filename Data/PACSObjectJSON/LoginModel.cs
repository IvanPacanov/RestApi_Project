using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi_Dicom.Data.PACSObjectJSON
{
    public class LoginModel
    {
        public string Ip_address { get; set; }

        public string Port_server { get; set; }

        public string Aet_server { get; set; }

        public string Port_client { get; set; }

        public string Aet_client { get; set; }
    }
}
