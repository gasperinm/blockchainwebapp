using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainWebApp.Models
{
    public class CarData
    {
        [Required]
        public string Vin { get; set; }

        [Required]
        public string Owners { get; set; } //number of owners

        [Required]
        public string Date { get; set; }  //date of first registration

        [Required]
        public string Registration { get; set; }

        [Required]
        public string License { get; set; }

        [Required]
        public string VehicleName { get; set; }
    }
}
