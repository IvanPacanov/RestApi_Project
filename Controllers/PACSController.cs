﻿using gdcm;
using Microsoft.AspNetCore.Mvc;
using RestApi_Dicom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        [Route("find")]
        public ActionResult Find(BaseRootQuery zapytanie)
        {
            gdcm.DataSetArrayType wynik = new gdcm.DataSetArrayType();
            
            bool stan = gdcm.CompositeNetworkFunctions.CFind("127.0.0.1", 10100, zapytanie, wynik, "KLIENT", "ARCHIWUM");
            if (stan)
                return Ok(wynik);
            else
                return NotFound();
        
        }

        [HttpGet]
        [Route("store")]
        public ActionResult Store(string store)
        {
            return Ok(true);
        }
    }
}