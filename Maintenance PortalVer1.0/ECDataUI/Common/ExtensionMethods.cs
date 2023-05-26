using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Text;
using System.Web.Mvc;

namespace ECDataUI.Common
{
    public static class ExtensionMethods
    {


        public static object CopyObject(this object sourceObj, object destObj)
        {


            //1)  check if any object null throw nullargumentexception

            PropertyInfo[] destObjProperties = destObj.GetType().GetProperties();
            PropertyInfo[] sourceObjProperties = sourceObj.GetType().GetProperties();

            var typeOfSourceObj = sourceObj.GetType();
            var typeOfDestObj = destObj.GetType();

            foreach (var fieldOfA in typeOfSourceObj.GetFields())
            {
                var fieldOfDestObj = typeOfDestObj.GetField(fieldOfA.Name);
                fieldOfDestObj.SetValue(destObj, fieldOfA.GetValue(sourceObj));
            }
            List<string> destObjPropNamesList = new List<string>();

            foreach (var item in destObjProperties)
            {
                destObjPropNamesList.Add(item.Name);
            }
            foreach (var propertyOfSourceObj in sourceObjProperties)
            {

                if (destObjPropNamesList.Contains(propertyOfSourceObj.Name))
                {
                    #region For nested class.incomplete code
                    //if (propertyOfSourceObj.GetType().IsClass)
                    //{

                    //    object returned = propertyOfSourceObj.CopyObject(Activator.CreateInstance(propertyOfSourceObj.GetType()));

                    //    var propertyOfB = typeOfDestObj.GetProperty(propertyOfSourceObj.Name);

                    //    propertyOfB.SetValue(destObj, propertyOfSourceObj.GetValue(returned));
                    //}
                    //else
                    //{
                    //    var propertyOfB = typeOfDestObj.GetProperty(propertyOfSourceObj.Name);

                    //    propertyOfB.SetValue(destObj, propertyOfSourceObj.GetValue(sourceObj));
                    //} 
                    #endregion
                    var propertyOfB = typeOfDestObj.GetProperty(propertyOfSourceObj.Name);

                    propertyOfB.SetValue(destObj, propertyOfSourceObj.GetValue(sourceObj));

                }
            }


            return destObj;

            //if (sourceObjProperties.Length != destObjProperties.Length)
            //{

            //    throw new Exception("Both models should have same number of properties");
            //}




        }



        public static string FormatErrorMessage(this ModelStateDictionary modelstate)
        {
            StringBuilder strErrorMessage = new StringBuilder();
            int count = 0;
            foreach (var modelStateValue in modelstate.Values)
            {
                foreach (var error in modelStateValue.Errors)
                {
                    if (count == 0)
                    {
                        strErrorMessage.Append("<ul  style='list-style-type:none'>");
                    }
                    count++;
                    strErrorMessage.Append("<li>");
                    strErrorMessage.Append(error.ErrorMessage);
                    strErrorMessage.Append("</li>");
                }
            }
            strErrorMessage.Append("</ul>");
            return strErrorMessage.ToString();
        }


        public static string FormatErrorMessageInString(this ModelStateDictionary modelstate)
        {
            String strErrorMessage = string.Empty;
        //    int count = 0;
            foreach (var modelStateValue in modelstate.Values)
            {
                foreach (var error in modelStateValue.Errors)
                {
                //    if (count == 0)
                //    {
                //        strErrorMessage.Append("<ul  style='list-style-type:none'>");
                //    }
                //    count++;
                //    strErrorMessage.Append("<li>");
                     strErrorMessage = error.ErrorMessage;
                  //  strErrorMessage.Append("</li>");
                }
            }
          //  strErrorMessage.Append("</ul>");
            return strErrorMessage;
        }


        /// <summary>Trim all String properties of the given object.
        /// it does not trim nested object. 
        /// Call explictly for nested object.
        /// </summary>
        public static TSelf TrimAllStringProperties<TSelf>(this TSelf input)
        {
            var stringProperties = input.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            foreach (var stringProperty in stringProperties)
            {
                string currentValue = (string)stringProperty.GetValue(input, null);
                if (currentValue != null)
                    stringProperty.SetValue(input, currentValue.Trim(), null);
            }
            return input;
        }

    }
}