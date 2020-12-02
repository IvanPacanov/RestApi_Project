using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdcm;
using Microsoft.AspNetCore.Mvc;
using RestApi_Dicom.Data.PACSObjectJSON;

namespace RestApi_Dicom.Data
{
    public interface ICommanderPACS
    {
        ActionResult Store(TestObject store);
        ActionResult Find(PatientModel patientModel);

    }
}
