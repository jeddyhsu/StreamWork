function FollowStreamTutor(tutor, i) {
    $.ajax({
        url: '/Home/ProfileView',
        type: 'post',
        datatype: 'json',
        data: {
            'followRequest': 'follow',
            'tutorId': tutor
        }
    });
    if (i == null) {
        $('#FollowButton').hide();
        $('#UnfollowButton').show();
    }
    else {
        $('#UnfollowButton-' + i).show();
        $('#FollowButton-' + i).hide();
    }
}

function UnfollowStreamTutor(tutor, i) {
    $.ajax({
        url: '/Home/ProfileView',
        type: 'post',
        datatype: 'json',
        data: {
            'unFollowRequest': 'Unfollow',
            'tutorId': tutor
        }
    });
    if (i == null) {
        $('#FollowButton').show();
        $('#UnfollowButton').hide();
    }
    else {
        $('#FollowButton-' + i).show();
        $('#UnfollowButton-' + i).hide();
    }
}