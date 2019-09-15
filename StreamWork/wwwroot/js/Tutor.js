
//Changes Stream
function RegisterStreamTitleAndStreamSubjectAndCustomThumbanail() {
    var streamTitle = $('#streamTitle').val();
    var streamSubject = $('#streamSubject').val();
    var formData = new FormData()
   
    if (streamTitle == "" || streamSubject == 'Select Subject') {
        alert("You must have title for your stream and you must select a subject")
        return;
    }

    var streamInfo = streamTitle + '|' + streamSubject;
    var totalFile = document.getElementById("uploadThumbnail").files.length;
    if (totalFile != 0) {
        formData.append(streamInfo, document.getElementById("uploadThumbnail").files[0])
    }
    else {
        formData.append(streamInfo, 'No Thumbnail');
    }


      $.ajax({
            url: '/Tutor/TutorStream',
            type: 'post',
            dataType: 'json',
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.message === "Saved") {;
                    alert("Your broadcast is visible to students!");
                    location.reload();
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

//Sends profile caption and paragraph to backend for saving
    function RegisterProfilePhotoAndCaption() {
        var formData = new FormData();
        var totalFiles = document.getElementById("uploadProfilePic").files.length;
        if(totalFiles > 0){
            for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("uploadProfilePic").files[i];
            var streamName = $("#ProfileCaption").val() + "|" + $("#ProfileParagraph").val();
            formData.append(streamName, file);
            }
        }
        else{
            var streamName = $("#ProfileCaption").val() + "|" + $("#ProfileParagraph").val();
            formData.append(streamName, file);
        }
       
        $.ajax({
            type: "POST",
            url: '/Tutor/ProfileTutor',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.message === "Success") {
                    location.reload()
                }
            }
        });
    }

function ValidateKey() {
    $.ajax({
        url: '/Tutor/TutorStream',
        type: 'post',
        dataType: 'json',
        data: {
            'channelKey': $('#channelKey').val()
        },
        success: function (data) {
            if (data.message === "Success") {
                $('#channelKeyModal').modal('hide')
                alert("Key validated, welcome StreamTutor")
            }
            else {
                alert("Invalid channel key, make sure you have entered the channel key correctley")
            }
        }
    });
}

function PrintLoad() {
    alert("data has loaded!!!")
}

function RetreivePlayer() {
    var myPlayer = dacast.players["135034_c_505911"];
    //var isPlaying = myPlayer.playing();
}
