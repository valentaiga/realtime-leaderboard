# To apply migrations

Add new  
`dotnet ef migrations add Create_MatchTable --context ChronicleDbContext --project BE/BackOffice/BackOffice.Chronicle.Migrations/ --output-dir Migrations`

Remove last one  
`dotnet ef migrations remove --context ChronicleDbContext --project BE/BackOffice/BackOffice.Chronicle.Migrations/`