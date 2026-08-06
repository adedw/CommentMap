import * as esbuild from "esbuild";
import { cp, mkdir, readFile } from "node:fs/promises";
import postcss from "postcss";
import tailwindcss from "@tailwindcss/postcss";

const copies = [
  ["node_modules/aspnet-client-validation/dist/aspnet-validation.min.js", "wwwroot/js/"],
];

await Promise.all(
  copies.map(async ([from, to]) => {
    await mkdir(to, { recursive: true });
    await cp(from, to + from.split("/").pop(), { force: true });
  })
);

const postcssPlugin = {
  name: "postcss",
  setup(build) {
    build.onLoad({ filter: /\.css$/ }, async (args) => {
      const source = await readFile(args.path, "utf8");
      const result = await postcss([tailwindcss({ base: process.cwd() })])
        .process(source, { from: args.path, to: args.path });
      return { contents: result.css, loader: "css" };
    });
  },
};

await esbuild.build({
  entryPoints: {
    "js/Comments.min": "./Pages/Comments/Index.cshtml.ts",
    "js/AddComment.min": "./Pages/Comments/Add.cshtml.ts",
    "js/site.min": "./Scripts/site.ts",
    "css/app.min": "./Styles/app.css",
    "css/ol.min": "./node_modules/ol/ol.css",
  },
  plugins: [postcssPlugin],
  target: "es2018",
  bundle: true,
  minify: true,
  sourcemap: true,
  outdir: "./wwwroot",
});
