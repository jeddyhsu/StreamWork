function ResizeStreamInfoSection() {
$(document).ready(function () {
    var height = $('#player-row').height();
    var offest = height + 71;
    $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
})
  $(window).resize(function () {
    var height = $('#player-row').height();
    var offest = height + 71;
    $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
  })
}

function ResizeStreamInfoSectionArchivedStreams() {
    $(document).ready(function () {
        var height = $('#player-row').height();
        var offest = height + 71 + $('#tutor-info').height() + 21;
        $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
    })

    $(window).resize(function () {
        var height = $('#player-row').height();
        var offest = height + 71 + $('#tutor-info').height() + 21;
        $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
    })
}

function ResizeStreamSection() {
$(document).ready(function () {
    var windowWidth = $(window).width();
    if (windowWidth > 991) {
        var width = $('#player-card').width();
        $('#player-card').css('width', (windowWidth - 422) + 'px');
    }
    else {
        $('#player-card').css('width', '100%');
    }
})
  $(window).resize(function () {
    var windowWidth = $(window).width();
    if (windowWidth > 991) {
      var width = $('#player-card').width();
      $('#player-card').css('width', (windowWidth - 422) + 'px');
    }
    else {
      $('#player-card').css('width', '100%');
    }
  })
}