export default class PopupViewModel {
  public show: KnockoutObservable<boolean>;

  constructor() {
    this.show = ko.observable(false);
    this.submit = this.submit.bind(this);
  }

  public submit() {
    console.log("submit");
    this.show(false);
  }
}
