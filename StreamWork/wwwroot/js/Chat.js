var count = 0;
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (user, message, questionNumber, questionType) {
    $('#tutorQuestionGroup').append("<li id='question-" + questionNumber + "'class='list-group-item' > " + message + "</li > ");
    if (questionType != "regularQuestion") {
        var problemSpan = document.getElementById("question-" + questionNumber);
        MQ.StaticMath(problemSpan);
    }
});

function StartConnectionToServerTutor(tutorId) {
    connection.start().then(function () {
        connection.invoke("RegisterConnection", tutorId).catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

function StartConnectionToServerStudent() {
    connection.start().then(function () {
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

function SendMessageToTutor(nameOfUser, tutorId) {
    var message = "";
    var questionType = "";
    var equationMessage = document.getElementById("latex").textContent;
    var regularMessage = document.getElementById("regularQuestions").value;
    if (equationMessage == "") {
        message = regularMessage;
        questionType = "regularQuestion";
    }
    else {
        message = equationMessage;
        questionType = "equationQuestion";
    }
    connection.invoke("SendMessageToTutor", nameOfUser, tutorId, message, questionType).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("regularQuestions").value = "";
    event.preventDefault();
}

var inputCheck = false;
function SwitchInputs() {
    if (!inputCheck) {
        document.getElementById("regularQuestions").style.display = 'none';
        document.getElementById("equationQuestions").style.display = 'block';
        inputCheck = true;
    }
    else {
        document.getElementById("regularQuestions").style.display = 'block';
        document.getElementById("equationQuestions").style.display = 'none';
        inputCheck = false;
    }
}