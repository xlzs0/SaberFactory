﻿using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using BGLib.Polyglot;
using SaberFactory.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.BSML.Tags
{
    public class CustomButtonTag : BSMLTag
    {
        private static readonly Color _defaultNormalColor = new Color(0.086f, 0.090f, 0.101f, 0.8f);

        //private static readonly Color _defaultHoveredColor = new Color(0.086f, 0.090f, 0.101f);
        private static readonly Color _defaultHoveredColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        public override string[] Aliases => new[] { CustomComponentHandler.ComponentPrefix + ".button" };
        protected virtual string PrefabButton => "PracticeButton";

        private Button buttonPrefab;

        public override GameObject CreateObject(Transform parent)
        {
            if (buttonPrefab == null)
            {
                buttonPrefab = Resources.FindObjectsOfTypeAll<Button>().Last(x => x.name == PrefabButton);
            }

            var button = Object.Instantiate(buttonPrefab, parent, false);
            button.name = "BSMLButton";
            button.interactable = true;
            button.GetComponentInChildren<LocalizedTextMeshProUGUI>(true).TryDestroy();

            var externalComponents = button.gameObject.AddComponent<ExternalComponents>();

            var textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.richText = true;
            externalComponents.Components.Add(textMesh);

            Object.Destroy(button.transform.Find("Content").GetComponent<LayoutElement>());

            var bgImage = button.transform.Find("BG").gameObject.GetComponent<ImageView>();

            button.gameObject.GetComponent<ButtonStaticAnimations>().TryDestroy();
            var buttonStateColors = button.gameObject.AddComponent<ButtonStateColors>();
            externalComponents.Components.Add(buttonStateColors);
            buttonStateColors.Image = bgImage;
            buttonStateColors.NormalColor = _defaultNormalColor;
            buttonStateColors.HoveredColor = _defaultHoveredColor;
            buttonStateColors.SelectionDidChange(NoTransitionsButton.SelectionState.Normal);

            button.gameObject.GetComponent<NoTransitionsButton>().selectionStateDidChangeEvent +=
                buttonStateColors.SelectionDidChange;

            var buttonImageController = button.gameObject.AddComponent<ButtonImageController>();
            externalComponents.Components.Add(buttonImageController);
            buttonImageController.BackgroundImage = bgImage;
            buttonImageController.LineImage = button.transform.Find("Underline").gameObject.GetComponent<ImageView>();
            buttonImageController.ShowLine(false);
            bgImage.Cast<ImageView>().SetSkew(0);

            var buttonSizeFitter = button.gameObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            // miss me with those small ass buttons
            button.gameObject.GetComponent<LayoutElement>().preferredHeight = 10;

            var stackLayoutGroup = button.GetComponentInChildren<LayoutGroup>();
            if (stackLayoutGroup != null)
            {
                externalComponents.Components.Add(stackLayoutGroup);
            }

            if (!button.gameObject.activeSelf)
            {
                button.gameObject.SetActive(true);
            }

            return button.gameObject;
        }
    }
}