﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Middleware;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.Integration.AspNet.Core
{
    public interface IBotConfigurationBuilder
    {
        List<IMiddleware> Middleware { get; }
    }
}
