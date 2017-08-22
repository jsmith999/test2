using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conta.DAL.Model {
    public partial class ProjectStatus {
        public ProjectStatus() { }
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
