var sectionCount = 0;
var topicCount = 0;
var streamId = "";
var isStreamEdited = false;

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
    $('#slider-object').css("transform", "translate3d(193px, 0px, 0px)")
}

function SliderComment() {
    $('#comment-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(290px, 0px, 0px)")
}

function AddSection(event) {
    sectionCount++;
    event.preventDefault();
    var section = ` <div id="divider-${sectionCount}" class="divider"></div>
                    <div id="form-section-${sectionCount}" class="form-group col-lg-12">
                        <label id="SectionLabelTitle-${sectionCount}" class="form-header d-inline-block">Section ${sectionCount} Title</label>
                        <img id="RemoveSectionIcon-${sectionCount}" src="/images/TutorAssets/TutorDashboard/Remove.png" class="d-inline-block form-section-topic-remove-icon" onclick="RemoveSection(${sectionCount})" />
                        <input name="SectionTitle-${sectionCount}" id="SectionTitle-${sectionCount}" class="form-control border rounded-0 form-input" placeholder="Title of section ${sectionCount}!">
                        <label class="form-header pt-3">Description</label>
                        <textarea name="SectionDescription-${sectionCount}" id="SectionDescription-${sectionCount}" class="form-control border rounded-0 form-textarea" placeholder="Tell us what you are studying, concentrations, passions, and other extra curricular activities here!"></textarea>
                    </div>`

    $("#form-row-section").append(section);
    var e = document.getElementById("form-section-" + sectionCount);
    e.scrollIntoView();
}

function SaveSection(event, type) { //saves all sections
    var form = $('#form-section-tutor');
    var serialize = form.serialize();
    serialize = serialize.replace(/%0D%0A/g, '*--*');

    $.ajax({
        type: "POST",
        url: "/Tutor/SaveSection",
        dataType: 'json',
        data: serialize,
        success(data) {
            if (data === "Failed") {
                location.reload();
            }
            else {
                if (type != "remove") {
                    $("#sectionTopicNotification").fadeTo(2000, 500).slideUp(500, function () {
                        $("#sectionTopicNotification").slideUp(500);
                    });
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

    //shift all other sections down one 
    var sectionAbove = sectionNumber + 1;
    for (var i = sectionAbove; i <= sectionCount; i++) {
        document.getElementById("divider-" + i).id = "divider-" + (i - 1);
        document.getElementById("form-section-" + i).id = "form-section-" + (i - 1);

        var label = document.getElementById("SectionLabelTitle-" + i);
        label.id = "SectionLabelTitle-" + (i - 1);
        label.textContent = "Section " + (i - 1) + " Title"

        var removeIcon = document.getElementById("RemoveSectionIcon-" + i)
        removeIcon.id = "RemoveSectionIcon-" + (i - 1);
        removeIcon.setAttribute("onclick", "RemoveSection(" + (i - 1) + ")");

        var sectionTitle = document.getElementById("SectionTitle-" + i);
        sectionTitle.id = "SectionTitle-" + (i - 1);
        sectionTitle.name = "SectionTitle-" + (i - 1);
        sectionTitle.setAttribute("placeholder", "Title of section " + (i - 1) + "!");

        var sectionDescription = document.getElementById("SectionDescription-" + i);
        sectionDescription.id = "SectionDescription-" + (i - 1);
        sectionDescription.name = "SectionDescription-" + (i - 1);
    }

    SaveSection(event, "remove");

    sectionCount--;
}

function AddTopic(event) {
    topicCount++;
    event.preventDefault();
    var topic = `<div id="divider-topic-${topicCount}" class="divider"></div>
                <div id="form-topic-${topicCount}" class="form-group col-lg-12 border p-2">
                    <label class="form-header">Topic</label>
                    <img id="RemoveTopicIcon-${topicCount}" src="/images/TutorAssets/TutorDashboard/Remove.png" class="d-inline-block form-section-topic-remove-icon" onclick="RemoveTopic(${topicCount})" />
                    <select id="Topic-${topicCount}" name="Topic-${topicCount}" class="form-control border form-input rounded-0">
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
                    <textarea id="ListOfSubjects-${topicCount}" name="ListOfSubjects-${topicCount}" class="form-control border rounded-0 form-textarea" placeholder="Enter list of subjects here!"></textarea>
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
        url: '/Tutor/SaveTopic',
        type: 'post',
        dataType: 'json',
        data: serialize,
        success(data) {
            if (data === "Failed") {
                location.reload();
            }
            else {
                $("#sectionTopicNotification").fadeTo(2000, 500).slideUp(500, function () {
                    $("#sectionTopicNotification").slideUp(500);
                });
            }
        }
    })
}

function SaveProfile() {
    var formData = new FormData();
    var totalFiles = document.getElementById("uploadProfilePicture");
    formData.append("FirstName", $('#first-name').val());
    formData.append("LastName", $('#last-name').val());
    formData.append("Occupation", $('#occupation-major').val());
    formData.append("Location", $('#location').val());
    formData.append("Timezone", $('#timezone').val());
    formData.append("LinkedInUrl", $('#linkedin-url').val());
    if (totalFiles.files.length > 0)
        formData.append("ProfilePicture", totalFiles.files[0]);

    $.ajax({
        url: '/Tutor/SaveProfile',
        type: 'POST',
        dataType: 'json',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.message === "Success") {
                document.getElementById("header-name").innerHTML = data.firstName + " " + data.lastName
                document.getElementById("header-occupation").innerHTML = data.occupation
                document.getElementById("header-location").innerHTML = data.location
                $("#profileInformationNotification").fadeTo(2000, 500).slideUp(500, function () {
                    $("#profileInformationNotification").slideUp(500);
                });
            }
        }
    })
}

function DeleteStream() {
    $.ajax({
        url: '/Tutor/DeleteStream',
        type: 'POST',
        dataType: 'json',
        data: {
            'id': videoId,
        },
        success: function (data) {
            $('#videoInfo-' + videoId).hide();
        }
    });
}

function OpenEditStreamModal(id, modalId) {
    OpenModal(modalId)
    streamId = id
}

function OpenDeleteStreamConfirmModal() {
    $('#deleteStreamConfirmModal').modal('show');
}

function SaveEditedStreamInfo() {
    var formData = new FormData()
    var streamTitle = $('#streamTitleEdit-' + streamId).val();
    var streamDescription = $('#streamDescriptionEdit-' + streamId).val();
    var totalFile = document.getElementById("uploadThumbnailEdit-" + streamId)

    formData.append("StreamId", streamId);
    formData.append("StreamTitle", streamTitle);
    formData.append("StreamDescription", streamDescription);
    if (totalFile.files.length > 0) formData.append("StreamThumbnail", totalFile.files[0]);

    $.ajax({
        url: '/Tutor/SaveEditedStreamInfo',
        type: 'POST',
        dataType: 'json',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.message === "Success") {
                isStreamEdited = true;
                document.getElementById("streamTitle-" + streamId).innerHTML = data.title;
                document.getElementById("streamThumbnail-" + streamId).src = data.thumbnail;
                document.getElementById("streamTitleEdit-" + streamId).setAttribute("value", data.title)
                document.getElementById("streamDescriptionEdit-" + streamId).setAttribute("value", data.description)
                document.getElementById("previewStreamThumbnailEdit-" + streamId).setAttribute("src", data.thumbnail)
                $("#streamEditModalNotification-" + streamId).fadeTo(2000, 500).slideUp(500, function () {
                    $("#streamEditModalNotification-" + + streamId).slideUp(500);
                });
            }
        }
    });
}

function SaveUniversityInfo() {
    var form = $('#universityForm');
    if (!form[0].checkValidity()) {
        form[0].reportValidity();
        return;
    }
    var abbreviation = $('#universityAbbreviation').val()
    var name = $('#universityName').val();
    var container = document.getElementById("universityElement");

    var htmlString = "<div class='p-4'><p class='form-university-header'>" + abbreviation + "</p><p class='form-header'>" + name + "</p></div>"
    container.innerHTML = htmlString;

    $.ajax({
        url: '/Tutor/SaveUniversity',
        type: 'post',
        datatype: 'json',
        data: {
            'abbr': abbreviation,
            'name': name
        },
        success: function (data) {
            if (data.message === "Success") {
                $("#universityModalNotification" + streamId).fadeTo(2000, 500).slideUp(500, function () {
                    $("#universityModalNotification" + + streamId).slideUp(500);
                });
            }
        }
    })
}
    
function SaveProfileBanner(image) {
    var formData = new FormData();
    formData.append("ProfileBanner", image.files[0]);
    $.ajax({
        url: '/Tutor/SaveBanner',
        type: 'POST',
        dataType: 'json',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.message === "Success") {
                $('#previewProfileBanner').attr('src', data.banner)
            }
        }
    });
}       
        
       
    
    
    
