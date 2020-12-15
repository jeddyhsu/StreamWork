using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.Services;

namespace StreamWork.Pages.Stream
{
    [IgnoreAntiforgeryToken]
    public class Webhook : PageModel
    {
        StreamService streamService;

        public Webhook(StreamService stream)
        {
            streamService = stream;
        }

        public async Task<IActionResult> OnPost(string id, string action, string streamName, string category)
        {
            if (action == "liveStreamStarted") await streamService.StreamStarted(id, action, streamName, category);
            else if (action == "liveStreamEnded") await streamService.StreamEnded(id, action, streamName, category);
            else if (action == "vodReady") await streamService.VideoReady(id, action, streamName);

            return null;
        }
    }
}
