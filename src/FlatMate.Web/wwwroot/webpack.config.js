const glob = require("glob");
const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = env => {
    if (!env) {
        env = "Debug";
    }

    console.log();
    console.log(`Running webpack in ${env} mode`);
    console.log();

    /**
     * Setup Javascript configuration
     */
    const jsConfig = {};
    jsConfig.name = "js";
    jsConfig.entry = "./js/app.ts";
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
        filename: "app.js",
        path: path.resolve(__dirname, "dist")
    };

    if ("Debug" === env) {
        jsConfig.devtool = "inline-source-map";
    }

    if ("Release" === env) {
        jsConfig.plugins.push(new webpack.optimize.UglifyJsPlugin());
    }

    // jsConfig.entry = {
    //     app: "./js/app.ts",
    //     vendor: ["knockout", "es6-promise", "blazy", "array.prototype.find", "dateformat"]
    // };
    // jsConfig.output = {
    //     filename: 'bundle.js',
    //     path: path.resolve(__dirname, 'dist')
    //     // path: path.resolve(__dirname, "./dist"),
    //     // publicPath: "/dist/",
    //     // filename: "[name].js"
    // };
    // jsConfig.resolve = {
    //     extensions: [".ts", ".js"],
    //     // alias: { "tslib$": "tslib/tslib.es6.js" },
    //     // modules: [
    //     //     path.resolve("./node_modules"),
    //     //     path.resolve("./js")
    //     // ]
    // };
    // jsConfig.module = {
    //     rules: [{ test: /\.ts$/, loader: "ts-loader", exclude: /node_modules/ }]
    // };
    // jsConfig.plugins = [
    //     new webpack.ProvidePlugin({
    //         "__assign": ["tslib", "__assign"],
    //         "__awaiter": ["tslib", "__awaiter"],
    //         "__extends": ["tslib", "__extends"],
    //         "__generator": ["tslib", "__generator"]
    //     })];
    //
    // if ("Debug" === env) {
    //     jsConfig.devtool = "inline-source-map";
    // }
    //
    // if ("Release" === env) {
    //     jsConfig.plugins.push(new webpack.optimize.UglifyJsPlugin());
    // }

    /**
     * Setup CSS configuration
     */
    const cssLoaderOptions = { minimize: false, url: false };
    if ("Release" === env) {
        cssLoaderOptions.minimize = true;
    }

    const cssConfig = {};
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
