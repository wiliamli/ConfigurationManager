﻿using System;

namespace Jwell.ConfigurationManager.Core
{
    public class ConfigConsts
    {
        public const string NamespaceApplication = "application";
        public const string ClusterNameDefault = "default";
        public const string ClusterNamespaceSeparator = "+";
        public const string NoAppidPlaceholder = "JwellNoAppIdPlaceHolder";
        public const string DefaultMetaServerUrl = "http://localhost:8080";

        public static bool IsUnix { get; } = Environment.CurrentDirectory[0] == '/';
        public static string DefaultLocalCacheDir { get; } = IsUnix ? "/opt/data" : @"C:\opt\data";
    }
}

