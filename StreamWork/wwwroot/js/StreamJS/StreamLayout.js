function ResizeStreamInfoSection() {
  $(window).resize(function () {
    var height = $('#player-row').height();
    var offest = height + 58;
    $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
  })

  var height = $('#player-row').height();
  var offest = height + 58;
  $('#player').css('height', 'calc(100vh - ' + offest + 'px)')
}

function ResizeStreamSection() {
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

  var windowWidth = $(window).width();
  if (windowWidth > 991) {
    var width = $('#player-card').width();
    $('#player-card').css('width', (windowWidth - 422) + 'px');
  }
  else {
    $('#player-card').css('width', '100%');
  }
}