import AddCommentViewModel from "./AddCommentViewModel";

$(() => {
  const root = document.getElementById("root");
  if (!root) {
    return;
  }

  const longitude = root.querySelector("[data-bind=\"value: localLongitude\"]").getAttribute("value");
  const latitude = root.querySelector("[data-bind=\"value: localLatitude\"]").getAttribute("value");
  const locale = root.querySelector("[data-locale]").getAttribute("data-locale");

  ko.applyBindings(new AddCommentViewModel(Number(longitude), Number(latitude), locale), root);
})
