# RapidNovel

Desktop app to write Web novel faster with help of AI and planning.

Cross-platform (Windows / Linux / macOS) novel-writing app built with **Avalonia UI** and an MVVM architecture. It combines world-building/reference data (characters, places, objects, timeline) with structured writing tools (drafts, chapters, books) — aiming to help authors produce web novels faster with AI assistance.

## Features

### 📁 Project (enabled only when a project is loaded)
- **Characters** – Create and manage fictional characters with their attributes, roles, descriptions, and relationships.
- **Places** – Manage the settings/locations used throughout the story.
- **Objects** – Manage key items, props, and artifacts relevant to the plot.
- **Timeline** – View and edit the chronological sequence of events across the novel.

### ✍️ Write (enabled only when a project is loaded)
- **Drafts** – Write and organize rough, unpolished drafts before refining them.
- **Chapters** – Structured chapter-level writing and organization.
- **Books** – Manage multiple books — support for volumes, series, and multi-book projects.

## Planned / Inferred Roadmap
- **AI assistance** for writing (per the project tagline): story generation, continuation, editing, and idea suggestions.
- **Project-tied state management** via `ProjectViewModel` with `IsProjectLoaded` guarding project-scoped actions.
- **Reference database** linking characters, places, objects, and timeline entries to chapters.
