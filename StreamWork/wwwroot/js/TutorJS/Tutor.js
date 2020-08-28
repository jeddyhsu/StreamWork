function ReadImageUrl(image, type) {
    cropperType = type;
    if (image != null && image.files.length > 0) {
        var reader = new FileReader();
        reader.onload = function (e) {
            if (type == "Banner") {
                $('#imagecropper-image').attr("src", reader.result)
                OpenModal('imagecropper-modal')
                OpenCropper(1120, 300, 3)
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

            image.value = ""
        }

        reader.readAsDataURL(image.files[0]);
    }
}