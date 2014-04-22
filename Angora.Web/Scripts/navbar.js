$(function () {
    $.get('/account/username', null, function (data) {
        $('#nav-bar-user-name').prepend(data);
    });
});