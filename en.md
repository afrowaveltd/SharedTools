# Afrowave.Localization – Examples

Welcome to the `Examples` directory!  
Here you will find real, ready-to-run code samples showing how to use the Afrowave.Localization library in every common .NET scenario.

Our goal is to make onboarding as easy as possible, for absolute beginners as well as experienced developers.

---

## 🗂 Folder Structure & Navigation

Each subfolder here focuses on a different .NET generation or typical developer need:

- **Modern/**  
  Examples for .NET 6, .NET 7, .NET 8, and .NET 10+.  
  Uses top-level statements, async/await, and all the newest C# features.  
  **Recommended for all new projects!**

- **Legacy/**  
  Designed for .NET Framework 4.8, classic WinForms/WPF, and old EntityFramework projects.  
  Uses classic Program.cs with Main() and synchronous code where needed.  
  Perfect if you maintain corporate, desktop, or enterprise legacy apps.

- **Generic/**  
  Written in portable C# that should run everywhere:  
  .NET Core 3.1+, .NET 5+, Mono, Unity, and more.  
  Good for libraries, plugins, or scenarios where you want the broadest compatibility.

Every example subfolder contains a `/Readme/en.md` explaining:
- What the scenario demonstrates
- Which .NET version it targets
- How to run it

---

## 🔎 How to Find the Right Example

- **Are you starting a new project with the latest .NET?**  
  Go to `/Modern` and use the simplest sample as your base.

- **Are you working with old company code, desktop UI, or need EntityFramework 4.x?**  
  Go to `/Legacy` – all samples there use classic structure for maximum compatibility.

- **Do you need code that works everywhere, even on niche .NET platforms?**  
  Try `/Generic` – it's written for the widest possible support.

---

## 💡 For Newbies: Which Example Should I Choose?

- **If you are just learning C# or .NET and want the easiest, cleanest start:**  
  Open `/Modern/SimpleJsonWriterDemo`  
  - It uses the newest, most readable syntax  
  - You can copy-paste it to your project  
  - If something doesn't work, check that you have the newest [.NET SDK](https://dotnet.microsoft.com/en-us/download) (10+ is best)

- **If your company/project is still using “the old .NET” or you need to support classic WinForms/WPF:**  
  Look at `/Legacy/SimpleJsonWriterLegacyDemo`  
  - This uses the Program.cs and Main() method you're used to  
  - Great for any Visual Studio 2019+ project targeting .NET Framework 4.8

- **Still not sure?**  
  Start with `/Generic/SimpleJsonWriterGenericDemo` – it will run almost everywhere and is easy to adapt to your own needs.

> **Tip:** Each sample project prints its output to the console and is fully commented.  
> If you get stuck, read the `/Readme/en.md` in each folder or open a GitHub Issue — we are here to help!

---

Happy coding and welcome to the Afrowave.Localization community!  
