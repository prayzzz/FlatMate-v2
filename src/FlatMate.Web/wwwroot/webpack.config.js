const webpack = require("webpack");
const glob = require("glob");
const ExtractTextPlugin = require("extract-text-webpack-plugin");

module.exports = [
    {
        name: "js",
        entry: {
            app: "./js/app.ts",
            vendor: ["knockout", "knockout-dragdrop"]
        },
        output: {
            filename: "./dist/[name].js"
        },
        devtool: "source-map",
        resolve: {
            extensions: [".webpack.js", ".web.js", ".ts", ".js", ".scss"]
        },
        module: {
            loaders: [
                { test: /\.ts$/, loader: "ts-loader" }
            ]
        },
        plugins: [
            new webpack.optimize.CommonsChunkPlugin({ name: "vendor" })
        ]
    },
    {
        name: "css",
        entry: glob.sync("./css/**/*.scss"),
        output: {
            filename: "./dist/app.css"
        },
        module: {
            rules: [
                {
                    test: /\.scss$/,
                    use: ExtractTextPlugin.extract({
                        fallback: "style-loader",
                        use: "css-loader!sass-loader"
                    })
                }
            ]
        },
        plugins: [
            new ExtractTextPlugin("./dist/app.css")
        ]
    }
];