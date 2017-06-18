using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlDal.ServiceHandler
{
    class MaterialServiceHandler : TableService<Material, int>
    {
        private IEnumerable<Material> cache; 
        internal MaterialServiceHandler()
        {
            TableName = "Material";
            KeyName = "Key";

            cache = GetList();
        }

        public Material GetItem(int key) { return cache.FirstOrDefault(x => x.Key == key); }

        protected override Material DataToModel(System.Data.DataRow row)
        {
            var result = new Material();
            DataToModel(result, row);
            return result;
        }

        protected override void DataToModel(Material item, System.Data.DataRow row)
        {
            item.Key = (int)row[0];
            item.Name = row[1] as string;
            item.MeasuringUnit = row[2] as string;
            item.UnitPrice = (double)row[3];
            item.Category = (int)row[4];
        }

        protected override void ModelToData(Material item, System.Data.DataRow row)
        {
            //row[0] = item.Key;
            Debug.Assert((int)row[0] == item.Key);
            row[1] = item.Name;
            row[2] = item.MeasuringUnit;
            row[3] = item.UnitPrice;
            row[4] = item.Category;
        }

        protected override System.Data.DataRow FindRow(System.Data.DataTable table, Material item)
        {
            var results = table.Select("Key = " + item.Key);
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }

        public override int GetKeyValue(Material item) { return item.Key; }
    }
}
