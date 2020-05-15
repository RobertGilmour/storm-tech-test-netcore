using Microsoft.AspNetCore.Identity;
using Todo.Data.Entities;
using Todo.EntityModelMappers.TodoLists;
using Todo.Models.TodoLists;
using Xunit;

namespace Todo.Tests.WhenTodoListIsConvertedToDetailsViewModel
{
    public class AndHideCompletedItemsFalse
    {
        private readonly TodoListDetailViewmodel resultViewModel;

        public AndHideCompletedItemsFalse()
        {
            var user = new IdentityUser("alice@example.com");
            user.Email = user.UserName;
            var todoList = new TestTodoListBuilder(user, "shopping list")
                .WithItem("Completed Item", Importance.Medium, completed: true)
                .WithItem("Uncompleted Item", Importance.Medium)
                .Build();

            resultViewModel = TodoListDetailViewmodelFactory.Create(todoList, false, TodoListSortFields.Default);
        }

        [Fact]
        public void ContainsCompletedItem()
        {
            Assert.Contains(resultViewModel.Items, item => item.Title == "Completed Item");
        }

        [Fact]
        public void ContainsUncompletedItem()
        {
            Assert.Contains(resultViewModel.Items, item => item.Title == "Uncompleted Item");
        }
    }
}
