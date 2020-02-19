$(function () {
    $('#loaderTutor').hide()
    $('#loaderStudent').hide()
});

//Handles signing up
function SignUpStudent() {
    var nameFirst = $("#nameFirstS").val();
    var nameLast = $("#nameLastS").val();
    var email = $("#emailS").val();
    var username = $("#usernameS").val();
    var password = $('#passwordS').val();
    var confirmPassword = $('#passwordConfirmS').val();
    var role = 'student';

    $('#loaderStudent').show();

    $.ajax({
        url: '/Home/SignUp',
        type: 'post',
        dataType: 'json',
        data: {
            'nameFirst': nameFirst,
            'nameLast': nameLast,
            'email': email,
            'username': username,
            'password': password,
            'passwordConfirm': confirmPassword,
            'role': role,
        },
        success: function(data) {
            if (data.message === "Success") {
                window.location.href = '/Home/Login?dest=-Home-Profile';
                $('#loaderStudent').hide()
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
    var payPalAddress = $("#payPalAddressT").val();
    var username = $("#usernameT").val();
    var password = $('#passwordT').val();
    var confirmPassword = $('#passwordConfirmT').val();
    var role = 'tutor';

    var transcriptUpload = document.getElementById("Transcript");
    var letterOfRecUpload = document.getElementById("LetterOfRec");
    var resumeUpload = document.getElementById("Resume");

    if (transcript.length != 1 || resume.length != 1 || letterOfrec.length != 1) {
        if (transcript.length != 1)
            transcriptUpload.innerHTML == "Transcript is required"
        if (resume.length != 1)
            letterOfRecUpload.innerHTML == "Resume is required"
        if (resume.length != 1)
            resumeUpload.innerHTML == "LOR is required"

        return;
    }

    formData.append("Transcript", transcript[0]);
    formData.append("LetterOfRec", letterOfrec[0]);
    formData.append("Resume", resume[0]);
    formData.append("nameFirst", nameFirst);
    formData.append("nameLast", nameLast);
    formData.append("email", email);
    formData.append("payPalAddress", payPalAddress);
    formData.append("username", username);
    formData.append("password", password);
    formData.append("confirmPassword", confirmPassword);
    formData.append("role", role);

    $('#loaderTutor').show();

    $.ajax({
        url: '/Home/SignUp',
        type: 'post',
        dataType: 'json',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.message === "Success") {
                window.location.href = '/Home/Login?dest=-Home-Profile';
                $('#loaderTutor').hide()
            }
        }
    });
}

function RunStudentChecks() {
    var nameFirst = $("#nameFirstS").val();
    var nameLast = $("#nameLastS").val();
    var email = $("#emailS").val();
    var username = $("#usernameS").val();
    var password = $('#passwordS').val();
    var confirmPassword = $('#passwordConfirmS').val();

    if (nameFirst == "" || nameLast == "" || email == "" || username == "" || password == "" || confirmPassword == "") {
        OpenNotificationModal("Please fill out all fields")
        return;
    }

    if (password != confirmPassword) {
        OpenNotificationModal("Passwords do not match")
        return;
    }

    if (ValidateEmail(email) == false) {
        OpenNotificationModal("Invalid Email")
        return
    }

    if (ValidatePassword(password) == false) {
        return;
    }

    $.ajax({
        url: '/Home/SignUp',
        type: 'post',
        dataType: 'json',
        data: {
            'username': username
        },
        success: function (data) {
            if (data.message === "Success") {
                OpenCheckStudentModal();
            } else {
                OpenNotificationModal("It looks like that username is already taken")
            }
        }
    });
}

function RunTutorChecks() {
    var nameFirst = $("#nameFirstT").val();
    var nameLast = $("#nameLastT").val();
    var email = $("#emailT").val();
    var payPalAddress = $("#payPalAddressT").val();
    var username = $("#usernameT").val();
    var password = $('#passwordT').val();
    var confirmPassword = $('#passwordConfirmT').val();

    if (nameFirst == "" || nameLast == "" || email == "" || payPalAddress == "" || username == "" || password == "" || confirmPassword == "") {
        OpenNotificationModal("Please fill out all fields");
        return;
    }

    if (password != confirmPassword) {
        OpenNotificationModal("Passwords do not match");
        return;
    }

    if (ValidateEmail(email) == false) {
        OpenNotificationModal("Invalid Email");
        return;
    }

    if (ValidateEmail(payPalAddress) == false) {
        OpenNotificationModal("Invalid Email");
        return;
    }

    if (ValidatePassword(password) == false) {
        return;
    }

    $.ajax({
        url: '/Home/SignUp',
        type: 'post',
        dataType: 'json',
        data: {
            'username': username
        },
        success: function (data) {
            if (data.message === "Success") {
                OpenCheckTutorModal();
            } else {
                OpenNotificationModal("It looks like that username is already taken")
            }
        }
    });
}

function ValidatePassword(password){
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

function ValidateEmail(email) {
    if (email.includes("@") && email.includes(".")){
        return true;
    }

    return false
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
        success: function (data) {

            if (data.message === "Failed") {
                OpenNotificationModal("Wrong Username or Password")
                return;
            }

            const urlParams = new URLSearchParams(window.location.search);
            var dest = urlParams.get('dest');
            var subject = urlParams.get('s');

            if (dest != null) {
                if (dest.includes("-Home-Profile")) {
                    if (data.message === "Tutor") {
                        window.location.href = '/Tutor/ProfileTutor'
                        return;
                    }
                    else {
                        window.location.href = '/Student/ProfileStudent'
                        return;
                    }
                }

                if (dest.includes("Home") || dest.includes("StreamViews")) {
                    window.location.href = dest.split('-').join('/');
                    return;
                }

                if (data.message === "Tutor" && dest.includes("Tutor")) {
                    window.location.href = dest.split('-').join('/');
                    return;
                }
                else {
                    window.location.href = '/Student/ProfileStudent'
                    return;
                }

                if (data.message === "Student" && dest.includes("Student")) {
                    window.location.href = dest.split('-').join('/');
                    return;
                }
                else {
                    window.location.href = '/Student/ProfileTutor'
                    return;
                }
            }
        }
    });
}

function NavigateToPage (url) {
    window.location.href = '/Home/Login?dest=' + url.split('/').join('-');
    return
}

function redirect () {
    window.location.href = '/Home/Subscribe';
    return;
}

function StartBroadcast(type) {
    if(type == "tutor")
        window.location.href = '/Tutor/ProfileTutor';
    else if(type == "student")
        window.location.href = '/Student/ProfileStudent';
    else
        window.location.href = '/Home/Login?dest=-Home-Profile'
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
                OpenNotificationModal('Email sent! Check your email to reset your password')
                window.location.href = '/Home/Login'
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
                OpenNotificationModal('Password has been changed!')
                window.location.href = '/Home/Login'
            }
            else if (data.message === 'Failed') {
                OpenNotificationModal('Passwords do not match. Please try again.')
            }
            else if (data.message === 'QueryFailed') {
                OpenNotificationModal('Key failure. Make sure to use the link from te most recent change password email.')
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

function OpenCheckStudentModal() {
    $('#checkStudentModal').modal('show');
}

function OpenNotificationModal(body) {
    var notification = document.getElementById('notificationBody');
    notification.textContent = body;
    $('#notificationModal').modal('show')
}


