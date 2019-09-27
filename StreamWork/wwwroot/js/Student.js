function GetArchivedVideosBasedOnSubject(subject) {
    $.ajax({
        url: '/Student/ArchivedStreams',
        type: 'POST',
        dataType: 'json',
        data: {
            'Subject': subject
        },
    })
}