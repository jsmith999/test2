using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
//using Conta.DAL.ServiceHandler;

namespace Conta.DAL.Model {
    public class Client {
        //static Client() { DataService.Registered.Add(typeof(Client), new ClientServiceHandler()); }

        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
