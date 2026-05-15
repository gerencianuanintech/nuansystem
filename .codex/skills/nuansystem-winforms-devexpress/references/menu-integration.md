# Menu Integration Checklist

Use this when a new WinForms screen must appear in the main menu.

## Ask First

Before editing files, confirm missing values with the user:

- Destination folder/module name.
- Parent menu/category.
- Visible menu label.
- `FormKey`.
- Display order.
- Operations to enable.
- Whether default admin access should be seeded.

## Frontend

- Add the form under `src/Frontend/NuanSystem.WinForms.Forms/{Folder}`.
- Add view model and service files under matching module folders when needed.
- Register factories and service clients in `src/Frontend/NuanSystem.WinForms/Program.cs`.
- Ensure `MainForm` can open the new `FormKey` if it uses explicit form factories.
- Configure grid column settings with the same `FormKey` when using grid customization.

## Security/Menu SQL

- Insert or update `SecurityForms` with `Code`, `Name`, `Description`, `FormKey`, `FormType`, `IsVisible`, and `IsActive`.
- Insert or update `SecurityMenus` with `ParentId`, `Code`, `Name`, `Description`, `MenuType`, `FormId`, `FormKey`, icons, `DisplayOrder`, `IsVisible`, and `IsActive`.
- Seed `SecurityRoleMenus` for the admin role when the user wants default access.
- Seed form operations when the screen uses operation-level permissions.
- Keep menu codes in the form `MENU.{PARENT}.{ITEM}` and use kebab-case `FormKey`.

## Validation

- Confirm the menu loads through `/api/security/navigation`.
- Confirm operation access loads for the same `FormKey`.
- Build the frontend project after registering factories or constructor dependencies.
