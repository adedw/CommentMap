const STORAGE_KEY = "commentmap-theme";

function currentTheme(): string {
  return document.documentElement.getAttribute("data-theme") === "business" ? "business" : "corporate";
}

function applyTheme(theme: string): void {
  document.documentElement.setAttribute("data-theme", theme);
  const toggle = document.querySelector<HTMLInputElement>("[data-theme-toggle]");
  if (toggle) {
    toggle.checked = theme === "business";
  }
}

const storedTheme = localStorage.getItem(STORAGE_KEY);
if (storedTheme === "corporate" || storedTheme === "business") {
  applyTheme(storedTheme);
}

const themeToggle = document.querySelector<HTMLInputElement>("[data-theme-toggle]");
themeToggle?.addEventListener("change", () => {
  const theme = currentTheme() === "business" ? "corporate" : "business";
  localStorage.setItem(STORAGE_KEY, theme);
  applyTheme(theme);
});
