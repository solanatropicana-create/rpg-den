using System;
using System.Globalization;
using System.Text;
using Godot;
using FD.Actors;
using FD.World;

namespace FD.Dev;

/// <summary>
/// Development harness (autoload). User arguments go after "--" on the Godot command line:
/// <code>
/// --shot=/path.png        save a screenshot after warm-up, then quit
/// --warm=N                frames to wait before the screenshot / bench start (default 40)
/// --hour=H                set the game clock (e.g. 19.5)
/// --timescale=K           game seconds per real second (default 60 → 24-minute day; 0 freezes time)
/// --cam=x,y,z:tx,ty,tz    free camera at x,y,z looking at tx,ty,tz (player camera bypassed);
///                         a y written as @h means h metres above the terrain at that x,z (e.g. --cam=10,@1.7,5:0,@1.5,0)
/// --player=x,z[,yawDeg]   teleport the player (ground-snapped); yaw: 0 = south(+z), 90 = east, 180 = north
/// --campitch=deg          player camera pitch (negative looks down), --zoom=m camera distance
/// --fov=deg               camera field of view
/// --bench=N               after warm-up, measure N frames, print FPS / draw calls / primitives / objects, quit
/// --selftest              run headless world checks, print PASS/FAIL, exit with code 0/1
/// --mapdump=/path.png     write the heightfield debug map and quit
/// --dumpmodel=name        print the node tree / animations of assets/models/name.glb and quit
/// --tonemap=agx|filmic|aces  override the tonemapper, --exposure=K multiply exposure (look-dev)
/// --hold=a,b              keep input actions pressed (e.g. --hold=move_forward,sprint) to film motion
/// --noassets              ignore all glb files (procedural placeholders everywhere)
/// --probe=x,z;x,z…        print height, slope, ground cover and distances at points, then quit
/// --seed=N --prehistory=N  macro world seed and prehistory days (Faz 2; default 1 and 1600)
/// --worldtest=N           headless: macro link + N local days (macro steps, names, census from the sim), PASS/FAIL
/// </code>
/// </summary>
public partial class Dev : Node
{
    public static Dev Instance { get; private set; }

    public string ShotPath, MapDumpPath, DumpModel, Tonemap;
    public string[] Hold;
    public string Probe;
    string _camSpec;
    public float? Exposure;
    public int Warm = 40, Bench;
    public float? Hour, TimeScale, CamPitch, Zoom, Fov;
    public Vector3? CamPos, CamTarget;
    public Vector2? PlayerXZ;
    public float? PlayerYawDeg;
    public bool SelfTest;
    public int LifeTestDays;
    /// <summary>Faz 2: macro world seed (--seed=N) and prehistory length (--prehistory=N days; shorter for quick dev shots)</summary>
    public double? Seed;
    public int? Prehistory;
    public int WorldTestDays;
    /// <summary>--fighttest: headless checks of the real-time d20 fight (no region needed)</summary>
    public bool FightTest;
    /// <summary>--party=wizard,cleric: dev companions (level 2) when the party has none</summary>
    public string PartySpec;
    /// <summary>--fight[=seconds]: start a fight with the goblins near the player after warm-up (or at once); --pauseat=T pauses it at fight time T</summary>
    public float? FightAt, PauseAt;
    /// <summary>--autopause=0|1 overrides the setting (tests and shots)</summary>
    public bool? AutoPause;
    /// <summary>--camptest[=N]: headless: walk the player (with --party) into the goblin camp, fight N times, check the results</summary>
    public int CampTest;
    /// <summary>--fate=rob|capture|kill: force what the goblins do after a lost fight (tests)</summary>
    public string Fate;
    /// <summary>--partytest: headless hiring, following, wages, alignment, Tab</summary>
    public bool PartyTest;
    /// <summary>--econtest: headless shops, trade, herbs, loot, inn, reward, cure</summary>
    public bool EconTest;
    /// <summary>--panel=inv|inn|smith|priest: open that panel after warm-up (shots)</summary>
    public string Panel;
    /// <summary>--gtest: headless talk, rumours, board, NPC heroes fighting the camp</summary>
    public bool GTest;
    /// <summary>--savetest: iron-mode slot round trip</summary>
    public bool SaveTest;
    /// <summary>--mood=festival|shortage|plague|monsters: force the village's state (street scenes)</summary>
    public string Mood;
    /// <summary>--scenario (with --newgame=…): the brief's acceptance scenario, headless</summary>
    public bool Scenario;
    bool _saveTestStarted;
    /// <summary>--play: skip the title menu (dev; a default character is made)</summary>
    public bool Play;
    /// <summary>--ui=menu|create: stay in the boot UI on that screen (with --shot: screenshot it)</summary>
    public string UiScreen;
    /// <summary>--newgame=race,class[,seed[,name]]: run the boot flow headless (creation → world → save → region)</summary>
    public string NewGame;
    /// <summary>tests, shots and tools go straight to the region (Boot skips the menu)</summary>
    public bool SkipMenu => UiScreen == null && (Play || SelfTest || LifeTestDays > 0 || WorldTestDays > 0 || CampTest > 0 || PartyTest || EconTest || GTest || SaveTest || ShotPath != null || Bench > 0 || MapDumpPath != null
                            || DumpModel != null || Probe != null || Follow != null || CamPos.HasValue);
    public string Follow, CardFor;
    public bool DebugHud, OpenMap;
    public float FollowDist = 5f, FollowHeight = 1.9f, FollowAngle = 35f;

    int _frames;
    bool _regionHooked, _done;
    Camera3D _freeCam;
    double _benchStart;
    int _benchFrameStart;
    ulong _benchUsec;

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
        Parse(OS.GetCmdlineUserArgs());
        if (Hour.HasValue) GameClock.SetHour(Hour.Value);
        if (TimeScale.HasValue) GameClock.TimeScale = TimeScale.Value;
        if (ShotPath != null || Bench > 0) GameClock.TimeScale = TimeScale ?? 0f;   // deterministic shots
        Region.Built += OnRegionReady;
        if (FightTest) CallDeferred(nameof(RunFightTest));
    }

    void RunFightTest() => FD.Dev.FightTest.Run(this);

    void Parse(string[] args)
    {
        var ci = CultureInfo.InvariantCulture;
        float F(string s) => float.Parse(s, ci);
        foreach (var a in args)
        {
            int eq = a.IndexOf('=');
            string key = eq > 0 ? a[..eq] : a, val = eq > 0 ? a[(eq + 1)..] : "";
            try
            {
                switch (key)
                {
                    case "--shot": ShotPath = val; break;
                    case "--mapdump": MapDumpPath = val; break;
                    case "--dumpmodel": DumpModel = val; break;
                    case "--tonemap": Tonemap = val; break;
                    case "--hold": Hold = val.Split(','); break;
                    case "--noassets": FD.Core.Models.ForceMissing = true; break;
                    case "--probe": Probe = val; break;
                    case "--exposure": Exposure = F(val); break;
                    case "--warm": Warm = int.Parse(val, ci); break;
                    case "--hour": Hour = F(val); break;
                    case "--timescale": TimeScale = F(val); break;
                    case "--bench": Bench = int.Parse(val, ci); break;
                    case "--selftest": SelfTest = true; break;
                    case "--lifetest": LifeTestDays = val == "" ? 2 : int.Parse(val, ci); break;
                    case "--seed": Seed = double.Parse(val, ci); break;
                    case "--prehistory": Prehistory = int.Parse(val, ci); break;
                    case "--worldtest": WorldTestDays = val == "" ? 3 : int.Parse(val, ci); break;
                    case "--play": Play = true; break;
                    case "--fighttest": FightTest = true; break;
                    case "--party": PartySpec = val; break;
                    case "--fight": FightAt = val == "" ? 0f : F(val); break;
                    case "--pauseat": PauseAt = F(val); break;
                    case "--autopause": AutoPause = val != "0"; break;
                    case "--camptest": CampTest = val == "" ? 1 : int.Parse(val, ci); break;
                    case "--fate": Fate = val; break;
                    case "--partytest": PartyTest = true; break;
                    case "--econtest": EconTest = true; break;
                    case "--panel": Panel = val; break;
                    case "--gtest": GTest = true; break;
                    case "--savetest": SaveTest = true; break;
                    case "--mood": Mood = val; break;
                    case "--weather": Weather.Force = val; break;
                    case "--scenario": Scenario = true; break;
                    case "--ui": UiScreen = val; break;
                    case "--newgame": NewGame = val; UiScreen ??= "newgame"; break;
                    case "--follow": Follow = val; break;
                    case "--followcam":
                    {
                        var p = val.Split(',');
                        FollowDist = F(p[0]); if (p.Length > 1) FollowHeight = F(p[1]); if (p.Length > 2) FollowAngle = F(p[2]);
                        break;
                    }
                    case "--card": CardFor = val; break;
                    case "--debughud": DebugHud = true; break;
                    case "--map": OpenMap = true; break;
                    case "--campitch": CamPitch = F(val); break;
                    case "--zoom": Zoom = F(val); break;
                    case "--fov": Fov = F(val); break;
                    case "--cam":
                        _camSpec = val;
                        CamPos = Vector3.Zero;   // resolved against the terrain once the region exists
                        break;
                    case "--player":
                    {
                        var p = val.Split(',');
                        PlayerXZ = new Vector2(F(p[0]), F(p[1]));
                        if (p.Length > 2) PlayerYawDeg = F(p[2]);
                        break;
                    }
                }
            }
            catch (Exception e) { GD.PushError($"Dev: bad argument '{a}': {e.Message}"); }
        }
    }

    void OnRegionReady(Region region)
    {
        _regionHooked = true;
        if (DumpModel != null)
        {
            var root = FD.Core.Models.Instantiate(DumpModel);
            if (root == null) GD.Print($"[Dev] model {DumpModel} not found");
            else
            {
                void Walk(Node n, int d)
                {
                    string extra = n is MeshInstance3D mi ? $" mesh tris~{(mi.Mesh?.GetFaces().Length ?? 0) / 3} visible={mi.Visible}" : "";
                    if (n is AnimationPlayer ap) extra = " anims: " + string.Join(", ", ap.GetAnimationList());
                    GD.Print(new string(' ', d * 2) + n.Name + " : " + n.GetClass() + extra);
                    foreach (var c in n.GetChildren()) Walk(c, d + 1);
                }
                Walk(root, 0);
                root.Free();
            }
            GetTree().Quit();
            return;
        }
        var hf = region.Heightfield;
        if (MapDumpPath != null)
        {
            hf.SaveDebugMap(MapDumpPath, 1200);
            GD.Print($"[Dev] map written to {MapDumpPath}");
            GetTree().Quit();
            return;
        }
        if (Probe != null)
        {
            var ci = CultureInfo.InvariantCulture;
            foreach (var pt in Probe.Split(';'))
            {
                var c = pt.Split(',');
                float x = float.Parse(c[0], ci), z = float.Parse(c[1], ci);
                float sd = hf.StreamDistance(x, z, out float s);
                GD.Print($"[Probe] ({x:F1},{z:F1}) h={hf.Height(x, z):F2} slope={hf.Slope(x, z):F1}° road={hf.RoadDistance(x, z):F1} " +
                         $"trail={hf.TrailDistance(x, z):F1} stream={sd:F1}(s={s:F0}) forest={hf.ForestDensity(x, z):F2} {hf.Biome(x, z)}");
            }
            GetTree().Quit();
            return;
        }
        var player = region.Player;
        if (PlayerXZ.HasValue && player != null)
        {
            float yaw = PlayerYawDeg.HasValue ? Mathf.DegToRad(PlayerYawDeg.Value) : RegionSpec.YawFacing(RegionSpec.PlayerStartFacing);
            player.Teleport(PlayerXZ.Value, yaw, hf);
        }
        if (player != null && (CamPitch.HasValue || Zoom.HasValue))
            player.SetCameraAngles(player.CameraYaw, Mathf.DegToRad(CamPitch ?? -16f), Zoom ?? player.Zoom);
        if (Fov.HasValue && player != null) player.Camera.Fov = Fov.Value;

        if (_camSpec != null)
        {
            var ci = CultureInfo.InvariantCulture;
            Vector3 P(string spec)
            {
                var c = spec.Split(',');
                float x = float.Parse(c[0], ci), z = float.Parse(c[2], ci);
                float y = c[1].StartsWith("@") ? hf.Height(x, z) + float.Parse(c[1][1..], ci) : float.Parse(c[1], ci);
                return new Vector3(x, y, z);
            }
            var parts = _camSpec.Split(':');
            CamPos = P(parts[0]);
            if (parts.Length > 1) CamTarget = P(parts[1]);
        }
        if (Hold != null) foreach (var a in Hold) if (InputMap.HasAction(a)) Input.ActionPress(a);
        if (CamPos.HasValue)
        {
            _freeCam = new Camera3D { Name = "DevCamera", Fov = Fov ?? 70f, Near = 0.1f, Far = 3000f };
            region.AddChild(_freeCam);
            _freeCam.GlobalPosition = CamPos.Value;
            if (CamTarget.HasValue) _freeCam.LookAt(CamTarget.Value, Vector3.Up);
            _freeCam.MakeCurrent();
            if (player != null) player.InputEnabled = false;
            // keep the player near the camera so near-field systems (grass) are consistent
        }
        if (Tonemap != null)
            region.DayNight.Env.TonemapMode = Tonemap switch
            {
                "filmic" => Godot.Environment.ToneMapper.Filmic, "aces" => Godot.Environment.ToneMapper.Aces,
                "linear" => Godot.Environment.ToneMapper.Linear, _ => Godot.Environment.ToneMapper.Agx,
            };
        if (Exposure.HasValue) region.DayNight.ExposureScale = Exposure.Value;
        if (SelfTest) CallDeferred(nameof(RunSelfTest));
        if (LifeTestDays > 0) CallDeferred(nameof(RunLifeTest));
        if (WorldTestDays > 0) CallDeferred(nameof(RunWorldTest));
        if (AutoPause.HasValue) FD.Game.Settings.Current.AutoPause = AutoPause.Value;
        if (region.Combat != null && PauseAt.HasValue) region.Combat.PauseAt = PauseAt;
        if (region.Combat != null && Fate != null) region.Combat.ForceFate = Fate;
        if (CampTest > 0) CallDeferred(nameof(RunCampTest));
        if (PartyTest) CallDeferred(nameof(RunPartyTest));
        if (EconTest) CallDeferred(nameof(RunEconTest));
        if (GTest) CallDeferred(nameof(RunGTest));
        if (SaveTest && !_saveTestStarted) { _saveTestStarted = true; CallDeferred(nameof(RunSaveTest)); }
        if (Scenario) CallDeferred(nameof(RunScenario));
        if (DebugHud) region.Hud?.SetDebug(true);
        if (OpenMap) region.Hud?.ToggleMap();
    }

    void RunLifeTest() => FD.Dev.LifeTest.Run(this, Region.Current, LifeTestDays);
    void RunWorldTest() => FD.Dev.WorldTest.Run(this, Region.Current, WorldTestDays);
    void RunCampTest() => FD.Dev.CampTest.Run(this, Region.Current, CampTest);
    void RunPartyTest() => FD.Dev.PartyTest.Run(this, Region.Current);
    void RunEconTest() => FD.Dev.EconTest.Run(this, Region.Current);
    void RunGTest() => FD.Dev.GTest.Run(this, Region.Current);
    void RunSaveTest() => FD.Dev.SaveTest.Run(this, Region.Current);
    void RunScenario() => FD.Dev.ScenarioTest.Run(this, Region.Current);

    void OpenPanelDev()
    {
        var r = Region.Current; if (r == null) return;
        var me = r.Player.Character;
        me.Inv.Silver += 60; me.Inv.Add("herb", 5); me.Inv.Add("trinket", 2); me.Inv.Add("scimitar"); me.Inv.Add("potion");
        switch (Panel)
        {
            case "inv": r.Inventory.Toggle(); break;
            case "inn": r.Trade.Open(FD.Game.Economy.Open(r.Session, FD.Game.ShopKind.Inn, "Hancı")); break;
            case "smith": r.Trade.Open(FD.Game.Economy.Open(r.Session, FD.Game.ShopKind.Smith, "Demirci")); break;
            case "priest": r.Trade.Open(FD.Game.Economy.Open(r.Session, FD.Game.ShopKind.Priest, "Rahip")); break;
        }
    }

    /// <summary>--fight: start a fight with the goblins near the player (the camp's if none are near: they are brought over)</summary>
    void DevFight()
    {
        var r = Region.Current;
        if (r?.Combat == null || r.Combat.Active) return;
        var pp = new System.Numerics.Vector2(r.Player.GlobalPosition.X, r.Player.GlobalPosition.Z);
        FD.Sim.Life.Person first = null;
        foreach (var p in r.Life.People)
            if (p.Role == FD.Sim.Life.Role.Goblin && !p.Dead && p.Present && (first == null || System.Numerics.Vector2.Distance(p.Pos, pp) < System.Numerics.Vector2.Distance(first.Pos, pp))) first = p;
        if (first == null) return;
        if (System.Numerics.Vector2.Distance(first.Pos, pp) > 30f)
        {
            // bring the nearest few over (dev only)
            int n = 0;
            foreach (var p in r.Life.People)
            {
                if (p.Role != FD.Sim.Life.Role.Goblin || p.Dead || !p.Present || n >= 5) continue;
                float a = n * 0.9f;
                p.Pos = pp + new System.Numerics.Vector2(MathF.Sin(a) * 9f, -7f - MathF.Cos(a) * 4f);
                p.Motion = FD.Sim.Life.Motion.Doing;
                n++;
            }
        }
        r.Combat.Start(first, $"{first.Name} saldırdı!");
    }

    /// <summary>--follow=Name: keep the free camera (or the player) near that person, looking at them.</summary>
    void FollowPerson()
    {
        var life = FD.Life.LifeWorld.Instance;
        if (life == null || Follow == null) return;
        FD.Sim.Life.Person who = null;
        foreach (var p in life.Sim.People)
            if (p.FullName.Contains(Follow, StringComparison.OrdinalIgnoreCase) || p.Name.Contains(Follow, StringComparison.OrdinalIgnoreCase)) { who = p; break; }
        if (who == null) return;
        var a = life.ActorOf(who);
        if (a == null) return;
        var region = Region.Current;
        Vector3 t = a.Visible ? a.GlobalPosition : new Vector3(who.Pos.X, region.Heightfield.Height(who.Pos.X, who.Pos.Y), who.Pos.Y);
        float ang = Mathf.DegToRad(FollowAngle) + MathF.Atan2(who.Dir.X, who.Dir.Y);
        var camPos = t + new Vector3(MathF.Sin(ang) * FollowDist, FollowHeight, MathF.Cos(ang) * FollowDist);
        float gy = region.Heightfield.Height(camPos.X, camPos.Z) + 0.6f;
        if (camPos.Y < gy) camPos.Y = gy;
        if (_freeCam == null)
        {
            _freeCam = new Camera3D { Name = "FollowCamera", Fov = Fov ?? 60f, Near = 0.08f, Far = 3000f };
            region.AddChild(_freeCam);
            _freeCam.MakeCurrent();
        }
        _freeCam.GlobalPosition = camPos;
        _freeCam.LookAt(t + Vector3.Up * 1.0f, Vector3.Up);
        // keep the player (near-field grass, interaction) next to the camera but out of shot
        if (region.Player != null) { region.Player.InputEnabled = false; region.Player.GlobalPosition = camPos - new Vector3(0, 1.6f, 0) + (camPos - t).Normalized() * 1.5f; }
        if (CardFor != null && _frames == Warm - 3) region.Hud?.OpenCardFor(who);
    }

    void RunSelfTest() => FD.Dev.SelfTest.Run(this, Region.Current);

    public override void _Process(double delta)
    {
        if (_done || !_regionHooked) return;
        _frames++;
        if (FightAt is float fa && _frames == Math.Max(2, (int)(fa * 30))) DevFight();
        if (Panel != null && _frames == Math.Max(3, Warm - 6)) OpenPanelDev();
        if (Follow != null) FollowPerson();
        else if (CardFor != null && _frames == Warm - 3)
        {
            var life = FD.Life.LifeWorld.Instance;
            if (life != null) foreach (var p in life.Sim.People) if (p.FullName.Contains(CardFor, StringComparison.OrdinalIgnoreCase)) { Region.Current.Hud?.OpenCardFor(p); break; }
        }
        if (Bench > 0)
        {
            if (_frames == Warm) { _benchStart = Time.GetTicksMsec() / 1000.0; _benchFrameStart = _frames; _benchUsec = Time.GetTicksUsec(); }
            if (_frames == Warm + Bench) { PrintBench(); _done = true; GetTree().Quit(); }
            return;
        }
        if (ShotPath != null && _frames == Warm)
        {
            var img = GetViewport().GetTexture().GetImage();
            var err = img.SavePng(ShotPath);
            GD.Print($"[Dev] screenshot {ShotPath} {img.GetSize()} ({err}) at {GameClock.TimeString}");
            _done = true;
            GetTree().Quit();
        }
    }

    void PrintBench()
    {
        double secs = (Time.GetTicksUsec() - _benchUsec) / 1e6;
        int frames = _frames - _benchFrameStart;
        var sb = new StringBuilder();
        var cam = GetViewport().GetCamera3D();
        sb.Append($"[Bench] frames={frames} time={secs:F2}s avgFPS={frames / secs:F2} engineFPS={Engine.GetFramesPerSecond():F1}");
        sb.Append($" drawCalls={Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame)}");
        sb.Append($" primitives={Performance.GetMonitor(Performance.Monitor.RenderTotalPrimitivesInFrame)}");
        sb.Append($" objects={Performance.GetMonitor(Performance.Monitor.RenderTotalObjectsInFrame)}");
        sb.Append($" videoMemMB={Performance.GetMonitor(Performance.Monitor.RenderVideoMemUsed) / 1048576.0:F0}");
        sb.Append($" nodes={Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)}");
        sb.Append($" physicsMs={Performance.GetMonitor(Performance.Monitor.TimePhysicsProcess) * 1000:F2}");
        sb.Append($" processMs={Performance.GetMonitor(Performance.Monitor.TimeProcess) * 1000:F2}");
        var reg = Region.Current;
        if (reg != null)
            sb.Append($" vegInstances={reg.Vegetation?.TotalInstances} trees={reg.Vegetation?.TreeCount} grassTiles={reg.Grass?.TileCount} grassInstances={reg.Grass?.LiveInstances}");
        if (cam != null) sb.Append($" cam={cam.GlobalPosition.X:F0},{cam.GlobalPosition.Y:F0},{cam.GlobalPosition.Z:F0}");
        sb.Append($" lifeSimMs={FD.Life.LifeWorld.SimMs:F2} actorSyncMs={FD.Life.LifeWorld.SyncMs:F2} lifeOtherMs={FD.Life.LifeWorld.OtherMs:F2}");
        GD.Print(sb.ToString());
    }
}
