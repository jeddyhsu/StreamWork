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
    event.preventDefault();
    var section = "<div class='divider'></div><div id='formfrom' class='form-group col-lg-12'><label class='formHeaders'>Section 2 Title</label> <input class='form-control border rounded-0 formInput' placeholder='Title of section two!'><label class='formHeaders pt-3'>Description</label><textarea class='form-control border rounded - 0 formTextArea' placeholder='Tell us what you're studying, concentrations, passions, and other extra curricular activities here!'></textarea></div>"
    document.getElementById("formRow").innerHTML += section
    var e = document.getElementById("formfrom");
    e.scrollIntoView();
}


    
   
        
        
                                       