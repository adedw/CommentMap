import CommentsViewModel from "./CommentsViewModel";

$(() => {
  const elements = document.querySelectorAll("[data-location]");
  const coordinates: [number, number][] = Array.from(elements)
    .map((element) => JSON.parse(element.getAttribute("data-location")));

  const root = document.getElementById("root");

  ko.applyBindings(new CommentsViewModel(coordinates), root);
});
