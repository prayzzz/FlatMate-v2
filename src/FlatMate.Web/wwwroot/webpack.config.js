const glob = require("glob");
const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = (env, argv) => {
    let mode = "production";
    if (argv.mode) {
        mode = argv.mode.toLowerCase();
    }

    console.log();
    console.log(`Running webpack in '${mode}' mode`);
    console.log();

    /**
     * Setup Javascript configuration
     */
    const jsConfig = {};
    jsConfig.mode = mode;
    jsConfig.name = "js";
    jsConfig.entry = {
        app: "./js/app.ts",
        // vendor: ["knockout", "es6-promise", "blazy", "array.prototype.find", "dateformat"]
    };
    jsConfig.module = {
        rules: [
            {
                test: /\.tsx?$/,
                use: "ts-loader",
                exclude: /node_modules/
            }
        ]
    };
    jsConfig.resolve = {
        extensions: [".tsx", ".ts", ".js"],
        modules: [
            path.resolve("./node_modules"),
            path.resolve("./js")
        ]
    };
    jsConfig.plugins = [
        new webpack.ProvidePlugin({
            "__assign": ["tslib", "__assign"],
            "__awaiter": ["tslib", "__awaiter"],
            "__extends": ["tslib", "__extends"],
            "__generator": ["tslib", "__generator"]
        })];
    jsConfig.output = {
        filename: "[name].js",
        path: path.resolve(__dirname, "dist")
    };

    if ("development" === mode) {
        jsConfig.devtool = "inline-source-map";
    }

    /**
     * Setup CSS configuration
     */
    const cssLoaderOptions = { minimize: false, url: false };
    if ("production" === mode) {
        cssLoaderOptions.minimize = true;
    }

    const cssConfig = {};
    cssConfig.mode = mode;
    cssConfig.name = "css";
    cssConfig.entry = glob.sync("./css/**/*.scss");
    cssConfig.module = {
        rules: [
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    { loader: "css-loader", options: cssLoaderOptions },
                    { loader: "sass-loader", options: {} }
                ]
            }
        ]
    };
    cssConfig.plugins = [new MiniCssExtractPlugin({
        filename: "app.css",
        chunkFilename: "[id].css"
    })];

    return [jsConfig, cssConfig];
};
