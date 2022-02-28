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