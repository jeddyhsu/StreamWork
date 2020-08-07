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
    ChangePlaceHolder("🔎  Search Streams")
    page = "stream"
}

function SliderSchedule() {
    $('#videos-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(110px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchSchedule(event)")
    $('#search-form').attr("onsubmit", "SearchSchedule(event)")
    ChangePlaceHolder("🔎  Search Upcoming Streams")
    page = "schedule"
}

function SliderVideos() {
    $('#videos-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(200px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchVideos(event)")
    $('#search-form').attr("onsubmit", "SearchVideos(event)")
    ChangePlaceHolder("🔎  Search Videos")
    page = "video"
}

function SliderTutors() {
    $('#tutors-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(275px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchTutors(event)")
    $('#search-form').attr("onsubmit", "SearchTutors(event)")
    ChangePlaceHolder("🔎  Search Tutors")
}

function FilterByCards(subject) {
    for (var i = 0; i < subjectArray.length; i++) {
        if (subjectArray[i] != subject) $('.' + subjectArray[i] + "-" + page).hide()
        else $('.' + subjectArray[i] + "-" + page).show()
    }
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
            document.getElementById('stream-row').innerHTML = "";
            var element = "";
            if (data.results.length > 0) {
                for (var i = 0; i < data.results.length; i++) {
                    element += `<div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 ${data.results[i].streamSubject}-stream">
                                <div class="card mt-3 border-0" style="border-bottom-left-radius:20px; border-bottom-right-radius:20px; border-top-left-radius:20px; border-top-right-radius:20px">
                                    <div class="card-title">
                                        <a href="/Stream/Live/${data.results[i].username}"><img asp-append-version="true" style="width:100%; height:100%; border-top-left-radius:20px; border-top-right-radius:20px;" src="${data.results[i].streamThumbnail}"></a>
                                    </div>
                                    <div class="card-body pt-0 pb-1">
                                        <h5 class="text-truncate form-header mb-0">${data.results[i].streamTitle}</h5>
                                        <p class="text-truncate form-header" style="font-size:10px">${data.results[i].name.replace('|', ' ')}</p>
                                    </div>
                                    <div class="card-footer" style="background-color:${data.results[i].streamColor}; border-bottom-left-radius:20px; border-bottom-right-radius:20px;"></div>
                                </div>
                            </div>`
                }
            }
            else {
                element = `<div class="col-9" style="height:200px">
                                <p class="form-sub-header pl-3 pr-3 pt-3 pb-0" style="font-size:22px">No live streams with the search term ${searchTerm}.</p>
                                <p class="form-sub-header pl-3 pr-3" style="font-size:14px; font-family:'Roboto', serif">Feel free to look at some past streams in the videos tab.</p>
                           </div>`
            }
            
            document.getElementById('stream-row').innerHTML = element;
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
            document.getElementById('schedule-row').innerHTML = "";
            var element = "";
            if (data.results.length > 0) {
                for (var i = 0; i < data.results.length; i++) {
                    element += `<div class="col-lg-6 col-md-12 mt-2 w-100 ${data.results[i].streamSubject}-schedule">
                                    <div class="card mb-3 border-0">
                                    <div class="card-body">
                                        <div class="d-inline-flex">
                                            <img class="rounded m-1 schedule-image" src="${data.results[i].subjectThumbnail}" />
                                            <div class="text-center m-1 schedule-image schedule-border">
                                                <p class="form-header schedule-month">${moment(data.results[i].date).format('MMM')}</p>
                                            </div>
                                            <div class="text-center m-1 schedule-image schedule-border">
                                                <p class="form-header schedule-day">${moment(data.results[i].date).format('DD')}</p>
                                                <p class="form-header schedule-dow">${moment(data.results[i].date).format('ddd')}</p>
                                            </div>
                                            <div class="m-1" style="height:75px;">
                                                <p class="form-header m-0">${data.results[i].streamTitle}</p>
                                                <p class="mt-1 mb-0 roboto schedule-tutor"><a class="streamWork-green" href="/Profiles/Tutor/${data.results[i].username}">${data.results[i].name.replace('|', ' ')}</a> Tutoring ${data.results[i].streamSubject}</p>
                                                <p class="schedule-time">${data.results[i].timeStart} - ${data.results[i].timeStop} [${data.results[i].timeZone}]</p>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>`
                }
            }
            else {
                element = `<div class="col-9" style="height:200px">
                               <p class="form-sub-header pl-3 pr-3 pt-3 pb-0" style="font-size:22px">No scheduled streams with the search term ${searchTerm}.</p>
                               <p class="form-sub-header pl-3 pr-3" style="font-size:14px; font-family:'Roboto', serif">Feel free to look at some past streams in the videos tab.</p>
                           </div>`
            }

            document.getElementById('schedule-row').innerHTML = element;
        }
    });
}

function SearchVideos(event) {
    event.preventDefault();
    var searchTerm = $('#searchQuery').val();
    var filter = $('#filter').val();
    $.ajax({
        url: '/Home/Browse/?handler=SearchVideos',
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
            document.getElementById('video-row').innerHTML = "";
            var element = "";
            if (data.results.length > 0) {
                for (var i = 0; i < data.results.length; i++) {
                    element += ` <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 ${data.results[i].streamSubject}-video">
                                    <div class="card mt-3 border-0" style="border-bottom-left-radius:20px; border-bottom-right-radius:20px; border-top-left-radius:20px; border-top-right-radius:20px">
                                        <div class="card-title">
                                            <a href="/Stream/Archive/${data.results[i].username}/${data.results[i].streamID}/32169"><img asp-append-version="true" style="width:100%; height:100%; border-top-left-radius:20px; border-top-right-radius:20px;" src="${data.results[i].streamThumbnail}"></a>
                                        </div>
                                        <div class="card-body pt-0 pb-1">
                                            <h5 class="text-truncate form-header mb-0">${data.results[i].streamTitle}</h5>
                                            <input type="hidden" value="${data.results[i].streamDescription}" />
                                            <p class="text-truncate form-header mb-1" style="font-size:12px">${moment(data.results[i].startTime).format("MMM d YYYY")}</p>
                                            <p class="text-truncate form-header" style="font-size:10px">${data.results[i].name.replace("|", " ")}</p>
                                        </div>
                                        <div class="card-footer" style="background-color:${data.results[i].streamColor}; border-bottom-left-radius:20px; border-bottom-right-radius:20px;"></div>
                                    </div>
                                </div>`
                }
            }
            else {
                element = `<div class="col-9" style="height:200px">
                               <p class="form-sub-header pl-3 pr-3 pt-3 pb-0" style="font-size:22px">No videos with the search term ${searchTerm}.</p>
                               <p class="form-sub-header pl-3 pr-3" style="font-size:14px; font-family:'Roboto', serif">Try using a different search term.</p>
                           </div>`
            }
            

            document.getElementById('video-row').innerHTML = element;
        }
    });
}

function SearchTutors(event) {
    //TODO
}

