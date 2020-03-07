
# Foundry Groups

API for hierarchical group and user organization

*seed data example:*

Accounts

    [
      {
        "Name": "Administrator",
        "Id": "9fd3c38e-58b0-4af1-80d1-1895af91f1f9",
        "IsAdministrator": true
      },
      {
        "Name": "Larry",
        "Id": "73670ED1-D905-4D19-9FE3-053A4297D4A3",
        "IsAdministrator": false
      }
    ]

**Groups**

    [
      {
        "Name": "New Group",
        "Description": "New Group",
        "Summary": "New Group",
        "LogoUrl": "",
        "Members": [
          {
            "AccountId": "9fd3c38e-58b0-4af1-80d1-1895af91f1f9",
            "IsOwner": true,
            "IsManager": true
          },
          {
            "AccountId": "73670ED1-D905-4D19-9FE3-053A4297D4A3",
            "IsOwner": false,
            "IsManager": false
          }
        ]
      }
    ]

