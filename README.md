# Warehouse Robot Management System

C# Windows Forms application for managing warehouse robot jobs, robot status, task allocation and local job history. The system simulates how multiple warehouse robots can be assigned to jobs while checking distance, travel time and battery usage.

## Overview

This project focuses on warehouse robot task management and job allocation.

Jobs are loaded from XML files, stored locally and assigned to available robots based on job requirements and robot state. The system calculates travel distance, estimated completion time and battery usage, while maintaining job records and robot status information in a local database.

## Key Features

- Created a C# Windows Forms application for warehouse robot job management
- Loaded and created robot jobs using XML files
- Assigned jobs to robots based on availability, distance and battery checks
- Calculated distance, travel time and estimated battery usage for each job
- Stored robot, job and history data using a local SQLite database
- Added status views for robot availability, job history and system statistics
- Included a read-only SQL query interface for inspecting stored data

## Technologies Used

- C#
- .NET Windows Forms
- SQLite
- SQL
- XML file handling
- Object-oriented programming
- Visual Studio

## Repository Structure

    warehouse_robot_management_system/
    ├── code/
    ├── database/
    ├── README.md
    └── LICENSE

## System Pipeline

    XML Job File
         ↓
    Job Loading / Creation
         ↓
    Local Database Storage
         ↓
    Robot Availability Check
         ↓
    Distance, Time and Battery Calculation
         ↓
    Robot Job Assignment
         ↓
    Job History and Statistics Update

## Method

The system manages a set of warehouse robots and incoming jobs. Each job contains information such as pickup/drop-off locations and task requirements. The application checks robot availability and estimates whether a robot can complete the task based on distance, time and battery usage.

Job and robot information is stored locally, allowing the system to display current robot status, previous jobs and summary statistics. A read-only SQL query interface is included to inspect stored data without directly modifying records.

## Setup

Install:

    Visual Studio
    .NET Desktop Development workload
    SQLite support if required

Clone the repository:

    git clone https://github.com/musa-z/warehouse_robot_management_system.git
    cd warehouse_robot_management_system

Open the project in Visual Studio from the code folder.

Set up the database:

    Use the SQL/database file in the database folder to create or load the required tables.
    Place Robots.db in the same folder as the executable if required by the application.

Run the application:

    Build and run the Windows Forms project in Visual Studio.

## Example Use Cases

This project is relevant to systems involving:

- Warehouse robot job assignment
- Fleet/task management interfaces
- Local database-backed engineering tools
- Robot scheduling simulations
- Manufacturing or logistics automation software

## Limitations and Future Improvements

The system simulates warehouse robot task allocation and does not connect to physical robots.

Future improvements would include adding live robot communication, improving route planning, adding map-based visualisation, supporting real-time robot telemetry, improving the task allocation algorithm and adding a web-based dashboard.
