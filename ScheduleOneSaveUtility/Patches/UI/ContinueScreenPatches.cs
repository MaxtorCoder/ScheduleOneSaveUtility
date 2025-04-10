using HarmonyLib;

using ScheduleOne.UI.MainMenu;
using ScheduleOneSaveUtility.UI;
using UnityEngine;

namespace ScheduleOneSaveUtility.Patches.UI
{
    [HarmonyPatch(typeof(ContinueScreen))]
    public class ContinueScreenPatches
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(ContinueScreen __instance)
        {
            if (!__instance.gameObject.GetComponent<ContinueScreenComponent>())
                __instance.gameObject.AddComponent<ContinueScreenComponent>();
        }
    }

    public class ContinueScreenComponent : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log("Adding \"Duplicate Save\" buttons...");
            UIManager.AddDuplicateSaveButtons();
        }
    }
}
