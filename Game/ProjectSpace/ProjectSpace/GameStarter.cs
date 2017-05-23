using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Gwen.Control;

namespace OutpostOmega
{
    /// <summary>
    /// Helps starting up the game
    /// </summary>
    static class GameStarter
    {

        /// <summary>
        /// Asynchronously saves a world. Displays a loadingscreen while saving
        /// </summary>
        /// <param name="sceneManager">The current scene manager</param>
        /// <param name="GwenFrame">GWEN Frame the loadingscreen gets docked to</param>
        /// <param name="world">the gameworld</param>
        /// <param name="target">the target savefile</param>
        public static void Save(SceneManager sceneManager, Base GwenFrame, Game.World world, FileInfo target)
        {
            var loadScreen = new Drawing.UI.LoadingScreen(sceneManager.Active, GwenFrame, string.Format("Saving world '{0}' to {1}", world.ID, target.Name));

            //Add saving-operation to global threadpool
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(SaveGame), new object[4] { sceneManager, loadScreen, world, target });
        }

        /// <summary>
        /// Saves a game and stores it on the harddrive
        /// </summary>
        /// <param name="state">Object-array containing the scenemanger, the loadingscreen, the gameworld and the target fileinfo</param>
        private static void SaveGame(object state)
        {
            var sceneManager = (SceneManager)((object[])state)[0];
            var loadScreen = (Drawing.UI.LoadingScreen)((object[])state)[1];
            var world = (Game.World)((object[])state)[2];
            var target = (FileInfo)((object[])state)[3];


#if DEBUG
            OutpostOmega.Data.DataHandler.SaveToFile(world, target);
#else
            try
            {
                OutpostOmega.Data.DataHandler.SaveToFile(world, target);
            }
            catch (Exception e)
            {
                loadScreen.Abort("Unexpected error while saving. World could not be saved.");
            }
#endif

            loadScreen.Hide();

            //var sceneHandle = sceneManager.AddScene(new Scenes.Game(sceneManager.Active.Game, world));
            //sceneManager.ScheduleSceneChange(sceneHandle);
        }

        /// <summary>
        /// Asynchronously loads a savegame. Displays a loadingscreen while loading
        /// </summary>
        /// <param name="sceneManager">The current scene manager</param>
        /// <param name="GwenFrame">GWEN Frame the loadingscreen gets docked to</param>
        /// <param name="file">the Savefile</param>
        public static void Load(SceneManager sceneManager, Base GwenFrame, FileInfo file)
        {
            var loadScreen = new Drawing.UI.LoadingScreen(sceneManager.Active, GwenFrame, "Loading World");

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(LoadGame), new object[3] { sceneManager, loadScreen, file });
        }

        /// <summary>
        /// Loads a game and boots a new gamescene with it
        /// </summary>
        /// <param name="state">Object-array containing the scenemanger, the loadingscreen and the targetfile</param>
        private static void LoadGame(object state)
        {
            var sceneManager = (SceneManager)((object[])state)[0];
            var loadScreen = (Drawing.UI.LoadingScreen)((object[])state)[1];
            var targetFile = (FileInfo)((object[])state)[2];

            Game.World world = null;

#if DEBUG
            world = OutpostOmega.Data.DataHandler.LoadWorldFromFile(targetFile);
#else
            try
            {
                world = OutpostOmega.Data.DataHandler.LoadWorldFromFile(targetFile);
            }
            catch (Exception e)
            {

            }
#endif
            if (world == null)
            {
                loadScreen.Abort("Savegame corrupted. Could not load.");
                return;
            }


            var sceneHandle = sceneManager.AddScene(new Scenes.Game(sceneManager.Active.Game, world));
            sceneManager.ScheduleSceneChange(sceneHandle);
        }
    }
}
