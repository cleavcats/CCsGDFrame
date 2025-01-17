using System.Reflection;
using System;

namespace Cleavcats
{
    public static class MethodCall
    {
        /// <summary>
        /// // 简易 invoke ，支持动态参数列表
        /// </summary>
        /// <param name="func"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object Call(object src_obj, MethodInfo func, params object[] args)
        {
            //try { return func.Invoke(null, args); } catch { throw new Exception("方法执行中出现错误"); }
            if (args == null) args = new string[0];
            ParameterInfo[] args_type = func.GetParameters();
            // 抄写参数
            object[] args_obj = new object[args_type.Length];
            // 最后一个参数是否为数组（可能得动态参数列表）
            bool is_动态参数列表;
            if (args_obj.Length == 0) is_动态参数列表 = false;
            else is_动态参数列表 = typeof(Array).IsAssignableFrom(args_type[args_type.Length - 1].ParameterType);
            int 抄写长度 = args.Length;
            if (is_动态参数列表) if (抄写长度 >= args_type.Length) 抄写长度 = args_type.Length - 1;// 如果是动态参数列表，则最后一个参数位置
            for (int i = 0; i < 抄写长度; i++/*抄写传入的参数*/)
            {
                args_obj[i] = args[i];
            }
            if (args.Length < args_type.Length/*参数不足，使用默认值补充*/)
            {
                if (args_type[args.Length].HasDefaultValue == false) { if (args.Length + 1 != args_type.Length) throw new Exception("参数不足"); }
                for (int i = args.Length; i < args_type.Length; i++) { args_obj[i] = args_type[i].DefaultValue; }
                if (is_动态参数列表) { args_obj[args_type.Length - 1] = Activator.CreateInstance(args_type[args_type.Length - 1].ParameterType, new object[1] { 0 }); }
                try { return func.Invoke(src_obj, args_obj); } catch { throw new Exception("方法执行中出现错误"); }
            }
            if (is_动态参数列表)
            {
                int len = args.Length - args_type.Length + 1;
                args_obj[args_type.Length - 1] = Activator.CreateInstance(args_type[args_type.Length - 1].ParameterType, new object[1] { len });
                Type 数组成员类型 = args_type[args_type.Length - 1].ParameterType.GetElementType();
                for (int i = 0; i < len; i++)
                {
                    ((Array)args_obj[args_type.Length - 1]).SetValue(args[args_type.Length - 1 + i], i);
                }
                try { return func.Invoke(src_obj, args_obj); } catch { throw new Exception("方法执行中出现错误"); }
            }
            else/*参数太多则抛弃*/ { try { return func.Invoke(src_obj, args_obj); } catch { throw new Exception("方法执行中出现错误"); } }
        }
        static object Inner_将字符串转化为指定类型(Type type, string str)
        {
            if (str == "null") return null;
            else if (str == "NULL") return null;
            else if (str == "Null") return null;
            else if (typeof(bool).IsAssignableFrom(type)) { return Convert.ToBoolean(str); }
            else if (typeof(short).IsAssignableFrom(type)) { return Convert.ToInt16(str); }
            else if (typeof(ushort).IsAssignableFrom(type)) { return Convert.ToUInt16(str); }
            else if (typeof(int).IsAssignableFrom(type)) { return Convert.ToInt32(str); }
            else if (typeof(uint).IsAssignableFrom(type)) { return Convert.ToUInt32(str); }
            else if (typeof(long).IsAssignableFrom(type)) { return Convert.ToInt64(str); }
            else if (typeof(ulong).IsAssignableFrom(type)) { return Convert.ToUInt64(str); }
            else if (typeof(double).IsAssignableFrom(type)) { return Convert.ToDouble(str); }
            else if (typeof(float).IsAssignableFrom(type)) { return Convert.ToSingle(str); }
            else if (typeof(string).IsAssignableFrom(type)) { return Convert.ToString(str); }
            else { throw new NullReferenceException(); }
        }
        /// <summary>
        /// // Invoke 指定的 func，自动将参数从 string 转换到正确格式（基本类型），支持动态参数列表
        /// </summary>
        public static object CallByString(object src_obj, MethodInfo func, params string[] args)
        {
            if (args == null) args = new string[0];
            ParameterInfo[] args_type = func.GetParameters();
            // 抄写参数
            object[] args_obj = new object[args_type.Length];
            // 最后一个参数是否为数组（可能得动态参数列表）
            bool is_动态参数列表;
            if (args_obj.Length == 0) is_动态参数列表 = false;
            else is_动态参数列表 = typeof(Array).IsAssignableFrom(args_type[args_type.Length - 1].ParameterType);
            int 抄写长度 = args.Length;
            if (is_动态参数列表) if (抄写长度 >= args_type.Length) 抄写长度 = args_type.Length - 1;// 如果是动态参数列表，则最后一个参数位置
            for (int i = 0; i < 抄写长度; i++/*抄写传入的参数*/)
            {
                try { args_obj[i] = Inner_将字符串转化为指定类型(args_type[i].ParameterType, args[i]); }
                catch (NullReferenceException) { throw new Exception("指令存在不支持的参数类型（仅限基本数据类型），不能通过命令行调用，使用对应的命令行版本指令可能解决这个问题"); }
                catch (FormatException) { throw new Exception("第" + (i + 1) + "个参数的类型错误"); }
            }
            if (args.Length < args_type.Length/*参数不足，使用默认值补充*/)
            {
                if (args_type[args.Length].HasDefaultValue == false) { if (args.Length + 1 != args_type.Length) throw new Exception("参数不足"); }
                for (int i = args.Length; i < args_type.Length; i++) { args_obj[i] = args_type[i].DefaultValue; }
                if (is_动态参数列表) { args_obj[args_type.Length - 1] = Activator.CreateInstance(args_type[args_type.Length - 1].ParameterType, new object[1] { 0 }); }
                try { return func.Invoke(src_obj, args_obj); } catch { throw new Exception("方法执行中出现错误"); }
            }
            if (is_动态参数列表)
            {
                int len = args.Length - args_type.Length + 1;
                args_obj[args_type.Length - 1] = Activator.CreateInstance(args_type[args_type.Length - 1].ParameterType, new object[1] { len });
                Type 数组成员类型 = args_type[args_type.Length - 1].ParameterType.GetElementType();
                for (int i = 0; i < len; i++)
                {
                    try { ((Array)args_obj[args_type.Length - 1]).SetValue(Inner_将字符串转化为指定类型(数组成员类型, args[args_type.Length - 1 + i]), i); }
                    catch (NullReferenceException) { throw new Exception("指令存在不支持的参数类型（仅限基本数据类型），不能通过命令行调用，使用对应的命令行版本指令可能解决这个问题"); }
                    catch (FormatException) { throw new Exception("第" + (args.Length - 1) + "个参数的类型错误"); }
                }
                try { return func.Invoke(src_obj, args_obj); } catch { throw new Exception("方法执行中出现错误"); }
            }
            else/*参数太多则抛弃*/ { try { return func.Invoke(src_obj, args_obj); } catch { throw new Exception("方法执行中出现错误"); } }
        }
    }
}