﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using ModestTree;
using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.Modifiers;
using SaberFactory.UI.Lib;
using SiraUtil;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;
using Zenject;

namespace SaberFactory.UI.CustomSaber.Views.Modifiers
{
    internal class MainModifierPanelView : SubView, INavigationCategoryView
    {
        public ENavigationCategory Category => ENavigationCategory.Modifier;

//#if !PAT
//        protected override string _resourceName => PatViewPath;
//        [UIValue("pat-preview")] private string PatPreview => "https://www.youtube.com/watch?v=xI-YFoPilxU";
//        [UIValue("has-pat-preview")] private bool HasPatPreview => true;
//        [UIValue("additional-pat-info")] private string AdditionalPatInfo => "Feature needs to be implemented on saber";
//#endif

        [UIObject("container")] private readonly GameObject _container = null;
        [UIComponent("component-list")] private readonly CustomListTableData _componentList = null;
        [Inject] private readonly BsmlDecorator _decorator = null;
        [Inject] private readonly EditorInstanceManager _instanceManager = null;
        [Inject] private readonly GizmoAssets _gizmoAssets = null;

        private bool IsNotCustomizable
        {
            get => _isNotCustomizable;
            set
            {
                _isNotCustomizable = value;
                OnPropertyChanged();
            }
        }

        private ModifyableComponentManager _modifyableComponentManager;
        private bool IsAvailable => _modifyableComponentManager?.IsAvailable ?? false;
        private List<BaseModifierImpl> _items;
        private BaseModifierImpl _currentItem;

        private bool _isNotCustomizable;

        public override void DidOpen()
        {
//#if !PAT
//            return;
//#endif

#pragma warning disable CS0162 // Unreachable code detected
            _modifyableComponentManager = _instanceManager.CurrentPiece?.Model.ModifyableComponentManager;

            if (IsAvailable)
            {
                IsNotCustomizable = false;
                _gizmoAssets.Activate();
                SetupMod();
            }
            else
            {
                IsNotCustomizable = true;
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        public override void DidClose()
        {
//#if !PAT
//            return;
//#endif

#pragma warning disable CS0162 // Unreachable code detected
            _modifyableComponentManager = null;

            _componentList.Data = new List<CustomListTableData.CustomCellInfo>();
            _componentList.TableView.ReloadData();
            ClearCurrentView();
            GizmoDrawer.Deactivate();
#pragma warning restore CS0162 // Unreachable code detected
        }

        public void SetupMod()
        {
            var list = new List<CustomListTableData.CustomCellInfo>();
            _items = _modifyableComponentManager.GetAllMods();
            
            foreach (var mod in _items)
            {
                list.Add(new CustomListTableData.CustomCellInfo(mod.Name, mod.TypeName));
            }
            
            _componentList.Data = list;
            _componentList.TableView.ReloadData();
            
            if (_items.Count > 0)
            {
                _componentList.TableView.SelectCellWithIdx(0, true);
            }
        }

        private void ClearCurrentView()
        {
            for (var i = 0; i < _container.transform.childCount; i++)
            {
                Destroy(_container.transform.GetChild(0).gameObject);
            }
        }

        [UIAction("component-selected")]
        private void ComponentSelected(TableView table, int idx)
        {
            if (!IsAvailable)
            {
                return;
            }
            
            _currentItem = _items[idx];
            ClearCurrentView();
            _currentItem.ParserParams = _decorator.ParseFromString(_currentItem.DrawUi(), _container, _currentItem);
            _currentItem.WasSelected();
        }

        [UIAction("reset-click")]
        private void ResetClick()
        {
            if (!IsAvailable)
            {
                return;
            }
            
            if (_modifyableComponentManager is null)
            {
                return;
            }
            
            Debug.LogWarning($"Resetting {_currentItem.Id}");
            
            _modifyableComponentManager.Reset(_currentItem.Id);
            
            ReloadSaber();
        }

        [UIAction("reset-all-click")]
        private void ResetAllClick()
        {
            if (!IsAvailable)
            {
                return;
            }
            
            if (_modifyableComponentManager is null)
            {
                return;
            }
            
            Debug.LogWarning("Resetting all");
            
            _modifyableComponentManager.ResetAll();
            
            ReloadSaber();
        }

        private void ReloadSaber()
        {
            _instanceManager.Refresh();
            DidClose();
            ClearCurrentView();
            DidOpen();
        }

        private void Update()
        {
            if (_currentItem == null)
            {
                return;
            }
            
            _currentItem.OnTick();
        }
    }
}