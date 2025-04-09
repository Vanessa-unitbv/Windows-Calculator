This repository contains a feature-rich calculator application built with C# and WPF. The calculator offers both standard and programmer calculation modes with a clean, user-friendly interface.

## Features

### Standard Calculator
- Basic arithmetic operations (addition, subtraction, multiplication, division)
- Advanced mathematical functions (square, square root, reciprocal)
- Memory functions (MC, MR, MS, M+, M-)
- Percentage calculations
- Order of operations support for complex expressions
- Keyboard input support

### Programmer Calculator
- Multiple number system support (Hexadecimal, Decimal, Octal, Binary)
- Real-time conversion between number systems
- Arithmetic operations in any number system
- Contextual button enabling based on selected number system

### General Features
- Digit grouping option for better readability
- Copy, cut, and paste functionality
- Visual feedback when operating the calculator
- Persistent settings across application sessions
- Error handling for invalid operations (division by zero, etc.)
- Backspace functionality
- Clear and Clear Entry operations

## Technical Implementation

The application follows a well-structured design with clear separation of concerns:

- `CalculatorEngine`: Core calculation logic
- `CalculatorManager`: Handles standard calculator operations and UI interaction
- `ProgrammerCalculatorManager`: Manages programmer calculator functionality
- `CalculatorMemoryManager`: Manages memory stack and operations
- `CalculatorModeManager`: Handles switching between calculator modes
- `ClipboardManager`: Manages copy, cut, and paste operations
- `SettingsManager`: Handles saving and loading application settings

The application uses XAML for the UI with a custom button style and a warm, visually appealing color scheme. Settings are stored in XML format for persistence between sessions.

## Usage

The calculator supports both mouse and keyboard input, making it versatile for different user preferences. The mode can be changed from the menu or by using keyboard shortcuts (Ctrl+Tab). The application also provides helpful error messages to guide users when performing invalid operations.
