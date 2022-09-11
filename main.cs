using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Core.Movement;
using AOSharp.Common.GameData;


namespace sono
{
    public class sono : AOPluginEntry
    {
        public Settings Settings;
        private MovementController mc;
        Window sonoWin;
        public override void Run(string pluginDir)
        {
            try
            {
                mc = new MovementController(drawPath: true);
                Settings = new Settings("sono");
                Settings.AddVariable("Vision", true);
                Game.OnUpdate += OnUpdate;
                Chat.RegisterCommand("openwindow", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    sonoWin = Window.CreateFromXml("MainWindow", $"{pluginDir}\\main.xml");
                    sonoWin.Show(true);
                    if (sonoWin.IsValid)
                    {
                        if (sonoWin.FindView("tText", out TextView testView))
                        {
                            testView.Text = "1337";
                        }
                        if (sonoWin.FindView("pbhealth", out PowerBarView pbHealth))
                        {
                            pbHealth.Value = 100;
                        }
                        if (sonoWin.FindView("pbmana", out PowerBarView pbMana))
                        {
                            pbMana.Value = 100;
                        }
                        if (sonoWin.FindView("ocvision", out View ocVision))
                        {
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        private void OnUpdate(object s, float deltaTime)
        {
            if (sonoWin != null && sonoWin.IsValid)
            {
                if (sonoWin.FindView("testTextView", out TextView testView))
                {
                    //testView.Text = Targeting.TargetChar.GetStat(Stat.MonsterData).ToString();
                }
                if (sonoWin.FindView("pbhealth", out PowerBarView pbHealth))
                {
                    if (Targeting.TargetChar == null)
                    {
                        pbHealth.Value = 0;
                    }
                    else
                    {
                        pbHealth.Value = Targeting.TargetChar.HealthPercent / 100;
                        pbHealth.SetLabel($"{Targeting.TargetChar.Health} / {Targeting.TargetChar.MaxHealth}");
                    }
                }
                if (sonoWin.FindView("pbmana", out PowerBarView pbMana))
                {
                    if (Targeting.TargetChar == null)
                    {
                        pbMana.Value = 0;
                    }
                    else
                    {
                        pbMana.Value = Targeting.TargetChar.NanoPercent / 100;
                        pbMana.SetLabel($"{Targeting.TargetChar.Nano} / {Targeting.TargetChar.MaxNano}, {Targeting.TargetChar.NanoPercent}");
                    }
                }
            }
            Vector3 testa = new Vector3(0, DynelManager.LocalPlayer.GetAttackRange() / 50, 0);
            //Debug.DrawSphere(DynelManager.LocalPlayer.Position + testa, 0.3f, DebuggingColor.LightBlue);
            foreach (SimpleChar character in DynelManager.Characters)
            {
                if (Settings["Vision"].AsBool())
                {
                    Vector3 v3Up = new Vector3(0, 1, 0);
                    Vector3 v3Scale = new Vector3(0, character.GetStat(Stat.Scale) / 100, 0);
                    Vector3 v3Shift = new Vector3(-character.GetStat(Stat.CharRadius), 0, 0);
                    Vector3 v3Delta = DynelManager.LocalPlayer.Position - character.Position;
                    double dubLength = Math.Sqrt(v3Delta.X * v3Delta.X + v3Delta.Y * v3Delta.Y + v3Delta.Z * v3Delta.Z);
                    Vector3 v3Norm = new Vector3(v3Delta.X / dubLength, v3Delta.Y / dubLength, v3Delta.Z / dubLength);
                    Vector3 v3X = Vector3.Cross(v3Up, v3Norm);

                    //Debug.DrawLine(character.Position + v3Scale, character.Position + v3Norm * character.GetStat(Stat.CharRadius) + v3Scale, DebuggingColor.Blue);
                    //Debug.DrawLine(character.Position + v3Scale, character.Position + v3Up * character.GetStat(Stat.CharRadius) + v3Scale, DebuggingColor.Green);
                    //Debug.DrawLine(character.Position + v3Scale, character.Position + v3X * character.GetStat(Stat.CharRadius) + v3Scale, DebuggingColor.Yellow);
                    Debug.DrawLine(character.Position + v3Scale * 2f - v3X * character.GetStat(Stat.CharRadius) * 0.9f, character.Position + v3Scale * 2f - v3X * character.GetStat(Stat.CharRadius) * 0.2f, DebuggingColor.Red);
                    Debug.DrawLine(character.Position + v3Scale * 2.1f - v3X * character.GetStat(Stat.CharRadius) * 0.9f, character.Position + v3Scale * 2.1f - v3X * character.GetStat(Stat.CharRadius) * 0.2f, DebuggingColor.Red);
                    Debug.DrawLine(character.Position + v3Scale * 2f - v3X * character.GetStat(Stat.CharRadius) * 0.9f, character.Position + v3Scale * 2f - v3X * character.GetStat(Stat.CharRadius) * 0.9f + v3Up * 0.1f, DebuggingColor.Red);
                    Debug.DrawLine(character.Position + v3Scale * 2f - v3X * character.GetStat(Stat.CharRadius) * 0.2f, character.Position + v3Scale * 2f - v3X * character.GetStat(Stat.CharRadius) * 0.2f + v3Up * 0.1f, DebuggingColor.Red);
                    if (character.IsPathing)
                    {
                        Debug.DrawLine(character.Position + v3Scale, character.PathingDestination + v3Scale, DebuggingColor.LightBlue);
                        Debug.DrawSphere(character.PathingDestination + v3Scale, 0.2f, DebuggingColor.LightBlue);
                    }
                }
            }
        }

        public override void Teardown()
        {
            Chat.WriteLine("Teardown time!");
        }
    }
}
