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
  console.log(newItem);

  var contextualClass = "";
  switch (newItem.importance) {
  case 0:
    contextualClass = "list-group-item-danger";
    break;
  case 2:
    contextualClass = "list-group-item-info";
    break;
  }

  var newListItem = $("<li />").addClass("list-group-item").addClass(contextualClass).appendTo(todoList);
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

  clearForm();

  addNewItemCollapsibleDiv.classList.remove("in");
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

clearForm();