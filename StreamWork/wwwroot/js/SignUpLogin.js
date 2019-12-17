//Handles signing up
function SignUpStudent() {

    var nameFirst = $("#nameFirstS").val();
    var nameLast = $("#nameLastS").val();
    var email = $("#emailS").val();
    var payPalAddress = $("#payPalAddressS").val();
    var username = $("#usernameS").val();
    var password = $('#passwordS').val();
    var confirmPassword = $('#passwordConfirmS').val();
    var role = 'student';

    if (nameFirst == "" || nameLast == "" || email == "" || payPalAddress == "" || username == "" || password == "" || confirmPassword == "") {
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

    if (ValidateEmail(payPalAddress) == false) {
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
            'nameFirst': nameFirst,
            'nameLast': nameLast,
            'email': email,
            'payPalAddress': payPalAddress,
            'username': username,
            'password': password,
            'passwordConfirm': confirmPassword,
            'role': role,
        },
        success: function(data) {
            if (data.message === "Success") {

                window.location.href = '/Home/Login';
            } else if (data.message === "Wrong Password") {
                OpenNotificationModal("Passwords do not match")
            } else {
                OpenNotificationModal("Username already exists")
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

    if (transcript.length != 1 && resume.length != 1 && letterOfrec != 1) {
        OpenNotificationModal("A transcript, resume and letter of recommendation are required");
        return;
    }

    formData.append("Transcript", transcript[0]);
    if (letterOfrec.length != 0) {
        formData.append("LetterOfRec", letterOfrec[0]);
    }
    formData.append("Resume", resume[0]);
    formData.append("nameFirst", nameFirst);
    formData.append("nameLast", nameLast);
    formData.append("email", email);
    formData.append("payPalAddress", payPalAddress);
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
            } else {
                OpenNotificationModal("Username already exists")
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

                if (dest.includes("-Home-Subject")) {
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

function StartBroadcast(profileType, approvedTutor, loggedIn){
    if(profileType == "tutor" && loggedIn != "" && approvedTutor){
        window.location.href = "/Tutor/ProfileTutor"
    }
    else{
        window.location.href="Home/Login?dest=-Home-Profile"
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
    var path = (location.pathname + location.search).substr(1);
    var newPassword = $('#newPassword').val()
    var currentPassword = $('#confirmNewPassword').val()


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
            'path': path
        },
        success: function (data) {
            if (data.message === 'Success') {
                OpenNotificationModal('Password has been changed!')
                window.location.href = '/Home/Login'
            }
            else if (data.message === 'Failed') {
                OpenNotificationModal('Passwords do not match. Please try again.')
            }
            else {
                OpenNotificationModal('Error')
            }
        }
    })
}

function OpenNotificationModal(body) {
    var notification = document.getElementById('notificationBody');
    notification.textContent = body;
    $('#notificationModal').modal('show')
}


