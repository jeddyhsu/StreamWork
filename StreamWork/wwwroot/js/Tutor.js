
var oDT = "";

//Changes Stream
function RegisterStreamTitleAndStreamSubjectAndCustomThumbanail() {
    var streamTitle = $('#streamTitle').val();
    var streamSubject = $('#streamSubject').val();
    var formData = new FormData()
   
    if (streamTitle == "" || streamSubject == 'Select Subject') {
        alert("You must have title for your stream and you must select a subject")
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
                OpenNotificationModal("Your broadcast is visible to students!");
            }
        }
    });   
}

function ValidateKey() {
    $.ajax({
        url: '/Tutor/TutorStream',
        type: 'post',
        dataType: 'json',
        data: {
            'channelKey': $('#channelKey').val()
        },
        success: function (data) {
            if (data.message === "Success") {
                $('#channelKeyModal').modal('hide')
                $('#registerStreamModal').modal('show')
                OpenNotificationModal("Key validated, welcome StreamTutor")
            }
            else {
                OpenNotificationModal("Invalid channel key, make sure you have entered the channel key correctley")
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
        url: '/Tutor/ProfileTutor',
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
        url: '/Tutor/ProfileTutor',
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
        url: '/Tutor/ProfileTutor',
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
    var dateIn12Days = currentDate.setDate(currentDate.getDate() + 11);

    $('#datetimepickerAdd').datetimepicker({
        minDate: new Date(),
        maxDate: dateIn12Days,
    });

    $('#datetimepickerEdit').datetimepicker({
        minDate: new Date(),
        maxDate: dateIn12Days,
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

function WriteTutorGreeting1() {
    document.getElementById('ProfileCaptionGreeting').style.display = "none";
    document.getElementById('ProfileParagraphGreeting').style.display = "none";
    document.getElementById('ProfileCaptionOnPage').style.display = "block";
    document.getElementById('ProfileParagraphOnPage').style.display = "block";
}




