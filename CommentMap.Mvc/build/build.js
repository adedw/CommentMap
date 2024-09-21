import * as esbuild from "esbuild";

await esbuild.build({
  entryPoints: {
    "js/Comments.min": "./Scripts/Comments.ts",
    "js/AddComment.min": "./Scripts/AddComment.ts",
    "css/ol.min": "./node_modules/ol/ol.css",
    "css/validation-summary-errors.min": "./Styles/validation-summary-errors.css"
  },
  bundle: true,
  minify: true,
  sourcemap: true,
  outdir: "./wwwroot",
});
