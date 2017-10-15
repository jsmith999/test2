using Conta.Dal.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conta.DAL.Model {
    public partial class Client : IDalData {
        public Client() { }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        [ForeignKey("Address")]
        //private int AddressIdKey { get; set; }
        public int AddressId { get; set; }
        public Conta.DAL.Model.Address Address { get; set; }

        public string Email { get; set; }
    }
}
