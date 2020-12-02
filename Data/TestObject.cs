using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi_Dicom.Data
{
    public class TestObject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public int[] CoordinateXY { get; set; }

        [Required]
        public string Base64 { get; set; }

        public bool Status { get; set; }

        public Guid guid = Guid.NewGuid();

        public void IsFinish()
        {
            Status = true;
        }
    }
}
