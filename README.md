# UniversalDbUpdater

This project implements ideas, how I want to handle database updates.
I recommend using this tool for uncritical private projects, since it's far from ready for productiv use.

Also the versioning is a bit messed up, since I tried a Continious Deployment approach.

## Idea

The idea behind this tool is, to provide a valid database to each git commit.
Therefore all SQL scripts are versioned together with the source code.

All scripts are build on top of each other, e.g.:
* the 1. script starts with a blank database and creates a user table
* the 2. script adds a phonenumber column to the user table
* the 3. script creates a new table products
* and so on

Executed scripts are tracked by the database.
Use the `init` command to create the needed table.
Each script has to add itself to this table.
Use the `create` command to create to a script file with the needed INSERT statement.

**Pay Attention:** The filename of script and the filename inserted into the database have to be identical!
If this is not the case, the script will be executed multiple times.

**Pay Attention:** Do not modify scripts after they're pushed to your repository.
Your CI-System or another user could already have excuted this script.
Create a new script instead.

## MSSQL

Since the SqlCommand class can only execute single statements and 
I'm not aware of a way to execute whole SQL scripts with dotnet core, 
I'm splitting the statements of a script at the `GO` statement.

Make sure to provide a `GO` statement after each statement.
See [this test resource](https://github.com/prayzzz/UniversalDbUpdater/blob/master/test/UniversalDbUpdater.Test/MsSql/Resources/2016-10-01_18-00-00_Script01.sql) as reference.

## Commands

```

 universaldbupdater common-command [options]
 universaldbupdater database-command -t type [options]

Common Commands:
 h help             Shows this help
 
Database Commands:
 b backup           Creates a backup of the database
 c create           Creates a new script file
 e execute          Executes missing scripts
 i init             First time initialization
 s show             Shows scripts missing in database

Options:
 --backup        Backup Directory (Default: ./backup/ )
 --database      Database name
 --host          Host address / ip 
 --isecurity     Integrated Security (for mssql) 
 --port          Host port
 --password      Password
 --schema        Schema for script table (Default: infrastructure)
 --scripts       Scripts Directory (Default: ./ )
 --table         Tablename for storing executed scripts (Default: dbscripts)
 --type          Database type: mssql, mysql
 --user          Username

```

NuGet Icon by PICOL (https://www.iconfinder.com/iconsets/picol-vector)