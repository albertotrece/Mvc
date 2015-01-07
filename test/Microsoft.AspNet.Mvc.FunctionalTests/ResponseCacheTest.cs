﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using ResponseCacheWebSite;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class ResponseCacheTest
    {
        private readonly IServiceProvider _provider = TestHelper.CreateServices("ResponseCacheWebSite");
        private readonly Action<IApplicationBuilder> _app = new Startup().Configure;

        [Fact]
        public async Task ResponseCache_SetsAllHeaders()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();

            // Act
            var response = await client.GetAsync("http://localhost/CacheHeaders/Index");

            // Assert
            var data = Assert.Single(response.Headers.GetValues("Cache-control"));
            Assert.Equal("public, max-age=100", data);
            data = Assert.Single(response.Headers.GetValues("Vary"));
            Assert.Equal("Accept", data);
        }

        public static IEnumerable<object[]> CacheControlData
        {
            get
            {
                yield return new object[] { "http://localhost/CacheHeaders/PublicCache", "public, max-age=100" };
                yield return new object[] { "http://localhost/CacheHeaders/ClientCache", "max-age=100, private" };
                yield return new object[] { "http://localhost/CacheHeaders/NoStore", "no-store" };
                yield return new object[] { "http://localhost/CacheHeaders/NoCacheAtAll", "no-store, no-cache" };
            }
        }

        [Theory]
        [MemberData(nameof(CacheControlData))]
        public async Task ResponseCache_SetsDifferentCacheControlHeaders(string url, string expected)
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            var data = Assert.Single(response.Headers.GetValues("Cache-control"));
            Assert.Equal(expected, data);
        }

        [Fact]
        public async Task SetsHeadersForAllActionsOfClass()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();

            // Act
            var response1 = await client.GetAsync("http://localhost/ClassLevelCache/GetHelloWorld");
            var response2 = await client.GetAsync("http://localhost/ClassLevelCache/GetFooBar");

            // Assert
            var data = Assert.Single(response1.Headers.GetValues("Cache-control"));
            Assert.Equal("public, max-age=100", data);
            data = Assert.Single(response1.Headers.GetValues("Vary"));
            Assert.Equal("Accept", data);

            data = Assert.Single(response2.Headers.GetValues("Cache-control"));
            Assert.Equal("public, max-age=100", data);
            data = Assert.Single(response2.Headers.GetValues("Vary"));
            Assert.Equal("Accept", data);
        }

        [Fact]
        public async Task HeadersSetInActionOverridesTheOnesInClass()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();

            // Act
            var response = await client.GetAsync("http://localhost/ClassLevelCache/ConflictExistingHeader");

            // Assert
            var data = Assert.Single(response.Headers.GetValues("Cache-control"));
            Assert.Equal("public, max-age=20", data);
            data = Assert.Single(response.Headers.GetValues("Vary"));
            Assert.Equal("Accept", data);
        }

        [Fact]
        public async Task HeadersToNotCacheAParticularAction()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();

            // Act
            var response = await client.GetAsync("http://localhost/ClassLevelCache/DoNotCacheThisAction");

            // Assert
            var data = Assert.Single(response.Headers.GetValues("Cache-control"));
            Assert.Equal("no-store, no-cache", data);
            data = Assert.Single(response.Headers.GetValues("Vary"));
            Assert.Equal("Accept", data);
        }
    }
}