using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Controllers;
using Todo.Data;
using Todo.Data.Entities;
using Todo.EntityModelMappers.TodoItems;
using Todo.Models.TodoItems;
using Todo.Services;

namespace Todo.Areas.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly GravatarService gravatarService;

        public TodoItemsController(ApplicationDbContext dbContext, GravatarService gravatarService)
        {
            this.dbContext = dbContext;
            this.gravatarService = gravatarService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewListItem([FromBody] TodoItemCreateFields fields)
        {
            var todoList = await dbContext.TodoLists.FirstOrDefaultAsync(list => list.TodoListId == fields.TodoListId);
            if (todoList == null)
            {
                ModelState.AddModelError(nameof(fields.TodoListId), "Todo List does not exist");
            }

            var responsibleParty = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == fields.ResponsiblePartyId);
            if (responsibleParty == null)
            {
                ModelState.AddModelError(nameof(fields.ResponsiblePartyId), "Responsible party does not exist");
            }

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var item = new TodoItem(fields.TodoListId, fields.ResponsiblePartyId, fields.Title, fields.Importance);

            todoList.Items.Add(item);
            await dbContext.SaveChangesAsync();

            var viewModel = TodoItemSummaryViewmodelFactory.Create(item);

            viewModel.ResponsibleParty.FullName = await gravatarService.GetUserFullName(viewModel.ResponsibleParty.Email);

            return Ok(viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> EditListItem([FromBody] TodoItemEditFields fields)
        {
            var responsibleParty = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == fields.ResponsiblePartyId);
            if (responsibleParty == null)
            {
                ModelState.AddModelError(nameof(fields.ResponsiblePartyId), "Responsible party does not exist");
            }

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var existingTodoItem = await dbContext.TodoItems
                .Include(item => item.TodoList)
                .FirstOrDefaultAsync(item => item.TodoItemId == fields.TodoItemId);

            if (existingTodoItem == null)
            {
                return NotFound();
            }

            TodoItemEditFieldsFactory.Update(fields, existingTodoItem);

            dbContext.Update(existingTodoItem);
            await dbContext.SaveChangesAsync();

            return Ok(TodoItemEditFieldsFactory.Create(existingTodoItem));
        }
    }
}
