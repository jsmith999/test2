using Conta.DAL.Model;
using System;
using System.Collections.Generic;

namespace XmlDal.ServiceHandler {
    class ProjectItemComparer : IComparer<IUniformProjectGrid> {
        public int Compare(IUniformProjectGrid left, IUniformProjectGrid right) {
            if (left.Parent != right.Parent)
                return Math.Sign(left.Parent - right.Parent);
            return Math.Sign(left.Order - right.Order);
        }
    }
}
