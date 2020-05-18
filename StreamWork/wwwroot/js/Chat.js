﻿var count = 0;
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (name, message, profilePicture, questionNumber, questionType) {
    $('#chatField').append("<li style='background-color:transparent; border:none' id='question-" + questionNumber + "'class='list-group-item'><span><img class='profilePictureSetting' src=" + profilePicture + "/></span> " + name + ": " + message + "</li > ");
    if (questionType != "regularQuestion") {
        var problemSpan = document.getElementById("question-" + questionNumber);
        MQ.StaticMath(problemSpan);
    }
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
    var message = "";
    var questionType = "";
    var equationMessage = document.getElementById("latex").textContent;
    var regularMessage = document.getElementById("regularQuestions").value;
    if (equationMessage == "") {
        message = regularMessage;
        questionType = "regularQuestion";
    }
    else {
        message = equationMessage;
        questionType = "equationQuestion";
    }
    connection.invoke("SendMessageToChatRoom", chatId, userId, name, message, questionType, profilePicture).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("regularQuestions").value = "";
    event.preventDefault();
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