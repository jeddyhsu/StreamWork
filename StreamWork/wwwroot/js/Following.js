function FollowStreamTutor(followerId, followeeId, i) {
    $.ajax({
        url: '/Home/AddFollower',
        type: 'post',
        datatype: 'json',
        data: {
            'followerId': followerId,
            'followeeId': followeeId,
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

function UnfollowStreamTutor(followerId, followeeId, i) {
    $.ajax({
        url: '/Home/RemoveFollower',
        type: 'post',
        datatype: 'json',
        data: {
            'followerId': followerId,
            'followeeId': followeeId,
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