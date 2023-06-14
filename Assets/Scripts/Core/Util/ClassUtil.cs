using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Util
{
    /// <summary>
    /// 反射等工具
    /// </summary>
    public class ClassUtil
    {
        /// <summary>
        /// 获取命名空间的所有类 @
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static List<Type> GetClasses(string nameSpace)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            List<Type> classlist = new List<Type>();
            foreach (Type type in asm.GetTypes())
            {
                if (type.Namespace == nameSpace)
                    classlist.Add(type);
            }

            return classlist;
        }

        /// <summary>
        /// 根据接口获取子类
        /// </summary>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        public static List<Type> GetClassesFromInterface(string interfaceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Type[] types = asm.GetTypes();
            List<Type> classlist = new List<Type>();
            foreach (Type type in types)
            {
                var t= type.GetInterface(interfaceName);
                // Debug.Log($"命名空间：{type.Namespace}");
                if (t!=null)
                {
                    classlist.Add(type);
                }
            }

            return classlist;
        }
    }
}