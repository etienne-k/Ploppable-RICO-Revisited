using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Math;
using ColossalFramework.Plugins;
using System;
using UnityEngine;
using ICities;
using ColossalFramework.UI;

namespace PloppableAI
{
	public class PloppableExperiment : IUserMod 
	{

		public string Name 
		{
			get { return "PloppableRCI"; }
		}

		public string Description 
		{
			get { return "Adds Ploppable RCI"; }
		}
	}

	//This is my current attempts at making the UI button with some code copied from various mods. Still learning how it all works. 

	public class PloppableTool : ToolBase
	{
		UIButton mainButton;
		BuildingInfo BuildingIm;
		UIScrollablePanel BuildingPanel;
		UIButton BuildingB;
		UITabContainer Tabs;
		bool m_active;
	
		protected override void Awake()
		{
			//this.m_dataLock = new object();
			this.m_active = false;
			base.Awake();
		}

		public void InitGui () {

			mainButton = UIView.GetAView().FindUIComponent<UIButton>("MarqueeBulldozer");

			if (mainButton == null) {
				
				var RCIButton = UIView.GetAView().FindUIComponent<UIMultiStateButton>("BulldozerButton");

				mainButton = RCIButton.parent.AddUIComponent<UIButton>();
				mainButton.name = "RCIButton";
				mainButton.size = new Vector2(36, 36);
				mainButton.normalBgSprite = "ZoningOptionMarquee";
				mainButton.focusedFgSprite = "ToolbarIconGroup6Focused";
				mainButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
				mainButton.relativePosition = new Vector2
					(
						RCIButton.relativePosition.x + RCIButton.width / 2.0f - mainButton.width - RCIButton.width,
						RCIButton.relativePosition.y + RCIButton.height / 2.0f - mainButton.height / 2.0f
					);
				

				mainButton.eventClick += buttonClicked;

				BuildingPanel = UIView.GetAView().FindUIComponent("TSBar").AddUIComponent<UIScrollablePanel>();

				BuildingPanel.backgroundSprite = "SubcategoriesPanel";
				BuildingPanel.isVisible = false;
				BuildingPanel.name = "BuildingPanel";
				BuildingPanel.size = new Vector2(400, 115);
				BuildingPanel.relativePosition = new Vector2
					(
						RCIButton.relativePosition.x +RCIButton.width / 2.0f - BuildingPanel.width ,
						RCIButton.relativePosition.y - BuildingPanel.height 
					);

				/*
				int num3 = PrefabCollection<BuildingInfo>.LoadedCount();
				for (int num = 1; num <= num3; num++) { 

					string namer = PrefabCollection<BuildingInfo>.PrefabName ((uint)num);
					BuildingInfo Holder = PrefabCollection<BuildingInfo>.FindLoaded (namer);

					if (Holder.m_buildingAI is PloppableResidential) {
						BuildingIm = Holder;
					}
				}
				*/



				BuildingIm = PrefabCollection<BuildingInfo>.FindLoaded ("Pittsfield Building.Pittsfield Building_Data");

				Debug.Log (Convert.ToString (BuildingIm.m_Thumbnail));

				BuildingB = BuildingPanel.AddUIComponent<UIButton>();
				BuildingB.name = "BuildingButton";
				BuildingB.size = new Vector2(80, 80);
				BuildingB.atlas = BuildingIm.m_Atlas;
				BuildingB.normalFgSprite = "PittsfieldThumb";
				BuildingB.focusedFgSprite = "PittsfieldThumb" + "Focused";
				BuildingB.hoveredFgSprite = "PittsfieldThumb" + "Hovered";
				BuildingB.pressedFgSprite = "PittsfieldThumb" + "Pressed";
				BuildingB.disabledFgSprite = "PittsfieldThumb" + "Disabled";

				BuildingB.isEnabled = enabled;

				BuildingB.eventClick += BuildingBClicked;

				//Tabs = BuildingPanel.AddUIComponent<UITabContainer> ();
				//Tabs.AddTabPage
			}

		}

		void BuildingBClicked(UIComponent component, UIMouseEventParameter eventParam){
			
			BuildingTool buildingTool = ToolsModifierControl.SetTool<BuildingTool>();
			{
				buildingTool.m_prefab = BuildingIm;
				buildingTool.m_relocate = 0;
			}
		}

		void buttonClicked(UIComponent component, UIMouseEventParameter eventParam)
		{
			this.enabled = true;
			BuildingPanel.isVisible = true;
		}
		protected override void OnDisable()
		{
			if (BuildingPanel != null)
				BuildingPanel.isVisible = false;
			base.OnDisable();
		}
		protected override void OnEnable()
		{
			UIView.GetAView().FindUIComponent<UITabstrip>("MainToolstrip").selectedIndex = -1;
			base.OnEnable();
		}


	}
}