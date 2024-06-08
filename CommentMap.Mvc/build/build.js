import * as esbuild from "esbuild";

await esbuild.build({
    entryPoints: ["./Scripts/comments.ts"],
    bundle: true,
    minify: true,
    sourcemap: true,
    outfile: "./wwwroot/js/comments.min.js",
})
