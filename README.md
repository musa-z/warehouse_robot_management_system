# Warehouse Robot Management System

## Overview
**Project:** Warehouse Robot Management System.

Simulates job handling and task allocation for multiple robots. Jobs are created from XML files, stored locally, and assigned to robots while calculating distance, time, and battery usage.

---

## Features
- Create and load jobs from XML
- Automatic robot assignment and charging check
- Distance, time, and battery calculations
- View robot status, job history, and statistics
- Read-only SQL query interface

---

## Code
- Written in C# (.NET Windows Forms)
- Source files located in the `code/` folder
- Database file found in `database` folder

---

## Setup
- Install Visual Studio (.NET Desktop)
- Run the SQL file in the `database/` folder to create required tables
- Place `Robots.db` in the same folder as the executable
- `Jobs.xml` is created automatically if missing

```bash
# Clone the repo
git clone https://github.com/musa-z/warehouse-robot-management-system.git

# Open the solution in Visual Studio and run
