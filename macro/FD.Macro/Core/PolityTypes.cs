using System.Collections.Generic;

// Faz 1b-6: devlet, inanç ve örgüt modelinin durum tipleri (claude/devlet-orgut-spec.md). Tanımlar ve kurallar: Core/Polity.cs,
// devlet yaşamı: Modules/States.cs, örgütler: Modules/Orgs.cs.

namespace FD.Macro;

/// <summary>Spec "Kişi": önemli NPC (yönetici, varis, örgüt lideri, şube ustası). Kahramanlar ayrı (<see cref="Hero"/>).</summary>
public sealed class Person
{
    public int Id;
    public string Name;
    public string Race;
    /// <summary>ruler | heir | leader | master | noble</summary>
    public string Role;
    /// <summary>bağlı olduğu devlet (yönetici, varis, soylu); -1 yok</summary>
    public int Civ = -1;
    /// <summary>bağlı olduğu örgüt (lider, şube ustası); null yok</summary>
    public int? Org;
    /// <summary>doğduğu gün (yaş = (gün − Born) / Sim.YEAR; doğumdan önceki günler negatif olabilir)</summary>
    public double Born;
    public double? Died;
    /// <summary>ölüm ya da görevden düşme nedeni (kronik)</summary>
    public string Fate;
    public Alignment Align;
    /// <summary>inancı: sun | old | pact | none</summary>
    public string Faith;
    /// <summary>hanedan / soy adı</summary>
    public string House;
    /// <summary>göreve geldiği gün</summary>
    public double? Since;
    /// <summary>örgüt üyelikleri (yönetici gizlice Pakt'a bağlı olabilir)</summary>
    public List<Membership> Orgs;
}

/// <summary>Spec §2 "Yasa": hükümet tipinin varsayılanı + yöneticinin hizalaması + örgüt lobileri.</summary>
public sealed class LawState
{
    /// <summary>sertlik 0–1</summary>
    public double Harsh;
    /// <summary>kölelik serbest</summary>
    public bool Slavery;
    /// <summary>resmî inanç (sun | old; null: yok, hoşgörülü)</summary>
    public string Faith;
    /// <summary>inançlara hoşgörü 0–1 (sun, old, pact, none)</summary>
    public JsObj<double> FaithTol = new();
    /// <summary>ırk hoşgörüsü 0–1 (yazılmayan ırk 1)</summary>
    public JsObj<double> RaceTol = new();
    /// <summary>kaçakçılık cezası 0–1</summary>
    public double Smuggle;
    /// <summary>rüşvet kolaylığı 0–1</summary>
    public double Bribe;
    /// <summary>devriye sıklığı 0–1</summary>
    public double Patrol;
    /// <summary>teokrasi: suçlu ve "kâfir" hapis madeninde zorla çalıştırılır</summary>
    public bool PrisonMine;
}

/// <summary>Spec §4: örgüt (sınıflar burada yaşar). Tanım: <see cref="OrgDef"/> (Polity.ORGS).</summary>
public sealed class Org
{
    public int Id;
    /// <summary>OrgDef kimliği (church, order, pact, circle, hunters, thieves, academy, companies, bards, merchants, explorers, slavers, freedom)</summary>
    public string Kind;
    public string Name;
    /// <summary>merkez yerleşimi (çekirdek şehirde landmark bina; null: merkezsiz)</summary>
    public int? Hq;
    public List<OrgBranch> Branches = new();
    /// <summary>toplam üye (şubelerin toplamı)</summary>
    public double Members;
    public double Gold;
    /// <summary>örgüt birlikleri (kiralanır ya da müttefik olarak gelir)</summary>
    public double Troops;
    /// <summary>1–3 öncelikli hedef (OrgDef.Goals'tan)</summary>
    public List<string> Goals = new();
    /// <summary>diğer örgütlerle dostluk/düşmanlık (örgüt id'si → −100..100)</summary>
    public JsNumObj<double> Rel = new();
    /// <summary>lider (Person id)</summary>
    public int? Leader;
    public bool Alive;
    public double Founded;
    public double? Died;
    /// <summary>dağılan örgütün yeniden kurulacağı gün (en erken)</summary>
    public double? RebirthDay;
    public int Rebirths;
    /// <summary>son makro karar günü</summary>
    public double LastAct = -9999;
    /// <summary>sayaçlar (şube açılışı, gölge savaşı, görev, hizmet…)</summary>
    public JsObj<double> Tally = new();
}

/// <summary>Örgütün bir yerleşimdeki şubesi (düzey 1–3, gizli olabilir).</summary>
public sealed class OrgBranch
{
    public int Settlement;
    public int Level;
    public bool Hidden;
    public double Members;
    public double Since;
    /// <summary>şube ustası (Person id)</summary>
    public int? Master;
}

/// <summary>Spec §5: kişi başına üyelik kaydı {örgüt, rütbe, örgüt içi itibar, aidat borcu, son görev}.</summary>
public sealed class Membership
{
    public int Org;
    /// <summary>örgütün türü (OrgDef kimliği; örgüt dağılıp yeniden kurulsa da aynı)</summary>
    public string Kind;
    /// <summary>0 aday · 1 üye · 2–3 rütbe · 4 iç çember</summary>
    public int Rank;
    public double Rep;
    public double Dues;
    public double LastTask = -9999;
    public double Since;
}
