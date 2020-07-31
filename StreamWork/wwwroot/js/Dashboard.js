var sectionCount = 0; //used for sections
var topicCount = 0; // //used for topics

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
    $('#slider-object-profile-edit-modal').css("transform", "translate3d(180px, 0px, 0px)")
}


//Profile
function SaveProfile() {
    var formData = new FormData();
    var totalFiles = document.getElementById("upload-profile-picture");
    formData.append("FirstName", $('#first-name').val());
    formData.append("LastName", $('#last-name').val());
    formData.append("Occupation", $('#occupation-major').val());
    formData.append("Location", $('#location').val());
    formData.append("Timezone", $('#timezone').val());
    formData.append("LinkedInUrl", $('#linkedin-url').val());
    formData.append("InstagramURL", $('#instagram-url').val());
    formData.append("FacebookURL", $('#facebook-url').val());
    formData.append("TwitterURL", $('#twitter-url').val());
    if (totalFiles.files.length > 0)
        formData.append("ProfilePicture", totalFiles.files[0]);

    $.ajax({
        url: '/Profile/Profile/?handler=SaveProfile',
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
                $('#header-name').text(data.savedInfo[0] + " " + data.savedInfo[1]);
                $('#header-first-name').val(data.savedInfo[0]);
                $('#header-last-name').val(data.savedInfo[1]);
                if (data.savedInfo[2] == "") $('#header-occupation').text("Occupation / Major")
                else $('#header-occupation').text(data.savedInfo[2]);
                if (data.savedInfo[3] == "") $('#header-location').text("City, State");
                else $('#header-location').text(data.savedInfo[3]);
                $('#header-timezone').val(data.savedInfo[4]);
                $('#header-linkedin-url').val(data.savedInfo[5])

                var c = new Date().valueOf();
                $("#header-profile-picture").attr('src', data.savedInfo[6] + `?nocache=${c}`);
                $("#navbar-profile-picture").attr('src', data.savedInfo[6] + `?nocache=${c}`);

                ShowBannerNotification("profile-information-notification")
            }
        }
    })
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
}

//Sections
function AddSection(event) {
    sectionCount++;
    event.preventDefault();
    var section = ` <div id="divider-${sectionCount}" class="divider"></div>
                    <div id="form-section-${sectionCount}" class="form-group col-lg-12">
                        <label id="section-label-title-${sectionCount}" class="form-header d-inline-block">Section ${sectionCount} Title</label>
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
        url: "/Profile/Profile?handler=SaveSection",
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
        label.textContent = "Section " + (i - 1) + " Title"

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
        url: '/Profile/Profile/?handler=SaveTopic',
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
        url: '/Profile/Profile/?handler=SaveUniversity',
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

//Banner
function SaveProfileBanner(image) {
    var formData = new FormData();
    formData.append("ProfileBanner", image.files[0]);
    $.ajax({
        url: '/Profile/Profile/?handler=SaveBanner',
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
                $('#preview-profile-banner').attr('src', data.banner + `?nocache=${new Date().valueOf()}`);
            }
        }
    });
}
