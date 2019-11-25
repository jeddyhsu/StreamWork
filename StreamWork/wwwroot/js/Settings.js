﻿
function EditOverview() {

    var name = $('#Name').val();
    var profileCaption = $('#ProfileCaption').val();
    var profileParagraph = $('#ProfileParagraph').val();

    $.ajax({
        url: '/Tutor/TutorSettings',
        type: 'post',
        dataType: 'json',
        data: {
            'name': name,
            'profileCaption': profileCaption,
            'profileParagraph': profileParagraph
        },
        success: function (data) {
            if (data.message === "Success") {
                alert("Profile Saved!");
                location.reload();
            }
        }
    });
}

function EditTutorSecurity() {

    var currentPassword = $('#CurrentPassword').val();
    var newPassword = $('#NewPassword').val();
    var confirmPassword = $('#ConfirmPassword').val();

    if (newPassword != confirmPassword) {
        alert("Passwords do not match.");
        return;
    }

    if (ValidatePassword(newPassword) == false) {
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
        success: function data() {
            if (data.message === "Success") {
                alert("Password Changed!");
                location.reload();
            }
            else {
                alert("Incorrect Password");
            }
        }
    })
}

function EditStudentSecurity() {

    var currentPassword = $('#CurrentPassword').val();
    var newPassword = $('#NewPassword').val();
    var confirmPassword = $('#ConfirmPassword').val();

    if (newPassword != confirmPassword) {
        alert("Passwords do not match.");
        return;
    }

    if (ValidatePassword(newPassword) == false) {
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
        success: function data() {
            if (data.message === "Success") {
                alert("Password Changed!");
                location.reload();
            }
            else {
                alert("Incorrect Password");
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
            alert("Password must contain one lowercase letter, one uppercase letter, and one number");
            return false;
        }
    }
    else {
        alert("Password must be at least 8 characters long");
        return false;
    }

    return true;
}