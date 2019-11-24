
function EditOverview() {

    var name = $('#Name').val();
    var profileCaption = $('#ProfileCaption').val();
    var profileParagraph = $('#ProfileParagraph').val();

    $.ajax({
        url: '/Tutor/TutorSettings',
        type: 'post',
        dataType: 'json',
        data: {
            'name': name,
            'profileCaption': profileCaption,
            'profileParagraph': profileParagraph
        },
        success: function (data) {
            if (data.message === "Success") {
                alert("Profile Saved!");
                location.reload();
            }
        }
    });
}

function EditSecurity() {

    var currentPassword = $('#CurrentPassword').val();
    var newPassword = $('#NewPassword').val();
    var confirmPassword = $('#ConfirmPassword').val();

    if (newPassword != confirmPassword) {
        alert("Passwords do not match.");
        return;
    }

    $.ajax({
        url: '/Tutor/TutorSettings',
        type: 'post',
        dataType: 'json',
        data: {

            'currentPassword': currentPassword,
            'newPassword': newPassword,
            'confirmPassword': confirmPassword
        },
        success: function data() {
            if (data.message === "Success") {
                alert("Password Changed!");
                location.reload();
            }
            else {
                alert("Incorrect Password");
            }
        }
    })
}