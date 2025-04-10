using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ScheduleOneSaveUtility.UI;

public static class UIUtils
{
    public static GameObject CreateButtonFromPrefab(string text, float fontSize, GameObject prefab,
        Color? fontColor = null, UnityAction onClick = null)
    {
        var button = Object.Instantiate(prefab);
        button.name = text + "Button";

        var textMeshComponent = button.GetComponentInChildren<TextMeshProUGUI>();
        textMeshComponent.text = text;
        textMeshComponent.fontSize = fontSize;

        if (fontColor.HasValue)
            textMeshComponent.color = fontColor.Value;

        if (onClick != null)
        {
            var buttonComponent = button.GetComponent<Button>();
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(onClick);
        }

        return button;
    }
}
