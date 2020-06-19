function OpenNotificationModal(body, id, type) {
    var notification = document.getElementById('notificationMessage');
    notification.textContent = body;
    if (type == "Failed") document.getElementById("notificationImage").src = "/images/NotificationAssets/Error.png"
    OpenModal(id);
}

function DiscardImageAndDiscardFormChangesAndCloseModal(imageURL, imageDestId, formId, modalId) {
    $('#' + imageDestId).attr('src', imageURL);
    DiscardChangesAndCloseModal(formId, modalId);
}

function DiscardChangesAndCloseModal(formId, modalId) {
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