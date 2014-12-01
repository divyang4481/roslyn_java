﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace CSharpSyntaxGenerator
{
	[XmlRoot]
	public class Tree
	{
		[XmlAttribute]
		public string Root;

		[XmlElement(ElementName = "Node", Type = typeof(Node))]
		[XmlElement(ElementName = "AbstractNode", Type = typeof(AbstractNode))]
		[XmlElement(ElementName = "PredefinedNode", Type = typeof(PredefinedNode))]
		public List<TreeType> Types;
	}
}