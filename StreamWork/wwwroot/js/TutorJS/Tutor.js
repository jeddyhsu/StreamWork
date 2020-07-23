function ReadImageUrl(image, destination) {
    if (image != null && image.files.length > 0) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#' + destination).attr("src", e.target.result)
        }

        reader.readAsDataURL(image.files[0]);
    }
}

