# UnitySyncAPI
A lightweight prototype of a Unity3D networking system using a PHP API and database for client-side data synchronization.

This project is an **experimental prototype** created over 3–4 days to explore simple networking concepts for multiplayer games.
It is **not a production-ready solution**, may contain bugs, and is **not the most optimized approach** to real-time data synchronization.

# Overview
The system consists of three main parts:
1. **Unity3D Client** – a prototype game that communicates with a PHP backend through an API.
    - Includes classes responsible for synchronizing data between clients via the server.
    - Each client sends its own state to the API, and other clients update their data accordingly.
    - The logic is fully client-sided, intended only as a conceptual test.
2. **API** – provides endpoints for managing player data, game sessions, and synchronization.
3. **Server Manager** – a lightweight background script intended to run on the server.
    - Periodically checks for inactive servers or players who stopped sending heartbeats.
    - Cleans up or resets empty sessions automatically.

# Features
  - Basic client–server synchronization using HTTP requests
  - Simple player state management via PHP API
  - Prototype server manager for heartbeat monitoring
  - Unity-side classes for sending and receiving synchronization data
  - PHP + MySQL (MariaDB) backend

# Notes
This is a prototype – built for testing ideas, not for production use.
Certain features required for a complete multiplayer game (e.g. authentication, security, authoritative server logic) are not implemented.
The project may contain inefficient or temporary solutions, as the goal was to create a working proof of concept in a short time.

# Screenshots

# Technologies Used
- Unity3D (C#)
- PHP 8+
- MySQL / MariaDB
- HTTP-based client-server communication
