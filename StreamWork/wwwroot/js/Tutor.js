
var oDT = "";
var streamId = "";

function RegisterStreamTitleAndStreamSubjectAndCustomThumbanail() {
    var streamTitle = $('#streamTitle').val();
    var streamSubject = $('#streamSubject').val();
    var formData = new FormData()
   
    if (streamTitle == "" || streamSubject == 'Select Subject') {
        alert("Please select a title and subject!")
        return;
    }

    var streamInfo = streamTitle + '|' + streamSubject;
    var totalFile = document.getElementById("uploadThumbnail").files.length;
    if (totalFile != 0) {
     formData.append(streamInfo, document.getElementById("uploadThumbnail").files[0])
    }
    else {
        formData.append(streamInfo, 'No Thumbnail');
    }

    document.getElementById("StartStream").disabled = true;
    document.getElementById('loaderStartStream').style.display = 'block';

    $.ajax({
        url: '/Tutor/TutorStream',
        type: 'post',
        dataType: 'json',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.message === "Success") {
                 $('#registerStreamModal').modal('hide'),
                 document.getElementById('loaderStartStream').style.display = 'none';
                 alert("Your broadcast is visible to students!");
            }
            else {
                alert("You must wait at least five minutes in between streams");
            }
        }
    });   
}

function CheckIfStreamIsLive(channelKey) {
   

    $.ajax({
        url: '/Tutor/CheckIfStreamIsLive',
        type: 'post',
        dataType: 'json',
        data: {
            'channelKey': channelKey
        },
        success: function (data) {
            if (data.message === "Success") {
                RegisterStreamTitleAndStreamSubjectAndCustomThumbanail();
            }
            else {
                alert("Looks like we can't see your stream preview. Make sure you have started on your encoder!")
            }
        }
    });
}

function AddStreamToSchedule() {
    var streamName = $('#streamNameAdd').val();
    var dateTime = $('#dateTimeAdd').val();

    if (streamName == "" || dateTime == "") {
        OpenNotificationModal("Please fill out all fields!");
        return;
    }

    $.ajax({
        url: '/Tutor/StreamCalendarUtil',
        type: 'post',
        dataType: 'json',
        data: {
            'streamName': streamName,
            'dateTime': dateTime,
            'originalDateTime': null
        },
         success: function (data) {
            if (data.message === "Success") {
                location.reload();
            }
            else {
                OpenNotificationModal("Something went wrong.")
            }
        }
    });
}

function UpdateStreamSchedule() {
    var streamName = $('#streamNameEdit').val();
    var dateTime = $('#dateTimeEdit').val();
    var originalDateTime = odt;

    if (streamName == "" || dateTime == "") {
        OpenNotificationModal("Please fill out all fields!");
        return;
    }

    $.ajax({
        url: '/Tutor/StreamCalendarUtil',
        type: 'post',
        dataType: 'json',
        data: {
            'streamName': streamName,
            'dateTime': dateTime,
            'originalDateTime': originalDateTime
        },
        success: function (data) {
            if (data.message === "Success") {
                location.reload()
            }
            else {
                OpenNotificationModal("Something went wrong.")
            }
        }
    });
}

function RemoveStreamOnSchedule() {
    var streamName = $('#streamNameEdit').val();
    var dateTime = $('#dateTimeEdit').val();
    var originalDateTime = odt;

    $.ajax({
        url: '/Tutor/StreamCalendarUtil',
        type: 'post',
        dataType: 'json',
        data: {
            'streamName': streamName,
            'dateTime': dateTime,
            'originalDateTime': originalDateTime,
            'remove': true
        },
        success: function (data) {
            if (data.message === "Success") {
                location.reload();
            }
            else {
                OpenNotificationModal("Something went wrong.")
            }
        }
    });
}

function OpenEditScheduleModal(name, time, date) {

    var dateTime = date + ' ' + time;
    odt = dateTime;

    document.getElementById('streamNameEdit').value = name;
    document.getElementById('dateTimeEdit').value = dateTime;
    $('#editScheduleModal').modal('show');
}

function OpenAddScheduleModal() {
    $('#addScheduleModal').modal('show');
}

function OpenTutorGreetingModal() {
    $('#tutorGreetingModal').modal('show');
}

$(function () {
    var currentDate = new Date();
    var dateIn7Days = currentDate.setDate(currentDate.getDate() + 7);

    $('#datetimepickerAdd').datetimepicker({
        minDate: new Date(),
        maxDate: dateIn7Days,
    });

    $('#datetimepickerEdit').datetimepicker({
        minDate: new Date(),
        maxDate: dateIn7Days,
    });

});

function PrintLoad() {
    alert("data has loaded!!!")
}

function RetreivePlayer() {
    var myPlayer = dacast.players["135034_c_505911"];
    //var isPlaying = myPlayer.playing();
}

function OpenNotificationModal(body) {
    var notification = document.getElementById('notificationBody');
    notification.textContent = body;
    $('#notificationModal').modal('show')
}

function WriteTutorGreeting() {
    document.getElementById("ProfileCaptionOnPage").style.display = "none";
    document.getElementById('ProfileParagraphOnPage').style.display = "none";
    document.getElementById('ProfileCaptionGreeting').style.display = "block";
    document.getElementById('ProfileParagraphGreeting').style.display = "block";
}

function WriteTutorProfileCaptionAndParagraph() {
    document.getElementById('ProfileCaptionGreeting').style.display = "none";
    document.getElementById('ProfileParagraphGreeting').style.display = "none";
    document.getElementById('ProfileCaptionOnPage').style.display = "block";
    document.getElementById('ProfileParagraphOnPage').style.display = "block";
}

function OpenViewRecommendationsModal() {
    $('#viewRecommendationsModal').modal('show');
    $('#recommendationsBadge').hide();
}

function ClearRecommendation(index, id) {
    $.ajax({
        url: '/Tutor/ClearRecommendation',
        type: 'POST',
        dataType: 'json',
        data: {
            'id': id,
        },
        success: function (data) {
            $('#recommendation-' + index).hide();
        }
    });
}

function DeleteStream() {
    $.ajax({
        url: '/Tutor/DeleteStream',
        type: 'POST',
        dataType: 'json',
        data: {
            'id': streamId,
        },
        success: function (data) {
            $('#videoInfo-' + streamId).hide();
        }
    });
}

function OpenDeleteStreamModal(id) {
    $('#deleteStreamModal').modal('show');
    streamId = id
}




