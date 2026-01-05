INSERT INTO public."Users" ("Id", "FullName", "Email", "PasswordHash", "Role", "CreatedAt")
VALUES (
    '00000000-0000-0000-0000-000000000000', 
    'System Admin', 
    'admin@example.com', 
    'default_hash', 
    'Admin', 
    NOW()
)
ON CONFLICT ("Id") DO NOTHING;