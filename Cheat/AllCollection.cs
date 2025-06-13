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
                Enumerable.Range(55101, 3),
                Enumerable.Range(109101, 3),
                Enumerable.Range(159101, 3),
                Enumerable.Range(209101, 3),
                Enumerable.Range(259101, 3),
                Enumerable.Range(309101, 3),
                Enumerable.Range(359101, 3),
                Enumerable.Range(409101, 3),
                Enumerable.Range(459101, 3),
                Enumerable.Range(509101, 3),
                // 舞代神、将、极、舞舞牌
                Enumerable.Range(6101, 51),
                // 段位牌
                Enumerable.Range(250051, 10),
                // WEC
                Enumerable.Range(507001, 7)}
            .Cast<IEnumerable<int>>()
            .SelectMany(x => x)
            .ToArray();
            partnerList = [];
            iconList = [];
            titleList = [];
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UserData), "get_FrameList")]
    public static void FrameList(ref List<UserItem> __result, CollectionProcess __instance)
    {

        List<int> list2 = (from i in __result
                           where i.stock > 0
                           select i.itemId).ToList();

        foreach (KeyValuePair<int, FrameData> frame2 in Singleton<DataManager>.Instance.GetFrames())
        {
            if (list2.Contains(frame2.Value.GetID())) continue;
            if (ExcludedCollection.frameList.Contains(frame2.Value.GetID())) continue;

            list2.Add(frame2.Value.GetID());
            __result.Add(new UserItem(frame2.Value.GetID()));
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UserData), "get_IconList")]
    public static void IconList(ref List<UserItem> __result, CollectionProcess __instance)
    {
        List<int> list2 = (from i in __result
                           where i.stock > 0
                           select i.itemId).ToList();

        foreach (KeyValuePair<int, IconData> icon2 in Singleton<DataManager>.Instance.GetIcons())
        {
            if (list2.Contains(icon2.Value.GetID())) continue;
            if (ExcludedCollection.iconList.Contains(icon2.Value.GetID())) continue;

            list2.Add(icon2.Value.GetID());
            __result.Add(new UserItem(icon2.Value.GetID()));
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UserData), "get_PlateList")]
    public static void PlateList(ref List<UserItem> __result, CollectionProcess __instance)
    {
        List<int> list2 = (from i in __result
                           where i.stock > 0
                           select i.itemId).ToList();

        foreach (KeyValuePair<int, PlateData> plate2 in Singleton<DataManager>.Instance.GetPlates())
        {
            if (list2.Contains(plate2.Value.GetID())) continue;
            if (ExcludedCollection.plateList.Contains(plate2.Value.GetID())) continue;

            list2.Add(plate2.Value.GetID());
            __result.Add(new UserItem(plate2.Value.GetID()));
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UserData), "get_PartnerList")]
    public static void PartnerList(ref List<UserItem> __result, CollectionProcess __instance)
    {
        List<int> list2 = (from i in __result
                           where i.stock > 0
                           select i.itemId).ToList();

        foreach (KeyValuePair<int, PartnerData> partner2 in Singleton<DataManager>.Instance.GetPartners())
        {
            if (list2.Contains(partner2.Value.GetID())) continue;
            if (ExcludedCollection.partnerList.Contains(partner2.Value.GetID())) continue;

            list2.Add(partner2.Value.GetID());
            __result.Add(new UserItem(partner2.Value.GetID()));
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UserData), "get_TitleList")]
    public static void TitleList(ref List<UserItem> __result, CollectionProcess __instance)
    {
        List<int> list2 = (from i in __result
                           where i.stock > 0
                           select i.itemId).ToList();

        foreach (KeyValuePair<int, TitleData> title2 in Singleton<DataManager>.Instance.GetTitles())
        {
            if (list2.Contains(title2.Value.GetID())) continue;
            if (ExcludedCollection.titleList.Contains(title2.Value.GetID())) continue;

            list2.Add(title2.Value.GetID());
            __result.Add(new UserItem(title2.Value.GetID()));
        }
    }
}