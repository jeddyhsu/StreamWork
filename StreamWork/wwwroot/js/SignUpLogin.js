﻿//Handles signing up
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
        alert("Please fill out all fields");
        return;
    }

    if (password != confirmPassword) {
        alert("Passwords do not match");
        return;
    }

    if (ValidateEmail(email) == false) {
        alert("Invalid Email!");
        return
    }

    if (ValidateEmail(payPalAddress) == false) {
        alert("Invalid Email!");
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
    var payPalAddress = $("#payPalAddressT").val();
    var username = $("#usernameT").val();
    var password = $('#passwordT').val();
    var confirmPassword = $('#passwordConfirmT').val();
    var role = 'tutor';

    if (nameFirst == "" || nameLast == "" || email == "" || payPalAddress == "" || username == "" || password == "" || confirmPassword == "") {
        alert("Please fill out all fields");
        return;
    }

    if (password != confirmPassword) {
        alert("Passwords do not match");
        return;
    }

    if (ValidateEmail(email) == false) {
        alert("Invalid Email!");
        return
    }

    if (ValidateEmail(payPalAddress) == false) {
        alert("Invalid Email!");
        return
    }

    if (ValidatePassword(password) == false) {
        return;
    }
    
    if (transcript.length != 1 && resume.length != 1) {
        alert("A transcript and resume are required");
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
            } else if (data.message === "Wrong Password") {
                alert("Passwords do not match");
            } else {
                alert("Username already exists");
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

function ValidateEmail(email) {
    if (email.includes("@")) {
        return true;
    }
    return false
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

function checkLoggedIn(loggedIn, url) {
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


function StartBroadcast(profileType, approvedTutor, loggedIn){
    if(profileType == "tutor" && loggedIn != "" && approvedTutor){
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
    var newPassword = $('#newPassword').val()
    var currentPassword = $('#confirmNewPassword').val()


    if (newPassword != currentPassword) {
        alert("Passwords do not match");
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


