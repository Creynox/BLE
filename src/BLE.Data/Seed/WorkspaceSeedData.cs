using System;
using BLE.Domain.Entities;

namespace BLE.Data.Seed;

public static class WorkspaceSeedData
{
    public static readonly DateTime SeedTimestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static class PruefverfahrenIds
    {
        public static readonly Guid Sieb = Guid.Parse("36A8607F-92D3-4D9C-B6D5-2DB8A6355409");
        public static readonly Guid Korn = Guid.Parse("D4F5389D-2195-4E1B-9568-7F7D4F7FDEDD");
        public static readonly Guid Sae = Guid.Parse("9ADF7C86-409B-46F0-957B-0D9C4B7B64F0");
        public static readonly Guid Koch = Guid.Parse("A6C0C49C-3F3C-4F66-A4B6-9EFC0266F9B9");
        public static readonly Guid Bind = Guid.Parse("8B66F6D4-3D6F-478A-BF42-4CF2F5A9A0E2");
        public static readonly Guid Frost = Guid.Parse("32AD7066-76A4-4B53-8387-49753D1953A8");
    }

    public static class VorlageIds
    {
        public static readonly Guid Sts045 = Guid.Parse("0B6A1E44-7009-4A63-BF4D-5B1B6C2C81C0");
        public static readonly Guid ZweiBisAchtBeton = Guid.Parse("3E6A0732-91E5-4703-A8E2-2452B8B9E046");
        public static readonly Guid KsmMb = Guid.Parse("48F1B1C5-8C4D-49F7-8E21-F712B857D0E2");
        public static readonly Guid Bs0_2Abgemagert = Guid.Parse("0D54C3D1-7F28-4E3E-A8BE-02F6E9E37F2A");
    }

    public static class VorlageItemIds
    {
        public static readonly Guid Sts045Sieb = Guid.Parse("F8FCE8CB-EE7E-4C3B-9A1E-1607D4F3F258");
        public static readonly Guid Sts045Korn = Guid.Parse("959C37B2-1B41-469F-BA4A-6C4505E6DA93");
        public static readonly Guid Sts045Sae = Guid.Parse("0BD473C1-3933-4B97-9E26-9F4B0B1747AB");
        public static readonly Guid Sts045Koch = Guid.Parse("BB3D0294-3103-4948-9F35-2E0F5B85F14C");

        public static readonly Guid ZweiAchtSieb = Guid.Parse("C0E98539-9BB2-48CF-9C0D-5D39B4FC4008");
        public static readonly Guid ZweiAchtKorn = Guid.Parse("69F3BB40-0C6A-4F10-815B-8AC749D5B1E6");
        public static readonly Guid ZweiAchtSae = Guid.Parse("AF9F0245-417E-43C4-83AE-567F62A8EDE1");

        public static readonly Guid KsmMbSae = Guid.Parse("7E128E41-9EB4-461F-BE99-9C0AE3D02961");
        public static readonly Guid KsmMbBind = Guid.Parse("8E3BF7CC-D79F-483B-B2F4-3F1FF19D4F50");

        public static readonly Guid BsAbgSieb = Guid.Parse("D36F6F73-DA64-428B-8D3E-4BE4E2AFFBD6");
        public static readonly Guid BsAbgKorn = Guid.Parse("4E661D63-7060-4709-AD20-C8BCBF9E247D");
    }

    public static readonly Pruefverfahren[] Pruefverfahren =
    {
        new Pruefverfahren
        {
            Id = PruefverfahrenIds.Sieb,
            NormwerkId = Guid.Empty,
            Code = "SIEB",
            Titel = "Siebanalyse",
            Name = "Siebanalyse",
            Aktiv = true,
            CreatedAt = SeedTimestamp
        },
        new Pruefverfahren
        {
            Id = PruefverfahrenIds.Korn,
            NormwerkId = Guid.Empty,
            Code = "KORN",
            Titel = "Kornform",
            Name = "Kornform",
            Aktiv = true,
            CreatedAt = SeedTimestamp
        },
        new Pruefverfahren
        {
            Id = PruefverfahrenIds.Sae,
            NormwerkId = Guid.Empty,
            Code = "SAE",
            Titel = "Siebäquivalent",
            Name = "Siebäquivalent",
            Aktiv = true,
            CreatedAt = SeedTimestamp
        },
        new Pruefverfahren
        {
            Id = PruefverfahrenIds.Koch,
            NormwerkId = Guid.Empty,
            Code = "KOCH",
            Titel = "Kochversuch",
            Name = "Kochversuch",
            Aktiv = true,
            CreatedAt = SeedTimestamp
        },
        new Pruefverfahren
        {
            Id = PruefverfahrenIds.Bind,
            NormwerkId = Guid.Empty,
            Code = "BIND",
            Titel = "Bindemittelgehalt",
            Name = "Bindemittelgehalt",
            Aktiv = true,
            CreatedAt = SeedTimestamp
        },
        new Pruefverfahren
        {
            Id = PruefverfahrenIds.Frost,
            NormwerkId = Guid.Empty,
            Code = "FROST",
            Titel = "Frost/Tausalz",
            Name = "Frost/Tausalz",
            Aktiv = true,
            CreatedAt = SeedTimestamp
        }
    };

    public static readonly PruefplanVorlage[] PruefplanVorlagen =
    {
        new PruefplanVorlage
        {
            Id = VorlageIds.Sts045,
            Name = "STS 0-45 – Standard",
            Version = "v2025-10-A",
            GueltigAbUtc = SeedTimestamp,
            CreatedAt = SeedTimestamp
        },
        new PruefplanVorlage
        {
            Id = VorlageIds.ZweiBisAchtBeton,
            Name = "2-8 Beton – Standard",
            Version = "v2025-10-B",
            GueltigAbUtc = SeedTimestamp,
            CreatedAt = SeedTimestamp
        },
        new PruefplanVorlage
        {
            Id = VorlageIds.KsmMb,
            Name = "KSM-MB – Füller",
            Version = "v2025-10-C",
            GueltigAbUtc = SeedTimestamp,
            CreatedAt = SeedTimestamp
        },
        new PruefplanVorlage
        {
            Id = VorlageIds.Bs0_2Abgemagert,
            Name = "BS 0-2 abgemagert (Enzberg)",
            Version = "v2025-10-D",
            GueltigAbUtc = SeedTimestamp,
            CreatedAt = SeedTimestamp
        }
    };

    public static readonly PruefplanVorlageItem[] PruefplanVorlageItems =
    {
        new PruefplanVorlageItem { Id = VorlageItemIds.Sts045Sieb, VorlageId = VorlageIds.Sts045, PruefverfahrenId = PruefverfahrenIds.Sieb, Pflicht = true, Reihenfolge = 1, CreatedAt = SeedTimestamp },
        new PruefplanVorlageItem { Id = VorlageItemIds.Sts045Korn, VorlageId = VorlageIds.Sts045, PruefverfahrenId = PruefverfahrenIds.Korn, Pflicht = true, Reihenfolge = 2, CreatedAt = SeedTimestamp },
        new PruefplanVorlageItem { Id = VorlageItemIds.Sts045Sae, VorlageId = VorlageIds.Sts045, PruefverfahrenId = PruefverfahrenIds.Sae, Pflicht = false, Reihenfolge = 3, CreatedAt = SeedTimestamp },
        new PruefplanVorlageItem { Id = VorlageItemIds.Sts045Koch, VorlageId = VorlageIds.Sts045, PruefverfahrenId = PruefverfahrenIds.Koch, Pflicht = true, Reihenfolge = 4, CreatedAt = SeedTimestamp },

        new PruefplanVorlageItem { Id = VorlageItemIds.ZweiAchtSieb, VorlageId = VorlageIds.ZweiBisAchtBeton, PruefverfahrenId = PruefverfahrenIds.Sieb, Pflicht = true, Reihenfolge = 1, CreatedAt = SeedTimestamp },
        new PruefplanVorlageItem { Id = VorlageItemIds.ZweiAchtKorn, VorlageId = VorlageIds.ZweiBisAchtBeton, PruefverfahrenId = PruefverfahrenIds.Korn, Pflicht = true, Reihenfolge = 2, CreatedAt = SeedTimestamp },
        new PruefplanVorlageItem { Id = VorlageItemIds.ZweiAchtSae, VorlageId = VorlageIds.ZweiBisAchtBeton, PruefverfahrenId = PruefverfahrenIds.Sae, Pflicht = true, Reihenfolge = 3, CreatedAt = SeedTimestamp },

        new PruefplanVorlageItem { Id = VorlageItemIds.KsmMbSae, VorlageId = VorlageIds.KsmMb, PruefverfahrenId = PruefverfahrenIds.Sae, Pflicht = true, Reihenfolge = 1, CreatedAt = SeedTimestamp },
        new PruefplanVorlageItem { Id = VorlageItemIds.KsmMbBind, VorlageId = VorlageIds.KsmMb, PruefverfahrenId = PruefverfahrenIds.Bind, Pflicht = false, Reihenfolge = 2, CreatedAt = SeedTimestamp },

        new PruefplanVorlageItem { Id = VorlageItemIds.BsAbgSieb, VorlageId = VorlageIds.Bs0_2Abgemagert, PruefverfahrenId = PruefverfahrenIds.Sieb, Pflicht = true, Reihenfolge = 1, CreatedAt = SeedTimestamp },
        new PruefplanVorlageItem { Id = VorlageItemIds.BsAbgKorn, VorlageId = VorlageIds.Bs0_2Abgemagert, PruefverfahrenId = PruefverfahrenIds.Korn, Pflicht = false, Reihenfolge = 2, CreatedAt = SeedTimestamp }
    };
}
