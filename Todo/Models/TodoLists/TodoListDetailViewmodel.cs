using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using Remotion.Linq.Clauses;
using Todo.Models.TodoItems;

namespace Todo.Models.TodoLists
{
    public class TodoListDetailViewmodel
    {
        public int TodoListId { get; }
        public string Title { get; }
        public ICollection<TodoItemSummaryViewmodel> Items { get; }

        [Display(Name = "Hide Completed Items")]
        public bool HideCompletedItems { get; set; }

        [Display(Name = "Order By")]
        public TodoListSortFields OrderByField { get; set; }

        public TodoListDetailViewmodel(int todoListId,
            string title,
            ICollection<TodoItemSummaryViewmodel> items,
            bool hideCompletedItems,
            TodoListSortFields orderByField)
        {
            Items = items;
            TodoListId = todoListId;
            Title = title;
            HideCompletedItems = hideCompletedItems;
            OrderByField = orderByField;
        }
    }
}