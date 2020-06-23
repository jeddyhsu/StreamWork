
function Dashboard(profileType) {
    if (profileType == "tutor") {
        window.location.href = "/Tutor/TutorDashboard"
    }
    else {
         window.location.href = "/Student/ProfileStudent"
    }
}

function Logout() {
    $.ajax({
        url: '/Home/Logout',
        type: 'post',
        dataType: 'json',
        data: {
            'Logout': 'logout'
        },
        success: function (data) {
            if (data.message === "Success") {
                window.location.href = "/"
            }
        }
    })
}
