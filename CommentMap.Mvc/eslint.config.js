import globals from "globals";
import pluginJs from "@eslint/js";
import tseslint from "typescript-eslint";


export default [
  {
    languageOptions: { globals: globals.browser },
    ignores: ["./wwwroot", "./build", "./eslint.config.js"],
  },
  pluginJs.configs.recommended,
  ...tseslint.configs.recommended,
];
