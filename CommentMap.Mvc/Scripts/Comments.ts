import CommentsViewModel from "./CommentsViewModel";
import setupModalBindingHandler from "./ModalBindingHandler";

$(() => {
  const root = document.getElementById("root");
  if (!root) {
    return;
  }

  const elements = document.querySelectorAll("[data-location]");
  const coordinates: [number, number][] = Array.from(elements)
    .map((element) => JSON.parse(element.getAttribute("data-location")));

  setupModalBindingHandler();
  ko.applyBindings(new CommentsViewModel(coordinates), root);
});
