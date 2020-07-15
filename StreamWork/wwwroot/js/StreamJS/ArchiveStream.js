$(window).resize(function () {
    var height = $('#player-row').height();
    var tutorInfoHeight = $('#tutor-info').height();
    var offest = height + 58 + tutorInfoHeight + 21;
    $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
})

$(document).ready(function () {
    var height = $('#player-row').height();
    var tutorInfoHeight = $('#tutor-info').height();
    var offest = height + 58 + tutorInfoHeight + 21;
    $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
})

function SectionShowMore(id) {
    $('#section-' + id).addClass('section-cutoff-all')
    $('#section-sm-btn-' + id).attr('onclick', 'SectionShowLess(' + id + ')');
    $('#section-sm-btn-' + id).text('Show Less');
}

function SectionShowLess(id) {
    $('#section-' + id).removeClass('section-cutoff-all').addClass('section-cutoff')
    $('#section-sm-btn-' + id).attr('onclick', 'SectionShowMore(' + id + ')');
    $('#section-sm-btn-' + id).text('Show More');
}
