﻿var pickedScheduleId = "";


//Schedule
$(function () {
    $('#schedule-date-picker').datetimepicker({
        format: 'L',
        minDate: new Date(),
    });
});

$(document).ready(function () {
    $('#schedule-date-picker').on('change.datetimepicker', function (e) {
        AddDateToModal(e.date)
    })
})

$(function () {
    $('#schedule-time-start-picker').datetimepicker({
        format: 'LT',
    });
})

$(function () {
    $('#schedule-time-stop-picker').datetimepicker({
        format: 'LT'
    });
})

function AddDateToModal(date) {
    document.getElementById("schedule-date-mask").innerHTML = ReturnMask(date);
}

function CheckIfTimezoneIsValidForSchedule() {
    if ($('#header-timezone').val() == "") {
        ShowBannerNotification("schedule-timezone-notification")

        return;
    }

    document.getElementById("schedule-date-mask").innerHTML = ReturnMask(new Date());
    OpenModal("schedule-modal");
}

function ReturnMask(date) {
    var month = moment(String(date)).format("MMM");
    var day = moment(String(date)).format("D");
    var dow = moment(String(date)).format("ddd");

    var dateHTML = ` <div class="text-center" data-target="#schedule-date-picker" data-toggle="datetimepicker">
                        <p id="schedule-dow" class="form-header m-0" style="font-size:18px">${month}</p>
                        <p id="schedule-day" class="form-header m-0" style="font-size:30px">${day}</p>
                        <p id="schedule-dow" class="form-header m-0" style="font-size:18px">${dow}</p>
                    </div>`

    return dateHTML;
}

function SaveScheduleTask(id, type) {
    var form = $('#schedule-modal-form');

    if ($('#schedule-title').val() == "") {
        EnableShowPopover('schedule-title')
        return;
    }
    else if ($('#schedule-subject').val() == "") {
        EnableShowPopover('schedule-subject')
        return;
    }
    else if ($('#schedule-description').val() == "") {
        EnableShowPopover('schedule-description')
        return;
    }
    else if ($('#schedule-time-start-picker-value').val() == "") {
        EnableShowPopover('schedule-time-start-picker-value')
        return;
    }
    else if ($('#schedule-time-stop-picker-value').val() == "") {
        EnableShowPopover('schedule-time-stop-picker-value')
        return;
    }

    var startTime = moment($('#schedule-time-start-picker-value').val(), "HH:mm:ss").format("hh:mm A");
    var stopTime = moment($('#schedule-time-start-picker-value').val(), "HH:mm:ss").format("hh:mm A");

    if (startTime == "Invalid date" || stopTime == "Invalid date") {
        ShowBannerNotification("schedule-modal-check-time-format-notification")
        return;
    }

    var formData = new FormData();
    formData.append("StreamTitle", $('#schedule-title').val());
    formData.append("StreamSubject", $('#schedule-subject').val());
    formData.append("StreamDescription", $('#schedule-description').val());
    formData.append("TimeStart", $('#schedule-time-start-picker-value').val());
    formData.append("TimeStop", $('#schedule-time-stop-picker-value').val());

    var stringD = String($('#schedule-date-picker').datetimepicker('viewDate')).split(" ");
    formData.append("Date", stringD[0] + " " + stringD[1] + " " + stringD[2] + " " + stringD[3] + " " + stringD[4])

    if (id != "") formData.append("Id", id);

    $.ajax({
        url: '/Tutor/TutorDashboard/?handler=SaveScheduleTask',
        type: 'POST',
        datatype: 'json',
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                SortTasks(data);
                if (type != 'edit') DiscardCalendarModalAndCloseModal();
                else {
                    ShowBannerNotification("schedule-modal-notification")
                }
            }
            else {
                ShowBannerNotification("schedule-invalid-date-time-notification")
            }
        }
    })
}

function SortTasks(data) {
    document.getElementById("taskRow").innerHTML = "";
    var element = "";
    if (data.sorted.length > 0) {
        for (var i = 0; i < data.sorted.length; i++) {
            var month = moment(String(data.sorted[i].date).replace("T", " ")).format("MMM");
            var day = moment(String(data.sorted[i].date).replace("T", " ")).format("D");
            var dow = moment(String(data.sorted[i].date).replace("T", " ")).format("ddd");

            element += `<div class="col-lg-6 col-md-12 mt-2">
                            <div class="card h-100 card-border" onmouseover=" $('#edit-schedule-icon-${data.sorted[i].id}').css('display','block')" onmouseout="$('#edit-schedule-icon-${data.sorted[i].id}').css('display','none')" onclick="EditScheduleTask('${data.sorted[i].id}')">
                                <div class="image-container w-100">
                                    <div class="top-right">
                                        <img id="edit-schedule-icon-${data.sorted[i].id}" class="p-1" style="width:30px; cursor:pointer; display:none" src="/images/TutorAssets/TutorDashboard/Edit.png" onclick="EditScheduleTask('${data.sorted[i].id}')" />
                                    </div>
                                </div>
                                <div class="card-body d-lg-block d-md-block d-sm-block d-none" onclick="window.location.href = '/Tutor/TutorStream/${data.sorted[i].id}'; $('#edit-schedule-icon-${data.sorted[i].id}').css('display','none')">
                                    <input type="hidden" id="schedule-date-${data.sorted[i].id}" value="${data.sorted[i].date}" />
                                    <div class="d-inline-flex">
                                        <img class="rounded m-1" src="${data.sorted[i].subjectThumbnail}" style="width:75px; height:75px" />
                                        <div class="text-center m-1 schedule-border" style="width:75px; height:75px;">
                                            <p id="schedule-month-${data.sorted[i].id}" class="form-header mt-4" style="font-size:18px">${month}</p>
                                        </div>
                                        <div class="text-center m-1 schedule-border" style="width:75px; height:75px;">
                                            <p id="schedule-day-${data.sorted[i].id}" class="form-header mb-0 mt-2" style="font-size:22px">${day}</p>
                                            <p id="schedule-dow-${data.sorted[i].id}" class="form-header" style="font-size:14px">${dow}</p>
                                        </div>
                                        <div class="m-1" style="height:75px;">
                                            <p id="schedule-stream-title-${data.sorted[i].id}" class="form-header m-0 schedule-title">${data.sorted[i].streamTitle}</p>
                                            <p id="schedule-stream-subject-${data.sorted[i].id}" class="mt-1 mb-0 schedule-tutor">${data.sorted[i].streamSubject}</p>
                                            <input id="schedule-stream-description-${data.sorted[i].id}" type="hidden" value="${data.sorted[i].streamDescription}">
                                            <p class="mt-1" style="font-size:14px">${data.sorted[i].timeStart} - ${data.sorted[i].timeStop} [${data.sorted[i].timeZone}]</p>
                                            <input type="hidden" id="schedule-time-start-${data.sorted[i].id}" value="${data.sorted[i].timeStart}" />
                                            <input type="hidden" id="schedule-time-stop-${data.sorted[i].id}" value="${data.sorted[i].timeStop}" />
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body d-lg-none d-md-none d-sm-none d-block" onclick="window.location.href = '/Tutor/TutorStream/${data.sorted[i].id}'; $('#edit-schedule-icon-${data.sorted[i].id}').css('display','none')">
                                    <input type="hidden" id="schedule-date-${data.sorted[i].id}" value="${data.sorted[i].date}" />
                                    <div class="d-flex justify-content-center">
                                        <img class="rounded m-1" src="${data.sorted[i].subjectThumbnail}" style="width:75px; height:75px" />
                                        <div class="text-center m-1 schedule-border" style="width:75px; height:75px;">
                                            <p id="schedule-month-${data.sorted[i].id}" class="form-header mt-4" style="font-size:18px">${month}</p>
                                        </div>
                                        <div class="text-center m-1 schedule-border" style="width:75px; height:75px;">
                                            <p id="schedule-day-${data.sorted[i].id}" class="form-header mb-0 mt-2" style="font-size:22px">${day}</p>
                                            <p id="schedule-dow-${data.sorted[i].id}" class="form-header" style="font-size:14px">${dow}</p>
                                        </div>
                                    </div>
                                     <div class="m-1 text-center">
                                        <p id="schedule-stream-title-${data.sorted[i].id}" class="form-header m-0 schedule-title">${data.sorted[i].streamTitle}</p>
                                        <p id="schedule-stream-subject-${data.sorted[i].id}" class="mt-1 mb-0 schedule-tutor">${data.sorted[i].streamSubject}</p>
                                        <input id="schedule-stream-description-${data.sorted[i].id}" type="hidden" value="${data.sorted[i].streamDescription}">
                                        <p class="mt-1" style="font-size:14px">${data.sorted[i].timeStart} - ${data.sorted[i].timeStop} [${data.sorted[i].timeZone}]</p>
                                        <input type="hidden" id="schedule-time-start-${data.sorted[i].id}" value="${data.sorted[i].timeStart}" />
                                        <input type="hidden" id="schedule-time-stop-${data.sorted[i].id}" value="${data.sorted[i].timeStop}" />
                                    </div>
                                </div>
                            </div>
                         </div>`
        }
    }
    else {
        element = `<div class="col-lg-6 col-md-12  mt-2">
                        <div class="card">
                            <div class="card-body">
                                <div class="d-inline-flex">
                                    <img class="rounded m-1 d-block mr-auto ml-auto" src="/images/ScheduleAssets/CalendarAdd.png" style="width:75px; height:75px" onclick="CheckIfTimezoneIsValidForSchedule()"/>
                                    <div class="m-1" style="height:75px;">
                                        <p id="schedule-stream-title" class="form-header m-0">Schedule Stream</p>
                                        <p id="schedule-stream-subject" class="form-header mt-1 mb-0" style="font-size:10px; font-family:'Roboto', serif">Stream Topic</p>
                                        <p id="schedule-stream-time" class="form-header mt-1" style="font-size:12px; font-family:'Roboto', serif">Click "Add Stream" or the plus</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                     </div>`
    }



    document.getElementById("taskRow").innerHTML = element;
}

function EditScheduleTask(id) {
    OpenModal("schedule-modal");

    document.getElementById("schedule-date-mask").innerHTML = ReturnMask($('#schedule-date-' + id).val());
    $('#schedule-date-picker').datetimepicker('viewDate', moment($('#schedule-date-' + id).val()))
    $('#schedule-title').val($('#schedule-stream-title-' + id).text());
    $('#schedule-subject').val($('#schedule-stream-subject-' + id).text());
    $('#schedule-description').val($('#schedule-stream-description-' + id).val());
    $('#schedule-time-start-picker-value').val($('#schedule-time-start-' + id).val());
    $('#schedule-time-stop-picker-value').val($('#schedule-time-stop-' + id).val());
    $('#schedule-time-start-picker').val($('#schedule-time-start-' + id).val());
    $('#schedule-time-stop-picker').val($('#schedule-time-stop-' + id).val());

    document.getElementById("schedule-buttons").innerHTML = `<div class="row">
                                                                <div class="col-6 pr-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#6B6B6B; color:white" onclick="ShowDeleteScheduleTaskBanner('${id}')">Delete Scheduled Stream</button>
                                                                </div>
                                                                <div class="col-6 pl-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white; height:100%" onclick="SaveScheduleTask('${id}', 'edit')">Save Changes</button>
                                                                </div>
                                                             </div>`
}

function ShowDeleteScheduleTaskBanner(id) {
    $('#schedule-modal-delete-task-notification').show()
    document.getElementById("schedule-buttons").innerHTML = `<div class="row">
                                                                <div class="col-6 pr-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#AC0001; color:white" onclick="DeleteScheduleTask('${id}')">Confirm Delete</button>
                                                                </div>
                                                                <div class="col-6 pl-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white; height:100%" onclick="SaveScheduleTask('${id}', 'edit')">Save Changes</button>
                                                                </div>
                                                             </div>`
}

function DeleteScheduleTask(id) {
    $.ajax({
        url: '/Tutor/TutorDashboard/?handler=DeleteScheduleTask',
        type: 'POST',
        datatype: 'json',
        data: {
            'taskId': id,
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                SortTasks(data);
                DiscardCalendarModalAndCloseModal()
            }
        }
    })
}

//Streams
function EditVideo(id) {
    OpenModal("edit-video-modal");

    $('#video-title-edit').val($('#video-title-' + id).text());
    $('#video-description-edit').val($('#video-description-' + id).val());
    document.getElementById("preview-video-thumbnail-edit").src = document.getElementById("video-thumbnail-" + id).src
    document.getElementById("archived-video-edit-buttons").innerHTML = `<div class="row">
                                                                    <div class="col-6 pr-0">
                                                                        <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#6B6B6B; color:white" onclick="ShowDeleteVideoTaskBanner('${id}')">Delete Video</button>
                                                                    </div>
                                                                    <div class="col-6 pl-0">
                                                                        <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="SaveEditedVideo('${id}')">Save Changes</button>
                                                                    </div>
                                                                </div>`
}

function ShowDeleteVideoTaskBanner(id) {
    $('#edit-video-modal-delete-video-notification').show()
    document.getElementById("archived-video-edit-buttons").innerHTML = `<div class="row">
                                                                <div class="col-6 pr-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#AC0001; color:white" onclick="DeleteVideo('${id}')">Confirm Delete</button>
                                                                </div>
                                                                <div class="col-6 pl-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white; height:100%" onclick="SaveEditedVideo('${id}')">Save Changes</button>
                                                                </div>
                                                             </div>`
}

function SaveEditedVideo(id) {
    if ($('#video-title-edit').val() == "") {
        EnableShowPopover('video-title-edit')
        return;
    }
    else if ($('#video-description-edit').val() == "") {
        EnableShowPopover('video-description-edit')
        return;
    }

    $('#edit-video-modal-loader').removeClass('d-none').addClass('d-block');
    var formData = new FormData()

    formData.append("VideoId", id);
    formData.append("VideoTitle", $('#video-title-edit').val());
    formData.append("VideoDescription", $('#video-description-edit').val());

    if (cropper != null) formData.append("VideoThumbnail", cropperBlob);

    cropper = null; //makes sure that cropper object is destroyed once we upload is an image

    $.ajax({
        url: '/Tutor/TutorDashboard/?handler=SaveEditedVideo',
        type: 'POST',
        dataType: 'json',
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                $("#video-title-" + id).text(data.savedInfo[0]); 
                $("#video-description-" + id).val(data.savedInfo[1]) 
                $("#video-thumbnail-" + id).attr('src', data.savedInfo[2] + `?nocache=${new Date().valueOf()}`);

                ShowBannerNotification("edit-video-modal-notification")
            }

            $('#edit-video-modal-loader').removeClass('d-block').addClass('d-none');
        }
    });
}

function DeleteVideo(id) {
    $.ajax({
        url: '/Tutor/TutorDashboard/?handler=DeleteVideo',
        type: 'POST',
        dataType: 'json',
        data: {
            'id': id,
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            DiscardChangesAndCloseModal('edit-video-modal-form', 'edit-video-modal')
            $('#edit-video-modal-delete-video-notification').hide()
            $('#videoInfo-' + id).remove();
            if ($('.video').length < 1) {
                $('#video-row').html(`<div class="col-12 text-center">
                                        <p class="form-sub-header p-0" style="font-size:22px">Looks like you don't have any videos yet...</p>
                                        <p class="form-sub-header p-0" style="font-size:14px; font-family:'Roboto', serif">Help other students find the help they need by putting up content!</p>
                                        <button class="streamWork-primary mt-3" onclick="window.location.href ='/Tutor/TutorStream/SW'" style="font-size:14px; font-family:'Roboto', serif">Start your first stream</button>
                                      </div>`)
            }
        }
    });
}