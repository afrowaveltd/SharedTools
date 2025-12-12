# Afrowave Community Web Apps

This repository contains web applications and services used internally by the **Afrowave Community**.

The focus of this repository is to build practical, educational and infrastructure-related web tools that support learning, experimentation and community collaboration.  
These applications are not generic libraries – they represent concrete products, portals and services built on top of shared Afrowave tooling.

---

## 📦 Repository Scope

This repository may contain:

- Web portals (Blazor, Razor Pages, minimal UI where appropriate)
- API services (controllers-based APIs, not minimal APIs by default)
- Internal hubs and coordinators (e.g. communication hubs, network-related services)
- Educational and interactive tools (labs, simulations, games)
- Infrastructure-facing applications used by the Afrowave Community

All applications here **consume Afrowave SharedTools exclusively via NuGet packages**.  
Direct project references to SharedTools are intentionally not used.

---

## 🧱 Architecture Principles

- **Strict layering**  
  Each application is split into clear layers (API, logic, UI, infrastructure).
- **Dependency Injection everywhere**  
  No static global state, no hidden dependencies.
- **Shared first**  
  If something can be reused elsewhere, it belongs to `Afrowave.SharedTools`.
- **Backward compatibility is a priority**  
  Public contracts are preserved whenever technically possible.

---

## 📚 SharedTools Usage

Reusable components such as:

- models
- validation helpers
- networking utilities
- mocking and testing helpers
- localization tools

are implemented in **Afrowave.SharedTools** and consumed here as NuGet dependencies.

This repository focuses on *using* those tools, not redefining them.

---

## 🔢 Versioning Policy (Short)

This repository follows a strict backward-compatibility approach.

Version format:

> MAJOR.MINOR.PATCH[-alpha|-beta]


- **PATCH** – internal changes, fixes or security updates without external behavior change
- **MINOR** – new functionality or externally observable behavior change (non-breaking)
- Each new **MINOR** starts at `PATCH = 0`
- All versions are **alpha by default**
- **beta** is used optionally to mark a stabilized or frozen state
- A **final** release (without suffix) is always the highest PATCH of a given MINOR series

Breaking changes are avoided wherever technically possible.

---

## 🧪 Development Status

Most projects in this repository are under active development and experimentation.  
Stability grows over time, but learning, clarity and correctness are always preferred over shortcuts.

---

## ❤️ Philosophy

Afrowave Community tools are built to:

- teach by doing
- encourage curiosity
- reward attention and understanding
- remain transparent and readable
- respect long-term compatibility

If something looks simple, it is intentional.

---

## 📄 License

License information will be added per project where applicable.
