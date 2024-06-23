export default function setupModalBindingHandler() {
    ko.bindingHandlers.modal = { init, update };
}

function init(element: HTMLElement, valueAccessor: KnockoutObservable<{ show: KnockoutObservable<boolean> }>) {
  element.addEventListener("hidden.bs.modal", () => {
    const showAccessor: { show: KnockoutObservable<boolean> } = valueAccessor();
    showAccessor.show(false);
  });
}

function update(element, valueAccessor: KnockoutObservable<{ show: KnockoutObservable<boolean> }>) {
  const isShown = showAccessor(valueAccessor);
  $(element).modal(isShown ? "show" : "hide");
}

function showAccessor(accessor: KnockoutObservable<{ show: KnockoutObservable<boolean> }>) {
  var obj = ko.utils.unwrapObservable(accessor());
  return ko.utils.unwrapObservable(obj.show);
}
