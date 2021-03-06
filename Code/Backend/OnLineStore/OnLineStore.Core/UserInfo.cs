﻿namespace OnLineStore.Core
{
    public class UserInfo : IUserInfo
    {
        public UserInfo()
        {
        }

        public UserInfo(string name)
        {
            Name = name;
        }

        public string Domain { get; set; }

        public string Name { get; set; }

        public string[] Roles { get; set; }
    }
}
