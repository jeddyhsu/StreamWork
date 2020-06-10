﻿var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").withAutomaticReconnect().build();
var initialUserName = "";
var initialChatId = "";
var toolTipCount = 0;
var chatCount = 0;
var muted = true;

connection.on("ReceiveMessage", function (name, message, profilePicture, questionNumber, userName, chatColor, date) {
    var listName = "";

    if (initialChatId == userName) listName = "<h5 class='mb-0 chatName' style='color:" + chatColor + "'>" + name + "<span><img id='tutortip" + toolTipCount + "' class='pl-1' src='/images/ChatAssets/Tutor.png' data-toggle='tooltip' data-placement='top' title='StreamTutor'/></span><span class='chatDate'>" + date + "</span></h5>";
    else if (initialUserName == userName) listName = "<h5 class='mb-0 chatName' style='color:" + chatColor + "'>" + name + " (you)" + "<span class='chatDate'> " + date + "</span></h5>";
    else listName = "<h5 class='mb-0 chatName' style='color:" + chatColor + "'>" + name + "<span class='chatDate'> " + date + "</span></h5>";

    if ((chatCount + 1) % 2 != 0) {
        var listItem = "<li class='list-group-item chatList border-right-0 border-left-0'><div class='row'><div class='col-12'><input align='left' type='image' class='chatProfilePicture rounded' src=" + profilePicture + "/>" + listName + "<p id='question-" + questionNumber + "'class='chatMessage'>" + message + "</p> </div></div></li>"
    }
    else {
        var listItem = "<li class='list-group-item chatListAlternate border-right-0 border-left-0'><div class='row'><div class='col-12'><input align='left' type='image' class='chatProfilePicture rounded' src=" + profilePicture + "/>" + listName + "<p id='question-" + questionNumber + "'class='chatMessage'>" + message + "</p> </div></div></li>"
    }

    chatCount++;
    $('#chatField').append(listItem);
    $("#tutortip" + toolTipCount).tooltip();
    toolTipCount++;
    window.scroll(0, document.documentElement.offsetHeight);
    if (!muted && initialUserName != userName) PlayAudio();
    //var problemSpan = document.getElementById("question-" + questionNumber);
    //MQ.StaticMath(problemSpan);
});

function PlayAudio() {
    var audioElement = document.createElement('audio');
    audioElement.setAttribute('src', '/media/juntos.mp3')
    if (!muted){
        var promise = audioElement.play();
        promise.then(function () {
            document.getElementById("sound").src = '/images/ChatAssets/Unmute.png';
            muted = false;

        }).catch(function () {
            return document.getElementById("sound").src = '/images/ChatAssets/Mute.png';
        })
    }
}

function ToggleMuteAndUnmute() {
    if (!muted) {
        document.getElementById("sound").src = '/images/ChatAssets/Mute.png';
        muted = true;
    }
    else {
        document.getElementById("sound").src = '/images/ChatAssets/Unmute.png';
        muted = false;
        PlayAudio();
    }
}

function JoinChatRoom(chatId, userName) {
    initialChatId = chatId;
    initialUserName = userName;
    connection.start().then(function () {
        connection.invoke("JoinChatRoom", chatId).catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

function GetMessage(chatId, userName, name, profilePicture, chatColor) {
    var message = document.getElementById("chatInput").value;
    if (message == "") return;
    CleanAndSendMessage(message, chatId, userName, name, profilePicture, chatColor);
}

function CleanAndSendMessage(message, chatId, userName, name, profilePicture, chatColor) {
    $.getJSON('https://api.dillilabs.com:8084/79c76f03-8337-4430-b6ed-b42787c3e82a/devil/isprofane?text=' + message, function (data) {
        if (!data) {
            var date = new Date();
            var offset = date.getTimezoneOffset();
            connection.invoke("SendMessageToChatRoom", chatId, userName, name, message, profilePicture, chatColor, date, offset).catch(function (err) {
                return console.error(err.toString());
                
            });
        }
    });

    document.getElementById("chatInput").value = "";
    event.preventDefault();
}

function PopoutChat(chatId, chatInfo) {
    var windowObjectRef;
    var windowFeatures = "menubar=no, toolbar=no,location=yes,resizable=yes,scrollbars=yes,status=yes, width=500, height=600";
    windowObjectRef = window.open('https://www.streamwork.live/chat/streamworkchat?chatId=' + chatId + "&chatInfo=" + chatInfo, 'StreamWork Chat', windowFeatures);
}

//function Alert(count) {
//    var MQ = MathQuill.getInterface(2);
//    for (i = 0; i < count; i++) {
//        var problemSpan = document.getElementById("chat-" + i);
//        MQ.StaticMath(problemSpan);
//    }
//}

//function WriteCommmand(command) {
//    var mathFieldSpan = document.getElementById('math-field');
//    var mathField = MQ.MathField(mathFieldSpan);
//    mathField.cmd(command);
//}

//function WriteExpression(expression) {
//    var mathFieldSpan = document.getElementById('math-field');
//    var mathField = MQ.MathField(mathFieldSpan);
//    mathField.write(expression);
//}

//function FormatMessage() {
//    var mathFieldSpan = document.getElementById('math-field')
//    var latexSpan = document.getElementById('latex');
//    var MQ = MathQuill.getInterface(2);
//    var mathField = MQ.MathField(mathFieldSpan, {
//        spaceBehavesLikeTab: false,
//        autoCommands: 'pi theta sqrt sum integral delta gamma infinity isin pm',
//        handlers: {
//            edit: function () {
//                latexSpan.textContent = mathField.latex();
//            }
//        }
//    });
//}

