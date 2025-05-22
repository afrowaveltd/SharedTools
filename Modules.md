# 📦 Afrowave.SharedTools Modules Overview

Tento soubor slouží jako přehled všech modulů ve struktuře `Afrowave.SharedTools`, včetně jejich účelu a stavu. Může být průběžně aktualizován.

---

## ✅ Existující moduly

| Název projektu                    | Účel                                                              | Stav             |
| --------------------------------- | ----------------------------------------------------------------- | ---------------- |
| Afrowave.SharedTools              | Hlavní metapackage (agreguje ostatní)                             | ✅ Hotovo         |
| Afrowave.SharedTools.Abstractions | Sdílené rozhraní, DI-friendly kontrakty                           | 🟡 Prázdné       |
| Afrowave.SharedTools.Api          | Pomůcky pro práci s HttpClientem, query parametry, API odpovědi   | 🟡 Prázdné       |
| Afrowave.SharedTools.Blazor       | LocalStorage, JSInterop a komponenty pro Blazor                   | 🟡 Prázdné       |
| Afrowave.SharedTools.EF           | LINQ rozšíření, Include helpers, DTO projekce                     | 🟡 Prázdné       |
| Afrowave.SharedTools.Helpers      | Obecné utility bez závislostí (např. DateTimeHelper, RetryHelper) | 🟡 Prázdné       |
| Afrowave.SharedTools.IO           | Nástroje pro práci se soubory, složkami, path normalizace         | 🟡 Prázdné       |
| Afrowave.SharedTools.Json         | Práce se System.Text.Json, merge, safe parse                      | 🟡 Prázdné       |
| Afrowave.SharedTools.Localization | Lokalizace, zprávy, lokalizovatelné texty bez překl. enginů       | 🟡 Prázdné       |
| Afrowave.SharedTools.Models       | DTOs, výčty, konstanty, `Result`, `Response`                      | ✅ Základ         |
| Afrowave.SharedTools.Terminal     | Spectre.Console, CLI výpisy, barevné zprávy                       | 🟡 Prázdné       |
| Afrowave.SharedTools.Text         | TextHelper, odstranění diakritiky, slugify                        | 🟡 Připravuje se |
| Afrowave.SharedTools.Time         | Práce s časem, zóny, formáty, převody                             | 🟡 Prázdné       |
| Afrowave.SharedTools.Validation   | Validace dat, e-mail, IBAN, vlastní výjimky                       | 🟡 Prázdné       |

---

## 📊 Plánovaný doplněk

Navrhuji přidat do rootu projektu `Status.md`, který bude:

* obsahovat přehled modulů
* značit stav (`✅`, `🟡`, `❌`)
* prolinkovat README soubory
* sloužit jako synchronizační referenční bod napříč vlákny

> Může být generován/přepisován i automaticky přes LangHub později 💡

---

🔁 Aktualizace dle potřeby – dokument budeme držet v Canvasu jako hlavní referenci pro vývoj.
