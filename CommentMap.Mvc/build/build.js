import * as esbuild from "esbuild";

await esbuild.build({
  entryPoints: {
    "js/Comments.min": "./Scripts/Comments.ts",
    "css/ol.min": "./node_modules/ol/ol.css",
    "js/AddComment.min": "./Scripts/AddComment.ts"
  },
  bundle: true,
  minify: true,
  sourcemap: true,
  outdir: "./wwwroot",
});
