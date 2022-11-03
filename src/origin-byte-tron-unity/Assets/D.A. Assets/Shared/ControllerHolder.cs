using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using UnityEngine;

namespace DA_Assets.FCU.Core
{
    public class ControllerHolder<T1> where T1 : MonoBehaviour
    {
        public T1 controller;
        public void SetValue<T>(ref T currentValue, T newValue)
        {
            if (typeof(T) == typeof(string))
            {
                if (currentValue == null)
                {
                    currentValue = (T)Convert.ChangeType(string.Empty, typeof(T));
                }
            }
            else if (currentValue == null)
            {
                currentValue = (T)Activator.CreateInstance(typeof(T));
            }

            if (currentValue.Equals(newValue) == false)
            {
                controller.SetDirty();
            }

            currentValue = newValue;
        }
    }
    public static class ControllerHolderExtensions
    {
        public static T SetController<T, T1>(this T type, T1 fcu) where T1 : MonoBehaviour where T : ControllerHolder<T1>
        {
            if (type == null)
            {
                type = (T)Activator.CreateInstance(typeof(T));
            }

            if (type.controller == null)
            {
                type.controller = fcu;
            }

            return type;
        }
    }
}