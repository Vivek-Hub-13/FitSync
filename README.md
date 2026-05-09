# 🏋️ FitSync — Fitness Activity Tracker

> A Windows desktop application for tracking fitness activities, monitoring calorie burn, and managing personal fitness goals.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [Supported Activities](#supported-activities)
- [Calorie Calculation (MET Formula)](#calorie-calculation-met-formula)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Setup](#setup)
  - [Running the Application](#running-the-application)
- [Application Screens](#application-screens)
- [Security Features](#security-features)
- [Known Limitations](#known-limitations)
- [Contributing](#contributing)

---

## Overview

**FitSync** is a Windows Forms desktop application built in **C# (.NET Framework 4.8)**. It allows users to register, log in securely, and track various physical fitness activities. The app computes calories burned using the **MET (Metabolic Equivalent of Task)** formula based on the user's body weight and the intensity/duration of the activity. A visual pie chart on the dashboard summarizes total calories burned per activity type.

---

## Features

- ✅ **User Registration & Login** — Secure account creation with password strength validation
- 🔒 **Account Lockout Protection** — Locks account after 3 failed login attempts; unlocked via security question
- 📊 **Dashboard** — Overview of calorie goal vs. total calories burned with an activity pie chart
- 🏃 **Multi-Activity Logging** — Track 6 different activity types with detailed inputs
- 🎯 **Calorie Goal Setting** — Users can set and update personal daily/cumulative calorie goals
- 📁 **Activity History** — Filterable history table sorted by date, showing all logged activities
- 👤 **Profile Settings** — View personal profile information and update calorie goal
- 🔔 **Goal Achievement Alert** — Notification shown when calorie goal is reached

---

## Tech Stack

| Component      | Technology                                   |
|----------------|----------------------------------------------|
| Language       | C# 7.3+                                      |
| Framework      | .NET Framework 4.8                           |
| UI             | Windows Forms (WinForms)                     |
| Database       | Microsoft Access (`.accdb`) via OLE DB       |
| Charting       | `System.Windows.Forms.DataVisualization`     |
| Live Charts    | LiveCharts v0.9.7 (WinForms & WPF bindings)  |
| OLE DB Driver  | Microsoft ACE OLEDB 12.0                     |
| IDE            | Visual Studio (Solution: `FitSync.sln`)      |

---

## Project Structure

```
FitSync/
├── FitSync.sln                      # Visual Studio solution file
├── packages/                        # NuGet packages
│   ├── LiveCharts.0.9.7/
│   ├── LiveCharts.WinForms.0.9.7.1/
│   ├── LiveCharts.Wpf.0.9.7/
│   └── System.Data.OleDb.9.0.3/
└── FitSync/                         # Main project directory
    ├── Program.cs                   # Application entry point
    ├── UserSession.cs               # Static session management (login/logout state)
    ├── FitSync.csproj               # Project configuration
    ├── App.config                   # Application configuration
    │
    ├── FrmLogin.cs/.Designer.cs     # Login screen
    ├── FrmRegistration.cs/.Designer.cs  # User registration screen
    ├── FrmDashboard.cs/.Designer.cs     # Main dashboard (stats + chart)
    ├── FrmAddActivity.cs/.Designer.cs   # Walking activity logging
    ├── FrmRunning.cs/.Designer.cs       # Running activity logging
    ├── FrmSwim.cs/.Designer.cs          # Swimming activity logging
    ├── FrmCycle.cs/.Designer.cs         # Cycling activity logging
    ├── FrmST.cs/.Designer.cs            # Strength Training logging
    ├── FrmCalisthenics.cs/.Designer.cs  # Calisthenics logging
    ├── FrmSettings.cs/.Designer.cs      # User profile & calorie goal settings
    │
    ├── Resources/                   # Icon/image assets
    │   ├── user.png
    │   ├── walking.png
    │   ├── swimming.png
    │   ├── cycling.png
    │   ├── training.png
    │   ├── athlete.png
    │   ├── calisthenics.png
    │   ├── analytics.png
    │   ├── logout.png
    │   ├── Bar chart.png
    │   ├── puzzle-pieces.png
    │   └── crossword.png
    └── Properties/                  # Assembly info, resources, settings
```

---

## Database Schema

The application uses a **Microsoft Access database** (`FitSync.accdb`) located at:
```
C:\FitSync\FitSync\bin\FitSync.accdb
```

### Tables

#### `UserInfo`
| Column           | Type    | Description                         |
|------------------|---------|-------------------------------------|
| Username         | Text    | Unique login identifier             |
| Password         | Text    | User password (plain text)          |
| FirstName        | Text    | First name                          |
| LastName         | Text    | Last name                           |
| Email            | Text    | Email address                       |
| PhoneNumber      | Text    | Contact number                      |
| Height           | Text    | Height (in cm)                      |
| Weight           | Number  | Weight in kg (used for calorie calc)|
| Gender           | Text    | Male / Female / Other               |
| SecurityQuestion | Text    | For account recovery                |
| SecurityAnswer   | Text    | Answer to security question         |
| Calorie Goal     | Number  | Personal calorie burn target        |

#### `History` (Unified Activity Log)
| Column        | Type    | Description                        |
|---------------|---------|------------------------------------|
| Username      | Text    | Linked user                        |
| Activity      | Text    | Activity type (e.g., "Running")    |
| Date          | Text    | Date of activity                   |
| Time          | Text    | Time of activity                   |
| TotalCalories | Number  | Calories burned in this session    |

#### Activity-Specific Tables
Each activity type has its own dedicated table storing detailed metrics:

| Table             | Key Fields                                               |
|-------------------|----------------------------------------------------------|
| `Walking`         | Steps, Distance (meters), Duration, CaloriesBurned       |
| `Running`         | Duration, Distance, Speed, Intensity, CaloriesBurned     |
| `Swimming`        | Duration, Laps, HeartRate, Intensity, CaloriesBurned     |
| `Cycling`         | Duration, Distance, Speed, Intensity, CaloriesBurned     |
| `StrengthTraining`| Duration, Reps, Intensity, CaloriesBurned                |
| `Calisthenics`    | Duration, HeartRate, Intensity, CaloriesBurned           |

---

## Supported Activities

| Activity         | Form       | Inputs Required                                        |
|------------------|------------|--------------------------------------------------------|
| 🚶 Walking        | FrmAddActivity | Steps, Distance (km), Duration (min), Time, Date  |
| 🏃 Running        | FrmRunning | Duration, Distance, Speed, Intensity, Time, Date       |
| 🏊 Swimming       | FrmSwim    | Duration, Laps, Heart Rate, Intensity, Time, Date      |
| 🚴 Cycling        | FrmCycle   | Duration, Distance, Speed, Intensity, Time, Date       |
| 🏋️ Strength Training | FrmST  | Duration, Reps, Intensity, Time, Date                  |
| 🤸 Calisthenics   | FrmCalisthenics | Duration, Heart Rate, Intensity, Time, Date      |

---

## Calorie Calculation (MET Formula)

All activities use the standard **MET (Metabolic Equivalent of Task)** formula:

```
Calories Burned = MET × Weight (kg) × Duration (hours)
```

### MET Values by Activity and Intensity

| Activity         | Low  | Moderate | High  |
|------------------|------|----------|-------|
| Running          | 6.0  | 9.8      | 12.0  |
| Swimming         | 4.8  | 7.0      | 10.0  |
| Cycling          | 6.0  | 8.0      | 12.0  |
| Strength Training| 3.5  | 6.0      | 8.0   |
| Calisthenics     | 4.0  | 6.0      | 8.0   |
| Walking          | 2.0–4.0 (auto, speed-based) | — | — |

> **Walking** uses a dynamic MET value determined by the calculated speed: `< 2 km/h → 2.0`, `2–4 km/h → 3.0`, `> 4 km/h → 4.0`.

---

## Getting Started

### Prerequisites

| Requirement                    | Version / Notes                                    |
|--------------------------------|----------------------------------------------------|
| Windows OS                     | Windows 10 or later (64-bit recommended)           |
| .NET Framework                 | 4.8 (installed by default on Windows 10+)          |
| Visual Studio                  | 2019 or later (Community edition is free)          |
| Microsoft Access Database Engine | [ACE OLEDB 12.0 redistributable](https://www.microsoft.com/en-us/download/details.aspx?id=54920) |
| Microsoft Access (optional)    | For viewing/editing the `.accdb` database directly |

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/FitSync.git
   cd FitSync
   ```

2. **Place the database file**

   The application expects the database at a fixed path. Create the directory and place the database:
   ```
   C:\FitSync\FitSync\bin\FitSync.accdb
   ```
   > ⚠️ This path is hardcoded in all form files. Ensure this folder exists and the `.accdb` file is placed there before running the app.

3. **Restore NuGet packages**

   Open `FitSync.sln` in Visual Studio, then:
   - Go to **Tools → NuGet Package Manager → Package Manager Console**
   - Run: `Update-Package -reinstall`
   
   Or right-click the solution in **Solution Explorer** → **Restore NuGet Packages**.

4. **Build the solution**
   - Press `Ctrl + Shift + B` or go to **Build → Build Solution**

### Running the Application

- Press **F5** (Debug mode) or **Ctrl + F5** (without debugger) in Visual Studio.
- The application will launch starting from the **Login screen** (`FrmLogin`).

---

## Application Screens

### 1. Login (`FrmLogin`)
- Enter username and password to log in.
- **Show Password** checkbox to toggle password visibility.
- After **3 failed attempts**, the account is temporarily locked and the security question flow is triggered.
- New users can click the registration link to go to the Registration screen.

### 2. Registration (`FrmRegistration`)
- Collects: First Name, Last Name, Username, Password, Confirm Password, Email, Phone, Height, Weight, Gender, Security Question, Security Answer.
- **Password Policy**: Minimum 12 characters, must include at least one uppercase and one lowercase letter.
- All fields are mandatory.

### 3. Dashboard (`FrmDashboard`)
- Shows **Calorie Goal** and **Total Calories Burned** (from History).
- Displays a **Pie Chart** of calories burned grouped by activity type.
- Contains an activity **filter dropdown** and a history **DataGrid**.
- Navigation buttons to Add Activity, Settings, and Logout.
- Alerts the user with a congratulatory message when the calorie goal is met or exceeded.

### 4. Add Activity / Activity Forms
- A hub screen (`FrmAddActivity`) for walking that also provides navigation buttons to all other activity types.
- Each activity screen logs data to both its specific table and the shared `History` table.

### 5. Settings (`FrmSettings`)
- Displays the current user's profile information (read-only).
- Allows users to set or update their **Calorie Goal**.
- Updates the dashboard's calorie data in real-time if it's open.

---

## Security Features

| Feature                       | Implementation                                               |
|-------------------------------|--------------------------------------------------------------|
| Login attempt limiting        | Counter increments on failure; locked at 3 attempts          |
| Account unlock via Q&A        | Security question answer verified against the database       |
| Password strength enforcement | Regex: min 12 chars, at least 1 upper + 1 lower case letter  |
| Session management            | Static `UserSession` class tracks `CurrentUsername` in memory|

> ⚠️ **Note:** Passwords are currently stored in **plain text** in the database. For production use, password hashing (e.g., `bcrypt` or `SHA-256` with salt) should be implemented.

---

## Known Limitations

- **Hardcoded database path**: The `.accdb` path (`C:\FitSync\FitSync\bin\FitSync.accdb`) is hardcoded across all forms. Moving the database requires changing all connection strings.
- **Plain text passwords**: Passwords are not hashed. This is a significant security concern for any real-world deployment.
- **No data export**: There is currently no export functionality (e.g., to PDF or CSV) for activity history.
- **Single-user session**: The `UserSession` static class supports only one logged-in user per application instance.
- **No input sanitization beyond validation**: Inputs like Date and Time are stored as raw strings.

---

## Contributing

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin feature/your-feature`
5. Open a Pull Request

---

## License

This project is intended for academic and educational purposes. Please check with the repository owner for licensing terms before commercial use.

---

*Built with ❤️ using C# Windows Forms*
