# Overview

[![CI status](https://github.com/VirtoCommerce/vc-module-experience-api/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-experience-api/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-experience-api&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-experience-api) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-experience-api&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-experience-api) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-experience-api&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-experience-api) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-experience-api&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-experience-api)

The project "Experience API" it is primarily a intermediated layer between clients and enterprise  services powered by GraphQL protocol and is tightly coupled to a specific user/touchpoint  experience with fast and reliable access, it represents an implementation of Backend for Frontend design pattern (BFF).

**The context diagram:**
![image](https://user-images.githubusercontent.com/7566324/84039908-38258300-a9a2-11ea-9421-2c51462d69af.png)

## Key concepts
- Use GraphQL protocol to leverage more selective and flexible control of resulting data retrieving from API;
- Fast and reliable indexed search thanks to integration with ES 7.x  and single data source for indexed search and data storage (<= 300ms);
- Autonomy. Shared nothing with rest VC data infrastructure except index data source;
- Tracing and performance requests metrics.

## Key features
- [X-Catalog docs](./docs/x-catalog-reference.md)
- [X-Purchase cart docs](./docs/x-purchase-cart-reference.md)
- [X-Purchase order docs](./docs/x-purchase-order-reference.md)
- [X-UserProfile module](https://github.com/VirtoCommerce/vc-module-profile-experience-api) (moved to a separate module)
- [Recommendations Gateway API](./docs/gateway-api-reference.md) (prototype)

## How to use
### Playground IDE
To explore the GraphQL API, you can use an interactive  [graphql-playground](https://github.com/prisma-labs/graphql-playground) environment.
To open playground console open  `ui/playground` in the platform manager application.
```
http://localhost:10645/ui/playground
```
### Curl
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
## Getting started
Read this [article...](docs/getting-started.md)

## How to extend
Read this [article...](docs/x-api-extensions.md)

## Where to find logs
Read this [article...](docs/application-insights-integration.md)

## Limitation

The project has integration with Elastic Search 7.x and Azure Search Service providers for indexing search.

Lucene search provider not supported.
## Documentation

[Experience API Module Document](/docs/index.md)

[View on GitHub](https://github.com/VirtoCommerce/vc-module-experience-api)

## References
* Deploy: https://virtocommerce.com/docs/latest/developer-guide/deploy-module-from-source-code/
* Installation:https://www.virtocommerce.com/docs/latest/user-guide/modules/
* Home: https://virtocommerce.com
* Community: https://www.virtocommerce.org
* [Download Latest Release](https://github.com/VirtoCommerce/vc-module-experience-api/releases)

## License
Copyright (c) Virto Solutions LTD.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
