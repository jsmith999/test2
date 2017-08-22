using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conta.DAL.Model {
    partial class Address {
        public override string ToString() {
            return string.Format("{0} {1}{2}, {3} {4} {5} [{6}]",
                StreetNo, Street1,
                string.IsNullOrWhiteSpace(Street2) ? string.Empty : " " + Street2,
                City, Province, Country, PostalCode);
        }
    }
}
