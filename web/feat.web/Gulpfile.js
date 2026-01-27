"use strict";

const sass = require("gulp-sass")(require('sass'));
const sourcemaps = require('gulp-sourcemaps');

const { task, series, dest, src } = require("gulp");

let paths = {
    src: 'AssetSrc/',
    dist: 'wwwroot/'
}
task('govuk-js', function() {
    return src('node_modules/govuk-frontend/dist/govuk/govuk-frontend.min.js')
        .pipe(dest(paths.dist + 'js'));
});
task('govuk-assets', function() {
    return src('node_modules/govuk-frontend/dist/govuk/assets/**/*')
        .pipe(dest(paths.dist + 'assets'));
});

task('dfe-js', function() {
    return src('node_modules/dfe-frontend/dist/dfefrontend-*.min.js', { sourcemaps: true })
        .pipe(dest(paths.dist + 'js'));
});

task('dfe-assets', function() {
    return src('node_modules/dfe-frontend/packages/assets/**/*', {encoding:false})
        .pipe(dest(paths.dist + 'assets'));
});

task('moj-js', function() {
    return src('node_modules/@ministryofjustice/frontend/moj/moj-frontend.min.js', { sourcemaps: true })
        .pipe(dest(paths.dist + 'js'));
});

task('moj-assets', function() {
    return src('node_modules/@ministryofjustice/frontend/moj/assets/**/*', {encoding:false})
        .pipe(dest(paths.dist + 'assets'));
});

task('moj-css', function() {
    return src('node_modules/@ministryofjustice/frontend/moj/moj-frontend.min.css', { sourcemaps: true })
        .pipe(dest(paths.dist + 'css'));
});

task('autocomplete-js', function() {
    return src('node_modules/accessible-autocomplete/dist/accessible-autocomplete.min.js', { sourcemaps: true })
        .pipe(dest(paths.dist + 'js'));
});

task('autocomplete-css', function() {
    return src('node_modules/accessible-autocomplete/dist/accessible-autocomplete.min.css', { sourcemaps: true })
        .pipe(dest(paths.dist + 'css'));
});

task("sass", function () {
    return src(paths.src + '/scss/*.scss')
        .pipe(sourcemaps.init())
        .pipe(sass({
            // style: 'compressed',
            quietDeps: true
        }).on('error', sass.logError))
        .pipe(dest(paths.dist + '/css'))
    // .pipe(connect.reload());
});

task("images", function() {
    return src(paths.src + '/assets/**/*', {encoding:false})
        .pipe(dest(paths.dist + 'assets'));
})

task("dev", series(
    "govuk-js",
    "govuk-assets",
    "dfe-js",
    "dfe-assets",
    "moj-js",
    "moj-assets",
    "moj-css",
    "autocomplete-js",
    "autocomplete-css",
    "images",
    "sass"     
));

task("default", series("dev"))