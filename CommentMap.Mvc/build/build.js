import * as esbuild from "esbuild";

await esbuild.build({
  entryPoints: {
    "js/comments.min": "./Scripts/comments.ts",
    "css/ol.min": "./node_modules/ol/ol.css",
    "js/addComment.min": "./Scripts/addComment.ts"
  },
  bundle: true,
  minify: true,
  sourcemap: true,
  outdir: "./wwwroot",
});
