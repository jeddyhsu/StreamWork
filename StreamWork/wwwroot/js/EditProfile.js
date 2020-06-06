
    //Sends profile caption and paragraph to backend for saving
    function RegisterProfilePhotoAndCaption() {
        var formData = new FormData();
        var profileCaption = $("#ProfileCaption").val()
        var profileParagraph = $("#ProfileParagraph").val()
        var totalFiles = document.getElementById("uploadProfilePic");

        formData.append("ProfileCaption", profileCaption);
        formData.append("ProfileParagraph", profileParagraph);
        if (totalFiles.files.length > 0)
            formData.append("ProfilePicture", totalFiles.files[0]);
       
        $.ajax({
            type: "POST",
            url: '/Home/EditProfileInformation',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.message === "Success") {
                    document.getElementById('ProfileCaption').value = data.caption;
                    document.getElementById('ProfileCaptionOnPage').innerHTML = data.caption;
                    document.getElementById('ProfileParagraph').value = data.paragraph;
                    document.getElementById('ProfileParagraphOnPage').innerHTML = data.paragraph;
                    if (data.picture != null) {
                        document.getElementById('previewProfilePic').src = data.picture;
                        document.getElementById('ProfilePictureOnPage').src = data.picture; 
                    }
                    $('#editModal').modal('hide');
                    OpenNotificationModalSuccess("Changes have been saved")
                }
            }
        });
    }

 //Opens editing MODAL
 function EditProfile() {
     $('#editModal').modal('show')
 }
    
//Reads picture url and sends to backend for saving
 function readURL(input,pictureType) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            if (pictureType == 'Thumbnail') {
                reader.onload = function (e) {
                    $('#previewThumbnail')
                        .attr('src', e.target.result);
                }
            }
            else {
            reader.onload = function (e) {
                    $('#previewProfilePic')
                        .attr('src', e.target.result);
                }
            }
            reader.readAsDataURL(input.files[0]);
        }
    }

function Dashboard(profileType) {
    if (profileType == "tutor") {
        window.location.href = "/Tutor/ProfileTutor"
    }
    else {
         window.location.href = "/Student/ProfileStudent"
    }
}

function Logout() {
    $.ajax({
        url: '/Home/Logout',
        type: 'post',
        dataType: 'json',
        data: {
            'Logout': 'logout'
        },
        success: function (data) {
            if (data.message === "Success") {
                window.location.href = "/"
            }
        }
    })
}

function encodeURL(url) {
    var encodedURI = encodeURIComponent(url);
    window.location.href = encodedURI;
}

//Notifications
function OpenNotificationModal(body) {
    var notification = document.getElementById('notifyBody');
    notification.textContent = body;
    $('#notifyModal').modal('show')
}

function OpenNotificationModalSuccess(body) {
    var notification = document.getElementById('notifyBodySuccess');
    notification.textContent = body;
    $('#notifyModalSuccess').modal('show')
}