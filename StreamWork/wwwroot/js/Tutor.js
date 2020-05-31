
var oDT = "";
var videoId = "";
var isEdited = false;
var chatId = "";
var chatInfo = ""

//Streaming
function CheckIfStreamIsLive(channelKey, chatid, chatinfo) {
    chatId = chatid;
    chatInfo = chatinfo;

    document.getElementById("ExitStream").style.display = 'none';
    document.getElementById("StartStream").style.display = 'none';
    document.getElementById('loaderStartStream').style.display = 'block'

    $.ajax({
        url: '/Tutor/CheckIfStreamIsLive',
        type: 'post',
        dataType: 'json',
        data: {
            'channelKey': channelKey
        },
        success: function (data) {
            if (data.message === "Success") {
                OpenNotifyStudentsConfirmModal();
            }
            else {
                Hide();
                OpenNotificationModal("Stream is not live, make sure you have started on your encoder!")
            }
        }
    });
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

function RegisterStreamTitleAndStreamSubjectAndCustomThumbanail(notifyStudent) {
    var streamTitle = $('#streamTitle').val();
    var streamSubject = $('#streamSubject').val();
    var streamDescription = $('#streamDescription').val();
        
    var formData = new FormData()

    if (streamTitle == "" || streamSubject == 'Select Subject' || streamDescription == "") {
        Hide();
        OpenNotificationModal("Please give a title, subject and description")
        return;
    }

    var streamInfo = streamTitle + '|' + streamSubject + '|' + streamDescription + '|' + notifyStudent;

    var totalFile = document.getElementById("uploadThumbnail").files.length;
    if (totalFile != 0) formData.append(streamInfo, document.getElementById("uploadThumbnail").files[0])
    else formData.append(streamInfo, 'No Thumbnail');

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
                Hide();
                OpenNotificationModalSuccess("Your broadcast is visible to students!");
                PopoutChat()
            }
            else {
                alert("You must wait at least five minutes in between streams");
            }
        }
    });   

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


//Edit/Delete Streams
function DeleteStream() {
    $.ajax({
        url: '/Tutor/DeleteStream',
        type: 'POST',
        dataType: 'json',
        data: {
            'id': videoId,
        },
        success: function (data) {
            $('#videoInfo-' + videoId).hide();
        }
    });
}

function OpenEditDeleteStreamModal(id, title, description, thumbnail) {
    $('#editDeleteStreamModal').modal('show');
    videoId = id
    if (!isEdited) {
        document.getElementById("streamTitleEdit").value = title;
        document.getElementById("streamDescriptionEdit").value = description;
        document.getElementById("previewThumbnail").src = thumbnail;
    }
}

function OpenDeleteStreamConfirmModal() {
    $('#deleteStreamConfirmModal').modal('show');
}

function SaveEditedStreamInfo() {
    var streamTitle = $('#streamTitleEdit').val();
    var streamDescription = $('#streamDescriptionEdit').val();
    var streamInfo = videoId + "|" + streamTitle + '|' + streamDescription;
    var formData = new FormData()
    var totalFile = document.getElementById("uploadThumbnailEdit").files.length;
    if (totalFile != 0) formData.append(streamInfo, document.getElementById("uploadThumbnailEdit").files[0])
    else formData.append(streamInfo, 'No Thumbnail');

    $.ajax({
        url: '/Tutor/SaveEditedStreamInfo',
        type: 'POST',
        dataType: 'json',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.message === "Success") {
                isEdited = true;
                document.getElementById("streamTitle-" + videoId).innerHTML = data.title;
                document.getElementById("streamThumbnail-" + videoId).src = data.thumbnail;
                document.getElementById("streamTitleEdit").value = data.title;
                document.getElementById("streamDescriptionEdit").value = data.description;
                document.getElementById("previewThumbnail").src = data.thumbnail;
                OpenNotificationModalSuccess("Changes have been saved");
            }
        }
    });
}


//Notifications
function OpenNotificationModal(body) {
    var notification = document.getElementById('notifyBody');
    notification.textContent = body;
    $('#notifyModal').modal('show')
}

function OpenNotificationModalSuccess(body) {
    var notification = document.getElementById('notifyBodySuccess');
    notification.textContent = body;
    $('#notifyModalSuccess').modal('show')
}

//Chat
function PopoutChat() {
    var windowObjectRef;
    var windowFeatures = "menubar=no, toolbar=no,location=yes,resizable=yes,scrollbars=yes,status=yes, width=500, height=600";
    windowObjectRef = window.open('https://www.streamwork.live/chat/streamworkchat?chatId=' + chatId + "&chatInfo=" + chatInfo, 'StreamWork Chat', windowFeatures);
}



