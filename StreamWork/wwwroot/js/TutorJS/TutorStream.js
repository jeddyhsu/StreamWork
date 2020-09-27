var pickedScheduleId = "";

$(document).ready(function () {
    $('#frame').contents().find('.liveOfflineMsg').html('<div>Live-stream offline</div>');
});

//Streaming
function IsLive(channelKey, event) {
    event.preventDefault()
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
            }
            else {
                $("#tutor-stream-error-notification").html(`<b>It looks like you haven't started on your encoder!</b> Please start on your encoder to continue.`)
                ShowBannerNotification("tutor-stream-error-notification");
                document.getElementById("GoLive").disabled = false;
            }
        }
    });
}

function RegisterStream() {
    document.getElementById("GoLive").disabled = true;
    var formData = new FormData()
    var form = $('#register-stream-form');
    var totalFile = document.getElementById("upload-stream-thumbnail")

    if ($('#stream-title').val() == "") {
        EnableShowPopover('stream-title')
        document.getElementById("GoLive").disabled = false;
        return;
    }
    else if ($('#stream-subject').val() == "") {
        EnableShowPopover('stream-subject')
        document.getElementById("GoLive").disabled = false;
        return;
    }
    else if ($('#stream-description').val() == "") {
        EnableShowPopover('stream-description')
        document.getElementById("GoLive").disabled = false;
        return;
    }

    formData.append("StreamTitle", $('#stream-title').val());
    formData.append("StreamSubject", $('#stream-subject').val());
    formData.append("StreamDescription", $('#stream-description').val());
    formData.append("ScheduleId", pickedScheduleId);
    formData.append("NotifyStudent", $('#defaultCheck1').is(":checked"));

    if (cropper != null) {
        formData.append("StreamThumbnail", cropperBlob);
        cropper = null; //makes sure that cropper object is destroyed once we upload is an image
    }
    else {
        $("#tutor-stream-error-notification").html(`<b>Thumbnail required!</b> Thumbnails make your streams more attractive to click on.`)
        ShowBannerNotification("tutor-stream-error-notification");
        document.getElementById("GoLive").disabled = false;
        return;
    }

    $('#GoLive').html('Establishing Connection <div class="loader ml-2"></div>')
        
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
                OpenModal('stream-connection-established-modal');
                $('#GoLive').removeClass('d-flex').addClass('d-none');
                $('#stream-status').text('Live To Viewers')
                $('#live-chat').attr('src', '/Chat/LiveChat/' + data.results[0])
                $('#stream-title').attr('readonly', 'readonly');
                $('#stream-subject').attr('readonly', 'readonly');
                $('#stream-description').attr('readonly', 'readonly');
                $('#upload-stream-thumbnail-button').hide();
                $('#upload-stream-thumbnail-button-info').hide()
                $('#stream-schedule-row').hide()
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
    $('#stream-description').val(streamDescription);

    $('.schedule-task').css("border-style", "none")
    $('#schedule-task-' + id).css("border-style", "solid")
    $('#schedule-fill-clear').removeClass('d-none').addClass('d-inline-block')

    FillMask(id)

    pickedScheduleId = id;
}

function FillMask(id) {
    var x = $('#schedule-mask');
    var y = $('#schedule-task-' + id).html()
    $('#schedule-mask').html($('#schedule-task-' + id).html())

    pickedScheduleId = id;
}

function ClearFill() {
    $('#stream-title').val("");
    $('#stream-subject').val("");
    $('#stream-description').val("");

    $('.schedule-task').css("border-style", "none")
    $('#schedule-fill-clear').removeClass('d-inline-block').addClass('d-none')

    $('#schedule-mask').html(`<div class="card-body">
                                <div>
                                    <p id="schedule-stream-title" class="form-header m-0">Select a stream from your calendar.</p>
                                    <p id="schedule-stream-subject" class="form-sub-header p-0 mt-1 mb-0" style="font-size:10px; font-family:'Roboto', serif">This will automatically fill in the information with the information that you used when scheduling the stream.</p>
                                </div>
                              </div>`)

    pickedScheduleId = "";
}

function PopoutChat() {
    var windowObjectRef;
    var windowFeatures = "menubar=no, toolbar=no,location=yes,resizable=yes,scrollbars=yes,status=yes, width=500, height=600";
    windowObjectRef = window.open('https://www.streamwork.live/chat/streamworkchat?chatId=' + chatId + "&chatInfo=" + chatInfo, 'StreamWork Chat', windowFeatures);
}

function ShowQRCode(event) {
    event.preventDefault()
    $('#qr-code').show()
    $('#hide-qr-code').show()
}

function HideQRCode() {
    $('#qr-code').hide()
    $('#hide-qr-code').hide()
}