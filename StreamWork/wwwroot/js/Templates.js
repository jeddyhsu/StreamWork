function GetVideoTemplate(id, streamSubject, streamId, streamThumbnail, streamTitle, streamColor, name, username) {
    var videoTemplate = `<div class="card mt-3 border h-100 stream-video-radius">
                                <div class="card-title">
                                    <a href="/Stream/Archive/${streamId}/32169"><img class="stream-video-image" src="${streamThumbnail}"></a>
                                </div>
                                <div class="card-body pt-0 pb-1">
                                    <h5 class="form-header multiline-truncate mb-0">${streamTitle}</h5>
                                    <p class="form-header mb-1 mt-1" style="font-size:10px"><a class="streamWork-green" href="/Profiles/Tutor/${username}">${name.replace('|', ' ')}</a></p>
                                </div>
                                <div class="card-footer pt-1 pb-1 stream-video-footer-radius" style="background-color:${streamColor};">
                                    <p class="form-header streamWork-white p-0 m-0 roboto float-right" style="font-size:12px;"><b>${streamSubject}</b></p>
                                </div>
                          </div>`
    return videoTemplate;
}

function GetStreamTemplate(id, streamSubject, streamThumbnail, streamTitle, streamColor, name, username) {
    var streamTemplate = ` <div class="card mt-3 border h-100 stream-video-radius">
                                <div class="card-title">
                                    <a href="/Stream/Live/${username}"><img class="stream-video-image" src="${streamThumbnail}"></a>
                                </div>
                                <div class="card-body pt-0 pb-1">
                                    <h5 class="form-header multiline-truncate mb-0">${streamTitle}</h5>
                                    <p class="form-header mb-1 mt-1" style="font-size:10px"><a class="streamWork-green" href="/Profiles/Tutor/${username}">${name.replace('|', ' ')}</a></p>
                                </div>
                                <div class="card-footer pt-1 pb-1 stream-video-footer-radius" style="background-color:${streamColor};">
                                    <p class="form-header streamWork-white p-0 m-0 roboto float-right" style="font-size:12px;"><b>${streamSubject}</b></p>
                                </div>
                          </div>`
    return streamTemplate;
}

function GetTutoDashVideoTemplate(id, streamSubject, streamId, streamThumbnail, streamTitle, streamColor, streamDescription) {
    var videoTemplate = `<div class="card h-100 mt-3 border-0 stream-video-radius">
                            <div class="card-title">
                                <a href="../Stream/Archive/${streamId}/32169"><img id="video-thumbnail-${id}" class="stream-video-image" src="${streamThumbnail}"></a>
                            </div>
                            <div class="card-body pt-0 pb-1">
                                <h5 id="video-title-${id}" class="form-header multiline-truncate mb-0">${streamTitle}</h5>
                                <p class="form-header mb-1 mt-1" style="font-size:10px">${streamSubject}</p>
                                <input id="video-description-${id}" type="hidden" value="${streamDescription}" />
                            </div>
                            <div class="card-footer p-0 stream-video-footer-radius" style="background-color:${streamColor};"><span style="color:white; cursor:pointer; float:right; padding-right:10px" onclick="EditVideo('${id}')">&#8943;</span></div>
                        </div>`
    return videoTemplate;
}

function GetScheduleTemplate(subjectThumbnail, date, streamTitle, streamSubject, timeStart, timeStop, timeZone, username, name) {
    var scheduleTemplate = `<div class="card h-100 border-0">
                                <div class="card-body">
                                    <div class="d-flex justify-content-center">
                                        <img class="rounded m-1 schedule-image" src="${subjectThumbnail}" />
                                        <div class="text-center m-1 schedule-image schedule-border">
                                            <p class="form-header schedule-month">${moment(date).format('MMM')}</p>
                                        </div>
                                        <div class="text-center m-1 schedule-image schedule-border">
                                            <p class="form-header schedule-day">${moment(date).format('D')}</p>
                                            <p class="form-header schedule-dow">${moment(date).format('ddd')}</p>
                                        </div>
                                    </div>
                                    <div class="m-1 text-center">
                                        <p class="form-header m-0 schedule-title">${streamTitle}</p>
                                        <p class="mt-1 mb-0 roboto schedule-tutor"><a class="streamWork-green" href="/Profiles/Tutor/${username}">${name.replace('|', ' ')}</a> Tutoring ${streamSubject}</p>
                                        <p class="schedule-time">${timeStart} - ${timeStop} [${timeZone}]</p>
                                    </div>
                                </div>
                            </div>`

    return scheduleTemplate;
}

function GetTutorTemplate(username, profileBanner, profilePicture, profileColor, name, profileCaption, topicColor, topic) {
    var tutorTemplate = `<div class="card rounded pointer border-0 h-100" style="min-height:250px;" onclick="window.location.href='/Profiles/Tutor/${username}'">
                            <div class="card-body p-0">
                                <img class="card-img-top" src="${profileBanner}" height="150" style="object-fit:cover">
                                <div class="profile-picture-overlap-container">
                                    <img id="header-profile-picture" align="left" class="rounded" src="${profilePicture}" style="width:70px;" />
                                </div>
                                <div class="d-inline-block">
                                    <p class="form-header header-padding mb-0 mt-2" style="font-size:16px; color:${profileColor}">${name.replace('|', ' ')}</p>
                                    <p class="form-sub-header header-padding mb-3" style="font-size:12px;">${profileCaption == null ? "" : profileCaption}</p>
                                </div>
                           </div>
                           <div class="card-footer pt-1 pb-1 " style="background-color:${topicColor};">
                               <p class="form-header streamWork-white p-0 m-0 roboto float-right" style="font-size:12px;"><b>${topic}</b></p>
                           </div>
                        </div>`

    return tutorTemplate;
}