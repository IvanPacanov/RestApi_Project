using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi_Dicom.Data.PACSObjectJSON
{
    public class SeriesModel
    {
        public string Id { get; set; }

        public string NameFolder { get; set; }

        public LoginModel LoginModel { get; set; }

    }
}
