﻿using System.IO;
using UnityEngine;
using Verse;

namespace RimFlix
{
    internal class Dialog_Preview : Window
    {
        private float padding = 12;
        private float headerHeight = 40;
        private float texDim = 80;

        private string path;
        private string name;
        private Texture2D frameTex;

        private Texture tubeTex;
        private Texture flatTex;
        private Texture megaTex;
        private Texture ultraTex;

        private Vector3 tubeVec;
        private Vector3 flatVec;
        private Vector3 megaVec;
        private Vector3 ultraVec;

        private RimFlixSettings settings;

        public Dialog_Preview(string path, string name)
        {
            this.doCloseX = true;
            this.doCloseButton = true;
            this.forcePause = true;
            this.absorbInputAroundWindow = true;

            this.path = path;
            this.name = name;
            this.frameTex = LoadPNG(this.path);

            this.tubeTex = ThingDef.Named("TubeTelevision").graphic.MatSouth.mainTexture;
            this.flatTex = ThingDef.Named("FlatscreenTelevision").graphic.MatSouth.mainTexture;
            this.megaTex = ThingDef.Named("MegascreenTelevision").graphic.MatSouth.mainTexture;
            this.ultraTex = ThingDef.Named("UltrascreenTV").graphic.MatSouth.mainTexture;

            this.tubeVec = ThingDef.Named("TubeTelevision").graphicData.drawSize;
            this.flatVec = ThingDef.Named("FlatscreenTelevision").graphicData.drawSize;
            this.megaVec = ThingDef.Named("MegascreenTelevision").graphicData.drawSize;
            this.ultraVec = ThingDef.Named("UltrascreenTV").graphicData.drawSize;

            this.settings = LoadedModManager.GetMod<RimFlixMod>().GetSettings<RimFlixSettings>();
        }

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(610f, 245f);
            }
        }

        private void DoHeader(Rect rect)
        {
            Vector2 labelSize = Text.CalcSize("RimFlix_PreviewLabel".Translate());
            Rect labelRect = rect.LeftPartPixels(labelSize.x);
            Rect nameRect = rect.RightPartPixels(rect.width - labelRect.width - this.padding / 2);

            GUI.color = Color.gray;
            Widgets.Label(labelRect, "RimFlix_PreviewLabel".Translate());
            GUI.color = Color.white;
            Widgets.Label(nameRect, this.name);
        }

        private void DoMain(Rect rect)
        {
            // Building textures
            float x = rect.x;
            float y = rect.y;
            Rect tubeRect = new Rect(x, y, this.texDim * tubeVec.x, this.texDim * tubeVec.y);
            x += tubeRect.width + this.padding * 2;
            Rect flatRect = new Rect(x, y, this.texDim * flatVec.x, this.texDim * flatVec.y);
            x += flatRect.width + this.padding * 2;
            Rect megaRect = new Rect(x, y, this.texDim * megaVec.x, this.texDim * megaVec.y);
            x += megaRect.width + this.padding * 2;
            Rect ultraRect = new Rect(x, y, this.texDim * ultraVec.x, this.texDim * ultraVec.y);

            GUI.DrawTexture(tubeRect, this.tubeTex);
            GUI.DrawTexture(flatRect, this.flatTex);
            GUI.DrawTexture(megaRect, this.megaTex);
            GUI.DrawTexture(ultraRect, this.ultraTex);

            // Overlay textures
            Rect tubeFrame = new Rect(tubeRect.position, GetSize(this.tubeVec, RimFlixSettings.TubeScale));
            tubeFrame.center = tubeRect.center + this.texDim * RimFlixSettings.TubeOffset;
            Rect flatFrame = new Rect(flatRect.position, GetSize(this.flatVec, RimFlixSettings.FlatScale));
            flatFrame.center = flatRect.center + this.texDim * RimFlixSettings.FlatOffset;
            Rect megaFrame = new Rect(megaRect.position, GetSize(this.megaVec, RimFlixSettings.MegaScale));
            megaFrame.center = megaRect.center + this.texDim * RimFlixSettings.MegaOffset;
            Rect ultraFrame = new Rect(ultraRect.position, GetSize(this.ultraVec, RimFlixSettings.UltraScale));
            ultraFrame.center = ultraRect.center + this.texDim * RimFlixSettings.UltraOffset;

            GUI.DrawTexture(tubeFrame, this.frameTex);
            GUI.DrawTexture(flatFrame, this.frameTex);
            GUI.DrawTexture(megaFrame, this.frameTex);
            GUI.DrawTexture(ultraFrame, this.frameTex);
            // Draw borders on mouseover
            if (Mouse.IsOver(tubeRect))
            {
                Widgets.DrawBox(tubeFrame);
            }
            if (Mouse.IsOver(flatRect))
            {
                Widgets.DrawBox(flatFrame);
            }
            if (Mouse.IsOver(megaRect))
            {
                Widgets.DrawBox(megaFrame);
            }
            if (Mouse.IsOver(ultraRect))
            {
                Widgets.DrawBox(ultraFrame);
            }
        }

        public override void DoWindowContents(Rect rect)
        {
            // Avoid Close button overlap
            rect.yMax -= 50;

            // Header title
            Rect headerRect = rect.TopPartPixels(headerHeight);
            DoHeader(headerRect);

            // Main previews
            Rect mainRect = rect.BottomPartPixels(texDim + this.padding * 4);
            float innerWidth = this.texDim * (this.tubeVec.x + this.flatVec.x + this.megaVec.x + this.ultraVec.x) + this.padding * 4;
            mainRect.xMin += (mainRect.width - innerWidth) / 2;
            mainRect.yMin += this.padding * 2;
            DoMain(mainRect);
        }

        private Vector2 GetSize(Vector2 parentSize, Vector2 scale)
        {
            Vector2 screenSize = Vector2.Scale(parentSize, scale) * this.texDim;
            Vector2 frameSize = new Vector2(this.frameTex.width, this.frameTex.height);
            if (this.settings.DrawType == DrawType.Fit)
            {
                if (frameSize.x / screenSize.x > frameSize.y / screenSize.y)
                {
                    screenSize.y = screenSize.x * frameSize.y / frameSize.x;
                }
                else
                {
                    screenSize.x = screenSize.y * frameSize.x / frameSize.y;
                }
            }
            return screenSize;
        }

        private Texture2D LoadPNG(string filePath)
        {
            Texture2D texture2D = null;
            if (File.Exists(filePath))
            {
                byte[] data = File.ReadAllBytes(filePath);
                texture2D = new Texture2D(2, 2, TextureFormat.Alpha8, true);
                texture2D.LoadImage(data);
                texture2D.Compress(true);
                texture2D.name = Path.GetFileNameWithoutExtension(filePath);
                texture2D.filterMode = FilterMode.Bilinear;
                texture2D.anisoLevel = 2;
                texture2D.Apply(true, true);
            }
            return texture2D;
        }
    }
}