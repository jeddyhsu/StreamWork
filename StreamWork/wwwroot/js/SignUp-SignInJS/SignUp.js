var subjects = 0;
var transcriptUploaded = false;
var resumeUploaded = false;
var oauthStarted = false;
var oauthToken = "";
var profileType = "";

function goToTab(tab) {
    for (let i = 1; i <= 10; i++) {
        $('#tab-' + i).hide();
    }
    $('#tab-' + tab).show();
}

function tab1UpdateNext() {
    $('#emailAddress').removeClass('input-invalid');
    $('#emailAddress').popover('hide');
    $('#emailAddress').popover('disable');
    $('#emailAddress-wrapper').popover('hide');
    $('#emailAddress-wrapper').popover('disable');
    if ($('#emailAddress').val().length == 0) {
        $('#tab-1-next').removeClass('streamWork-primary');
        $('#tab-1-next').addClass('streamWork-disabled');
    } else {
        $('#tab-1-next').removeClass('streamWork-disabled');
        $('#tab-1-next').addClass('streamWork-primary');
    }
}

function tab1Next() {
    if ($('#emailAddress').val().length > 0) {
        $.ajax({
            url: '/Home/SignUp/?handler=IsAddressValid',
            type: 'GET',
            datatype: 'json',
            data: {
                emailAddress: $('#emailAddress').val()
            }
        }).done(function (data) {
            if (data) {
                $.ajax({
                    url: '/Home/SignUp/?handler=IsAddressAvailable',
                    type: 'GET',
                    datatype: 'json',
                    data: {
                        emailAddress: $('#emailAddress').val()
                    }
                }).done(function (data) {
                    if (data) {
                        $('#emailAddress').removeClass('input-invalid');
                        $('#emailAddress').popover('hide');
                        $('#emailAddress').popover('disable');
                        $('#emailAddress-wrapper').popover('hide');
                        $('#emailAddress-wrapper').popover('disable');
                        goToTab(2);
                    } else {
                        $('#emailAddress').addClass('input-invalid');
                        $('#emailAddress-wrapper').popover('enable');
                        $('#emailAddress-wrapper').popover('show');
                    }
                });
            } else {
                $('#emailAddress').addClass('input-invalid');
                $('#emailAddress').popover('enable');
                $('#emailAddress').popover('show');
            }
        });
    }
}

function setInCollege(inCollege) {
    let primary;
    let secondary;
    if (inCollege) {
        primary = $('#tab-2-inCollege');
        secondary = $('#tab-2-inHighSchool');
        $('#tab-2-schoolType').text('College / University Name');
    } else {
        primary = $('#tab-2-inHighSchool');
        secondary = $('#tab-2-inCollege');
        $('#tab-2-schoolType').text('High School Name');
    }

    primary.addClass('streamWork-primary').removeClass('streamWork-secondary');
    secondary.addClass('streamWork-secondary').removeClass('streamWork-primary');

    let isTutor = $("input[name='profile-type']:checked").val()
    if (isTutor === 'true' || isTutor === 'false') {
        $('#tab-2-bottom').show();
    }
}

function setIsTutor(isTutor) {
    if (isTutor) {
        $('#tab-2-next').text('Sign Up As a Tutor');
    } else {
        $('#tab-2-next').text('Sign Up As a Student');
    }

    if ($('#tab-2-inCollege').hasClass('streamWork-primary') || $('#tab-2-inHighSchool').hasClass('streamWork-primary')) {
        $('#tab-2-bottom').show();
    }
}

function tab2UpdateNext() {
    if ($('#schoolName').val().length == 0) {
        $('#tab-2-next').removeClass('streamWork-primary');
        $('#tab-2-next').addClass('streamWork-disabled');
    } else {
        $('#tab-2-next').removeClass('streamWork-disabled');
        $('#tab-2-next').addClass('streamWork-primary');
    }
}

function tab2Next() {
    if ($('#schoolName').val().length > 0) {
        if ($("input[name='profile-type']:checked").val() === 'true') {
            if (oauthStarted) {
                goToTab(10);
            }
            else {
                goToTab(6);
            }
        } else {
            goToTab(3);
        }
    }
}

function toggleSubject(subject) {
    let button = $('#tab-3-' + subject);
    if (button.hasClass('streamWork-primary-rect')) {
        button.removeClass('streamWork-primary-rect');
        button.addClass('streamWork-secondary-rect');
        subjects -= 1;
    } else {
        button.removeClass('streamWork-secondary-rect');
        button.addClass('streamWork-primary-rect');
        subjects += 1;
    }

    $('#tab-3-selected').text('Selected: ' + subjects);

    if (subjects >= 1) {
        $('#tab-3-next').removeClass('streamWork-disabled');
        $('#tab-3-next').addClass('streamWork-primary');
    } else {
        $('#tab-3-next').removeClass('streamWork-primary');
        $('#tab-3-next').addClass('streamWork-disabled');
    }
}

function goToTab3Tab1() {
    $('#tab-3-tab-2').hide();
    $('#tab-3-tab-1').show();
}

function goToTab3Tab2() {
    $('#tab-3-tab-1').hide();
    $('#tab-3-tab-2').show();
}

function tab3Next() {
    if (subjects >= 1) {
        if (oauthStarted) goToTab(9);
        else goToTab(4);
    }
}

function tab4UpdateNext() {
    $('#student-username').removeClass('input-invalid');
    $('#student-username').popover('hide');
    $('#student-username').popover('disable');
    $('#student-username-wrapper').popover('hide');
    $('#student-username-wrapper').popover('disable');
    $('#student-password').removeClass('input-invalid');
    $('#student-password').popover('hide');
    $('#student-password').popover('disable');
    $('#student-confirmPassword').removeClass('input-invalid');
    $('#student-confirmPassword').popover('hide');
    $('#student-confirmPassword').popover('disable');
    if ($('#student-firstName').val().length == 0 ||
        $('#student-lastName').val().length == 0 ||
        $('#student-username').val().length == 0 ||
        $('#student-password').val().length == 0 ||
        $('#student-confirmPassword').val().length == 0) {
        $('#tab-4-next').removeClass('streamWork-primary');
        $('#tab-4-next').addClass('streamWork-disabled');
    } else {
        $('#tab-4-next').removeClass('streamWork-disabled');
        $('#tab-4-next').addClass('streamWork-primary');
    }
}

function tab4Next() {
    if ($('#student-firstName').val().length > 0 &&
        $('#student-lastName').val().length > 0 &&
        $('#student-username').val().length > 0 &&
        $('#student-password').val().length > 0 &&
        $('#student-confirmPassword').val().length > 0) {
        if (/^[A-Za-z0-9_-]+$/.test($('#student-username').val())) {
            $.ajax({
                url: '/Home/SignUp/?handler=IsUsernameAvailable',
                type: 'GET',
                data: {
                    username: $('#student-username').val()
                }
            }).done(function (data) {
                if (data) {
                    const re = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/
                    if (re.test($('#student-password').val())) {
                        if ($('#student-password').val() === $('#student-confirmPassword').val()) {
                            $('#student-username').removeClass('input-invalid');
                            $('#student-username').popover('hide');
                            $('#student-username').popover('disable');
                            $('#student-username-wrapper').popover('hide');
                            $('#student-username-wrapper').popover('disable');
                            $('#student-password').removeClass('input-invalid');
                            $('#student-password').popover('hide');
                            $('#student-password').popover('disable');
                            $('#student-confirmPassword').removeClass('input-invalid');
                            $('#student-confirmPassword').popover('hide');
                            $('#student-confirmPassword').popover('disable');
                            signUpStudent();
                        } else {
                            $('#student-confirmPassword').addClass('input-invalid');
                            $('#student-confirmPassword').popover('enable');
                            $('#student-confirmPassword').popover('show');
                        }
                    } else {
                        $('#student-password').addClass('input-invalid');
                        $('#student-password').popover('enable');
                        $('#student-password').popover('show');
                    }
                } else {
                    $('#student-username').addClass('input-invalid');
                    $('#student-username-wrapper').popover('enable');
                    $('#student-username-wrapper').popover('show');
                }
            });
        } else {
            $('#student-username').addClass('input-invalid');
            $('#student-username').popover('enable');
            $('#student-username').popover('show');
        }
    }
}

function tab6UpdateNext() {
    $('#tutor-username').removeClass('input-invalid');
    $('#tutor-username').popover('hide');
    $('#tutor-username').popover('disable');
    $('#tutor-username-wrapper').popover('hide');
    $('#tutor-username-wrapper').popover('disable');
    $('#tutor-password').removeClass('input-invalid');
    $('#tutor-password').popover('hide');
    $('#tutor-password').popover('disable');
    $('#tutor-confirmPassword').removeClass('input-invalid');
    $('#tutor-confirmPassword').popover('hide');
    $('#tutor-confirmPassword').popover('disable');
    if ($('#tutor-firstName').val().length == 0 ||
        $('#tutor-lastName').val().length == 0 ||
        $('#tutor-username').val().length == 0 ||
        $('#tutor-password').val().length == 0 ||
        $('#tutor-confirmPassword').val().length == 0) {
        $('#tab-6-next').removeClass('streamWork-primary');
        $('#tab-6-next').addClass('streamWork-disabled');
    } else {
        $('#tab-6-next').removeClass('streamWork-disabled');
        $('#tab-6-next').addClass('streamWork-primary');
    }
}

function tab6Next() {
    if ($('#tutor-firstName').val().length > 0 &&
        $('#tutor-lastName').val().length > 0 &&
        $('#tutor-username').val().length > 0 &&
        $('#tutor-password').val().length > 0 &&
        $('#tutor-confirmPassword').val().length > 0) {
        const re = /^[A-Za-z0-9_-]+$/
        if (re.test($('#tutor-username').val())) {
            $.ajax({
                url: '/Home/SignUp/?handler=IsUsernameAvailable',
                type: 'GET',
                data: {
                    username: $('#tutor-username').val()
                }
            }).done(function (data) {
                if (data) {
                    if (/^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/.test($('#tutor-password').val())) {
                        if ($('#tutor-password').val() === $('#tutor-confirmPassword').val()) {
                            $('#tutor-username').removeClass('input-invalid');
                            $('#tutor-username').popover('hide');
                            $('#tutor-username').popover('disable');
                            $('#tutor-username-wrapper').popover('hide');
                            $('#tutor-username-wrapper').popover('disable');
                            $('#tutor-password').removeClass('input-invalid');
                            $('#tutor-password').popover('hide');
                            $('#tutor-password').popover('disable');
                            $('#tutor-confirmPassword').removeClass('input-invalid');
                            $('#tutor-confirmPassword').popover('hide');
                            $('#tutor-confirmPassword').popover('disable');
                            goToTab(7);
                        } else {
                            $('#tutor-confirmPassword').addClass('input-invalid');
                            $('#tutor-confirmPassword').popover('enable');
                            $('#tutor-confirmPassword').popover('show');
                        }
                    } else {
                        $('#tutor-password').addClass('input-invalid');
                        $('#tutor-password').popover('enable');
                        $('#tutor-password').popover('show');
                    }
                } else {
                    $('#tutor-username').addClass('input-invalid');
                    $('#tutor-username-wrapper').popover('enable');
                    $('#tutor-username-wrapper').popover('show');
                }
            });
        } else {
            $('#tutor-username').addClass('input-invalid');
            $('#tutor-username').popover('enable');
            $('#tutor-username').popover('show');
        }
    }
}

function tab7UpdateNext() {
    $('#transcript-label').popover('hide');
    $('#transcript-label').popover('disable');
    $('#resume-label').popover('hide');
    $('#resume-label').popover('disable');
    if (!transcriptUploaded || !resumeUploaded) {
        $('#tab-7-next').removeClass('streamWork-primary');
        $('#tab-7-next').addClass('streamWork-disabled');
    } else {
        $('#tab-7-next').removeClass('streamWork-disabled');
        $('#tab-7-next').addClass('streamWork-primary');
    }
}

function tab7Next() {
    if (transcriptUploaded && resumeUploaded) {
        signUpTutor()
        $('#transcript-label').popover('hide');
        $('#transcript-label').popover('disable');
        $('#resume-label').popover('hide');
        $('#resume-label').popover('disable');
        goToTab(8);
    }
}

function tab9OauthUpdateNext() {
    $('#student-username-oauth').removeClass('input-invalid');
    $('#student-username-oauth').popover('hide');
    $('#student-username-oauth').popover('disable');
    $('#student-username-oauth-wrapper').popover('hide');
    $('#student-username-oauth-wrapper').popover('disable');
    if ($('#student-username-oauth').val().length == 0) {
        $('#tab-9-next').removeClass('streamWork-primary');
        $('#tab-9-next').addClass('streamWork-disabled');
    } else {
        $('#tab-9-next').removeClass('streamWork-disabled');
        $('#tab-9-next').addClass('streamWork-primary');
    }
}

function tab9OauthNext() {
    if ($('#student-username-oauth').val().length > 0) {
        if (/^[A-Za-z0-9_-]+$/.test($('#student-username-oauth').val())) {
            $.ajax({
                url: '/Home/SignUp/?handler=IsUsernameAvailable',
                type: 'GET',
                data: {
                    username: $('#student-username-oauth').val()
                }
            }).done(function (data) {
                if (data) {
                    signUpStudent();
                } else {
                    $('#student-username-oauth').addClass('input-invalid');
                    $('#student-username-oauth-wrapper').popover('enable');
                    $('#student-username-oauth-wrapper').popover('show');
                }
            });
        } else {
            $('#student-username-oauth').addClass('input-invalid');
            $('#student-username-oauth').popover('enable');
            $('#student-username-oauth').popover('show');
        }
    }
}

function tab10OauthUpdateNext() {
    $('#tutor-username-oauth').removeClass('input-invalid');
    $('#tutor-username-oauth').popover('hide');
    $('#tutor-username-oauth').popover('disable');
    $('#tutor-username-oauth-wrapper').popover('hide');
    $('#tutor-username-oauth-wrapper').popover('disable');
    if ($('#tutor-username-oauth').val().length == 0) {
        $('#tab-10-next').removeClass('streamWork-primary');
        $('#tab-10-next').addClass('streamWork-disabled');
    } else {
        $('#tab-10-next').removeClass('streamWork-disabled');
        $('#tab-10-next').addClass('streamWork-primary');
    }
}

function tab10OauthNext() {
    if ($('#tutor-username-oauth').val().length > 0) {
        if (/^[A-Za-z0-9_-]+$/.test($('#tutor-username-oauth').val())) {
            $.ajax({
                url: '/Home/SignUp/?handler=IsUsernameAvailable',
                type: 'GET',
                data: {
                    username: $('#tutor-username-oauth').val()
                }
            }).done(function (data) {
                if (data) {
                    goToTab(7);
                } else {
                    $('#tutor-username-oauth').addClass('input-invalid');
                    $('#tutor-username-oauth-wrapper').popover('enable');
                    $('#tutor-username-oauth-wrapper').popover('show');
                }
            });
        } else {
            $('#tutor-username-oauth').addClass('input-invalid');
            $('#tutor-username-oauth').popover('enable');
            $('#tutor-username-oauth').popover('show');
        }
    }
}

function signUpStudent() {
    profileType = "student"
    var formData = new FormData();
    if (!oauthStarted) {
        formData.append('EmailAddress', $('#emailAddress').val());
        formData.append('FirstName', $('#student-firstName').val());
        formData.append('LastName', $('#student-lastName').val());
        formData.append('Username', $('#student-username').val());
        formData.append('Password', $('#student-password').val());
    }
    else {
        formData.append('Username', $('#student-username-oauth').val());
        formData.append('Token', oauthToken);
    }

    formData.append('InCollege', $('#tab-2-inCollege').hasClass('streamWork-primary'));
    formData.append('SchoolName', $('#schoolName').val());
    formData.append('Topics', $('#tab-3-humanities').hasClass('streamWork-primary-rect') + "|" + $('#tab-3-math').hasClass('streamWork-primary-rect') + "|" + $('#tab-3-science').hasClass('streamWork-primary-rect') + "|" + $('#tab-3-art').hasClass('streamWork-primary-rect') + "|" + $('#tab-3-engineering').hasClass('streamWork-primary-rect') + "|" + $('#tab-3-business').hasClass('streamWork-primary-rect') + "|" + $('#tab-3-law').hasClass('streamWork-primary-rect') + "|" + $('#tab-3-other').hasClass('streamWork-primary-rect'))

    try {
        $.ajax({
            url: '/Home/SignUp/?handler=SignUpStudent',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            cache: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
        }).done(function (data) {
            goToTab(5);
        });
    } catch (e) {
        alert(e.message);
    }
}

function signUpTutor() {
    profileType = "tutor"
    var formData = new FormData();
    if (!oauthStarted) {
        formData.append('EmailAddress', $('#emailAddress').val());
        formData.append('FirstName', $('#tutor-firstName').val());
        formData.append('LastName', $('#tutor-lastName').val());
        formData.append('Username', $('#tutor-username').val());
        formData.append('Password', $('#tutor-password').val());
    }
    else {
        formData.append('Username', $('#tutor-username-oauth').val());
        formData.append('Token', oauthToken);
    }

    formData.append('InCollege', $('#tab-2-inCollege').hasClass('streamWork-primary'));
    formData.append('SchoolName', $('#schoolName').val());
    formData.append('Transcript', $('#transcript')[0].files[0]);
    formData.append('Resume', $('#resume')[0].files[0]);

    $.ajax({
        url: '/Home/SignUp/?handler=SignUpTutor',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        cache: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
    }).done(function (data) {
        //goToTab(5);
    });
}

function Route() {
    if (profileType === "tutor") {
        window.location.href = "/Tutor/TutorDashboard";
    }
    else {
        window.location.href = "/";
    }
}

$(function () {
    tab1UpdateNext();
    tab2UpdateNext();
    tab4UpdateNext();
    tab6UpdateNext();
    tab7UpdateNext();

    $('#transcript').change(function (e) {
        $('#transcript-label').text('Transcript: "' + e.target.files[0].name + '"');
        transcriptUploaded = e.target.files.length > 0;
        tab7UpdateNext();
    });
    $('#resume').change(function (e) {
        $('#resume-label').text('Résumé: "' + e.target.files[0].name + '"');
        resumeUploaded = e.target.files.length > 0;
        tab7UpdateNext();
    });
});