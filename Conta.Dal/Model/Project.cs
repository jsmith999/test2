using Conta.Dal.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conta.DAL.Model {
    public partial class Project : IDalData {
        public Project() { }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [ForeignKey("Status")]
        public int StatusKey { get; set; }
        public Conta.DAL.Model.ProjectStatus Status { get; set; }

        [ForeignKey("Address")]  // TODO : generate
        public int AddressKey { get; set; }
        public Conta.DAL.Model.Address Address { get; set; }

        public string Receivable { get; set; }
        public string AgeOfReceivable { get; set; }
        
        [ForeignKey("Client")]  // TODO : generate
        public int ClientId { get; set; }
        //public Conta.DAL.Model.Client Client { get; set; }
    }
}
