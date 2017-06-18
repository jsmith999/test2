using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Conta.DAL {
    public class ParentFilter<T> {
        private PropertyInfo keyProperty;
        private object keyValue;

        public ParentFilter(object parent) { 
            if (parent != null)
                Init(parent); 
        }

        public bool Filter(T target) {
            return keyProperty == null ||
                keyValue == null ||
                object.Equals(keyValue, keyProperty.GetValue(target));
        }

        private void Init(object parent) {
            keyProperty = GetFKProperty(parent);
            //Debug.Assert(keyProperty != null, "Missing ForeignKey attribute for " + parent.GetType().Name);
            if (keyProperty == null) return;

            keyValue = GetKeyValue(parent);
            //Debug.Assert(keyValue != null, "Missing key attribute or value");
            if (keyValue == null) keyProperty = null;
        }

        private PropertyInfo GetFKProperty(object foreignKey) {
            foreach (var prop in typeof(T).GetProperties()) {
                var fkAttr = prop.GetCustomAttributes(true).FirstOrDefault(x => x is ForeignKeyAttribute) as ForeignKeyAttribute;
                if (fkAttr != null &&
                    fkAttr.Name == foreignKey.GetType().Name)
                    return prop;
            }

            return null;
        }

        private object GetKeyValue(object foreignKey) {
            foreach (var prop in foreignKey.GetType().GetProperties()) {
                var fkAttr = prop.GetCustomAttributes(true).FirstOrDefault(x => x is KeyAttribute) as KeyAttribute;
                if (fkAttr != null)
                    return prop.GetValue(foreignKey);
            }

            return null;
        }
    }
}

