using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conta.DAL.Model {
    public partial class Client {
        public Client() { }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        private int AddressIdKey { get; set; }
        public Conta.DAL.Model.Address Address { get; set; }
        public int AddressId { get; set; }
        public string Email { get; set; }
    }
}
