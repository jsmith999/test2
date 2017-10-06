using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conta.DAL.Model {
    public partial class Attach {
        public Attach() { }
        [Key]
        public int Id { get; set; }
        [Key]
        public string RefType { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public byte[] Contents { get; set; }
    }
}