﻿using ColossalFramework.UI;
using UnityEngine;


namespace PloppableRICO
{
    /// <summary>
    /// Panel that contains the building preview image.
    /// </summary>
    public class UIPreviewPanel : UIPanel
    {
        // Panel components.
        private UITextureSprite previewSprite;
        private UISprite noPreviewSprite;
        private UIPreviewRenderer previewRender;
        private UILabel buildingName;
        private UILabel buildingLevel;
        private UILabel buildingSize;

        // Currently selected building and its pre-rendered (by game) equivalent for rendering.
        private BuildingData currentSelection;
        private BuildingInfo renderPrefab;

        // Flag to indicate if we need to render the builing with the next Update().
        private bool queueRender;


        /// <summary>
        /// Render and show a preview of a building.
        /// </summary>
        /// <param name="building">The building to render</param>
        public void Show(BuildingData building)
        {
            // Update current selection to the new building.
            currentSelection = building;
            renderPrefab = (currentSelection == null || currentSelection.name == null) ? null : (PrefabCollection<BuildingInfo>.FindLoaded(currentSelection.name));

            // Generate render if there's a selection with a mesh.
            if (renderPrefab != null && renderPrefab.m_mesh != null)
            {
                // Set default values.
                previewRender.CameraRotation = 30f;
                previewRender.Zoom = 4f;

                // Set mesh and material for render.
                previewRender.SetTarget(renderPrefab);

                // Set background.
                previewSprite.texture = previewRender.Texture;
                noPreviewSprite.isVisible = false;

                // Render at next update.
                queueRender = true;

            }
            else
            {
                // No valid current selection with a mesh; reset background.
                previewSprite.texture = null;
                noPreviewSprite.isVisible = true;
            }

            // Hide any empty building names.
            if (building == null)
            {
                buildingName.isVisible = false;
                buildingLevel.isVisible = false;
                buildingSize.isVisible = false;
            }
            else
            {
                // Set and show building name.
                buildingName.isVisible = true;
                buildingName.text = currentSelection.DisplayName;
                UIUtils.TruncateLabel(buildingName, width - 45);
                buildingName.autoHeight = true;

                // Set and show building level.
                buildingLevel.isVisible = true;
                buildingLevel.text = Translations.Translate("PRR_LEVEL") + " " + Mathf.Min((int)currentSelection.prefab.GetClassLevel() + 1, Util.MaxLevelOf(currentSelection.prefab.GetSubService()));
                UIUtils.TruncateLabel(buildingLevel, width - 45);
                buildingLevel.autoHeight = true;

                // Set and show building size.
                buildingSize.isVisible = true;
                buildingSize.text = currentSelection.prefab.GetWidth() + "x" + currentSelection.prefab.GetLength();
                UIUtils.TruncateLabel(buildingSize, width - 45);
                buildingSize.autoHeight = true;
            }
        }


        /// <summary>
        /// (Re-)render the preview image if we need to.
        /// Doing it in Update avoids the game 'flickering' in the background that can occur if rendering occurs at random times.
        /// Called by Unity every frame.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Don't do anything if a render hasn't been signalled.
            if (!queueRender)
            {
                return;
            }

            // Clear flag.
            queueRender = false;

            // Don't do anything if there's no prefab to render.
            if (renderPrefab == null)
            {
                return;
            }

            // If the selected building has colour variations, temporarily set the colour to the default for rendering.
            if (renderPrefab.m_useColorVariations)
            {
                Color originalColor = renderPrefab.m_material.color;
                renderPrefab.m_material.color = renderPrefab.m_color0;
                previewRender.Render(false);
                renderPrefab.m_material.color = originalColor;
            }
            else
            {
                // No temporary colour change needed.
                previewRender.Render(false);
            }
        }


        /// <summary>
        /// Performs initial setup for the panel; we no longer use Start() as that's not sufficiently reliable (race conditions), and is no longer needed, with the new create/destroy process.
        /// </summary>
        internal void Setup()
        {
            // Set background and sprites.
            backgroundSprite = "GenericPanel";

            previewSprite = AddUIComponent<UITextureSprite>();
            previewSprite.size = size;
            previewSprite.relativePosition = Vector3.zero;

            noPreviewSprite = AddUIComponent<UISprite>();
            noPreviewSprite.size = size;
            noPreviewSprite.relativePosition = Vector3.zero;

            // Initialise renderer; use double size for anti-aliasing.
            previewRender = gameObject.AddComponent<UIPreviewRenderer>();
            previewRender.Size = previewSprite.size * 2;

            // Click-and-drag rotation.
            eventMouseDown += (component, mouseEvent) =>
            {
                eventMouseMove += RotateCamera;
            };

            eventMouseUp += (component, mouseEvent) =>
            {
                eventMouseMove -= RotateCamera;
            };

            // Zoom with mouse wheel.
            eventMouseWheel += (component, mouseEvent) =>
            {
                previewRender.Zoom -= Mathf.Sign(mouseEvent.wheelDelta) * 0.25f;

                // Render at next update.
                queueRender = true;
            };

            // Display building name.
            buildingName = AddUIComponent<UILabel>();
            buildingName.textScale = 0.9f;
            buildingName.useDropShadow = true;
            buildingName.dropShadowColor = new Color32(80, 80, 80, 255);
            buildingName.dropShadowOffset = new Vector2(2, -2);
            buildingName.text = "Name";
            buildingName.isVisible = false;
            buildingName.relativePosition = new Vector3(5, 10);

            // Display building level.
            buildingLevel = AddUIComponent<UILabel>();
            buildingLevel.textScale = 0.9f;
            buildingLevel.useDropShadow = true;
            buildingLevel.dropShadowColor = new Color32(80, 80, 80, 255);
            buildingLevel.dropShadowOffset = new Vector2(2, -2);
            buildingLevel.text = "Level";
            buildingLevel.isVisible = false;
            buildingLevel.relativePosition = new Vector3(5, height - 20);

            // Display building size.
            buildingSize = AddUIComponent<UILabel>();
            buildingSize.textScale = 0.9f;
            buildingSize.useDropShadow = true;
            buildingSize.dropShadowColor = new Color32(80, 80, 80, 255);
            buildingSize.dropShadowOffset = new Vector2(2, -2);
            buildingSize.text = "Size";
            buildingSize.isVisible = false;
            buildingSize.relativePosition = new Vector3(width - 50, height - 20);
        }


        /// <summary>
        /// Rotates the preview camera (model rotation) in accordance with mouse movement.
        /// </summary>
        /// <param name="c">Not used</param>
        /// <param name="p">Mouse event</param>
        private void RotateCamera(UIComponent c, UIMouseEventParameter p)
        {
            previewRender.CameraRotation -= p.moveDelta.x / previewSprite.width * 360f;

            // Render at next update.
            queueRender = true;
        }
    }
}