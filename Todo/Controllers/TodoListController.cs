using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Data.Entities;
using Todo.EntityModelMappers.TodoLists;
using Todo.Models.TodoLists;
using Todo.Services;

namespace Todo.Controllers
{
    [Authorize]
    public class TodoListController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUserStore<IdentityUser> userStore;
        private readonly GravatarService gravatarService;

        public TodoListController(ApplicationDbContext dbContext, IUserStore<IdentityUser> userStore, GravatarService gravatarService)
        {
            this.dbContext = dbContext;
            this.userStore = userStore;
            this.gravatarService = gravatarService;
        }

        public IActionResult Index()
        {
            var userId = User.Id();
            var todoLists = dbContext.RelevantTodoLists(userId);
            var viewmodel = TodoListIndexViewmodelFactory.Create(todoLists);
            return View(viewmodel);
        }

        public IActionResult Detail(int todoListId, bool hideCompletedItems, TodoListSortFields orderByField)
        {
            var todoList = dbContext.SingleTodoList(todoListId);
            var viewmodel = TodoListDetailViewmodelFactory.Create(todoList, hideCompletedItems, orderByField);

            var userNames = viewmodel.Items
                .Select(i => i.ResponsibleParty.Email)
                .Distinct()
                .ToDictionary(i => i, i => gravatarService.GetUserFullName(i));

            foreach (var todoItem in viewmodel.Items)
            {
                todoItem.ResponsibleParty.FullName = userNames[todoItem.ResponsibleParty.Email];
            }

            return View(viewmodel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TodoListFields());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoListFields fields)
        {
            if (!ModelState.IsValid) { return View(fields); }

            var currentUser = await userStore.FindByIdAsync(User.Id(), CancellationToken.None);

            var todoList = new TodoList(currentUser, fields.Title);

            await dbContext.AddAsync(todoList);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Create", "TodoItem", new {todoList.TodoListId});
        }
    }
}