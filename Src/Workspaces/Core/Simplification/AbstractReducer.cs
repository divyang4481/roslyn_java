﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis.Options;

namespace Microsoft.CodeAnalysis.Simplification
{
    internal abstract partial class AbstractReducer
    {
        public abstract IExpressionRewriter CreateExpressionRewriter(OptionSet optionSet, CancellationToken cancellationToken);
    }
}
