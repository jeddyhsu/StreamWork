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
                                    <p class="form-header mb-1 mt-1" style="font-size:10px"><a class="streamWork-green" href="/Profiles/Tutor/${name.replace('|', ' ')}</a></p>
                                </div>
                                <div class="card-footer pt-1 pb-1 stream-video-footer-radius" style="background-color:${streamColor}">
                                    <p class="form-header p-0 m-0 streamWork-white roboto float-right" style="font-size:12px"><b>${streamSubject}</b></p>
                                </div>
                           </div>`
    return streamTemplate;
}