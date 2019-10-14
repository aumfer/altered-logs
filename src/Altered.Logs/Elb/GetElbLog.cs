using Amazon.S3;
using Amazon.S3.Util;
using Altered.Aws;
using Altered.Aws.S3;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Reactive.Linq;
using System.IO.Compression;
using Altered.Aws.Elb;
using System.IO;

namespace Altered.Logs.Elb
{
    public sealed class GetElbLog : AlteredPipeline<S3EventNotification, CsvReader>
    {
        public GetElbLog(IAmazonS3 s3) : base((s3Event) =>
         (from objs in s3.GetBucketObjectsAsync(s3Event).ToObservable()
          from obj in objs
          select obj.ResponseStream)
            .ToElbCsv().ToTask())
        {
        }
    }
}
