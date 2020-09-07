function onSignIn(googleProfile) {
    var currentURL = window.location.pathname
    $.ajax({
        url: '/Home/SignUp/?handler=CheckIfOauthUserExists',
        type: 'POST',
        data: {
            email: googleProfile.getBasicProfile().getEmail(),
            route: currentURL,
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
    }).done(function (data) {
        if (data == null) {
            oauthToken = googleProfile.getAuthResponse().id_token;
            oauthStarted = true;
            goToTab('studentOrTutor');
        }
        else {
            RouteToPage(data)
        }
    });
}

function SignInOauth() {
    var auth2 = gapi.auth2.getAuthInstance();
    auth2.signIn().then(function () {
        var profile = auth2.currentUser.get()
        onSignIn(profile)
        console.log('User signed in.');
    });
}

function SignOutOauth() {
    var auth2 = gapi.auth2.getAuthInstance();
    auth2.signOut().then(function () {
        console.log('User signed out.');
    });
}

function SignIn(route) {
    var formData = new FormData();
    formData.append('Username', $('#username').val())
    formData.append('Password', $('#password').val())
    formData.append('Time', moment().utcOffset())

    var currentURL = document.location.href

    $.ajax({
        url: currentURL + '?handler=SignIn',
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
            RouteToPage(data)
        }
    })
}

function SignInModal(route) {
    var formData = new FormData();
    formData.append('Username', $('#username').val())
    formData.append('Password', $('#password').val())
    formData.append('Time', moment().utcOffset())

    $.ajax({
        url: '/Home/SignIn/SW?handler=SignIn',
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
            if (data.message === "Failed") {
                ShowBannerNotification('invalid-username-password-notification')
            }
            else {
                window.location.href = route;
            }
        }
    })
}

function SignOut() {
    $.ajax({
        url: '/Home/SignIn/SW/?handler=SignOut',
        type: 'POST',
        datatype: 'json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            SignOutOauth()
            window.location.href = "/"
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

function togglePasswordVisibility(field) {
    $('#' + field + '-eye').toggleClass('fa-eye fa-eye-slash');
    let passwordField = $('#' + field);
    if (passwordField.attr('type') == 'password') {
        passwordField.attr('type', 'text');
    } else {
        passwordField.attr('type', 'password');
    }
}

function RouteToPage(data) {
    if (data.message === "Tutor") {
        window.location.href = '/' + data.message + '/' + data.message + 'Dashboard'
    }
    else if (data.message === "Student") {
        window.location.href = '/Home/Browse/SW'
    }
    else if (data.message === "Route") {
        window.location.href = data.route
    }
    else {
        ShowBannerNotification('invalid-username-password-notification')
    }
}