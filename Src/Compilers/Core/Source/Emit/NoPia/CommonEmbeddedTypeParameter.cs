﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Roslyn.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Cci = Microsoft.Cci;

namespace Microsoft.CodeAnalysis.Emit.NoPia
{
    internal abstract partial class EmbeddedTypesManager<
        TPEModuleBuilder,
        TEmbeddedTypesManager,
        TSyntaxNode,
        TAttributeData,
        TSymbol,
        TAssemblySymbol,
        TNamedTypeSymbol,
        TFieldSymbol,
        TMethodSymbol,
        TEventSymbol,
        TPropertySymbol,
        TParameterSymbol,
        TTypeParameterSymbol,
        TEmbeddedType,
        TEmbeddedField,
        TEmbeddedMethod,
        TEmbeddedEvent,
        TEmbeddedProperty,
        TEmbeddedParameter,
        TEmbeddedTypeParameter>
    {
        internal abstract class CommonEmbeddedTypeParameter : Cci.IGenericMethodParameter
        {
            public readonly TEmbeddedMethod ContainingMethod;
            public readonly TTypeParameterSymbol UnderlyingTypeParameter;

            protected CommonEmbeddedTypeParameter(TEmbeddedMethod containingMethod, TTypeParameterSymbol underlyingTypeParameter)
            {
                this.ContainingMethod = containingMethod;
                this.UnderlyingTypeParameter = underlyingTypeParameter;
            }

            protected abstract IEnumerable<Cci.ITypeReference> GetConstraints(Context context);
            protected abstract bool MustBeReferenceType { get; }
            protected abstract bool MustBeValueType { get; }
            protected abstract bool MustHaveDefaultConstructor { get; }
            protected abstract string Name { get; }
            protected abstract ushort Index { get; }

            Cci.IMethodDefinition Cci.IGenericMethodParameter.DefiningMethod
            {
                get
                {
                    return ContainingMethod;
                }
            }

            IEnumerable<Cci.ITypeReference> Cci.IGenericParameter.GetConstraints(Context context)
            {
                return GetConstraints(context);
            }

            bool Cci.IGenericParameter.MustBeReferenceType
            {
                get
                {
                    return MustBeReferenceType;
                }
            }

            bool Cci.IGenericParameter.MustBeValueType
            {
                get
                {
                    return MustBeValueType;
                }
            }

            bool Cci.IGenericParameter.MustHaveDefaultConstructor
            {
                get
                {
                    return MustHaveDefaultConstructor;
                }
            }

            Cci.TypeParameterVariance Cci.IGenericParameter.Variance
            {
                get
                {
                    // Method type parameters are not variant
                    return Cci.TypeParameterVariance.NonVariant;
                }
            }

            Cci.IGenericMethodParameter Cci.IGenericParameter.AsGenericMethodParameter
            {
                get
                {
                    return this;
                }
            }

            Cci.IGenericTypeParameter Cci.IGenericParameter.AsGenericTypeParameter
            {
                get
                {
                    return null;
                }
            }

            bool Cci.ITypeReference.IsEnum
            {
                get { return false; }
            }

            bool Cci.ITypeReference.IsValueType
            {
                get { return false; }
            }

            Cci.ITypeDefinition Cci.ITypeReference.GetResolvedType(Context context)
            {
                return null;
            }

            Cci.PrimitiveTypeCode Cci.ITypeReference.TypeCode(Context context)
            {
                return Cci.PrimitiveTypeCode.NotPrimitive;
            }

            TypeHandle Cci.ITypeReference.TypeDef
            {
                get { return default(TypeHandle); }
            }

            Cci.IGenericMethodParameterReference Cci.ITypeReference.AsGenericMethodParameterReference
            {
                get { return this; }
            }

            Cci.IGenericTypeInstanceReference Cci.ITypeReference.AsGenericTypeInstanceReference
            {
                get { return null; }
            }

            Cci.IGenericTypeParameterReference Cci.ITypeReference.AsGenericTypeParameterReference
            {
                get { return null; }
            }

            Cci.INamespaceTypeDefinition Cci.ITypeReference.AsNamespaceTypeDefinition(Context context)
            {
                return null;
            }

            Cci.INamespaceTypeReference Cci.ITypeReference.AsNamespaceTypeReference
            {
                get { return null; }
            }

            Cci.INestedTypeDefinition Cci.ITypeReference.AsNestedTypeDefinition(Context context)
            {
                return null;
            }

            Cci.INestedTypeReference Cci.ITypeReference.AsNestedTypeReference
            {
                get { return null; }
            }

            Cci.ISpecializedNestedTypeReference Cci.ITypeReference.AsSpecializedNestedTypeReference
            {
                get { return null; }
            }

            Cci.ITypeDefinition Cci.ITypeReference.AsTypeDefinition(Context context)
            {
                return null;
            }

            IEnumerable<Cci.ICustomAttribute> Cci.IReference.GetAttributes(Context context)
            {
                // TODO:
                return SpecializedCollections.EmptyEnumerable<Cci.ICustomAttribute>();
            }

            void Cci.IReference.Dispatch(Cci.MetadataVisitor visitor)
            {
                throw ExceptionUtilities.Unreachable;
            }

            Cci.IDefinition Cci.IReference.AsDefinition(Context context)
            {
                return null;
            }

            string Cci.INamedEntity.Name
            {
                get { return Name; }
            }

            ushort Cci.IParameterListEntry.Index
            {
                get
                {
                    return Index;
                }
            }

            Cci.IMethodReference Cci.IGenericMethodParameterReference.DefiningMethod
            {
                get { return ContainingMethod; }
            }
        }
    }
}