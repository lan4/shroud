using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;

using FlatRedBall;

namespace Shroud.Utilities
{
    public static class GameProperties
    {
        // Global Enemy Vars
        public static float EnemyMoveSpeed = 5.0f;
        public static float EnemyNodeTolerance = 0.3f;
        public static float WorldRotation = (float)Math.PI*3.0f/2.0f;
        public static string OldProfileString = "";
        public static string ProfileString = "";
        public static string LevelString = "";
        public static bool IsPaused = false;
        public static Game1 game;

        public static void RescaleSprite(Sprite s)
        {
            float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(s.Z);
            s.ScaleX = .5f * s.Texture.Width / pixelsPerUnit;
            s.ScaleY = .5f * s.Texture.Height / pixelsPerUnit;
        }

        public static void CreateProfiles()
        {
            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            //create new file
            using (StreamWriter writeFile = new StreamWriter(new IsolatedStorageFileStream("profiles2.txt", FileMode.Create, FileAccess.Write, myIsolatedStorage)))
            {
                //string defaultText = "-1 l1";
                writeFile.WriteLine("-1 l1");
                writeFile.WriteLine("-2 l1");
                writeFile.WriteLine("-3 l1");
                writeFile.WriteLine("-4 l1");
                writeFile.WriteLine("-5 l1");
                writeFile.WriteLine("-6 l1");
                writeFile.Flush();
                writeFile.Close();
            }

            myIsolatedStorage.Dispose();

            //System.Diagnostics.Debug.WriteLine("HERE" + Read());
        }

        public static void Save()
        {
            string oldProfiles = Read();

            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            //Open existing file
            IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("profiles2.txt", FileMode.Truncate, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                string newProfiles = oldProfiles.Replace(OldProfileString, ProfileString);
                writer.Write(newProfiles);
                writer.Close();
            }

            myIsolatedStorage.Dispose();

            OldProfileString = ProfileString;
        }

        public static string Read()
        {
            string text;

            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("profiles2.txt", FileMode.Open, FileAccess.Read);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                text = reader.ReadToEnd();
                reader.Close();
            }

            myIsolatedStorage.Dispose();

            return text;
        }

        public static void Quit()
        {
            game.Exit();
        }
    }
}
