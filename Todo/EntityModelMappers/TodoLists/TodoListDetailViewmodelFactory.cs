using System.Collections.Generic;
using System.Linq;
using Todo.Data.Entities;
using Todo.EntityModelMappers.TodoItems;
using Todo.Models.TodoLists;

namespace Todo.EntityModelMappers.TodoLists
{
    public static class TodoListDetailViewmodelFactory
    {
        public static TodoListDetailViewmodel Create(
            TodoList todoList, 
            bool hideCompletedItems,
            TodoListSortFields orderByField)
        {
            IEnumerable<TodoItem> Order(IEnumerable<TodoItem> unorderedItems)
            {
                switch (orderByField)
                {
                    case TodoListSortFields.Importance:
                        return unorderedItems.OrderBy(item => item.Importance);
                    case TodoListSortFields.Rank:
                        return unorderedItems.OrderBy(item => item.Rank);
                    default:
                        return unorderedItems;
                }
            }

            var items = Order(todoList.Items
                .Where(i => !hideCompletedItems || !i.IsDone))
                .Select(TodoItemSummaryViewmodelFactory.Create)
                .ToList();

            return new TodoListDetailViewmodel(todoList.TodoListId, todoList.Title, items, hideCompletedItems, orderByField);
        }
    }
}