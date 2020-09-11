﻿const subjectArray = ["Mathematics", "Science", "Engineering", "Business", "Law", "Art", "Humanities", "Other"]
var page = "stream";
var parameterTable = {} //saves params in the browse page
parameterTable["Stream"] = ["", ""]
parameterTable["Schedule"] = ["", ""]
parameterTable["Tutor"] = ["", ""]

function ChangePlaceHolder(text) {
    $('#searchQuery').attr('placeholder', text);
}

function SliderStreams() {
    $('#streams-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(20px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchStreams(event)")
    $('#search-form').attr("onsubmit", "SearchStreams(event)")
    $('#filter').attr("onchange", "SearchStreams(event)")
    ChangePlaceHolder("Search Streams")
    $('#searchQuery').val((parameterTable["Stream"])[0])
    $('#filter').val((parameterTable["Stream"])[1])
    if ((parameterTable["Stream"])[1] == "") $('#clear-filter').hide();
    else $('#clear-filter').show();
    page = "stream"
}

function SliderSchedule() {
    $('#videos-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(110px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchSchedule(event)")
    $('#search-form').attr("onsubmit", "SearchSchedule(event)")
    $('#filter').attr("onchange", "SearchSchedule(event)")
    ChangePlaceHolder("Search Upcoming Streams")
    $('#searchQuery').val((parameterTable["Schedule"])[0])
    $('#filter').val((parameterTable["Schedule"])[1])
    if ((parameterTable["Schedule"])[1] == "") $('#clear-filter').hide();
    else $('#clear-filter').show();
    page = "schedule"
}

function SliderTutors() {
    $('#tutors-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(196px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchTutors(event)")
    $('#search-form').attr("onsubmit", "SearchTutors(event)")
    $('#filter').attr("onchange", "SearchTutors(event)")
    ChangePlaceHolder("Search Tutors")
    $('#searchQuery').val((parameterTable["Tutor"])[0])
    $('#filter').val((parameterTable["Tutor"])[1])
    if ((parameterTable["Tutor"])[1] == "") $('#clear-filter').hide();
    else $('#clear-filter').show();
    page = "tutor"
}

function ClearSearchAndFilter() {
    $('#searchQuery').val('')
    ClearFilter();
}

function ClearFilter() {
    $('#filter').val('')
    if (page == "stream") {
        SearchStreams(event)
    }
    else if (page == "schedule") {
        SearchSchedule(event)
    }
    else {
        SearchTutors(event)
    }

    $('#clear-filter').hide();
}

function SearchStreams(event) {
    if (event != undefined)
        event.preventDefault();
    var searchTerm = $('#searchQuery').val();
    var filter = $('#filter').val();
    parameterTable["Stream"] = [searchTerm, filter]
    if (filter != "") $('#clear-filter').show();
    $.ajax({
        url: '/Home/Browse/SW/?handler=SearchStreams',
        type: 'POST',
        dataType: 'json',
        data: {
            'searchTerm': searchTerm,
            'filter': filter,
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if ($('#live-channel-count').val() > 0) {
                $('.stream').hide();
                $('#stream-none-found').removeClass('d-block').addClass('d-none');
                if (data.channels.length > 0) {
                    for (var i = 0; i < data.channels.length; i++) {
                        $('#stream-' + data.channels[i].id).show();
                    }
                }
                else {
                    $('#stream-none-found').removeClass('d-none').addClass('d-block');
                }
            }
            
            $('.video').hide();
            $('#video-none-found').removeClass('d-block').addClass('d-none');
            if (data.videos.length > 0) {
                for (var i = 0; i < data.videos.length; i++) {
                    $('#video-' + data.videos[i].id).show();
                }
            }
            else {
                $('#video-none-found').removeClass('d-none').addClass('d-block');
            }
        }
    });
}

function SearchSchedule(event) {
    if (event != undefined)
        event.preventDefault();
    var searchTerm = $('#searchQuery').val();
    var filter = $('#filter').val();
    parameterTable["Schedule"] = [searchTerm, filter]
    if (filter != "") $('#clear-filter').show();
    $.ajax({
        url: '/Home/Browse/SW/?handler=SearchSchedule',
        type: 'POST',
        dataType: 'json',
        data: {
            'searchTerm': searchTerm,
            'filter': filter,
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            $('.schedule').hide();
            $('#schedule-none-found').removeClass('d-block').addClass('d-none');
            if (data.results.length > 0) {
                for (var i = 0; i < data.results.length; i++) {
                    $('#schedule-' + data.results[i].id).show();
                }
            }
            else {
                $('#schedule-none-found').removeClass('d-none').addClass('d-block');
            }
        }
    });
}

function SearchTutors(event) {
    if (event != undefined)
        event.preventDefault();
    var searchTerm = $('#searchQuery').val();
    var filter = $('#filter').val();
    parameterTable["Tutor"] = [searchTerm, filter]
    if (filter != "") $('#clear-filter').show();
    $.ajax({
        url: '/Home/Browse/SW/?handler=SearchTutors',
        type: 'POST',
        dataType: 'json',
        data: {
            'searchTerm': searchTerm,
            'filter': filter,
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            $('.tutor').hide();
            $('#tutor-none-found').removeClass('d-block').addClass('d-none');
            if (data.results.length > 0) {
                for (var i = 0; i < data.results.length; i++) {
                    $('#tutor-' + data.results[i].id).show();
                }
            }
            else {
                $('#tutor-none-found').removeClass('d-none').addClass('d-block');
            }
        }
    });
}

function SearchAll(event) {
    SearchStreams(event)
    SearchSchedule(event)
    SearchTutors(event)
}

