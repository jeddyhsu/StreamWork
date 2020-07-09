var commentCount = 0;

function SaveComment(parentId) {
    var message = "";
    if (parentId == "" || parentId == null) message = $('#comment-message').val();
    else message = $('#comment-reply').val();

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
                var replies = ``;
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

                var comment = `<li class="border-bottom border-left border-right" style="background-color:white">
                                    <div class="card border-0">
                                        <div class="card-body">
                                            <img class="comment-profile-picture" align="left" src="${data.savedInfo[0]}"/>
                                            <p class="form-header comment-name mb-0">${data.savedInfo[1]}</p>
                                            <p class="mb-1 comment-message">${data.savedInfo[2]}</p>
                                            ${reply}
                                            <a class="comment-cancel pl-1" id="comment-cancel-${data.savedInfo[3]}" onclick="CancelReply('${data.savedInfo[3]}')"><b>Cancel</b></a>
                                            <input id="comment-replies-count-${data.savedInfo[3]}" type="hidden" value="0" />
                                            <a class="comment-replies pl-1" id="show-replies-${data.savedInfo[3]}" style="display:none" onclick="ShowReplyComments('${data.savedInfo[3]}')"><b>Show 0 Replies</b></a>
                                            <div style="padding-left:40px" id="reply-box-${data.savedInfo[3]}"></div>
                                            <ul class="comment-list" style="display:none" class="mt-2" id="comment-reply-list-${data.savedInfo[3]}"></ul>
                                        </div>
                                    </div>
                                </li>`
                if (parentId == "" || parentId == null) {
                    $('#comment-list').append(comment);
                    $('#comment-message').val("")
                }
                else {
                    $('#comment-reply-list-' + parentId).append(comment);
                    $('#comment-reply').val("")
                }
            }
        }
    });
}

function ShowReplyBox(profilePicture, parentId) {
    var reply = `<div class="card rounded-0 comment-send-reply-box mt-2">
                    <div class="card-body w-100">
                        <div class="d-flex flex-row">
                            <img class="comment-profile-picture" src="${profilePicture}" />
                            <textarea id="comment-reply" class="form-control form-textarea comment-send-reply-textarea ml-2 mb-1" placeholder="Send reply..."></textarea>
                            <button onclick="SaveComment('${parentId}')" class="streamWork-primary comment-send-reply-button ml-2">Reply</button>
                        </div>
                    </div>
                 </div>`
    $('#comment-cancel-' + parentId).css("display", "inline-block");
    $('#reply-box-' + parentId).html(reply);
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
