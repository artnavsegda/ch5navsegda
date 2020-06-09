
const merge = require('webpack-merge');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const WriteJsonPlugin = require('write-json-webpack-plugin');
const ConcatPlugin = require('webpack-concat-plugin');
const common = require('./webpack.common.js');
const pkg = require('./package.json');
// config details
const appConfig = require('./app.config');
const appName = appConfig.appName;
const appVersion = pkg.version;
const crLib = appConfig.crLib;
const mainJs = appConfig.mainJs;
const componentsjs = appConfig.componentsjs;
const jsList = [...crLib];
const componentsList = [...mainJs, ...componentsjs];

// app version
const appVersionInfo = {};
appVersionInfo.appName = appName;
appVersionInfo.appVersion = appVersion;

module.exports = merge(common, {
  mode: 'production',
  plugins: [
    new ConcatPlugin({
      uglify: true,
      sourceMap: false,
      name: 'result',
      outputPath: 'assets/vendor/',
      fileName: 'cr-com-lib.js',
      filesToConcat: jsList,
      attributes: {
          async: true
      }
  }),
  new WriteJsonPlugin({
    object: appVersionInfo,
    path: 'assets/data/',
    filename: 'app.manifest.json',
    flatten:true,
    pretty: true
  }),
  new ConcatPlugin({
      uglify: true,
      sourceMap: false,
      name: 'result',
      outputPath: 'assets/vendor/',
      fileName: 'component.js',
      filesToConcat: componentsList,
      attributes: {
          async: true
      }
  }),
    new HtmlWebpackPlugin({
      filename: 'index.html',
      inject: false,
      template: './app/index.html'
    })
  ]
});