using System;

namespace Rotorsoft.Maui
{
    internal struct SortPropertyInfo
    {
        public string PropertyName;
        public bool IsDescending;

        public object GetValue(object o)
        {
            if (string.IsNullOrWhiteSpace(PropertyName))
            {
                return o;
            }

            string[] propertyParts = PropertyName.Split('.');

            object currentObject = o;
            foreach (string propertyPart in propertyParts)
            {
                currentObject = currentObject?.GetType().GetProperty(propertyPart)?.GetValue(currentObject);
            }

            return currentObject;
        }
    }
}
