
using System;

using XClip.DataObjects;
using XClip.Repositories;

namespace XClip
{
    public class XVideoManager
    {
        private readonly IXVideoRepository _videoRepo = new XVideoRepository();
        private readonly IXVideoItemRepository _videoItemRepository = new XVideoItemRepository();

        public void Save(int userId, XVideo video)
        {
            _videoRepo.Save(userId, video);

            video.Items.ForEach(x =>
            {
                _videoItemRepository.Save(video.Id, x);
            });
        }

        public XVideo Get(Guid uId)
        {
            var video = _videoRepo.Get(uId);

            if (video != null)
                video.Items = _videoItemRepository.GetVideoItems(video.Id);

            return video;
        }

        public XVideo Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}