//Streaming
function IsLive(channelKey) {
    document.getElementById("GoLive").disabled = true;

    $.ajax({
        url: '/Tutor/TutorStream?handler=IsLive',
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
                document.getElementById("GoLive").disabled = false;
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
        url: '/Tutor/TutorStream?handler=RegisterStream',
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
                $('#registerStreamModal').modal('hide'),
                OpenNotificationModalSuccess("Your broadcast is visible to students!");
                PopoutChat()
            }
        }
    });
}

function PopoutChat() {
    var windowObjectRef;
    var windowFeatures = "menubar=no, toolbar=no,location=yes,resizable=yes,scrollbars=yes,status=yes, width=500, height=600";
    windowObjectRef = window.open('https://www.streamwork.live/chat/streamworkchat?chatId=' + chatId + "&chatInfo=" + chatInfo, 'StreamWork Chat', windowFeatures);
}