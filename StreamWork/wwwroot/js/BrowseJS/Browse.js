const subjectArray = ["Mathematics", "Science", "Engineering", "Business", "Law", "Art", "Humanities", "Other"]
const options = { //for schedule, videos and streams
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

const options2 = { //for tutors
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
        "topic",
        "name"
    ]
};
var page = "stream";
var parameterTable = {} //saves params in the browse page
parameterTable["Stream"] = ["", ""]
parameterTable["Schedule"] = ["", ""]
parameterTable["Tutor"] = ["", ""]
var streamJson = null;
var videoJson = null;
var scheduleJson = null;
var tutorJson = null;

function ChangePlaceHolder(text) {
    $('#searchQuery').attr('placeholder', text);
}

function SliderStreams() {
    $('#streams-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(20px, 0px, 0px)")
    $('#search-form').attr("onsubmit", "event.preventDefault(); SearchStreams()")
    $('#browse-search').attr("onclick", "SearchStreams()")
    $('#filter').attr("onchange", "Filter()")
    ChangePlaceHolder("Search Streams")
    $('#searchQuery').val((parameterTable["Stream"])[0])
    $('#filter').val((parameterTable["Stream"])[1])
    if ((parameterTable["Stream"])[1] == "") $('#clear-filter').hide();
    else $('#clear-filter').show();
    page = "stream"
}

function SliderSchedule(event) {
    $('#videos-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(110px, 0px, 0px)")
    $('#search-form').attr("onsubmit", "event.preventDefault(); SearchSchedule()")
    $('#browse-search').attr("onclick", "SearchSchedule()")
    $('#filter').attr("onchange", "Filter()")
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
    $('#search-form').attr("onsubmit", "event.preventDefault(); SearchTutors()")
    $('#browse-search').attr("onclick", "SearchTutors()")
    $('#filter').attr("onchange", " Filter()")
    ChangePlaceHolder("Search Tutors")
    $('#searchQuery').val((parameterTable["Tutor"])[0])
    $('#filter').val((parameterTable["Tutor"])[1])
    if ((parameterTable["Tutor"])[1] == "") $('#clear-filter').hide();
    else $('#clear-filter').show();
    page = "tutor"
}

function Filter() {
    if (page == "stream") {
        SearchStreams()
    }
    else if (page == "schedule") {
        SearchSchedule()
    }
    else {
        SearchTutors()
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
        SearchSchedule()
    }
    else {
        SearchTutors()
    }

    $('#clear-filter').hide();
}

function SearchStreams() {
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

            SearchStreamsAlgo()
            SearchVideoAlgo();
        }
    });
}

function SearchStreamsAlgo() {
    var pattern = $('#searchQuery').val();
    var filter = $('#filter').val();

    parameterTable["Stream"] = [pattern, filter]

    if (streamJson == null) return;

    parameterTable["Stream"] = [pattern, filter]

    var fuse = null;
    var output = null;

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

    $('#stream-row').html("");
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

function SearchVideoAlgo() {
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

function SearchSchedule() {
    $.ajax({
        url: '/Home/Browse/SW/?handler=SearchSchedule',
        type: 'POST',
        dataType: 'json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.results.length > 0) scheduleJson = data.results
            SearchScheduleAlgo();
        }
    });
}

function SearchScheduleAlgo() {
    var pattern = $('#searchQuery').val();
    var filter = $('#filter').val();

    parameterTable["Schedule"] = [pattern, filter]

    var fuse = null;
    var output = null;

    if (pattern == "") {  //if pattern is empty then show all videos
        $('#schedule-row').html("");
        for (var i = 0; i < scheduleJson.length; i++) {
            if (filter != "") {
                if (filter != scheduleJson[i].streamSubject) { continue; }
            }
            var schedule = GetScheduleTemplate(scheduleJson[i].subjectThumbnail,
                                               scheduleJson[i].date,
                                               scheduleJson[i].streamTitle,
                                               scheduleJson[i].streamSubject,
                                               scheduleJson[i].timeStart,
                                               scheduleJson[i].timeStop,
                                               scheduleJson[i].timeZone)

            var rowForSchedule = `<div id="schedule-${scheduleJson[i].id}" class="col-lg-6 col-md-12 mt-2 w-100 mb-3 schedule schedule-subject-${schedule[i].streamSubject}">${schedule}</div>`
            $('#schedule-row').append(rowForSchedule);
        }

        if ($('.schedule').length <= 0) $('#schedule-none-found').removeClass('d-none').addClass('d-block');  //notification that says there are no schedule tasks with that search term (check is there are any schedule classes)
        else $('#schedule-none-found').removeClass('d-block').addClass('d-none');

        return;
    }

    fuse = new Fuse(scheduleJson, options);
    output = fuse.search(pattern);

    $('#schedule-row').html("");
    $('#schedule-none-found').removeClass('d-block').addClass('d-none');

    if (output.length > 0) { //if pattern is not empty then show all videos with the fuzzy search
        for (var i = 0; i < output.length; i++) {
            if (filter != "") {
                if (filter != output[i].item.streamSubject) { continue; }
            }
            var schedule = GetScheduleTemplate(output[i].item.subjectThumbnail,
                                               output[i].item.date,
                                               output[i].item.streamTitle,
                                               output[i].item.streamSubject,
                                               output[i].item.timeStart,
                                               output[i].item.timeStop,
                                               output[i].item.timeZone)

            var rowForSchedule = `<div id="schedule-${output[i].item.id}" class="col-lg-6 col-md-12 mt-2 w-100 mb-3 schedule schedule-subject-${output[i].item.streamSubject}">${schedule}</div>`
            $('#schedule-row').append(rowForSchedule);
        }
    }

    if ($('.schedule').length <= 0) $('#schedule-none-found').removeClass('d-none').addClass('d-block');  //notification that says there are no schedule tasks with that search term (check is there are any schedule classes)
    else $('#schedule-none-found').removeClass('d-block').addClass('d-none');
}

function SearchTutors() {
    $.ajax({
        url: '/Home/Browse/SW/?handler=SearchTutors',
        type: 'POST',
        dataType: 'json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.results.length > 0) tutorJson = data.results
            SearchTutorsAlgo();
        }
    });
}

function SearchTutorsAlgo() {
    var pattern = $('#searchQuery').val();
    var filter = $('#filter').val();

    parameterTable["Tutor"] = [pattern, filter]

    var fuse = null;
    var output = null;

    if (pattern == "") {  //if pattern is empty then show all videos
        $('#tutor-row').html("");
        for (var i = 0; i < tutorJson.length; i++) {
            if (filter != "") {
                if (filter != tutorJson[i].streamSubject) { continue; }
            }

             var tutor = GetTutorTemplate(tutorJson[i].username,
                                         tutorJson[i].profileBanner,
                                         tutorJson[i].profilePicture,
                                         tutorJson[i].profileColor,
                                         tutorJson[i].name,
                                         tutorJson[i].profileCaption,
                                         tutorJson[i].topicColor,
                                         tutorJson[i].topic)

            var rowForTutor = `<div id="schedule-${tutorJson[i].id}" class="col-lg-3 col-md-6 col-sm-12 mt-3 mb-3 tutor tutor-subject-${tutorJson[i].topic}">${tutor}</div>`
            $('#tutor-row').append(rowForTutor);
        }

        if ($('.tutor').length <= 0) $('#tutor-none-found').removeClass('d-none').addClass('d-block');  //notification that says there are no schedule tasks with that search term (check is there are any schedule classes)
        else $('#tutor-none-found').removeClass('d-block').addClass('d-none');

        return;
    }

    fuse = new Fuse(tutorJson, options2);
    output = fuse.search(pattern);

    $('#tutor-row').html("");
    $('#tutor-none-found').removeClass('d-block').addClass('d-none');

    if (output.length > 0) { //if pattern is not empty then show all videos with the fuzzy search
        for (var i = 0; i < output.length; i++) {
            if (filter != "") {
                if (filter != output[i].item.topic) { continue; }
            }

            var tutor = GetTutorTemplate(output[i].item.username,
                                         output[i].item.profileBanner,
                                         output[i].item.profilePicture,
                                         output[i].item.profileColor,
                                         output[i].item.name,
                                         output[i].item.profileCaption,
                                         output[i].item.topicColor,
                                         output[i].item.topic)

            var rowForTutor = `<div id="schedule-${tutorJson[i].id}" class="col-lg-3 col-md-6 col-sm-12 mt-3 mb-3 tutor tutor-subject-${tutorJson[i].topic}">${tutor}</div>`
            $('#tutor-row').append(rowForTutor);
        }
    }

    if ($('.tutor').length <= 0) $('#tutor-none-found').removeClass('d-none').addClass('d-block');  //notification that says there are no schedule tasks with that search term (check is there are any schedule classes)
    else $('#tutor-none-found').removeClass('d-block').addClass('d-none');
}
