﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading;

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
	internal partial class SyntaxList
	{
		internal class WithManyWeakChildren : SyntaxList
		{
			private readonly ArrayElement<WeakReference<SyntaxNode>>[] children;

			// We calculate and store the positions of all children here. This way, getting the position
			// of all children is O(N) [N being the list size], otherwise it is O(N^2) because getting
			// the position of a child later requires traversing all previous siblings.
			private readonly int[] childPositions;

			internal WithManyWeakChildren(Syntax.InternalSyntax.SyntaxList.WithManyChildrenBase green, SyntaxNode parent, int position)
				: base(green, parent, position)
			{
				int count = green.SlotCount;
				this.children = new ArrayElement<WeakReference<SyntaxNode>>[count];

				var childOffsets = new int[count];

				int childPosition = position;
				var greenChildren = green.children;
				for (int i = 0; i < childOffsets.Length; ++i)
				{
					childOffsets[i] = childPosition;
					childPosition += greenChildren[i].Value.FullWidth;
				}

				this.childPositions = childOffsets;
			}

			internal override int GetChildPosition(int index)
			{
				return childPositions[index];
			}

			internal override SyntaxNode GetNodeSlot(int index)
			{
				return GetWeakRedElement(ref this.children[index].Value, index);
			}

			internal override SyntaxNode GetCachedSlot(int index)
			{
				SyntaxNode value = null;

				var weak = this.children[index].Value;
				if (weak != null)
				{
					weak.TryGetTarget(out value);
				}

				return null;
			}

			public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
			{
				throw new NotImplementedException();
			}

			public override void Accept(CSharpSyntaxVisitor visitor)
			{
				throw new NotImplementedException();
			}
		}
	}
}