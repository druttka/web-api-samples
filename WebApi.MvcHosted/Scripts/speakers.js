﻿/// <reference path="/Scripts/jquery-1.7.1.min.js" />

// TODO: This is very sloppy just to facilitate the samples and should be brought up to standard.
$(function () {
    $('input[type="submit"]', $('#add-speaker')).click(function () {
        var postData = { name: $('input[name="name"]').val(), fame: $('input[name="fame"]').val() };
        $.ajax({
            beforeSend: function (jqXHR) {
                jqXHR.setRequestHeader('Authorization', 'Token token="foobarbaz"');
            },
            data: postData,
            type: 'POST',
            url: '/api/speaker',
            success: function (data, textStatus, jqXHR) {
                $('<li>').html(data.Name + " (Fame is " + data.Fame + ")")
                    .appendTo($('#speaker-list'));

                $.ajax({
                    beforeSend: function (jqXHR) {
                        //jqXHR.setRequestHeader('Authorization', 'Token token="foobarbaz"');
                    },
                    dataType: 'jsonp',
                    url: '/api/speaker',
                    type: 'GET',
                    success: function (speakers) {
                        console.log(speakers.length);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (jqXHR.status === 401)
                            console.log('gotta login first');
                        else
                            console.log('oh phooey. check fiddler');
                    }
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.status === 401)
                    console.log('gotta login first');
                else
                    console.log('oh phooey. check fiddler');
            }
        });
        return false;
    });
});