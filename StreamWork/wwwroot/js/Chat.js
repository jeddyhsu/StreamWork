var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var initialUserId = "";

connection.on("ReceiveMessage", function (name, message, profilePicture, questionNumber, date, userID, chatColor) {

    var listName = "";
    if (initialUserId == userID) listName = "<h5 class='text-truncate mb-0 chatName' style='color:" + chatColor + "'>" + name + " (you)" + "<span class='chatDate'> " + date + "</span></h5>";
    else listName = "<h5 class='text-truncate mb-0 chatName' style='color:" + chatColor + "'>" + name + "<span class='chatDate'> " + date + "</span></h5>";
       
    var listItem = "<li class='list-group-item chatList'><div class='row'><div class='col-12'><input align='left' type='image' class='chatProfilePicture' src=" + profilePicture + "/>" + listName + "<p id='question-" + questionNumber + "'class='chatMessage'>" + message + "</p> </div></div></li>"

    $('#chatField').append(listItem);
    window.scroll(0, document.documentElement.offsetHeight);
    if (initialUserId != userID) PlayAudio();
    //var problemSpan = document.getElementById("question-" + questionNumber);
    //MQ.StaticMath(problemSpan);
});

function PlayAudio() {
    var audioElement = document.createElement('audio');
    audioElement.setAttribute('src', '/media/juntos.mp3')
    audioElement.play();
}

function JoinChatRoom(chatId, userId) {
    initialUserId = userId;
    connection.start().then(function () {
        connection.invoke("JoinChatRoom", chatId).catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

function SendMessageToChatRoom(chatId, userId, name, profilePicture, chatColor) {
    var message = document.getElementById("chatInput").value;
    FormatMessage(message);
    connection.invoke("SendMessageToChatRoom", chatId, userId, name, message, profilePicture, chatColor).catch(function (err) {
        return console.error(err.toString());
    });
    
    event.preventDefault();
}

function Alert(count) {
    var MQ = MathQuill.getInterface(2);
    for (i = 0; i < count; i++) {
        var problemSpan = document.getElementById("chat-" + i);
        MQ.StaticMath(problemSpan);
    }
}

function WriteCommmand(command) {
    var mathFieldSpan = document.getElementById('math-field');
    var mathField = MQ.MathField(mathFieldSpan);
    mathField.cmd(command);
}

function WriteExpression(expression) {
    var mathFieldSpan = document.getElementById('math-field');
    var mathField = MQ.MathField(mathFieldSpan);
    mathField.write(expression);
}

function FormatMessage() {
    var mathFieldSpan = document.getElementById('math-field')
    var latexSpan = document.getElementById('latex');
    var MQ = MathQuill.getInterface(2);
    var mathField = MQ.MathField(mathFieldSpan, {
        spaceBehavesLikeTab: false,
        autoCommands: 'pi theta sqrt sum integral delta gamma infinity isin pm',
        handlers: {
            edit: function () {
                latexSpan.textContent = mathField.latex();
            }
        }
    });
}

function PopoutChat(chatId) {
    var windowObjectRef;
    var windowFeatures = "menubar=no, toolbar=no,location=yes,resizable=yes,scrollbars=yes,status=yes, width=500, height=600";
    windowObjectRef = window.open('http://localhost:58539/chat/streamworkchat?chatId=' + chatId, 'StreamWork Chat', windowFeatures);

}