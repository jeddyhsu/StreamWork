var timerId;

function Follow(followerId, followeeId, buttonId) {
    if (!(timerId == null)) { 
        clearTimeout(timerId); //reset this call if its been less then 8000ms PREVENT SPAMMING
    }
    timerId = setTimeout(function () { //set this call and execute after 8000ms
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
    }, 8000); //8000ms Timeout 8 seconds

    var newId = "following-" + buttonId.split('-')[1]
    $("#" + buttonId).html("Following")
    $("#" + buttonId).attr("onclick", "Unfollow('" + followerId + "','" + followeeId + "','" + newId + "')")
    $("#" + buttonId).attr("id", newId) 
}

function Unfollow(followerId, followeeId, buttonId) {
    if (!(timerId == null)) {
        clearTimeout(timerId); //reset this call if its been less then 8000ms PREVENT SPAMMING
    }
    timerId = setTimeout(function () { //set this call and execute after 8000ms
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
    }, 8000); //8000ms Timeout 8 seconds

    var newId = "follow-" + buttonId.split('-')[1]
    $("#" + buttonId).html("Follow")
    $("#" + buttonId).attr("onclick", "Follow('" + followerId + "','" + followeeId + "','" + newId + "')")
    $("#" + buttonId).attr("id", newId) 
}