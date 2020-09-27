const acceptedImageTypes = ['image/jpeg', 'image/png'];


function ReadImageUrl(image, type) {
    if (image != null && image.files.length > 0) {
        if (!acceptedImageTypes.includes(image.files[0].type)) {
            OpenNotificationModal('File must be either PNG or JPG.', 'notification-image-invalid-modal');
            return;
        }

        if ((image.files[0].size / 1024) > 2048) {
            ShowBannerNotification('image-too-large-notification');
            return;
        }

        var reader = new FileReader();
        reader.onload = function (e) {
            if (type == "Banner") {
                $('#image-container').html(`<img id="imagecropper-image" src="${reader.result}">`)
                OpenModal('imagecropper-modal')
                OpenCropper(1120, 300, 3)
            }
            else if (type == "Profile Picture") {
                $('#image-container').html(`<img id="imagecropper-image" src="${reader.result}">`)
                OpenModal('imagecropper-modal')
                OpenCropper(240, 320, 1)
            }
            else if (type == "Thumbnail Edit" || type == "Thumbnail") {
                $('#image-container').html(`<img id="imagecropper-image" src="${reader.result}">`)
                OpenModal('imagecropper-modal')
                OpenCropper(960, 540, 3)
            }

            ResetButtons();
            $('#cropper-buttons').html(` <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#6B6B6B; color:white" onclick="UploadImage()">Change Image</button>
                                         <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="SendCroppedImage()">Save Image</button>`)

            image.value = ""
        }

        reader.readAsDataURL(image.files[0]);
    }
}

function ReadImageSrcUrl(type, dImage) {
    cropperType = type;
    if (type == "Banner") {
        $('#image-container').html(`<img id="imagecropper-image" src="${document.getElementById('preview-profile-banner').src}" width="100%"/>`);
        CheckIfImageIsDefaultImage(dImage)
        OpenModal('imagecropper-modal')
        
    }
    else if (type == "Profile Picture") {
        $('#image-container').html(`<img id="imagecropper-image" src="${document.getElementById('preview-profile-picture').src}" class="d-block mr-auto ml-auto" style="width:320px;"/>`);
        CheckIfImageIsDefaultImage(dImage)
        OpenModal('imagecropper-modal')
    }
    else if (type == "Thumbnail Edit" || type == "Thumbnail") {
        if (type == "Thumbnail Edit") {
            $('#image-container').html(`<img id="imagecropper-image" src="${document.getElementById('preview-video-thumbnail-edit').src}" class="d-block mr-auto ml-auto" style="height:450px;"/>`);
        }
        else {
            $('#image-container').html(`<img id="imagecropper-image" src="${document.getElementById('preview-stream-thumbnail').src}" class="d-block mr-auto ml-auto" style="height:450px;"/>`);
        }
       
        OpenModal('imagecropper-modal')
        $('#cropper-buttons').html(`<button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="UploadImage()">Change Image</button>`)
    }
}

function CheckIfImageIsDefaultImage(dImage) {
    if (cropperType == "Banner") {
        if (dImage == $('#preview-profile-banner').attr('src')) {
            $('#cropper-buttons').html(`<button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="UploadImage()">Change Image</button>`)
        }
    }
    else if (cropperType == "Profile Picture") {
        if (dImage == $('#preview-profile-picture').attr('src')) {
            $('#cropper-buttons').html(`<button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="UploadImage()">Change Image</button>`)
        }
    }
}