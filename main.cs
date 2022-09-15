//line and box around target/fight target
//pathing needs to be a different color
//overhaul sit logic
//add instinctive control to buffs
//add iron circle to buffs

using System;
using System.Collections.Generic;
//using System.IO;
//using System.Linq;

using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Core.Movement;
using AOSharp.Common.GameData;
using AOSharp.Core.Inventory;

using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using SmokeLounge.AOtomation.Messaging.Messages;
using System.Text;

namespace sono
{
    public class WorldItem : IEquatable<WorldItem>
    {
        public WorldItem(int id, int itemid, Vector3 color)
        {
            this.id = id;
            this.itemid = itemid;
            this.color = color;
        }
        public int id { get; set; }
        public int itemid { get; set; }
        public Vector3 color {get; set; }

        public bool Equals(WorldItem other)
        {
            if(this.id == other.id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class sono : AOPluginEntry
    {
        Vector3 _pathColor;
        Vector3 _radarColor;
        Vector3 _healthColor;
        Vector3 _emptyColor;
        Vector3 _goodColor;
        Vector3 _badColor;
        public Settings Settings;
        public List<WorldItem> flw = new List<WorldItem>();
        private float _tick;
        private MovementController mc;
        Window sonoWin;
        public override void Run(string pluginDir)
        {
            try
            {
                _pathColor = v3Web(255, 185, 127);
                _radarColor = v3Web(97, 195, 195);
                _healthColor = v3Web(255, 0, 149);
                _emptyColor = v3Web(226, 255, 164);
                _goodColor = v3Web(42, 186, 124);
                _badColor = v3Web(255, 114, 58);
                mc = new MovementController(drawPath: true);
                Settings = new Settings("sono");
                Settings.AddVariable("Vision", true);
                Game.OnUpdate += OnUpdate;
                Network.N3MessageReceived += Network_N3MessageReceived;
                Network.PacketReceived += Network_PacketReceived;
                //Network.N3MessageSent += Network_N3MessageSent;
                Chat.WriteLine("wyd1227");
                Chat.RegisterCommand("openwindow", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    
                    /*
                    int _val;
                    foreach (Stat _stat in (Stat[]) Enum.GetValues(typeof(Stat)))
                    {
                        _val = Targeting.Target.GetStat(_stat); 
                        if (_val != 0 && _val.ToString() != "1234567890")
                        {
                            Chat.WriteLine($"{_stat} = {_val.ToString()}");
                        }
                    }
                    Chat.WriteLine("done");
                    */
                    
                    List<Item> characterItems = Inventory.Items;
                    //List<Item> characterItems = Inventory.Items;
                    /*
                    foreach (Item item in characterItems)
                    {
                        Chat.WriteLine($"{item.Slot} - {item.Id} - {item.Name} - {item.QualityLevel} - {item.UniqueIdentity}");
                    }
                    */
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
                foreach (Mission mission in Mission.List)
                {
                    if(string.Equals(mission.DisplayName, "Famous Last Words"))
                    {
                        //290938 + 6
                        flw.Add(new WorldItem(290632, 290938, _emptyColor));
                        flw.Add(new WorldItem(290633, 290939, _emptyColor));
                        flw.Add(new WorldItem(290634, 290940, _emptyColor));
                        flw.Add(new WorldItem(290635, 290941, _emptyColor));
                        flw.Add(new WorldItem(290636, 290942, _emptyColor));
                        flw.Add(new WorldItem(290637, 290943, _emptyColor));
                        flw.Add(new WorldItem(290637, 0, _badColor));
                        flw.Add(new WorldItem(290637, 0, _badColor));
                        flw.Add(new WorldItem(290637, 0, _badColor));
                        foreach (WorldItem _wi in flw) {
                            if (Inventory.Find(_wi.itemid, out Item item)) {
                                _wi.color = _badColor;
                            } else if (_wi.itemid != 0) {
                                _wi.color = _goodColor;
                            }
                        }
                    }
                    /*
                    Chat.WriteLine($"   {mission.Identity}");
                    Chat.WriteLine($"       Ptr: {mission.Pointer.ToString("X4")}");
                    Chat.WriteLine($"       Source: {mission.Source}");
                    Chat.WriteLine($"       Playfield: {(mission.Location != null ? mission.Location.Playfield.ToString() : "NULL")}");
                    Chat.WriteLine($"       WorldPos: {(mission.Location != null ? mission.Location.Pos.ToString() : "NULL")}");
                    Chat.WriteLine($"       DisplayName: {mission.DisplayName}");
                    (Mission:41CC0B38)
                        Ptr: 1AE5DC40
                        Source: (SimpleChar:5595AC03)
                        DisplayName: Famous Last Words
                    */
                }
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        private void OnUpdate(object s, float deltaTime)
        {
            _tick = _tick + 1;
            if (sonoWin != null && sonoWin.IsValid && Targeting.Target != null)
            {
                if (sonoWin.FindView("tText1", out TextView tText1))
                {

                    //string stat2 = Targeting.Target.GetStat((Stat)15).ToString();
                    //tText1.Text = $"{stat2}";
                }
                /*
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
                */
            }
            foreach (SimpleChar character in DynelManager.Characters)
            {
                if (Settings["Vision"].AsBool())
                {
                    float _hpperc = character.GetStat(Stat.PercentRemainingHealth) / 100;
                    float _range = 20 - DynelManager.LocalPlayer.Radius + character.Radius;
                    float _health = character.GetStat(Stat.Health);
                    float _maxhealth = character.GetStat(Stat.MaxHealth);
                    float _healthpc = Math.Max(0, _health / _maxhealth);
                    float _scale = character.GetStat(Stat.CharRadius) * 0.75f;
                    Vector3 v3Up = new Vector3(0, 1, 0);
                    Vector3 v3Delta = DynelManager.LocalPlayer.Position - character.Position;
                    Vector3 v3Norm = new Vector3(v3Delta.X / character.DistanceFrom(DynelManager.LocalPlayer), v3Delta.Y / character.DistanceFrom(DynelManager.LocalPlayer), v3Delta.Z / character.DistanceFrom(DynelManager.LocalPlayer));
                    Debug.DrawSphere(DynelManager.LocalPlayer.Position, DynelManager.LocalPlayer.Radius, _pathColor);
                    Vector3 v3X = Vector3.Cross(v3Up, v3Norm);
                    Vector3 _anchor = new Vector3(0, _scale, 0);
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
                    //diag
                    Debug.DrawLine(character.Position + _anchor - v3X * character.Radius * 2, character.Position + _anchor + new Vector3(0, character.Radius / 2, 0) - v3X * character.Radius * 7 * _healthpc - v3X * character.Radius * 2, _healthColor);
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
            foreach (Dynel _d in DynelManager.AllDynels)
            {
                if (Settings["Vision"].AsBool())
                {
                    int _tempid = _d.GetStat(Stat.ACGItemTemplateID);
                    foreach (WorldItem _wi in flw)
                    {
                        if (_wi.id == _tempid) 
                        {
                            Debug.DrawSphere(_d.Position, 1f, _wi.color);
                        }
                    }
                }
            }
        }
        
        public override void Teardown()
        {
            Chat.WriteLine("Teardown time!");
        }
        private Vector3 v3Web(int r, int g, int b)
        {
            return new Vector3((float)r / 255, (float)g / 255, (float)b / 255);
        }
        private void Network_N3MessageReceived(object s, SmokeLounge.AOtomation.Messaging.Messages.N3Message n3Msg)
        {
            {
                if (n3Msg.N3MessageType == N3MessageType.GenericCmd) {
                    GenericCmdMessage _cmd = (GenericCmdMessage)n3Msg;
                    //Chat.WriteLine($"cmd: {_cmd.Temp1} {_cmd.Temp4} {_cmd.Count} {_cmd.Action} {_cmd.User} {_cmd.Source} {_cmd.Target} {_cmd.Target.Type}");
                } else if (n3Msg.N3MessageType == N3MessageType.TemplateAction) {
                    TemplateActionMessage _temp = (TemplateActionMessage)n3Msg;
                    //Chat.WriteLine($"template: {_temp.ItemLowId} {_temp.ItemHighId} {_temp.Quality} {_temp.Unknown1} {_temp.Unknown2} {_temp.Unknown3} {_temp.Unknown4}");
                    //List<Item> characterItems = Inventory.Items;
                    flw.Find(x => x.itemid == _temp.ItemLowId).color = _badColor;
                } else if (n3Msg.N3MessageType == N3MessageType.CharacterAction) {
                    CharacterActionMessage _act = (CharacterActionMessage)n3Msg;
                    //Chat.WriteLine($"template: a: {_act.Action}, t: {_act.Target}, p1: {_act.Parameter1}, p2: {_act.Parameter2}, u1: {_act.Unknown1}, u2: {_act.Unknown2}");
                    if(_act.Action == SmokeLounge.AOtomation.Messaging.GameData.CharacterActionType.DeleteItem) {
                        foreach (WorldItem _wi in flw) {
                            if (Inventory.Find(_wi.itemid, out Item item)) {
                                _wi.color = _badColor;
                            } else if (_wi.itemid != 0) {
                                _wi.color = _goodColor;
                            }
                        }
                    }
                /*} else if (n3Msg.N3MessageType == N3MessageType.QuestFullUpdate) {
                    Chat.WriteLine("aaaaay");
                    CreateQuestMessage _radiant = (CreateQuestMessage)n3Msg;
                    QuestMessage _radiant2 = (QuestMessage)n3Msg;
                    Chat.WriteLine($"rad_id: {_radiant.MissionId}, act: {_radiant2.Action}, mission: {_radiant2.Mission}, uk1: {_radiant2.Unknown1}, uk2: {_radiant2.Unknown2}, uk3: {_radiant2.Unknown3}");
                */
                } else {
                    //Chat.WriteLine($"n3receive: {n3Msg.N3MessageType} {n3Msg.Identity}");
                }
            }
        }
        private void Network_PacketReceived(object s, byte[] packet)
        {
            N3MessageType msgType = (N3MessageType)((packet[16] << 24) + (packet[17] << 16) + (packet[18] << 8) + packet[19]);
            if (msgType == N3MessageType.QuestFullUpdate) {
                string _fupack = BitConverter.ToString(packet).Replace("-", "");
                string _funohead = _fupack.Substring(114);
                string _titlehex = _funohead.Substring(0, _funohead.IndexOf("00"));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < _titlehex.Length; i += 2)
                {
                    string hs = _titlehex.Substring(i, 2);
                    sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
                }
                string missionTitle = sb.ToString();
                Chat.WriteLine($"QuestFullUpdate: {missionTitle}");
            }
        }
    }
}
