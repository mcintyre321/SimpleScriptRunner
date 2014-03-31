Available on nuget at http://nuget.org/packages/SimpleScriptRunner/


This tool can be used to run a directory of numbered sql scripts against an MSSQL database. The tool can easily be extended to run against other versioned objects.

This is very handy if you are maintaining multiple database environments (e.g. dev, test, production) or working in a team.

The usage for is
```
SimpleScriptRunner.exe <serverName> <databaseName> <path to folder containing sql scripts> [options]
```
or for SQL Authentication
```
SimpleScriptRunner.exe <serverName> <username> <password> <databaseName> <path to folder containing sql scripts> [options]
```

the sql scripts should start with the script version number and then a space, and be grouped into numbered Release folders.

e.g.
```
Release 1\0001 - Create Users table.sql
Release 1\0002 - Create Customers table.sql
Release 2\0001 - Create Items table.sql
```

```[options]``` can be any combination of the following switches

||-usetransactions||Each numbered migration will take place in a new transaction, and rollback if there are any issues||
||-requirerollback||Each script x.sql must have a corresponding x.rollback.sql. When the script is run, the rollup is executed, then the rollback, then the rollup again, to confirm the rollback doesn't cause errors||
||-requireidempotency||Each script will be run twice, to confirm idempotency||

*What it does*

The tool will only execute scripts with higher numbers than have been previously executed - i.e. if the last script to be applied was script 0003, scripts 0004 and onwards will be applied - with one exception. If the last script to be applied has been modified since it was last executed, it will be executed again. This is handy if you are working on a script and need to make slight changes to it. The tool will also create the database on the server if it does not exist.

*Getting started*
Assuming your solution file is called MySolution and that you are running a local copy of SQL Express, add a project called 'MySolution.Database' or something similar to Visual Studio, then add the following line to the project post build command line
```
SimpleScriptRunner.exe .\sqlexpress $(SolutionName)Db_$(ConfigurationName) $(ProjectDir)
```
Now add a script called 0001 - hello world.sql to the project root. When you build, a database called MySolutionDb_Debug will be created on your local sql instance. Done!
