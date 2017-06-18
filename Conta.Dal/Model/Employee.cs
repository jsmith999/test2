using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
//using Conta.DAL.ServiceHandler;

namespace Conta.DAL.Model {
    public class Employee {
        //static Employee() { DataService.Registered.Add(typeof(Employee), new EmployeeServiceHandler()); }

        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public int Position { get; set; }
    }
}
