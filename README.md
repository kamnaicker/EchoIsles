# 🎮 Echo Isles

> **CM3030 – Game Development (2025)**  
> Built with **Unity 6.2 LTS** · Single Player · Keyboard + Mouse · WebGL Target

---

## 🧭 Overview

**Echo Isles** is a short, stylised **isometric adventure-puzzle** game set in the vertical ruins of a forgotten island city.  
Players climb through the environment by **recording and replaying echoes of themselves** to solve spatial challenges and reveal fragments of the island’s memory.  
The focus is on **clarity, polish, and atmosphere** — no combat, no heavy text, just exploration, logic, and visual storytelling.

---

## 🧩 Core Concept

| Property     | Details                                                                                |
| ------------ | -------------------------------------------------------------------------------------- |
| **Genre**    | Isometric Puzzle Adventure                                                             |
| **Length**   | 5 – 10 minutes (per module brief)                                                      |
| **Engine**   | Unity 6.2 LTS + URP                                                                    |
| **Platform** | WebGL (build for playtesting and submission)                                           |
| **Controls** | Keyboard & Mouse only                                                                  |
| **Goal**     | Deliver a polished vertical slice demonstrating strong design, iteration, and teamwork |

---

## ⚙️ Key Features

- **Echo Mechanic ** – Record and replay past actions to solve puzzles.
- **Isometric Exploration ** – Fixed camera view with vertical level design.
- **Environmental Puzzles ** – Levers, pressure plates, timed doors, and layered paths.
- **Memory Fragments ** – Visual storytelling through light, architecture, and text snippets.
- **Minimal UI & Clean Audio Design ** – Focus on readability and immersion.

---

## 🧠 Development Focus

1. **Echo Mechanic** – Record/replay system; must be clear within seconds of play.
2. **Puzzle Design** – Teach → Test → Twist progression across 2–3 short stages.
3. **Movement & Camera** – Smooth isometric controls and precise positioning.
4. **Visual Clarity & Art** – Low-poly ruins with URP lighting and fog.
5. **Audio & Feedback** – Ambient music + distinct echo and interaction cues.
6. **UX / UI** – Minimal interface (show record indicator + restart prompt only).

---

## 🛠️ Tech Stack

| Component                           | Purpose                                                  |
| ----------------------------------- | -------------------------------------------------------- |
| **Unity 6.2 LTS**                   | Core engine (LTS = stability & compatibility)            |
| **URP – Universal Render Pipeline** | Lightweight stylised rendering                           |
| **C# Scripts**                      | Player controller, Echo system, Puzzle logic, Game state |
| **Git / GitHub**                    | Version control + collaboration                          |
| **Jira Software**                   | Agile sprint planning & issue tracking                   |
| **WebGL Builds**                    | For feedback milestones and final submission             |

---

## 📂 Repository Structure (Subject To Change)

EchoIsles/
┣ Assets/
┃ ┣ \_Project/
┃ ┃ ┣ Art/
┃ ┃ ┣ Audio/
┃ ┃ ┣ Code/
┃ ┃ ┣ Prefabs/
┃ ┃ ┣ Scenes/
┃ ┃ ┣ ScriptableObjects/
┃ ┃ ┗ UI/
┣ Packages/
┣ ProjectSettings/
┣ README.md
┗ .gitignore

---

## 🔗 Jira Integration

This repository is connected to the **Echo Isles Jira** board for sprint and issue tracking.  
Include the Jira issue key in each commit message to auto-link work items.

**Example Commit Message**

```bash
git commit -m "EIS-14: Added echo replay prototype and lever trigger logic"
```
