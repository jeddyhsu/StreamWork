//Handles signing up
function SignUp() {
    $.ajax({
        url: '/Home/SignUp',
        type: 'post',
        dataType: 'json',
        data: {
            'nameFirst': $("#nameFirst").val(),
            'nameLast': $("#nameLast").val(),
            'email': $("#email").val(),
            'username': $("#username").val(),
            'password': $('#password').val(),
            'passwordConfirm': $('#passwordConfirm').val(),
            'channelId': $('#channelId').val(),
            'role': $('#role').val(),
        },
        success: function(data) {
            if (data.message === "Success") {

                window.location.href = '/Home/Login';
            } else if (data.message === "Wrong Password") {

                alert("Passwords do not match");
            } else {
                alert("Username already exsists");
            }
        }
    });
}

// Try to log in with session info
function TryLogin() {
    $.ajax({
        url: '/Home/TryLogin',
        type: 'post',
        dataType: 'json',
        data: {
            'tryLogin': "do"
        },
        success: function(data) {
            var verified = false;
            var profile = false;
            var tutor = false;

            if (data.message === "Welcome") {
                verified = true;
            } else if (data.message == "Welcome, StreamTutor") {
                verified = true;
                tutor = true;
            }

            if (verified) {
                const urlParams = new URLSearchParams(window.location.search);
                var dest = urlParams.get('dest');
                if (dest == '-Home-Profile') {
                    if (tutor) {
                        window.location.href = '/Tutor/ProfileTutor';
                    } else {
                        window.location.href = '/Student/ProfileStudent';
                    }
                } else {
                    window.location.href = dest.split('-').join('/');
                }
            }
        },
    });
}

//Handles logging in
function Login() {
    $.ajax({
        url: '/Home/Login',
        type: 'post',
        dataType: 'json',
        data: {
            'username': $("#username").val(),
            'password': $('#password').val()
        },
        success: function(data) {
            var verified = false;
            var profile = false;
            var tutor = false;

            if (data.message === "Welcome") {
                verified = true;
            } else if (data.message == "Welcome, StreamTutor") {
                verified = true;
                tutor = true;
            } else {
                alert("Wrong Username or Password");
            }

            if (verified) {
                const urlParams = new URLSearchParams(window.location.search);
                var dest = urlParams.get('dest');
                if (dest == '-Home-Profile') {
                    if (tutor) {
                        window.location.href = '/Tutor/ProfileTutor';
                    } else {
                        window.location.href = '/Student/ProfileStudent';
                    }
                } else {
                    window.location.href = dest.split('-').join('/');
                }
            }
        }
    });
}

// 
function checkLoggedIn (url) {
    $.ajax({
        url: '/Home/TryLogin',
        type: 'post',
        dataType: 'json',
        data: {
            'tryLogin': "do"
        },
        success: function(data) {
            var verified = false;
            var profile = false;
            var tutor = false;

            if (data.message == "Welcome" || data.message == "Welcome, StreamTutor") {

            } else {
                window.location.href = '/Home/Login?dest=' + url.split('/').join('-');
            }
        },
    });
}


function RecoverPassword() {
    $.ajax({
        url: '/Home/PasswordRecovery',
        type: 'post',
        dataType: 'json',
        data: {
            'username': $("#username").val(),
        },
        success: function (data) {
            if (data.message === 'Success') {
                alert('Email sent! Check your email for the password')
                window.location.href = '/Home/Login'
            }
            else {
                alert('Error')
            }
        }
    })
}

function ChangePassword() {

    //gets url path with parameters
    var path = (location.pathname+location.search).substr(1)

    $.ajax({
        url: '/Home/ChangePassword',
        type: 'post',
        dataType: 'json',
        data: {
            'newPassword': $('#newPassword').val(),
            'confirmNewPassword': $('#confirmNewPassword').val(),
            'path': path
        },
        success: function (data) {
            if (data.message === 'Success') {
                alert('Password has been changed!')
                window.location.href = '/Home/Login'
            }
            else if (data.message === 'Invalid Password Match') {
                alert('Passwords do not match. Please try again.')
            }
            else {
                alert('Error')
            }
        }
    })
}