using Conta.Dal.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conta.DAL.Model {
    public partial class Address : IDalData {
        public Address() { }
        [Key]
        public int Id { get; set; }
        public string StreetNo { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Description { get; set; }
    }
}
