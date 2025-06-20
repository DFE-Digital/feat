"use strict";

const sass = require("gulp-sass")(require('sass'));
const { task, series, dest, src, parallel } = require("gulp");
    


let paths = {
    src: 'AssetSrc/',
    dist: 'wwwroot/'
}

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

task("sass", function () {
    return src(paths.src + '/scss/**/*.scss')
        .pipe(sass({
            style: 'development',
            quietDeps: true,
        }).on('error', sass.logError))
        .pipe(dest(paths.dist + '/css'))
    // .pipe(connect.reload());
});

task("images", function() {
    return src(paths.src + '/assets/**/*', {encoding:false})
        .pipe(dest(paths.dist + 'assets'));
})

task("dev", series(
        "govuk-assets",
        "dfe-js",
        "dfe-assets",
        "moj-js",
        "moj-assets",
        "images",
        "sass"     
    ));

task("default", series("dev"))