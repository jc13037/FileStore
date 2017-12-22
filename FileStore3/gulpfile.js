/// <binding BeforeBuild='default' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp'),
    cache = require('gulp-cached'); //If cached version identical to current file then it doesn't pass it downstream so this file won't be copied 

//copy node_modules to wwwroot

gulp.task('default', function () {

    try {

        gulp.src('node_modules/bootstrap/**')
            .pipe(cache('lib'))
            .pipe(gulp.dest('wwwroot/lib/bootstrap'));

        gulp.src('node_modules/jquery/**')
            .pipe(cache('lib'))
            .pipe(gulp.dest('wwwroot/lib/jquery'));

        gulp.src('node_modules/jquery-validation/**')
            .pipe(cache('lib'))
            .pipe(gulp.dest('wwwroot/lib/jquery-validation'));

        gulp.src('node_modules/jquery-validation-unobtrusive/**')
            .pipe(cache('lib'))
            .pipe(gulp.dest('wwwroot/lib/jquery-validation-unobtrusive'));

        gulp.src('node_modules/popper.js/**')
            .pipe(cache('lib'))
            .pipe(gulp.dest('wwwroot/lib/popper.js'));

        gulp.src('node_modules/tether/**')
            .pipe(cache('lib'))
            .pipe(gulp.dest('wwwroot/lib/tether'));
    }
    catch (e) {
        return -1;
    }
    return 0;
});