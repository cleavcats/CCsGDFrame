using System;
using System.Reflection;
using System.Collections.Generic;

namespace Cleavcats
{
    public static class TypeSearch
    {
        /// <summary>
        /// // 找到所有 继承自 target 的类
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static List<Type> FindAllChildType(Type parent)
        {
            List<Type> returns = new List<Type>();
            Assembly[] all_assembly = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly_now in all_assembly)
            {
                Type[] all_type = assembly_now.GetTypes();
                foreach (Type founded in all_type)
                {
                    if (founded.Name == parent.Name) continue;
                    if (parent.IsAssignableFrom(founded) == false) continue;
                    returns.Add(founded);
                }
            }
            return returns;
        }
        /// <summary>
        /// // 在指定类型的嵌套类型中，寻找继承自 founded_type 的嵌套类
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="founded_type"></param>
        /// <returns></returns>
        public static List<Type> FindAllChildType(Type parent, Type founded_type)
        {
            List<Type> returns = new List<Type>();
            Type[] list;
            list = parent.GetNestedTypes(BindingFlags.Public);
            foreach (Type founded in list)
            {
                if (founded_type.IsAssignableFrom(founded) == false) continue;
                returns.Add(founded);
            }
            return returns;
        }
        /// <summary>
        /// // 从指定类型的 变量中寻找，找到所有继承自 founded_type 的 变量（仅限 public ）
        /// <br>// 如果 obj == null ，则表明 寻找的是静态变量</br>
        /// </summary>
        public static List<FieldInfo> FindAllChildField(Type parent, object obj, Type founded_type)
        {
            List<FieldInfo> returns = new List<FieldInfo>();
            FieldInfo[] fields;
            if (obj == null) fields = parent.GetFields(BindingFlags.Static | BindingFlags.Public);
            else fields = parent.GetFields(BindingFlags.Public);
            foreach (FieldInfo founded in fields)
            {
                if (founded_type.IsAssignableFrom(founded.FieldType) == false) continue;
                returns.Add(founded);
            }
            return returns;
        }
    }
}