function search() {
    var searchSubject = $("#searchSubject").val();
    if (searchSubject == "Any") {
        window.location.href = '/Home/Search?q=' + $("#searchQuery").val();
    } else {
        window.location.href = '/Home/Search?s=' + searchSubject + '&q=' + $("#searchQuery").val();
    }
    return false;
}

function searchArchives() {
    var searchSubject = $("#searchSubject").val();
    if (searchSubject == "Any") {
        window.location.href = '/Student/ArchivedStreams?q=' + $("#searchQuery").val();
    } else {
        window.location.href = '/Student/ArchivedStreams?s=' + searchSubject + '&q=' + $("#searchQuery").val();
    }
    return false;
}

function searchStreams() {
    var searchSubject = $("#searchSubject").val();
    if (searchSubject == "Any") {
        window.location.href = '/Student/ProfileStudent?q=' + $("#searchQuery").val();
    } else {
        window.location.href = '/Student/ProfileStudent?s=' + searchSubject + '&q=' + $("#searchQuery").val();
    }
    return false;
}