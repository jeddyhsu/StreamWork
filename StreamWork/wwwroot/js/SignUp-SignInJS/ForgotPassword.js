var forgotPasswordTabs = [
    'username-tab',
    'changePassword-tab',
    'passwordChanged-tab'
]

function goToForgotPasswordTab(tab) {
    for (let i of forgotPasswordTabs) {
        $('#' + i).hide();
    }
    $('#' + tab).show();
}

function usernameTabUpdateNext() {
    if ($('#username').val().length == 0) {
        $('#username-tab-next').removeClass('streamWork-primary');
        $('#username-tab-next').addClass('streamWork-disabled');
    } else {
        $('#username-tab-next').removeClass('streamWork-disabled');
        $('#username-tab-next').addClass('streamWork-primary');
    }
}

function usernameTabNext() {
    if ($('#username').val().length > 0) {
        $.ajax({
            url: '/Home/ForgotPassword/?handler=SendChangePasswordEmail',
            type: 'GET',
            datatype: 'json',
            data: {
                username: $('#username').val()
            }
        }).done(function () {
            goToForgotPasswordTab('changePassword-tab');
        });
    }
}

function changePasswordTabUpdateNext() {
    $('#changePasswordCode').removeClass('input-invalid');
    $('#changePasswordCode').popover('hide');
    $('#changePasswordCode').popover('disable');
    $('#password').removeClass('input-invalid');
    $('#password').popover('hide');
    $('#password').popover('disable');
    $('#confirmPassword').removeClass('input-invalid');
    $('#confirmPassword').popover('hide');
    $('#confirmPassword').popover('disable');
    if ($('#changePasswordCode').val().length == 0 || $('#password').val().length == 0 || $('#confirmPassword').val().length == 0) {
        $('#changePassword-tab-next').removeClass('streamWork-primary');
        $('#changePassword-tab-next').addClass('streamWork-disabled');
    } else {
        $('#changePassword-tab-next').removeClass('streamWork-disabled');
        $('#changePassword-tab-next').addClass('streamWork-primary');
    }
}

function changePasswordTabNext() {
    if ($('#changePasswordCode').val().length > 0 &&
        $('#password').val().length > 0 &&
        $('#confirmPassword').val().length > 0) {
        $.ajax({
            url: '/Home/ForgotPassword/?handler=CheckChangePasswordCode',
            type: 'GET',
            data: {
                username: $('#username').val(),
                changePasswordCode: $('#changePasswordCode').val()
            }
        }).done(function (data) {
            if (data) {
                if (/^(?=.*[0-9])(?=.*[A-Za-z]).{8,}$/.test($('#password').val())) {
                    if ($('#password').val() === $('#confirmPassword').val()) {
                        $.ajax({
                            url: '/Home/ForgotPassword/?handler=ChangePassword',
                            type: 'GET',
                            data: {
                                username: $('#username').val(),
                                changePasswordCode: $('#changePasswordCode').val(),
                                password: $('#password').val()
                            }
                        }).done(function () {
                            $('#changePasswordCode').removeClass('input-invalid');
                            $('#changePasswordCode').popover('hide');
                            $('#changePasswordCode').popover('disable');
                            $('#password').removeClass('input-invalid');
                            $('#password').popover('hide');
                            $('#password').popover('disable');
                            $('#confirmPassword').removeClass('input-invalid');
                            $('#confirmPassword').popover('hide');
                            $('#confirmPassword').popover('disable');
                            goToForgotPasswordTab('passwordChanged-tab')
                        });
                    } else {
                        $('#confirmPassword').addClass('input-invalid');
                        $('#confirmPassword').popover('enable');
                        $('#confirmPassword').popover('show');
                    }
                } else {
                    $('#password').addClass('input-invalid');
                    $('#password').popover('enable');
                    $('#password').popover('show');
                }
            } else {
                $('#changePasswordCode').addClass('input-invalid');
                $('#changePasswordCode').popover('enable');
                $('#changePasswordCode').popover('show');
            }
        });
    }
}