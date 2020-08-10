const connection = new signalR.HubConnectionBuilder()
    .withUrl("/pushnotificationhub")
    .withAutomaticReconnect([0, 1000, 5000, 10000, 15000, 20000, 30000, 45000, 60000, null])
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveMessage", function () {
   
});

function AddNotificationToPipeline() {
    var notification = `<div class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-delay="3000">
                            <div class="toast-header">
                                <img src="..." class="rounded mr-2" alt="...">
                                <strong class="mr-auto">Bootstrap</strong>
                                <small class="text-muted">just now</small>
                                <button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="toast-body">
                                See? Just like this.
                            </div>
                        </div>`

    $('#notification-pipeline').append(notification)
    $(".toast").toast('show');
}

function SendNotification(username, message) {
    connection.start().then(function () {
        connection.invoke("SendPrivateMessage", username, message).catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });
}



