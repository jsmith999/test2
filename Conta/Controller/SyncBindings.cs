using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conta.Controller {
    class SyncBindings {
        private BindingSource bs;
        private DataGridView dgv;

        public SyncBindings(BindingSource bs, DataGridView dgv) {
            this.bs = bs;
            this.dgv = dgv;

            //dgv.SelectionChanged += new EventHandler(dgv_SelectionChanged);
        }

        void dgv_SelectionChanged(object sender, EventArgs e) {
            var index = dgv.SelectedRows.Count == 0 ? -1 : dgv.SelectedRows[0].Index;
            bs.Position = index;
        }
    }
}
