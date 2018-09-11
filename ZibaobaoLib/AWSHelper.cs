using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ZibaobaoLib.Helpers;

namespace ZibaobaoLib
{
    public class AWSHelper
    {
        static string ZibaobaoBucket = "relaxingtech.com";

        // Initialize the Amazon Cognito credentials provider
        static CognitoAWSCredentials Credentials;

        static AWSHelper _instance;
        public static AWSHelper Instance => _instance ?? (_instance = new AWSHelper());

        static AWSHelper()
        {
            var loggingConfig = AWSConfigs.LoggingConfig;
            loggingConfig.LogMetrics = true;
            loggingConfig.LogResponses = ResponseLoggingOption.Always;
            loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;
            loggingConfig.LogTo = LoggingOptions.SystemDiagnostics;
            AWSConfigs.AWSRegion = "ap-southeast-1";
            AWSConfigsS3.UseSignatureVersion4 = true;

            Credentials = new CognitoAWSCredentials(
                "868311720350",
                "ap-southeast-1:ff736cfc-7e26-4f76-9de8-fb0180e862ea", // Identity pool ID
                "arn:aws:iam::868311720350:role/Cognito_ZiBaobaoAdventureUnauth_Role",
                null,
                RegionEndpoint.APSoutheast1 // Region
            );
        }

        public IAmazonS3 AWSS3Client => new AmazonS3Client(Credentials, RegionEndpoint.APSoutheast1);
        public TransferUtility AWSS3TransferClient => new TransferUtility(AWSS3Client);
        public async Task AddBucket(string remotePath)
        {
            remotePath = remotePath.ToLower().Replace("\\", "/");
            using (var s3Client = AWSS3Client)
            {
                var listOfBuckets = await s3Client.ListBucketsAsync();
                bool found = false;
                foreach (S3Bucket bucket in listOfBuckets.Buckets)
                {
                    if (bucket.BucketName == remotePath)
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    try
                    {
                        var response = await s3Client.PutBucketAsync(new PutBucketRequest { BucketName = remotePath, BucketRegion = S3Region.APS1});
                    }
                    catch (Exception e)
                    {
                        X1LogHelper.Exception(e);
                    }
                }
            }
        }
        public async Task UploadFile(string remotePath, string localPath)
        {
            try
            {
                using (var s3Client = AWSS3Client)
                {
                    using (var dataStream = ZibaobaoLibContext.Instance.PersistentStorage.LoadContent(localPath))
                    {
                        remotePath = remotePath.Replace("\\", "/");
                        var request = new PutObjectRequest
                        {
                            BucketName = ZibaobaoBucket,
                            Key = remotePath,
                            InputStream = dataStream
                        };

                        try
                        {
                            var response = await s3Client.PutObjectAsync(request);
                            {
                                // response. += Request_WriteObjectProgressEvent;
                                /*using (var stream = response.ResponseStream)
                                {
                                    ZibaobaoLibContext.Instance.PersistentStorage.SaveContent(localPath, stream);
                                }*/
                                //OnFileAvailable?.Invoke(this, new StringEventArgs(localPath));
                            }
                        }
                        catch (AmazonS3Exception s3Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=====ERROR ========");
            }
        }

        public async Task UploadString(string remotePath, string content)
        {
            try
            {
                using (var s3Client = AWSS3Client)
                {
                    remotePath = remotePath.Replace("\\", "/");
                    var request = new PutObjectRequest
                    {
                        BucketName = ZibaobaoBucket,
                        Key = remotePath,
                        ContentBody = content
                    };

                    try
                    {
                        await s3Client.PutObjectAsync(request);
                    }
                    catch (Exception e)
                    {
                        X1LogHelper.Exception(e);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=====ERROR ========");
            }
        }

        public async Task DownloadFile(string remotePath, string localPath)
        {
            try
            {
                using (var s3Client = AWSS3Client)
                {
                    remotePath = remotePath.Replace("\\", "/");
                    var request = new GetObjectRequest
                    {
                        BucketName = ZibaobaoBucket,
                        Key = remotePath
                    };

                    try
                    {
                        using (var response = await s3Client.GetObjectAsync(request))
                        {
                            response.WriteObjectProgressEvent += Request_WriteObjectProgressEvent;
                            using (var stream = response.ResponseStream)
                            {
                                ZibaobaoLibContext.Instance.PersistentStorage.SaveContent(localPath, stream);
                            }
                            OnFileAvailable?.Invoke(this, new StringEventArgs(localPath));
                        }
                    }
                    catch (AmazonS3Exception s3Exception)
                    {
                    }

                    /*
                    localPath = localPath.Replace(BaobaoGameContextFactory.Instance.PersistentStorage.PersonalPath, "").Trim('\\', '/');
                    TransferUtilityDownloadRequest request = new TransferUtilityDownloadRequest
                    {
                        BucketName = ZibaobaoBucket,
                        Key = remotePath,
                        FilePath = localPath,
                    };
                    request.WriteObjectProgressEvent += Request_WriteObjectProgressEvent;
                    using (var client = new TransferUtility(s3Client))
                    {
                        await client.DownloadAsync(request);
                    }*/
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=====ERROR ========");
            }
        }

        void Request_WriteObjectProgressEvent(object sender, WriteObjectProgressArgs e)
        {
            // show progress
            System.Diagnostics.Debug.WriteLine("=======UpdateFileProgress=======");
        }

        private void Request_UploadObjectProgressEvent(object sender, UploadProgressArgs e)
        {
            // show progress
            System.Diagnostics.Debug.WriteLine("=======UpdateFileProgress=======");
        }

        public event EventHandler<StringEventArgs> OnFileAvailable;

    }
}