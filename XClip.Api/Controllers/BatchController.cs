
using System;
using System.Collections.Generic;
using System.Web.Http;

using XClip.Api.Models;
using XClip.DataObjects;

namespace XClip.Api.Controllers
{
    [RoutePrefix("api/batches")]
    public class BatchController : ApiController
    {

        [HttpPost]
        [Route]
        public IHttpActionResult Post([FromBody]batch batch)
        {

            // get the mediaSource
            var source = new SourceManager().Get(batch.sourceId);

            var newBatch = new XBatch
            {
                Filename = source.Filename,
                InputDir = string.Empty,
                Items = new List<XBatchItem>(),
                OutputDir = string.Empty,
                OutputMask = string.Empty,
                SrcId = batch.sourceId
            };

            var batchItems = new List<XBatchItem>();
            foreach (var x in batch.items)
            {
                var bi = new XBatchItem()
                {
                    Tags = new List<int>(),
                    Duration = string.Empty,
                    Index = x.index,
                    Start = x.start,
                    Stop = x.stop
                };

                x.tags.ForEach(y =>
                {
                    bi.Tags.Add(y);
                });

                batch.tags.ForEach(z =>
                {
                    bi.Tags.Add(z);
                });

                newBatch.Items.Add(bi);
            }

            var created = new BatchManager().Save(newBatch);

            if (created)
                return Ok(newBatch);

            return BadRequest("Batch creation failed");

        }
    }
}