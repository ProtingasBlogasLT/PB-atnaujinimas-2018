const gulp      = require("gulp");
const sass      = require("gulp-sass");
const concat    = require("gulp-concat");
const concatCss = require("gulp-concat-css");
const uglify    = require("gulp-uglify");
const imagemin  = require("gulp-imagemin");
const cleanCss  = require("gulp-clean-css");
const rename    = require("gulp-rename");
const del       = require("del");

// copy html files
gulp.task('html', function (done) {
    gulp.src('./src/layout/**/*.html')
        .pipe(gulp.dest('./dist'));
    done();
});

// copy fonts
gulp.task('fonts', function (done) {
    gulp.src('./node_modules/@fortawesome/fontawesome-free/webfonts/*')
        .pipe(gulp.dest('./dist/webfonts'));
    done();
});

// compile sass and minify css
gulp.task('sass', function (done) {
    gulp.src([
        './node_modules/bootstrap/scss/bootstrap.scss',
        './node_modules/@fortawesome/fontawesome-free/scss/fontawesome.scss',
        './node_modules/@fortawesome/fontawesome-free/scss/regular.scss',
        './node_modules/@fortawesome/fontawesome-free/scss/solid.scss',
        './node_modules/@fortawesome/fontawesome-free/scss/brands.scss',
        './src/sass/**/*.scss'
    ])
    .pipe(sass().on('error', sass.logError))
    .pipe(concatCss('styles.css', {'rebaseUrls': false}))
    .pipe(cleanCss())
    .pipe(rename({suffix: '.min'}))
    .pipe(gulp.dest('./dist'));
    done();
});

// join and minify scripts
gulp.task('scripts', function (done) {
    gulp.src([
        './node_modules/jquery/dist/jquery.js',
        './node_modules/popper.js/dist/umd/popper.js',
        './node_modules/bootstrap/dist/js/bootstrap.js',
        './node_modules/@fortawesome/fontawesome-free/js/all.js',
        './src/scripts/main.js'
    ])
    .pipe(concat('scripts.js'))
    //.pipe(uglify())
    .pipe(rename({suffix: '.min'}))
    .pipe(gulp.dest('./dist'));
    done();
});

// watch tasks
gulp.task('watch', function (done) {
    gulp.watch('./src/layout/**/*.html', gulp.series('html'));
    gulp.watch('./src/sass/**/*.scss', gulp.series('sass'));
    gulp.watch('./src/scripts/main.js', gulp.series('scripts'));
    done();
});

// delete everything from dist
gulp.task('delete', function (done) {
    del(['./dist/*']);
    done();
});

// rebuild everything in dist
gulp.task('rebuild', gulp.series('delete', gulp.parallel('html', 'fonts', 'sass', 'scripts')));

// default task
gulp.task('default', gulp.series('watch'));
