﻿const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .withAutomaticReconnect([0, 1000, 5000, 10000, 15000, 20000, 30000, 45000, 60000, 70000, 80000, 93000, 100000, null])
    .configureLogging(signalR.LogLevel.Information)
    .build();

var clientUsername = "";
var toolTipCount = 0;
var chatCount = 0;
var muted = true;

connection.onreconnected(connectionId => {
    console.log("Reconnected!!")
    var chatId = $('#chat-id').val();
    var username = $('#current-username').val();
    JoinChat(chatId, connectionId);
});

connection.on("ReceiveMessage", function (chat) {
    chat = JSON.parse(chat);
    var listName = "";
    var isStreamOnline = ""

    if (chat.ArchivedVideoId == null) {
        isStreamOnline = "(stream offline - not saved)"
    }

    if (chat.ChatId == chat.Username) {
        listName = ` <p class="chat-name" style="color:${chat.ChatColor}">${chat.Name.replace('|', ' ')}
                        <span><img id="tutor-tip-${toolTipCount}" class="pl-1" style="width:28px" src="/images/ChatAssets/Tutor.svg" data-toggle="tooltip" data-placement="top" title="StreamTutor"></span>
                        <span class='chat-date'>${chat.DateString} ${isStreamOnline}</span>
                     </p>`
    }
    else if (clientUsername == chat.Username) {
        listName = `<p class="chat-name" style="color:${chat.ChatColor}">${chat.Name.replace('|', ' ')} (you)
                         <span class="chat-date">${chat.DateString} ${isStreamOnline}</span>
                    </p>`
    }
    else {
        listName = `<p class="chat-name" style="color:${chat.ChatColor}">${chat.Name.replace('|', ' ')}
                        <span class="chat-date">${chat.DateString} ${isStreamOnline}</span>
                    </p>`
    }

    var colorWay = "";
    if ((chatCount + 1) % 2 != 0) colorWay = "chat-list-primary-color-way"
    else colorWay = "chat-list-secondary-color-way"

    var listItem = `<li id="message-${chatCount}" class='list-group-item ${colorWay} border-0'>
                            <div class="row">
                                <div class="col-12">
                                    <input align="left" type="image" class="chat-profile-picture rounded pointer" onclick="window.parent.location.href='/Profiles/Student/${chat.Username}'" src=${chat.ProfilePicture} />
                                    ${listName}
                                    <p id="chat-${chatCount}" class="chat-message">${chat.Message}</p>
                                </div>
                            </div>
                        </li>`

    chatCount++;
    $('#chat-field').append(listItem);
    $("#tutor-tip-" + toolTipCount).tooltip();
    toolTipCount++;
    window.scroll(0, document.documentElement.offsetHeight);
    if (!muted && clientUsername != chat.Username) PlayAudio();
});

function PlayAudio() {
    var audioElement = document.createElement('audio');
    audioElement.setAttribute('src', '/media/juntos.mp3')
    var promise = audioElement.play();
    promise.then(function () {
        document.getElementById("sound").src = '/images/ChatAssets/Unmute.svg';
        muted = false;

    }).catch(function () {
        return document.getElementById("sound").src = '/images/ChatAssets/Mute.svg';
    })
}

function ToggleMuteAndUnmute() {
    if (!muted) {
        document.getElementById("sound").src = '/images/ChatAssets/Mute.svg';
        muted = true;
    }
    else {
        document.getElementById("sound").src = '/images/ChatAssets/Unmute.svg';
        muted = false;
        PlayAudio();
    }
}

function JoinChatRoom(chatId, userName, connectionId) {
    clientUsername = userName;
    connection.start().then(function () {
        JoinChat(chatId, connectionId)
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

function JoinChat(chatId, connectionId) {
    connection.invoke("JoinChatRoom", chatId, connectionId != null ? connectionId : null).catch(function (err) {
        return console.error(err.toString());
    });
}

function GetMessage(chatId, userName, name, profilePicture, chatColor){
    var message = $("#chatInput").html().replace(/<div>/gi, '<br>').replace(/<\/div>/gi, '')
    if (message == "") return;
    CleanAndSendMessage(message, chatId, userName, name, profilePicture, chatColor);
}

function CleanAndSendMessage(message, chatId, userName, name, profilePicture, chatColor) {
    var offset = moment().utcOffset();
    connection.invoke("SendMessageToChatRoom", chatId, userName, name, message, profilePicture, chatColor, offset).catch(function (err) {
        return console.error(err.toString());

    });

    $("#chatInput").text("");
    event.preventDefault();
}

function PopoutChat(chatId, chatInfo) {
    var windowObjectRef;
    var windowFeatures = "menubar=no, toolbar=no,location=yes,resizable=yes,scrollbars=yes,status=yes, width=300, height=600";
    windowObjectRef = window.open('/chat/LiveChat?chatId=' + chatId + "&chatInfo=" + chatInfo, 'StreamWork Chat', windowFeatures);
}

function CheckIfEmpty() {
    if ($('#chatInput').val() == "") {
        $('#sendQuestionButton').removeClass().addClass('streamWork-disabled')
        document.getElementById('sendQuestionButton').disabled = true;
    }
    else {
        $('#sendQuestionButton').removeClass().addClass('streamWork-primary')
        document.getElementById('sendQuestionButton').disabled = false;
    }
}