/// <reference path="/Scripts/jquery-1.7.1.min.js" />
$(function () {
    $('input[type="submit"]', $('#add-speaker')).click(function () {
        var postData = { name: $('input[name="name"]').val(), fame: $('input[name="fame"]').val() };
        $.ajax({
            data: postData,
            type: 'POST',
            url: '/api/speaker',
            success: function (data, textStatus, jqXHR) {
                                    $('<li>').html(data.Name + " (Fame is " + data.Fame + ")")
                                        .appendTo($('#speaker-list'));
                $.getJSON("/api/speaker?callback=?", null,
                    function (speakers) {
                        alert(speakers.length);
                    });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.status == 401)
                    alert('gotta login first');
                else
                    alert('oh phooey. check fiddler');
            }
        });
        return false;
    });
});