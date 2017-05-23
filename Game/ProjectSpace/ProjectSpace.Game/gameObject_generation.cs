using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Reflection.Emit;

namespace OutpostOmega.Game
{
    /// <summary>
    /// Generation methods for dynamicaly generating gameObjects
    /// </summary>
    public partial class GameObject
    {
        public static AssemblyBuilder AddonAssembly { get; set; }
        public static ModuleBuilder GameObjectModule { get; set; }

        public static Type Generate(string name, Jitter.LinearMath.JVector Position, Action<object, OpenTK.Input.MouseState, OpenTK.Input.KeyboardState, double> Test)
        {
            var builder = GetTypeBuilder(name, typeof(GameObjects.Structures.Structure));



            CreatePassThroughConstuctors(builder, typeof(GameObjects.Structures.Structure));

            AddAttribute(typeof(GameObjects.Attributes.Definition), new Type[2] { typeof(string), typeof(string) }, new object[2] { name, "Generated object" }, builder);

            AddAttribute(typeof(GameObjects.Attributes.Construction), new Type[3] { typeof(Type), typeof(Type), typeof(string) }, new object[3] { typeof(GameObjects.Structures.Frame), typeof(GameObjects.Items.Tools.Wrench), @"Content\Model\Structure\Frame.dae" }, builder);

            //builder.DefineMethod(Test.GetMethodInfo(), Test.GetMethodInfo());
            var method = builder.DefineMethod("Update",
                MethodAttributes.Public | 
                MethodAttributes.HideBySig | 
                MethodAttributes.NewSlot |
                MethodAttributes.Virtual, typeof(void), new Type[3] { typeof(OpenTK.Input.MouseState), typeof(OpenTK.Input.KeyboardState), typeof(double) });

            var generator = method.GetILGenerator();

            var testmethod = Test.GetMethodInfo();
            //var testbody = testmethod.GetMethodBody();
            //var tst = testbody.GetILAsByteArray();
            
            /*var sig = SignatureHelper.GetLocalVarSigHelper(GameObjectModule);
            foreach (var paramInfo in testmethod.GetParameters())
            {
                sig.AddArgument(paramInfo.ParameterType);                
            }*/
            //var sigArray = sig.GetSignature();

            generator.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < testmethod.GetParameters().Length-1; i++)
            {
                generator.Emit(OpCodes.Ldarg, i+1);
            }

            generator.Emit(OpCodes.Call, testmethod);
            generator.Emit(OpCodes.Ret);

            //method.SetMethodBody(tst, testbody.MaxStackSize, sigArray, null, null);

            builder.DefineMethodOverride(method, typeof(GameObjects.Structures.Structure).GetMethod("Update"));

            

            return builder.CreateType();
        }

        private static void AddAttribute(Type Attribute, Type[] Constructor, object[] Data, TypeBuilder Builder)
        {
            CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(
                Attribute.GetConstructor(Constructor), Data);

            Builder.SetCustomAttribute(caBuilder);
        }

        public static TypeBuilder GetTypeBuilder(string Name, Type parentType)
        {
            var typeSignature = Name;
            if (AddonAssembly == null)
            {
                CreateNewAssembly();
            }
            TypeBuilder tb = GameObjectModule.DefineType(typeSignature
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , parentType);
            return tb;
        }

        /// <summary>
        /// Creates a new addon assembly. Careful with that. Do only use if there is no world loaded. Otherwise major problems could occour
        /// </summary>
        public static void CreateNewAssembly()
        {
            // We just overwrite the old assembly. With the run and collect flag the old assembly should dispose on its own when all of its instances are disposed
            var an = new AssemblyName("AddonAssembly" + DateTime.Now.ToBinary().ToString());
            AddonAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndCollect);

            GameObjectModule = AddonAssembly.DefineDynamicModule("GameObjectModule");
        }

        /// <summary>Creates one constructor for each public constructor in the base class. Each constructor simply
        /// forwards its arguments to the base constructor, and matches the base constructor's signature.
        /// Supports optional values, and custom attributes on constructors and parameters.
        /// Does not support n-ary (variadic) constructors</summary>
        public static void CreatePassThroughConstuctors(TypeBuilder builder, Type baseType)
        {
            foreach (var constructor in baseType.GetConstructors()) {
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false)) {
                    //throw new InvalidOperationException("Variadic constructors are not supported");
                    continue;
                }

                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
                var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

                var ctor = builder.DefineConstructor(MethodAttributes.Public, constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
                for (var i = 0; i < parameters.Length; ++i) {
                    var parameter = parameters[i];
                    var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                    if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0) {
                        parameterBuilder.SetConstant(parameter.RawDefaultValue);
                    }

                    foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData())) {
                        parameterBuilder.SetCustomAttribute(attribute);
                    }
                }

                foreach (var attribute in BuildCustomAttributes(constructor.GetCustomAttributesData())) {
                    ctor.SetCustomAttribute(attribute);
                }

                var emitter = ctor.GetILGenerator();
                emitter.Emit(OpCodes.Nop);

                // Load `this` and call base constructor with arguments
                emitter.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parameters.Length; ++i) {
                    emitter.Emit(OpCodes.Ldarg, i);
                }
                emitter.Emit(OpCodes.Call, constructor);

                emitter.Emit(OpCodes.Ret);
            }
        }


        private static CustomAttributeBuilder[] BuildCustomAttributes(IEnumerable<CustomAttributeData> customAttributes)
        {
            return customAttributes.Select(attribute => {
                var attributeArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
                var namedPropertyInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
                var namedPropertyValues = attribute.NamedArguments.Where(a => a.MemberInfo is PropertyInfo).Select(a => a.TypedValue.Value).ToArray();
                var namedFieldInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<FieldInfo>().ToArray();
                var namedFieldValues = attribute.NamedArguments.Where(a => a.MemberInfo is FieldInfo).Select(a => a.TypedValue.Value).ToArray();
                return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
            }).ToArray();
        }
    }
}
