using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyExtension
{
    /// <summary>
    /// 複製物件
    /// </summary>
    /// <returns></returns>
    public static class DeepClone
    {
        /// <summary>
        /// 複製物件 泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCloneByMemoryStream<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// 複製物件 泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objSource">複製的物件</param>
        /// <param name="objTarget">新物件目標</param>
        /// <returns>T objTarget</returns>
        public static T DeepCloneByReflection<T>(this T objSource, T objTarget)
        {
            //Get the type of source object and create a new instance of that type
            Type typeSource = objSource.GetType();
            //T objTarget = (T)Activator.CreateInstance(typeof(T));
            //Get all the properties of source object 
            PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //Assign all source property to taget object 's properties
            foreach (PropertyInfo property in propertyInfo)
            {
                //Check whether property can be written to
                if (property.CanWrite)
                {
                    //check whether property type is value type, enum or string type
                    if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
                    {
                        property.SetValue(objTarget, property.GetValue(objSource, null), null);
                    }
                    //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                    else
                    {
                        object objPropertyValue = property.GetValue(objSource, null);
                        if (objPropertyValue == null)
                        {
                            property.SetValue(objTarget, null, null);
                        }
                        else
                        {
                            property.SetValue(objTarget, objPropertyValue.DeepCloneByReflection(objTarget), null);
                        }
                    }
                }
            }

            return objTarget;
        }
    }
}
