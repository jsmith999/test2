using Conta.Dal.Feedback;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace XmlDal.ServiceHandler
{
    class ProjectItemServiceHandler : ITableService<IUniformProjectGrid>
    {
        ProjectItemMaterialServiceHandler materials;
        ProjectItemsCategoryServiceHandler categories;

        internal ProjectItemServiceHandler()
            : base()
        {
            //TableName = "ProjectItem";
            //KeyName = "Key";
            Broadcaster = new BroadcastService();

            materials = new ProjectItemMaterialServiceHandler();
            categories = new ProjectItemsCategoryServiceHandler();
        }

        #region ITableService
        public IEnumerable<IUniformProjectGrid> GetList(LambdaExpression where = null, string toSearch = null)
        {
            // TODO : use where & search (?)
            var ctgList = this.categories.GetList().Where(x => !x.IsDeleted);
            var matList = this.materials.GetList();
            var result = new List<IUniformProjectGrid>();
            result.AddRange(ctgList);
            result.AddRange(matList);
            result.Sort(new ProjectItemComparer());
            return result;
        }

        public IEnumerable<IUniformProjectGrid> GetList(object parent) {
            var filter = new ParentFilter<IUniformProjectGrid>(parent);
            var unfiltered = GetList(null, null) as IEnumerable<IUniformProjectGrid>;
            var result = unfiltered.Where(x => filter.Filter(x));   // TODO : move to DAL
            return result;
        }

        public IUniformProjectGrid Create()
        {
            throw new NotImplementedException();
        }

        public bool Update(IUniformProjectGrid item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item is ProjectItemCategory)
                return categories.Update(item as ProjectItemCategory);

            if (item is ProjectItemDetail)
                return materials.Update(item as ProjectItemDetailMaterial);

            throw new NotImplementedException("Update(" + item.GetType().Name + ")");
        }

        public IUniformProjectGrid Delete(IUniformProjectGrid item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item is ProjectItemCategory)
                return categories.Delete(item as ProjectItemCategory);

            if (item is ProjectItemDetail)
                return materials.Delete(item as ProjectItemDetailMaterial);

            throw new NotImplementedException("Delete(" + item.GetType().Name + ")");
        }

        public bool Lock(IUniformProjectGrid item, bool locked)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item is ProjectItemCategory)
                return categories.Lock(item as ProjectItemCategory, locked);

            if (item is ProjectItemDetail)
                return materials.Lock(item as ProjectItemDetailMaterial, locked);

            throw new NotImplementedException("Lock(" + item.GetType().Name + ")");
        }

        public bool AreEqual(IUniformProjectGrid left, IUniformProjectGrid right)
        {
            return left.Parent == right.Parent &&
                left.Order == right.Order;
        }

        public Conta.Dal.Feedback.BroadcastService Broadcaster { get; private set; }
        #endregion
    }
}
