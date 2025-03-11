﻿using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using SaberFactory.UI.Lib;
using UnityEngine;
using VRUIControls;

namespace SaberFactory.Helpers
{
    internal static class UIHelpers
    {
        private static readonly FieldAccessor<ImageView, float>.Accessor SkewAcc =
            FieldAccessor<ImageView, float>.GetAccessor("_skew");

        private static readonly FieldAccessor<ImageView, bool>.Accessor GradientAcc =
            FieldAccessor<ImageView, bool>.GetAccessor("_gradient");

        public static async Task<BSMLParserParams> ParseFromResourceAsync(string resource, GameObject parent, object host)
        {
            var data = await Readers.ReadResourceAsync(resource);
            var content = Encoding.UTF8.GetString(data, 3, data.Length - 3);
            return BSMLParser.Instance.Parse(content, parent, host);
        }

        public static BSMLParserParams ParseFromResource(string resource, GameObject parent, object host)
        {
            var data = Readers.ReadResource(resource);
            var content = Encoding.UTF8.GetString(data, 3, data.Length - 3);
            return BSMLParser.Instance.Parse(content, parent, host);
        }

        public static void SetSkew(this ImageView image, float skew)
        {
            SkewAcc(ref image) = skew;
        }

        public static void SetGradient(this ImageView image, bool gradient)
        {
            GradientAcc(ref image) = gradient;
            image.SetVerticesDirty();
        }

        public static void AddVrRaycaster(GameObject go, PhysicsRaycasterWithCache raycasterWithCache)
        {
            var isActive = go.activeSelf;
            if (isActive)
            {
                go.SetActive(false);
            }
            var raycaster = go.AddComponent<VRGraphicRaycaster>();
            RaycasterWithCacheAcc(ref raycaster) = raycasterWithCache;
            if (isActive)
            {
                go.SetActive(true);
            }
        }

        public static async Task DoHorizontalTransition(this Transform transform, CancellationToken cancelToken)
        {
            var moveOffset = 20f;
            await AnimationHelper.AsyncAnimation(0.8f, cancelToken, t => { transform.localPosition = new Vector3(moveOffset * (1f - t), 0, 0); });
        }

        public static async Task DoVerticalTransition(this Transform transform, CancellationToken cancelToken)
        {
            var moveOffset = 20f;
            await AnimationHelper.AsyncAnimation(0.8f, cancelToken, t => { transform.localPosition = new Vector3(0, moveOffset * (1f - t), 0); });
        }

        public static async Task DoZTransition(this Transform transform, CancellationToken cancelToken)
        {
            var moveOffset = 20f;
            await AnimationHelper.AsyncAnimation(0.8f, cancelToken, t => { transform.localPosition = new Vector3(0, 0, moveOffset * (1f - t)); });
        }

        public static async Task AnimateIn(this IAnimatableUi animatable, CancellationToken cancelToken)
        {
            if (!(animatable is MonoBehaviour comp))
            {
                return;
            }

            switch (animatable.AnimationType)
            {
                case IAnimatableUi.EAnimationType.Horizontal:
                    await DoHorizontalTransition(comp.transform, cancelToken);
                    break;
                case IAnimatableUi.EAnimationType.Vertical:
                    await DoVerticalTransition(comp.transform, cancelToken);
                    break;
                case IAnimatableUi.EAnimationType.Z:
                    await DoZTransition(comp.transform, cancelToken);
                    break;
            }
        }

        public static RectTransform GetRect(this Transform transform)
        {
            return transform.Cast<RectTransform>();
        }

        public static RectTransform GetRect(this GameObject go)
        {
            return go.transform.Cast<RectTransform>();
        }

        private static readonly FieldAccessor<VRGraphicRaycaster, PhysicsRaycasterWithCache>.Accessor RaycasterWithCacheAcc =
            FieldAccessor<VRGraphicRaycaster, PhysicsRaycasterWithCache>.GetAccessor("_physicsRaycaster");
    }
}