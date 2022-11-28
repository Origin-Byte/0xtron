using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System.Collections.Generic;

namespace DA_Assets.FCU.Extensions
{
    public static class ImageExtensions 
    {
        public static bool IsFilled(this FObject fobject)
        {
            if (fobject.ArcData.Equals(default(ArcData)))
            {
                return false;
            }

            return fobject.ArcData.EndingAngle < 6.28f;
        }
        public static bool ContainsLinearGradients(this List<Fill> fills)
        {
            if (fills.IsEmpty())
            {
                return false;
            }

            foreach (Fill fill in fills)
            {
                if (fill.Type.Contains("LINEAR"))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool ContainsGradients(this List<Fill> fills)
        {
            if (fills.IsEmpty())
            {
                return false;
            }

            foreach (Fill fill in fills)
            {
                if (fill.Type.Contains("GRADIENT"))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool IsSolidFillsOnly(this List<Fill> fills)
        {
            if (fills.IsEmpty())
            {
                return false;
            }

            foreach (Fill fill in fills)
            {
                if (fill.Type != "SOLID")
                {
                    return false;
                }
            }

            return true;
        }
    }
}