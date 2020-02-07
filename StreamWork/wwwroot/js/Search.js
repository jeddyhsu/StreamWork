function search() {
    var searchSubject = $("#searchSubject").val();
    if (searchSubject == "Any") {
        window.location.href = '/Home/Search?q=' + $("#searchQuery").val();
    } else {
        window.location.href = '/Home/Search?s=' + searchSubject + '&q=' + $("#searchQuery").val();
    }
    return false;
}