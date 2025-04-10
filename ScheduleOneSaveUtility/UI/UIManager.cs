using MelonLoader;

using ScheduleOne.Persistence;
using ScheduleOne.Persistence.Datas;

using UnityEngine;

namespace ScheduleOneSaveUtility.UI
{
    public static class UIManager
    {
        static GameObject _continueButtonPrefab;

        /// <summary>
        /// Find the continue button from the <see cref="MainMenuScreen"/>
        /// </summary>
        public static void FindContinueButton() => _continueButtonPrefab = GameObject.Find("MainMenu/Home/Bank/Continue");

        /// <summary>
        /// Draw the "Duplicate Save" button in the UI
        /// </summary>
        public static void AddDuplicateSaveButtons()
        {
            if (!_continueButtonPrefab)
            {
                Melon<Core>.Logger.Error("Failed to find continue button in main menu...");
                return;
            }

            var container = GameObject.Find("MainMenu/Continue/Container");
            for (var i = 0; i < container.transform.childCount; i++)
            {
                var slot = container.transform.GetChild(i);
                if (!slot)
                {
                    Melon<Core>.Logger.Error($"Slot for index: {i} is null.");
                    continue;
                }

                var slotContainer = slot.Find("Container");
                if (!slotContainer)
                {
                    Melon<Core>.Logger.Error($"Container {i} could not be found or is not active.");
                    continue;
                }

                var saveIndex = i;

                var duplicateButton = UIUtils.CreateButtonFromPrefab("Duplicate", 16, _continueButtonPrefab,
                    onClick: () =>
                {
                    DuplicateSaveButtonEvent(saveIndex);
                });
                duplicateButton.transform.SetParent(slotContainer.transform, false);
                duplicateButton.transform.position = slotContainer.transform.position;
                duplicateButton.transform.localPosition = new(451f, 15f, 0f);

                var deleteButton = UIUtils.CreateButtonFromPrefab("Delete", 16, _continueButtonPrefab, new(1f, 0.28f, 0.14f, 1f),
                    onClick: () =>
                {
                    DeleteSaveButtonEvent(saveIndex);
                });
                deleteButton.transform.SetParent(slotContainer.transform, false);
                deleteButton.transform.position = slotContainer.transform.position;
                deleteButton.transform.localPosition = new(575f, 15f, 0f);
            }
        }

        static void DuplicateSaveButtonEvent(int currentSaveIndex)
        {
            var saveGame = LoadManager.SaveGames[currentSaveIndex];
            if (saveGame == null)
                return;

            var nextIndex = -1;
            for (var i = 0; i < LoadManager.SaveGames.Length; ++i)
            {
                if (LoadManager.SaveGames[i] != null)
                    continue;

                nextIndex = i;
                break;
            }

            // List is full, ignore the rest
            if (nextIndex == -1)
                return;

            var savePath = Path.Combine(SaveManager.Instance.IndividualSavesContainerPath, $"SaveGame_{nextIndex + 1}");

            // Do not execute any of this if save path already exists
            if (Directory.Exists(savePath))
                return;

            saveGame.SaveSlotNumber = nextIndex + 1;
            saveGame.DateCreated = DateTime.Now;
            saveGame.OrganisationName = saveGame.OrganisationName.Contains("Duplicate") ?
                saveGame.OrganisationName.Replace($"Duplicate {nextIndex - 1}", $"Duplicate {nextIndex}") :
                $"{saveGame.OrganisationName} (Duplicate {nextIndex})";

            Melon<Core>.Logger.Msg($"Saving game {currentSaveIndex} -> {nextIndex} ({saveGame.OrganisationName})");

            var originalSavePath = Path.Combine(SaveManager.Instance.IndividualSavesContainerPath, $"SaveGame_{currentSaveIndex + 1}");
            CopyDirectory(originalSavePath, savePath, true);

            // Replace GameData in Game.json as we have a different organisation name now.
            var gameData = JsonUtility.FromJson<GameData>(File.ReadAllText(Path.Combine(savePath, "Game.json")));
            gameData.OrganisationName = saveGame.OrganisationName;
            File.WriteAllText(Path.Combine(savePath, "Game.json"), JsonUtility.ToJson(gameData));

            // Refresh the save data so it get's displayed in UI
            LoadManager.Instance.RefreshSaveInfo();
        }

        static void DeleteSaveButtonEvent(int currentSaveIndex)
        {
            var saveGame = LoadManager.SaveGames[currentSaveIndex];
            if (saveGame == null)
                return;

            // Delete the current save...
            var originalSavePath = Path.Combine(SaveManager.Instance.IndividualSavesContainerPath, $"SaveGame_{currentSaveIndex + 1}");
            Directory.Delete(originalSavePath, true);

            // Refresh the save data so it get's displayed in UI
            LoadManager.Instance.RefreshSaveInfo();
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory and check if it exists
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            var dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (var file in dir.GetFiles())
            {
                var targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (!recursive)
                return;

            foreach (var subDir in dirs)
            {
                var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}
