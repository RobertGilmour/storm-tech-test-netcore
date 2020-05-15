using Microsoft.AspNetCore.Identity;
using Todo.Data.Entities;
using Todo.EntityModelMappers.TodoLists;
using Todo.Models.TodoLists;
using Xunit;

namespace Todo.Tests.WhenTodoListIsConvertedToDetailsViewModel
{
    public class AndOrderByFieldRank
    {
        private readonly TodoListDetailViewmodel resultViewModel;

        public AndOrderByFieldRank()
        {
            var user = new IdentityUser("alice@example.com");
            user.Email = user.UserName;
            var todoList = new TestTodoListBuilder(user, "shopping list")
                .WithItem("1", Importance.Low, rank: 3)
                .WithItem("2", Importance.High, rank: 2)
                .WithItem("3", Importance.Medium, rank: 1)
                .Build();

            resultViewModel = TodoListDetailViewmodelFactory.Create(todoList, true, TodoListSortFields.Rank);
        }

        [Fact]
        public void TodoListOrderedByRank()
        {
            Assert.Collection(resultViewModel.Items,
                item => Assert.Equal("3", item.Title),
                item => Assert.Equal("2", item.Title),
                item => Assert.Equal("1", item.Title));
        }
    }
}
