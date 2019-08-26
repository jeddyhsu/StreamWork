//Changes Stream
function RegisterStreamTitleAndStreamSubject() {

    var streamTitle = $('#streamTitle').val();
    var streamSubject = $('#streamSubject').val();

    if (streamTitle == "" || streamSubject == 'Select Subject') {
        alert("You must have title for your stream and you must select a subject")
        return;
    }

      $.ajax({
            url: '/Tutor/TutorStream',
            type: 'post',
            dataType: 'json',
            data: {
                'streamTitle': streamTitle,
                'streamSubject': streamSubject
            },
            success: function (data) {
                if (data.message === "Saved") {
                    location.reload()
                    alert("Your broadcast is visible to students!")
                }
            }
        });
}

 //Opens editing MODAL
 function EditProfile() {
        $('#myModal').modal('show')
    }
    
//Reads picture url and sends to backend for saving
 function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#blah')
                    .attr('src', e.target.result);
            };
            reader.readAsDataURL(input.files[0]);
        }
    }

//Sends profile caption and paragraph to backend for saving
    function RegisterProfilePhotoAndCaption() {
        var formData = new FormData();
        var totalFiles = document.getElementById("file-upload").files.length;
        if(totalFiles > 0){
            for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("file-upload").files[i];
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
    
//Stop LiveStream
function StopStream(stop) {
        $.ajax({
            url: '/Tutor/TutorStream',
            type: 'post',
            dataType: 'json',
            data: {
                'Stop': stop
            },
            success: function (data) {
                if (data.message === "Stopped") {
                    location.reload()
                    alert("Your broadcast has stopped");
                } else {
                    alert("Broadcast Failed to Start");
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
