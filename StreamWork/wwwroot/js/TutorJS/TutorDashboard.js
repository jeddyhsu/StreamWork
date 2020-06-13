var sectionCount = 0;

function SliderProfile() {
    $('#profile-tab').tab('show');
    $('#sliderObject').css("transform", "translate3d(15px, 0px, 0px)")
}

function SliderSchedule() {
    $('#schedule-tab').tab('show');
    $('#sliderObject').css("transform", "translate3d(110px, 0px, 0px)")
}

function SliderStream() {
    $('#stream-tab').tab('show');
    $('#sliderObject').css("transform", "translate3d(210px, 0px, 0px)")
}

function SliderComment() {
    $('#comment-tab').tab('show');
    $('#sliderObject').css("transform", "translate3d(315px, 0px, 0px)")
}

function AddSection(event) {
    sectionCount++;
    event.preventDefault();
    var section = "<div class='divider'></div><div id='formSection-" + sectionCount + "' class='form-group col-lg-12'><label class='formHeaders' > Section " + sectionCount + " Title</label><input id='SectionTitle-" + sectionCount + "' name='SectionTitle-" + sectionCount + "' class='form-control border rounded-0 formInput' placeholder='Title of section " + sectionCount + "!'><label class='formHeaders pt-3'>Description</label><textarea id='SectionDescription-" + sectionCount + "' name='SectionDescription-" + sectionCount + "' class='form-control border rounded - 0 formTextArea' placeholder='Tell us what you' re studying, concentrations, passions, and other extra curricular activities here!'></textarea></div > "
    document.getElementById("formRowSection").innerHTML += section
    var e = document.getElementById("formSection-" + sectionCount);
    e.scrollIntoView();
}

function AddTopic(event) {
    event.preventDefault();
    var topic = "<div class='divider'></div><div id='formTopic' class='form-group col-lg-12 border p-2'><label class='formHeaders'>Topic</label ><select class='form-control border formInput rounded-0'><option>-Select-Topic-</option><option>1</option><option>2</option><option>3</option><option>4</option><option>5</option></select><label class='formHeaders pt-3'>List Of Subject</label><textarea class='form-control border rounded-0 formTextArea' placeholder='Enter list of topics here!'></textarea></div>"
    document.getElementById("formRowTopic").innerHTML += topic
    var e = document.getElementById("formTopic");
    e.scrollIntoView();
}

function OpenModal() {
    var modal = document.getElementById("profileInformationModal");
    modal.style.display = "block";
}

function CloseModal() {
    var modal = document.getElementById("profileInformationModal");
    modal.style.display = "none";
}

function SaveBasicInformation(event) {

    var form = $('#SectionForm');
    var seriallize = form.serialize();

    $.ajax({
        type: "POST",
        url: "/Tutor/SaveSection",
        dataType: 'json',
        data: seriallize,
        success(data) {
            if (data === "Failed") {
                location.reload();
            }
        }
    })
}

function RemoveSection(sectionNumber) {
    event.preventDefault();
    var section = document.getElementById("formSection-" + sectionNumber);
    var divider = document.getElementById("divider-" + sectionNumber);
    section.remove();
    divider.remove();

    //shift all other sections down one 
    var sectionAbove = sectionNumber + 1; 
    for (var i = sectionAbove; i <= sectionCount; i++) {
        document.getElementById("divider-" + i).id = "divider-" + (i - 1);
        document.getElementById("formSection-" + i).id = "formSection-" + (i - 1);
        var label = document.getElementById("SectionLabelTitle-" + i);
        label.id = "SectionLabelTitle-" + (i - 1);
        label.textContent = "Section " + (i - 1) + " Title"
        document.getElementById("RemoveSectionIcon-" + (i - 1)).setAttribute("onclick", "RemoveSection(" + (i - 1) + ")");
        var sectionTitle = document.getElementById("SectionTitle-" + i);
        sectionTitle.id = "SectionTitle-" + (i - 1);
        sectionTitle.name = "SectionTitle-" + (i - 1);
        sectionTitle.setAttribute("placeholder", "Title of section " + (i - 1) + "!");
        var sectionDescription = document.getElementById("SectionDescription-" + i);
        sectionTitle.id = "SectionDescription-" + (i - 1);
        sectionDescription.name = "SectionDescription-" + (i - 1);
    }

    sectionCount--;
}

    

        

   
    
        
        
        
        
        
       
    
    
    
