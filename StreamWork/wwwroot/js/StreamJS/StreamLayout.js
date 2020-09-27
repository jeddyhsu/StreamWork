function ResizeStreamInfoSection() {
    $(document).ready(function () {
        ResizeStreamInfoSectionAlgo()
    })
    $(window).resize(function () {
        ResizeStreamInfoSectionAlgo()
  })
}

function ResizeStreamInfoSectionAlgo() {
    var height = $('#player-row').height();

    if ($(window).width() - 500 < $(window).height()) {
        $('#player').css('height', '100%')
        $('#player').attr('style', 'position: absolute; top: 0; left: 0; width: 100%; height: 100%;')
        $('#player-div').attr('style', 'position: relative; padding-bottom: 56.25%; height: 0;')
    }
    else {
        var height = $('#player-row').height();
        var offest = height + 71;
        $('#player-div').attr('style', '')
        $('#player').attr('style', 'width:100%')
        $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
        console.log("Greater: " + $(window).height() + ": " + ($('#player').height() + height + 71))
    }
}

function ResizeStreamInfoSectionArchivedStreams() {
    $(document).ready(function () {
        ResizeStreamInfoSectionArchivedStreamsAlgo()
    })
    $(window).resize(function () {
        ResizeStreamInfoSectionArchivedStreamsAlgo()
    })
}

function ResizeStreamInfoSectionArchivedStreamsAlgo() {
    var height = $('#player-row').height();

    if ($(window).width() - 500 < $(window).height()) {
        $('#player').css('height', '100%')
        $('#player').attr('style', 'position: absolute; top: 0; left: 0; width: 100%; height: 100%;')
        $('#player-div').attr('style', 'position: relative; padding-bottom: 56.25%; height: 0;')
    }
    else {
        var height = $('#player-row').height();
        var offest = height + 71 + $('#tutor-info').height() + 21;
        $('#player-div').attr('style', '')
        $('#player').attr('style', 'width:100%')
        $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
        console.log("Greater: " + $(window).height() + ": " + ($('#player').height() + height + 71))
    }
}

function ResizeStreamSection() {
$(document).ready(function () {
    var windowWidth = $(window).width();
    if (windowWidth > 991) {
        var width = $('#player-card').width();
        $('#player-card').css('width', (windowWidth - 422) + 'px');
        $('#chat-card').css('min-width', '420px');
        $('#chat-card').css('max-width', '420px');
    }
    else {
        $('#player-card').css('width', '100%');
        $('#chat-card').css('min-width', '100%');
    }
})
  $(window).resize(function () {
    var windowWidth = $(window).width();
    if (windowWidth > 991) {
      var width = $('#player-card').width();
        $('#player-card').css('width', (windowWidth - 422) + 'px');
        $('#chat-card').css('min-width', '420px');
        $('#chat-card').css('max-width', '420px');
    }
    else {
        $('#player-card').css('width', '100%');
        $('#chat-card').css('min-width', '100%');
    }
  })
}