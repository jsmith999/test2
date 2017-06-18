using System;
using System.Collections.Generic;

namespace Conta.DAL {
    public class DataService : ITableService {
        public static Dictionary<Type, ITableService> Registered = new Dictionary<Type, ITableService>();
        public static DataService Instance = new DataService();

        public static void Test() {
            //Debug.WriteLine((Instance.Update(new Dal.Employees { Id = 1, FirstName = "Adam", LastName = "ZZTop", }) as Dal.Employees).Id);
            //Debug.WriteLine(Instance.Update(new Dal.Client { Id = 1, FirstName = "Adam", LastName = "ZZTop", }).Id);
        }

        private DataService() { }

        #region ITableService
        public IEnumerable<T> GetList<T>() where T : class {
            return Registered.ContainsKey(typeof(T)) ?
                Registered[typeof(T)].GetList<T>() :
                null;
        }

        public IEnumerable<T> GetList<T>(string toSearch) where T : class {
            return Registered.ContainsKey(typeof(T)) ?
                Registered[typeof(T)].GetList<T>(toSearch) :
                null;
        }

        public T Create<T>() where T : class {
            return Registered.ContainsKey(typeof(T)) ?
                Registered[typeof(T)].Create<T>() :
                default(T);
        }

        public T Update<T>(T item) where T : class {
            return item != null && Registered.ContainsKey(typeof(T)) ?
                Registered[typeof(T)].Update(item) :
                default(T);
        }

        public T Delete<T>(T item) where T : class {
            return item != null && Registered.ContainsKey(typeof(T)) ?
                Registered[typeof(T)].Delete(item) :
                default(T);
        }

        public bool Lock<T>(T item, bool locked) where T : class {
            return item != null && Registered.ContainsKey(typeof(T)) ?
                Registered[typeof(T)].Lock(item, locked) :
                false;
        }
        #endregion
    }
}
