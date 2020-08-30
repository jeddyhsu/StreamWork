const connection = new signalR.HubConnectionBuilder()
    .withUrl("/pushnotificationhub")
    .withAutomaticReconnect([0, 1000, 5000, 10000, 15000, 20000, 30000, 45000, 60000, null])
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
   
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveNotification", function (notification, pushNotification, notificationId) {
    AddNotificationToPipeline(pushNotification, notificationId);
    AddNotificationToList(notification)
    $('#notification-bell-mask').html(`<img src="/images/GenericAssets/NotificationBell.svg" width="65" class="pointer" onclick="OpenModal('notification-modal'); UpdateToSeen('${notification.receiverUsername}');" />`)
});

function AddNotificationToPipeline(notification, notificationId) {
    $('#notification-pipeline').append(notification)
    $("#toast-notification-" + notificationId).toast('show');
}

function AddNotificationToList(notification) {
    if ($('#no-notification').length) {
        $('#notification-list').html('')
    }

    $('#notification-list').prepend(notification)
}

function SendNotification(username, message) {
    connection.invoke("SendPrivateMessage", username, message).catch(function (err) {
        return console.error(err.toString());
    });
}



