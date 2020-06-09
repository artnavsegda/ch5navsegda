const path = require('path');
const glob = require('glob');

const appName = `Ch5HorizontalSample`;
const basePath = path.resolve(__dirname);
const distPath = `dist/${appName}`;
const nodeModules = `./node_modules/`;
const srcRoot = `./app/`;
const fontAwesomeCssBasePath = `${nodeModules}@fortawesome/fontawesome-free/css`;
const crLib = glob.sync(`${nodeModules}/@crestron/ch5-crcomlib/build_bundles/umd/cr-com-lib.js`);
const mainJs = glob.sync(`${srcRoot}/assets/vendor/*.js`);
const componentsjs = glob.sync(`${srcRoot}/components/**/*.js`);


module.exports = {
    appName: appName,
    basePath: basePath,
    distPath: distPath,
    nodeModules: nodeModules,
    srcRoot: srcRoot,
    fontAwesomeCssBasePath: fontAwesomeCssBasePath,
    crLib: crLib,
    mainJs: mainJs,
    componentsjs: componentsjs
  };
