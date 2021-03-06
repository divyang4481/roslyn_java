﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
	internal struct SeparatedSyntaxListBuilder<TNode> where TNode : CSharpSyntaxNode
	{
		private readonly SyntaxListBuilder builder;

		public SeparatedSyntaxListBuilder(int size)
			: this(new SyntaxListBuilder(size))
		{
		}

		public static SeparatedSyntaxListBuilder<TNode> Create()
		{
			return new SeparatedSyntaxListBuilder<TNode>(8);
		}

		internal SeparatedSyntaxListBuilder(SyntaxListBuilder builder)
		{
			this.builder = builder;
		}

		public bool IsNull
		{
			get
			{
				return this.builder == null;
			}
		}

		public int Count
		{
			get
			{
				return this.builder.Count;
			}
		}

		public CSharpSyntaxNode this[int index]
		{
			get
			{
				return this.builder[index];
			}

			set
			{
				this.builder[index] = value;
			}
		}

		public void Clear()
		{
			this.builder.Clear();
		}

		public SeparatedSyntaxListBuilder<TNode> Add(TNode node)
		{
			this.builder.Add(node);
			return this;
		}

		public void AddSeparator(SyntaxToken separatorToken)
		{
			this.builder.Add(separatorToken);
		}

		public void AddRange(TNode[] items, int offset, int length)
		{
			this.builder.AddRange(items, offset, length);
		}

		public void AddRange(SeparatedSyntaxList<TNode> nodes)
		{
			this.builder.AddRange(nodes.GetWithSeparators());
		}

		public bool Any(SyntaxKind kind)
		{
			return this.builder.Any(kind);
		}

		public SeparatedSyntaxList<TNode> ToList()
		{
			return builder == null
				? default(SeparatedSyntaxList<TNode>)
				: new SeparatedSyntaxList<TNode>(new SyntaxList<CSharpSyntaxNode>(this.builder.ToListNode()));
		}

		/// <summary>
		/// WARN WARN WARN: This should be used with extreme caution - the underlying builder does
		/// not give any indication that it is from a separated syntax list but the constraints
		/// (node, token, node, token, ...) should still be maintained.
		/// </summary>
		/// <remarks>
		/// In order to avoid creating a separate pool of SeparatedSyntaxListBuilders, we expose
		/// our underlying SyntaxListBuilder to SyntaxListPool.
		/// </remarks>
		internal SyntaxListBuilder UnderlyingBuilder
		{
			get { return builder; }
		}

		public static implicit operator SeparatedSyntaxList<TNode>(SeparatedSyntaxListBuilder<TNode> builder)
		{
			return builder.ToList();
		}
	}
}