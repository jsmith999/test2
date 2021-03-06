﻿using Conta.Dal;
using Conta.DAL;
using Conta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conta.DAL.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Conta.UiController.Controller;

namespace Conta.Model {
    public class UiMaterial : UiBase {
        #region Service
        private static TheService service;

        public static IDataClientService Service { get { return service = (service ?? new TheService()); } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly Material original;

        public UiMaterial(Material original)
            : base() {
            this.original = original;
        }

        #region properties
        [Browsable(false)]
        public int Key { get { return original.Key; } }
        [StringLength(20)]
        public string Name {
            get { return original.Name; }
            set { SetProp(original.Name, value, x => original.Name = x, "Name"); }
        }
        public string MeasuringUnit {
            get { return original.MeasuringUnit; }
            set { SetProp(original.MeasuringUnit, value, x => original.MeasuringUnit = x, "MeasuringUnit"); }
        }
        public double UnitPrice {
            get { return original.UnitPrice; }
            set { SetProp(original.UnitPrice, value, x => original.UnitPrice = x, "UnitPrice"); }
        }
        // TODO ref to ProjectItemCategory
        [LookupBoundProperty(/*DataSource*/"UiProjectCategory", /*DisplayMember*/"Description", /*ValueMember*/"Category", /*LookupMember*/"Id")]
        public int Category {
            get { return original.Category; }
            set { SetProp(original.Category, value, x => original.Category = x, "Category"); }
        }
        #endregion

        public override string ToString() {
            return string.Format("[{0}] {1}", Category, Name);
        }
        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<Material, UiMaterial> {

            internal TheService() : base(XmlDal.DataContext.Materials, new KeyValuePair<string, Type>[] { }) { }

            protected override Material GetOriginal(UiMaterial item) { return item.original; }

            protected override UiMaterial Create(Material original) { return new UiMaterial(original); }
        }
        #endregion
    }
}
