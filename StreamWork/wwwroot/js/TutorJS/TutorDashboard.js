var sectionCount = 0;
var topicCount = 0;

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

$(function () {
    $('#schedule-date-picker').datetimepicker({
        format: 'L',
        minDate: new Date()
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

$(function () {
    $('#schedule-time-start-edit-picker').datetimepicker({
        format: 'LT'
    });
})

$(function () {
    $('#schedule-time-stop-edit-picker').datetimepicker({
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

    OpenModal("scheduleModal");
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

function SaveScheduleTask(id) {

    var form = $('#scheduleModalForm');
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

            element += ` <div class="col-lg-6 col-md-12 mt-2">
                                                    <div class="card card-border" onclick="EditScheduleTask('${data.sorted[i].id}')">
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
        element = ` <div class="col-lg-6 col-md-12  mt-2">
                                                <div class="card">
                                                    <div class="card-body">
                                                        <div class="d-inline-flex">
                                                            <img class="rounded m-1" src="/images/ScheduleAssets/CalendarAdd.png" style="width:75px; height:75px" />
                                                            <div class="m-1" style="height:75px;">
                                                                <p id="schedule-stream-title" class="form-header m-0">Schedule Stream</p>
                                                                <p id="schedule-stream-subject" class="form-header mt-1 mb-0" style="font-size:10px">Stream Topic</p>
                                                                <p id="schedule-stream-time" class="form-header mt-1">Click "Add Stream" or the plus</p>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>`
    }
       
    

        document.getElementById("taskRow").innerHTML = element;
        CloseModal("scheduleModal")
}

function EditScheduleTask(id) {
    OpenModal("scheduleModal");

    document.getElementById("schedule-date-mask").innerHTML = ReturnMask($('#schedule-date-' + id).val());
    $('#schedule-date-picker').datetimepicker('viewDate',moment($('#schedule-date-' + id).val()))
    $('#schedule-title').val($('#schedule-stream-title-' + id).text());
    $('#schedule-subject').val($('#schedule-stream-subject-' + id).text());
    $('#schedule-time-start-picker-value').val($('#schedule-time-start-' + id).val());
    $('#schedule-time-stop-picker-value').val($('#schedule-time-stop-' + id).val());
    $('#schedule-time-start-picker').val($('#schedule-time-start-' + id).val());
    $('#schedule-time-stop-picker').val($('#schedule-time-stop-' + id).val());


    document.getElementById("schedule-buttons").innerHTML = `<div class="row">
                                                                <div class="col-6 pr-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#6B6B6B; color:white" onclick="DeleteScheduleTask('${id}')">Delete Scheduled Stream</button>
                                                                </div>
                                                                <div class="col-6 pl-0">
                                                                    <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white; height:100%" onclick="SaveScheduleTask('${id}')">Save Changes</button>
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
            }
        }
    })
}

function EditStream(id) {
    OpenModal("edit-stream-modal");

    $('#stream-title-edit').val($('#stream-title-' + id).text());
    $('#stream-description-edit').val($('#stream-description-' + id).val());
    document.getElementById("preview-stream-thumbnail-edit").src = document.getElementById("stream-thumbnail-" + id).src
    document.getElementById("preview-stream-thumbnail-edit").src = document.getElementById("stream-thumbnail-" + id).src

    document.getElementById("stream-edit-buttons").innerHTML = ` <div class="row">
                                                                    <div class="col-6 pr-0">
                                                                        <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#6B6B6B; color:white" onclick="DeleteScheduleTask('${id}')">Delete Stream</button>
                                                                    </div>
                                                                    <div class="col-6 pl-0">
                                                                        <button class="btn border-0 rounded-0 p-3 w-100" style="background-color:#004643; color:white" onclick="SaveEditedStreamInfo('${id}')">Save Changes</button>
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
                document.getElementById("stream-thumbnail-" + id).src = data.thumbnail;

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

//function OpenDeleteStreamConfirmModal() {
//    $('#deleteStreamConfirmModal').modal('show');
//}

