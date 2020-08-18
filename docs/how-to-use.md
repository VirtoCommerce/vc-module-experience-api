# How to use
## Playground IDE
To explore the GraphQL API, you can use an interactive  [graphql-playground](https://github.com/prisma-labs/graphql-playground) environment.
To open playground console open  `ui/playground` in the platform manager application.
```
http://localhost:10645/ui/playground
```
## Curl
```curl
POST https://{platform-url}/graphql
```
It accepts POST requests with following fields in a JSON body:
- `query` - String - GraphQL query as a string
- `variables` - Object - Optional - containing JSON object that defines variables for your query
- `operationName` - String - Optional - the name of the operation, in case you defined several of them in the query

Here is an example of a GraphQL query:
```curl
$ curl -X POST http://localhost:10645/graphql \
  -H "Content-Type:application/json" \
  -H "Authorization:Bearer ..." \
  -d '{"operationName":null,"variables":{},"query":"{ product(id: \"019e93d973cd4adab99b6f9cbb4ca97a\") { name }}"}'
```
