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
    ChangePlaceHolder("Search Streams")
    page = "stream"
}

function SliderSchedule() {
    $('#videos-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(110px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchSchedule(event)")
    $('#search-form').attr("onsubmit", "SearchSchedule(event)")
    ChangePlaceHolder("Search Upcoming Streams")
    page = "schedule"
}

function SliderVideos() {
    $('#videos-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(200px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchVideos(event)")
    $('#search-form').attr("onsubmit", "SearchVideos(event)")
    ChangePlaceHolder("Search Videos")
    page = "video"
}

function SliderTutors() {
    $('#tutors-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(275px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchTutors(event)")
    $('#search-form').attr("onsubmit", "SearchTutors(event)")
    ChangePlaceHolder("Search Tutors")
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
                                <p class="form-sub-header pl-3 pr-3 pt-3 pb-0" style="font-size:22px">No live streams with the currrent search term.</p>
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
            $('.video').hide();
            $('#video-none-found').removeClass('d-block').addClass('d-none');
            if (data.results.length > 0) {
                for (var i = 0; i < data.results.length; i++) {
                    $('#video-' + data.results[i].id).show();
                }
            }
            else {
                $('#video-none-found').removeClass('d-none').addClass('d-block');
            }
        }
    });
}

function SearchTutors(event) {
    //TODO
}

