﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection.Metadata;

using ILCompiler.DependencyAnalysisFramework;

using Internal.TypeSystem;
using Internal.TypeSystem.Ecma;

namespace ILTrim.DependencyAnalysis
{
    /// <summary>
    /// Represents a type that is considered allocated at runtime (e.g. with a "new").
    /// </summary>
    public class ConstructedTypeNode : DependencyNodeCore<NodeFactory>
    {
        private readonly EcmaType _type;

        public ConstructedTypeNode(EcmaType type)
        {
            _type = type;
        }

        public override bool HasConditionalStaticDependencies
        {
            get
            {
                // If there's any virtual method, we have conditional dependencies.
                foreach (MethodDesc method in _type.GetAllVirtualMethods())
                {
                    return true;
                }

                // Even if a type has no virtual methods, if it introduces a new interface,
                // we might end up with with conditional dependencies.
                //
                // Consider:
                //
                // interface IFooer { void Foo(); }
                // class Base { public virtual void Foo() { } }
                // class Derived : Base, IFooer { }
                //
                // Notice Derived has no virtual methods, but needs to keep track of the
                // fact that Base.Foo now implements IFooer.Foo.
                if (_type.RuntimeInterfaces.Length > 0)
                {
                    return true;
                }

                // TODO: limit this somehow. We don't know if there's any flow annotations
                return !_type.IsInterface;
            }
        }

        public override IEnumerable<CombinedDependencyListEntry> GetConditionalStaticDependencies(NodeFactory factory)
        {
            // For each virtual method slot (e.g. Object.GetHashCode()), check whether the current type
            // provides an implementation of the virtual method (e.g. SomeFoo.GetHashCode()),
            // if so, make sure we generate the body.
            if (factory.IsModuleTrimmed(_type.EcmaModule))
            {
                foreach (MethodDesc decl in _type.EnumAllVirtualSlots())
                {
                    MethodDesc impl = _type.FindVirtualFunctionTargetMethodOnObjectType(decl);

                    // We're only interested in the case when it's implemented on this type.
                    // If the implementation comes from a base type, that's covered by the base type
                    // ConstructedTypeNode.
                    if (impl.OwningType == _type)
                    {
                        // If the slot defining virtual method is used, make sure we generate the implementation method.
                        var ecmaImpl = (EcmaMethod)impl.GetTypicalMethodDefinition();

                        EcmaMethod declDefinition = (EcmaMethod)decl.GetTypicalMethodDefinition();
                        VirtualMethodUseNode declUse = factory.VirtualMethodUse(declDefinition);

                        yield return new(
                            factory.MethodDefinition(ecmaImpl.Module, ecmaImpl.Handle),
                            declUse,
                            "Virtual method");

                        var implHandle = TryGetMethodImplementationHandle(_type, declDefinition);
                        if (!implHandle.IsNil)
                        {
                            yield return new(
                                factory.MethodImplementation(_type.EcmaModule, implHandle),
                                declUse,
                                "Explicitly implemented virtual method");
                        }
                    }
                }
            }

            // For each interface, figure out what implements the individual interface methods on it.
            foreach (DefType intface in _type.RuntimeInterfaces)
            {
                foreach (MethodDesc interfaceMethod in intface.GetAllVirtualMethods())
                {
                    // TODO: static virtual methods (not in the type system yet)
                    if (interfaceMethod.Signature.IsStatic)
                        continue;

                    MethodDesc implMethod = _type.ResolveInterfaceMethodToVirtualMethodOnType(interfaceMethod);
                    if (implMethod != null)
                    {
                        var interfaceMethodDefinition = (EcmaMethod)interfaceMethod.GetTypicalMethodDefinition();
                        VirtualMethodUseNode interfaceMethodUse = factory.VirtualMethodUse(interfaceMethodDefinition);

                        // Interface method implementation provided within the class hierarchy.
                        yield return new(factory.VirtualMethodUse((EcmaMethod)implMethod.GetTypicalMethodDefinition()),
                            interfaceMethodUse,
                            "Interface method");

                        if (factory.IsModuleTrimmed(_type.EcmaModule))
                        {
                            MethodImplementationHandle implHandle = TryGetMethodImplementationHandle(_type, interfaceMethodDefinition);
                            if (!implHandle.IsNil)
                            {
                                yield return new(factory.MethodImplementation(_type.EcmaModule, implHandle),
                                    interfaceMethodUse,
                                    "Explicitly implemented interface method");
                            }
                        }
                    }
                    else
                    {
                        // Is the implementation provided by a default interface method?
                        var resolution = _type.ResolveInterfaceMethodToDefaultImplementationOnType(interfaceMethod, out implMethod);
                        if (resolution == DefaultInterfaceMethodResolution.DefaultImplementation || resolution == DefaultInterfaceMethodResolution.Reabstraction)
                        {
                            yield return new(factory.VirtualMethodUse((EcmaMethod)implMethod.GetTypicalMethodDefinition()),
                                factory.VirtualMethodUse((EcmaMethod)interfaceMethod.GetTypicalMethodDefinition()),
                                "Default interface method");
                        }
                        else
                        {
                            // TODO: if there's a diamond, we should consider both implementations used
                        }
                    }
                }
            }

            // For each interface, make the interface considered constructed if the interface is used
            foreach (DefType intface in _type.RuntimeInterfaces)
            {
                EcmaType interfaceDefinition = (EcmaType)intface.GetTypeDefinition();
                yield return new(factory.ConstructedType(interfaceDefinition),
                    factory.InterfaceUse(interfaceDefinition),
                    "Used interface on a constructed type");
            }

            // Check to see if we have any dataflow annotations on the type.
            // The check below also covers flow annotations inherited through base classes and implemented interfaces.
            if (!_type.IsInterface /* "IFoo x; x.GetType();" -> this doesn't actually return an interface type */
                && factory.FlowAnnotations.GetTypeAnnotation(_type) != default)
            {
                // We have some flow annotations on this type.
                //
                // The flow annotations are supposed to ensure that should we call object.GetType on a location
                // typed as one of the annotated subclasses of this type, this type is going to have the specified
                // members kept. We don't keep them right away, but condition them on the object.GetType being called.
                //
                // Now we figure out where the annotations are coming from:

                DefType baseType = _type.BaseType;
                if (baseType != null && factory.FlowAnnotations.GetTypeAnnotation(baseType) != default)
                {
                    // There's an annotation on the base type. If object.GetType was called on something
                    // statically typed as the base type, we might actually be calling it on this type.
                    // Ensure we have the flow dependencies.
                    yield return new(
                        factory.ObjectGetTypeFlowDependencies(_type),
                        factory.ObjectGetTypeFlowDependencies((EcmaType)baseType.GetTypeDefinition()),
                        "GetType called on the base type");

                    // We don't have to follow all the bases since the base MethodTable will bubble this up
                }

                foreach (DefType interfaceType in _type.RuntimeInterfaces)
                {
                    if (factory.FlowAnnotations.GetTypeAnnotation(interfaceType) != default)
                    {
                        // There's an annotation on the interface type. If object.GetType was called on something
                        // statically typed as the interface type, we might actually be calling it on this type.
                        // Ensure we have the flow dependencies.
                        yield return new(
                            factory.ObjectGetTypeFlowDependencies(_type),
                            factory.ObjectGetTypeFlowDependencies((EcmaType)interfaceType.GetTypeDefinition()),
                            "GetType called on the interface");
                    }

                    // We don't have to recurse into the interface because we're inspecting runtime interfaces
                    // and this list is already flattened.
                }

                // Note we don't add any conditional dependencies if this type itself was annotated and none
                // of the bases/interfaces are annotated.
                // ObjectGetTypeFlowDependencies don't need to be conditional in that case. They'll be added as needed.
            }
        }

        public override IEnumerable<DependencyListEntry> GetStaticDependencies(NodeFactory factory)
        {
            // Call GetTypeDefinition in case the base is an instantiated generic type.
            TypeDesc baseType = _type.BaseType?.GetTypeDefinition();
            if (baseType != null)
            {
                yield return new(factory.ConstructedType((EcmaType)baseType), "Base type");
            }
        }

        protected override string GetName(NodeFactory factory)
        {
            return $"{_type} constructed";
        }

        public override bool InterestingForDynamicDependencyAnalysis => false;
        public override bool HasDynamicDependencies => false;
        public override bool StaticDependenciesAreComputed => true;
        public override IEnumerable<CombinedDependencyListEntry> SearchDynamicDependencies(List<DependencyNodeCore<NodeFactory>> markedNodes, int firstNode, NodeFactory factory) => null;

        private static MethodImplementationHandle TryGetMethodImplementationHandle(EcmaType implementingType, EcmaMethod declMethod)
        {
            MetadataReader reader = implementingType.MetadataReader;

            foreach (MethodImplementationHandle implRecordHandle in reader.GetTypeDefinition(implementingType.Handle).GetMethodImplementations())
            {
                MethodImplementation implRecord = reader.GetMethodImplementation(implRecordHandle);
                if (implementingType.EcmaModule.TryGetMethod(implRecord.MethodDeclaration) == declMethod)
                {
                    return implRecordHandle;
                }
            }

            return default;
        }
    }
}
