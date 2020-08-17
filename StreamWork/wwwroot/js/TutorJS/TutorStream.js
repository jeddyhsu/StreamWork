var pickedScheduleId = "";

//Streaming
function IsLive(channelKey) {
    document.getElementById("GoLive").disabled = true;

    $.ajax({
        url: '/Tutor/TutorStream/32169?handler=IsLive',
        type: 'post',
        dataType: 'json',
        data: {
            'channelKey': channelKey
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                RegisterStream();
                document.getElementById("GoLive").disabled = true;
            }
            else {
                $("#tutor-stream-error-notification").html(`<b>It looks like you haven't started on your encoder!</b> Please start on your encoder to continue.`)
                ShowBannerNotification("tutor-stream-error-notification");
                document.getElementById("GoLive").disabled = false;
            }
        }
    });
}

function RegisterStream () {
    var formData = new FormData()
    var form = $('#register-stream-form');
    var totalFile = document.getElementById("upload-stream-thumbnail")

    if (!form[0].checkValidity()) {
        form[0].reportValidity();
        return;
    }

    formData.append("StreamTitle", $('#stream-title').val());
    formData.append("StreamSubject", $('#stream-subject').val());
    formData.append("StreamDescription", $('#stream-description').val());
    formData.append("ScheduleId", pickedScheduleId);
    //formData.append("NotifiyStudent", notifyStudent);

    if (totalFile.files.length > 0) {
        formData.append("StreamThumbnail", totalFile.files[0]);
    }
    else {
        $("#tutor-stream-error-notification").html(`<b>Thumbnail required!</b> Thumbnails make your streams more attractive to click on.`)
        ShowBannerNotification("tutor-stream-error-notification");
        return;
    }
        
    $.ajax({
        url: '/Tutor/TutorStream/32169?handler=RegisterStream',
        type: 'post',
        dataType: 'json',
        data: formData,
        contentType: false,
        processData: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                CloseModal('register-stream-modal')
                $('#live-chat').attr('src', '/chat/LiveChat?chatId=' + data.results[1] + '&chatInfo=' + data.results[0])
            }
        }
    });
}

function FillFromSchedule(id) {
    var streamTitle = $('#schedule-stream-title-' + id).text()
    var streamSubject = $('#schedule-stream-subject-' + id).text()
    var streamDescription = $('#schedule-stream-description-' + id).val()

    $('#stream-title').val(streamTitle);
    $('#stream-subject').val(streamSubject);
    $('#stream-description').text(streamDescription);

    $('.schedule-task').css("border-style", "none")
    $('#schedule-task-' + id).css("border-style", "solid")
    $('#schedule-fill-clear').css('display', 'block');

    pickedScheduleId = id;
}

function ClearFill() {
    $('#stream-title').val("");
    $('#stream-subject').val("");
    $('#stream-description').text("");

    $('.schedule-task').css("border-style", "none")
    $('#schedule-fill-clear').css('display', 'none');

    pickedScheduleId = "";
}

function PopoutChat() {
    var windowObjectRef;
    var windowFeatures = "menubar=no, toolbar=no,location=yes,resizable=yes,scrollbars=yes,status=yes, width=500, height=600";
    windowObjectRef = window.open('https://www.streamwork.live/chat/streamworkchat?chatId=' + chatId + "&chatInfo=" + chatInfo, 'StreamWork Chat', windowFeatures);
}