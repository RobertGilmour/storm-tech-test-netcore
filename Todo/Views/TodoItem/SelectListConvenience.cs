﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Todo.Data;
using Todo.Data.Entities;
using Todo.Models.TodoLists;

namespace Todo.Views.TodoItem
{
    public static class SelectListConvenience
    {
        public static readonly SelectListItem[] ImportanceSelectListItems =
        {
            new SelectListItem {Text = "High", Value = Importance.High.ToString()},
            new SelectListItem {Text = "Medium", Value = Importance.Medium.ToString()},
            new SelectListItem {Text = "Low", Value = Importance.Low.ToString()},
        };

        public static List<SelectListItem> UserSelectListItems(this ApplicationDbContext dbContext)
        {
            return dbContext.Users.Select(u => new SelectListItem {Text = u.UserName, Value = u.Id}).ToList();
        }

        public static readonly SelectListItem[] TodoListDetailsSortFields =
        {
            new SelectListItem {Text = "Default", Value = TodoListSortFields.Default.ToString()},
            new SelectListItem {Text = "Importance", Value = TodoListSortFields.Importance.ToString()},
            new SelectListItem {Text = "Rank", Value = TodoListSortFields.Rank.ToString()},
        };
    }
}