using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DA_Assets.FCU.Extensions
{
    public static class TagExtensions
    {
        public static void SetTag(this FObject fobject, FCU_Tag tag)
        {
            fobject.Tags = new List<FCU_Tag>
            {
                tag
            };
        }
        public static void AddTag(this FObject fobject, FCU_Tag tag)
        {
            switch (tag)
            {
                case FCU_Tag.Image:
                    fobject.RemoveTag(FCU_Tag.Vector);
                    break;
            }

            if (fobject.Tags.Contains(tag) == false)
            {
                fobject.Tags.Add(tag);
            }
        }
        public static void RemoveTag(this FObject fobject, FCU_Tag tag)
        {
            List<FCU_Tag> tags = new List<FCU_Tag>();

            foreach (FCU_Tag item in fobject.Tags)
            {
                if (item != tag)
                {
                    tags.Add(item);
                }
            }

            fobject.Tags = tags;
        }
        public static bool ContainsTag(this FObject fobject, FCU_Tag tag)
        {
            if (fobject.Tags.IsEmpty())
            {
                return false;
            }

            return fobject.Tags.Contains(tag);
        }
        public static bool HasParentTag(this FObject fobject)
        {
            foreach (var tag in fobject.Tags)
            {
                TagConfig tc = tag.GetTagConfig();

                if (tc.IsParent)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool IsImageOnly(this FObject fobject)
        {
            if (fobject.Tags.IsEmpty())
            {
                return false;
            }

            if (fobject.ContainsTag(FCU_Tag.Image) == false)
            {
                return false;
            }

            if (fobject.Tags.Count() == 1 && fobject.ContainsTag(FCU_Tag.Image))
            {
                return true;
            }

            if (fobject.ContainsTag(FCU_Tag.Container))
            {
                return false;
            }

            return fobject.CantBeInsideSingleImage();
        }
        public static bool CantBeInsideSingleImage(this FObject fobject)
        {
            bool canIgnoreOtherTags = false;

            foreach (FCU_Tag fcuTag in fobject.Tags)
            {
                TagConfig tc = fcuTag.GetTagConfig();

                if (tc.CanBeInsideSingleImage == false)
                {
                    canIgnoreOtherTags = true;
                    break;
                }
            }

            return canIgnoreOtherTags;
        }
        public static TagConfig GetTagConfig(this FCU_Tag fcuTag)
        {
            TagConfig tagConfig = Config.FCU_Config.Instance.TagConfigs.FirstOrDefault(x => x.FCU_Tag == fcuTag);
            return tagConfig;
        }
    }
}