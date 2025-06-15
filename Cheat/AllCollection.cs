using HarmonyLib;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using System.Collections.Generic;
using System.Linq;


namespace SinmaiAssist.Cheat;

public class AllCollection
{
    public static readonly bool ExcludedCollectionEnabled
            = SinmaiAssist.MainConfig.Cheat.AllCollection.ExcludeSomeItems;
    public static readonly bool ForceRemoveExcludedItems
            = SinmaiAssist.MainConfig.Cheat.AllCollection.ForceRemoveExcludedItems;

    public static class ExcludedCollection
    {
        public static readonly int[] frameList;
        public static readonly int[] iconList;
        public static readonly int[] plateList;
        public static readonly int[] titleList;
        public static readonly int[] partnerList;


        static ExcludedCollection()
        {
            frameList = [
                109101, 109102, 209101, 209102, 209103,
                259101, 309101, 309102, 309103, 409101,
                409102, 409103, 509101, 509102, 509103
            ];
            plateList = new object[] {
                // DX代神、将、极、舞舞牌
                Enumerable.Range(55101, 4),
                Enumerable.Range(109101, 4),
                Enumerable.Range(159101, 4),
                Enumerable.Range(209101, 4),
                Enumerable.Range(259101, 4),
                Enumerable.Range(309101, 4),
                Enumerable.Range(359101, 4),
                Enumerable.Range(409101, 4),
                Enumerable.Range(459101, 4),
                Enumerable.Range(509101, 4),
                // 舞代神、将、极、舞舞牌
                Enumerable.Range(6101, 52),
                // 段位牌
                Enumerable.Range(250051, 10),
                Enumerable.Range(150051, 1),
                Enumerable.Range(450051, 1),
                // WEC
                Enumerable.Range(507001, 8)}
            .Cast<IEnumerable<int>>()
            .SelectMany(x => x)
            .ToArray();
            partnerList = [];
            iconList = [];
            titleList = [
                458582, 458583, 409000, 409001, 409002,
                409003, 409004, 409005, 409006, 409007
            ];
        }
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(UserData), "get_FrameList")]
    public static void FrameList(ref List<UserItem> __result, CollectionProcess __instance)
    {
        bool isExcluded;
        bool isGained;
        UserItem newItem;
        // bug: 
        List<int> list2 = (from i in __result
                           where i.stock > 0
                           select i.itemId).ToList();

        foreach (KeyValuePair<int, FrameData> frame2 in Singleton<DataManager>.Instance.GetFrames())
        {
            isGained = list2.Contains(frame2.Value.GetID());
            isExcluded = ExcludedCollection.frameList.Contains(frame2.Value.GetID());

            // case: items that should not be included
            if (isGained && !isExcluded) continue;

            // case: items that are excluded but not forced to proceed, ignoring this item
            if (isExcluded && ExcludedCollectionEnabled && !ForceRemoveExcludedItems)
                continue;

            // fallback case: items that are decided to be added or removed
            newItem = new UserItem(frame2.Value.GetID());
            if (isExcluded && ForceRemoveExcludedItems)
            {
                newItem.stock = 0;
                newItem.isValid = false;
            }

            __result.Add(newItem);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UserData), "get_IconList")]
    public static void IconList(ref List<UserItem> __result, CollectionProcess __instance)
    {
        bool isExcluded;
        bool isGained;
        UserItem newItem;
        List<int> list2 = (from i in __result
                           where i.stock > 0
                           select i.itemId).ToList();

        foreach (KeyValuePair<int, IconData> icon2 in Singleton<DataManager>.Instance.GetIcons())
        {
            isGained = list2.Contains(icon2.Value.GetID());
            isExcluded = ExcludedCollection.iconList.Contains(icon2.Value.GetID());

            // case: items that should not be included
            if (isGained && !isExcluded) continue;

            // case: items that are excluded but not forced to proceed, ignoring this item
            if (isExcluded && ExcludedCollectionEnabled && !ForceRemoveExcludedItems)
                continue;

            // fallback case: items that are decided to be added or removed
            newItem = new UserItem(icon2.Value.GetID());
            if (isExcluded && ForceRemoveExcludedItems)
            {
                newItem.stock = 0;
                newItem.isValid = false;
            }

            __result.Add(newItem);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UserData), "get_PlateList")]
    public static void PlateList(ref List<UserItem> __result, CollectionProcess __instance)
    {
        bool isExcluded;
        bool isGained;
        UserItem newItem;
        List<int> list2 = (from i in __result
                           where i.stock > 0
                           select i.itemId).ToList();

        foreach (KeyValuePair<int, PlateData> plate2 in Singleton<DataManager>.Instance.GetPlates())
        {
            isGained = list2.Contains(plate2.Value.GetID());
            isExcluded = ExcludedCollection.plateList.Contains(plate2.Value.GetID());

            // case: items that should not be included
            if (isGained && !isExcluded) continue;

            // case: items that are excluded but not forced to proceed, ignoring this item
            if (isExcluded && ExcludedCollectionEnabled && !ForceRemoveExcludedItems)
                continue;

            // fallback case: items that are decided to be added or removed
            newItem = new UserItem(plate2.Value.GetID());
            if (isExcluded && ForceRemoveExcludedItems)
            {
                newItem.stock = 0;
                newItem.isValid = false;
            }

            __result.Add(newItem);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UserData), "get_PartnerList")]
    public static void PartnerList(ref List<UserItem> __result, CollectionProcess __instance)
    {
        bool isExcluded;
        bool isGained;
        UserItem newItem;
        List<int> list2 = (from i in __result
                           where i.stock > 0
                           select i.itemId).ToList();

        foreach (KeyValuePair<int, PartnerData> partner2 in Singleton<DataManager>.Instance.GetPartners())
        {
            isGained = list2.Contains(partner2.Value.GetID());
            isExcluded = ExcludedCollection.partnerList.Contains(partner2.Value.GetID());

            // case: items that should not be included
            if (isGained && !isExcluded) continue;

            // case: items that are excluded but not forced to proceed, ignoring this item
            if (isExcluded && ExcludedCollectionEnabled && !ForceRemoveExcludedItems)
                continue;

            // fallback case: items that are decided to be added or removed
            newItem = new UserItem(partner2.Value.GetID());
            if (isExcluded && ForceRemoveExcludedItems)
            {
                newItem.stock = 0;
                newItem.isValid = false;
            }

            __result.Add(newItem);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UserData), "get_TitleList")]
    public static void TitleList(ref List<UserItem> __result, CollectionProcess __instance)
    {
        bool isExcluded;
        bool isGained;
        UserItem newItem;
        List<int> list2 = (from i in __result
                           where i.stock > 0
                           select i.itemId).ToList();

        foreach (KeyValuePair<int, TitleData> title2 in Singleton<DataManager>.Instance.GetTitles())
        {
            isGained = list2.Contains(title2.Value.GetID());
            isExcluded = ExcludedCollection.titleList.Contains(title2.Value.GetID());

            // case: items that should not be included
            if (isGained && !isExcluded) continue;

            // case: items that are excluded but not forced to proceed, ignoring this item
            if (isExcluded && ExcludedCollectionEnabled && !ForceRemoveExcludedItems)
                continue;

            // fallback case: items that are decided to be added or removed
            newItem = new UserItem(title2.Value.GetID());
            if (isExcluded && ForceRemoveExcludedItems)
            {
                newItem.stock = 0;
                newItem.isValid = false;
            }

            __result.Add(newItem);
        }
    }
}