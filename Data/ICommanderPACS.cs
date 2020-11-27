using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdcm;
using Microsoft.AspNetCore.Mvc;

namespace RestApi_Dicom.Data
{
    public interface ICommanderPACS
    {
        ActionResult Store(string store);
        ActionResult Find(BaseRootQuery baseRootQuery);

    }
}
