using Conta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conta.UiController.Model.Reports {
    public class ProjectCategorySplit {
        public static List<ProjectCategorySplit> GetData(/*TODO : filter*/) {
            return new ProjectCategorySplit().DoGetData();
        }

        private List<ProjectCategorySplit> DoGetData(/*TODO : filter*/) {
            var result = new List<ProjectCategorySplit>();
            var projects = UiProject.Service.GetList(/*TODO : filter*/);
            foreach (UiProject prj in projects) {
                var item = new ProjectCategorySplit {
                    Name = prj.Name,
                    Budget = prj.Price,
                    Budgets = new List<CategoryBudget>(),
                };

                if (UiProjectCategory.Service == null) UiProjectCategory.InitService();
                foreach (UiProjectCategory cat in UiProjectCategory.Service.GetList()) {
                    var budget = new CategoryBudget();
                    budget.CategoryName = cat.Name;
                    foreach (var catItem in (UiProjectItemDetail.Service as UiProjectItemDetail.TheService).GetList(prj.Id, cat.Key))
                        budget.Value += catItem.Value;

                    if (budget.Value != 0d)
                        item.Budgets.Add(budget);
                }

                result.Add(item);
            }

            return result;
        }

        public string Name { get; set; }
        public double Budget { get; set; }
        public List<CategoryBudget> Budgets { get; set; }

        public class CategoryBudget {
            public string CategoryName { get; set; }
            public double Value { get; set; }
        }
    }
}
