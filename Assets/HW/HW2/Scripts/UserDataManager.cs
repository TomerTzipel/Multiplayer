using System;
using UnityEngine;
using System.Collections.Generic;
using Fusion;

namespace HW2
{
    public class UserDataManager : MonoBehaviour
    {
        private Dictionary<PlayerRef, UserData> userDataDict;
        private UserData localUserData;

        public void Awake()
        {
            userDataDict = new Dictionary<PlayerRef, UserData>();
        }

        void addUserData(PlayerRef player, UserData userData)
        {
            userDataDict.Add(player, userData);
        }

        void removeUserData(PlayerRef player)
        {
            userDataDict.Remove(player);
        }
        
        
    }
}
