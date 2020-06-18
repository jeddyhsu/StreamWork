//Streaming
function CheckIfStreamIsLive(channelKey, chatid, chatinfo) {
    chatId = chatid;
    chatInfo = chatinfo;

    document.getElementById("GoLive").disabled = true;

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
                OpenNotificationModal("Stream is not live, make sure you have started on your encoder!", 'notificationModal', 'Failed')
                document.getElementById("GoLive").disabled = false;
            }
        }
    });
}

function RegisterStreamTitleAndStreamSubjectAndCustomThumbanail(notifyStudent) {
    var formData = new FormData()
    var streamTitle = $('#streamTitle').val();
    var streamSubject = $('#streamSubject').val();
    var streamDescription = $('#streamDescription').val();
    var totalFile = document.getElementById("uploadThumbnail");

    if (streamTitle == "" || streamSubject == 'Select Subject' || streamDescription == "") {
        OpenNotificationModal("Please give a title, subject and description")
        return;
    }

    formData.append("StreamTitle", streamTitle);
    formData.append("StreamSubject", streamSubject);
    formData.append("StreamDescription", streamDescription);
    formData.append("NotifiyStudent", notifyStudent);
    if (totalFile.files.length > 0)
        formData.append("StreamThumbnail", totalFile.files[0]);

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