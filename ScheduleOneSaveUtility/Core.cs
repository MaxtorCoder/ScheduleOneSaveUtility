using MelonLoader;
using ScheduleOneSaveUtility.UI;

[assembly: MelonInfo(typeof(ScheduleOneSaveUtility.Core), "SaveUtility", "1.0.0", "MaxtorCoder")]
[assembly: MelonGame("TVGS", "Schedule I")]
namespace ScheduleOneSaveUtility
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Menu")
                UIManager.FindContinueButton();
        }
    }
}
