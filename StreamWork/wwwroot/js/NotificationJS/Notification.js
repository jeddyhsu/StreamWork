function UpdateToSeen(username) {
    $('#notification-bell-mask').html(`<img src="/images/GenericAssets/Bell.svg" width="65" class="pointer" onclick="OpenModal('notification-modal')" />`)
    $.ajax({
        url: '/Notifications/NotificationModal/?handler=UpdateToSeen',
        type: 'POST',
        datatype: 'json',
        data: {
            'username': username
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        }
    })
}

function DeleteNotification(id) {
    $.ajax({
        url: '/Notifications/NotificationModal/?handler=DeleteNotification',
        type: 'POST',
        datatype: 'json',
        data: {
            'id': id
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                $('#notification-' + id).remove();
                if ($('#notification-list li').length == 0) {
                    $('#notification-list').append(`<li id="no-notification" class="form-header text-center p-3 mb-0" style="font-size:18px;">No notifications at this time!</p>`)
                }
            }
        }
    })
}



