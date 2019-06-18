function RegisterThumbnailAndStreamTitle(){
    $("#uploader").change(function () {
         var formData = new FormData();
         var totalFiles = document.getElementById("imageUploadForm").files.length;
         for (var i = 0; i < totalFiles; i++) {
           var file = document.getElementById("imageUploadForm").files[i];
           formData.append("imageUploadForm", file);
         }

         $.ajax({
           type: "POST",
           url: '/Home/Upload',
           data: formData,
           dataType: 'json',
           contentType: false,
           processData: false
             //success: function(response) {
             //    alert('succes!!');
             //},
             //error: function(error) {
             //    alert("errror");
             //}
         });
    })
         
}
          