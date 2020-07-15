var commentCount = 0;

function SaveComment(parentId) {
    var message = "";
    if (parentId == "" || parentId == null) {
        message = $('#comment-send-').val();
    }
    else {
        message = $('#comment-reply-' + parentId).val();
    }

    $.ajax({
        url: '/Comment/CommentModel/?handler=SaveComment',
        type: 'POST',
        datatype: 'json',
        data: {
            'senderUsername': $('#comment-tutor-username').val(),
            'receiverUsername': $('#comment-username').val(),
            'message': message,
            'parentId': parentId,
            'streamId': $('#comment-streamId').val(),
        },
         beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                var reply = ``;
                if (parentId == "" || parentId == null) {
                    reply = `<a class="comment-replies" onclick="ShowReplyBox('${data.savedInfo[0]}', '${data.savedInfo[3]}')"><b>Reply</b></a>`
                }
                else {
                    var numberOfComments = parseInt($("#comment-replies-count-" + parentId).val());
                    var currentText = $('#show-replies-' + parentId).text().split(' ');
                    $('#show-replies-' + parentId).css("display", "inline-block");
                    $('#show-replies-' + parentId).html(`<b>${currentText[0]} ${numberOfComments + 1} Replies</b>`);
                    $("#comment-replies-count-" + parentId).val(numberOfComments + 1)
                }

                var date = moment().format("MMM DD YYYY");
                var comment = `<li id="comment-${data.savedInfo[3]}" class="border-bottom border-left border-right" style="background-color:white">
                                    <div class="card border-0">
                                        <div class="card-body">
                                            <img class="comment-profile-picture" align="left" src="${data.savedInfo[0]}"/>
                                            <img src="/images/GenericAssets/Trash.png" class="comment-remove pl-1 float-right" id="comment-remove-${data.savedInfo[3]}" onclick="OpenDeleteConfirmation('${data.savedInfo[3]}', '${parentId}')" />
                                            <img src="/images/GenericAssets/Edit.png" class="comment-edit pl-1 float-right" id="comment-edit-${data.savedInfo[3]}" onclick="ShowEditBox('${data.savedInfo[0]}', '${data.savedInfo[3]}')" />
                                            <p class="form-header comment-name mb-0">${data.savedInfo[1]}<span class="form-sub-header ml-2" style="font-size:10px; font-family:'Roboto', serif">${date}<span id="edited-holder-${data.savedInfo[3]}" class="ml-2"></span></span></p>
                                            <input type="hidden" id="comment-send-hidden-${data.savedInfo[3]}" value="${data.savedInfo[2]}" />
                                            <div id="comment-send-holder-${data.savedInfo[3]}">
                                                <p class="mb-1 comment-send" id="comment-send-${data.savedInfo[3]}">${data.savedInfo[2]}</p>
                                            </div>
                                            ${reply}
                                            <input id="comment-replies-count-${data.savedInfo[3]}" type="hidden" value="0" />
                                            <a class="comment-replies pl-1" id="show-replies-${data.savedInfo[3]}" style="display:none" onclick="ShowReplyComments('${data.savedInfo[3]}')"><b>Show 0 Replies</b></a>
                                            <div style="padding-left:40px" id="reply-box-${data.savedInfo[3]}"></div>
                                            <ul class="comment-list" style="display:none" class="mt-2" id="comment-reply-list-${data.savedInfo[3]}"></ul>
                                        </div>
                                    </div>
                                </li>`
                if (parentId == "" || parentId == null) {
                    $('#comment-list').append(comment);
                    $('#comment-send-').val("")
                    $('#comment-send-').attr('style', '40px !important');
                    ButtonEnabledDisabled('send', '');
                }
                else {
                    $('#comment-reply-list-' + parentId).append(comment);
                    $('#comment-reply-' + parentId).val("")
                    ButtonEnabledDisabled('reply', parentId);
                }
            }
        }
    });
}

function DeleteComment(commentId, parentId) {
    $.ajax({
        url: '/Comment/CommentModel/?handler=DeleteComment',
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
            CloseModal('notification-modal');
        }
    });
}

function EditComment(commentId) {
    $.ajax({
        url: '/Comment/CommentModel/?handler=EditComment',
        type: 'POST',
        datatype: 'json',
        data: {
            'message': $('#comment-save-' + commentId).val(),
            'commentId': commentId,
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            $('#comment-send-hidden-' + commentId).val(data.savedInfo[0]);
            $('#comment-edit-' + commentId).css("display", "block");
            $('#comment-remove-' + commentId).css("display", "block");
            $('#comment-send-holder-' + commentId).html(`<p class="mb-1 comment-send" id="comment-send-${data.savedInfo[1]}">${data.savedInfo[0]}</p>`)
            $('#edited-holder-' + commentId).html(`(edited)`);
        }
    });
}

function ShowEditBox(profilePicture, commentId) {
    var message = $('#comment-send-hidden-' + commentId).val();
    var edit = `<div class="d-flex flex-row">
                    <textarea id="comment-save-${commentId}" class="form-control form-textarea comment-send-reply-textarea ml-2 mb-1" onkeydown="ButtonEnabledDisabled('save', '${commentId}')" onkeyup="ButtonEnabledDisabled('save', '${commentId}')">${message}</textarea>
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

function ShowReplyBox(profilePicture, parentId) {
    var reply = `<div class="card rounded-0 comment-send-reply-box mt-2">
                    <div class="card-body w-100">
                        <div class="d-flex flex-row">
                            <img class="comment-profile-picture" src="${profilePicture}" />
                            <textarea id="comment-reply-${parentId}" class="form-control form-textarea comment-send-reply-textarea ml-2 mb-1" placeholder="Send reply..." onkeydown="ButtonEnabledDisabled('reply', '${parentId}')" onkeyup="ButtonEnabledDisabled('reply', '${parentId}')"></textarea>
                            <button onclick="CancelReply('${parentId}')" class="streamWork-secondary comment-cancel-button ml-2">Cancel</button>
                            <button id="reply-comment-button-${parentId}" onclick="SaveComment('${parentId}')" class="streamWork-primary comment-send-reply-button ml-2">Reply</button>
                        </div>
                    </div>
                 </div>`

    $('#reply-box-' + parentId).html(reply);
    ButtonEnabledDisabled('reply', parentId)
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

function OpenDeleteModal() {
    
}

function ButtonEnabledDisabled(type, id) {
    if ($('#comment-' + type + '-' + id).val() == "") {
        document.getElementById(type + "-comment-button-" + id).disabled = true;
        $('#' + type + '-comment-button-' + id).css('border-color', 'transparent');
        $('#' + type + '-comment-button-' + id).css('background-color', 'grey');
    }
    else {
        document.getElementById(type + "-comment-button-" + id).disabled = false;
        $('#' + type + '-comment-button-' + id).css('background-color', '#004643');
    }
}

function OpenDeleteConfirmation(commentId, parentId) {
    var confirmation = ` <div class="custom-modal-content" style="width:300px; height:200px">
                            <div class="close ml-auto" onclick="CloseModal('notification-modal')">&times;</div>
                            <h5 id="notificationMessage" class="form-header text-center pl-3 pr-3" style="padding-top:50px; font-size:16px;">Notification</h5>
                            <div class="btn-group " style="margin-top:20px; margin-left:75px;">
                                <button class="btn" style="background-color:#AC0001; color:white" onclick="DeleteComment('${commentId}','${parentId}')">Delete</button>
                                <button class="btn" style="background-color:#004643; color:white" onclick="CloseModal('notification-modal')">Cancel</button>
                            </div>
                        </div>`
    $('#notification-modal').html(confirmation);
    OpenNotificationModal('Are you sure you want to delete? This is a irreverible action!', 'notification-modal', 'Failed')

}