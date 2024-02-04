
# Softwareentwiklungsprojeckt:  Verleihplattform für KI-Werkstatt (HTW)
* Teammitglieder:
	1. <Name1>Ahmed Hani Abdulatif Kutbi
	2. <Name2>Heltonn Harold Ngalemo Tchaleu
	3. <Name3>Maxime Fopossi Kemegni

* Team: <Teamnummer>Gruppe 07
* Semester: WS 2022/2023

## Ausgangspunkt
Eine Webanwendung soll erstellt werden, wo Users Hardwares(Resourcen) für eine bestimmste Zeit buchen können. Authenfizierung ist durch den LDAP Protokolle.

## Visual Studio 2022 and SQL Server

 Hier sind die Tools, die man benötigt, um die Webanwendung richten nutzen zu können:
1. Microsoft SQL Server Management Studio 2022
2. Visual StudioVersion 2022
3. Cisco AnyConnect Secure Mobility Client

## Steps to run

1. Update the server name in the connection string in appsettings.json in Bookingplatform.
2. Open the Package Manager Console (Menu-bar\Tools\NuGet Package Manager\Package Manager Console)
3. Write update-database and click enter
4. Rebuild the whole solution.
5. Run the program.

Hinweis:-
Login: Um Login Form zu benutzen soll man eine VPN-Verbindung mit HTW-Vpn haben. 

## Technologies and frameworks used:
***
A list of technologies used within the project:

1. ASP.NET Core MVC: Version 6.0
2. Entity Framework Version 6.0

## NuGet Packages
1. ASP.NET Core MVC
2. Microsoft.EntityFrameworkCore.SqlServer
3. EntityFrameworkCoreTools
4. Microsoft.EntityFrameworkCore
5. Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
6. Microsoft.VisualStudio.Web.CodeGeneration.Design
7. System.DirectoryServices.Protocols

## Unit Testing
***
A list of technologies used within the project:
1. FakeItEasy version 7.3.1
2. FluentAssertions version 6.8.0
3. XUnit version 2.4.1
4. EntityFramework version 6.4.4

The Project was tested with a pair of some functional tests and all tests went to be succesfull

## Doxygen Documentation
The Code was documentated using Doxygen.
Documentation of the Project Source Code and the Documentation of the ProjectsTest will be available in the Project Doxygen File. 

## Links, Hinweise etc.
0. https://gitlab.rz.htw-berlin.de/-/ide/project/softwareentwicklungsprojekt/wise2022-23/team7
