using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conta.DAL.Model {
    public partial class Project {
        public Project() { }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StatusKey { get; set; }
        public Conta.DAL.Model.ProjectStatus Status { get; set; }
        public int AddressKey { get; set; }
        public Conta.DAL.Model.Address Address { get; set; }
        public string Receivable { get; set; }
        public string AgeOfReceivable { get; set; }
        public int ClientKey { get; set; }
        public Conta.DAL.Model.Client Client { get; set; }
    }
}
