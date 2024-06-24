export default class ConfirmDeletePopupViewModel {
  public id: KnockoutObservable<string>;
  public title: KnockoutObservable<string>;

  constructor() {
    this.id = ko.observable(undefined);
    this.title = ko.observable(undefined);
  }

  public open(id: string, title: string) {
    this.id(id);
    this.title(title);
  }

  public close() {
    this.id(undefined);
    this.title(undefined);
  }
}
