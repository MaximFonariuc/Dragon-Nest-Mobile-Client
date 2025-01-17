# Dragon-Nest-Mobile-Client

This project is a Unity-based application originally developed in Unity 5.5.2f1 and currently upgraded to Unity 2022.3.55f1. Below are the steps to set up and run the project.

---

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Setup Instructions](#setup-instructions)
3. [Running the Project](#running-the-project)
4. [Server Setup](#server-setup)
5. [Screenshots](#screenshots)
6. [Repository Links](#repository-links)

---

## Prerequisites
- **Unity 2022.3.55f1**: Download and install from [Unity Hub](https://unity.com/download).
- **MySQL 8.0 Server**: Ensure it is properly configured.
- **Git**: For cloning the repository.

---

## Setup Instructions

1. **Clone the Repository**  
   Clone this repository to your local machine using the following command:
   ```bash
   git clone <repository-url>
2. **Extract Heavy Files**
   Before opening the project, navigate to the root directory and run LFSUtility.bat. This utility will unpack(DECOMPRESS) large files that are stored using a space-efficient mechanism.
3.Open the Project in Unity
   Launch Unity Hub and open the project using Unity 2022.3.55f1.

## Running the Project

1. **Start the Scene**  
   After opening the project, navigate to the DN_Updater scene. This is the starting point for the application.
2. **Server Connection**
   If your servers are correctly set up, the application will transition to the login scene. Ensure that all required MySQL databases are properly configured.

## Server Setup
   All necessary server files and documentation are available in the Server [Repository](https://github.com/MaximFonariuc/Dragon-Nest-Mobile-Server). Follow the instructions provided there to set up the required MySQL databases and server configurations.

## Screenshots
   Here are some screenshots to guide you through the setup and running process:
   
1. **LFSUtility Execution**
   LFSUtility
2. **DN_Updater Scene**
   DN_Updater Scene
3. **Login Scene**
   Login Scene

## Repository Links
   Client Repository: Link to this [repository](https://github.com/MaximFonariuc/Dragon-Nest-Mobile-Client)
   Server Repository: Link to the [server repository](https://github.com/MaximFonariuc/Dragon-Nest-Mobile-Server)

## Notes
   Ensure all dependencies are correctly installed before running the project.
   If you encounter any issues, refer to the server repository for additional documentation.
