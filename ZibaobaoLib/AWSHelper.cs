using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ZibaobaoLib.Helpers;

namespace ZibaobaoLib
{
    public interface FileHandler
    {
        void OnFileAvailable(string path, string content=null);
    }
    public class AWSHelper
    {
        static string ZibaobaoBucket = "relaxingtech.com";

        // Initialize the Amazon Cognito credentials provider
        static CognitoAWSCredentials Credentials;

        static AWSHelper _instance;
        public static AWSHelper Instance => _instance ?? (_instance = new AWSHelper());

        Dictionary<string, FileHandler> _fileHandlers = new Dictionary<string, FileHandler>();

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

            //key id: AKIAJ73CRJAXPE2TXXOQ
            //access: eVatiCgh8cCGQ8uyAGCpWF8lR5SYGC542kXiCBCo
        }

        public IAmazonS3 AWSS3Client => new AmazonS3Client(Credentials, RegionEndpoint.APSoutheast1);
        public TransferUtility AWSS3TransferClient => new TransferUtility(AWSS3Client);
        public async Task AddBucket(string remotePath)
        {
            remotePath = ConvertToRemotePath(remotePath);
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

        public static string ConvertToRemotePath(string localPath)
        {
            return localPath.Replace("\\", "/");
        }

        public async Task<List<string>> ListFiles(string remotePath, bool includingFolder=false,bool includingFiles=true)
        {
            var list = new List<string>();
            try
            {
                using (var s3Client = AWSS3Client)
                {
                    remotePath = ConvertToRemotePath(remotePath);
                    try
                    {
                        var response = await s3Client.ListObjectsAsync(ZibaobaoBucket, remotePath);
                        {
                            foreach (var s3Object in response.S3Objects)
                            {
                                var name = s3Object.Key;
                                if (!includingFolder && name.EndsWith("/"))
                                {
                                    continue;
                                }
                                if (!includingFiles && !name.EndsWith("/"))
                                {
                                    continue;
                                }
                                if (name.StartsWith(remotePath))
                                {
                                    var localPath = name.Substring(remotePath.Length).Trim('/');
                                    if (!string.IsNullOrEmpty(localPath))
                                    {
                                        list.Add(localPath);
                                    }
                                }
                            }
                        }
                    }
                    catch (AmazonS3Exception e)
                    {
                        X1LogHelper.Exception(e);
                    }
                }
            }
            catch (Exception ex)
            {
                X1LogHelper.Exception(ex);
            }
            return list;
        }

        public async Task UploadFile(string remotePath, string localPath)
        {
            try
            {
                using (var s3Client = AWSS3Client)
                {
                    using (var dataStream = ZibaobaoLibContext.Instance.PersistentStorage.LoadContent(localPath))
                    {
                        remotePath = ConvertToRemotePath(remotePath);
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
                    remotePath = ConvertToRemotePath(remotePath);
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
        public async Task DownloadFile(string remotePath, string localPath=null, FileHandler handler = null)
        {
            try
            {
                var pathKey = localPath ?? remotePath;
                using (var s3Client = AWSS3Client)
                {
                    if (handler != null)
                    {
                        _fileHandlers[pathKey] = handler;
                    }
                    var request = new GetObjectRequest
                    {
                        BucketName = ZibaobaoBucket,
                        Key = ConvertToRemotePath(remotePath)
                    };

                    try
                    {
                        using (var response = await s3Client.GetObjectAsync(request))
                        {
                            response.WriteObjectProgressEvent += Request_WriteObjectProgressEvent;
                            string content = null;
                            using (var stream = response.ResponseStream)
                            {
                                if (!string.IsNullOrEmpty(localPath))
                                {
                                    ZibaobaoLibContext.Instance.PersistentStorage.SaveContent(localPath, stream);
                                }
                                else
                                {
                                    using (var reader = new StreamReader(stream))
                                    {
                                        content = reader.ReadToEnd();
                                    }
                                }
                            }

                            if (_fileHandlers.ContainsKey(pathKey))
                            {
                                _fileHandlers[pathKey].OnFileAvailable(pathKey, content);
                                _fileHandlers.Remove(pathKey);
                            }
                            else
                            {
                                OnFileAvailable?.Invoke(this, new StringEventArgs(pathKey));
                            }
                        }
                    }
                    catch (AmazonS3Exception s3Exception)
                    {
                        X1LogHelper.Exception(s3Exception);
                    }
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