using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Fusion;

namespace HW2
{
    public class UserDataManager : MonoBehaviour
    {
        public Dictionary<PlayerRef, UserData> UserDataDict { get; private set; }
        
        private List<string> playerNames;
        private List<Color> playerColors;

        public void Awake()
        {
            UserDataDict = new Dictionary<PlayerRef, UserData>();
            playerNames = new List<string>();
            playerColors = new List<Color>();
        }

        public bool TryAddUserData(PlayerRef player, UserData userData)
        {
            if (!playerNames.Contains(userData.nickname) &&
                userData.nickname.All(char.IsLetterOrDigit) &&
                !playerColors.Contains(userData.color) && 
                UserDataDict.TryAdd(player, userData))
            {
                playerNames.Add(userData.nickname);
                playerColors.Add(userData.color);
                return true;
            }
            
            return false;
        }

        public void removeUserData(PlayerRef player)
        {
            if (UserDataDict.ContainsKey(player)) {UserDataDict.Remove(player);}
        }
    }
}
