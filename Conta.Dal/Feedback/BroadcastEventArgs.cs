using System;

namespace Conta.Dal.Feedback {
    public enum DalObjectStatus {
        Normal, // UnLocked
        Locked,
        Deleted,
        Updated,
    }

    public class BroadcastEventArgs : EventArgs {
        public BroadcastEventArgs(DalObjectStatus status,
            object dalObject) {
            if (dalObject == null)
                throw new ArgumentNullException("dalObject");

            Status = status;
            DalObject = dalObject;
        }

        public DalObjectStatus Status { get; private set; }
        public object DalObject { get; private set; }
    }
}
