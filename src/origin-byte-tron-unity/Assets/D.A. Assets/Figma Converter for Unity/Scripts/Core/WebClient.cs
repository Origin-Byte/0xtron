using DA_Assets.FCU.Model;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using DA_Assets.FCU.Config;
using DA_Assets.Shared;
using Random = UnityEngine.Random;
using Console = DA_Assets.Shared.Console;
using DA_Assets.FCU.Extensions;
using System.Linq;
using System.IO;
using System.Collections.Concurrent;
using System;
using DA_Assets.Shared.CodeHelpers;

#if JSON_NET_EXISTS
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace DA_Assets.FCU.Core
{
    [Serializable]
    public class WebClient : ControllerHolder<FigmaConverterUnity>
    {
        [SerializeField] float pbarProgress;
        [SerializeField] float pbarBytes;
        public float PbarProgress { get => pbarProgress; set => SetValue(ref pbarProgress, value); }
        public float PbarBytes { get => pbarBytes; set => SetValue(ref pbarBytes, value); }

        public IEnumerator Auth(Return<AuthResult> @return)
        {
            string code = "";

            bool gettingCode = true;

            Thread thread = null;

            Console.WriteLine(LocKey.log_open_auth_page.Localize());

            thread = new Thread(x =>
            {
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 1923);

                server.Bind(endpoint);
                server.Listen(1);

                Socket socket = server.Accept();

                byte[] bytes = new byte[1000];
                socket.Receive(bytes);
                string rawCode = Encoding.UTF8.GetString(bytes);

                string toSend = "HTTP/1.1 200 OK\nContent-Type: text/html\nConnection: close\n\n" + @"
                    <html>
                        <head>
                            <style type='text/css'>body,html{background-color: #000000;color: #fff;font-family: Segoe UI;text-align: center;}h2{left: 0; position: absolute; top: calc(50% - 25px); width: 100%;}</style>
                            <title>Wait for redirect...</title>
                            <script type='text/javascript'> window.onload=function(){window.location.href='https://figma.com';}</script>
                        </head>
                        <body>
                            <h2>Authorization completed. The page will close automatically.</h2>
                        </body>
                    </html>";
                bytes = Encoding.UTF8.GetBytes(toSend);

                NetworkStream stream = new NetworkStream(socket);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();

                stream.Close();
                socket.Close();
                server.Close();

                code = rawCode.GetBetween("?code=", "&state=");
                gettingCode = false;
                thread.Abort();
            });

            thread.Start();

            int state = Random.Range(0, int.MaxValue);
            string formattedOauthUrl = string.Format(FCU_Config.Instance.OAuthUrl, FCU_Config.Instance.ClientId, FCU_Config.Instance.RedirectUri, state.ToString());

            Application.OpenURL(formattedOauthUrl);

            while (gettingCode)
            {
                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay01);
            }

            Request tokenRequest = RequestCreator.CreateTokenRequest(code);

            yield return controller.Model.DynamicCoroutine(MakeRequest<AuthResult>(tokenRequest, result =>
            {
                @return.Invoke(result);
            }));
        }
        public IEnumerator GetCurrentFigmaUser(Return<FigmaUser> @return)
        {
            Request request = new Request
            {
                Query = "https://api.figma.com/v1/me",
                RequestType = RequestType.Get,
                RequestHeader = new RequestHeader
                {
                    Name = "Authorization",
                    Value = $"Bearer {controller.Model.MainSettings.Token}"
                }
            };

            yield return controller.Model.DynamicCoroutine(MakeRequest<FigmaUser>(request, result =>
            {
                @return.Invoke(result);
            }));
        }

        public IEnumerator DownloadProject(Return<FigmaProject> @return)
        {
            Request projectRequest = RequestCreator.CreateProjectRequest(
                controller.Model.MainSettings.Token,
                controller.Model.MainSettings.ProjectUrl);

            yield return controller.Model.DynamicCoroutine(MakeRequest<FigmaProject>(projectRequest, result =>
            {
                @return.Invoke(result);
            }));
        }
        public IEnumerator SetImageLinks(List<FObject> pageFObjects)
        {
            List<string> ids = pageFObjects
                .Where(x => x.DownloadableFile)
                .Select(x => x.Id)
                .ToList();

            List<List<string>> chunks = ids.ToChunks(Config.FCU_Config.Instance.GetImageLinksChunkSize);
            List<List<FigmaImage>> filledChunks = new List<List<FigmaImage>>();

            foreach (List<string> chunk in chunks)
            {
                Request request = RequestCreator.CreateImageLinksRequest(chunk, controller);

                controller.Model.DynamicCoroutine(GetImageLinks(request, result =>
                {
                    if (result.Success)
                    {
                        filledChunks.Add(result.Result);
                    }
                    else
                    {
                        Console.LogError($"{LocKey.log_unknown_error.Localize(result.Error.ParseError())}");
                        filledChunks.Add(default);
                    }
                }));
            }

            int tempCount = -1;


            while (chunks.Count() != filledChunks.Count())
            {
                if (tempCount != filledChunks.Count())
                {
                    Console.WriteLine(LocKey.log_getting_links.Localize(), filledChunks.FromChunks().Count(), chunks.FromChunks().Count());
                    tempCount = filledChunks.Count();
                }

                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay1);
            }

            List<FigmaImage> figmaImages = filledChunks.FromChunks();

            if (FCU_Config.Instance.Https == false)
            {
                figmaImages.ForEach(x => x.Link = x.Link.Replace("https://", "http://"));
            }

            int addedLinksCount = 0;

            for (int i = 0; i < pageFObjects.Count(); i++)
            {
                for (int j = 0; j < figmaImages.Count(); j++)
                {
                    if (pageFObjects[i].Id == figmaImages[j].Id)
                    {
                        if (string.IsNullOrWhiteSpace(figmaImages[j].Link))
                        {
                            Console.LogError($"Can't get link, please check the following component: {pageFObjects[i].Hierarchy}");
                            pageFObjects[i].DownloadableFile = false;
                        }
                        else
                        {
                            pageFObjects[i].Link = figmaImages[j].Link;
                        }

                        addedLinksCount++;
                    }
                }
            }

            Console.WriteLine(LocKey.log_links_added.Localize(), addedLinksCount, chunks.FromChunks().Count());
        }
        public IEnumerator GetImageLinks(Request request, Return<List<FigmaImage>> @return)
        {
            List<FigmaImage> figmaImages = new List<FigmaImage>();

            yield return controller.Model.DynamicCoroutine(MakeRequest<FigmaImageRequest>(request, (result) =>
            {
                if (result.Success)
                {
                    foreach (var image in result.Result.Images)
                    {
                        figmaImages.Add(new FigmaImage
                        {
                            Id = image.Key,
                            Link = image.Value
                        });
                    }

                    @return.Invoke(new CoroutineResult<List<FigmaImage>>
                    {
                        Success = true,
                        Result = figmaImages
                    });
                }
                else
                {
                    @return.Invoke(new CoroutineResult<List<FigmaImage>>
                    {
                        Success = false,
                        Error = result.Error
                    });
                }
            }));
        }
        public IEnumerator DownloadImages(List<FObject> fobjects)
        {
            List<FObject> childToDownload = fobjects.Where(x => x.DownloadableFile).ToList();
            ConcurrentBag<DownloadedImage> images = new ConcurrentBag<DownloadedImage>();

            foreach (FObject fobject in childToDownload)
            {
                Request request = new Request
                {
                    RequestType = RequestType.GetFile,
                    Query = fobject.Link
                };

                controller.Model.DynamicCoroutine(MakeRequest<byte[]>(request, (result) =>
                {
                    if (result.Success)
                    {
                        images.Add(new DownloadedImage
                        {
                            FObject = fobject,
                            ByteImage = result.Result
                        });
                    }
                    else
                    {
                        Console.LogError(LocKey.log_unknown_error.Localize(result.Error.ParseError()));

                        images.Add(new DownloadedImage
                        {
                            FObject = fobject,
                        });
                    }
                }));
            }

            int tempCount = -1;

            while (childToDownload.Count() != images.Count())
            {
                if (tempCount != images.Count())
                {
                    Console.WriteLine(LocKey.log_downloading_images.Localize(), images.Count(), childToDownload.Count());
                    tempCount = images.Count();
                }

                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay1);
            }

            foreach (DownloadedImage image in images)
            {
                if (image.ByteImage == null || image.ByteImage.Length < 1)
                {
                    continue;
                }

                File.WriteAllBytes(image.FObject.AssetPath, image.ByteImage);
            }

            Console.WriteLine(LocKey.log_draw_components.Localize(), fobjects.Count());

            yield return null;
        }
        public IEnumerator MakeRequest<T>(Request request, Return<T> @return)
        {
            UnityWebRequest webRequest;

            switch (request.RequestType)
            {
                case RequestType.Post:
                    webRequest = UnityWebRequest.Post(request.Query, request.WWWForm);
                    break;
                default:
                    webRequest = UnityWebRequest.Get(request.Query);
                    break;
            }

            using (webRequest)
            {
                if (request.RequestHeader.Equals(default(RequestHeader)) == false)
                {
                    webRequest.SetRequestHeader(request.RequestHeader.Name, request.RequestHeader.Value);
                }

                webRequest.SendWebRequest();

                yield return controller.Model.DynamicCoroutine(UpdateRequestProgressBar(webRequest));

                bool isRequestError;
#if UNITY_2020_1_OR_NEWER
                isRequestError = webRequest.result == UnityWebRequest.Result.ConnectionError;
#else
                isRequestError = webRequest.isNetworkError || webRequest.isHttpError;
#endif
                if (isRequestError)
                {
                    CoroutineResult<T> errorResult = new CoroutineResult<T>
                    {
                        Success = false
                    };

                    if (webRequest.error.Contains("SSL"))
                    {
                        Console.LogError(LocKey.log_ssl_error.Localize(webRequest.error));
                        controller.StopImport();
                        yield break;
                    }
                    else
                    {
                        errorResult.Error = new FigmaError
                        {
                            Status = (int)webRequest.responseCode,
                            Error = webRequest.error
                        };
                    }

                    @return.Invoke(errorResult);
                    yield break;
                }

                yield return controller.Model.DynamicCoroutine(MoveRequestProgressBarToEnd());

                T requestResult;

                if (request.RequestType == RequestType.GetFile)
                {
                    requestResult = (T)(object)webRequest.downloadHandler.data;
                }
                else
                {
                    string text = webRequest.downloadHandler.text;

                    if (controller.Model.MainSettings.DebugMode)
                    {
                        controller.Model.DynamicCoroutine(request.WriteLog(text));
                    }

                    if (text.Contains($"<H1>413 ERROR</H1>"))
                    {
                        Console.LogError(LocKey.log_anti_ddos.Localize());
                        controller.StopImport();
                        yield break;
                    }
#if JSON_NET_EXISTS
                    if (text.IsJsonValid())
                    {
                        JToken parsedJson = JToken.Parse(text);
                        text = parsedJson.ToString(Formatting.Indented);
                    }
                    else
                    {
                        @return.Invoke(new CoroutineResult<T>
                        {
                            Success = false,
                            Error = new FigmaError
                            {
                                Status = 998,
                                Error = text
                            }
                        });

                        yield break;
                    }

                    bool figmaErrorExists = text.TryParseJson(out FigmaError figmaError);

                    if (figmaErrorExists)
                    {
                        @return.Invoke(new CoroutineResult<T>
                        {
                            Success = false,
                            Error = figmaError
                        });

                        yield break;
                    }

                    requestResult = JsonConvert.DeserializeObject<T>(text, JsonExtensions.JsonSerializerSettings);
                    controller.Cacher.Cache(requestResult, text);
#else
                    Console.LogError(LocKey.log_json_net_not_found.Localize());
                    controller.StopImport();
                    yield break;                 
#endif
                }

                @return.Invoke(new CoroutineResult<T>
                {
                    Success = true,
                    Result = requestResult
                });
            }
        }
        private IEnumerator UpdateRequestProgressBar(UnityWebRequest webRequest)
        {
            while (webRequest.isDone == false)
            {
                if (pbarProgress < 1f)
                {
                    pbarProgress += FCU_Config.Instance.Delay001;
                }
                else
                {
                    pbarProgress = 0;
                }

                if (webRequest.downloadedBytes == 0)
                {
                    pbarBytes += 100;
                }
                else
                {
                    pbarBytes = webRequest.downloadedBytes;
                }

                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay001);
            }
        }
        private IEnumerator MoveRequestProgressBarToEnd()
        {
            while (true)
            {
                if (pbarProgress < 1f)
                {
                    pbarProgress += FCU_Config.Instance.Delay001;
                    yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay001);
                }
                else
                {
                    pbarProgress = 0f;
                    pbarBytes = 0f;
                    break;
                }
            }
        }
    }
    public struct Request
    {
        public string Query;
        public RequestType RequestType;
        public RequestHeader RequestHeader;
        public WWWForm WWWForm;
    }
    public struct RequestHeader
    {
        public string Name;
        public string Value;
    }
    public enum RequestType
    {
        Get,
        Post,
        GetFile,
    }
    public struct AuthResult
    {
        public string access_token;
        public string expires_in;
        public string refresh_token;
    }
    public struct FigmaImageRequest
    {
#if JSON_NET_EXISTS
        [JsonProperty("err")]
#endif
        public string Error;
#if JSON_NET_EXISTS
        [JsonProperty("images")]
#endif
        public Dictionary<string, string> Images;
    }
    public struct DownloadedImage
    {
        public FObject FObject;
        public byte[] ByteImage;
    }
    public struct FigmaImage
    {
        public string Id;
        public string Link;
    }
}
