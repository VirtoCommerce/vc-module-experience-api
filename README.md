---
# 🔥 Important Notice: Migration to New xAPI Modules
We have migrated to a new xAPI architecture to better support the evolving needs of our business API with GraphQL. The VirtoCommerce.ExperienceApi module has been replaced with a suite of new, more specialized modules. This change is part of our effort to simplify business API development and streamline our release cycle.

## 🎬 Action Required
Please transition from the legacy VirtoCommerce.ExperienceApi module to the new modules listed below:

* [VirtoCommerce.xApi](https://github.com/VirtoCommerce/vc-module-x-api): Core business API module.
* [VirtoCommerce.xCart](https://github.com/VirtoCommerce/vc-module-x-cart): Handles cart-related functionalities.
* [VirtoCommerce.xCatalog](https://github.com/VirtoCommerce/vc-module-x-catalog): Manages catalog-related operations.
* [VirtoCommerce.xCMS](https://github.com/VirtoCommerce/vc-module-x-cms): Content management system integration.
* [VirtoCommerce.xOrder](https://github.com/VirtoCommerce/vc-module-x-order): Manages order processing.

Please refer to the following update path instructions for more detailed guidance on updating the new modules.

## Breaking Changes
### 👌 Frontend
* **GraphQL Schema Compatibility**: All GraphQL schemas remain compatible, so no frontend modifications are required directly due to schema changes. *(Note: deprecated mutation `valdateCoupon` was removed. Use `validateCoupon` query instead)*.
* **API Endpoint Changes**: If your frontend directly calls endpoints provided by VirtoCommerce.ExperienceApi, verify and update the endpoint URLs to match the new module structure if necessary.
* **Testing**: Thoroughly test frontend interactions to ensure smooth functionality with the new backend modules.

### 🔥 Custom Modules
* **Dependency Changes**: Custom modules that depended on VirtoCommerce.ExperienceApi will need to be updated to depend on the appropriate new modules (VirtoCommerce.Xapi, VirtoCommerce.XCart, VirtoCommerce.XCatalog, VirtoCommerce.XCMS, VirtoCommerce.XOrder).
* **Uninstall Old Packages**: Ensure to uninstall the NuGet packages from VirtoCommerce.ExperienceApi and replace them with the new packages.
Code Adjustments: Review and adjust your code to align with the new module structures and namespaces.

## Update Path
To transition to the new modules, follow these steps:

1. Uninstall VirtoCommerce.ExperienceApi.
2. Install the new modules:
  - VirtoCommerce.Xapi
  - VirtoCommerce.XCart
  - VirtoCommerce.XCatalog
  - VirtoCommerce.XCMS
  - VirtoCommerce.XOrder

3. Update other modules to the new version if required, ensuring they now depend on VirtoCommerce.Xapi:
  - VirtoCommerce.MarketingExperienceApi
  - VirtoCommerce.Quote
  - VirtoCommerce.CustomerReviews
  - VirtoCommerce.Skyflow
  - VirtoCommerce.TaskManagement
  - VirtoCommerce.FileExperienceApi
  - VirtoCommerce.WhiteLabeling

4. For any custom modules, uninstall the NuGet packages from VirtoCommerce.ExperienceApi and replace them with the new ones.
5. Models, service interfaces, GraphQL schema types and input types, commands, queries and aggregates are moved to respective Core projects of the new modules (XCatalog.Core. XCart.Core, etc) with namespaces adjusted. Data projects contain service implementations, command and query builders, command and query handlers, and middleware.
6. Schema: validateCoupon command (was marked as Deprecated) was removed, use validateCoupon query.

## 💕 Update and Support
VirtoCommerce.ExperienceApi is archived and will be supported in Stable 8 and Stable 9 releases. Future developments will focus on the new VirtoCommerce.Xapi and related modules. The latest Edge release has adopted the new modules.

---

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

## Documentation
* [Experience API Documentation](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/)
* [Getting started](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/getting-started/)
* [How to use GraphiQL](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/graphiql/)
* [How to use Postman](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/postman/)
* [How to extend](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/x-api-extensions/)
* [Virto Commerce Frontend architecture](https://docs.virtocommerce.org/storefront/developer-guide/architecture/)
* [View on GitHub](https://github.com/VirtoCommerce/vc-module-experience-api)


## References

* [Deployment](https://docs.virtocommerce.org/platform/developer-guide/Tutorials-and-How-tos/Tutorials/deploy-module-from-source-code/)
* [Installation](https://docs.virtocommerce.org/platform/user-guide/modules-installation/)
* [Home](https://virtocommerce.com)
* [Community](https://www.virtocommerce.org)
* [Download latest release](https://github.com/VirtoCommerce/vc-module-experience-api/releases/latest)
  

## License
Copyright (c) Virto Solutions LTD.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
