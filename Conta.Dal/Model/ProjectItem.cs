using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Conta.DAL.Model {
    public class ProjectItemCategory  {
        public ProjectItemCategory() { }
        [Key]
        public int Key { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        //public override int Parent { get { return this.Key; } }
        //public override int Order { get { return 0; } }

        public virtual string Observations { get; set; }
    }

    public class ProjectItemDetail  {
        // ProjectItem 
        [Key]
        public int Key { get; set; }
        [ForeignKey("Project")]
        public int Project { get; set; }
        public virtual string Name { get; set; }
        public virtual string MeasuringUnit { get; set; }
#if(DEBUG)
        private double quantity;
        public double Quantity {
            get { return quantity; }
            set {
                //Debug.WriteLine("DAL Quantity : {0} -> {1}", quantity, value);
                quantity = value;
            }
        }
#else
        public double Quantity { get; set; }
#endif
        public virtual double UnitPrice { get; set; }
        [StringLength(20)]
        public string Observations { get; set; }
        [ForeignKey("ProjectCategory")]
        public int Category { get; set; }

        [Browsable(false)]
        public int Order { get; set; }

        [Browsable(false)]
        //[ForeignKey("Material")]
        public int MaterialKey { get; set; }

        [Browsable(false)]
        public int Parent { get { return this.Category; } }
    }

    public class ProjectItemDetailMaterial : ProjectItemDetail {
        public Material Material { get; set; }
        // Material
        public override string Name { get { return Material == null ? "?" : Material.Name; } set { if (Material != null) Material.Name = value; } }
        public override string MeasuringUnit { get { return Material == null ? "?" : Material.MeasuringUnit; } set { if (Material != null) Material.MeasuringUnit = value; } }
        public override double UnitPrice { get { return Material == null ? 0d : Material.UnitPrice; } set { if (Material != null) Material.UnitPrice = value; } }
        //public override int Category { get { return Material == null ? 0 : Material.Category; } set { if (Material != null) Material.Category = value; } }
    }

    public class Material {
        [Key]
        public int Key { get; set; }
        public string Name { get; set; }
        public string MeasuringUnit { get; set; }
        public double UnitPrice { get; set; }
        public int Category { get; set; }
    }
}
