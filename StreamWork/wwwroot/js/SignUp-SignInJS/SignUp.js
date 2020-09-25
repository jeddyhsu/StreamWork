var studentTopics = 0;
var tutorTopics = 0;
var transcriptUploaded = false;
var resumeUploaded = false;
var oauthStarted = false;
var oauthToken = "";
var profileType = "";

var tabs = [
    'email',
    'emailVerification',
    'studentOrTutor',
    'studentTopics',
    'studentProfileInfo',
    'studentProfileInfoOAuth',
    'studentComplete',
    'tutorTopics',
    'tutorProfileInfo',
    'tutorProfileInfoOAuth',
    'tutorResumeTranscript',
    'tutorComplete',
]

function goToTab(tab) {
    for (let i of tabs) {
        $('#' + i).hide();
    }
    $('#' + tab).show();
}

function emailUpdateNext() {
    $('#emailAddress').removeClass('input-invalid');
    $('#emailAddress').popover('hide');
    $('#emailAddress').popover('disable');
    $('#emailAddress-wrapper').popover('hide');
    $('#emailAddress-wrapper').popover('disable');
    if ($('#emailAddress').val().length == 0) {
        $('#email-next').removeClass('streamWork-primary');
        $('#email-next').addClass('streamWork-disabled');
    } else {
        $('#email-next').removeClass('streamWork-disabled');
        $('#email-next').addClass('streamWork-primary');
    }
}

function emailNext() {
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
                        sendVerificationEmail();
                        goToTab('emailVerification');
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

function sendVerificationEmail() {
    $.ajax({
        url: '/Home/SignUp/?handler=SendVerificationEmail',
        type: 'GET',
        datatype: 'json',
        data: {
            emailAddress: $('#emailAddress').val()
        }
    }).done(function (data) {
        $('#emailVerificationCode').removeClass('input-invalid');
        $('#emailVerificationCode').popover('hide');
        $('#emailVerificationCode').popover('disable');
    });
}

function emailVerificationUpdateNext() {
    $('#emailVerificationCode').removeClass('input-invalid');
    $('#emailVerificationCode').popover('hide');
    $('#emailVerificationCode').popover('disable');
    if ($('#emailVerificationCode').val().length == 0) {
        $('#emailVerification-next').removeClass('streamWork-primary');
        $('#emailVerification-next').addClass('streamWork-disabled');
    } else {
        $('#emailVerification-next').removeClass('streamWork-disabled');
        $('#emailVerification-next').addClass('streamWork-primary');
    }
}

function emailVerificationNext() {
    if ($('#emailVerificationCode').val().length > 0) {
        $.ajax({
            url: '/Home/SignUp/?handler=CheckVerificationCode',
            type: 'GET',
            datatype: 'json',
            data: {
                emailAddress: $('#emailAddress').val(),
                verificationCode: $('#emailVerificationCode').val()
            }
        }).done(function (data) {
            if (data) {
                $('#emailVerificationCode').removeClass('input-invalid');
                $('#emailVerificationCode').popover('hide');
                $('#emailVerificationCode').popover('disable');
                goToTab('studentOrTutor');
            } else {
                $('#emailVerificationCode').addClass('input-invalid');
                $('#emailVerificationCode').popover('enable');
                $('#emailVerificationCode').popover('show');
            }
        });
    }
}

function setIsTutor(isTutor) {
    if (isTutor) {
        $('#studentOrTutor-next').text('Sign Up As a StreamTutor');
        $('#studentOrTutor-image-1').hide();
        $('#studentOrTutor-image-2').show();
        $('#tutor-dot').css('display', 'inline-block');
    } else {
        $('#studentOrTutor-next').text('Sign Up As a Student');
        $('#studentOrTutor-image-2').hide();
        $('#studentOrTutor-image-1').show();
        $('#tutor-dot').hide();
    }

    if ($('#studentOrTutor-inCollege').hasClass('streamWork-primary') || $('#studentOrTutor-inHighSchool').hasClass('streamWork-primary')) {
        $('#studentOrTutor-bottom').show();
    }
}

function setInCollege(inCollege) {
    let primary;
    let secondary;
    if (inCollege) {
        primary = $('#studentOrTutor-inCollege');
        secondary = $('#studentOrTutor-inHighSchool');
        $('#studentOrTutor-schoolType').text('College / University Name');
    } else {
        primary = $('#studentOrTutor-inHighSchool');
        secondary = $('#studentOrTutor-inCollege');
        $('#studentOrTutor-schoolType').text('High School Name');
    }

    primary.addClass('streamWork-primary').removeClass('streamWork-secondary');
    secondary.addClass('streamWork-secondary').removeClass('streamWork-primary');

    let isTutor = $("input[name='profile-type']:checked").val()
    if (isTutor === 'true' || isTutor === 'false') {
        $('#studentOrTutor-bottom').show();
    }
}

function studentOrTutorUpdateNext() {
    if ($('#schoolName').val().length == 0) {
        $('#studentOrTutor-next').removeClass('streamWork-primary');
        $('#studentOrTutor-next').addClass('streamWork-disabled');
    } else {
        $('#studentOrTutor-next').removeClass('streamWork-disabled');
        $('#studentOrTutor-next').addClass('streamWork-primary');
    }
}

function studentOrTutorNext() {
    if ($('#schoolName').val().length > 0) {
        if ($("input[name='profile-type']:checked").val() === 'true') {
            goToTab('tutorTopics');
        } else {
            goToTab('studentTopics');
        }
    }
}

function toggleStudentTopic(topic) {
    let button = $('#studentTopics-' + topic);
    if (button.hasClass('streamWork-primary-rect')) {
        button.removeClass('streamWork-primary-rect');
        button.addClass('streamWork-secondary-rect');
        studentTopics -= 1;
    } else {
        button.removeClass('streamWork-secondary-rect');
        button.addClass('streamWork-primary-rect');
        studentTopics += 1;
    }

    $('#studentTopics-selected').text('Selected: ' + studentTopics);

    if (studentTopics >= 1) {
        $('#studentTopics-next').removeClass('streamWork-disabled');
        $('#studentTopics-next').addClass('streamWork-primary');
    } else {
        $('#studentTopics-next').removeClass('streamWork-primary');
        $('#studentTopics-next').addClass('streamWork-disabled');
    }
}

function goToStudentTopicsTab1() {
    $('#studentTopics-tab-2').hide();
    $('#studentTopics-tab-1').show();
}

function goToStudentTopicsTab2() {
    $('#studentTopics-tab-1').hide();
    $('#studentTopics-tab-2').show();
}

function studentTopicsNext() {
    if (studentTopics >= 1) {
        if (oauthStarted) goToTab('studentProfileInfoOAuth');
        else goToTab('studentProfileInfo');
    }
}

function studentProfileInfoUpdateNext() {
    $('#student-firstName').removeClass('input-invalid');
    $('#student-firstName').popover('hide');
    $('#student-firstName').popover('disable');
    $('#student-lastName').removeClass('input-invalid');
    $('#student-lastName').popover('hide');
    $('#student-lastName').popover('disable');
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
        $('#studentProfileInfo-next').removeClass('streamWork-primary');
        $('#studentProfileInfo-next').addClass('streamWork-disabled');
    } else {
        $('#studentProfileInfo-next').removeClass('streamWork-disabled');
        $('#studentProfileInfo-next').addClass('streamWork-primary');
    }
}

function studentProfileInfoNext() {
    if ($('#student-firstName').val().length > 0 &&
        $('#student-lastName').val().length > 0 &&
        $('#student-username').val().length > 0 &&
        $('#student-password').val().length > 0 &&
        $('#student-confirmPassword').val().length > 0) {
        if (/^[^0-9\t\n\/<>?;:"`!@#$%^&*()\[\]{}_+=|\\]+$/.test($('#student-firstName').val())) {
            if (/^[^0-9\t\n\/<>?;:"`!@#$%^&*()\[\]{}_+=|\\]+$/.test($('#student-lastName').val())) {
                if (/^[A-Za-z0-9_-]+$/.test($('#student-username').val())) {
                    $.ajax({
                        url: '/Home/SignUp/?handler=IsUsernameAvailable',
                        type: 'GET',
                        data: {
                            username: $('#student-username').val()
                        }
                    }).done(function (data) {
                        if (data) {
                            const re = /^(?=.*[0-9])(?=.*[A-Za-z]).{8,}$/
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
            } else {
                $('#student-lastName').addClass('input-invalid');
                $('#student-lastName').popover('enable');
                $('#student-lastName').popover('show');
            }
        } else {
            $('#student-firstName').addClass('input-invalid');
            $('#student-firstName').popover('enable');
            $('#student-firstName').popover('show');
        }
    }
}

function studentProfileInfoOAuthUpdateNext() {
    $('#student-username-oauth').removeClass('input-invalid');
    $('#student-username-oauth').popover('hide');
    $('#student-username-oauth').popover('disable');
    $('#student-username-oauth-wrapper').popover('hide');
    $('#student-username-oauth-wrapper').popover('disable');
    if ($('#student-username-oauth').val().length == 0) {
        $('#studentProfileInfoOAuth-next').removeClass('streamWork-primary');
        $('#studentProfileInfoOAuth-next').addClass('streamWork-disabled');
    } else {
        $('#studentProfileInfoOAuth-next').removeClass('streamWork-disabled');
        $('#studentProfileInfoOAuth-next').addClass('streamWork-primary');
    }
}

function studentProfileInfoOAuthNext() {
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

function toggleTutorTopic(topic) {
    let button = $('#tutorTopics-' + topic);
    if (button.hasClass('streamWork-primary-rect')) {
        button.removeClass('streamWork-primary-rect');
        button.addClass('streamWork-secondary-rect');
        tutorTopics -= 1;
    } else {
        button.removeClass('streamWork-secondary-rect');
        button.addClass('streamWork-primary-rect');
        tutorTopics += 1;
    }

    $('#tutorTopics-selected').text('Selected: ' + tutorTopics);

    if (tutorTopics >= 1) {
        $('#tutorTopics-next').removeClass('streamWork-disabled');
        $('#tutorTopics-next').addClass('streamWork-primary');
    } else {
        $('#tutorTopics-next').removeClass('streamWork-primary');
        $('#tutorTopics-next').addClass('streamWork-disabled');
    }
}

function goToTutorTopicsTab1() {
    $('#tutorTopics-tab-2').hide();
    $('#tutorTopics-tab-1').show();
}

function goToTutorTopicsTab2() {
    $('#tutorTopics-tab-1').hide();
    $('#tutorTopics-tab-2').show();
}

function tutorTopicsNext() {
    if (tutorTopics >= 1) {
        if (oauthStarted) goToTab('tutorProfileInfoOAuth');
        else goToTab('tutorProfileInfo');
    }
}

function tutorProfileInfoUpdateNext() {
    $('#tutor-firstName').removeClass('input-invalid');
    $('#tutor-firstName').popover('hide');
    $('#tutor-firstName').popover('disable');
    $('#tutor-lastName').removeClass('input-invalid');
    $('#tutor-lastName').popover('hide');
    $('#tutor-lastName').popover('disable');
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
    $('#tutor-payPal').removeClass('input-invalid');
    $('#tutor-payPal').popover('hide');
    $('#tutor-payPal').popover('disable');
    if ($('#tutor-firstName').val().length == 0 ||
        $('#tutor-lastName').val().length == 0 ||
        $('#tutor-username').val().length == 0 ||
        $('#tutor-password').val().length == 0 ||
        $('#tutor-confirmPassword').val().length == 0 ||
        $('#tutor-payPal').val().length == 0) {
        $('#tutorProfileInfo-next').removeClass('streamWork-primary');
        $('#tutorProfileInfo-next').addClass('streamWork-disabled');
    } else {
        $('#tutorProfileInfo-next').removeClass('streamWork-disabled');
        $('#tutorProfileInfo-next').addClass('streamWork-primary');
    }
}

function tutorProfileInfoNext() {
    if ($('#tutor-firstName').val().length > 0 &&
        $('#tutor-lastName').val().length > 0 &&
        $('#tutor-username').val().length > 0 &&
        $('#tutor-password').val().length > 0 &&
        $('#tutor-confirmPassword').val().length > 0) {
        if (/^[^0-9\t\n\/<>?;:"`!@#$%^&*()\[\]{}_+=|\\]+$/.test($('#tutor-firstName').val())) {
            if (/^[^0-9\t\n\/<>?;:"`!@#$%^&*()\[\]{}_+=|\\]+$/.test($('#tutor-lastName').val())) {
                if (/^[A-Za-z0-9_-]+$/.test($('#tutor-username').val())) {
                    $.ajax({
                        url: '/Home/SignUp/?handler=IsUsernameAvailable',
                        type: 'GET',
                        data: {
                            username: $('#tutor-username').val()
                        }
                    }).done(function (data) {
                        if (data) {
                            if (/^(?=.*[0-9])(?=.*[A-Za-z]).{8,}$/.test($('#tutor-password').val())) {
                                if ($('#tutor-password').val() === $('#tutor-confirmPassword').val()) {
                                    $.ajax({
                                        url: '/Home/SignUp/?handler=IsAddressValid',
                                        type: 'GET',
                                        data: {
                                            emailAddress: $('#tutor-payPal').val()
                                        }
                                    }).done(function (data) {
                                        if (data) {
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
                                            $('#tutor-payPal').removeClass('input-invalid');
                                            $('#tutor-payPal').popover('hide');
                                            $('#tutor-payPal').popover('disable');
                                            goToTab('tutorResumeTranscript');
                                        } else {
                                            $('#tutor-payPal').addClass('input-invalid');
                                            $('#tutor-payPal').popover('enable');
                                            $('#tutor-payPal').popover('show');
                                        }
                                    });
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
            } else {
                $('#tutor-lastName').addClass('input-invalid');
                $('#tutor-lastName').popover('enable');
                $('#tutor-lastName').popover('show');
            }
        } else {
            $('#tutor-firstName').addClass('input-invalid');
            $('#tutor-firstName').popover('enable');
            $('#tutor-firstName').popover('show');
        }
    }
}

function tutorProfileInfoOAuthUpdateNext() {
    $('#tutor-username-oauth').removeClass('input-invalid');
    $('#tutor-username-oauth').popover('hide');
    $('#tutor-username-oauth').popover('disable');
    $('#tutor-username-oauth-wrapper').popover('hide');
    $('#tutor-username-oauth-wrapper').popover('disable');
    if ($('#tutor-username-oauth').val().length == 0) {
        $('#tutorProfileInfoOAuth-next').removeClass('streamWork-primary');
        $('#tutorProfileInfoOAuth-next').addClass('streamWork-disabled');
    } else {
        $('#tutorProfileInfoOAuth-next').removeClass('streamWork-disabled');
        $('#tutorProfileInfoOAuth-next').addClass('streamWork-primary');
    }
}

function tutorProfileInfoOAuthNext() {
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
                    $.ajax({
                        url: '/Home/SignUp/?handler=IsAddressValid',
                        type: 'GET',
                        data: {
                            emailAddress: $('#tutor-payPal-oauth').val()
                        }
                    }).done(function (data) {
                        if (data) {
                            $('#tutor-username-oauth').removeClass('input-invalid');
                            $('#tutor-username-oauth').popover('hide');
                            $('#tutor-username-oauth').popover('disable');
                            $('#tutor-username-wrapper-oauth').popover('hide');
                            $('#tutor-username-wrapper-oauth').popover('disable');
                            $('#tutor-payPal-oauth').removeClass('input-invalid');
                            $('#tutor-payPal-oauth').popover('hide');
                            $('#tutor-payPal-oauth').popover('disable');
                            goToTab('tutorResumeTranscript');
                        } else {
                            $('#tutor-payPal-oauth').addClass('input-invalid');
                            $('#tutor-payPal-oauth').popover('enable');
                            $('#tutor-payPal-oauth').popover('show');
                        }
                    });
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

function tutorResumeTranscriptUpdateNext() {
    $('#transcript-label').popover('hide');
    $('#transcript-label').popover('disable');
    $('#resume-label').popover('hide');
    $('#resume-label').popover('disable');
    if (!transcriptUploaded || !resumeUploaded) {
        $('#tutorResumeTranscript-next').removeClass('streamWork-primary');
        $('#tutorResumeTranscript-next').addClass('streamWork-disabled');
    } else {
        $('#tutorResumeTranscript-next').removeClass('streamWork-disabled');
        $('#tutorResumeTranscript-next').addClass('streamWork-primary');
    }
}

function tutorResumeTranscriptNext() {
    if (transcriptUploaded && resumeUploaded) {
        signUpTutor()
        $('#transcript-label').popover('hide');
        $('#transcript-label').popover('disable');
        $('#resume-label').popover('hide');
        $('#resume-label').popover('disable');
    }
}

function tutorResumeTranscriptPrev() {
    if (oauthStarted) goToTab('tutorProfileInfoOAuth');
    else goToTab('tutorProfileInfo');
}

function signUpStudent() {
    $('#loader-student-profile-auth').removeClass('d-none').addClass('d-block');
    $('#loader-student-profile-info').removeClass('d-none').addClass('d-block');
    $('#studentProfileInfoOAuth-next').removeClass('streamWork-primary').addClass('streamWork-disabled')
    $('#studentProfileInfo-next').removeClass('streamWork-primary').addClass('streamWork-disabled')
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

    formData.append('InCollege', $('#studentOrTutor-inCollege').hasClass('streamWork-primary'));
    formData.append('SchoolName', $('#schoolName').val());
    formData.append('Topics',
        $('#studentTopics-humanities').hasClass('streamWork-primary-rect') + "|" +
        $('#studentTopics-math').hasClass('streamWork-primary-rect') + "|" +
        $('#studentTopics-science').hasClass('streamWork-primary-rect') + "|" +
        $('#studentTopics-art').hasClass('streamWork-primary-rect') + "|" +
        $('#studentTopics-engineering').hasClass('streamWork-primary-rect') + "|" +
        $('#studentTopics-business').hasClass('streamWork-primary-rect') + "|" +
        $('#studentTopics-law').hasClass('streamWork-primary-rect') + "|" +
        $('#studentTopics-other').hasClass('streamWork-primary-rect'));

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
            $('#loader-student-profile-auth').removeClass('d-block').addClass('d-none');
            $('#loader-student-profile-info').removeClass('d-block').addClass('d-none');
            goToTab('studentComplete');
        });
    } catch (e) {
        alert(e.message);
    }
}

function signUpTutor() {
    $('#loader-tutor').removeClass('d-none').addClass('d-block');
    $('#tutorResumeTranscript-next').removeClass('streamWork-primary').addClass('streamWork-disabled')
    profileType = "tutor"
    var formData = new FormData();
    if (!oauthStarted) {
        formData.append('EmailAddress', $('#emailAddress').val());
        formData.append('FirstName', $('#tutor-firstName').val());
        formData.append('LastName', $('#tutor-lastName').val());
        formData.append('Username', $('#tutor-username').val());
        formData.append('Password', $('#tutor-password').val());
        formData.append('PayPalAddress', $('#tutor-payPal').val());
    }
    else {
        formData.append('Username', $('#tutor-username-oauth').val());
        formData.append('PayPalAddress', $('#tutor-payPal-oauth').val());
        formData.append('Token', oauthToken);
    }

    formData.append('InCollege', $('#tab-2-inCollege').hasClass('streamWork-primary'));
    formData.append('SchoolName', $('#schoolName').val());
    formData.append('Transcript', $('#transcript')[0].files[0]);
    formData.append('Resume', $('#resume')[0].files[0]);

    formData.append('Topics',
        $('#tutorTopics-humanities').hasClass('streamWork-primary-rect') + "|" +
        $('#tutorTopics-math').hasClass('streamWork-primary-rect') + "|" +
        $('#tutorTopics-science').hasClass('streamWork-primary-rect') + "|" +
        $('#tutorTopics-art').hasClass('streamWork-primary-rect') + "|" +
        $('#tutorTopics-engineering').hasClass('streamWork-primary-rect') + "|" +
        $('#tutorTopics-business').hasClass('streamWork-primary-rect') + "|" +
        $('#tutorTopics-law').hasClass('streamWork-primary-rect') + "|" +
        $('#tutorTopics-other').hasClass('streamWork-primary-rect'));

    try {
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
            $('#loader-tutor').removeClass('d-block').addClass('d-none');
            goToTab('tutorComplete');
        });
    } catch (e) {
        alert(e.message);
    }
}

function Route() {
    if (profileType == "tutor") {
        window.location.href = "/Tutor/TutorDashboard";
    }
    else {
        window.location.href = "/Home/Browse/SW";
    }
}

$(function () {
    $('#transcript').change(function (e) {
        $('#transcript-label').text('Transcript: "' + e.target.files[0].name + '"');
        transcriptUploaded = e.target.files.length > 0;
        tutorResumeTranscriptUpdateNext();
    });
    $('#resume').change(function (e) {
        $('#resume-label').text('Résumé: "' + e.target.files[0].name + '"');
        resumeUploaded = e.target.files.length > 0;
        tutorResumeTranscriptUpdateNext();
    });
    emailUpdateNext();
    emailVerificationUpdateNext();
    studentOrTutorUpdateNext();
    studentProfileInfoUpdateNext();
    studentProfileInfoOAuthUpdateNext();
    tutorProfileInfoUpdateNext();
    tutorProfileInfoOAuthUpdateNext();
    tutorResumeTranscriptUpdateNext();
});