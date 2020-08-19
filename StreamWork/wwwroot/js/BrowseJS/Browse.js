const subjectArray = ["Mathematics", "Science", "Engineering", "Business", "Law", "Art", "Humanities", "Other"]
var page = "stream";

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
    page = "stream"
}

function SliderSchedule() {
    $('#videos-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(110px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchSchedule(event)")
    $('#search-form').attr("onsubmit", "SearchSchedule(event)")
    $('#filter').attr("onchange", "SearchSchedule(event)")
    ChangePlaceHolder("Search Upcoming Streams")
    page = "schedule"
}

function SliderTutors() {
    $('#tutors-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(196px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchTutors(event)")
    $('#search-form').attr("onsubmit", "SearchTutors(event)")
    $('#filter').attr("onchange", "SearchTutors(event)")
    ChangePlaceHolder("Search Tutors")
    page = "tutor"
}

function SearchStreams(event) {
    event.preventDefault();
    var searchTerm = $('#searchQuery').val();
    var filter = $('#filter').val();
    $.ajax({
        url: '/Home/Browse/?handler=SearchStreams',
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
            if ($('#live-channel-count') > 0) {
                $('.stream').hide();
                $('#stream-none-found').removeClass('d-block').addClass('d-none');
                if (data.streams.length > 0) {
                    for (var i = 0; i < data.streams.length; i++) {
                        $('#stream-' + data.streams[i].id).show();
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
    event.preventDefault();
    var searchTerm = $('#searchQuery').val();
    var filter = $('#filter').val();
    $.ajax({
        url: '/Home/Browse/?handler=SearchSchedule',
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
    event.preventDefault();
    var searchTerm = $('#searchQuery').val();
    var filter = $('#filter').val();
    $.ajax({
        url: '/Home/Browse/?handler=SearchTutors',
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
                    $('#tutor-' + data.results[i].username).show();
                }
            }
            else {
                $('#tutor-none-found').removeClass('d-none').addClass('d-block');
            }
        }
    });
}

