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
            if (data.message === "Welcome") {
                window.location.href = '/Student/ProfileStudent';
            } else if (data.message == "Welcome, StreamTutor") {
                window.location.href = '/Tutor/ProfileTutor';
            } else {
                alert("Wrong Username or Password");
            }
        }
    });
}