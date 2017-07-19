using System;

namespace Conta.UiController.Controller {
    [AttributeUsage(AttributeTargets.Property)]
    public class LookupBoundPropertyAttribute : Attribute {
        public LookupBoundPropertyAttribute(string dataSource,
        string displayMember,
        string valueMember,
        string lookupMember) {
            DataSource = dataSource;
            DisplayMember = displayMember;
            ValueMember = valueMember;
            LookupMember = lookupMember;
        }

        public string DataSource { get; private set; }
        public string DisplayMember { get; private set; }
        public string ValueMember { get; private set; }
        public string LookupMember { get; private set; }
    }
}
