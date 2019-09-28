
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
                location.reload()
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
