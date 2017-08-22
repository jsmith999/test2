using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conta.DAL.Model {
    public partial class Payment {
        public Payment() { }
        [Key]
        public int Id { get; set; }
        public int Project { get; set; }
        public DateTime Date { get; set; }
        public float Sum { get; set; }
        public string Description { get; set; }
    }
}
