function FilterStreams(subject) {
    var subjectDivs = $('.streamsubject');

    $.each(subjectDivs, function (index, value) {
        $(this).show();
    });

    $.each(subjectDivs, function (index, value) {
        if ($(this).hasClass(subject) == false) {
            $(this).hide();
        }
    });
}