export default [
  {
    files: ["*.js", "*.jsx"],
    languageOptions: {
      parser: "@babel/eslint-parser",
      parserOptions: {
        requireConfigFile: false,
        babelOptions: {
          presets: ["@babel/preset-react"],
        },
      },
    },
    rules: {
      "react/react-in-jsx-scope": "error",
    },
  },
];
