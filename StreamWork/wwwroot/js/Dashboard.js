var sectionCount = 0; //used for sections
var topicCount = 0; // //used for topics
const colors = ['#D9534F', '#F0AD4E', '#56C0E0', '#5CB85C', '#1C7CD5', '#8B4FD9']
var cropper = null; // cropper object
var cropperType = ""; //type of cropper ie: profile pic, banner
var cropperBlob = null; //blob that gets saved for profile pictures that are uploded in a modal

const options = {
    isCaseSensitive: false,
    // includeScore: false,
    shouldSort: true,
    // includeMatches: false,
    // findAllMatches: false,
    // minMatchCharLength: 1,
    // location: 0,
    // threshold: 0.6,
    // distance: 100,
    // useExtendedSearch: false,
    // ignoreLocation: false,
    // ignoreFieldNorm: false,
    keys: [
        "streamTitle",
        "streamDescription",
        "streamSubject",
    ]
};

//Sliders
function SliderProfile() {
    $('#profile-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(15px, 0px, 0px)")
}

function SliderSchedule() {
    $('#schedule-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(100px, 0px, 0px)")
}

function SliderStream() {
    $('#stream-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(187px, 0px, 0px)")
}

function SliderComment() {
    $('#comment-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(290px, 0px, 0px)")
}

function SliderFollowing() {
    $('#following-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(100px, 0px, 0px)")
}

function SliderProfileInformation() {
    $('#profile-info-tab').tab('show');
    $('#slider-object-profile-edit-modal').css("transform", "translate3d(15px, 0px, 0px)")
}

function SliderSocialMedia() {
    $('#social-media-tab').tab('show');
    $('#slider-object-profile-edit-modal').css("transform", "translate3d(110px, 0px, 0px)")
}

//Profile
function SaveProfile() {
    var formData = new FormData();
    formData.append("FirstName", $('#first-name').val());
    formData.append("LastName", $('#last-name').val());
    formData.append("Occupation", $('#occupation-major').val());
    formData.append("Location", $('#location').val());
    formData.append("Timezone", $('#timezone').val());

    var isSMValid = CheckSMUrls($('#linkedin-url').val(), $('#instagram-url').val(), $('#facebook-url').val(), $('#twitter-url').val());
    if (isSMValid != "all valid") {
        $('#profile-information-sm-p').text(isSMValid)
        ShowBannerNotification("profile-information-sm-notification")
        return;
    }
   
    formData.append("LinkedInUrl", $('#linkedin-url').val());
    formData.append("InstagramURL", $('#instagram-url').val());
    formData.append("FacebookURL", $('#facebook-url').val());
    formData.append("TwitterURL", $('#twitter-url').val());

    if (cropper != null)
        formData.append("ProfilePicture", cropperBlob);

    cropper = null; //makes sure that cropper object is destroyed once we upload is an image

    $.ajax({
        url: '/Profiles/Profile/?handler=SaveProfile',
        type: 'POST',
        dataType: 'json',
        data: formData,
        contentType: false,
        processData: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                ShowBannerNotification("profile-information-notification")

                $('#header-name').text(data.savedInfo[0] + " " + data.savedInfo[1]);
                $('#header-first-name').val(data.savedInfo[0]);
                $('#header-last-name').val(data.savedInfo[1]);
                if (data.savedInfo[2] == "") $('#header-occupation').text("Occupation / Major")
                else $('#header-occupation').text(data.savedInfo[2]);
                if (data.savedInfo[3] == "") $('#header-location').text("City, State");
                else $('#header-location').text(data.savedInfo[3]);
                $('#header-timezone').val(data.savedInfo[4]);
                $('#header-linkedin-url').val(data.savedInfo[5])

                var cacheDate = new Date().valueOf();
                $("#header-profile-picture").attr('src', data.savedInfo[6] + `?nocache=${cacheDate}`);
                $("#navbar-profile-picture").attr('src', data.savedInfo[6] + `?nocache=${cacheDate}`);
            }
        }
    })
}

function CheckSMUrls(linkden, insta, facebook, twitter) {
    if (linkden != null && linkden != "") {
        if (/(ftp|http|https):\/\/?(?:www\.)?linkedin.com(\w+:{0,1}\w*@)?(\S+)(:([0-9])+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/.test(linkden)) {
        }else {
            return "LinkedIn";
        }
    }

    if (insta != null && insta != "") {
        if (/^(http|https)\:\/\/www.instagram.com\/.*/i.test(insta)) {
        } else {
            return "Instagram";
        }
    }

    if (facebook != null && facebook != "") {
        if (/^(http|https)\:\/\/www.facebook.com\/.*/i.test(facebook)) {
        } else {
            return "Facebook";
        }
    }

    if (twitter != null && twitter != "") {
        if (/http(?:s)?:\/\/(?:www\.)?twitter\.com\/([a-zA-Z0-9_]+)/.test(twitter)) {
        } else {
            return "Twitter";
        }
    }

    return "all valid"
}

function EditProfile() {
    OpenModal('profile-information-modal')

    document.getElementById("preview-profile-picture").src = document.getElementById("header-profile-picture").src;
    $('#first-name').val($('#header-first-name').val());
    $('#last-name').val($('#header-last-name').val());
    if($('#header-occupation').text() != "Occupation / Major") $('#occupation-major').val($('#header-occupation').text());
    if ($('#header-location').text() != "City, State") $('#location').val($('#header-location').text());
    $('#timezone').val($('#header-timezone').val());
    $('#linkedin-url').val($('#header-linkedin-url').val());
    $('#instagram-url').val($('#header-instagram-url').val());
    $('#facebook-url').val($('#header-facebook-url').val());
    $('#twitter-url').val($('#header-twitter-url').val());
    $($('#profile-color').val()).css('border-style', 'dotted')
}

//Sections
function AddSection(event) {
    sectionCount++;
    event.preventDefault();
    var section = ` <div id="divider-${sectionCount}" class="divider"></div>
                    <div id="form-section-${sectionCount}" class="form-group col-lg-12">
                        <label id="section-label-title-${sectionCount}" class="form-header d-inline-block">Headline ${sectionCount}</label>
                        <img id="remove-section-icon-${sectionCount}" src="/images/TutorAssets/TutorDashboard/Remove.svg" class="d-inline-block form-icon float-right" onclick="RemoveSection(${sectionCount})" />
                        <input name="section-title-${sectionCount}" id="section-title-${sectionCount}" class="form-control border rounded-0 form-input" placeholder="Title of section ${sectionCount}!">
                        <label class="form-header pt-3">Description</label>
                        <textarea name="section-description-${sectionCount}" id="section-description-${sectionCount}" class="form-control border rounded-0 form-textarea" placeholder="Tell us what you are studying, concentrations, passions, and other extra curricular activities here!"></textarea>
                    </div>`

    $("#form-row-section").append(section);
    var e = document.getElementById("form-section-" + sectionCount);
    e.scrollIntoView();
}

function SaveSection(event, type) {
    var form = $('#form-section-tutor');
    var serialize = form.serialize();
    serialize = serialize.replace(/%0D%0A/g, '*--*');
    serialize = serialize.replace('\r', '');

    $.ajax({
        type: "POST",
        url: "/Profiles/Profile?handler=SaveSection",
        dataType: 'json',
        data: serialize,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success(data) {
            if (data === "Failed") {
                location.reload();
            }
            else {
                if (type != "remove") {
                    ShowBannerNotification("section-topic-notification")
                }
            }
        }
    })
}

function RemoveSection(sectionNumber) {
    var section = document.getElementById("form-section-" + sectionNumber);
    var divider = document.getElementById("divider-" + sectionNumber);

    section.remove();
    divider.remove();

    var sectionAbove = sectionNumber + 1;
    for (var i = sectionAbove; i <= sectionCount; i++) { //shift all other sections down one 
        document.getElementById("divider-" + i).id = "divider-" + (i - 1);
        document.getElementById("form-section-" + i).id = "form-section-" + (i - 1);

        var label = document.getElementById("section-label-title-" + i);
        label.id = "section-label-title-" + (i - 1);
        label.textContent = "Headline " + (i - 1)

        var removeIcon = document.getElementById("remove-section-icon-" + i)
        removeIcon.id = "remove-section-icon-" + (i - 1);
        removeIcon.setAttribute("onclick", "RemoveSection(" + (i - 1) + ")");

        var sectionTitle = document.getElementById("section-title-" + i);
        sectionTitle.id = "section-title-" + (i - 1);
        sectionTitle.name = "section-title-" + (i - 1);
        sectionTitle.setAttribute("placeholder", "Title of section " + (i - 1) + "!");

        var sectionDescription = document.getElementById("section-description-" + i);
        sectionDescription.id = "section-description-" + (i - 1);
        sectionDescription.name = "section-description-" + (i - 1);
    }

    SaveSection(event, "remove");

    sectionCount--;
}


//Topics
function AddTopic(event) {
    topicCount++;
    event.preventDefault();
    var topic = `<div id="divider-topic-${topicCount}" class="divider"></div>
                    <div id="form-topic-${topicCount}" class="form-group col-lg-12 border p-2">
                        <label class="form-header">Topic</label>
                        <img id="remove-topic-icon-${topicCount}" src="/images/TutorAssets/TutorDashboard/Remove.svg" class="d-inline-block form-icon float-right" onclick="RemoveTopic(${topicCount})" />
                        <select id="topic-${topicCount}" name="topic-${topicCount}" class="form-control form-control-sm border rounded-0">
                            <option>-Select-Topic-</option>
                            <option>Mathematics</option>
                            <option>Science</option>
                            <option>Engineering</option>
                            <option>Business</option>
                            <option>Law</option>
                            <option>Art</option>
                            <option>Humanities</option>
                            <option>Others</option>
                        </select>
                        <label class="form-header pt-3">List Of Subjects</label>
                        <textarea id="list-of-subjects-${topicCount}" name="list-of-subjects-${topicCount}" class="form-control border rounded-0 form-textarea" placeholder="Enter list of subjects here!"></textarea>
                    </div>`
    
    $("#form-row-topic").append(topic);
    var e = document.getElementById("form-topic-" + topicCount);
    e.scrollIntoView();
}

function RemoveTopic(topicNumber) {
    var topic = document.getElementById("form-topic-" + topicNumber);
    var divider = document.getElementById("divider-topic-" + topicNumber);

    topic.remove();
    divider.remove();

    SaveTopic();

    topicCount--;
}

function SaveTopic() {
    var form = $('#form-topic-tutor');
    var serialize = form.serialize();
    serialize = serialize.replace(/%0D%0A/g, '*--*');

    $.ajax({
        url: '/Profiles/Profile/?handler=SaveTopic',
        type: 'post',
        dataType: 'json',
        data: serialize,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success(data) {
            if (data === "Failed") {
                location.reload();
            }
            else {
                ShowBannerNotification("section-topic-notification")
            }
        }
    })
}

//University
function EditUniversityInfo() {
    OpenModal('university-edit-modal')

    $('#university-edit-abbreviation').val($('#university-abbreviation').text());
    $('#university-edit-name').val($('#university-name').text());
}

function SaveUniversityInfo() {
    var form = $('#university-edit-modal-form');
    if (!form[0].checkValidity()) {
        form[0].reportValidity();
        return;
    }
    var abbreviation = $('#university-edit-abbreviation').val()
    var name = $('#university-edit-name').val();
    var container = document.getElementById("university-element");

    var htmlString = "<div class='p-4'><p id='university-abbreviation' class='form-university-header'>" + abbreviation + "</p><p id='university-name' class='form-header'>" + name + "</p></div>"
    container.innerHTML = htmlString;

    $.ajax({
        url: '/Profiles/Profile/?handler=SaveUniversity',
        type: 'post',
        datatype: 'json',
        data: {
            'abbr': abbreviation,
            'name': name
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                $('#university-abbreviation').text(data.abbreviation)
                $('#university-name').text(data.name)
                ShowBannerNotification("university-edit-modal-notification")
            }
        }
    })
}

//Seach Videos

function SearchVideos(event, username, dash) { //dash is for the tutor dashboard vdieos since they have a different video template (they can edit)
    if (event != undefined)
        event.preventDefault();
    $.ajax({
        url: '/Profiles/Profile/?handler=SearchVideos',
        type: 'POST',
        dataType: 'json',
        data: {
            'username': username
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.videos.length > 0) videoJson = data.videos

            SearchVideoAlgo(event, dash);
        }
    });
}

function SearchVideoAlgo(event, dash) {
    if (event != undefined)
        event.preventDefault();

    var pattern = $('#searchQuery').val();
    var filter = $('#filter').val();

    var fuse = null;
    var output = null;

    if (pattern == "") {  //if pattern is empty then show all videos
        $('#video-row').html("");
        for (var i = 0; i < videoJson.length; i++) {
            if (filter != "") {
                if (filter != videoJson[i].streamSubject) { continue; }
            }

            var video = ""
            var rowForVideo = ""

            if (dash) {
                video = GetTutoDashVideoTemplate(videoJson[i].id,
                    videoJson[i].streamSubject,
                    videoJson[i].streamID,
                    videoJson[i].streamThumbnail,
                    videoJson[i].streamTitle,
                    videoJson[i].streamColor,
                    videoJson[i].streamDescription)

                rowForVideo = `<div id="video-${videoJson[i].id}" class="col-xl-3 col-lg-4 col-md-6 col-sm-6 col-12 mb-3 video video-subject-${videoJson[i].streamSubject}">${video}</div>`
            }
            else {
                video = GetVideoTemplate(videoJson[i].id,
                    videoJson[i].streamSubject,
                    videoJson[i].streamID,
                    videoJson[i].streamThumbnail,
                    videoJson[i].streamTitle,
                    videoJson[i].streamColor,
                    videoJson[i].name,
                    videoJson[i].username)

                rowForVideo = `<div id="video-${videoJson[i].id}" class="col-lg-4 col-md-6 col-sm-6 col-12 mb-3 video video-subject-${videoJson[i].streamSubject}">${video}</div>`
            }
           
            $('#video-row').append(rowForVideo);
        }

        if ($('.video').length <= 0) $('#video-none-found').removeClass('d-none').addClass('d-block');  //notification that says there are no videos with that search term (check is there are any video classes)
        else $('#video-none-found').removeClass('d-block').addClass('d-none');

        return;
    }

    fuse = new Fuse(videoJson, options);
    output = fuse.search(pattern);

    $('#video-row').html("");
    $('#video-none-found').removeClass('d-block').addClass('d-none');
    if (output.length > 0) { //if pattern is not empty then show all videos with the fuzzy search
        for (var i = 0; i < output.length; i++) {
            if (filter != "") {
                if (filter != output[i].item.streamSubject) { continue; }
            }

            var video = ""
            var rowForVideo = ""

            if (dash) {
                video = GetTutoDashVideoTemplate(videoJson[i].id,
                    output[i].item.streamSubject,
                    output[i].item.streamID,
                    output[i].item.streamThumbnail,
                    output[i].item.streamTitle,
                    output[i].item.streamColor,
                    output[i].item.streamDescription)

                rowForVideo = `<div id="video-${output[i].item.id}" class="col-xl-3 col-lg-4 col-md-6 col-sm-6 col-12 mb-3 video video-subject-${output[i].item.streamSubject}">${video}</div>`
            }
            else {
                video = GetVideoTemplate(output[i].item.id,
                    output[i].item.streamSubject,
                    output[i].item.streamID,
                    output[i].item.streamThumbnail,
                    output[i].item.streamTitle,
                    output[i].item.streamColor,
                    output[i].item.name,
                    output[i].item.username)

                rowForVideo = `<div id="video-${output[i].item.id}" class="col-lg-4 col-md-6 col-sm-6 col-12 mb-3 video video-subject-${output[i].item.streamSubject}">${video}</div>`
            }

             
            $('#video-row').append(rowForVideo);
        }
    }

    if ($('.video').length <= 0) $('#video-none-found').removeClass('d-none').addClass('d-block');  //notification that says there are no videos with that search term (check is there are any video classes)
    else $('#video-none-found').removeClass('d-block').addClass('d-none');
}

function Filter(event, username, dash) {
    $('#clear-filter').show();
    SearchVideos(event, username, dash)
}

function ClearFilter(event, username, dash) {
    $('#filter').val('')
    $('#clear-filter').hide();
    SearchVideos(event, username, dash)
}

//Banner
function SaveProfileBanner(image) {
    var formData = new FormData();
    //alert(image.size)
    formData.append("ProfileBanner", image);
    $.ajax({
        url: '/Profiles/Profile/?handler=SaveBanner',
        type: 'POST',
        dataType: 'json',
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.message === "Success") {
                CloseModal('notification-loading-image-modal')
                $('#preview-profile-banner').attr('src', data.banner + `?nocache=${new Date().valueOf()}`);
            }
        }
    });
}

function ChangeColor(event, color) {
    event.preventDefault()
    for (var i = 0; i < colors.length; i++) {
        $(colors[i]).css('border-style', 'none');
    }

    $(color).css('border-style', 'dotted');
    $('#profile-color').val(color)
    $('#header-name').css('color', color)

    $.ajax({
        url: '/Profiles/Profile/?handler=ChangeColor',
        type: 'POST',
        dataType: 'json',
        data: {
            'color': color,
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
        }
    });
}

//Cropper Tool

function OpenCropper(minCropBoxW, minCropBoxH, viewMode) {
    var image = document.getElementById('imagecropper-image');
    cropper = new Cropper(image, {
        viewMode: viewMode,
        dragMode: 'move',
        minCropBoxWidth: minCropBoxW,
        minCropBoxHeight: minCropBoxH,
        toggleDragModeOnDblclick: false,
        cropBoxResizable: false,
        rotatable: false,
        zoomable: false,
        zoomOnTouch: false,
        zoomOnWheel: false,
        background: false,
        checkCrossOrigin: false,
        data: {
            width: minCropBoxW,
            height: minCropBoxH,
        }
    });
}

function DestroyCropper() {
    if (cropper != null) {
        cropper.destroy()
    }
}

function SendCroppedImage() {
    cropper.getCroppedCanvas({
        imageSmoothingEnabled: false,
        imageSmoothingQuality: 'high',
    });

    cropper.getCroppedCanvas().toBlob((blob) => {
        if (cropperType == "Banner") {
            SaveProfileBanner(blob)
            OpenModal('notification-loading-image-modal')
        }
        else if (cropperType == "Profile Picture") {
            var url = URL.createObjectURL(blob);
            $('#preview-profile-picture').attr('src', url)
            cropperBlob = blob
        }
        else if (cropperType == "Thumbnail Edit") {
            var url = URL.createObjectURL(blob);
            $('#preview-video-thumbnail-edit').attr('src', url)
            cropperBlob = blob
        }
        else if (cropperType == "Thumbnail") {
            var url = URL.createObjectURL(blob);
            $('#preview-stream-thumbnail').attr('src', url)
            cropperBlob = blob
        }
   })

   CloseModal('imagecropper-modal')
   ResetButtons();
   DestroyCropper()
}

function UploadImage() {
    if (cropperType == "Banner") {
        $('#upload-profile-banner').click()
    }
}

function ResetButtons() {
    $('#cropper-buttons').html(` <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#6B6B6B; color:white" onclick="OpenConfirmDeleteImage()">Delete Image</button>
                                 <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="UploadImage()">Change Image</button>`)
    $('#delete-image-notification').hide();
}


function OpenConfirmDeleteImage() {
    $('#cropper-buttons').html(` <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#AC0001; color:white" onclick="SendCroppedImage()">Confirm Delete</button>
                                 <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="UploadImage()">Change Image</button>`)
    $('#delete-image-notification').show();
}

function DeleteImage() {

}