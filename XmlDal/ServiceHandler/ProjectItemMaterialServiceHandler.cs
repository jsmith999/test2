using Conta.DAL.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace XmlDal.ServiceHandler {
    public class ProjectItemMaterialServiceHandler : TableService<ProjectItemDetailMaterial, int> {
        public const string TheTableName = "ProjectItemDetail";

        private MaterialServiceHandler materialServiceHandler;

        internal ProjectItemMaterialServiceHandler()
            : base() {
            TableName = TheTableName;
            KeyName = "Key";

            materialServiceHandler = new MaterialServiceHandler();
        }

        public List<ProjectItemDetailMaterial> GetList(int project, int category) {
            var result = GetList();
            return result.Where(x => x.Project == project && x.Category == category).ToList();
        }

        protected override ProjectItemDetailMaterial DataToModel(System.Data.DataRow row) {
            var result = new ProjectItemDetailMaterial();
            DataToModel(result, row);
            return result;
        }

        protected override void DataToModel(ProjectItemDetailMaterial item, System.Data.DataRow row) {
            item.Key = (int)row[0];
            item.Project = (int)row[1];
            item.Category = (int)row[2];    // foreign key to categories
            item.MaterialKey = (int)row[3];    // foreign key to materials
            item.Quantity = (double)row[4];
            item.Observations = row[5] as string;
            item.Order = (int)row[6];

            // add materials fields
            item.Material = materialServiceHandler.GetItem(item.MaterialKey);
            if (item.Material == null) {
                Trace.WriteLine("Could not find material[" + item.MaterialKey + "]");
                return;
            }

            item.Name = item.Material.Name;
            //item.Category = m.Category;   // TODO ?
            item.MeasuringUnit = item.Material.MeasuringUnit;
            item.UnitPrice = item.Material.UnitPrice;
        }

        protected override void ModelToData(ProjectItemDetailMaterial item, System.Data.DataRow row) {
            //row[0] = item.Key;
            Debug.Assert((int)row[0] == item.Key);
            row[1] = item.Project;
            row[2] = item.Category;    // foreign key to categories
            row[3] = item.MaterialKey;    // foreign key to materials
            row[4] = item.Quantity;
            row[5] = item.Observations;
            row[6] = item.Order;

            // add materials fields
            var m = materialServiceHandler.GetItem(item.MaterialKey);
            m.Name = item.Name;
            //item.Category = m.Category;   // TODO ?
            m.MeasuringUnit = item.MeasuringUnit;
            m.UnitPrice = item.UnitPrice;
            materialServiceHandler.Update(m);
        }

        protected override System.Data.DataRow FindRow(System.Data.DataTable table, ProjectItemDetailMaterial item) {
            var results = table.Select("Key = " + item.Key);
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }

        public override int GetKeyValue(ProjectItemDetailMaterial item) { return item.Key; }
    }
    /*
    // TODO : split ProjectItemMaterialServiceHandler into ProjectItemServiceHandler & MaterialServiceHandler 
    class ProjectItemServiceHandler : TableService<IUniformProjectGrid, int> {
        ProjectItemMaterialServiceHandler materials;
        ProjectItemsCategoryServiceHandler categories;

        internal ProjectItemServiceHandler()
            : base() {
            TableName = "ProjectItem";
            KeyName = "Key";

            materials = new ProjectItemMaterialServiceHandler();
            categories = new ProjectItemsCategoryServiceHandler();
        }

        protected override IEnumerable<IUniformProjectGrid> DoGetList(string toSearch) {
            var ctgList = this.categories.GetList(toSearch);
            var matList = this.materials.GetList(toSearch);
            var result = new List<IUniformProjectGrid>();
            result.AddRange(ctgList);
            result.AddRange(matList);
            result.Sort(new ProjectItemComparer());
            return result;
        }

        protected override IUniformProjectGrid DataToModel(System.Data.DataRow row)
        {
            return row.Table.TableName == ProjectItemMaterialServiceHandler.TheTableName ? materials.DataToModel(row)
                : categories.DataToModel(row) as ProjectItem;
        }

        protected override void DataToModel(IUniformProjectGrid item, System.Data.DataRow row)
        {
            if (item is ProjectItemDetailMaterial)
                materials.DataToModel(item as ProjectItemDetailMaterial, row);
            else
                categories.DataToModel(item as ProjectItemCategory, row);
        }

        protected override void ModelToData(IUniformProjectGrid item, System.Data.DataRow row)
        {
            if (item is ProjectItemDetailMaterial)
                materials.ModelToData(item as ProjectItemDetailMaterial, row);
            else
                categories.ModelToData(item as ProjectItemCategory, row);
        }

        protected override System.Data.DataRow FindRow(System.Data.DataTable table, IUniformProjectGrid item) {
            return table.TableName == ProjectItemMaterialServiceHandler.TheTableName ?
                materials.FindRow(table, item as ProjectItemDetailMaterial) :
                categories.FindRow(table, item as ProjectItemCategory);
        }

        public override int GetKeyValue(IUniformProjectGrid item) {
            return item is ProjectItemDetailMaterial ?
                materials.GetKeyValue(item as ProjectItemDetailMaterial) :
                categories.GetKeyValue(item as ProjectItemCategory);
        }
    }
    /* */
}
