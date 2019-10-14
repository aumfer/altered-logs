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
using System.Linq;
using Altered.Aws.Alb;
using System.IO.Compression;
using System.Reactive.Concurrency;

namespace Altered.Logs.Alb
{
    public sealed class GetAlbLog : AlteredPipeline<S3EventNotification, CsvReader>
    {
        public GetAlbLog(IAmazonS3 s3) : base((s3Event) =>
         (from objs in s3.GetBucketObjectsAsync(s3Event).ToObservable()
         from obj in objs
         from zip in new[] { new GZipStream(obj.ResponseStream, CompressionMode.Decompress) }//.Use()
         select zip)
            .ToAlbCsv().ToTask())
        {

        }
    }
}
