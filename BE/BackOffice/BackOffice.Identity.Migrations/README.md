# To apply migrations

Add new  
`dotnet ef migrations add Create_MatchTable --context IdentityDbContext --project BE/BackOffice/BackOffice.Identity.Migrations/ --output-dir Migrations`

Remove last one  
`dotnet ef migrations remove --context IdentityDbContext --project BE/BackOffice/BackOffice.Identity.Migrations/`