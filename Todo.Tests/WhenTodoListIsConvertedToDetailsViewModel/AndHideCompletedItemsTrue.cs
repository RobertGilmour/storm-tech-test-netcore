using Microsoft.AspNetCore.Identity;
using Todo.Data.Entities;
using Todo.EntityModelMappers.TodoLists;
using Todo.Models.TodoLists;
using Xunit;

namespace Todo.Tests.WhenTodoListIsConvertedToDetailsViewModel
{
    public class AndHideCompletedItemsTrue
    {
        private readonly TodoListDetailViewmodel resultViewModel;

        public AndHideCompletedItemsTrue()
        {
            var user = new IdentityUser("alice@example.com");
            user.Email = user.UserName;
            var todoList = new TestTodoListBuilder(user, "shopping list")
                .WithItem("Completed Item", Importance.Medium, completed: true)
                .WithItem("Uncompleted Item", Importance.Medium)
                .Build();

            resultViewModel = TodoListDetailViewmodelFactory.Create(todoList, true);
        }

        [Fact]
        public void DoesNotContainCompletedItem()
        {
            Assert.DoesNotContain(resultViewModel.Items, item => item.IsDone);
        }

        [Fact]
        public void ContainsUncompletedItem()
        {
            Assert.Contains(resultViewModel.Items, item => item.Title == "Uncompleted Item");
        }
    }
}
