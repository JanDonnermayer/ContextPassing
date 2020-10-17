using System;

namespace ContextPassing.API
{
    public static class Configuration
    {
        public static string BlobEndpoint =>
            Environment.GetEnvironmentVariable("BLOB_STORAGE_ENDPOINT");

        public static string CartServicePublicApi =>
            Environment.GetEnvironmentVariable("ENDPOINT");

        public static string CartServiceInternalApi =>
            Environment.GetEnvironmentVariable("ENDPOINT");

    }
}