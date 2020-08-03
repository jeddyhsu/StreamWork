const subjectArray = ["Mathematics", "Science", "Engineering", "Business", "Law", "Art", "Humanities", "Other"]

function ChangePlaceHolder(text) {
    $('#searchQuery').attr('placeholder', text);
}

function SliderStreams() {
    $('#streams-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(15px, 0px, 0px)")
    ChangePlaceHolder("🔎  Search Streams")
}

function SliderVideos() {
    $('#videos-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(100px, 0px, 0px)")
    ChangePlaceHolder("🔎  Search Videos")
}

function SliderTutors() {
    $('#tutors-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(182px, 0px, 0px)")
    ChangePlaceHolder("🔎  Search Tutors")
}

function FilterByCards(subject) {
    for (var i = 0; i < subjectArray.length; i++) {
        if (subjectArray[i] != subject) $('.' + subjectArray[i]).hide()
        else $('.' + subjectArray[i]).show()
    }
}

