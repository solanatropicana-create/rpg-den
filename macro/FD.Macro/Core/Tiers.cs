using System;
using System.Collections.Generic;

// Faz 1b-3: kademe kapıları. Teknoloji ağacı ve çağlar kalktı ("dünya büyüyerek değil, durum değiştirerek yaşar; büyüyen
// yalnız oyuncudur"). Eskiden bir ağaç düğümüne ya da çağa bağlı olan her şey artık yerleşim kademesine bağlı:
// 0 Kamp, 1 Köy, 2 Kasaba, 3 Şehir (kademe yalnız nüfusla belirlenir; bkz. Sim.TierOf).
//
// Kural: E. çağın düğümü → kademe ≥ E−1 (I. çağın düğümleri baştan açık).
// - Yere bağlı olanlar o yerleşimin kademesine bakar: yapılar (CivicDef.Tier), atölyeler ve demir girdisi
//   (WorkshopDef.Tier / AltTier), çıkarma yapısı seviyeleri (ExtractDef.Tier), tersane, alet aşınması, orman yenilenmesi.
// - Medeniyet çapındakiler medeniyetin kademesine (en büyük yerleşiminin kademesi, Sim.CivTier) bakar: özel birlikler
//   (ClassDef.Perks), gizli yataklar (DepositDef.HiddenTier), gemi türleri, açık deniz, deniz ticareti, yerleşim tavanı.

namespace FD.Macro;

/// <summary>Faz 1b-3: medeniyet çapındaki (ve birkaç yere bağlı) kademe kapıları. Yorumdaki ad eski ağaç düğümüdür.</summary>
public static class Gate
{
    // ---- medeniyet çapında (Sim.CivTier)
    /// <summary>Talim (II): asker yazımı, deri-mızrak piyadesi, 3 kervan muhafızı, lejyon turnuvası</summary>
    public const int TRAINING = 1;
    /// <summary>Yol Yapımı (II): öncü gönderme, başkentten yol döşeme</summary>
    public const int ROADS = 1;
    /// <summary>Takas (II): kara ticaret yolu (pazar yeri de Köy ister)</summary>
    public const int BARTER = 1;
    /// <summary>Bronz Aletler (II): alet talebi</summary>
    public const int TOOLS = 1;
    /// <summary>Arcana I (II): mana talebi (yatağın görünmesi DepositDef.HiddenTier)</summary>
    public const int ARCANA = 1;
    /// <summary>Kervancılık (III): kara ticaret yolu 40 karoya dek</summary>
    public const int CARAVANS = 2;
    /// <summary>Demircilik (III): askerin demir silahı ve zırhı (saldırı +1, asker AC +2)</summary>
    public const int SMITHING = 2;
    /// <summary>Hekimlik (III): salgın koruması, yaralı kurtarma, iksir talebi</summary>
    public const int MEDICINE = 2;
    /// <summary>Gemicilik (III): koga, denizaşırı koloni, ikmal gemileri, kara yolu varken deniz ticareti, korsan avı</summary>
    public const int SHIPBUILDING = 2;
    /// <summary>Seyir (III): açık deniz, keşif gemisi, ikinci koloni, hız ve fırtına koruması</summary>
    public const int NAVIGATION = 2;
    /// <summary>Deniz Ticareti (IV): geniş ambarlı gemiler, uzak deniz yolları</summary>
    public const int SEATRADE = 3;
    /// <summary>Donanma (IV): kadırga, denizden sefer ve akın, filo</summary>
    public const int NAVY = 3;
    /// <summary>Kuşatma Makineleri (IV): kuşatılan savunucuların AC'si −2</summary>
    public const int SIEGE = 3;
    /// <summary>Efsunlama (IV): askerin kılıcı efsunlanır</summary>
    public const int ENCHANTING = 3;
    /// <summary>Mithril İşçiliği (IV): askerlere mithril zırh</summary>
    public const int MITHRILWORK = 3;

    // ---- yere bağlı (yerleşimin kademesi)
    /// <summary>Demircilik (III): L2–L3 çıkarma yapıları aleti yarı hızda aşındırır</summary>
    public const int IRON_TOOLS = 2;
    /// <summary>Ormancılık (III): kesilen orman iki kat hızlı yenilenir, kereste verimi +%25</summary>
    public const int FORESTRY = 2;
}

public sealed partial class Sim
{
    public static readonly string[] TIER_TR = { "Kamp", "Köy", "Kasaba", "Şehir" };
    public static readonly double[] TIER_POP = { 0, 12, 40, 100 };
    /// <summary>histerezis: yerleşim kademesini nüfus eşiğin bu payının altına düşene dek korur (eşikte titremesin)</summary>
    public const double TIER_KEEP = 0.85;

    /// <summary>Faz 1b-4: "büyük şehir" (v3'ün çekirdek halkası) = Şehir kademesindeki yerleşim (nüfus ≥ 100; histerezisle ≥ 85).</summary>
    public const int BIG_TIER = 3;

    /// <summary>Faz 1b-4: yerleşim büyük şehir mi (Şehir kademesi, <see cref="BIG_TIER"/>).</summary>
    public static bool IsBig(Settlement s) => s != null && s.Tier >= BIG_TIER;

    /// <summary>Faz 1b-4: çekirdek şehir: Şehir kademesine bir kez varmış yerleşim (küçülse de). Terk edilmez: hiçbir yol (savaş, akın,
    /// açlık, salgın, göç; <see cref="RemovePop"/>) onu <see cref="CORE_MIN"/> kişinin altına indiremez; açlık, yaşlılık, salgın, kıtlık
    /// göçü ve han yolundan geçen mülteciler o sınırda ayrıca durur (Economy, Events, InnLife).</summary>
    public static bool IsCore(Settlement s) => s != null && (s.PeakTier ?? 0) >= BIG_TIER;

    /// <summary>Faz 1b-4: çekirdek şehrin hiçbir yoldan inemeyeceği en az nüfus (Köy eşiği)</summary>
    public const double CORE_MIN = 12;

    /// <summary>
    /// Medeniyet kademesine bağlı etkiler (birikimli; eski ana ağacın dengede ağırlığı olan medeniyet çapındaki etkileri):
    /// Kasaba: Kervancılık (ticaret altını +0,3); Şehir: Ticaret Ağları ve Deniz Ticareti (+0,7), Kale Yapımı (savunma AC +2).
    /// </summary>
    private static readonly JsObj<double>[] TIER_EFF =
    {
        null,
        null,
        new JsObj<double> { ["tradeGold"] = 0.3 },
        new JsObj<double> { ["tradeGold"] = 0.7, ["defAc"] = 2 },
    };

    /// <summary>Yerleşim kademesi yalnız nüfusla (eskiden çağ da gerekirdi). Eşiğin altına düşen yerleşim, nüfusu eşiğin
    /// <see cref="TIER_KEEP"/> payına inene dek kademesini korur.</summary>
    public int TierOf(Settlement s)
    {
        double P = Pop(s);
        int t = 0;
        for (int i = 1; i < 4; i++) if (P >= TIER_POP[i]) t = i;
        for (int k = s.Tier; k > t; k--) if (P >= TIER_POP[k] * TIER_KEEP) return k;
        return t;
    }

    /// <summary>Medeniyetin kademesi: en büyük (en yüksek kademeli) yerleşiminin kademesi; yerleşimi yoksa 0.</summary>
    public int CivTier(Civ c)
    {
        int t = 0;
        foreach (var s in W.Settlements) if (s.Alive && s.Civ == c.Id && s.Tier > t) t = s.Tier;
        return t;
    }

    /// <summary>Medeniyet çapında kapı (<see cref="Gate"/>): medeniyetin kademesi en az <paramref name="tier"/>.</summary>
    public bool CivAt(Civ c, int tier) => CivTier(c) >= tier;

    /// <summary>Yapı (civic) bu yerleşimin kademesinde kurulabilir mi.</summary>
    public static bool CivicOk(Settlement st, string kind) => st.Tier >= (D.CIVICS[kind].Tier ?? 0);

    /// <summary>Atölye bu yerleşimin kademesinde kurulabilir mi.</summary>
    public static bool WorkshopOk(Settlement st, string kind) => st.Tier >= (D.WORKSHOPS[kind].Tier ?? 0);

    /// <summary>Atölyenin alternatif girdisi (demir) bu yerleşimde kullanılabilir mi.</summary>
    public static bool WorkshopAltOk(Settlement st, WorkshopDef def) => def.Alt != null && st.Tier >= (def.AltTier ?? 0);

    /// <summary>Çıkarma yapısının bu seviyesi (1–3) bu yerleşimin kademesinde kurulabilir mi (seviye yoksa hayır).</summary>
    public static bool ExtractLevelOk(Settlement st, string kind, int level)
    {
        var tiers = D.EXTRACTS[kind].Tier;
        int? t = level >= 1 && level <= tiers.Count ? tiers[level - 1] : null;
        return t != null && st.Tier >= t.Value;
    }

    /// <summary>Hex'in sahibi yerleşimin kademesi (sahipsizse -1).</summary>
    public int TileTier(int i)
    {
        int o = W.Tiles[i].Owner;
        return o >= 0 ? Settlement(o)?.Tier ?? -1 : -1;
    }

    /// <summary>Etkiler: sınıf tabanı + medeniyet kademesiyle açılan sınıf ayrıcalıkları (ClassDef.Perks) + kademe etkileri
    /// (<see cref="TIER_EFF"/>). Kademe değişince (UpdateTerritory) yeniden hesaplanır.</summary>
    public void RecomputeEff(Civ c)
    {
        var cls = D.CLASSES[c.Cls];
        var e = new JsObj<double>();
        void AddE(JsObj<double> x)
        {
            if (x == null) return;
            foreach (var kv in x) e.Set(kv.Key, (e.Get(kv.Key) ?? 0) + kv.Value);
        }
        AddE(cls.Base);
        int tier = CivTier(c);
        if (cls.Perks != null) foreach (var p in cls.Perks) if (p.Tier <= tier) AddE(p.Eff);
        for (int t = 1; t <= tier && t < TIER_EFF.Length; t++) AddE(TIER_EFF[t]);
        c.Eff = e;
    }

    /// <summary>Medeniyetin kademesiyle açılmış özel birlikleri (ayrıcalık sırasıyla, tekrarsız).</summary>
    public List<string> CivUnits(Civ c)
    {
        var o = new List<string>();
        var perks = D.CLASSES[c.Cls].Perks;
        if (perks == null) return o;
        int tier = CivTier(c);
        foreach (var p in perks) if (p.Unit != null && p.Tier <= tier && !o.Contains(p.Unit)) o.Add(p.Unit);
        return o;
    }

    /// <summary>
    /// UpdateTerritory'den: yerleşimin kademesi ilk kez yükseldiğinde kroniğe yazılır; medeniyet ilk kez Kasaba ya da Şehir
    /// kademesine varınca (eski çağ atlamanın yerine) büyük olay olur, kademeyle görünür olan yataklar (mana, mithril) ilan edilir.
    /// </summary>
    private void TierRise(Settlement s, int from)
    {
        int peak = s.PeakTier ?? 0;
        if (s.Tier <= peak || s.Tier <= from) return;
        s.PeakTier = s.Tier;
        if (W.Day <= 0) return;
        var c = W.Civs[s.Civ];
        Metric("tier" + s.Tier);
        bool first = !J.T(c.Yearly.Get("tier" + s.Tier));
        if (first) c.Yearly.Set("tier" + s.Tier, W.Day);
        string what = s.Tier == 3 ? "bir şehre" : s.Tier == 2 ? "bir kasabaya" : "bir köye";
        Log("growth", $"{s.Name} {what} dönüştü ({J.S(Pop(s))} nüfus).", civ: c.Id, tile: s.Tile, major: first && s.Tier >= 2,
            cause: first && s.Tier >= 2 ? $"{Tr.Ek(c.Name, "in")} ilk {(s.Tier == 3 ? "şehri" : "kasabası")}" : null);
        if (!first) return;
        foreach (var kind in D.DEPOSITS.Keys())
        {
            if (D.DEPOSITS[kind].HiddenTier != s.Tier) continue;
            var seen = J.Filter(W.Deposits, d => d.Kind == kind && d.KnownBy.Contains(c.Id));
            if (seen.Count > 0) Log("discover", $"{c.Name} artık {J.TrLower(D.DEPOSITS[kind].Name)} yataklarını görebiliyor ({seen.Count}).", civ: c.Id, tile: seen[0].Tiles[0], major: true);
        }
    }
}
