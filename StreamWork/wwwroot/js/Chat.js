const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .withAutomaticReconnect([0, 1000, 5000, 10000, 15000, 20000, 30000, 45000, 60000, null])
    .configureLogging(signalR.LogLevel.Information)
    .build();

var clientUsername = "";
var toolTipCount = 0;
var chatCount = 0;
var muted = true;
var date = null;

connection.on("ReceiveMessage", function (chat) {
    chat = JSON.parse(chat);
    var date = moment(chat.Date).format("HH:mm")
    var listName = "";

    if (chat.ChatId == chat.Username) {
        listName = ` <p class="chat-name" style="color:${chat.ChatColor}">${chat.Name.replace('|', ' ')}
                        <span><img id="tutor-tip-${toolTipCount}" class="pl-1" src="/images/ChatAssets/Tutor.png" data-toggle="tooltip" data-placement="top" title="StreamTutor"></span>
                        <span class='chat-date'>${date}</span>
                     </p>`
    }
    else if (clientUsername == chat.Username) {
        listName = `<p class="chat-name" style="color:${chat.ChatColor}">${chat.Name.replace('|', ' ')} (you)
                         <span class="chat-date">${date}</span>
                    </p>`
    }
    else {
        listName = `<p class="chat-name" style="color:${chat.ChatColor}">${chat.Name.replace('|', ' ')}
                        <span class="chat-date">${date}</span>
                    </p>`
    }

    var colorWay = "";
    if ((chatCount + 1) % 2 != 0) colorWay = "chat-list-primary-color-way"
    else colorWay = "chat-list-secondary-color-way"

    var listItem = `<li id="message-${chatCount}" class='list-group-item ${colorWay} border-left-0 border-right-0'>
                            <div class="row">
                                <div class="col-12">
                                    <input align="left" type="image" class="chat-profile-picture rounded" src=${chat.ProfilePicture} />
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
    if (!muted && initialUserName != userName) PlayAudio();
});

function PlayAudio() {
    var audioElement = document.createElement('audio');
    audioElement.setAttribute('src', '/media/juntos.mp3')
    var promise = audioElement.play();
    promise.then(function () {
        document.getElementById("sound").src = '/images/ChatAssets/Unmute.png';
        muted = false;

    }).catch(function () {
        return document.getElementById("sound").src = '/images/ChatAssets/Mute.png';
    })
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
    clientUsername = userName;
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
            date = new Date();
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
