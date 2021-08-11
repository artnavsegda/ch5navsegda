/**
 * Any changes of the project variables below must be updated in package.json as well
 * appName: Scrips -> cleanjs, cleanjs:start, cleanjs:prod, build:archive
 * distPath (changed for dev): Scrips -> cleanjs:start, clean:start
 * distPath (changed for prod): Scrips -> cleanjs:prod, clean:prod, build:prod, build:archive, build:deploy
 * 
 * No changes should be made to other project variables.
 */
const path = require("path");
const glob = require("glob");

const appName = `Shell`;
const basePath = path.resolve(__dirname);
const nodeModules = `./node_modules/`;
const srcRoot = `./app`;
const srcTemplateRoot = `./app/template`;
const srcProjectRoot = `./app/project`;
const fontAwesomeCssBasePath = `${nodeModules}@fortawesome/fontawesome-free/css`;
const crLib = glob.sync(`${nodeModules}/@crestron/ch5-crcomlib/build_bundles/umd/cr-com-lib.js`);
const webXPanel = glob.sync(`${nodeModules}/@crestron/ch5-webxpanel/dist/umd/index.js`);
const mainTemplateJs = glob.sync(`${srcTemplateRoot}/libraries/*.js`);
const mainProjectJs = glob.sync(`${srcProjectRoot}/libraries/*.js`);
const componentsTemplatejs = glob.sync(`${srcTemplateRoot}/components/**/*.js`);
const componentsProjectjs = glob.sync(`${srcProjectRoot}/components/**/*.js`);

module.exports = {
  appName,
  basePath,
  distPath: {
    dev: 'dist/dev/' + `${appName}`,
    prod: 'dist/prod/' + `${appName}`
  },
  nodeModules,
  srcRoot,
  srcTemplateRoot,
  srcProjectRoot,
  fontAwesomeCssBasePath,
  webXPanel,
  crLib,
  mainTemplateJs,
  componentsTemplatejs,
  mainProjectJs,
  componentsProjectjs
};