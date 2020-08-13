function ResizeStreamInfoSection() {
    $(window).resize(function () {
        var height = $('#player-row').height();

        if ($(window).height() > 1000) {
            $('#player').css('height', '100%')
            $('#player-div').attr('style', 'position: relative; padding-bottom: 56.25%; height: 0;')
            return;
        }

        if ($('#player').height() + height + 71 >= $(window).height()) {
            var height = $('#player-row').height();
            var offest = height + 71;
            $('#player-div').attr('style', '')
            $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
            console.log("Greater: " + $(window).height() + ": " + ($('#player').height() + height + 71))
        }
        else{
            $('#player').css('height', '100%')
            $('#player-div').attr('style', 'position: relative; padding-bottom: 56.25%; height: 0;')
        }
  })
}

function ResizeStreamInfoSectionArchivedStreams() {
    $(window).resize(function () {
        var height = $('#player-row').height();

        if ($(window).height() > 1000) {
            $('#player').css('height', '100%')
            $('#player-div').attr('style', 'position: relative; padding-bottom: 56.25%; height: 0;')
            return;
        }

        if ($('#player').height() + height + 71 + $('#tutor-info').height() + 21 >= $(window).height()) {
            var height = $('#player-row').height();
            var offest = height + 71 + $('#tutor-info').height() + 21;
            $('#player-div').attr('style', '')
            $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
            console.log("Greater: " + $(window).height() + ": " + ($('#player').height() + height + 71))
        }
        else {
            $('#player').css('height', '100%')
            $('#player-div').attr('style', 'position: relative; padding-bottom: 56.25%; height: 0;')
        }
    })
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