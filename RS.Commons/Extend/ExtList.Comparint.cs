﻿using System.Collections.Generic;
using System.Linq;

namespace RS.Commons.Extend
{
    /// <summary>
    /// 详细用法请参看实例  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtList<T> : IEqualityComparer<T> where T : class, new()
    {
        private string[] comparintFiledName = { };

        public ExtList() { }
        public ExtList(params string[] comparintFiledName)
        {
            this.comparintFiledName = comparintFiledName;
        }
        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            if (x == null && y == null)
            {
                return false;
            }
            if (comparintFiledName.Length == 0)
            {
                return x.Equals(y);
            }
            bool result = true;
            var typeX = x.GetType();//获取类型
            var typeY = y.GetType();
            foreach (var filedName in comparintFiledName)
            {
                var xPropertyInfo = (from p in typeX.GetProperties() where p.Name.Equals(filedName) select p).FirstOrDefault();
                var yPropertyInfo = (from p in typeY.GetProperties() where p.Name.Equals(filedName) select p).FirstOrDefault();

                result = result
                    && xPropertyInfo != null && yPropertyInfo != null
                    && xPropertyInfo.GetValue(x, null).ToString().Equals(yPropertyInfo.GetValue(y, null));
            }
            return result;
        }
        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
