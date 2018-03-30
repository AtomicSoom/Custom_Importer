# Custom_Importer
- Made by Atomic Digital Design

Custom importer based on Preset system for Unity

Disclaimer : This is still a work in progress

Works with Unity 2018.1b12

Instructions :

You can create rule settings via the "Create" menu.

Each type settings can have multiple rule,
they have a specific Preset (some example are in "CustomImporter/Presets").
When an asset is imported it will check the rules one by one and the first valid one will apply it's Preset.
You can reorder the rules by priority using the context menu function "Sort By Priority".

The rule settings objects as to be linked to the asset importer via "CustomImporterLinks".
