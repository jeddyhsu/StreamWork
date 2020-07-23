function Follow(followerId, followeeId) {
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

    $("#Follow").setAttr("innerText", "Following")
    $("#Follow").setAttr("onclick", "Unfollow(" + followerId + "," + followeeId + ")") 
    $("#Follow").setAttr("id", "Following") 
}

function Unfollow(followerId, followeeId, i) {
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

    $("#Following").setAttr("innerText", "Follow")
    $("#Following").setAttr("onclick", "Follow(" + followerId + "," + followeeId + ")")
    $("#Following").setAttr("id", "Follow") 
}