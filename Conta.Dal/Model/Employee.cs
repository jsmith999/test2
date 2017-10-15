using Conta.Dal.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conta.DAL.Model {
    public partial class Employee : IDalData {
        public Employee() { }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DOB { get; set; }
        public float Salary { get; set; }
        public DateTime HireDate { get; set; }
    }
}
