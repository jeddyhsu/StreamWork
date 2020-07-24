function Follow(followerId, followeeId, buttonId) {
    $.ajax({
        url: '/Follows/FollowModel/?handler=Follow',
        type: 'post',
        datatype: 'json',
        data: {
            'followerId': followerId,
            'followeeId': followeeId,
        },
         beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
    });
    var newId = "following-" + buttonId.split('-')[1]
    $("#" + buttonId).html("Following")
    $("#" + buttonId).attr("onclick", "Unfollow('" + followerId + "','" + followeeId + "','" + newId + "')")
    $("#" + buttonId).attr("id", newId) 
}

function Unfollow(followerId, followeeId, buttonId) {
    $.ajax({
        url: '/Follows/FollowModel/?handler=Unfollow',
        type: 'post',
        datatype: 'json',
        data: {
            'followerId': followerId,
            'followeeId': followeeId,
        },
         beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
    });
    var newId = "follow-" + buttonId.split('-')[1]
    $("#" + buttonId).html("Follow")
    $("#" + buttonId).attr("onclick", "Follow('" + followerId + "','" + followeeId + "','" + newId + "')")
    $("#" + buttonId).attr("id", newId) 
}