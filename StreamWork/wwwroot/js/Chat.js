const connection = new signalR.HubConnectionBuilder()
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
                                    <div class="chat-message mt-0 pt-0">
                                        ${chat.Message}
                                    </div>
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

function GetMessage(chatId, userName, name, profilePicture, chatColor) {
    if ($("#chatInput").text() > 500) {
        OpenInvalidComment()
        return;
    }

    var message = $("#chatInput").html().replace(/<div>/gi, '<br>').replace(/<\/div>/gi, '')
    if (message.substring(0, 4) == "<br>")
        message = message.replace('<br>', '')
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

function PopoutChat(chatId) {
    var windowObjectRef;
    var windowFeatures = "menubar=no, toolbar=no,location=yes,resizable=yes,scrollbars=yes,status=yes, width=600, height=600";
    windowObjectRef = window.open('/Chat/Live/' + chatId, 'StreamWork Chat', windowFeatures);
}

function CheckIfEmpty() {
    if ($('#chatInput').text() == "") {
        $('#sendQuestionButton').removeClass('streamWork-primary').addClass('streamWork-disabled')
        document.getElementById('sendQuestionButton').disabled = true;
    }
    else {
        $('#sendQuestionButton').removeClass('streamWork-disabled').addClass('streamWork-primary')
        document.getElementById('sendQuestionButton').disabled = false;
    }

    if ($('#chatInput').text().length > 500) {
        $('#too-long').removeClass('d-none').addClass('d-block')
        $('#sendQuestionButton').removeClass('streamWork-primary').addClass('streamWork-disabled')
        document.getElementById('sendQuestionButton').disabled = true;
    }
    else {
        $('#too-long').removeClass('d-block').addClass('d-none')
    }
}

function OpenInvalidComment() {
    var confirmation = ` <div class="custom-modal-content" style="width:300px; height:200px" >
                            <div class="close ml-auto" onclick="CloseModal('notification-invalid-comment-modal')">&times;</div>
                            <h5 id="notification-message" class="form-header text-center pl-3 pr-3" style="padding-top:50px; font-size:16px;">Notification</h5>
                            <button class="btn d-block mr-auto ml-auto mt-3 w-50" style="background-color:#004643; color:white" onclick="CloseModal('notification-invalid-comment-modal')">Ok</button>
                        </div>`
    $('#notification-invalid-comment-modal').html(confirmation);
    OpenNotificationModal('The comment you have entered is invalid!', 'notification-invalid-comment-modal')
    $('#chatInput').text('')
    CheckIfEmpty()
}