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
    $('#slider-object').css("transform", "translate3d(193px, 0px, 0px)")
}

function SliderComment() {
    $('#comment-tab').tab('show');
    $('#slider-object').css("transform", "translate3d(290px, 0px, 0px)")
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
        url: "/Tutor/SaveSection",
        dataType: 'json',
        data: serialize,
        success(data) {
            if (data === "Failed") {
                location.reload();
            }
            else {
                if (type != "remove") {
                    $("#section-topic-notification").fadeTo(2000, 500).slideUp(500, function () {
                        $("#section-topic-notification").slideUp(500);
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
        url: '/Tutor/SaveTopic',
        type: 'post',
        dataType: 'json',
        data: serialize,
        success(data) {
            if (data === "Failed") {
                location.reload();
            }
            else {
                $("#section-topic-notification").fadeTo(2000, 500).slideUp(500, function () {
                    $("#section-topic-notification").slideUp(500);
                });
            }
        }
    })
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
                $('#header-name').text(data.firstName + " " + data.lastName);
                $('#header-first-name').val(data.firstName);
                $('#header-last-name').val(data.lastName);
                $('#header-occupation').text(data.occupation);
                $('#header-location').text(data.location);
                $('#header-timezone').val(data.timezone);
                $('#header-linkedin-url').val(data.linkedInUrl)

                var c = new Date().valueOf();
                $("#header-profile-picture").attr('src', data.profilePicture + `?nocache=${c}`);
                $("#navbar-profile-picture").attr('src', data.profilePicture + `?nocache=${c}`);
               
                $("#profile-information-notification").fadeTo(2000, 500).slideUp(500, function () {
                    $("#profile-information-notification").slideUp(500);
                });
            }
        }
    })
}

function EditProfile() {
    OpenModal('profile-information-modal')

    document.getElementById("preview-profile-picture").src = document.getElementById("header-profile-picture").src;
    $('#first-name').val($('#header-first-name').val());
    $('#last-name').val($('#header-last-name').val());
    $('#occupation-major').val($('#header-occupation').text());
    $('#location').val($('#header-location').text());
    $('#timezone').val($('#header-timezone').val());
    $('#linkedin-url').val($('#header-linkedin-url').val());
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
        url: '/Tutor/SaveUniversity',
        type: 'post',
        datatype: 'json',
        data: {
            'abbr': abbreviation,
            'name': name
        },
        success: function (data) {
            if (data.message === "Success") {
                $('#university-abbreviation').text(data.abbreviation)
                $('#university-name').text(data.name)
                $("#university-edit-modal-notification").fadeTo(2000, 500).slideUp(500, function () {
                    $("#university-edit-modal-notification").slideUp(500);
                });
            }
        }
    })
}

//Banner
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
                $('#preview-profile-banner').attr('src', data.banner + `?nocache=${new Date().valueOf()}`);
            }
        }
    });
}

//Schedule
$(function () {
    $('#schedule-date-picker').datetimepicker({
        format: 'L',
        minDate: new Date(),
    });
});

$(document).ready(function () {
    $('#schedule-date-picker').on('change.datetimepicker', function (e) {
        AddDateToModal(e.date)
    })
})

$(function () {
    $('#schedule-time-start-picker').datetimepicker({
        format: 'LT',
    });
})

$(function () {
    $('#schedule-time-stop-picker').datetimepicker({
        format: 'LT'
    });
})

function AddDateToModal(date) {
    document.getElementById("schedule-date-mask").innerHTML = ReturnMask(date);
}

function CheckIfTimezoneIsValidForSchedule() {
    if ($('#header-timezone').val() == "") {
        $("#schedule-timezone-notification").fadeTo(2000, 500).slideUp(500, function () {
            $("#schedule-timezone-notification").slideUp(500);
        });

        return;
    }

    document.getElementById("schedule-date-mask").innerHTML = ReturnMask(new Date());
    OpenModal("schedule-modal");
}

function ReturnMask(date) {
    var month = moment(String(date)).format("MMM");
    var day = moment(String(date)).format("D");
    var dow = moment(String(date)).format("ddd");

    var dateHTML = ` <div class="text-center" data-target="#schedule-date-picker" data-toggle="datetimepicker">
                        <p id="schedule-dow" class="form-header m-0" style="font-size:18px">${month}</p>
                        <p id="schedule-day" class="form-header m-0" style="font-size:30px">${day}</p>
                        <p id="schedule-dow" class="form-header m-0" style="font-size:18px">${dow}</p>
                    </div>`

    return dateHTML;
}

function SaveScheduleTask(id, type) {

    var form = $('#schedule-modal-form');
    if (!form[0].checkValidity()) {
        return form[0].reportValidity();
    }

    var formData = new FormData();
    formData.append("StreamTitle", $('#schedule-title').val());
    formData.append("StreamSubject", $('#schedule-subject').val());
    formData.append("TimeStart", $('#schedule-time-start-picker-value').val());
    formData.append("TimeStop", $('#schedule-time-stop-picker-value').val());

    var stringD = String($('#schedule-date-picker').datetimepicker('viewDate')).split(" ");
    formData.append("Date", stringD[0] + " " + stringD[1] + " " + stringD[2] + " " + stringD[3] + " " + stringD[4])

    if (id != "") formData.append("Id", id);

    $.ajax({
        url: '/Tutor/SaveScheduleTask',
        type: 'POST',
        datatype: 'json',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.message === "Success") {
                SortTasks(data);
                if (type != 'edit') DiscardCalendarModalAndCloseModal();
                else {
                    $("#schedule-modal-notification").fadeTo(2000, 500).slideUp(500, function () {
                        $("#schedule-modal-notification").slideUp(500);
                    });
                }
            }
        }
    })
}

function SortTasks(data) {
    document.getElementById("taskRow").innerHTML = "";
    var element = "";
    if (data.sorted.length > 0) {
        for (var i = 0; i < data.sorted.length; i++) {
            var month = moment(String(data.sorted[i].date).replace("T", " ")).format("MMM");
            var day = moment(String(data.sorted[i].date).replace("T", " ")).format("D");
            var dow = moment(String(data.sorted[i].date).replace("T", " ")).format("ddd");

            element += `<div class="col-lg-6 col-md-12 mt-2">
                            <div class="card card-border" onmouseover=" $('#edit-schedule-icon-${data.sorted[i].id}').css('display','block')" onmouseout="$('#edit-schedule-icon-${data.sorted[i].id}').css('display','none')" onclick="EditScheduleTask('${data.sorted[i].id}')">
                                <div class="image-container w-100">
                                    <div class="top-right">
                                        <img id="edit-schedule-icon-${data.sorted[i].id}" class="p-1" style="width:30px; cursor:pointer; display:none" src="/images/TutorAssets/TutorDashboard/Edit.png" onclick="EditScheduleTask('${data.sorted[i].id}')" />
                                    </div>
                                </div>
                                <div class="card-body">
                                    <input type="hidden" id="schedule-date-${data.sorted[i].id}" value="${data.sorted[i].date}" />
                                    <div class="d-inline-flex">
                                        <img class="rounded m-1" src="${data.sorted[i].subjectThumbnail}" style="width:75px; height:75px" />
                                        <div class="text-center m-1 schedule-border" style="width:75px; height:75px;">
                                            <p id="schedule-month-${data.sorted[i].id}" class="form-header mt-4" style="font-size:18px">${month}</p>
                                        </div>
                                        <div class="text-center m-1 schedule-border" style="width:75px; height:75px;">
                                            <p id="schedule-day-${data.sorted[i].id}" class="form-header mb-0 mt-2" style="font-size:22px">${day}</p>
                                            <p id="schedule-dow-${data.sorted[i].id}" class="form-header" style="font-size:14px">${dow}</p>
                                        </div>
                                        <div class="m-1" style="height:75px;">
                                            <p id="schedule-stream-title-${data.sorted[i].id}" class="form-header m-0">${data.sorted[i].streamTitle}</p>
                                            <p id="schedule-stream-subject-${data.sorted[i].id}" class="mt-1 mb-0" style="font-size:10px">${data.sorted[i].streamSubject}</p>
                                            <p class="mt-1" style="font-size:14px">${data.sorted[i].timeStart} - ${data.sorted[i].timeStop} [${data.sorted[i].timeZone}]</p>
                                            <input type="hidden" id="schedule-time-start-${data.sorted[i].id}" value="${data.sorted[i].timeStart}" />
                                            <input type="hidden" id="schedule-time-stop-${data.sorted[i].id}" value="${data.sorted[i].timeStop}" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                           </div>`
        }
    }
    else {
        element = `<div class="col-lg-6 col-md-12  mt-2">
                        <div class="card">
                            <div class="card-body">
                                <div class="d-inline-flex">
                                    <img class="rounded m-1 d-block mr-auto ml-auto" src="/images/ScheduleAssets/CalendarAdd.png" style="width:75px; height:75px" onclick="CheckIfTimezoneIsValidForSchedule()"/>
                                    <div class="m-1" style="height:75px;">
                                        <p id="schedule-stream-title" class="form-header m-0">Schedule Stream</p>
                                        <p id="schedule-stream-subject" class="form-header mt-1 mb-0" style="font-size:10px; font-family:'Roboto', serif">Stream Topic</p>
                                        <p id="schedule-stream-time" class="form-header mt-1" style="font-size:12px; font-family:'Roboto', serif">Click "Add Stream" or the plus</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                     </div>`
    }



    document.getElementById("taskRow").innerHTML = element;
}

function EditScheduleTask(id) {
    OpenModal("schedule-modal");

    document.getElementById("schedule-date-mask").innerHTML = ReturnMask($('#schedule-date-' + id).val());
    $('#schedule-date-picker').datetimepicker('viewDate', moment($('#schedule-date-' + id).val()))
    $('#schedule-title').val($('#schedule-stream-title-' + id).text());
    $('#schedule-subject').val($('#schedule-stream-subject-' + id).text());
    $('#schedule-time-start-picker-value').val($('#schedule-time-start-' + id).val());
    $('#schedule-time-stop-picker-value').val($('#schedule-time-stop-' + id).val());
    $('#schedule-time-start-picker').val($('#schedule-time-start-' + id).val());
    $('#schedule-time-stop-picker').val($('#schedule-time-stop-' + id).val());

    document.getElementById("schedule-buttons").innerHTML = `<div class="row">
                                                                <div class="col-6 pr-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#6B6B6B; color:white" onclick="ShowDeleteScheduleTaskBanner('${id}')">Delete Scheduled Stream</button>
                                                                </div>
                                                                <div class="col-6 pl-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white; height:100%" onclick="SaveScheduleTask('${id}', 'edit')">Save Changes</button>
                                                                </div>
                                                             </div>`
}

function ShowDeleteScheduleTaskBanner(id) {
    $('#schedule-modal-delete-task-notification').show()
    document.getElementById("schedule-buttons").innerHTML = `<div class="row">
                                                                <div class="col-6 pr-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#AC0001; color:white" onclick="DeleteScheduleTask('${id}')">Confirm Delete</button>
                                                                </div>
                                                                <div class="col-6 pl-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white; height:100%" onclick="SaveScheduleTask('${id}', 'edit')">Save Changes</button>
                                                                </div>
                                                             </div>`
}

function DeleteScheduleTask(id) {
    $.ajax({
        url: '/Tutor/DeleteScheduleTask',
        type: 'POST',
        datatype: 'json',
        data: {
            'taskId': id,
        },
        success: function (data) {
            if (data.message === "Success") {
                SortTasks(data);
                DiscardCalendarModalAndCloseModal()
            }
        }
    })
}


//Streams
function EditStream(id) {
    OpenModal("edit-stream-modal");

    $('#stream-title-edit').val($('#stream-title-' + id).text());
    $('#stream-description-edit').val($('#stream-description-' + id).val());
    document.getElementById("preview-stream-thumbnail-edit").src = document.getElementById("stream-thumbnail-" + id).src
    document.getElementById("preview-stream-thumbnail-edit").src = document.getElementById("stream-thumbnail-" + id).src

    document.getElementById("stream-edit-buttons").innerHTML = ` <div class="row">
                                                                    <div class="col-6 pr-0">
                                                                        <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#6B6B6B; color:white" onclick="ShowDeleteStreamTaskBanner('${id}')">Delete Stream</button>
                                                                    </div>
                                                                    <div class="col-6 pl-0">
                                                                        <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="SaveEditedStreamInfo('${id}')">Save Changes</button>
                                                                    </div>
                                                                </div>`
}

function ShowDeleteStreamTaskBanner(id) {
    $('#edit-stream-modal-delete-stream-notification').show()
    document.getElementById("stream-edit-buttons").innerHTML = `<div class="row">
                                                                <div class="col-6 pr-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#AC0001; color:white" onclick="DeleteStream('${id}')">Confirm Delete</button>
                                                                </div>
                                                                <div class="col-6 pl-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white; height:100%" onclick="SaveEditedStreamInfo('${id}')">Save Changes</button>
                                                                </div>
                                                             </div>`
}

function SaveEditedStreamInfo(id) {
    var formData = new FormData()
    var totalFile = document.getElementById("upload-thumbnail-edit")

    formData.append("StreamId", id);
    formData.append("StreamTitle", $('#stream-title-edit').val());
    formData.append("StreamDescription", $('#stream-description-edit').val());
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
                document.getElementById("stream-title-" + id).innerHTML = data.title;
                document.getElementById("stream-description-" + id).src = data.description;
                document.getElementById("stream-thumbnail-" + id).src = data.thumbnail + `?nocache=${new Date().valueOf()}`;

                $("#edit-stream-modal-notification").fadeTo(2000, 500).slideUp(500, function () {
                    $("#edit-stream-modal-notification").slideUp(500);
                });
            }
        }
    });
}

function DeleteStream(id) {
    $.ajax({
        url: '/Tutor/DeleteStream',
        type: 'POST',
        dataType: 'json',
        data: {
            'id': id,
        },
        success: function (data) {

        }
    });
}

function SearchStreams(event, name, username) { //filters by username
    event.preventDefault();
    var searchTerm = $('#searchQuery').val();
    var filter = $('#filter').val();
    $.ajax({
        url: '/Tutor/SearchArchivedStreams',
        type: 'POST',
        dataType: 'json',
        data: {
            'searchTerm': searchTerm,
            'filter': filter,
        },
        success: function (data) {
            document.getElementById("stream-row").innerHTML = "";
            var element = "";
            for (var i = 0; i < data.results.length; i++) {
                if (data.results[i].username == username) {
                    element += `<div id="streamInfo-${data.results[i].id}" class="col-lg-3 col-md-6 col-sm-6">
                                <div class="card mt-3 border-0" style="border-bottom-left-radius:20px; border-bottom-right-radius:20px; border-top-left-radius:20px; border-top-right-radius:20px;">
                                    <div class="card-title">
                                        <a href="../StreamViews/StreamPlaybackPage?streamId=${data.results[i].streamId}"><img id="stream-thumbnail-${data.results[i].id}" style="width:100%; height:100%; border-top-left-radius:20px; border-top-right-radius:20px;" src=${data.results[i].streamThumbnail}></a>
                                    </div>
                                    <div class="card-body pt-0 pb-1">
                                        <h5 id="stream-title-${data.results[i].id}" class="text-truncate form-header">${data.results[i].streamTitle}</h5>
                                        <input id="stream-description-${data.results[i].id}" type="hidden" value="${data.results[i].streamDescription}" />
                                        <p class="text-truncate form-header" style="font-size:10px">${name.replace("|", " ")}</p>
                                    </div>
                                    <div class="card-footer p-0" style="background-color:${data.results[i].streamColor}; border-bottom-left-radius:20px; border-bottom-right-radius:20px;"><span style="color:white; cursor:pointer; float:right; padding-right:10px" onclick="EditStream('${data.results[i].id}')">&#8943;</span></div>
                                </div>
                            </div>`
                }
            }

            document.getElementById("stream-row").innerHTML = element;
        }
    });
}