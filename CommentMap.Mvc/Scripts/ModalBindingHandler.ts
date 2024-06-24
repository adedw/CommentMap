import ConfirmDeletePopupViewModel from "./ConfirmDeletePopupViewModel";

export default function setupModalBindingHandler() {
  ko.bindingHandlers.modal = { init, update };
}

function init(element: HTMLElement, valueAccessor: KnockoutObservable<ConfirmDeletePopupViewModel>) {
  element.addEventListener("hidden.bs.modal", () => {
    const popupAccessor = valueAccessor();
    popupAccessor.close();
  });
}

function update(element: HTMLElement, valueAccessor: KnockoutObservable<ConfirmDeletePopupViewModel>) {
  const confirmDeletionForm: HTMLFormElement = element.querySelector("#confirm-deletion-form");

  const id = valueAccessor().id();
  const show = id !== undefined && id.length > 0;

  if (confirmDeletionForm) {
    const url = new URL(confirmDeletionForm.action);
    if (show) {
      url.searchParams.set("id", id);
    } else {
      url.searchParams.delete("id");
    }
    confirmDeletionForm.action = url.toString();
  }

  $(element).modal(show ? "show" : "hide");
}
