function FilterStreams(subject) {
    var subjectDivs = $('.streamsubject');

    $.each(subjectDivs, function (index, value) {
        $(this).show();
    });

    $.each(subjectDivs, function (index, value) {
        if ($(this).hasClass(subject) == false) {
            $(this).hide();
        }
    });
}

function FollowStreamTutor(tutor) {
    $.ajax({
        url: '/Home/ProfileView',
        type: 'post',
        datatype: 'json',
        data: {
            'followRequest': 'follow',
            'tutorId': tutor
        }
    });

    $('#FollowButton').hide();
    $('#UnfollowButton').show();
}

function UnfollowStreamTutor(tutor) {
    $.ajax({
        url: '/Home/ProfileView',
        type: 'post',
        datatype: 'json',
        data: {
            'unFollowRequest': 'Unfollow',
            'tutorId': tutor
        }
    });

    $('#FollowButton').show();
    $('#UnfollowButton').hide();
}