const subjectArray = ["Mathematics", "Science", "Engineering", "Business", "Law", "Art", "Humanities", "Other"]
const options = {
    isCaseSensitive: false,
    // includeScore: false,
    shouldSort: true,
    // includeMatches: false,
    // findAllMatches: false,
    // minMatchCharLength: 1,
    // location: 0,
    // threshold: 0.6,
    // distance: 100,
    // useExtendedSearch: false,
    // ignoreLocation: false,
    // ignoreFieldNorm: false,
    keys: [
        "streamTitle",
        "streamDescription",
        "streamSubject",
    ]
};
var page = "stream";
var parameterTable = {} //saves params in the browse page
parameterTable["Stream"] = ["", ""]
parameterTable["Schedule"] = ["", ""]
parameterTable["Tutor"] = ["", ""]
var streamJson = null;
var videoJson = null;

function ChangePlaceHolder(text) {
    $('#searchQuery').attr('placeholder', text);
}

function SliderStreams() {
    $('#streams-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(20px, 0px, 0px)")
    $('#search-form').attr("onsubmit", "SearchStreamsAlgo()")
    $('#filter').attr("onchange", "Filter()")
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

function Filter() {
    if (page == "stream") {
        SearchStreamsAlgo();
        SearchVideoAlgo();
    }
    else if (page == "schedule") {
        SearchSchedule(event)
    }
    else {
        SearchTutors(event)
    }

    $('#clear-filter').show();
}

function ClearSearchAndFilter() {
    $('#searchQuery').val('')
    ClearFilter();
}

function ClearFilter() {
    $('#filter').val('')
    if (page == "stream") {
        SearchStreamsAlgo();
        SearchVideoAlgo();
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
    $.ajax({
        url: '/Home/Browse/SW/?handler=SearchStreams',
        type: 'POST',
        dataType: 'json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.channels.length > 0) streamJson = data.channels
            if (data.videos.length > 0) videoJson = data.videos

            SearchStreamsAlgo(event)
            SearchVideoAlgo(event);
        }
    });
}

function SearchStreamsAlgo(event) {
    if (event != undefined)
        event.preventDefault();

    var pattern = $('#searchQuery').val();
    var filter = $('#filter').val();

    parameterTable["Stream"] = [pattern, filter]

    var fuse = null;
    var output = null;

    if ($('#live-channel-count').val() > 0) {

        if (pattern == "") { //if pattern is empty then show all streams
            $('#stream-row').html("");
            for (var i = 0; i < streamJson.length; i++) {
                if (filter != "") {
                    if (filter != streamJson[i].streamSubject) { continue; }
                }
                var stream = GetStreamTemplate(streamJson[i].id,
                                               streamJson[i].streamSubject,
                                               streamJson[i].streamThumbnail,
                                               streamJson[i].streamTitle,
                                               streamJson[i].streamColor,
                                               streamJson[i].name,
                                               streamJson[i].username)

                var rowForStream = `<div id="stream-${streamJson[i].id}" class="col-xl-3 col-lg-3 col-md-4 col-sm-6 col-12 mb-3 stream stream-subject-${streamJson[i].streamSubject}">${stream}</div>`
                $('#stream-row').append(rowForStream);
            }

            if ($('.stream').length <= 0) $('#stream-none-found').removeClass('d-none').addClass('d-block');  //notification that says there are no videos with that search term (check is there are any video classes)
            else $('#stream-none-found').removeClass('d-block').addClass('d-none');

            return;
        }

        fuse = new Fuse(streamJson, options); //fuzzy search
        output = fuse.search(pattern);

        $('.stream').hide();
        $('#stream-none-found').removeClass('d-block').addClass('d-none');

        if (output.length > 0) {  //if pattern is not empty then show all streams with the fuzzy search
            for (var i = 0; i < output.length; i++) {
                if (filter != "") {
                    if (filter != output[i].item.streamSubject) { continue; }
                }
                var stream = GetStreamTemplate(output[i].item.id,
                                               output[i].item.streamSubject,
                                               output[i].item.streamThumbnail,
                                               output[i].item.streamTitle,
                                               output[i].item.streamColor,
                                               output[i].item.name,
                    output[i].item.username)

                var rowForStream = `<div id="stream-${output[i].item.id}" class="col-xl-3 col-lg-3 col-md-4 col-sm-6 col-12 mb-3 stream stream-subject-${streamJson[i].streamSubject}">${stream}</div>`
                $('#stream-row').append(rowForStream);
            }
        }

        if ($('.stream').length <= 0) $('#stream-none-found').removeClass('d-none').addClass('d-block');  //notification that says there are no videos with that search term (check is there are any video classes)
        else $('#stream-none-found').removeClass('d-block').addClass('d-none');
    }
}

function SearchVideoAlgo() {
    if (event != undefined)
        event.preventDefault();

    var pattern = $('#searchQuery').val();
    var filter = $('#filter').val();

    var fuse = null;
    var output = null;

    if (pattern == "") {  //if pattern is empty then show all videos
        $('#video-row').html("");
        for (var i = 0; i < videoJson.length; i++) {
            if (filter != "") {
                if (filter != videoJson[i].streamSubject) { continue; }
            }
            var video = GetVideoTemplate(videoJson[i].id,
                videoJson[i].streamSubject,
                videoJson[i].streamID,
                videoJson[i].streamThumbnail,
                videoJson[i].streamTitle,
                videoJson[i].streamColor,
                videoJson[i].name,
                videoJson[i].username)

            var rowForVideo = `<div id="video-${videoJson[i].id}" class="col-xl-3 col-lg-3 col-md-4 col-sm-6 col-12 mb-3 video video-subject-${videoJson[i].streamSubject}">${video}</div>`
            $('#video-row').append(rowForVideo);
        }

        if ($('.video').length <= 0) $('#video-none-found').removeClass('d-none').addClass('d-block');  //notification that says there are no videos with that search term (check is there are any video classes)
        else $('#video-none-found').removeClass('d-block').addClass('d-none');

        return;
    }

    fuse = new Fuse(videoJson, options);
    output = fuse.search(pattern);

    $('#video-row').html("");
    $('#video-none-found').removeClass('d-block').addClass('d-none');
    if (output.length > 0) { //if pattern is not empty then show all videos with the fuzzy search
        for (var i = 0; i < output.length; i++) {
            if (filter != "") {
                if (filter != output[i].item.streamSubject) { continue; }
            }
            var video = GetVideoTemplate(output[i].item.id,
                output[i].item.streamSubject,
                output[i].item.streamID,
                output[i].item.streamThumbnail,
                output[i].item.streamTitle,
                output[i].item.streamColor,
                output[i].item.name,
                output[i].item.username)

            var rowForVideo = `<div id="video-${output[i].item.id}" class="col-xl-3 col-lg-3 col-md-4 col-sm-6 col-12 mb-3 video video-subject-${output[i].item.streamSubject}">${video}</div>`
            $('#video-row').append(rowForVideo);
        }
    }

    if ($('.video').length <= 0) $('#video-none-found').removeClass('d-none').addClass('d-block');  //notification that says there are no videos with that search term (check is there are any video classes)
    else $('#video-none-found').removeClass('d-block').addClass('d-none');
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

SearchStreams();
