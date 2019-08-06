//Changes Stream
$(document).ready(function () {

    $('select').on('change', function () {
        $.ajax({
            url: '/Tutor/TutorStream',
            type: 'post',
            dataType: 'json',
            data: {
                'streamTitle': $('#streamTitle').val(),
                'streamSubject': $('#streamSubject').val()
            },
            success: function (data) {
                if (data.message === "Saved") {
                    location.reload()
                    alert("Broadcast has started")
                }
            }
        });
    });
   
});

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
    function RegisterThumbnailAndStreamTitle() {
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
            url: '/Tutor/ProfileTutor',
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
            if (data.message === "Saved") {
                location.reload()
                alert("Key validated, welcome StreamTutor")
            }
        }
    })
}
