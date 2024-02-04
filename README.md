
# Softwareentwiklungsprojeckt:  Verleihplattform für KI-Werkstatt (HTW)
* Teammitglieder:
	1. <Name1>Ahmed Hani Abdulatif Kutbi
	2. <Name2>Heltonn Harold Ngalemo Tchaleu
	3. <Name3>Maxime Fopossi Kemegni

* Team: <Teamnummer>Gruppe 07
* Semester: WS 2022/2023


## Ausgangspunkt
Eine Webanwendung soll erstellt werden, wo Studenten und Professoren Hardware und Laptops für eine bestimmste Zeit buchen können. Authenfizierung ist durch den LDAP Protokolle. Die Webseite könnte nur in der HTW-Umgebung benutzt werden.


## Tools und Anwendungen
Hier sind die Tools und Anwendungen, die man benötigt, um die Webanwendung nutzen zu können:

1. Microsoft SQL Server Management Studio 2022
2. Visual Studio 2022
3. Cisco AnyConnect Secure Mobility Client VPN - HTW Umgebung

## Technologien und Frameworks
Eine Liste der im Projekt verwendeten Technologien und Frameworks:

1. ASP.NET Core MVC: Version 6.0
2. Entity Framework Core: Version 6.0


## Steps to run
1. Update the server name in the connection string in appsettings.json in Bookingplatform.
2. Open the Package Manager Console (Menu-bar\Tools\NuGet Package Manager\Package Manager Console)
3. Write update-database and click enter
4. Rebuild the whole solution.
5. Run the program.

# Hinweis:
Login: Um erfolgreich einzulogin, soll man ein HTW Konto haben und eine VPN-Verbindung mit dem HTW Server erstellen. 


## NuGet Packages
1. ASP.NET Core MVC
2. Microsoft.EntityFrameworkCore.SqlServer
3. EntityFrameworkCoreTools
4. Microsoft.EntityFrameworkCore
5. Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
6. Microsoft.VisualStudio.Web.CodeGeneration.Design
7. System.DirectoryServices.Protocols


## Unit Testing

1. FakeItEasy version 7.3.1
2. FluentAssertions version 6.8.0
3. XUnit version 2.4.1

The Project was tested with a pair of some functional tests and all tests went to be succesfull

## Doxygen Documentation
The Code was documentated using Doxygen.
Documentation of the Project Source Code and the Documentation of the ProjectsTest will be available in the Project Doxygen File. 
