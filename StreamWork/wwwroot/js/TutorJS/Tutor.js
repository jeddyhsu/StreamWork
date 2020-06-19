
var oDT = "";
var isEdited = false;
var chatId = "";
var chatInfo = ""


function ReadImageUrl(image, destination) {
    if (image != null && image.files.length > 0) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#' + destination).attr("src", e.target.result)
        }

        reader.readAsDataURL(image.files[0]);
    }
}


function OpenNotifyStudentsConfirmModal() {
    if ($('#notifyStudents').is(':checked')) {
        $('#registerStreamModal').modal('hide');
        $('#notifyStudentsConfirmModal').modal('show');
    }
    else RegisterStreamTitleAndStreamSubjectAndCustomThumbanail("no");
}

function NotifyStudentsConfirm() {
    RegisterStreamTitleAndStreamSubjectAndCustomThumbanail("yes");
}

function NotifyStudentsDecline() {
    RegisterStreamTitleAndStreamSubjectAndCustomThumbanail("no");
}



function Hide() {
    document.getElementById("ExitStream").style.display = 'block';
    document.getElementById("StartStream").style.display = 'block';
    document.getElementById('loaderStartStream').style.display = 'none';
}



//Scehdule
function AddStreamToSchedule() {
    var streamName = $('#streamNameAdd').val();
    var dateTime = $('#dateTimeAdd').val();

    if (streamName == "" || dateTime == "") {
        OpenNotificationModal("Please fill out all fields!");
        return;
    }

    //add date check here!!!!

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

    //add date check here!!!!

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

function OpenRegisterStreamModal() {
    Hide();
    $('#registerStreamModal').modal({
        backdrop: 'static',
        keyboard: false
    });
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


//Tutor Greetings
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


//Recommendations
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

