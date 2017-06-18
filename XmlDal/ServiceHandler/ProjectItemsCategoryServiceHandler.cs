using Conta.DAL.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace XmlDal.ServiceHandler {
    class ProjectItemsCategoryServiceHandler : TableService<ProjectItemCategory, int> {
        public ProjectItemsCategoryServiceHandler() {
            TableName = "ProjectItemCategory";
            KeyName = "Key";
        }

        #region TableService
        //public override IEnumerable<ProjectItemCategory> GetList(object parent) {
        //    var result = base.GetList(parent);
            
        //    return result;
        //}

        public override IEnumerable<ProjectItemCategory> GetList(LambdaExpression where = null, string filter = null) {
            return base.GetList(where, filter).Where(x => !x.IsDeleted);
        }

        protected override ProjectItemCategory DataToModel(System.Data.DataRow row) {
            var result = new ProjectItemCategory();
            DataToModel(result, row);
            return result;
        }

        protected override void DataToModel(ProjectItemCategory item, System.Data.DataRow row) {
            item.Key = (int)row[0];
            item.Name = row[1] as string;
            item.IsDeleted = (bool)row[2];
        }

        protected override void ModelToData(ProjectItemCategory item, System.Data.DataRow row) {
            //row[0] = item.Key;
            Debug.Assert((int)row[0] == item.Key);
            row[1] = item.Name;
            row[2] = item.IsDeleted;
        }

        protected override System.Data.DataRow FindRow(System.Data.DataTable table, ProjectItemCategory item) {
            var results = table.Select("Key = " + item.Key);
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }

        public override int GetKeyValue(ProjectItemCategory item) {
            return item.Key;
        }
        #endregion
    }
}
