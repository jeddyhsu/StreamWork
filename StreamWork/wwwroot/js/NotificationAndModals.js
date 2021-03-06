﻿function OpenNotificationModal(body, id) {
    var notification = document.getElementById('notification-message');
    notification.textContent = body;
    OpenModal(id);
}

function DiscardCalendarModalAndCloseModal() {
    document.getElementById("schedule-date-mask").innerHTML = `<img class="d-block" src="/images/ScheduleAssets/Calendar.png" data-target="#schedule-date-picker" data-toggle="datetimepicker" />`
    document.getElementById("schedule-buttons").innerHTML = ` <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="SaveScheduleTask()">Save Changes</button>`
    $('#schedule-date-picker').datetimepicker('hide');
    $('#schedule-time-start-picker').datetimepicker('hide');
    $('#schedule-time-stop-picker').datetimepicker('hide');
    $('#schedule-modal-delete-task-notification').hide()
    DiscardChangesAndCloseModal('schedule-modal-form', 'schedule-modal');
}

function DiscardImageAndDiscardFormChangesAndCloseModal(imageURL, imageDestId, formId, modalId) {
    $('#' + imageDestId).attr('src', imageURL);
    DiscardChangesAndCloseModal(formId, modalId);
}

function DiscardChangesAndCloseModal(formId, modalId) {
    $('.popover').popover('hide');
    $('#' + formId).trigger("reset");
    CloseModal(modalId);
}

function OpenModal(modalId) {
    var modal = document.getElementById(modalId);
    modal.style.display = "block";
}

function CloseModal(modalId) {
    var modal = document.getElementById(modalId);
    modal.style.display = "none";
}

function ShowBannerNotification(bannerName) {
    $("#" + bannerName).fadeTo(2500, 500).slideUp(600, function () {
        $("#" + bannerName).slideUp(500);
    });
}

function DisableHidePopover(id) {
    $('#' + id).removeClass('input-invalid');
    $('#' + id).popover('disable');
    $('#' + id).popover('hide');
}

function EnableShowPopover(id) {
    $('#' + id).addClass('input-invalid');
    $('#' + id).popover('enable');
    $('#' + id).popover('show');
}