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
        $('#FollowButtonUnfollowed-' + i).hide();
        $('#UnfollowButtonUnfollowed-' + i).show();
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
        $('#FollowButtonFollowed-' + i).show();
        $('#UnfollowButtonFollowed-' + i).hide();
    }
}