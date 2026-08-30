const downloadButton = document.getElementById("download-recovery-codes");
if (downloadButton) {
  const codes = Array.from(document.querySelectorAll("[data-recovery-code]"))
    .map((e) => e.textContent)
    .filter((code): code is string => code !== null);
  downloadButton.addEventListener("click", () => {
    const blob = new Blob([codes.join("\n")], { type: "text/plain" });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = "recovery-codes.txt";
    anchor.click();
    URL.revokeObjectURL(url);
  });
}
