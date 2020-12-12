using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DataCollection.Utils
{
    public static class JsonHelper
    {
        public static List<T> JsonMapObjList<T>(dynamic data) where T:class,new ()
        {
            var str = JsonConvert.SerializeObject(data);
            List<T> ts = JsonConvert.DeserializeObject<List<T>>(str);
            return ts;
        }
        public static T JsonMapObj<T>(dynamic data) where T : class, new()
        {
            var str = JsonConvert.SerializeObject(data);
            T t = JsonConvert.DeserializeObject<T>(str);
            return t;
        }
        public static string SerializeObj<T>(T t) where T:class
        {
            var result = JsonConvert.SerializeObject(t);
            return result;
        }

        public static T DeserializeObj<T>(string jsonStr) where T : class,new ()
        {
            var result = JsonConvert.DeserializeObject<T>(jsonStr);
            return result;
        }
    }
}
