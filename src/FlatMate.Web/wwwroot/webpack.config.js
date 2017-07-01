const glob = require("glob");
const path = require('path');
const webpack = require("webpack");
const ExtractTextPlugin = require("extract-text-webpack-plugin");

module.exports = (env) => {
    if (!env) {
        env = "Debug";
    }

    console.log();
    console.log(`Running webpack in ${env} mode`);
    console.log();

    /**
     * Setup Javascript configuration
     */
    var jsConfig = {};
    jsConfig.name = "js";
    jsConfig.entry = {
        app: "./js/app.ts",
        vendor: ["knockout", "knockout-dragdrop", "es6-promise"]
    };
    jsConfig.output = {
        path: path.resolve(__dirname, './dist'),
        publicPath: '/dist/',
        filename: '[name].js'
    };
    jsConfig.resolve = { extensions: [".ts", ".js"] };
    jsConfig.module = {
        loaders: [{ test: /\.ts$/, loader: "ts-loader" }]
    };
    jsConfig.plugins = [
        new webpack.optimize.CommonsChunkPlugin({ name: "vendor" })
    ];

    if ("Debug" === env) {
        jsConfig.devtool = 'source-map';
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

    var cssConfig = {};
    cssConfig.name = "css";
    cssConfig.entry = glob.sync("./css/**/*.scss");
    cssConfig.output = { filename: "./dist/app.css" };
    cssConfig.module = {
        rules: [
            {
                test: /\.scss$/,
                use: ExtractTextPlugin.extract({
                    fallback: "style-loader",
                    use: [
                        { loader: "css-loader", options: cssLoaderOptions },
                        { loader: "sass-loader" }
                    ]
                })
            }
        ]
    };
    cssConfig.plugins = [
        new ExtractTextPlugin("./dist/app.css")
    ];

    return [jsConfig, cssConfig];
};