var count = 0;
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (name, message, profilePicture, questionNumber) {
    $('#chatField').append("<li style='background-color:transparent; border:none' class='list-group-item'><span><img class='profilePictureSetting' src=" + profilePicture + "/></span>  " + name + ": <span id='question-" + questionNumber + "'>" + message + "</span></li > ");
    var problemSpan = document.getElementById("question-" + questionNumber);
    MQ.StaticMath(problemSpan);
});

function JoinChatRoom(chatId) {
    connection.start().then(function () {
        connection.invoke("JoinChatRoom", chatId).catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

function SendMessageToChatRoom(chatId, userId, name, profilePicture){
    var message = document.getElementById("latex").textContent;
    connection.invoke("SendMessageToChatRoom", chatId, userId, name, message, profilePicture).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("regularQuestions").value = "";
    event.preventDefault();
}

function Alert(count) {
    var MQ = MathQuill.getInterface(2);
    for (i = 0; i < count; i++) {
        var problemSpan = document.getElementById("chat-" + i);
        MQ.StaticMath(problemSpan);
    }
}

var inputCheck = false;
function SwitchInputs() {
    if (!inputCheck) {
        document.getElementById("regularQuestions").style.display = 'none';
        document.getElementById("equationQuestions").style.display = 'block';
        inputCheck = true;
    }
    else {
        document.getElementById("regularQuestions").style.display = 'block';
        document.getElementById("equationQuestions").style.display = 'none';
        inputCheck = false;
    }
}