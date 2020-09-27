//Student SignUp
function SignUpStudent() {
    var form = $('#formSignUpS');
    if (!form[0].checkValidity()) {
        form[0].reportValidity();
        return;
    }

    if ($('#passwordS').val() != $('#confirmPasswordS').val()) {
        OpenNotificationModal("Passwords do not match");
        return;
    }

    document.getElementById("loaderStudent").style.display = 'block';

    var formData = new FormData();
    formData.append("FirstName", $('#firstNameS').val());
    formData.append("LastName", $('#lastNameS').val());
    formData.append("EmailAddress", $('#emailAddressS').val());
    formData.append("Username", $('#usernameS').val());
    formData.append("Password", $('#passwordS').val());
    formData.append("College", $('#collegeS').val());
    formData.append("Role", 'student');

    $.ajax({
        url: '/Home/SignUp',
        type: 'post',
        dataType: 'json',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.message === "UsernameExists") {
                OpenNotificationModal("Username is taken")
            }
            else if (data.message === "EmailExists") {
                OpenNotificationModal("The email that you have provided is already being used with another account")
            }
            else {
                window.location.href = '/Home/Login?dest=-Home-Profile';
            }

            document.getElementById("loaderStudent").style.display = 'none';
        }
    });
}

//Tutor SignUp
function SignUpTutor() {
    var form = $('#formSignUpT');
    if (!form[0].checkValidity()) {
        form[0].reportValidity();
        return;
    }

    if ($('#passwordT').val() != $('#confirmPasswordT').val()) {
        OpenNotificationModal("Passwords do not match");
        return;
    }

    document.getElementById("loaderTutor").style.display = 'block';

    var formData = new FormData();
    formData.append("Transcript", document.getElementById("uploadTranscript").files[0]);
    formData.append("Resume", document.getElementById("uploadResume").files[0]);
    formData.append("FirstName", $('#firstNameT').val());
    formData.append("LastName", $('#lastNameT').val());
    formData.append("EmailAddress", $('#emailAddressT').val());
    formData.append("PayPalAddress", $('#payPalAddressT').val());
    formData.append("Username", $('#usernameT').val());
    formData.append("Password", $('#passwordT').val());
    formData.append("College", $('#collegeT').val());
    formData.append("Role", 'tutor');

    $.ajax({
        url: '/Home/SignUp',
        type: 'post',
        dataType: 'json',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.message === "UsernameExists") {
                OpenNotificationModal("Username is taken")
            }
            else if (data.message === "EmailExists" || data.message == "PayPalEmailExists") {
                OpenNotificationModal("The email that you have provided is already being used with another account")
            }
            else {
                window.location.href = '/Home/Login?dest=-Home-Profile';
            }

            document.getElementById("loaderTutor").style.display = 'none';
        }
    });
}


//Handles logging in
function Login() {
    var username = $("#username").val();
    var password = $('#password').val();

    if (username == "" || password == "") {
        OpenNotificationModal("Fill out all fields")
        return;
    }

    $.ajax({
        url: '/Home/Login',
        type: 'post',
        dataType: 'json',
        data: {
            'username': $("#username").val(),
            'password': $('#password').val()
        },
        success: function (data) {
            if (data.message === "WrongUsernameOrPassword") {
                OpenNotificationModal("Wrong username or password")
                return;
            }

            var url = window.location.search;
            var modifiedURL = "";
            if (url.includes("dest")) {
                modifiedURL = url.replace("?dest=", "");
            }

            if (modifiedURL != null) {
                if (modifiedURL.includes("-Home-Profile")) {
                    if (data.message === "Tutor") {
                        window.location.href = '/Tutor/TutorDashboard'
                        return;
                    }
                    else {
                        window.location.href = '/Student/ProfileStudent'
                        return;
                    }
                }

                if (modifiedURL.includes("Home") || modifiedURL.includes("StreamViews")) {
                    window.location.href = modifiedURL.split('-').join('/');
                    return;
                }

                if (data.message === "Tutor") {
                    window.location.href = '/Tutor/TutorDashboard'
                    return;
                }

                if (data.message === "Student") {
                    window.location.href = '/Student/ProfileStudent'
                    return;
                }
            }
        }
    });
}

function NavigateToPage(url) {
    window.location.href = '/Home/Login?dest=' + url.split('/').join('-');
    return
}

function redirect() {
    window.location.href = '/Home/Subscribe';
    return;
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
                OpenNotificationModalSuccess('Email sent! Check your email to reset your password')
            }
            else {
                OpenNotificationModal('Invalid username')
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

    var newPassword = $('#newPassword').val();
    var currentPassword = $('#confirmNewPassword').val();

    const urlParams = new URLSearchParams(window.location.search);

    if (newPassword != currentPassword) {
        OpenNotificationModal("Passwords do not match");
        return;
    }

    if (ValidatePassword(newPassword) == false) {
        return;
    }

    $.ajax({
        url: '/Home/ChangePassword',
        type: 'post',
        dataType: 'json',
        data: {
            'newPassword': $('#newPassword').val(),
            'confirmNewPassword': $('#confirmNewPassword').val(),
            'username': urlParams.get('username'),
            'key': urlParams.get('key')
        },
        success: function (data) {
            if (data.message === 'Success') {
                OpenNotificationModalSuccess('Password has been changed!')
                window.location.href = '/Home/Login?dest=-Home-Profile';
            }
            else if (data.message === 'Failed') {
                OpenNotificationModal('Passwords do not match. Please try again.')
            }
            else if (data.message === 'QueryFailed') {
                OpenNotificationModal('')
            }
            else {
                OpenNotificationModal('Error')
            }
        }
    })
}

function OpenCheckTutorModal() {
    $('#checkTutorModal').modal('show');
}

function OpenLoginSignUpModal() {
    $('#loginSignUpModal').modal('show');
}

function OpenLogoutModal() {
    $('#logoutModal').modal('show');
}

function OpenCheckStudentModal() {
    $('#checkStudentModal').modal('show');
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


