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

function GetScheduleTemplate(subjectThumbnail, date, streamTitle, streamSubject, timeStart, timeStop, timeZone) {
    var scheduleTemplate = `<div class="card border-0">
                                <div class="card-body">
                                    <div class="d-inline-flex">
                                        <img class="rounded m-1 schedule-image" src="${subjectThumbnail}" />
                                        <div class="text-center m-1 schedule-image schedule-border">
                                            <p class="form-header schedule-month">${moment(date).format('MMM')}</p>
                                        </div>
                                        <div class="text-center m-1 schedule-image schedule-border">
                                            <p class="form-header schedule-day">${moment(date).format('D')}</p>
                                            <p class="form-header schedule-dow">${moment(date).format('ddd')}</p>
                                        </div>
                                        <div class="m-1" style="height:75px;">
                                            <p class="form-header m-0">${streamTitle}</p>
                                            <p class="mt-1 mb-0" style="font-size:10px">${streamSubject}</p>
                                            <p class="schedule-time">${timeStart} - ${timeStop} [${timeZone}]</p>
                                        </div>
                                    </div>
                                </div>
                            </div>`

    return scheduleTemplate;
}