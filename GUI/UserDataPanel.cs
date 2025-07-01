using System;
using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Manager;
using Manager.UserDatas;
using MelonLoader;
using SinmaiAssist.Utils;
using UnityEngine;

namespace SinmaiAssist.GUI;

public class UserDataPanel
{
    private static UserData _player1 = null;
    private static UserData _player2 = null;
    //private static bool _isNewItem = false;
    private static bool _isAllowOverwriteLevel = false;

    private enum CollectionType
    {
        Icon = UserData.Collection.Icon,
        Plate = UserData.Collection.Plate,
        Title = UserData.Collection.Title,
        Partner = UserData.Collection.Partner,
        Frame = UserData.Collection.Frame
    }

    private static string[] _userInputId = ["", "", "", "", "", "", "", "", ""];

    public static void OnGUI()
    {
        GUILayout.Label($"User Info", MainGUI.Style.Title);
        try
        {
            _player1 = Singleton<UserDataManager>.Instance.GetUserData(0);
            _player2 = Singleton<UserDataManager>.Instance.GetUserData(1);
        }
        catch (Exception e)
        {
            // ignore
        }
        GUILayout.Label($"1P: {_player1.Detail.UserName} ({_player1.Detail.UserID})", MainGUI.Style.Text);
        GUILayout.Label($"2P: {_player2.Detail.UserName} ({_player2.Detail.UserID})", MainGUI.Style.Text);

        GUILayout.Label("Add Collections", MainGUI.Style.Title);
        foreach (CollectionType type in Enum.GetValues(typeof(CollectionType)))
        {
            GUILayout.BeginHorizontal();
            int typeId = (int)type;
            GUILayout.Label(type.ToString(), new GUIStyle(MainGUI.Style.Text) { fixedWidth = 40 });
            _userInputId[typeId] = GUILayout.TextField(_userInputId[typeId]);
            if (GUILayout.Button("Add", new GUIStyle(MainGUI.Style.Button) { fixedWidth = 25 }))
            {
                TryParseToIDs(_userInputId[typeId], out HashSet<int> itemAddList);
                AddCollections(0, type, itemAddList.ToArray());
                AddCollections(1, type, itemAddList.ToArray());
            }
            else if (GUILayout.Button("Del", new GUIStyle(MainGUI.Style.Button) { fixedWidth = 25 }))
            {
                TryParseToIDs(_userInputId[typeId], out HashSet<int> itemDelList);
                DelCollections(0, type, itemDelList.ToArray());
                DelCollections(1, type, itemDelList.ToArray());
            }
            GUILayout.EndHorizontal();
        }
        //_isNewItem = GUILayout.Toggle(_isNewItem, "Is New Item");

        GUILayout.Label("Unlock Music", MainGUI.Style.Title);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Music", new GUIStyle(MainGUI.Style.Text) { fixedWidth = 40 });
        _userInputId[0] = GUILayout.TextField(_userInputId[0]);
        if (GUILayout.Button("Add", new GUIStyle(MainGUI.Style.Button) { fixedWidth = 40 }))
        {
            TryParseToIDs(_userInputId[0], out HashSet<int> unlockMusicList);
            UnlockMusic(0, unlockMusicList.ToArray());
            UnlockMusic(1, unlockMusicList.ToArray());
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Edit Characters (id + level)", MainGUI.Style.Title);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Chara", new GUIStyle(MainGUI.Style.Text) { fixedWidth = 40 });
        _userInputId[7] = GUILayout.TextField(_userInputId[7]);
        _userInputId[8] = GUILayout.TextField(
            _userInputId[8],
            new GUIStyle(UnityEngine.GUI.skin.textField)
            {
                fixedWidth = 35,
                alignment = TextAnchor.MiddleCenter
            }
        );
        if (GUILayout.Button("Add", new GUIStyle(MainGUI.Style.Button) { fixedWidth = 40 }))
        {
            TryParseToIDs(_userInputId[7], out HashSet<int> charaEditList);
            TryParseToIDs(_userInputId[8], out HashSet<int> level);
            AddCharaterEx(0, charaEditList.ToArray(), (uint)level.FirstOrDefault() | 1U);
            AddCharaterEx(1, charaEditList.ToArray(), (uint)level.FirstOrDefault() | 1U);
        }
        GUILayout.EndHorizontal();
        _isAllowOverwriteLevel = GUILayout.Toggle(_isAllowOverwriteLevel, "Allow Overwritting level");

        GUILayout.Label("MaiMile", MainGUI.Style.Title);
        GUILayout.BeginHorizontal();
        GUILayout.Label("MaiMile", new GUIStyle(MainGUI.Style.Text) { fixedWidth = 40 });
        _userInputId[6] = GUILayout.TextField(_userInputId[6]);
        if (GUILayout.Button("Add", new GUIStyle(MainGUI.Style.Button) { fixedWidth = 40 }))
        {
            AddMaiMile(0, _userInputId[6]);
            AddMaiMile(1, _userInputId[6]);
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("User Data Backup", MainGUI.Style.Title);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("1P", MainGUI.Style.Button)) User.ExportBackupData(0);
        if (GUILayout.Button("2P", MainGUI.Style.Button)) User.ExportBackupData(1);
        GUILayout.EndHorizontal();

    }

    private static void AddCollections(long index, CollectionType type, in int[] ids)
    {
        int successCount = 0;
        int failCount = 0;
        bool isNewUtem = true;      // this maybe not affect anything, both values are effective
        UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
        if (userData.IsGuest())
        {
            GameMessageManager.SendMessage((int)index, "Guest Account\nUnable to add collections");
            return;
        }
        try
        {
            foreach (int id in ids)
            {
                if (userData.AddCollections((UserData.Collection)type, id, isNewUtem))
                {
                    successCount++;
                }
                else
                {
                    // if your account once had the collections and you had removed them via sending stock0,
                    // title server would still keep these columns even that the data are invalid
                    // and the game will complain as a result of the dirty data
                    // so this message will show even if you added the collection
                    failCount++;
                }
                GameMessageManager.SendMessage((int)index,
                    $"{successCount} {(successCount > 1 ? type : type.ToString() + 's')}s added without error, " +
                    $"{failCount} {(failCount > 1 ? type : type.ToString() + 's')} error");
                if (failCount > 0)
                {
                    GameMessageManager.SendMessage((int)index, $"Unknown error...\n" +
                        $"Usually caused by the dirty data of your account. " +
                        $"this program ensures the item to be added or existed, so most of time you can ignore this error\n" +
                        $"\n{type} {id}", title: "Warning");
                }

            }
        }
        catch (Exception e)
        {
            GameMessageManager.SendMessage((int)index, $"Unknown error");
            MelonLogger.Error(e);
        }
    }

    private static void DelCollections(long index, CollectionType type, in int[] ids)
    {
        UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
        if (userData.IsGuest())
        {
            GameMessageManager.SendMessage((int)index, "Guest Account\nUnable to delete collections");
            return;
        }
        try
        {
            List<UserItem> oldCollections;
            switch (type)
            {
                case CollectionType.Icon:
                    oldCollections = userData.IconList;
                    break;
                case CollectionType.Plate:
                    oldCollections = userData.PlateList;
                    break;
                case CollectionType.Title:
                    oldCollections = userData.TitleList;
                    break;
                case CollectionType.Partner:
                    oldCollections = userData.PartnerList;
                    break;
                case CollectionType.Frame:
                    oldCollections = userData.FrameList;
                    break;
                default:
                    GameMessageManager.SendMessage((int)index, $"Unsupported Collection Type: {type}");
                    return;

            }
            foreach (int id in ids)
            {
                var oldItem = oldCollections.Select((UserItem it) => it).FirstOrDefault((UserItem it) => it.itemId == id);
                if (oldItem == null)
                {
                    GameMessageManager.SendMessage((int)index, $"Item not found, do nothing...\n{type} {id}");
                    return;
                }
                oldItem.stock = 0;
                oldItem.isValid = false;
                GameMessageManager.SendMessage((int)index, $"Item deleted\n{type} {id}");
            }
        }
        catch (Exception e)
        {
            GameMessageManager.SendMessage((int)index, $"Unknown error");
            MelonLogger.Error(e);
        }
    }

    private static void AddCharaterEx(long index, in int[] ids, uint level = 1U) {
        UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
        if (userData.IsGuest())
        {
            GameMessageManager.SendMessage((int)index, "Guest Account\nUnable to edit character");
            return;
        }
        foreach (int id in ids)
        {
            var old_chara = userData.CharaList.FirstOrDefault((UserChara c) => c.ID == id);
            if (old_chara == null)
            {
                UserChara newChara = new UserChara(id);
                newChara.AddLevel(level - 1);
                userData.CharaList.Add(newChara);
                userData.NewCharaList.Add(id);
                GameMessageManager.SendMessage((int)index, $"Add Character {id}\nwith Lv.{level}");
            }
            else
            {
                if (!_isAllowOverwriteLevel)
                {
                    GameMessageManager.SendMessage((int)index, $"Character already exists, do nothing...\n{id}");
                    return;
                }
                if (level < old_chara.Level)
                {
                    GameMessageManager.SendMessage((int)index, $"Downgrading character level is prohibited\n{id}");
                    return;
                }
                GameMessageManager.SendMessage((int)index, $"Character {id} level up\nLv.{old_chara.Level} -> Lv.{level}");
                old_chara.AddLevel(level - old_chara.Level);
            }
        }
    }

    private static void UnlockMusic(long index, in int[] ids)
    {
        UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
        if (userData.IsGuest())
        {
            GameMessageManager.SendMessage((int)index, "Guest Account\nUnable to unlock music");
            return;
        }
        try
        {
            foreach (int id in ids)
            {
                if (!userData.IsUnlockMusic(UserData.MusicUnlock.Base, id))
                {
                    if (userData.AddUnlockMusic(UserData.MusicUnlock.Base, id))
                    {
                        GameMessageManager.SendMessage((int)index, $"Unlock Music \n{id}");
                    }
                    else
                    {
                        GameMessageManager.SendMessage((int)index, $"Failed to unlock music or already unlocked \n{id}");
                    }
                }
                else if (!userData.IsUnlockMusic(UserData.MusicUnlock.Master, id))
                {
                    userData.AddUnlockMusic(UserData.MusicUnlock.Master, id);
                    userData.AddUnlockMusic(UserData.MusicUnlock.ReMaster, id);
                    GameMessageManager.SendMessage((int)index, $"Unlock Master \n{id}");
                }
                else
                {
                    GameMessageManager.SendMessage((int)index, $"Failed to unlock Master or already unlocked\n{id}");
                }
            }
        }
        catch (Exception e)
        {
            GameMessageManager.SendMessage((int)index, $"Unknown error");
            MelonLogger.Error(e);
        }
    }

    private static void AddMaiMile(long index, string input)
    {
        UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
        if (SinmaiAssist.GameVersion < 25000)
        {
            GameMessageManager.SendMessage((int)index, "MaiMile is not supported in this version");
            return;
        }
        if (userData.IsGuest())
        {
            GameMessageManager.SendMessage((int)index, "Guest Account\nUnable to add MaiMile");
            return;
        }
        try
        {
            if (int.TryParse(input, out int addMile))
            {
                var haveMile = userData.Detail.Point;
                if (haveMile + addMile >= 99999)
                    addMile = 99999 - haveMile;
                var addMileBefore = haveMile + addMile;

                userData.AddPresentMile(addMile);
                GameMessageManager.SendMessage((int)index, $"Add {addMile} MaiMile\n ({addMileBefore} -> {haveMile})");
            }
            else
            {
                GameMessageManager.SendMessage((int)index, $"Invalid MaiMile\n {input}");
            }
        }
        catch (Exception e)
        {
            GameMessageManager.SendMessage((int)index, $"Unknown error");
            MelonLogger.Error(e);
        }
    }

    private static bool TryParseToIDs(string input, out HashSet<int> ids, Func<bool> error_hook = null)
    {
        char[] DELIMITER = { ',', '，' };
        char[] SEQ_INDICATOR = { '-' };
        var elems = input.Split(DELIMITER);

        int id;
        bool error = false;
        ids = new HashSet<int>();

        if (input.Length == 0 || input.Trim().Length == 0) return false; // no input

        foreach (var elem in elems.Select((string s) => s.Trim()))
        {
            if (int.TryParse(elem, out id))
            {
                ids.Add(id);
            }
            else
            {
                string[] elems2 = elem.Split(SEQ_INDICATOR);
                if (elems2.Length == 2 && int.TryParse(elems2[0], out int start) && int.TryParse(elems2[1], out int end))
                {
                    if (start > end)
                    {
                        error = true;
                        // ignore the rest of input
                        break;
                    }
                    for (int i = start; i <= end; i++)
                    {
                        ids.Add(i);
                    }
                }
                else
                {
                    error = true;
                }
            }
        }
        if (error_hook != null) error_hook();

        return error;
    }
}