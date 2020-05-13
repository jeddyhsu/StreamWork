var count = 0;
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (user, message) {
    var x = MQ.StaticMath("ax^2 + bx + c = 0");
    $('#tutorQuestionGroup').append("<li id='question-1' class='list-group-item'>" + message + "</li>");
    var problemSpan = document.getElementById("question-1");
    MQ.StaticMath(problemSpan);
});

function StartConnectionToServerTutor(tutorId) {
    connection.start().then(function () {
        connection.invoke("RegisterConnection", tutorId).catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

function StartConnectionToServerStudent() {
    connection.start().then(function () {
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

function SendMessageToTutor(nameOfUser, tutorId) {
    var message = document.getElementById("latex").textContent;
    connection.invoke("SendMessageToTutor", nameOfUser, tutorId, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
}