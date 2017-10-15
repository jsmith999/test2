using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;

namespace Conta.DAL.Model {
    /* not used
    public enum ProjectRowType {
        Category,
        Material,
        // others (work/er)?
        Unknown,
    }

    public interface IUniformProjectGrid {
        // first column ???
        // 2nd
        //bool HasDetails { get; set; }

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
    /* */
}
