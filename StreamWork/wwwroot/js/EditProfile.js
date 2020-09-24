const acceptedImageTypes = ['image/jpeg', 'image/png'];


function ReadImageUrl(image, type) {
    cropperType = type;
    if (image != null && image.files.length > 0) {
        if (!acceptedImageTypes.includes(image.files[0].type)) {
            OpenNotificationModal('File must be either PNG or JPG.', 'notification-image-invalid-modal');
            return;
        }

        if ((image.files[0].size / 1024) > 2048) {
            OpenNotificationModal('Image too large. 2MB max.', 'notification-image-invalid-modal');
            return;
        }

        $('#cropper-buttons').html(` <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#6B6B6B; color:white" onclick="SendCroppedImage()">Save Image</button>
                                     <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="UploadImage()">Change Image</button>`)

        var reader = new FileReader();
        reader.onload = function (e) {
            if (type == "Banner") {
                OpenModal('imagecropper-modal')
                $('#image-container').html(`<img id="imagecropper-image" src="${reader.result}">`)
                OpenCropper(1120, 300, 3)
            }
            else if (type == "Profile Picture") {
                $('#image-container').html(`<img id="imagecropper-image" src="${reader.result}">`)
                OpenModal('imagecropper-modal')
                OpenCropper(240, 320, 1)
            }
            else if (type == "Thumbnail Edit" || type == "Thumbnail") {
                $('#imagecropper-image').attr("src", reader.result)
                OpenModal('imagecropper-modal')
                OpenCropper(960, 540, 3)
            }

            image.value = ""
        }

        reader.readAsDataURL(image.files[0]);
    }
}

function ReadImageSrcUrl(type) {
    cropperType = type;
    var img = document.getElementById('preview-profile-banner');
    if (type == "Banner") {
        OpenModal('imagecropper-modal')
        $('#imagecropper-image').attr('src', img.src)
        //OpenCropper(1120, 300, 3)
    }
    else if (type == "Profile Picture") {
        $('#imagecropper-image').attr("src", reader.result)
        OpenModal('imagecropper-modal')
        OpenCropper(240, 320, 1)
    }
    else if (type == "Thumbnail Edit" || type == "Thumbnail") {
        $('#imagecropper-image').attr("src", reader.result)
        OpenModal('imagecropper-modal')
        OpenCropper(960, 540, 3)
    }
}