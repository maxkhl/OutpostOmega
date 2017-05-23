using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Reflection.Emit;
using LuaInterface;
using System.ComponentModel;
using System.Diagnostics;

namespace OutpostOmega.Game.Lua
{
    public class Builder
    {
        public static Dictionary<MethodInfo, LuaFunction> HookAllocation = new Dictionary<MethodInfo, LuaFunction>();
        public static Queue<Exception> Exceptions = new Queue<Exception>();

        public string ClassName { get; protected set; }

        private Type _ParentType { get; set; }

        private Dictionary<Type, object[]> _Attributes = new Dictionary<Type, object[]>();
        private Dictionary<MethodInfo, LuaFunction> _Hooks = new Dictionary<MethodInfo, LuaFunction>();

        public Builder(string Name, string Parent)
        {
            ClassName = Name;
            SetParent(Parent);
        }

        /// <summary>
        /// Adds a new attribute.
        /// </summary>
        public void AddAttribute(string AttributeType, LuaTable Arguments)
        {
            var args = new object[Arguments.Values.Count];
            Arguments.Values.CopyTo(args, 0);

            _Attributes.Add(FindType(AttributeType), args);
        }

        /// <summary>
        /// Sets the Parent of this builder
        /// </summary>
        [LuaHide()]
        public void SetParent(string Parent)
        {
            this._ParentType = FindType(Parent);
        }

        public void OnNewInstance(LuaInterface.LuaFunction function)
        {
            _Hooks.Add(FindMethod("Constructor", _ParentType), function);
        }

        /// <summary>
        /// Hooks the function to the specified method
        /// </summary>
        [LuaHide()]
        public void Hook(string Target, LuaInterface.LuaFunction function)
        {
            _Hooks.Add(FindMethod(Target, _ParentType), function);
        }

        /// <summary>
        /// Compiles this builder
        /// </summary>
        public Type Compile()
        {
            var builder = GetTypeBuilder(ClassName, _ParentType);

            // Compile passthrough-constructor
            CreatePassThroughConstuctors(builder, _ParentType);

            // Compile all attributes
            foreach(var attribute in _Attributes)
            {
                var Parameters = attribute.Key.GetConstructors()[0].GetParameters();
                Type[] Constructor = new Type[Parameters.Length];
                for(int i = 0; i < Parameters.Length; i++)
                {
                    Constructor[i] = Parameters[i].ParameterType;
                }
                AddAttribute(attribute.Key, Constructor, attribute.Value, builder);
            }

            // Compile all hooks
            foreach(var hook in _Hooks)
            {
                AddMethod(builder, _ParentType, hook.Key);
            }

            // Create the type
            var newType = builder.CreateType();

            // Announce all hooks
            foreach (var hook in _Hooks)
                HookAllocation.Add(newType.GetMethod(hook.Key.Name), hook.Value);

            return newType;
        }

        private static void AddMethod(TypeBuilder builder, Type baseType, MethodInfo OverrideMethod)
        {
            // Get all parameters
            var Parameters = OverrideMethod.GetParameters();
            Type[] ParameterTypes = new Type[Parameters.Length];
            for(int i = 0; i < Parameters.Length; i++)
            {
                ParameterTypes[i] = Parameters[i].ParameterType;
            }

            var method = builder.DefineMethod(OverrideMethod.Name,
                            OverrideMethod.Attributes,
                            typeof(void), ParameterTypes);

            var generator = method.GetILGenerator();


            var HookTranslatorMethod = typeof(Builder).GetMethod("HookTranslator", BindingFlags.Public | BindingFlags.Static);
            
            var blech = typeof(Builder).GetMethod("Test", BindingFlags.Public | BindingFlags.Static);
            var il = blech.GetMethodBody().GetILAsByteArray();

            generator.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < HookTranslatorMethod.GetParameters().Length - 1; i++)
            {
                if (OverrideMethod.GetParameters().Length > i)
                    generator.Emit(OpCodes.Ldarg, i + 1);
                else
                    generator.Emit(OpCodes.Ldnull);
            }

            generator.Emit(OpCodes.Call, HookTranslatorMethod);
            generator.Emit(OpCodes.Ret);

            builder.DefineMethodOverride(method, OverrideMethod);
        }
        public static void HookTranslator(object sender, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            StackFrame frame = new StackFrame(1);
            try
            {
                List<object> argList = new List<object>();
                if (sender != null)
                    argList.Add(sender);
                if (arg1 != null)
                    argList.Add(arg1);
                if (arg2 != null)
                    argList.Add(arg2);
                if (arg3 != null)
                    argList.Add(arg3);
                if (arg4 != null)
                    argList.Add(arg4);
                if (arg5 != null)
                    argList.Add(arg5);
                if (arg6 != null)
                    argList.Add(arg6);
                if (arg7 != null)
                    argList.Add(arg7);
                if (arg8 != null)
                    argList.Add(arg8);
                if (arg9 != null)
                    argList.Add(arg9);
                if (arg10 != null)
                    argList.Add(arg10);

                HookAllocation[(MethodInfo)frame.GetMethod()].Call(argList.ToArray());
            }
            catch (Exception e)
            {
                var methodInfo = (MethodInfo)frame.GetMethod();
                var luaFunction = HookAllocation[(MethodInfo)frame.GetMethod()];
                Exceptions.Enqueue(new Exception(string.Format("Could not execute hook '{0}' in '{1}' ({2}). Function reference '{3}'", methodInfo.Name, sender.ToString(), sender.GetType().Name, luaFunction.Reference), e));
            }
        }

        public static void Test()
        {
            //HookTranslator(CallerMemberNameAttribute)
        }

        #region Helper

        /// <summary>
        /// Searches for the given name of a type
        /// </summary>
        private Type FindType(string FindType)
        {
            var type = Type.GetType((string)FindType);
            if (type == null)
                throw new Exception(string.Format("Could not find type '{0}'", FindType));
            else
                return type;
        }

        /// <summary>
        /// Searches for the given name of a type
        /// </summary>
        private MethodInfo FindMethod(string MethodName, Type type)
        {
            if (type == null)
                throw new Exception(string.Format("Unable to find method '{0}'. Unable to allocate hook without a parent.", MethodName));
            else
            {
                var method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.Instance);
                if (method == null)
                    throw new Exception(string.Format("Parent '{1}' does not include method '{0}'. Unable to allocate hook.", MethodName, _ParentType.Name));
                else
                    return method;
            }
        }

        private TypeBuilder GetTypeBuilder(string Name, Type parentType)
        {
            var typeSignature = Name;
            if (GameObject.AddonAssembly == null)
            {
                var an = new AssemblyName("AddonAssembly");
                GameObject.AddonAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
                GameObject.GameObjectModule = GameObject.AddonAssembly.DefineDynamicModule("GameObjectModule");
            }
            TypeBuilder tb = GameObject.GameObjectModule.DefineType(typeSignature
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , parentType);
            return tb;
        }

        private void AddAttribute(Type Attribute, Type[] Constructor, object[] Data, TypeBuilder Builder)
        {
            CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(
                Attribute.GetConstructor(Constructor), Data);

            Builder.SetCustomAttribute(caBuilder);
        }

        /// <summary>Creates one constructor for each public constructor in the base class. Each constructor simply
        /// forwards its arguments to the base constructor, and matches the base constructor's signature.
        /// Supports optional values, and custom attributes on constructors and parameters.
        /// Does not support n-ary (variadic) constructors</summary>
        public static void CreatePassThroughConstuctors(TypeBuilder builder, Type baseType)
        {
            foreach (var constructor in baseType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
                {
                    //throw new InvalidOperationException("Variadic constructors are not supported");
                    continue;
                }

                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
                var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

                var ctor = builder.DefineConstructor(MethodAttributes.Public, constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                    if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0)
                    {
                        parameterBuilder.SetConstant(parameter.RawDefaultValue);
                    }

                    foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData()))
                    {
                        parameterBuilder.SetCustomAttribute(attribute);
                    }
                }

                foreach (var attribute in BuildCustomAttributes(constructor.GetCustomAttributesData()))
                {
                    ctor.SetCustomAttribute(attribute);
                }

                var emitter = ctor.GetILGenerator();
                emitter.Emit(OpCodes.Nop);

                // Load `this` and call base constructor with arguments
                emitter.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parameters.Length; ++i)
                {
                    emitter.Emit(OpCodes.Ldarg, i);
                }
                emitter.Emit(OpCodes.Call, constructor);

                emitter.Emit(OpCodes.Ret);
            }
        }

        private static CustomAttributeBuilder[] BuildCustomAttributes(IEnumerable<CustomAttributeData> customAttributes)
        {
            return customAttributes.Select(attribute =>
            {
                var attributeArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
                var namedPropertyInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
                var namedPropertyValues = attribute.NamedArguments.Where(a => a.MemberInfo is PropertyInfo).Select(a => a.TypedValue.Value).ToArray();
                var namedFieldInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<FieldInfo>().ToArray();
                var namedFieldValues = attribute.NamedArguments.Where(a => a.MemberInfo is FieldInfo).Select(a => a.TypedValue.Value).ToArray();
                return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
            }).ToArray();
        }

        #endregion
    }
}
