import AddCommentViewModel from "./AddCommentViewModel";

$(() => {
  const longitude = document.querySelector("[data-bind=\"value: longitude\"]").getAttribute("value");
  const latitude = document.querySelector("[data-bind=\"value: latitude\"]").getAttribute("value");
  const root = document.getElementById("root");
  ko.applyBindings(new AddCommentViewModel(Number(longitude), Number(latitude)), root);
})
