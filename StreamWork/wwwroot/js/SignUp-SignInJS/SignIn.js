function SignIn() {
    var formData = new FormData();
    formData.append('Username', $('#username').val())
    formData.append('Password', $('#password').val())

    $.ajax({
        url: '/Home/SignIn/?handler=SignIn',
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
            if (data.message === "Tutor" || data.message === "Student") {
                window.location.href = '/' + data.message + '/' + data.message + 'Dashboard'
            }
            else {
                ShowBannerNotification('invalid-username-password-notification')
            }
        }
    })
}

function SignOut() {
    $.ajax({
        url: '/Home/SignIn/?handler=SignOut',
        type: 'POST',
        datatype: 'json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            window.location.href = '/Home/SignIn'
        }
    })
}

function CheckIfBothFieldsAreEmpty() {
    var username = $('#username').val()
    var password = $('#password').val()

    if (username == "" || username == "undefined" || password == "" || password == "undefined") {
        document.getElementById('sign-in-button').disabled = true;
        $('#sign-in-button').addClass('streamWork-disabled mb-3')
    }
    else {
        document.getElementById('sign-in-button').disabled = false;
        $('#sign-in-button').removeClass().addClass('streamWork-primary mb-3')
    }
}

