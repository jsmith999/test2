using System;
using System.ComponentModel;

namespace None {
    /* DAL */
    class DalProjectItemsCategory {
        public int Key { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    class DalProjectItemDetail {
        // ProjectItem 
        public int Key { get; set; }
        public int Project { get; set; }
        public double Quantity { get; set; }
        public string Observations { get; set; }
        public int Order { get; set; }

        public int MaterialKey { get; set; }
        // Material
        public string Name { get; set; }
        public string MeasuringUnit { get; set; }
        public double UnitPrice { get; set; }
        public int Category { get; set; }
    }

    /* UI */
    class UiProjectItemsCategory {
        private readonly DalProjectItemsCategory original;
        public UiProjectItemsCategory(DalProjectItemsCategory original) { this.original = original; }

        [Browsable(false)]
        public int Key { get { return original.Key; } }
        public string Name { get { return original.Name; } }
        [Browsable(false)]
        public bool IsDeleted { get { return original.IsDeleted; } }
        // no Notification & validation : R/O
    }

    class UiProjectItemDetail {
        private readonly DalProjectItemDetail original;
        public UiProjectItemDetail(DalProjectItemDetail original) { this.original = original; }

        [Browsable(false)]
        public int Key { get { return original.Key; } }
        [Browsable(false)]
        public int Project { get; set; }
        public string Name { get; set; }
        public string MeasuringUnit { get { return original.MeasuringUnit; } }
        public double UnitPrice { get; set; }
        public double Quantity { get; set; }
        //public double Value { get { return UnitPrice * Quantity; } } 
        public string Observations { get; set; }
        [Browsable(false)]
        public int Category { get { return original.Category; } }
        public int Order { get; set; }
        [Browsable(false)]
        public int MaterialKey { get { return original.MaterialKey; } }
    }

    enum ProjectRowType {
        Category,
        Material,
        // others (work/er)?
    }

    interface IUniformProjectGrid {
        // first column ???
        // 2nd
        bool HasDetails { get; set; }

        // 3
        string Name { get; }

        string MeasuringUnit { get; }
        double Quantity { get; set; }
        double UnitPrice { get; set; }
        //double Value { get; }  // total, for category
        string Observations { get; set; }

        // ordering properties
        [Browsable(false)]
        int Parent { get; }
        [Browsable(false)]
        int Order { get; set; }
    }

    class ProjectItemsCategoryRow : UiProjectItemsCategory, IUniformProjectGrid {
        public ProjectItemsCategoryRow(DalProjectItemsCategory original)
            : base(original) {
            HasDetails = true;      // start with details (TODO : move to config/settings)
        }

        public bool HasDetails { get; set; }    // TODO : visible = true

        //string Name { get; }  // inherited

        public string MeasuringUnit { get { return string.Empty; } }    // TODO : visible = false
        public double Quantity { get; set; }    // TODO : visible = false
        public double UnitPrice { get; set; }   // TODO : visible = false
        public double Value { get { return 0d; /*TODO:calculate*/ } }  // total
        public string Observations { get; set; }

        [Browsable(false)]
        public int Parent { get { return base.Key; } }
        [Browsable(false)]
        public int Order {
            get { return 0; }
            set { /* ignore */ }
        }
    }

    class ProjectItemDetailCategoryRow : UiProjectItemDetail, IUniformProjectGrid {
        public ProjectItemDetailCategoryRow(DalProjectItemDetail original) : base(original) { }

        public bool HasDetails { get; set; }    // TODO : visible = false

        // inherited ->
        //string Name { get; }

        //string MeasuringUnit { get; }
        //double Quantity { get; set; }
        //double UnitPrice { get; set; }
        public double Value { get { return UnitPrice * Quantity; } }

        [Browsable(false)]
        public int Parent { get { return this.Category; } }
        //int Order { get; set; }   // inherited 
    }
}
