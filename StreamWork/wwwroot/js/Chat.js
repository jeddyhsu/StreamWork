var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var initialUserId = "";

connection.on("ReceiveMessage", function (name, message, profilePicture, questionNumber, date, userID) {

    var listName = "";
    if (initialUserId == userID) listName = "<h5 class='text-truncate mb-0 chatName'>" + name + " (you)" + "<span class='chatDate'> " + date + "</span></h5>";
    else listName = "<h5 class='text-truncate mb-0 chatName'>" + name + "<span class='chatDate'> " + date + "</span></h5>";
       
    var listItem = "<li class='list-group-item chatList'><div class='row'><div class='col-12'><input type='image' class='chatProfilePicture' src=" + profilePicture + "/>" + listName + "<p id='question-" + questionNumber + "'class='text-truncate mt-2 chatMessage'>" + message + "</p> </div></div></li>"

    $('#chatField').append(listItem);
    //var problemSpan = document.getElementById("question-" + questionNumber);
    //MQ.StaticMath(problemSpan);
});

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

function SendMessageToChatRoom(chatId, userId, name, profilePicture) {
    var message = document.getElementById("chatInput").value;
    FormatMessage(message);
    connection.invoke("SendMessageToChatRoom", chatId, userId, name, message, profilePicture).catch(function (err) {
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