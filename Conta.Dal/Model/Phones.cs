using Conta.Dal.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conta.DAL.Model {
    public partial class Phones : IDalData {
        public Phones() { }
        [Key]
        public int Id { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public string PhoneNo { get; set; }
        public string Extension { get; set; }
        public int PhoneType { get; set; }
        //public PhoneType PhoneType { get; set; }
        public bool IsMain { get; set; }
    }
}
