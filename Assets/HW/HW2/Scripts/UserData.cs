using UnityEngine;

namespace HW2
{
    public class UserData
    {
        public string nickname { get; private set; }
        public Color color { get; private set; }

        public UserData(string nickname, Color color)
        {
            this.nickname = nickname;
            this.color = color;
        }
    }   
}
