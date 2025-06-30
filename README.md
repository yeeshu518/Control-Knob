# Control-Knob
This is a Control knob for those who want to add some extra spice in their project . 
# YeesusKnob 🎛️

A modern, smooth, customizable knob control for Windows Forms (.NET), ideal for visualizing and adjusting values like sensitivity, volume, thresholds, and more.

![Image](https://github.com/user-attachments/assets/688d08c5-6477-4873-83cd-54261b6b89d1)

![Image](https://github.com/user-attachments/assets/2ca743f7-2bd3-4146-b9ac-954b6c5294a7)

![Image](https://github.com/user-attachments/assets/fb71530a-e03a-46a1-a029-5899c95f5944)

---

## ✨ Features

- 🌀 **Rotary knob UI** with smooth pointer movement  
- 🎚️ **Customizable tick labels** via the `Labels` collection  
- 🔴 **Color transition** from soft gray to red based on value  
- 🧲 **Snap-to-nearest-tick** behavior  
- 🎵 Optional **click sound** when value changes  
- 🧪 **Design-time support**: editable properties in the Properties window  
- 🎨 Change appearance (colors, fonts, caption) easily

---

## 🔧 How to Use

### ➕ Option 1: Add Directly to Your Project

1. Copy `YeesusKnob.cs` into your WinForms project.
2. Rebuild your project.
3. Drag and drop `YeesusKnob` from the toolbox onto your form.

> 💡 If you don't see it, right-click Toolbox → "Choose Items..." → Browse → select your compiled DLL or project output.

---

### 🛠 Option 2: Use as a Class Library

1. Create a Class Library project (`YeesusKnobLib`)
2. Add `YeesusKnob.cs` into that project
3. Reference the library from your main application
4. Build and drag from the toolbox

---

## 🧪 Example

```csharp
yeesusKnob1.Caption = "Sensitivity";
yeesusKnob1.Labels.Add(new LabelItem("0.25"));
yeesusKnob1.Labels.Add(new LabelItem("0.50"));
yeesusKnob1.Labels.Add(new LabelItem("1.00"));
yeesusKnob1.Labels.Add(new LabelItem("2.00"));
yeesusKnob1.Labels.Add(new LabelItem("4.00"));
yeesusKnob1.Labels.Add(new LabelItem("8.00"));
