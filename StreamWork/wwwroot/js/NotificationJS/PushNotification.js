const connection = new signalR.HubConnectionBuilder()
    .withUrl("/pushnotificationhub")
    .withAutomaticReconnect([0, 1000, 5000, 10000, 15000, 20000, 30000, 45000, 60000, null])
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
   
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveNotification", function (notification) {
    AddNotificationToPipeline(notification);
    AddNotificationToList(notification)
    $('#notification-bell-mask').html(`<img src="/images/GenericAssets/NotificationBell.svg" width="65" class="pointer" onclick="OpenModal('notification-modal'); UpdateToSeen('${notification.receiverUsername}');" />`)
});

function AddNotificationToPipeline(notification) {
    var notificationType = ""
    if (notification.type == "Comment" || notification.type == "Reply") {
        notificationType = `<a class="streamWork-green" href="/Stream/Archive/${notification.receiverUsername}/${notification.notificationInfo.split('|')[1]}/${notification.objectId}"><b>Reply</b></a>`
    }
    else {
        notificationType = `<a class="streamWork-green" href="/Profiles/Student/${notification.senderUsername}"><b>Profile</b></a>`
    }

    var pushNotification = `<div id="toast-notification-${notification.id}" class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-delay="10000">
                            <div class="toast-header streamWork-white" style="background-color: #004643">
                                <strong class="mr-auto">${notification.type} Alert!</strong>
                                <small class="streamWork-white">${moment(notification.date).format('h:mm A')}</small>
                                <button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="toast-body">
                                <span style="color:${notification.profileColor}">${notification.senderName.replace('|', ' ')}</span> ${notification.message}
                                <br>
                               ${notificationType}
                            </div>
                        </div>`

    $('#notification-pipeline').append(pushNotification)
    $("#toast-notification-" + notification.id).toast('show');
}

function AddNotificationToList(notification) {
    var notificationType = "";

    if (notification.type == "Comment" || notification.type == "Reply") {
        notificationType = `<p class="form-header mb-0" style="padding-left:40px;">
                               <span style="color:${notification.profileColor}">${notification.senderName.replace('|', ' ')}</span> ${notification.message} (${notification.notificationInfo.split('|')[0]}) <span class="form-subheader roboto" style="font-size:10px;">${moment(notification.date).format('hh:mm A')}</span>
                            </p>
                            <p class="mb-0" style="padding-left:40px; font-size:12px;">
                               ${notification.notificationInfo.split('|')[2]}
                            </p>
                            <a class="notification-destination" href="/Stream/Archive/${notification.receiverUsername}/${notification.notificationInfo.split('|')[1]}/${notification.objectId}" style="padding-left:10px"><b>Reply</b></a>`
    }
    else {
    notificationType = `<p class="form-header mb-0" style="padding-left:40px;">
                            <span style="color:${notification.profileColor}">${notification.senderName.replace('|', ' ')}</span> ${notification.message} <span class="form-subheader roboto" style="font-size:10px;">${moment(notification.date).format('hh:mm A')}</span>
                        </p>
                        <a class="notification-destination" href="/Profiles/Student/${notification.senderUsername}" style="padding-left:10px"><b>Profile</b></a>`
    }

    var pushNotification = `<li id="notification-${notification.id}" class="border-bottom">
                            <div class="row pt-3 pb-3">
                                <div class="col-12">
                                    <img id="notification-delete-${notification.id}" src="/images/GenericAssets/Trash.png" class="notification-remove mr-3 float-right" onclick="DeleteNotification('${notification.id}')" />
                                    <input align="left" type="image" class="rounded" src="${notification.senderProfilePicture}" style="width:30px" />
                                    <script>
                                        $('#notification-${notification.id}').hover(function () {
                                            $('#notification-delete-${notification.id}').css('display', 'block')
                                        }, function () {
                                            $('#notification-delete-${notification.id}').css('display', 'none')
                                        })
                                    </script>
                                        ${notificationType}
                                </div>
                            </div>
                         </li>`
    if ($('#no-notification').length) {
        $('#notification-list').html('')
    }

    $('#notification-list').prepend(pushNotification)
}


function SendNotification(username, message) {
    connection.invoke("SendPrivateMessage", username, message).catch(function (err) {
        return console.error(err.toString());
    });
}



