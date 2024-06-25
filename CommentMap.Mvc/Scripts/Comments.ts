import CommentsViewModel from "./CommentsViewModel";

$(() => {
  const root = document.getElementById("root");
  if (!root) {
    return;
  }

  const elements = document.querySelectorAll("[data-location]");
  const coordinates: [number, number][] = Array.from(elements)
    .map((element) => JSON.parse(element.getAttribute("data-location")));

  ko.applyBindings(new CommentsViewModel(coordinates), root);
});
