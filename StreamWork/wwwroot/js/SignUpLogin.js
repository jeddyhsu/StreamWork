//Handles signing up
function SignUpStudent() {
    $.ajax({
        url: '/Home/SignUp',
        type: 'post',
        dataType: 'json',
        data: {
            'nameFirst': $("#nameFirstS").val(),
            'nameLast': $("#nameLastS").val(),
            'email': $("#emailS").val(),
            'username': $("#usernameS").val(),
            'password': $('#passwordS').val(),
            'passwordConfirm': $('#passwordConfirmS').val(),
            'role': 'student',
        },
        success: function(data) {
            if (data.message === "Success") {

                window.location.href = '/Home/Login';
            } else if (data.message === "Wrong Password") {
                alert("Passwords do not match");
            } else {
                alert("Username already exists");
            }
        }
    });
}

function SignUpTutor() {

    var formData = new FormData();
    var transcript = document.getElementById("uploadTranscript").files;
    var letterOfrec = document.getElementById("uploadLetterofrec").files;
    var resume = document.getElementById("uploadResume").files;
    var nameFirst = $("#nameFirstT").val();
    var nameLast = $("#nameLastT").val();
    var email = $("#emailT").val();
    var username = $("#usernameT").val();
    var password = $('#passwordT').val();
    var confirmPassword = $('#passwordConfirmT').val();
    var role = 'Tutor';

    if (transcript.length != 1 && letterOfrec.length != 1 && resume.length != 1) {
        alert("A Transcript, Resume and Letter Of Recommandation are required");
        return;
    }

    formData.append("Transcript", transcript[0]);
    formData.append("LetterOfRec", letterOfrec[0]);
    formData.append("Resume", resume[0]);
    formData.append("nameFirst", nameFirst);
    formData.append("nameLast", nameLast);
    formData.append("email", email);
    formData.append("username", username);
    formData.append("password", password);
    formData.append("confirmPassword", confirmPassword);
    formData.append("role", role);

    $.ajax({
        url: '/Home/SignUp',
        type: 'post',
        dataType: 'json',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.message === "Success") {

                window.location.href = '/Home/Login';
            } else if (data.message === "Wrong Password") {
                alert("Passwords do not match");
            } else {
                alert("Username already exists");
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

            if (data.message === "Welcome Student") {
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
                if (dest == '-Home-Profile' || !dest) {
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
function checkLoggedIn (loggedIn, url) {
    if (loggedIn != "Logged In") {
        window.location.href = '/Home/Login?dest=' + url.split('/').join('-');
    }
}


function NavigateToPage (url) {
    window.location.href = '/Home/Login?dest=' + url.split('/').join('-');
    return
}

function redirect () {
    window.location.href = '/Home/Subscribe';
    return;
}


function StartBroadcast(profileType, loggedIn){
    if(profileType == "tutor" && loggedIn != ""){
        window.location.href = "/Tutor/ProfileTutor"
    }
    else{
        window.location.href="Home/Login?-Home-Profile"
    }
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
                alert('Email sent! Check your email to reset your password')
                window.location.href = '/Home/Login'
            }
            else {
                alert('Invalid username')
            }
        }
    })
}

function CheckIfStudentOrTutorIsSigningUp() {
    var path = (location.pathname + location.search).substr(1);
    var studentForm = document.getElementById("Student");
    var tutorForm = document.getElementById("Tutor");
    if (path.includes("Student")) {
        tutorForm.style.display = "none";
    }
    else {
        studentForm.style.display = "none";
    }
}

function ReadFile(input, type) {
    var file = input.files[0];
    var transcript = document.getElementById("Transcript");
    var letterOfRec = document.getElementById("LetterOfRec");
    var resume = document.getElementById("Resume");

    if (type == 'transcript') {
        transcript.innerHTML = file.name;
    }
    else if (type == 'letterofrec') {
        letterOfRec.innerHTML = file.name;
    }
    else if (type == 'resume') {
        resume.innerHTML = file.name;
    }
}

function ChangePassword() {

    //gets url path with parameters
    var path = (location.pathname + location.search).substr(1);
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