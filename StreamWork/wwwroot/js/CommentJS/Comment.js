var commentCount = 0;
function SaveComment(parentId, masterParent) {
    var message = "";
    if (parentId == "" || parentId == null) {
        message = GetStringWithoutAt('send', '');
        ButtonEnabledDisabled('send', '', true);
    }
    else {
        message = GetStringWithoutAt('reply', parentId);
        ButtonEnabledDisabled('reply', parentId, true);
    }

    $.ajax({
        url: '/Comments/CommentModel/?handler=SaveComment',
        type: 'POST',
        datatype: 'json',
        data: {
            'senderUsername': $('#comment-username').val(),
            'receiverUsername': $('#comment-tutor-username').val(),
            'message': message,
            'parentId': parentId,
            'masterParent': masterParent,
            'streamId': $('#comment-streamId').val(),
        },
         beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                var reply = ``;
                var at = ``;
                if (parentId == "" || parentId == null) {
                    reply = `<a class="comment-replies" onclick="ShowReplyBox('${data.comment.senderProfilePicture}', '${data.comment.id}', '${data.comment.senderName.replace('|', ' ')}')"><b>Reply</b></a>`
                }
                else {
                    IncrementDecrementComments(data.comment.senderProfilePicture, data.comment.id, data.comment.receiverName.replace('|', ' '), masterParent == "undefined" ? parentId : masterParent)
                    reply = `<a class="comment-replies" onclick="ShowReplyBox('${data.comment.senderProfilePicture}', '${data.comment.id}', '${data.comment.senderName.replace('|', ' ')}', '${masterParent == "undefined" ? parentId : masterParent}')"><b>Reply</b></a>`
                    at = `<span id="comment-at-${parentId}" class="comment-at" contenteditable="false"><b>@${data.comment.receiverName.replace('|', ' ')} </b></span>`
                }
                var date = moment().format("MMM DD YYYY");
                var comment = `<li id="comment-${data.comment.id}" class="border-bottom border-left border-right" style="background-color:white">
                                    <div class="card border-0">
                                        <div class="card-body">
                                            <img class="comment-profile-picture" align="left" src="${data.comment.senderProfilePicture}"/>
                                            <img src="/images/GenericAssets/Trash.png" class="comment-remove pl-1 float-right" id="comment-remove-${data.comment.id}" onclick="OpenDeleteConfirmation('${data.comment.id}', '${parentId}')" />
                                            <img src="/images/GenericAssets/Edit.png" class="comment-edit pl-1 float-right" id="comment-edit-${data.comment.id}" onclick="ShowEditBox('${data.comment.senderProfilePicture}', '${data.comment.id}')" />
                                            <script>
                                                $('#comment-${data.comment.id}').hover(function () {
                                                    $('#comment-edit-${data.comment.id}').css('display', 'block')
                                                    $('#comment-remove-${data.comment.id}').css('display', 'block')
                                                }, function () {
                                                    $('#comment-edit-${data.comment.id}').css('display', 'none')
                                                    $('#comment-remove-${data.comment.id}').css('display', 'none')
                                                })
                                            </script>
                                            <p class="form-header comment-name mb-0" style="color:${data.comment.profileColor}">${data.comment.senderName.replace('|', ' ')}<span class="form-sub-header ml-2" style="font-size:10px; font-family:'Roboto', serif">${date}<span id="edited-holder-${data.comment.id}" class="ml-2"></span></span></p>
                                            <input type="hidden" id="comment-send-hidden-${data.comment.id}" value="${message}" />
                                            <div id="comment-send-holder-${data.comment.id}">
                                                <p class="mb-1 comment-send" id="comment-send-${data.comment.id}">${at}${data.comment.message}</p>
                                            </div>
                                            ${reply}
                                            <input id="comment-replies-count-${data.comment.id}" type="hidden" value="0" />
                                            <a class="comment-replies pl-1" id="show-replies-${data.comment.id}" style="display:none" onclick="ShowReplyComments('${data.comment.id}')"><b>Show 0 Replies</b></a>
                                            <div style="padding-left:40px" id="reply-box-${data.comment.id}"></div>
                                            <ul class="comment-list" style="display:none" class="mt-2" id="comment-reply-list-${data.comment.id}"></ul>
                                        </div>
                                    </div>
                                </li>`
                if (parentId == "" || parentId == null) {
                    $('#comment-list').append(comment);
                    $('#comment-send-').html(``)
                    $('#comment-send-').attr('style', '40px !important');
                    GoToComment("", data.comment.id)
                }
                else {
                    var id = masterParent == "undefined" ? parentId : masterParent;
                    $('#comment-reply-list-' + id).append(comment);
                    CancelReply(parentId)
                    GoToComment(parentId, data.comment.id)
                }
            }
        }
    });
}

function IncrementDecrementComments(profilePicture, id, receiverName, parentId) {
    var numberOfComments = parseInt($("#comment-replies-count-" + parentId).val());
    var currentText = $('#show-replies-' + parentId).text().split(' ');
    $('#show-replies-' + parentId).css("display", "inline-block");
    $('#show-replies-' + parentId).html(`<b>${currentText[0]} ${numberOfComments + 1} Replies</b>`);
    $("#comment-replies-count-" + parentId).val(numberOfComments + 1)
}

function DeleteComment(commentId, parentId) {
    $.ajax({
        url: '/Comments/CommentModel/?handler=DeleteComment',
        type: 'POST',
        datatype: 'json',
        data: {
            'commentId': commentId,
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (parentId != null) {
                var numberOfComments = parseInt($("#comment-replies-count-" + parentId).val());
                if (numberOfComments - 1 == 0) {
                    $('#show-replies-' + parentId).css("display", "none");
                }
                else {
                    var replyWord = "Replies";
                    if (numberOfComments - 1 == 1) {
                        replyWord = "Reply";
                    }
                    var currentText = $('#show-replies-' + parentId).text().split(' ');
                    $('#show-replies-' + parentId).html(`<b>${currentText[0]} ${numberOfComments - 1} ${replyWord}</b>`);
                }
            }
            $("#comment-replies-count-" + parentId).val(numberOfComments - 1)
            $('#comment-' + commentId).remove();
            CloseModal('notification-delete-comment-modal');
        }
    });
}

function EditComment(commentId) {
    $.ajax({
        url: '/Comments/CommentModel/?handler=EditComment',
        type: 'POST',
        datatype: 'json',
        data: {
            'message': $('#comment-save-' + commentId).html().replace(/<div>/gi, '<br>').replace(/<\/div>/gi, ''),
            'commentId': commentId,
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            $('#comment-send-hidden-' + data.comment.id).val(data.comment.message);
            $('#comment-edit-' + data.comment.id).css("display", "block");
            $('#comment-remove-' + data.comment.id).css("display", "block");
            var at = data.comment.parentId != null ? `<span id="comment-at-${data.comment.id}" class="comment-at" contenteditable="false"><b>@${data.comment.receiverName.replace('|', ' ')}</b></span>` : ``
            $('#comment-send-holder-' + data.comment.id).html(`<p class="mb-1 comment-send" id="comment-send-${data.comment.id}">${at}${data.comment.message}</p>`)
            $('#edited-holder-' + data.comment.id).html(`(edited)`);
        }
    });
}

function ShowEditBox(profilePicture, commentId) {
    var message = $('#comment-send-hidden-' + commentId).val();
    var edit = `<div class="d-flex flex-row">
                    <div id="comment-save-${commentId}" class="comment-send-reply-textarea form-custom-textarea ml-2 mb-1 comment-text" onkeydown="ButtonEnabledDisabled('save', '${commentId}');" onkeyup="ButtonEnabledDisabled('save', '${commentId}')" contenteditable="true">${message}</div>
                    <button onclick="HideEditBox('${commentId}')" class="streamWork-secondary comment-cancel-button ml-2">Cancel</button>
                    <button id="save-comment-button-${commentId}" onclick="EditComment('${commentId}')" class="streamWork-primary comment-send-reply-button ml-2">Save</button>
                </div>`

    $('#comment-edit-' + commentId).css("display", "none");
    $('#comment-remove-' + commentId).css("display", "none");
    $('#comment-send-holder-' + commentId).html(edit);

    ButtonEnabledDisabled('save', commentId)
}

function HideEditBox(commentId) {
    var message = $('#comment-send-hidden-' + commentId).val();
    $('#comment-edit-' + commentId).css("display", "block");
    $('#comment-remove-' + commentId).css("display", "block");
    $('#comment-send-holder-' + commentId).html(`<p class="mb-1 comment-send" id="comment-send-${commentId}">${message}</p>`);
}

function ShowReplyBox(profilePicture, id, senderName, parentId) {
    var reply = `<div class="card rounded-0 comment-send-reply-box mt-2">
                    <div class="card-body w-100"> 
                        <div class="d-flex flex-row">
                            <img class="comment-profile-picture" src="${profilePicture}" />
                            <div id="comment-reply-${id}" class="comment-send-reply-textarea form-custom-textarea ml-2 mb-1 comment-text" onkeydown="ButtonEnabledDisabled('reply', '${id}');" onkeyup="ButtonEnabledDisabled('reply', '${id}')" contenteditable="true">
                               <span id="comment-at-${id}" class="comment-at comment-text" contenteditable="false"><b>@${senderName.replace('|', ' ')} </b></span>
                            </div>
                            <button onclick="CancelReply('${id}')" class="streamWork-secondary comment-cancel-button ml-2">Cancel</button>
                            <button id="reply-comment-button-${id}" onclick="SaveComment('${id}', '${parentId}')" class="streamWork-primary comment-send-reply-button ml-2">Reply</button>
                        </div>
                    </div>
                 </div>`

    $('#reply-box-' + id).html(reply);
    ButtonEnabledDisabled('reply', id)
}

function CancelReply(parentId) {
    $('#comment-cancel-' + parentId).css("display", "none");
    $('#reply-box-' + parentId).html("");
}

function ShowReplyComments(parentId) {
    $('#show-replies-' + parentId).attr('onclick', 'HideReplyComments("' + parentId + '")')
    var numberOfComments = parseInt($("#comment-replies-count-" + parentId).val());
    $('#show-replies-' + parentId).html(`<b>Hide ${numberOfComments} Replies</b>`);
    $('#comment-reply-list-' + parentId).css("display", "block")
}

function HideReplyComments(parentId) {
    $('#show-replies-' + parentId).attr('onclick', 'ShowReplyComments("' + parentId + '")')
    var numberOfComments = parseInt($("#comment-replies-count-" + parentId).val());
    $('#show-replies-' + parentId).html(`<b>Show ${numberOfComments} Replies</b>`);
    $('#comment-reply-list-' + parentId).css("display", "none")
}

function ButtonEnabledDisabled(type, id, forceDisable) {
    if (forceDisable == true || GetStringWithoutAt(type, id) == "" || $('#comment-' + type + '-' + id).text() == "") {
        document.getElementById(type + "-comment-button-" + id).disabled = true;
        $('#' + type + '-comment-button-' + id).css('border-color', 'transparent');
        $('#' + type + '-comment-button-' + id).css('background-color', 'grey');
    }
    else {
        document.getElementById(type + "-comment-button-" + id).disabled = false;
        $('#' + type + '-comment-button-' + id).css('background-color', '#004643');
    }
}

function GetStringWithoutAt(type, id) {
    var commentAt = $('#comment-at-' + id).text()
    var clone = $('#comment-' + type + '-' + id).clone()
    clone.find('.comment-at').remove();
    var commentHtml = clone.html().replace(/<div>/gi, '<br>').replace(/<\/div>/gi, '')
    var comment = commentHtml.trim() + " "
    var commentString = comment.replace(commentAt, '');

    return commentString;
}

function OpenDeleteConfirmation(commentId, parentId) {
    var confirmation = ` <div class="custom-modal-content" style="width:300px; height:200px">
                            <div class="close ml-auto" onclick="CloseModal('notification-delete-comment-modal')">&times;</div>
                            <h5 id="notification-message" class="form-header text-center pl-3 pr-3" style="padding-top:50px; font-size:16px;">Notification</h5>
                            <div class="btn-group " style="margin-top:20px; margin-left:75px;">
                                <button class="btn" style="background-color:#AC0001; color:white" onclick="DeleteComment('${commentId}','${parentId}')">Delete</button>
                                <button class="btn" style="background-color:#004643; color:white" onclick="CloseModal('notification-delete-comment-modal')">Cancel</button>
                            </div>
                        </div>`
    $('#notification-delete-comment-modal').html(confirmation);
    OpenNotificationModal('Are you sure you want to delete? This is a irreverible action!', 'notification-delete-comment-modal')

}

function GoToComment(parentId, commentId) {
    if (parentId != "" || parentId != null) {
        ShowReplyComments(parentId)
    }

    $('html, body').animate({
        scrollTop: ($('#comment-' + commentId).position().top + 300)
    }, 500);
}