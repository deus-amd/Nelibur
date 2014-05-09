﻿using System;
using System.Collections.Specialized;
using System.Linq;
using Nelibur.Core;
using Nelibur.ServiceModel.Contracts;
using Nelibur.ServiceModel.Serializers;
using Nelibur.ServiceModel.Services.Operations;

namespace Nelibur.ServiceModel.Extensions
{
    internal static class UrlExtensions
    {
        internal static string ToUrl<T>(this T value, Uri serviceAddress, string operationType, bool responseRequired = true)
            where T : class
        {
            var builder = new UriBuilder(serviceAddress);
            switch (operationType)
            {
                case OperationType.Post:
                    builder = (responseRequired
                        ? builder.AddPath(RestServiceMetadata.Path.PostWithResponse)
                        : builder.AddPath(RestServiceMetadata.Path.Post))
                        .AddQuery(UrlSerializer.FromType(typeof(T)).QueryParams);
                    break;
                case OperationType.Put:
                    builder = (responseRequired
                        ? builder.AddPath(RestServiceMetadata.Path.PutWithResponse)
                        : builder.AddPath(RestServiceMetadata.Path.Put))
                        .AddQuery(UrlSerializer.FromType(typeof(T)).QueryParams);
                    break;
                case OperationType.Get:
                    builder = (responseRequired
                        ? builder.AddPath(RestServiceMetadata.Path.GetWithResponse)
                        : builder.AddPath(RestServiceMetadata.Path.Get))
                        .AddQuery(UrlSerializer.FromValue(value).QueryParams);
                    break;
                case OperationType.Delete:
                    builder = (responseRequired
                        ? builder.AddPath(RestServiceMetadata.Path.DeleteWithResponse)
                        : builder.AddPath(RestServiceMetadata.Path.Delete))
                        .AddQuery(UrlSerializer.FromValue(value).QueryParams);
                    break;
                default:
                    string errorMessage = string.Format(
                        "OperationType {0} with void return is absent", operationType);
                    throw Error.InvalidOperation(errorMessage);
            }
            return builder.Uri.ToString();
        }

        private static UriBuilder AddPath(this UriBuilder builder, string path)
        {
            string currentPath = builder.Path;
            builder.Path = string.Format(currentPath.EndsWith("/") ? "{0}{1}" : "{0}/{1}", currentPath, path);
            return builder;
        }

        private static UriBuilder AddQuery(this UriBuilder builder, NameValueCollection queryCollection)
        {
            string[] query = queryCollection
                .AllKeys.Select(key => string.Format("{0}={1}", key, queryCollection[key]))
                .ToArray();
            builder.Query = string.Join("&", query);
            return builder;
        }
    }
}
