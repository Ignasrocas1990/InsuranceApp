/*
    Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
*/

using System;
using System.IO;
using Android.Util;
using SQLite;
using watch.Models;
using Xamarin.Essentials;

namespace watch.Services
{
    /// <summary>
    /// Used to connect to local SQL database
    /// using 3rd party nuget
    /// </summary>
    public class SqlService : IDisposable
    {
        private static SqlService _db;
        private readonly SQLiteConnection connection;
        private const string Tag = "mono-stdout";

        private SqlService()
        {
            try
            {
                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MyData.db");
                Log.Verbose(Tag,$"SQL Service dataPath : {databasePath}");
                connection = new SQLiteConnection(databasePath);
                connection.CreateTable<User>();
                Log.Verbose(Tag, $"SQL AddUser, Is Connection closed?: {connection.Handle.IsClosed}");
            }
            catch (Exception e)
            {
                Log.Verbose(Tag,"SQL CREATION ERROR : "+e.Message);
            }
        }
        public static SqlService GetInstance()
        {
            return _db ??= new SqlService();
        }
        /// <summary>
        /// adds user to database
        /// </summary>
        /// <param name="userId">customerId string</param>
        /// <param name="email">customer email string</param>
        /// <param name="pass">password string</param>
        public void AddUser(string userId, string email, string pass)
        {
             try
             {
                 Log.Verbose(Tag, $"SQL AddUser, Is Connection closed?: {connection.Handle.IsClosed}");
                 connection.Insert(new User()
                 {
                             UserId = userId,
                             Email = email,
                             Pass = pass
                 });
                 Log.Verbose(Tag, "User Added successfully");
             }
             catch (Exception e)
             {
                 Log.Verbose(Tag,"SQL AddUser Error : "+e.Message);
             }
            
        }
        /// <summary>
        /// finds if user exist 
        /// </summary>
        /// <returns>User instance or null</returns>
        public User FindUser()
        {
            try
            {
                Log.Verbose(Tag, $"SQL FindUser, Is Connection closed?: {connection.Handle.IsClosed}");
                var user =  connection.Get<User>(user => user.DelFlag == false);
                Log.Verbose(Tag, $"SQL,User is found : {user.Email}");
                return user;
                
            }
            catch (Exception e)
            {
                Log.Verbose(Tag,"SQL FindUser Error : "+e.Message);
            }
            Log.Verbose(Tag,"SQL FindUser is NULL");
            return null;
        }
        public User ReplaceUser(string userId,string email,string pass)
        {
            try
            {
                connection.DeleteAll<User>();
                var user = new User()
                {
                    UserId = userId,
                    Email = email,
                    Pass = pass
                };
                connection.Insert(user);
                return user;
            }
            catch (Exception e)
            {
                Log.Verbose(Tag,e.Message);
            }

            return null;
        }

        public void ClearDatabase()//TODO ---------------- REMOVE when submitting
        {
            try
            {
                connection.DeleteAll<User>();
                Log.Verbose(Tag,"Database Cleared successfully");

            }
            catch (Exception e)
            {
                Log.Verbose(Tag,e.Message);
            }
        }

            public void Dispose()
        {
            connection?.Dispose();
        }
    }
}