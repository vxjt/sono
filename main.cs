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
        private float _tick;
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
                        if (sonoWin.FindView("tText1", out TextView tText1))
                        {
                            tText1.Text = "xx";
                        }
                        if (sonoWin.FindView("tText2", out TextView tText2))
                        {
                            tText2.Text = "yy";
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
            _tick = _tick + 1;
            if (sonoWin != null && sonoWin.IsValid && Targeting.TargetChar != null)
            {
                if (sonoWin.FindView("tText1", out TextView tText1))
                {
                    string stat1 = Targeting.TargetChar.GetStat(Stat.Height).ToString();
                    string stat2 = Targeting.TargetChar.Radius.ToString();
                    string stat4 = Targeting.TargetChar.GetStat((Stat)889).ToString();
                    float stata = Targeting.TargetChar.GetStat(Stat.PercentRemainingHealth);

                    
                    tText1.Text = $"{stata}";
                }
                if (sonoWin.FindView("tText2", out TextView tText2))
                {
                    string _statmd = Targeting.TargetChar.GetStat(Stat.MonsterData).ToString();
                    string _statrs = Targeting.TargetChar.Runspeed.ToString();
                    string _statradius = Targeting.TargetChar.GetStat(Stat.CharRadius).ToString();
                    string _statradius2 = Targeting.TargetChar.Radius.ToString();
                    string _statdist = Targeting.TargetChar.DistanceFrom(DynelManager.LocalPlayer).ToString();
                    string _logidist = Targeting.TargetChar.GetLogicalRangeToTarget(DynelManager.LocalPlayer).ToString();
                    tText2.Text = $"md: {_statmd} rs: {_statrs} radius: {_statradius} radius2: {_statradius2} _statdist {_statdist} logic: {_logidist}";
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
            foreach (SimpleChar character in DynelManager.Characters)
            {
                if (Settings["Vision"].AsBool())
                {
                    Vector3 _pathColor = v3Web(255, 185, 127);
                    Vector3 _radarColor = v3Web(97, 195, 195);
                    Vector3 _healthColor = v3Web(255, 61, 61);
                    Vector3 _emptyColor = v3Web(255, 214, 214);
                    float _hpperc = character.GetStat(Stat.PercentRemainingHealth) / 100;
                    float _range = 20 - DynelManager.LocalPlayer.Radius + character.Radius;
                    float _health = character.GetStat(Stat.Health);
                    float _maxhealth = character.GetStat(Stat.MaxHealth);
                    float _healthpc = Math.Max(0, _health / _maxhealth);
                    Vector3 _anchor = new Vector3(0, character.Radius * 7, 0);
                    Vector3 v3Up = new Vector3(0, 1, 0);
                    Vector3 v3Delta = DynelManager.LocalPlayer.Position - character.Position;
                    Vector3 v3Norm = new Vector3(v3Delta.X / character.DistanceFrom(DynelManager.LocalPlayer), v3Delta.Y / character.DistanceFrom(DynelManager.LocalPlayer), v3Delta.Z / character.DistanceFrom(DynelManager.LocalPlayer));
                    Debug.DrawSphere(DynelManager.LocalPlayer.Position, DynelManager.LocalPlayer.Radius, _pathColor);
                    Vector3 v3X = Vector3.Cross(v3Up, v3Norm);
                    //Debug.DrawLine(character.Position, character.Position + v3Norm * character.GetStat(Stat.CharRadius), DebuggingColor.Blue);
                    //Debug.DrawLine(character.Position, character.Position + v3Up, DebuggingColor.Green);
                    //Debug.DrawLine(character.Position, character.Position + v3X * character.GetStat(Stat.CharRadius), DebuggingColor.Yellow);
                    //low horiz
                    Debug.DrawLine(character.Position + _anchor - v3X * character.Radius * 7 * _healthpc - v3X * character.Radius * 2, character.Position + _anchor - v3X * character.Radius * 9, _emptyColor);
                    Debug.DrawLine(character.Position + _anchor - v3X * character.Radius * 2, character.Position + _anchor - v3X * character.Radius * 7 * _healthpc - v3X * character.Radius * 2, _healthColor);
                    
                    //high horiz
                    Debug.DrawLine(character.Position + _anchor + new Vector3(0, character.Radius / 2, 0) - v3X * character.Radius * 2, character.Position + _anchor + new Vector3(0, character.Radius / 2, 0) - v3X * character.Radius * 7 * _healthpc - v3X * character.Radius * 2, _healthColor);
                    Debug.DrawLine(character.Position + _anchor + new Vector3(0, character.Radius / 2, 0) - v3X * character.Radius * 7 * _healthpc - v3X * character.Radius * 2, character.Position + _anchor + new Vector3(0, character.Radius / 2, 0) - v3X * character.Radius * 9, _emptyColor);
                    //left vert
                    Debug.DrawLine(character.Position + _anchor - v3X * character.Radius * 2, character.Position + _anchor + new Vector3(0, character.Radius / 2, 0) - v3X * character.Radius * 2, _healthColor);
                    //right verts
                    Debug.DrawLine(character.Position + _anchor - v3X * character.Radius * 9, character.Position + _anchor + new Vector3(0, character.Radius / 2, 0) - v3X * character.Radius * 9, _emptyColor);   
                    Debug.DrawLine(character.Position + _anchor - v3X * character.Radius * 7 * _healthpc - v3X * character.Radius * 2, character.Position + _anchor + new Vector3(0, character.Radius / 2, 0) - v3X * character.Radius * 7 * _healthpc - v3X * character.Radius * 2, _healthColor);
               
                    //Debug.DrawLine(character.Position + _anchor - v3X * character.Radius * 2, character.Position + _anchor - v3X * character.Radius * (7 + 2), _emptyColor);
                    if(character.IsNpc && !character.IsAttacking)
                    {
                        Debug.DrawLine(character.Position, character.DistanceFrom(DynelManager.LocalPlayer) < _range ? DynelManager.LocalPlayer.Position : character.Position + v3Norm * _range, _radarColor);
                    }
                     if (character.IsPathing)
                    {
                        Debug.DrawLine(character.Position, character.PathingDestination, _pathColor);
                        Debug.DrawSphere(character.PathingDestination + v3Up * (Convert.ToSingle(Math.Sin(_tick * character.Runspeed / 1000)) / 2f + 1.5f), 0.4f, _pathColor);
                    }
                }
            }
        }
        private Vector3 v3Web(int r, int g, int b)
        {
            return new Vector3((float)r / 255, (float)g / 255, (float)b / 255);
        }
        public override void Teardown()
        {
            Chat.WriteLine("Teardown time!");
        }
    }/*
    public Tuple<Vector3, Vector3, Vector3> Aggro(SimpleChar _them, SimpleChar _us){
        return (Vector3.Up, Vector3.Up, Vector3.Up);
    }*/
}
