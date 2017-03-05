const glob = require("glob");
const ExtractTextPlugin = require("extract-text-webpack-plugin");

module.exports = [
    {
        name: "js",
        entry: "./js/app.ts",
        output: {
            filename: "./dist/bundle.js"
        },
        devtool: "source-map",
        resolve: {
            extensions: [".webpack.js", ".web.js", ".ts", ".js", ".scss"]
        },
        module: {
            loaders: [
                { test: /\.ts$/, loader: "ts-loader" }
            ]
        }
    },
    {
        name: "css",
        entry: glob.sync("./css/**/*.scss"),
        output: {
            filename: "./dist/bundle.css"
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
            new ExtractTextPlugin("./dist/bundle.css")
        ]
    }
]