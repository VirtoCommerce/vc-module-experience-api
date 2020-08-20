# X-Profile

## Key features
- CRUD operations with **users**
- CRUD operations with **organizations**
- CRUD operations with **contacts**

## Structure
![Profile schema type](X-Profile-Type-Schema.jpg)

## Endpoints
### Queries
|#|Endpoint|Arguments|Return|
|-|-|-|-|
|1|[organization](#organization-query)|`id`|Organization|
|2|[contact](#contact-query)|`id`|Contact|
|3|[role](#role-query)|`roleName`|Role|
|4|[user](#user-query)|`id` `userName` `email` `loginProvider` `providerKey`|User|

### Mutations
|#|Endpoint|Arguments|Description|
|-|-|-|-|
|1|[createContact](#createcontact-mutation)|`id` `name!` `memberType` `addresses` `phones` `emails` `groups` `fullName` `firstName!` `lastName!` `middleName` `salutation` `photoUrl` `timeZone` `defaultLanguage` `organizations`|Creates contact|
|2|[updateContact](#updatecontact-mutation)|`id!` `name` `memberType` `addresses` `phones` `emails` `groups` `fullName` `firstName!` `lastName!` `middleName` `salutation` `photoUrl` `timeZone` `defaultLanguage` `organizations`|Updates contact|
|3|[deleteContact](#deletecontact-mutation)|`contactId!`|Deletes contact|
|4|[createUser](#createuser-mutation)|`id` `email` `createdBy` `createdDate` `isAdministrator` `lockoutEnabled` `lockoutEnd` `logins` `memberId` `password` `phoneNumber` `phoneContactConfirmed` `photoUrl` `roles` `storeId` `twoFactorEnabled` `userName` `userType`|Creates user|
|5|[updateUser](#updateuser-mutation)|`accessFailedCount` `email!` `id!` `isAdministrator` `lockoutEnabled` `lockoutEnd` `memberId` `phoneNumber` `phoneNumberConfirmed` `photoUrl` `roles` `storeId` `twoFactorEnabled` `userName!` `userType!` `passwordHash` `securityStamp!`|Updates user|
|6|[deleteUsers](#deleteusers-mutation)|`userNames!`|Delete users|
|7|[updateAddresses](#updateaddresses-mutation)|`contactId!` `addresses!`|Update addresses|
|8|[createOrganization](#createorganization-mutation)|`id` `name` `memberType` `addresses` `phones` `emails` `groups`|Creates organization|
|9|[updateOrganization](#updateorganization-mutation)|`id!` `name` `memberType` `addresses` `phones` `emails` `groups`|Updates organization|
|10|[updateRole](#updaterole-mutation)|`concurrencyStamp` `id!` `name!` `description` `permissions!`|Updates role|

## Examples
In this block you can find some examples of queries and mutations.

### Organization query
With this query you can get the organization by id
```
{
  organization(id: "689a72757c754bef97cde51afc663430") {
    id
    name
    ownerId
    parentId
    businessCategory
    addresses {
      addressType
    }
  }
}
```
### Contact query
With this query you can get the contact by id
```
{
  contact(id: "5f807280-bb1a-42b2-9a96-ed107269ea06") {
    id
    fullName
    memberType
    name
    organizationId
    organizations {
      name
    }
  }
}
```
### Role query
With this query you can get the role by name
```
{
  role(roleName: "Store administrator") {
    id
    name
    permissions
  }
}
```
### User query
With this query you can get the user by few arguments
```
{
  user(id: "9b605a3096ba4cc8bc0b8d80c397c59f") {
    accessFailedCount
    contact {
      id
      name
    }
    createdDate
    email
    isAdministrator
    passwordHash
  }
}
```
### CreateContact mutation
#### Query
```
mutation($command: InputCreateContactType!) {
  createContact(command: $command) {
    id
    name
    firstName
    lastName
  }
}
```
#### Variables
```
{
  "command": {
  "firstName": "testGraphQlFirstName",
  "lastName": "testGraphQlLastName",
  "name": "testGraphQlName"
	}
}
```
### UpdateContact mutation
#### Query
```
mutation($command: InputUpdateContactType!){
  updateContact(command: $command) {
    id
    name
  }
}
```
#### Variables
```
{
  "command": {
    "id": "550e9b14-ddde-46fe-bc28-0afec83ade96",
    "firstName": "testGraphQlFirstName2",
    "lastName": "testGraphQlLastName2"
	}
}
```
### DeleteContact mutation
#### Query
```
mutation($command: InputDeleteContactType!){
  deleteContact(command: $command)
}
```
#### Variables
```
{
  "command": {
    "contactId": "550e9b14-ddde-46fe-bc28-0afec83ade96"
  }
}
```
### CreateUser mutation
#### Query
```
mutation($command: InputCreateUserType!) {
  createUser(command: $command) {
    succeeded
  }
}
```
#### Variables
```
{
  "command": {
    "email": "graphql@test.local",
    "userName": "graphqlTestUserName",
    "userType": "Customer"
  }
}
```
### UpdateUser mutation
#### Query
```
mutation($command: InputUpdateUserType!) {
  updateUser(command: $command) {
    succeeded
    errors{
      code
      description
    }
  }
}
```
#### Variables
```
{
  "command": {
    "id": "ae6f1cd7-957d-4b30-864c-8f40232a4df3",
    "userName": "graphqlTestUserName2",
    "userType": "Manager",
    "securityStamp": "",
    "email": "graphql2@test.local"
  }
}
```
> SecurityStamp - A random value that must change whenever a users credentials change (password changed, login removed)
### DeleteUsers mutation
#### Query
```
mutation($command: InputDeleteUserType!) {
  deleteUsers(command: $command) {
    succeeded
    errors{
      code
      description
    }
  }
}
```
#### Variables
```
{
  "command": {
    "userNames": ["graphqlTestUserName2"]
  }
}
```
### UpdateAddresses mutation
#### Query
```
mutation($command: InputUpdateContactAddressType!) {
  updateAddresses(command: $command) {
    addresses {
      addressType
    }
  }
}
```
#### Variables
```
{
  "command": {
    "contactId": "820c58c5-b518-454b-aefd-2fc4616bd25e",
    "addresses": [
      {
        "countryCode": "testCountryCode",
        "countryName": "testCountryName",
        "line1": "testLine1",
        "postalCode": "testPostalCode",
        "city": "testCity",
        "addressType": 3 
      }
    ]
  }
}
```
> Address type: **1** - Billing, **2** - Shipping, **3** - BillingAndShipping
### CreateOrganization mutation
#### Query
```
mutation($command: InputCreateOrganizationType!) {
  createOrganization(command: $command) {
    id
    name
    memberType
  }
}
```
#### Variables
```
{
  "command": {
    "name": "testOrganizationName",
    "emails": ["testOrg.graphql.local"]
  }
}
```
### UpdateOrganization mutation
#### Query
```
mutation($command: InputUpdateOrganizationType!) {
  updateOrganization(command: $command) {
    id
    name
    memberType
  }
}
```
#### Variables
```
{
  "command": {
    "id": "5385b5b7-1772-4c08-8596-27503b8fdddd",
    "name": "EditedTestOrganization",
    "emails": ["test@graphql.local2"]
  }
}
```
### UpdateRole mutation
#### Query
```
mutation($command: InputUpdateRoleType!) {
  updateRole(command: $command) {
    succeeded
    errors {
      code
      description
    }
  }
}
```
#### Variables
```
{
  "command": {
    "id": "e75700bb597948cca7962e0bbcfdb97c",
    "name": "Use api",
    "permissions": [
      {
      	"name": "security:call_api"
      },
      {
        "name": "catalog:create"
      }
    ],
    "concurrencyStamp": ""
  }
}
```
> ConcurrencyStamp - A random value that should change whenever a role is persisted to the store