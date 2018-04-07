const glob = require("glob");
const path = require("path");
const webpack = require("webpack");
const ExtractTextPlugin = require("extract-text-webpack-plugin");

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
    jsConfig.entry = {
        app: "./js/app.ts",
        vendor: ["knockout", "es6-promise", "blazy", "array.prototype.find", "dateformat"]
    };
    jsConfig.output = {
        path: path.resolve(__dirname, "./dist"),
        publicPath: "/dist/",
        filename: "[name].js"
    };
    jsConfig.resolve = {
        extensions: [".ts", ".js"],
        alias: {'tslib$': 'tslib/tslib.es6.js'},
        modules: [
            path.resolve('./node_modules'),
            path.resolve('./js')
        ]
    };
    jsConfig.module = {
        loaders: [{ test: /\.ts$/, loader: "ts-loader" }]
    };
    jsConfig.plugins = [
        new webpack.optimize.CommonsChunkPlugin({ name: "vendor" }),
        new webpack.ProvidePlugin({
            '__assign': ['tslib', '__assign'],
            '__awaiter': ['tslib', '__awaiter'],
            '__extends': ['tslib', '__extends'],
            '__generator': ['tslib', '__generator']
        })];

    if ("Debug" === env) {
        jsConfig.devtool = "inline-source-map";
    }

    if ("Release" === env) {
        jsConfig.plugins.push(new webpack.optimize.UglifyJsPlugin());
    }

    /**
     * Setup CSS configuration
     */

    var cssLoaderOptions = { minimize: false, url: false };
    if ("Release" === env) {
        cssLoaderOptions.minimize = true;
    }

    const cssConfig = {};
    cssConfig.name = "css";
    cssConfig.entry = glob.sync("./css/**/*.scss");
    cssConfig.output = { filename: "./dist/app.css" };
    cssConfig.module = {
        rules: [
            {
                test: /\.scss$/,
                use: ExtractTextPlugin.extract({
                    fallback: "style-loader",
                    use: [{ loader: "css-loader", options: cssLoaderOptions }, { loader: "sass-loader" }]
                })
            }
        ]
    };
    cssConfig.plugins = [new ExtractTextPlugin("./dist/app.css")];

    return [jsConfig, cssConfig];
};
