var baseAddress = location.protocol + "//" + location.hostname + (location.port ? ":" + location.port : "");

var addNewItemCollapsibleDiv = document.getElementById("addNewItem");

var titleInput = document.getElementById("Title");
var importanceSelect = document.getElementById("Importance");
var responsiblePartySelect = document.getElementById("ResponsiblePartyId");
var todoListIdHidden = document.getElementById("TodoListId");

var titleErrors = document.getElementById("TitleErrors");
var importanceErrors = document.getElementById("ImportanceErrors");
var responsiblePartyErrors = document.getElementById("ResponsiblePartyIdErrors");

var todoList = document.getElementById("TodoList");

function clearForm() {
  titleErrors.innerHTML = "";
  importanceErrors.innerHTML = "";
  responsiblePartyErrors.innerHTML = "";

  titleInput.value = "";
  importanceSelect.value = "Medium";
  responsiblePartySelect.value = responsiblePartySelect.children[0].value;
}

function handleAddNewItemErrors(errors) {
  titleErrors.innerHTML = errors.Title ? errors.Title.join(", ") : "";
  importanceErrors.innerHTML = errors.Importance ? errors.Importance.join(", ") : "";
  responsiblePartyErrors.innerHTML = errors.ResponsiblePartyId ? errors.ResponsiblePartyId.join(", ") : "";
}

function handleAddNewItemSuccess(newItem) {
  var contextualClass = "";
  switch (newItem.importance) {
  case 0:
    contextualClass = "list-group-item-danger";
    break;
  case 2:
    contextualClass = "list-group-item-info";
    break;
  }

  var newListItem = $("<li />").addClass("list-group-item").addClass(contextualClass).addClass("todo-item").data("id", newItem.todoItemId).appendTo(todoList);
  var div = $("<div />").addClass("row").appendTo(newListItem);
  var titleDiv = $("<div />").addClass("col-md-8").appendTo(div);
  var link = $("<a />").attr("href", baseAddress + "/TodoItem/Edit?todoItemId=" + newItem.todoItemId).appendTo(titleDiv);
  $("<text />").text(newItem.title).appendTo(link);

  var responsiblePartyDiv = $("<div />").addClass("col-md-4").addClass("text-right").appendTo(div);
  var small = $("<small />")
    .text((newItem.responsibleParty.fullName
      ? newItem.responsibleParty.fullName + " (" + newItem.responsibleParty.userName + ")"
      : newItem.responsibleParty.userName) + " ").appendTo(responsiblePartyDiv);
  $("<img />").attr("src", "https://www.gravatar.com/avatar/" + newItem.responsibleParty.gravatarHash + "?s=30")
    .appendTo(small);

  var editDiv = $("<div />").addClass("col-xs-12").attr("id", "edit" + newItem.todoItemId).appendTo(div);
  var editForm = $("<form />")
    .attr("onsubmit", "event.preventDefault();handleEditTodoItem(" + newItem.todoItemId + ");").appendTo(editDiv);
  $("<input />").attr("type", "hidden").attr("id", "TodoItemId-" + newItem.todoItemId).val(newItem.todoItemId).appendTo(editForm);
  $("<input />").attr("type", "hidden").attr("id", "Title-" + newItem.todoItemId).val(newItem.title).appendTo(editForm);
  $("<input />").attr("type", "hidden").attr("id", "IsDone-" + newItem.todoItemId).val(newItem.isDone).appendTo(editForm);
  $("<input />").attr("type", "hidden").attr("id", "ResponsiblePartyId-" + newItem.todoItemId).val(newItem.responsibleParty.id).appendTo(editForm);
  $("<input />").attr("type", "hidden").attr("id", "Importance-" + newItem.todoItemId).val(newItem.importance).appendTo(editForm);

  var rankDiv = $("<div />").addClass("form-group").appendTo(editForm);
  $("<label />").attr("for", "Rank-" + newItem.todoItemId).text("Rank").appendTo(rankDiv);
  $("<input />").attr("id", "Rank-" + newItem.todoItemId).text("Rank").addClass("rank-input").addClass("form-control")
    .attr("type", "number").attr("min", "1").attr("step", "1").val(newItem.rank).attr("onchange", "handleEditTodoItem(" + newItem.todoItemId + ");")
    .appendTo(rankDiv);
  $("<span />").attr("id", "RankErrors-" + newItem.todoItemId).attr("for", "Rank-" + newItem.todoItemId).appendTo(rankDiv);

  clearForm();

  addNewItemCollapsibleDiv.classList.remove("in");

  sortTodoItems();
}

function handleAddNewItem(e) {
  e.preventDefault();
  var newItem = {
    todoListId: todoListIdHidden.value,
    title: titleInput.value,
    importance: importanceSelect.value,
    responsiblePartyId: responsiblePartySelect.value
  };

  fetch(baseAddress + "/api/TodoItems",
    {
      method: "POST",
      headers: {
        'Content-Type': "application/json"
      },
      body: JSON.stringify(newItem)
    }
  ).then(res => {
    if (res.ok) {
      res.json().then(content => {
        handleAddNewItemSuccess(content);
      });
    } else if (res.status === 400) {
      res.json().then(content => {
        handleAddNewItemErrors(content);
      });
    } else {
      alert("List item could not be added at this time, please try again later.");
    }
  }).catch(err => {
    console.log(err);
  });
}

function sortTodoItems() {
  if (document.getElementById("OrderByField").value === "Rank") {
    $("#TodoList li.todo-item").sort(function (item1, item2) {
      var id1 = $(item1).data("id");
      var id2 = $(item2).data("id");
      var value1 = $("#Rank-" + id1).val();
      var value2 = $("#Rank-" + id2).val();
      
      if (value1 === value2) {
        return id1 > id2 ? 1 : -1;
      }
      return value1 > value2 ? 1 : -1;
    }).appendTo("#TodoList");
  }
}

function handleEditItemSuccess(editedItem) {
  var todoItemId = editedItem.todoItemId;
  var rankErrors = document.getElementById("RankErrors-" + todoItemId);
  rankErrors.innerHTML = "";

  sortTodoItems();
}

function handleEditItemErrors(todoItemId, errors) {
  var rankErrors = document.getElementById("RankErrors-" + todoItemId);
  rankErrors.innerHTML = errors.Title ? errors.Title.join(", ") : "";
}

function handleEditTodoItem(todoItemId) {
  var editedItem = {
    todoItemId: document.getElementById("TodoItemId-" + todoItemId).value,
    title: document.getElementById("Title-" + todoItemId).value,
    isDone: document.getElementById("IsDone-" + todoItemId).value,
    responsiblePartyId: document.getElementById("ResponsiblePartyId-" + todoItemId).value,
    importance: document.getElementById("Importance-" + todoItemId).value,
    rank: document.getElementById("Rank-" + todoItemId).value
  };
  
  fetch(baseAddress + "/api/TodoItems",
    {
      method: "PUT",
      headers: {
        'Content-Type': "application/json"
      },
      body: JSON.stringify(editedItem)
    }
  ).then(res => {
    if (res.ok) {
      res.json().then(content => {
        handleEditItemSuccess(content);
      });
    } else if (res.status === 400) {
      res.json().then(content => {
        handleEditItemErrors(todoItemId, content);
      });
    } else {
      alert("List item could not be edited at this time, please try again later.");
    }
  }).catch(err => {
    console.log(err);
  });
}

clearForm();