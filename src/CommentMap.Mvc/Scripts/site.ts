const STORAGE_KEY = "commentmap-theme";

function currentTheme(): string {
  return document.documentElement.getAttribute("data-theme") === "dark" ? "dark" : "light";
}

function applyTheme(theme: string): void {
  document.documentElement.setAttribute("data-theme", theme);
  const toggle = document.querySelector<HTMLInputElement>("[data-theme-toggle]");
  if (toggle) {
    toggle.checked = theme === "dark";
  }
}

const storedTheme = localStorage.getItem(STORAGE_KEY);
if (storedTheme === "light" || storedTheme === "dark") {
  applyTheme(storedTheme);
}

const themeToggle = document.querySelector<HTMLInputElement>("[data-theme-toggle]");
themeToggle?.addEventListener("change", () => {
  const theme = currentTheme() === "dark" ? "light" : "dark";
  localStorage.setItem(STORAGE_KEY, theme);
  applyTheme(theme);
});

document.querySelectorAll<HTMLElement>("[data-alert-dismiss]").forEach((button) => {
  button.addEventListener("click", () => {
    button.closest("[data-alert]")?.remove();
  });
});
