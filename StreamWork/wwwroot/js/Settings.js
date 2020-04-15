function EditTutorSecurity() {
    var currentPassword = $('#CurrentPassword').val();
    var newPassword = $('#NewPassword').val();
    var confirmPassword = $('#ConfirmPassword').val();

    if (ValidatePassword(newPassword) == false) {
        return;
    }

    if (newPassword != confirmPassword) {
        OpenNotificationModal("Passwords do not match.");
        return;
    }

    $.ajax({
        url: '/Tutor/TutorSettings',
        type: 'post',
        dataType: 'json',
        data: {
            'currentPassword': currentPassword,
            'newPassword': newPassword,
            'confirmPassword': confirmPassword
        },
        success: function (data) {
            if (data.message === "Success") {
                OpenNotificationModalSuccess("Password Changed!");
                $('#CurrentPassword').val("");
                $('#NewPassword').val("");
                $('#ConfirmPassword').val("");
            }
            else {
                OpenNotificationModal("Incorrect Password");
            }
        }
    })
}

function EditStudentSecurity() {

    var currentPassword = $('#CurrentPassword').val();
    var newPassword = $('#NewPassword').val();
    var confirmPassword = $('#ConfirmPassword').val();

    if (ValidatePassword(newPassword) == false) {
        return;
    }

    if (newPassword != confirmPassword) {
        OpenNotificationModal("Passwords do not match.");
        return;
    }

    $.ajax({
        url: '/Student/StudentSettings',
        type: 'post',
        dataType: 'json',
        data: {
            'currentPassword': currentPassword,
            'newPassword': newPassword,
            'confirmPassword': confirmPassword
        },
        success: function(data) {
            if (data.message === "Success") {
                OpenNotificationModalSuccess("Password Changed!");
                $('#CurrentPassword').val("");
                $('#NewPassword').val("");
                $('#ConfirmPassword').val("");
            }
            else {
                OpenNotificationModal("Incorrect Password");
            }
        }
    })
}

function ValidatePassword(password) {
    var UpperCase = false;
    var LowerCase = false;
    var Number = false;
    var passArray = Array.from(password);

    if (passArray.length >= 8) {
        for (i = 0; i < passArray.length; i++) {
            if (passArray[i] >= '0' && passArray[i] <= '9') {
                Number = true;
            }
            else if (passArray[i] == passArray[i].toLowerCase()) {
                LowerCase = true;
            }
            else if (passArray[i] == passArray[i].toUpperCase()) {
                UpperCase = true;
            }
        }

        if (UpperCase == false || LowerCase == false || Number == false) {
            OpenNotificationModal("Password must contain one lowercase letter, one uppercase letter, and one number");
            return false;
        }
    }
    else {
        OpenNotificationModal("Password must be at least 8 characters long");
        return false;
    }

    return true;
}

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