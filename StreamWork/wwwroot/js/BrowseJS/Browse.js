const subjectArray = ["Mathematics", "Science", "Engineering", "Business", "Law", "Art", "Humanities", "Other"]

function ChangePlaceHolder(text) {
    $('#searchQuery').attr('placeholder', text);
}

function SliderStreams() {
    $('#streams-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(15px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchStreams(event, 'stream-row')")
    $('#search-form').attr("onsubmit", "SearchStreams(event, 'stream-row')")
    ChangePlaceHolder("🔎  Search Streams By Title")
}

function SliderVideos() {
    $('#videos-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(100px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchStreams(event, 'video-row')")
    $('#search-form').attr("onsubmit", "SearchStreams(event, 'video-row')")
    ChangePlaceHolder("🔎  Search Videos By Title")
}

function SliderTutors() {
    $('#tutors-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(182px, 0px, 0px)")
    $('#search-form').attr("onkeyup", "SearchStreams(event, 'tutor-row')")
    $('#search-form').attr("onsubmit", "SearchStreams(event, 'tutor-row')")
    ChangePlaceHolder("🔎  Search Tutors By Name")
}

function FilterByCards(subject) {
    for (var i = 0; i < subjectArray.length; i++) {
        if (subjectArray[i] != subject) $('.' + subjectArray[i]).hide()
        else $('.' + subjectArray[i]).show()
    }
}

function SearchStreams(event, id) { //filters by username
    event.preventDefault();
    var searchTerm = $('#searchQuery').val();
    var filter = $('#filter').val();
    $.ajax({
        url: '/Tutor/TutorDashboard/?handler=SearchArchivedStreams',
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
            document.getElementById(id).innerHTML = "";
            var element = "";
            for (var i = 0; i < data.results.length; i++) {
                element += ` <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 ${data.results[i].streamSubject}">
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

            document.getElementById(id).innerHTML = element;
        }
    });
}

