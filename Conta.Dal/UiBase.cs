using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using Conta.DAL;

namespace Conta.Dal {
    public interface IUiBase : INotifyPropertyChanged, IDataErrorInfo {
        bool IsLocked { get; set; }
        bool IsDirty { get; }

        bool Update();
        IDataClientService GetService();
        void RaisePropertyChanged(string propName);
    }

    public abstract class UiBase : IUiBase {
        //private IDataClientService service;
        private bool isLocked;

        protected UiBase() { }

        #region IUiBase Members
        [Browsable(false)]
        public bool IsLocked {
            get {
                return isLocked;
            }

            set {
                if (!SetProp(isLocked, value, v => this.isLocked = v, "IsLocked")) return;
                if (!value)
                    IsDirty = false;    // after update; will raise RowStatus
                else
                    RaisePropertyChanged("RowStatus");
            }
        }

        private bool isDirty;
        [Browsable(false)]
        public bool IsDirty {
            get { return isDirty; }
            private set { if (SetProp(isDirty, value, v => this.isDirty = v, "IsDirty")) RaisePropertyChanged("RowStatus"); }
        }

        /// <summary>
        /// Has to be a property to trigger changes in WPF.
        /// </summary>
        [Browsable(false)]
        public int RowStatus { get { return (isDirty ? 1 : 0) + (isLocked ? 2 : 0); } }

        public bool Update() {
            var result = GetService().Update(this);

            isDirty = false;
            isLocked = false;
            if (!result)
                RaisePropertyChanged(string.Empty);

            return result;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region IDataErrorInfo
        [Browsable(false)]
        public string Error {
            get {
                var result = new StringBuilder();
                foreach (var prop in this.GetType().GetProperties()) {
                    var attrs = prop.GetCustomAttributes(true);
                    var isBrowsable = true;
                    foreach (var att in attrs) {
                        if (att is BrowsableAttribute) {
                            if (att is BrowsableAttribute)
                                isBrowsable = (att as BrowsableAttribute).Browsable;
                            break;
                        }
                    }

                    if (isBrowsable) {
                        foreach (var att in attrs) {
                            if (att is ValidationAttribute) {
                                var validator = att as ValidationAttribute;
                                if (!validator.IsValid(prop.GetValue(this, null)))
                                    result.AppendLine(validator.FormatErrorMessage(prop.Name));
                            }
                        }
                    }
                }

                if (result.Length > 2)
                    result.Remove(result.Length - 2, 2); // remove last cr/lf
                return result.ToString();
            }
        }

        [Browsable(false)]
        public string this[string columnName] { get { return Validate(columnName); } }
        #endregion

        #endregion

        public abstract IDataClientService GetService();

        public void RaisePropertyChanged(string propName) {
#if(DEBUG)
            if (propName == "RowStatus") Debug.WriteLine("RowStatus : " + RowStatus.ToString() + " / " + isLocked + " / " + isDirty);
#endif
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        protected bool SetProp<T>(T original, T value, Action<T> setter, string propName) {
            if (object.Equals(original, value))
                return false;

            if (!IsDirty && propName != "IsLocked") {
                var canLock = GetService().Lock(this, true);
                if (!canLock)
                    return false;   // failed to acquire lock
                isDirty = true;     // do not use the property to avoid infinite recursion
                if (IsLocked == canLock)
                    RaisePropertyChanged("RowStatus");
                else
                    IsLocked = canLock;
            }
            setter(value);
            RaisePropertyChanged(propName);
            return true;
        }

        protected string Validate(string columnName) {
            var prop = this.GetType().GetProperty(columnName);
            if (prop == null) return null;  // property not found

            var attrs = prop.GetCustomAttributes(true);
            if (attrs.Length == 0) return string.Empty;

            var result = new StringBuilder();
            var value = prop.GetValue(this, null);
            foreach (var attr in attrs)
                if (attr is ValidationAttribute) {
                    var validation = attr as ValidationAttribute;
                    if (!validation.IsValid(value))
                        result.AppendLine(validation.FormatErrorMessage(columnName));
                }

            if (result.Length > 2) {
                result.Remove(result.Length - 2, 2); // remove last cr/lf
                Debug.WriteLine(string.Format("validation({0}) = [{1}]", columnName, result.ToString()));
            }

            return result.ToString();
        }
    }
}
