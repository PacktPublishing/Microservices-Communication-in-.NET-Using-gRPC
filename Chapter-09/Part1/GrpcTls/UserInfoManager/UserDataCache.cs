using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Users;

namespace UserInfoManager
{
    public class UserDataCache
    {
        private readonly List<UserInfo> users;

        public UserDataCache()
        {
            users = new List<UserInfo>();

            users.Add(new UserInfo
            {
                FirstName = "John",
                Surname = "Smith",
                Gender = "M",
                DateOfBirth = Timestamp.FromDateTime(DateTime.UtcNow.AddYears(-20)),
                Nationality = "English",
                Address = new AddressInfo
                {
                    FirstLine = "51 Park Lane",
                    PostcodeOrZipCode = "SW2 5BL",
                    Town = "London",
                    Country = "UK"
                }
            });

            users.Add(new UserInfo
            {
                FirstName = "Tamara",
                Surname = "Stevenson",
                Gender = "F",
                DateOfBirth = Timestamp.FromDateTime(DateTime.UtcNow.AddYears(-30).AddDays(58)),
                Nationality = "Australian",
                Address = new AddressInfo
                {
                    FirstLine = "7 Melbourne Road",
                    PostcodeOrZipCode = "1001",
                    Town = "Sydney",
                    Country = "Australia"
                }
            });

            users.Add(new UserInfo
            {
                FirstName = "Maksim",
                Surname = "Ivanov",
                Gender = "M",
                DateOfBirth = Timestamp.FromDateTime(DateTime.UtcNow.AddYears(-19).AddDays(45)),
                Nationality = "Russian",
                Address = new AddressInfo
                {
                    FirstLine = "19 Lomonosova Street, flat 74",
                    PostcodeOrZipCode = "103274",
                    Town = "Moscow",
                    Country = "Russian Federation"
                }
            });
        }

        public IEnumerable<UserInfo> GetUsers()
        {
            return users;
        }
    }
}
