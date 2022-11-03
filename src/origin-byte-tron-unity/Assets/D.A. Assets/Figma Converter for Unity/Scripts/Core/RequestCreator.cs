using DA_Assets.FCU.Config;
using DA_Assets.FCU.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU.Core
{
    public class RequestCreator
    {
        public static Request CreateImageLinksRequest(List<string> chunk, FigmaConverterUnity fcu)
        {
            string query = CreateImagesQuery(
                    chunk,
                    fcu.Model.MainSettings.ProjectUrl,
                    fcu.Model.MainSettings.ImageFormat.GetImageFormat(),
                    fcu.Model.MainSettings.ImageScale.GetImageScale());

            Request request = new Request
            {
                Query = query,
                RequestType = RequestType.Get,
                RequestHeader = new RequestHeader
                {
                    Name = "Authorization",
                    Value = $"Bearer {fcu.Model.MainSettings.Token}"
                }
            };

            return request;
        }
        public static string CreateImagesQuery(List<string> chunk, string projectId, string extension, float scale)
        {
            string joinedIds = string.Join(",", chunk);
            if (joinedIds[0] == ',')
            {
                joinedIds = joinedIds.Remove(0, 1);
            }

            string query = $"https://api.figma.com/v1/images/{projectId}?ids={joinedIds}&format={extension}&scale={scale}";
            return query;
        }
        public static Request CreateTokenRequest(string code)
        {
            string tokenQueryLink = string.Format(FCU_Config.Instance.AuthUrl, FCU_Config.Instance.ClientId, FCU_Config.Instance.ClientSecret, FCU_Config.Instance.RedirectUri, code);

            Request request = new Request
            {
                Query = tokenQueryLink,
                RequestType = RequestType.Post,
                WWWForm = new WWWForm()
            };

            return request;
        }
        public static Request CreateProjectRequest(string token, string projectId)
        {
            string query = string.Format(FCU_Config.Instance.ApiLink, projectId);

            Request request = new Request
            {
                Query = query,
                RequestType = RequestType.Get,
                RequestHeader = new RequestHeader
                {
                    Name = "Authorization",
                    Value = $"Bearer {token}"
                }
            };

            return request;
        }
    }
}