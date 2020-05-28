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
    var college = $('#collegeS').val();
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
            'college': college,
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
    var resume = document.getElementById("uploadResume").files;
    var nameFirst = $("#nameFirstT").val();
    var nameLast = $("#nameLastT").val();
    var email = $("#emailT").val();
    var payPalAddress = $("#payPalAddressT").val();
    var username = $("#usernameT").val();
    var password = $('#passwordT').val();
    var confirmPassword = $('#passwordConfirmT').val();
    var college = $('#collegeT').val();
    var role = 'tutor';

    formData.append("Transcript", transcript[0]);
    formData.append("Resume", resume[0]);
    formData.append("nameFirst", nameFirst);
    formData.append("nameLast", nameLast);
    formData.append("email", email);
    formData.append("payPalAddress", payPalAddress);
    formData.append("username", username);
    formData.append("password", password);
    formData.append("confirmPassword", confirmPassword);
    formData.append("college", college);
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
        OpenNotificationModal("Fill out all required fields (College is optional)")
        return;
    }

    if (password != confirmPassword) {
        OpenNotificationModal("Passwords do not match")
        return;
    }

    if (ValidateEmail(email) == false) {
        OpenNotificationModal("Invalid email")
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
            'username': username,
            'email': email,
        },
        success: function (data) {
            if (data.message === "Success") {
                OpenCheckStudentModal();
            }
            else if (data.message === "UsernameExists") {
                OpenNotificationModal("Username is taken")
            }
            else if (data.message === "EmailExists") {
                OpenNotificationModal("The email that you have provided is already being used with another account")
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
    var college = $('#collegeT').val();
    var transcript = document.getElementById("uploadTranscript").files;
    var resume = document.getElementById("uploadResume").files;

    if (nameFirst == "" || nameLast == "" || email == "" || payPalAddress == "" || username == "" || password == "" || confirmPassword == "" || college == "") {
        OpenNotificationModal("Fill out all fields");
        return;
    }

    if (password != confirmPassword) {
        OpenNotificationModal("Passwords do not match");
        return;
    }

    if (ValidateEmail(email) == false) {
        OpenNotificationModal("Invalid email");
        return;
    }

    if (ValidateEmail(payPalAddress) == false) {
        OpenNotificationModal("Invalid PayPal email");
        return;
    }

    if (ValidatePassword(password) == false) {
        return;
    }

    if (transcript.length != 1 || resume.length != 1) {
        OpenNotificationModal("Please provide both a transcript and resume");
        return;
    }

    $.ajax({
        url: '/Home/SignUp',
        type: 'post',
        dataType: 'json',
        data: {
            'username': username,
            'email': email,
            'payPalAddress': payPalAddress

        },
        success: function (data) {
            if (data.message === "Success") {
                SignUpTutor();
            }else if (data.message === "UsernameExists") {
                OpenNotificationModal("Username is taken")
            }
            else if (data.message === "EmailExists") {
                OpenNotificationModal("The email that you have provided is already being used with another account")
            }
            else if (data.message === "PayPalEmailExists") {
                OpenNotificationModal("The PayPal email that you have provided is already being used with another account")
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
            if (data.message === "Failed") {
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
                        window.location.href = '/Tutor/ProfileTutor'
                        return;
                    }
                    else {
                        window.location.href = '/Student/ProfileStudent'
                        return;
                    }
                }

                if (modifiedURL.includes("Home") || modifiedURL.includes("StreamViews")) {
                    window.location.href = modifiedURL.split('-').join('/');
                }

                if (data.message === "Tutor") {
                    window.location.href = '/Tutor/ProfileTutor'
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

function NavigateToPage (url) {
    window.location.href = '/Home/Login?dest=' + url.split('/').join('-');
    return
}

function redirect () {
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
            else if (data.message === 'QueryFailed'){
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


