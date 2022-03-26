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
using Xamarin.Essentials;

namespace watch.Services
{
    public class SqlService : IDisposable
    {
        private static SqlService db;
        SQLiteConnection connection;
        private const string TAG = "mono-stdout";

        private SqlService()
        {
            try
            {
                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MyData.db");
                Log.Verbose(TAG,$"SQL Service dataPath : {databasePath}");
                connection = new SQLiteConnection(databasePath);
                connection.CreateTable<User>();
                Log.Verbose(TAG, $"SQL AddUser, Is Connection closed?: {connection.Handle.IsClosed}");
            }
            catch (Exception e)
            {
                Log.Verbose(TAG,"SQL CREATION ERROR : "+e.Message);
            }
        }
        public static SqlService GetInstance()
        {
            return db ??= new SqlService();
        }
        
        public void AddUser(string userId, string email, string pass)
        {
             try
             {
                 Log.Verbose(TAG, $"SQL AddUser, Is Connection closed?: {connection.Handle.IsClosed}");
                 connection.Insert(new User()
                 {
                             UserId = userId,
                             Email = email,
                             Pass = pass
                 });
                 Log.Verbose(TAG, "User Added successfully");
             }
             catch (Exception e)
             {
                 Log.Verbose(TAG,"SQL AddUser Error : "+e.Message);
             }
            
        }

        public User FindUser()
        {
            try
            {
                Log.Verbose(TAG, $"SQL FindUser, Is Connection closed?: {connection.Handle.IsClosed}");
                var user =  connection.Get<User>(user => user.DelFlag == false);
                Log.Verbose(TAG, $"SQL,User is found : {user.Email}");
                return user;
                
            }
            catch (Exception e)
            {
                Log.Verbose(TAG,"SQL FindUser Error : "+e.Message);
            }
            Log.Verbose(TAG,"SQL FindUser is NULL");
            return null;
        }
        public User ReplaceUser(string userId,string email,string pass)
        {
            try
            {
                connection.DeleteAll<User>();
                User user = new User()
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
                Log.Verbose(TAG,e.Message);
            }

            return null;
        }

        public void ClearDatabase()
        {
            try
            {
                connection.DeleteAll<User>();
                Log.Verbose(TAG,"Database Cleared successfully");

            }
            catch (Exception e)
            {
                Log.Verbose(TAG,e.Message);
            }
        }

            public void Dispose()
        {
            connection?.Dispose();
        }
    }
}