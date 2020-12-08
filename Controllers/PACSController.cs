using gdcm;
using Microsoft.AspNetCore.Mvc;
using RestApi_Dicom.Data;
using RestApi_Dicom.Data.PACSObjectJSON;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestApi_Dicom.Controllers
{
    [Route("api/PACS")]
    [ApiController]
    public class PACSController : ControllerBase// Controller, ICommanderPACS
    {

        private ICommanderPACS _repository;
        public PACSController(ICommanderPACS _repository)
        {
            this._repository = _repository;
        }

        [HttpPost]
        [Route("test")]
        public ActionResult Test()
        {
            return Ok("Działa");
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult Login(LoginModel patientModel)
        {
            string results =  _repository.Find(patientModel);
            if (results != null && results != "Zapytanie CFind ma błędne dane." && results != "Błędne zapytanie")
                return Ok(results);
            else
                return NotFound(results);
        }

        [HttpPost]
        [Route("Series")]
        public ActionResult Find(PatientModel patient)
        {
            IEnumerable<SeriesModel> seriesModels = _repository.Series(patient);
            if (seriesModels != null)
                return Ok(JsonSerializer.Serialize(seriesModels));
            else
                return NotFound();
        }

        [HttpPost]
        [Route("Image")]
        public ActionResult Image(SeriesModel seriesModel)
        {
            byte[] data = _repository.TakeImage(seriesModel);
            if (data != null)
                return Ok(data);
            else
                return NotFound();
        }

    }
}
