Open Package Manager Console
Select the Reacher.Data project
Add model changes to db migration script:
	Add-Migration [Name_without_spaces]
Apply changes to database
	Update-Database
Rollback database to previous
	Update-Database [Migration_name]
Remove previous migration (code)
	Remove-Migration

Generate SQL Script
	Script-Migration -Idempotent