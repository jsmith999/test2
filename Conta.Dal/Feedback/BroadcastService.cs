using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conta.Dal.Feedback {
    public class BroadcastService {
        public BroadcastService() { }

        public event EventHandler<BroadcastEventArgs> StatusChange;

        internal void RaiseStatusChange(DalObjectStatus status,
            object dalObject) {
            if (StatusChange != null)
                StatusChange(this, new BroadcastEventArgs(status, dalObject));
        }
    }
}
