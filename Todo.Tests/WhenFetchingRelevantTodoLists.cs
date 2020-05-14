using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Todo.Data.Entities;
using Todo.Services;
using Xunit;

namespace Todo.Tests
{
    public class WhenFetchingRelevantTodoLists
    {
        private readonly List<TodoList> resultTodoLists;
        private readonly TodoList listWhereUserIsOwner;
        private readonly TodoList listWhereUserIsResponsibleForItem;
        private readonly TodoList listWhereUserIsBothOwnerAndResponsibleForItem;
        private readonly TodoList irrelevantList;

        public WhenFetchingRelevantTodoLists()
        {
            var user = new IdentityUser("alice@example.com");
            var otherUser = new IdentityUser("bob@example.com");

            listWhereUserIsOwner = new TodoList(user, "User is owner");
            listWhereUserIsResponsibleForItem = new TodoList(otherUser, "User is responsible for item");
            listWhereUserIsBothOwnerAndResponsibleForItem = new TodoList(user, "User is owner and responsible for item");
            irrelevantList = new TodoList(otherUser, "Irrelevant list");

            using (var ctx = new TestDbContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();

                ctx.AddRange(listWhereUserIsOwner, 
                    listWhereUserIsResponsibleForItem,
                    listWhereUserIsBothOwnerAndResponsibleForItem, 
                    irrelevantList);

                listWhereUserIsOwner.Items
                    .Add(new TodoItem(listWhereUserIsOwner.TodoListId, otherUser.Id, "Todo 1", Importance.Medium));

                listWhereUserIsResponsibleForItem.Items
                    .Add(new TodoItem(listWhereUserIsResponsibleForItem.TodoListId, user.Id, "Todo 2", Importance.Medium));

                listWhereUserIsBothOwnerAndResponsibleForItem.Items
                    .Add(new TodoItem(listWhereUserIsBothOwnerAndResponsibleForItem.TodoListId, user.Id, "Todo 3", Importance.Medium));

                irrelevantList.Items
                    .Add(new TodoItem(listWhereUserIsOwner.TodoListId, otherUser.Id, "Todo 4", Importance.Medium));

                ctx.SaveChanges();

                resultTodoLists = ctx.RelevantTodoLists(user.Id).ToList();
            }
        }

        [Fact]
        public void ContainsListWhereUserIsOwner()
        {
            Assert.Contains(resultTodoLists, item => item.TodoListId == listWhereUserIsOwner.TodoListId);
        }

        [Fact]
        public void ContainsListWhereUserIsResponsibleForTodoItem()
        {
            Assert.Contains(resultTodoLists, item => item.TodoListId == listWhereUserIsResponsibleForItem.TodoListId);
        }

        [Fact]
        public void ContainsListWhereUserIsOwnerAndResponsibleForTodoItem()
        {
            Assert.Contains(resultTodoLists, item => item.TodoListId == listWhereUserIsBothOwnerAndResponsibleForItem.TodoListId);
        }

        [Fact]
        public void DoesNotContainIrrelevantList()
        {
            Assert.DoesNotContain(resultTodoLists, item => item.TodoListId == irrelevantList.TodoListId);
        }
    }
}
