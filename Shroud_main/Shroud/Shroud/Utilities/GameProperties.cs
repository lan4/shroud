using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;

using FlatRedBall;
using Microsoft.Devices;

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
        public static string LevelToken = "";
        public static int NumLevels = 0;
        public static bool IsPaused = false;
        public static bool JumpBack = false;
        public static Game1 game;
        public static bool PlayerAlive = true;
        public static bool HiddenBadge = true;
        public static bool NoDieBadge = true;
        public static bool OneKillBadge = true;
        public static int TotalLevels = 2;

        public static void RescaleSprite(Sprite s)
        {
            float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(s.Z);
            s.ScaleX = .5f * s.Texture.Width / pixelsPerUnit;
            s.ScaleY = .5f * s.Texture.Height / pixelsPerUnit;
        }

        public static void RescaleSprite(Sprite s, float scale)
        {
            RescaleSprite(s);
            s.ScaleX *= scale;
            s.ScaleY *= scale;
        }

        public static void CreateProfiles()
        {
            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            //create new file
            using (StreamWriter writeFile = new StreamWriter(new IsolatedStorageFileStream("profiles2.txt", FileMode.Create, FileAccess.Write, myIsolatedStorage)))
            {
                //string defaultText = "-1 l1";
                writeFile.WriteLine("p-1 r l1");
                writeFile.WriteLine("p-2 t l1");
                writeFile.WriteLine("p-3 u l1");
                writeFile.WriteLine("p-4 v l1");
                writeFile.WriteLine("p-5 w l1");
                writeFile.WriteLine("p-6 x l1");
                writeFile.Flush();
                writeFile.Close();
            }

            myIsolatedStorage.Dispose();

            //System.Diagnostics.Debug.WriteLine("HERE" + Read());
        }

        public static void Delete()
        {
            if (ProfileString.Contains('r'))
            {
                ProfileString = "p-1 r l1";
            }
            else if (ProfileString.Contains('t'))
            {
                ProfileString = "p-2 t l1";
            }
            else if (ProfileString.Contains('u'))
            {
                ProfileString = "p-3 u l1";
            }
            else if (ProfileString.Contains('v'))
            {
                ProfileString = "p-4 v l1";
            }
            else if (ProfileString.Contains('w'))
            {
                ProfileString = "p-5 w l1";
            }
            else if (ProfileString.Contains('x'))
            {
                ProfileString = "p-6 x l1";
            }

            Save();
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

        public static void Vibrate()
        {
            VibrateController.Default.Start(TimeSpan.FromSeconds(.05));
        }
    }
}
