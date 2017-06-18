using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conta.DAL.Model {
    public class Project {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Budget { get; set; }
        [ForeignKey("Client")]
        public int ClientKey { get; set; }
    }
}
