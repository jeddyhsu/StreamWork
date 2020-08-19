var pickedScheduleId = "";


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
    if (!form[0].checkValidity()) {
        return form[0].reportValidity();
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
                            <div class="card card-border" onmouseover=" $('#edit-schedule-icon-${data.sorted[i].id}').css('display','block')" onmouseout="$('#edit-schedule-icon-${data.sorted[i].id}').css('display','none')" onclick="EditScheduleTask('${data.sorted[i].id}')">
                                <div class="image-container w-100">
                                    <div class="top-right">
                                        <img id="edit-schedule-icon-${data.sorted[i].id}" class="p-1" style="width:30px; cursor:pointer; display:none" src="/images/TutorAssets/TutorDashboard/Edit.png" onclick="EditScheduleTask('${data.sorted[i].id}')" />
                                    </div>
                                </div>
                                <div class="card-body" onclick="window.location.href = '/Tutor/TutorStream/${data.sorted[i].id}'; $('#edit-schedule-icon-${data.sorted[i].id}').css('display','none')">
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
                                            <p id="schedule-stream-title-${data.sorted[i].id}" class="form-header m-0">${data.sorted[i].streamTitle}</p>
                                            <p id="schedule-stream-subject-${data.sorted[i].id}" class="mt-1 mb-0" style="font-size:10px">${data.sorted[i].streamSubject}</p>
                                            <input id="schedule-stream-description-${data.sorted[i].id}" type="hidden" value="${data.sorted[i].streamDescription}">
                                            <p class="mt-1" style="font-size:14px">${data.sorted[i].timeStart} - ${data.sorted[i].timeStop} [${data.sorted[i].timeZone}]</p>
                                            <input type="hidden" id="schedule-time-start-${data.sorted[i].id}" value="${data.sorted[i].timeStart}" />
                                            <input type="hidden" id="schedule-time-stop-${data.sorted[i].id}" value="${data.sorted[i].timeStop}" />
                                        </div>
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
function EditStream(id) {
    OpenModal("edit-stream-modal");

    $('#stream-title-edit').val($('#stream-title-' + id).text());
    $('#stream-description-edit').val($('#stream-description-' + id).val());
    document.getElementById("preview-stream-thumbnail-edit").src = document.getElementById("stream-thumbnail-" + id).src
    document.getElementById("archived-stream-edit-buttons").innerHTML = `<div class="row">
                                                                    <div class="col-6 pr-0">
                                                                        <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#6B6B6B; color:white" onclick="ShowDeleteStreamTaskBanner('${id}')">Delete Stream</button>
                                                                    </div>
                                                                    <div class="col-6 pl-0">
                                                                        <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="SaveEditedStream('${id}')">Save Changes</button>
                                                                    </div>
                                                                </div>`
}

function ShowDeleteStreamTaskBanner(id) {
    $('#edit-stream-modal-delete-stream-notification').show()
    document.getElementById("archived-stream-edit-buttons").innerHTML = `<div class="row">
                                                                <div class="col-6 pr-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#AC0001; color:white" onclick="DeleteStream('${id}')">Confirm Delete</button>
                                                                </div>
                                                                <div class="col-6 pl-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white; height:100%" onclick="SaveEditedStream('${id}')">Save Changes</button>
                                                                </div>
                                                             </div>`
}

function SaveEditedStream(id) {
    var formData = new FormData()
    var totalFile = document.getElementById("upload-thumbnail-edit")

    formData.append("StreamId", id);
    formData.append("StreamTitle", $('#stream-title-edit').val());
    formData.append("StreamDescription", $('#stream-description-edit').val());
    if (totalFile.files.length > 0) formData.append("StreamThumbnail", totalFile.files[0]);

    $.ajax({
        url: '/Tutor/TutorDashboard/?handler=SaveEditedStream',
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
                $("#stream-title-" + id).text(data.savedInfo[0]); 
                $("#stream-description-" + id).val(data.savedInfo[1]) 
                $("#stream-thumbnail-" + id).attr('src', data.savedInfo[2] + `?nocache=${new Date().valueOf()}`);

                ShowBannerNotification("edit-stream-modal-notification")
            }
        }
    });
}

function DeleteStream(id) {
    $.ajax({
        url: '/Tutor/TutorDashboard/?handler=DeleteStream',
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
            DiscardChangesAndCloseModal('edit-stream-modal-form', 'edit-stream-modal')
            $('#edit-stream-modal-delete-stream-notification').hide()
            $('#streamInfo-' + id).hide();
        }
    });
}