/**
 * copy css unminified files into destination folder
 */
const merge = require('webpack-merge');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const WriteJsonPlugin = require('write-json-webpack-plugin');
const ConcatPlugin = require('webpack-concat-plugin');
const CopyPlugin = require('copy-webpack-plugin');
const common = require('./webpack.common.js');
const pkg = require('./package.json');
const BrowserSyncPlugin = require("browser-sync-webpack-plugin");
const appConfig = require('./app.config');
const projectConfig = require("./app/project-config.json");
const srcRoot = appConfig.srcRoot;

const appName = appConfig.appName;
const appVersion = pkg.version;
const distPath = appConfig.distPath.dev;
const crLib = appConfig.crLib;
const webXPanel = appConfig.webXPanel;
const jsList = projectConfig.useWebXPanel ? [...webXPanel, ...crLib] : [...crLib];
const mainTemplateJs = appConfig.mainTemplateJs;
const componentsTemplatejs = appConfig.componentsTemplatejs;
const mainProjectJs = appConfig.mainProjectJs;
const componentsProjectjs = appConfig.componentsProjectjs;
const componentsList = [...mainTemplateJs, ...mainProjectJs, ...componentsTemplatejs, ...componentsProjectjs];

// app version
const appVersionInfo = {};
appVersionInfo.appName = appName;
appVersionInfo.appVersion = appVersion;

module.exports = merge(common("dev"), {
  mode: "development",
  watch: true,
  plugins: [
    new ConcatPlugin({
      uglify: false,
      sourceMap: false,
      name: 'result',
      outputPath: 'libraries/',
      fileName: 'cr-com-lib.js',
      filesToConcat: jsList,
      attributes: {
        async: true
      }
    }),
    new ConcatPlugin({
      uglify: false,
      sourceMap: false,
      name: 'result',
      outputPath: 'libraries/',
      fileName: 'component.js',
      filesToConcat: componentsList,
      attributes: {
        async: true
      }
    }),
    new WriteJsonPlugin({
      object: appVersionInfo,
      path: 'assets/data/',
      filename: 'app.manifest.json',
      flatten: true,
      pretty: true
    }),
    new HtmlWebpackPlugin({
      filename: 'index.html',
      inject: false,
      template: './app/index.html'
    }),
    new BrowserSyncPlugin({
      host: "localhost",
      port: 3000,
      //files under watch
      files: ["./" + distPath + "/**/**/*.html", "./" + distPath + "/**/**/**/*.js", "./" + distPath + "/**/**/**/*.css"],
      server: {
        baseDir: [distPath]
      }
    })
  ]
});