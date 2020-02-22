function FilterStreams(subject) {
    var subjectDivs = $('.streamsubject');

    $.each(subjectDivs, function (index, value) {
        $(this).show();
    });

    $.each(subjectDivs, function (index, value) {
        if ($(this).hasClass(subject) == false) {
            $(this).hide();
        }
    });
}

function FollowStreamTutor(tutor) {
    $.ajax({
        url: '/Home/ProfileView',
        type: 'post',
        datatype: 'json',
        data: {
            'followRequest': 'follow',
            'tutorId': tutor
        }
    });

    $('#FollowButton').hide();
    $('#UnfollowButton').show();
}

function UnfollowStreamTutor(tutor) {
    $.ajax({
        url: '/Home/ProfileView',
        type: 'post',
        datatype: 'json',
        data: {
            'unFollowRequest': 'Unfollow',
            'tutorId': tutor
        }
    });

    $('#FollowButton').show();
    $('#UnfollowButton').hide();
}

function showSubmitRecommendationModal() {
    $('#submitRecommendationModal').modal('show');
}

function submitRecommendation(student, tutor) {
    var text = $("#submitRecommendationText").val()

    if (text != "") {
        $.ajax({
            type: "POST",
            url: '/Home/CreateRecommendation',
            dataType: 'json',
            data: {
                'student': student,
                'tutor': tutor,
                'recommendation': text,
            },
            success: function (data) {

            }
        });
    }
}

function WriteStudentGreeting() {
    document.getElementById("ProfileCaptionOnPage").style.display = "none";
    document.getElementById('ProfileParagraphOnPage').style.display = "none";
    document.getElementById('ProfileCaptionGreeting').style.display = "block";
    document.getElementById('ProfileParagraphGreeting').style.display = "block";
}

function WriteStudentProfileCaptionAndParagraph() {
    document.getElementById('ProfileCaptionGreeting').style.display = "none";
    document.getElementById('ProfileParagraphGreeting').style.display = "none";
    document.getElementById('ProfileCaptionOnPage').style.display = "block";
    document.getElementById('ProfileParagraphOnPage').style.display = "block";
}