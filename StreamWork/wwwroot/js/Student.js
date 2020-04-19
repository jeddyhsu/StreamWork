function FollowStreamTutor(tutor, i) {
  
    $.ajax({
        url: '/Home/ProfileView',
        type: 'post',
        datatype: 'json',
        data: {
            'followRequest': 'follow',
            'tutorId': tutor
        }
    });
    if (i == null) {
        $('#FollowButton').hide();
        $('#UnfollowButton').show();
    }
    else {
        $('#UnfollowButton-' + i).show();
        $('#FollowButton-' + i).hide();
    }
}

function UnfollowStreamTutor(tutor, i) {
    $.ajax({
        url: '/Home/ProfileView',
        type: 'post',
        datatype: 'json',
        data: {
            'unFollowRequest': 'Unfollow',
            'tutorId': tutor
        }
    });
    if (i == null) {
        $('#FollowButton').show();
        $('#UnfollowButton').hide();
    }
    else {
        $('#FollowButton-' + i).show();
        $('#UnfollowButton-' + i).hide();
    }
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