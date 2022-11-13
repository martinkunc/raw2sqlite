using System.Reflection;
using ThermoFisher.CommonCore.Data.Business;

namespace raw2sqlite
{
    internal class Cloner
    {
        public static void MemberwiseClone(object target, object source)
        {
            foreach (PropertyInfo curPropInfo in source.GetType().GetProperties())
            {
                object getValue = curPropInfo.GetGetMethod().Invoke(source, new object[] { });

                if (getValue != null && curPropInfo.CanWrite)
                    curPropInfo.GetSetMethod().Invoke(target, new object[] { getValue });
            }
        }
    }
}